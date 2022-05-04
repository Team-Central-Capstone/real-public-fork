
// using System;
// using System.Linq;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Net;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.ApplicationModels;
// using Microsoft.AspNetCore.Mvc.Controllers;
// using Microsoft.AspNetCore.Mvc.Filters;
// using Microsoft.Extensions.Logging;
// using Microsoft.OpenApi.Models;
// using Swashbuckle.AspNetCore.SwaggerGen;
// using Real.Data.Contexts;
// using System.Text;
// using System.Net.Sockets;
// using Microsoft.Extensions.Primitives;
// using Microsoft.Extensions.Configuration;

// namespace Real.Web.Filters {
//     public class AdminSafelistFilter : ActionFilterAttribute {
//         private readonly IConfiguration _Configuration;
//         private readonly ILogger<AdminSafelistFilter> _Logger;
//         private readonly CapstoneContext _context;
//         private readonly List<string> _SafeList;

//         public AdminSafelistFilter(ILogger<AdminSafelistFilter> logger, IConfiguration configuration, CapstoneContext context) {
//             _Logger = logger;
//             _Configuration = configuration;
//             _context = context;

//             _SafeList = _Configuration.GetSection("AdminSafelist").Get<string[]>().ToList();
//         }

//         // For the love of FUCK do not use await in this method.
//         // I spent hours trying to identify an issue. It seems using await breaks the MVC
//         // pipeline, rendering the page BEFORE sending a 403 error, which opens any page
//         // to the world.
//         #pragma warning disable 1998

//         public override async void OnActionExecuting(ActionExecutingContext context) {         
//             var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
//             var isAllowed = false;
//             StringValues forwardedFor = "";

//             if (descriptor != null) {
//                 var attrs = new List<object>();
//                 attrs.AddRange(descriptor.MethodInfo.GetCustomAttributes(true));
//                 attrs.AddRange(descriptor.ControllerTypeInfo.GetCustomAttributes(true));

//                 if (attrs.Any(x => x is AllowAnonymousAttribute)) {
//                     _Logger.LogInformation($"AllowAnonymousAttribute present");
//                     isAllowed = true;
//                 }
//             }
            
//             if (!isAllowed) {
//                 if (context.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out forwardedFor)) {
//                     var addresses = forwardedFor.ToString().Split(',').ToList();
//                     if (_SafeList.Intersect(addresses).Any()) {
//                         _Logger.LogInformation($"X-Forwarded-For present in safe list ({String.Join(",", _SafeList.Intersect(addresses))})");
//                         isAllowed = true;
//                     }
//                 }
//             }

//             if (isAllowed) {
//                 _Logger.LogInformation("Allowed");
//                 base.OnActionExecuting(context);
//                 return;
//             }
            
//             _Logger.LogError("Not allowed");

//             try {
//                 var a = new Model.Analytic {
//                     Namespace = null,
//                     Area = null,
//                     Controller = null,
//                     Action = null,
//                     UserName = context.HttpContext.User?.Identity?.Name,
//                     FirebaseUserId = (context.Controller as Controller)?.GetUserFirebaseId(),
//                     IPv4 = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
//                     IPv6 = context.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString(),
//                     Host = context.HttpContext.Request.Host.ToString(),
//                     Path = context.HttpContext.Request.Path.ToString(),
//                     QueryString = context.HttpContext.Request.QueryString.ToString()
//                 };
//                 a.AnalyticError = new Model.AnalyticError {
//                     AnalyticId = a.Id,
//                     TraceId = context.HttpContext.TraceIdentifier,
//                     RequestId = Activity.Current?.Id,
//                     Message = $"403 Forbidden{(String.IsNullOrEmpty(forwardedFor) ? "" : $"X-Forwarded-For={forwardedFor.ToString()}")}",
//                 };

//                 _context.Analytics.Add(a);
//                 _context.SaveChanges();
//             } catch (Exception ex) {
//                 Console.Error.WriteLine(ex.ToString());
//                 // do nothing
//             }

//             context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
//         }

//         #pragma warning restore 1998
        
//     }
// }