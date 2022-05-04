
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Real.Data.Contexts;
using Real.Model;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Real.Web.Filters {

    public static class DbLoggingFilterExtensions {
        public static MvcOptions UseDbLogging(this MvcOptions builder) {
            builder.Filters.Add(new DbLoggingFilter());
            return builder;
        }
    }

    public class DbLoggingFilter : /*IActionFilter,*/ IAsyncActionFilter {

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {

            // prevent logging AWS health check access
            if (context.HttpContext.Request.Path.ToString() == "/") {
                await next();
                return;
            }

            var firebaseUserId = (context.Controller as Controller).GetUserFirebaseId();
            var ip = context.HttpContext.Connection.RemoteIpAddress;

            if (context.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwardedFor)) {
                if (!IPAddress.TryParse(forwardedFor.ToString(), out ip))
                    IPAddress.TryParse(forwardedFor.ToString().Split(',').FirstOrDefault(), out ip);
            }
            if (context.HttpContext.Request.Headers.TryGetValue("UID", out StringValues uid))
                firebaseUserId = uid.ToString();
        

            var db = context.HttpContext.RequestServices.GetService(typeof(CapstoneContext)) as CapstoneContext;
            var trace = new Analytic {
                StartTimestamp = DateTime.UtcNow,
                Namespace = context.Controller.GetType().Namespace,
                Area = context.ActionDescriptor.RouteValues["area"] ?? "",
                Controller = context.ActionDescriptor.RouteValues["controller"],
                Action = context.ActionDescriptor.RouteValues["action"],
                UserName = context.HttpContext.User.Identity.Name,
                FirebaseUserId = firebaseUserId,
                IPv4 = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                IPv6 = context.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString(),
                Host = context.HttpContext.Request.Host.ToString(),
                Path = context.HttpContext.Request.Path.ToString(),
                QueryString = context.HttpContext.Request.QueryString.ToString(),
            };

            if (!String.IsNullOrEmpty(forwardedFor)) {
                trace.AnalyticDetail = new AnalyticDetail {
                    AnalyticId = trace.Id,
                    Message = $"X-Forwarded-For={forwardedFor.ToString()}",
                };
            }

            await next();
            
            trace.EndTimestamp = DateTime.UtcNow;

            db.Analytics.Add(trace);

            var updating = db.ChangeTracker
                .Entries()
                .Where(x => x.State != Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                .ToList();

            int rows = db.SaveChanges();
        }

    }
}