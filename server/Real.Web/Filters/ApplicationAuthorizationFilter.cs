
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Real.Data.Contexts;
using System.Text;
using System.Net.Sockets;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using Real.Web.Areas.API.Controllers;

namespace Real.Web.Filters {
    public class ApplicationAuthorizationFilter : ActionFilterAttribute {
        private readonly IConfiguration _Configuration;
        private readonly ILogger<ApplicationAuthorizationFilter> _Logger;
        private readonly CapstoneContext _context;
        private readonly EncryptionController _encryptionController;
        private readonly List<string> _IpSafeList;

        public ApplicationAuthorizationFilter(ILogger<ApplicationAuthorizationFilter> logger, IConfiguration configuration, CapstoneContext context, EncryptionController encryptionController) {
            _Logger = logger;
            _Configuration = configuration;
            _context = context;
            _encryptionController = encryptionController;
            _IpSafeList = _Configuration.GetSection("AdminSafelist").Get<string[]>().ToList();
        }

        // For the love of FUCK do not use await in this method.
        // I spent hours trying to identify an issue. It seems using await breaks the MVC
        // pipeline, rendering the page BEFORE sending a 403 error, which opens any page
        // to the world.
        #pragma warning disable 1998

        public override async void OnActionExecuting(ActionExecutingContext context) {
            var isAllowed = false;
            var forwardedFor = String.Empty;

            isAllowed = 
                _AllowAnonymous(context) ||
                _VerifyIpSafeList(context, out forwardedFor) ||
                _VeryifyEncryption(context)   
            ;

            if (isAllowed) {
                base.OnActionExecuting(context);
                return;
            }
            
            _Logger.LogError("Not allowed");

            try {
                var a = new Model.Analytic {
                    Namespace = null,
                    Area = null,
                    Controller = null,
                    Action = null,
                    UserName = context.HttpContext.User?.Identity?.Name,
                    FirebaseUserId = (context.Controller as Controller)?.GetUserFirebaseId(),
                    IPv4 = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    IPv6 = context.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString(),
                    Host = context.HttpContext.Request.Host.ToString(),
                    Path = context.HttpContext.Request.Path.ToString(),
                    QueryString = context.HttpContext.Request.QueryString.ToString()
                };
                a.AnalyticError = new Model.AnalyticError {
                    AnalyticId = a.Id,
                    TraceId = context.HttpContext.TraceIdentifier,
                    RequestId = Activity.Current?.Id,
                    Message = $"403 Forbidden {(String.IsNullOrEmpty(forwardedFor) ? "" : $"X-Forwarded-For={forwardedFor.ToString()}")}",
                };

                _context.Analytics.Add(a);
                _context.SaveChanges();
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
            }

            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        #pragma warning restore 1998


        internal bool _AllowAnonymous(ActionExecutingContext context) {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null) {
                var attrs = new List<object>();
                attrs.AddRange(descriptor.MethodInfo.GetCustomAttributes(true));
                attrs.AddRange(descriptor.ControllerTypeInfo.GetCustomAttributes(true));

                if (attrs.Any(x => x is AllowAnonymousAttribute)) {
                    _Logger.LogInformation($"AllowAnonymousAttribute present");
                    return true;
                }
            }

            return false;
        }
        
        internal bool _VerifyIpSafeList(ActionExecutingContext context, out string forwardedFor) {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            StringValues ip = "";
            forwardedFor = null;

            if (context.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out ip)) {
                forwardedFor = ip;
                var addresses = forwardedFor.ToString().Split(',').ToList();
                if (_IpSafeList.Intersect(addresses).Any()) {
                    _Logger.LogInformation($"X-Forwarded-For present in safe list ({String.Join(",", _IpSafeList.Intersect(addresses))})");
                    return true;
                }
            }

            return false;
        }

        internal bool _VeryifyEncryption(ActionExecutingContext context) {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var isAllowed = false;

            if (descriptor != null) {
                var attrs = new List<object>();
                attrs.AddRange(descriptor.MethodInfo.GetCustomAttributes(true));
                attrs.AddRange(descriptor.ControllerTypeInfo.GetCustomAttributes(true));
                var areaAttribute = attrs.SingleOrDefault(x => x is AreaAttribute) as AreaAttribute;

               if (areaAttribute?.RouteValue == "API") {
                    if (context.HttpContext.Request.Headers.TryGetValue("uid", out StringValues uid) && context.HttpContext.Request.Headers.TryGetValue("euid", out StringValues euid)) {
                        var decrypted = _encryptionController.Decrypt(euid).Value;
                        isAllowed = uid.Equals(decrypted);
                        _Logger.LogInformation($"API area, decryption {(isAllowed ? "passed" : "not passed")}");
                    }
                }
            }
            
            return isAllowed;
        }


        

    }
}