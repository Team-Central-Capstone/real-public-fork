using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Real.Web.Models;
using Real.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Data;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using System.Net.Mail;
using System.Net;
using Real.Web.Areas.API.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json.Linq;

namespace Real.Web.Controllers {

    public static class ControllerExtensions {

        public static bool Between(this DateTime d, DateTime min, DateTime max) {
            return d >= min && d <= max;
        }

        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false) {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using (var writer = new StringWriter()) {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                if (viewResult.Success == false)
                    return $"A view with the name {viewName} could not be found";

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }


    [AllowAnonymous]
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly CapstoneContext _db;
        private readonly Areas.API.Controllers.EmailController _emailController;
        private readonly Areas.API.Controllers.UserMatchesController _matchesController;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, CapstoneContext db, Areas.API.Controllers.EmailController emailController, Areas.API.Controllers.UserMatchesController matchesController) {
            _logger = logger;
            _env = env;
            _db = db;
            _emailController = emailController;
            _matchesController = matchesController;
        }

        public async Task<IActionResult> SeedLocationData() {
            var result = await _db.GenerateSeedLocationDataAsync();

            return Ok(result);
        }

        public async Task<IActionResult> UpdateLocationData() {
            var result = await _db.GenerateSeedLocationDataForExistingUsersAsync();

            return Ok(result);
        }

        public async Task<IActionResult> SeedProfileData() {
            var weights = Enum.GetValues(typeof(Model.UserSurveyResponseWeight));
            var rand = new Random(Guid.NewGuid().GetHashCode());
            
            var users = await _db.Users
                .Include(x => x.UserSurveyResponses)
                .Where(x => x.UserSurveyResponses.Count == 0)
                .Where(x => x.RegisteredTimestamp.Date == new DateTime(2022, 02,24).Date)
                // .Take(10)
                .ToListAsync();

            if (users?.Count == 0)
                return NotFound("No users found");

            var questions = await _db.SurveyQuestions
                .Include(x => x.Answers)
                .ToListAsync();
            
            foreach (var u in users) {
                foreach (var q in questions) {
                    var response = new Model.UserSurveyResponse {
                        UserId = u.Id,
                        SurveyQuestionId = q.Id,
                        UserSurveyResponseWeight = (Model.UserSurveyResponseWeight)weights.GetValue(rand.Next(weights.Length)),
                    };

                    switch (q.QuestionType) {
                        case Model.QuestionType.MultipleChoice:
                            q.Answers.ForEach(a => {
                                if (rand.Next()%2 == 0)
                                    response.SurveyAnswers.Add(a);
                            });
                            if (!response.SurveyAnswers.Any())
                                response.SurveyAnswers.Add(q.Answers.First());
                            break;

                        case Model.QuestionType.SingleChoice:
                            response.SurveyAnswers.Add(q.Answers[rand.Next(q.Answers.Count)]);
                            break;

                        case Model.QuestionType.YesNo:
                            response.SurveyAnswerResponse = rand.Next()%2 == 0 ? "Yes" : "No";
                            break;
                    }

                    u.UserSurveyResponses.Add(response);
                }
            }

            await _db.SaveChangesAsync();

            foreach (var id in users.Select(x => x.Id)) {
                await _db.Database.ExecuteSqlRawAsync("call sp_RecalculateProfileMatch({0})", id);
            }

            return Json(new {
                users
            });
        }

        public async Task<IActionResult> UpdatedNullProfileData() {
            // return RedirectToAction("UpdateLocationData");

                var rand = new Random(Guid.NewGuid().GetHashCode());

                var genders = await _db.UserGenders.ToListAsync();
                var bodytypes = await _db.UserBodyTypes.ToListAsync();
                var users = await _db.Users
                    .Include(x => x.GendersAttractedTo)
                    .ToListAsync();

                users.ForEach(x => {
                    if (x.HeightInches == 0)
                        x.HeightInches = rand.Next(60,84);
                    if (x.UserGenderId == null) {
                        // x.UserGender = genders[rand.Next(genders.Count)];
                        // x.UserGenderId = x.UserGender.Id;
                        x.UserGenderId = genders[rand.Next(genders.Count)].Id;
                    }
                    if (x.UserBodyTypeId == null) {
                        // x.UserBodyType = bodytypes[rand.Next(bodytypes.Count)];
                        // x.UserBodyTypeId = x.UserBodyType.Id;
                        x.UserBodyTypeId = bodytypes[rand.Next(bodytypes.Count)].Id;
                    }
                    if (x.GendersAttractedTo.Count == 0) {
                        genders.ForEach(g => {
                            if (rand.Next()%2 == 0)
                                x.GendersAttractedTo.Add(g);
                        });
                        if (!x.GendersAttractedTo.Any())
                            x.GendersAttractedTo.Add(genders.First());
                    }
                });

                if (_db.ChangeTracker.Entries().Any(x => x.State == EntityState.Modified)) {
                    await _db.SaveChangesAsync();

                    users.ForEach(x => {
                        x.UserGender = genders.First(y => y.Id == x.UserGenderId);
                        x.UserBodyType = bodytypes.First(y => y.Id == x.UserBodyTypeId);
                    });

                    return Json(users.Select(x => new {
                        x.LastName,
                        x.PreferredName,
                        x.HeightInches,
                        UserGender = x.UserGender.Name,
                        UserBodyType = x.UserBodyType.Name,
                        GendersAttractedTo = String.Join(",", x.GendersAttractedTo.Select(y => y.Name)),
                    }));
                }

            return null;
        }

        public async Task<IActionResult> CalculateSpeed() {
            var users = await _db.Users
                .Select(x => x.FirebaseUserId)
                .Distinct()
                .ToListAsync();

            foreach (var uid in users) {
                await _db.Database.ExecuteSqlRawAsync(@$"
                    create temporary table temp_speed (Id int, SpeedFromLast double);

                    insert into temp_speed
                    select Id, abs((DistanceBetweenPoints(Latitude, Longitude, lag(Latitude) over (partition by FirebaseUserId order by Timestamp), lag(Longitude) over (partition by FirebaseUserId order by Timestamp)) / 1000.0 * 0.62137) / (timestampdiff(microsecond, lag(Timestamp) over (partition by FirebaseUserId order by Timestamp), timestamp) / 1000.0 / 1000.0 / 3600.0)) as SpeedFromLast
                    from Locations
                    where FirebaseUserId='{uid}'
                    order by FirebaseUserId, Timestamp;

                    update Locations l inner join temp_speed t on l.Id = t.Id
                    set l.SpeedFromLast = t.SpeedFromLast;

                    drop temporary table temp_speed;
                ");
            }

            return Json(users);
        }

        public async Task<IActionResult> Index() {

            if (_env.IsDevelopment()) {
                // var startTime = DateTime.Now;


                // var left = await _db.Precalc_Locations
                //     .AsNoTracking()
                //     .ToListAsync();

                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, left complete");

                // var right = left.Select(x => x.Clone() as Model.Precalc_Location).ToList();

                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, right complete");


                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  writing files...");
                // await System.IO.File.WriteAllTextAsync(@"/Users/dan/Library/Mobile Documents/com~apple~CloudDocs/code/real/data/left.json", System.Text.Json.JsonSerializer.Serialize(left));
                // await System.IO.File.WriteAllTextAsync(@"/Users/dan/Library/Mobile Documents/com~apple~CloudDocs/code/real/data/right.json", System.Text.Json.JsonSerializer.Serialize(right));
                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, files complete");


                // var combined = left
                //     .Join(
                //         right,
                //         l => true,
                //         r => true,
                //         (l, r) => new {
                //             Left = l,
                //             Right = r,
                //         }
                //     );

                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, combined complete");

                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  writing file...");
                // await System.IO.File.WriteAllTextAsync(@"/Users/dan/Library/Mobile Documents/com~apple~CloudDocs/code/real/data/combined.json", System.Text.Json.JsonSerializer.Serialize(combined));
                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, file complete");

                // var combined2 = combined
                //     .Select(x => new {
                //         Left = x.Left,
                //         Right = x.Right,
                //         Distance = Model.DistanceBetweenPoints.Miles(x.Left.Latitude, x.Left.Longitude, x.Right.Latitude, x.Right.Longitude),
                //     })
                //     .Where(x => x.Distance <= 100);

                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, combined2 complete");

                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  writing file...");
                // await System.IO.File.WriteAllTextAsync(@"/Users/dan/Library/Mobile Documents/com~apple~CloudDocs/code/real/data/combined2.json", System.Text.Json.JsonSerializer.Serialize(combined2));
                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, file complete");

                // var combined3 = combined2
                //     .Where(x =>
                //         x.Left.FirebaseUserId != x.Right.FirebaseUserId
                //         && (
                //                x.Left.StartTime.Between(x.Right.StartTime, x.Right.EndTime)
                //             || x.Right.StartTime.Between(x.Left.StartTime, x.Left.EndTime)
                //             || x.Left.EndTime.Between(x.Right.StartTime, x.Right.EndTime)
                //             || x.Right.EndTime.Between(x.Left.StartTime, x.Left.EndTime)
                //             || x.Left.StartTime == x.Right.StartTime
                //             || x.Left.EndTime == x.Right.EndTime
                //         )
                //     );

                
                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, combined3 complete");
                
                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  writing file...");
                // await System.IO.File.WriteAllTextAsync(@"/Users/dan/Library/Mobile Documents/com~apple~CloudDocs/code/real/data/combined3.json", System.Text.Json.JsonSerializer.Serialize(combined3));
                // Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}:  runtime: {DateTime.Now - startTime}, file complete");

                
                // var endTime = DateTime.Now;
                // var runTime = endTime - startTime;

                // var result = await UpdatedNullProfileData();
                // if (result != null)
                //     return result;
            }

            var model = new HomeIndexViewModel {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            };

            if (model.IsAuthenticated) {
                var uid = this.GetUserFirebaseId();
                model.User = await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUserId.Equals(uid));
                model.User.CurrentLocation = await _db.Locations
                    .OrderBy(x => x.FirebaseUserId)
                    .ThenByDescending(x => x.Timestamp)
                    .Where(x => x.FirebaseUserId == uid)
                    .FirstOrDefaultAsync();
            }

            return View(model);
        }

        public IActionResult Privacy() {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> Error() {

            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Headers["host"];
            var req = HttpContext.Features?.Get<IExceptionHandlerPathFeature>()?.Path;
            var path = $"{scheme}://{host}{req}";

            var model = new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Exception = HttpContext.Features?.Get<IExceptionHandlerPathFeature>()?.Error ?? new InvalidProgramException(),
                Path = path,
                UserName = this.User.Identity.Name,
                FirebaseUserId = (this as Controller).GetUserFirebaseId(),
            };

            var email = new EmailViewModel {
                Recipient = "seraieis@gmail.com", 
                Subject = $"REAL error occurred",
                Body = await this.RenderViewAsync("ErrorEmail", model),
                Priority = MailPriority.High,
            };

            await _emailController.SendEmailAsync(email);

            var trace = new Model.Analytic {
                StartTimestamp = DateTime.UtcNow,
                EndTimestamp = DateTime.UtcNow,
                Namespace = this.GetType().Namespace,
                Area = this.Request.RouteValues["area"]?.ToString() ?? "",
                Controller = this.Request.RouteValues["controller"]?.ToString() ?? "",
                Action = this.Request.RouteValues["action"]?.ToString() ?? "",
                UserName = this.User.Identity.Name,
                FirebaseUserId = (this as Controller).GetUserFirebaseId(),
                IPv4 = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                IPv6 = this.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString(),
                Host = this.Request.Host.ToString(),
                Path = HttpContext.Features?.Get<IExceptionHandlerPathFeature>()?.Path,
                QueryString = this.Request.QueryString.ToString(),
            };
            trace.AnalyticError = new Model.AnalyticError {
                AnalyticId = trace.Id,
                TraceId = HttpContext.TraceIdentifier,
                RequestId = Activity.Current?.Id,
                Message = model.Exception.Message,
                StackTrace = model.Exception.StackTrace,
            };

            _db.Analytics.Add(trace);
            _db.SaveChanges();

            return View(model);
        }
    }
}
