
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
// using Real.Web.Areas.API.Controllers;

// namespace Real.Web.Filters {
//     public class VerifyEncryptionFilter : ActionFilterAttribute {
//         private readonly ILogger<VerifyEncryptionFilter> _Logger;
//         private readonly EncryptionController _encryptionController;

//         public VerifyEncryptionFilter(ILogger<VerifyEncryptionFilter> logger, EncryptionController encryptionController) {
//             _Logger = logger;
//             _encryptionController = encryptionController;
//         }

//         #pragma warning disable 1998

//         public override async void OnActionExecuting(ActionExecutingContext context) {         
//             var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
//             var isAllowed = false;

//             if (descriptor != null) {
//                 var attrs = new List<object>();
//                 attrs.AddRange(descriptor.MethodInfo.GetCustomAttributes(true));
//                 attrs.AddRange(descriptor.ControllerTypeInfo.GetCustomAttributes(true));
//                 var areaAttribute = attrs.SingleOrDefault(x => x is AreaAttribute) as AreaAttribute;

//                 if (attrs.Any(x => x is AllowAnonymousAttribute)) {
//                     _Logger.LogInformation($"AllowAnonymousAttribute present");
//                     isAllowed = true;
//                 } else if (areaAttribute?.RouteValue != "API") {
//                     _Logger.LogInformation($"Not in API area");
//                     isAllowed = true;
//                 } else if (areaAttribute?.RouteValue == "API") {
//                     if (context.HttpContext.Request.Headers.TryGetValue("uid", out StringValues uid) && context.HttpContext.Request.Headers.TryGetValue("euid", out StringValues euid)) {
//                         var decrypted = _encryptionController.Decrypt(euid).Value;
//                         isAllowed = uid.Equals(decrypted);
//                         _Logger.LogInformation($"API area, decryption {(isAllowed ? "passed" : "not passed")}");
//                     }
//                 }
//             }
            
//             if (isAllowed) {
//                 base.OnActionExecuting(context);
//             } else {
//                 context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
//             }
//         }

//         #pragma warning restore 1998
        
//     }
// }