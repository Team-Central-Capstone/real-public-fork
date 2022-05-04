using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Real.Web.Models;
using Real.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using Real.Web.Areas.API.Models;

namespace Real.Web.Controllers {

    [Route("account")]
    [Route("[controller]/[action]")]
    public class AccountController : Controller {
        private readonly ILogger<AccountController> _logger;
        private readonly CapstoneContext _db;
        private readonly Real.Web.Areas.API.Controllers.UsersController _usersController;

        public AccountController(ILogger<AccountController> logger, CapstoneContext db, Real.Web.Areas.API.Controllers.UsersController usersController) {
            _logger = logger;
            _db = db;
            _usersController = usersController;
        }

        public IActionResult Index() {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Profile(string uid, bool changes = false, int id = default) {
            if (String.IsNullOrEmpty(uid)) {
                uid = this.GetUserFirebaseId();
            }

            if (id == default)
                id = (await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == uid))?.Id ?? default;

            if (id == default)
                return new NotFoundResult();

            var result = await _usersController.GetProfileAsync(uid);
            var model = result.Value;
            model.Changes = changes;
            
            // ViewData["Users"] = await _db.Users
            //     .Where(x => (String.IsNullOrEmpty(x.FirebaseUserId) || !x.FirebaseUserId.StartsWith("testuser")) && x.FirebaseUserId != "6QUsr0aeWih0Z3Zb7ibbBPLjEk73")
            //     .OrderBy(x => x.LastName)
            //     .Select(x => new Real.Model.User {
            //         Id = x.Id,
            //         FirebaseUserId = x.FirebaseUserId,
            //         FirstName = x.FirstName,
            //         PreferredName = x.PreferredName,
            //         LastName = x.LastName,
            //     })
            //     .ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProfile([FromForm]UserProfileUpdateViewModel model) {

            var result = await _usersController.PutProfileAsync(this.GetUserFirebaseId(), model);
            model = result.Value;

            return RedirectToAction("Profile", new { uid = model.FirebaseUserId, changes = model.Changes, });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(UserProfileViewModel model) {

            if (model.File == null || model.User?.Id == 0)
                return BadRequest();

            var image = await _usersController.AddImageAsync(this.GetUserFirebaseId(), model.File);
            if (image == null || image.Value.Id == 0)
                return StatusCode(500);

            return RedirectToAction("Profile", new { uid = model.User.FirebaseUserId, changes = true, });
        }

        [HttpGet]
        public async Task<IActionResult> Get(int userId, int imageId) {
            if (userId <= 0 || imageId <= 0)
                return BadRequest();

            var image = await _usersController.GetImageByIdAsync(this.GetUserFirebaseId(), imageId);
            if (image == null)
                return NotFound();

            return Ok(new {
                image.Value.ContentType,
                image.Value.FileName,
                Image = image.Value.GetImageString()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(int userId, int imageId) {
            if (userId <= 0 || imageId <= 0)
                return BadRequest();

            var image = await _usersController.GetImageByIdAsync(this.GetUserFirebaseId(), imageId);
            if (image == null)
                return NotFound();

            var result = new FileContentResult(image.Value.Image, image.Value.ContentType);
            return result;
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int userId, int imageId) {
            await _usersController.DeleteImageAsync(this.GetUserFirebaseId(), imageId);

            return RedirectToAction("Profile");
        }


        [AllowAnonymous]
        [Route("signin-google")]
        public IActionResult GoogleSignIn() {
            var returnUrl = Request.Query["returnUrl"].ToString();
            var properties = new AuthenticationProperties {
                 RedirectUri = Url.Action("GoogleResponse", "account", new { area = "" }, this.Request.Scheme),
                 IsPersistent = true,
            };
            properties.Items.Add("returnUrl", returnUrl);
            
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [Route("googleresponse")]
        public async Task<IActionResult> GoogleResponse() {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new System.Security.Claims.ClaimsPrincipal(result.Principal.Identity)
            );

            var uid = this.GetUserFirebaseId();
            var user = await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUserId.Equals(uid));

            if (user == null) {
                user = new Model.User {
                    FirebaseUserId = uid,
                    RegisteredTimestamp = DateTime.UtcNow,
                };
                await _db.Users.AddAsync(user);
            } else {
                user.LastLoginTimestamp = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            var returnUrl = user?.Active ?? false ?
                result.Properties.Items["returnUrl"] : 
                Url.Action("Profile", "Account", new { area = "" }, "https");
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "" }, "https");

            // var returnUrl = user?.Active ?? false ?
            //     Url.Action("Index", "Home", new { area = "" }, "https") : 
            //     Url.Action("Profile", "Account", new { area = "" }, "https");
            
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        [Route("signout-google")]
        public async Task<IActionResult> GoogleSignOut() {
            var url = Url.Action("Index", "Home", new { area = "" }, this.Request.Scheme);
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
 
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(url);
        }



    }
}
