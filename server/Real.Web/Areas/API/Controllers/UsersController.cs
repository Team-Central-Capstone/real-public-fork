using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real.Data.Contexts;
using Real.Model;
using Real.Web.Areas.API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Controllers {

    /// <summary>
    /// Endpoints for interaction with the User, their profile information, and survey question responses.
    /// </summary>
    [Area("API")]
    [Route("API/[controller]")]
    [ApiController]
    public class UsersController : Controller {
        private readonly CapstoneContext _context;
        private readonly ValidationController _validationController;

        #region Constructor

        public UsersController(CapstoneContext context, ValidationController validationController) {
            _context = context;
            _validationController = validationController;
        }

        #endregion

        #region User endpoints

        // GET: https://ccsu-sp2022-3md-capstone.us-east-2.elasticbeanstalk.com/api/Users
        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [Produces("application/json", Type = typeof(ActionResult<IEnumerable<User>>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<IEnumerable<User>>))]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersAsync() {
            var query = _context.Users
                .Include(x => x.UserGender)
                .Include(x => x.UserBodyType)
                .Include(x => x.GendersAttractedTo)
                .AsQueryable();

            return Ok(await query.ToListAsync());
        }

        // GET: https://ccsu-sp2022-3md-capstone.us-east-2.elasticbeanstalk.com/api/Users/abcdefg
        /// <summary>
        /// Gets single user by Firebase UID
        /// </summary>
        [HttpGet("{userId}")]
        [Produces("application/json", Type = typeof(ActionResult<User>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<User>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<User>> GetUserAsync(string userId) {
            var user = await _context.Users
                .Include(x => x.UserGender)
                .Include(x => x.UserBodyType)
                .Include(x => x.GendersAttractedTo)
                .FirstOrDefaultAsync(x => x.FirebaseUserId == userId);

            if (user == null)
                return NotFound();

            return user;
        }

        /// <summary>
        /// Gets single user by database ID (mostly used by Dan)
        /// </summary>
        [HttpGet("byid/{id}")]
        [Produces("application/json", Type = typeof(ActionResult<User>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<User>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<User>> GetUserByIdAsync(int id) {
            var user = await _context.Users
                .Include(x => x.UserGender)
                .Include(x => x.UserBodyType)
                .Include(x => x.GendersAttractedTo)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound();

            return user;
        }


        // GET: api/Users/5/Locations
        /// <summary>
        /// Get all locations for a single record
        /// </summary>
        [HttpGet("Locations")]
        [Produces("application/json", Type = typeof(ActionResult<IEnumerable<Model.Location>>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<IEnumerable<Model.Location>>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<IEnumerable<Model.Location>>> GetAllLocations([FromQuery]string userId) {
            var results = await _context.Locations
                .Where(x => x.FirebaseUserId == userId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
            
            if (results == null || results.Count() == 0)
                return NotFound();

            return results;
        }

        // GET: api/Users/5/Nearby/
        /// <summary>
        /// Gets closest [take] users to the users current location (default is 100)
        /// </summary>
        [HttpGet("/Locations/Nearby")]
        [Produces("application/json", Type = typeof(ActionResult<IEnumerable<Model.Location>>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<IEnumerable<Model.Location>>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<IEnumerable<Model.Location>>> GetUsersAroundLocation([FromQuery]string userId, [FromQuery]double latitude, [FromQuery]double longitude, int take = 100) {

            var user = await _context.Users
                .Where(x => x.FirebaseUserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return BadRequest();

            if (latitude == 0 && longitude == 0) {
                var location = await _context.Locations
                    .OrderByDescending(x => x.Timestamp)
                    .Where(x => x.FirebaseUserId == user.FirebaseUserId)
                    .Take(1)
                    .FirstOrDefaultAsync();

                if (location == null)
                    return BadRequest();

                latitude = location.Latitude;
                longitude = location.Longitude;
            }

            var locations = await _context.Locations
                .AsNoTracking()
                .Join(
                    _context.Locations
                        .AsNoTracking()
                        .GroupBy(x => x.FirebaseUserId)
                        .Select(x => new Model.Location {
                            FirebaseUserId = x.Key,
                            Timestamp = x.Max(y => y.Timestamp),
                        }),
                    l => new { l.FirebaseUserId, l.Timestamp, },
                    l2 => new { l2.FirebaseUserId, l2.Timestamp },
                    (l, l2) => l
                )
                .OrderBy(x => CapstoneContext.DistanceBetweenPoints(latitude, longitude, x.Latitude, x.Longitude))
                .Where(x => x.FirebaseUserId != user.FirebaseUserId)
                .Select(x => new Model.Location {
                    FirebaseUserId = x.FirebaseUserId,
                    Id = x.Id,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Timestamp = x.Timestamp,
                    Distance = CapstoneContext.DistanceBetweenPoints(latitude, longitude, x.Latitude, x.Longitude)
                })
                .Take(take)
                .ToListAsync();

            if (locations == null)
                return NotFound();

            return locations;
        }

        // TODO: use for test graph
        /// <summary>
        /// Update user profile information
        /// </summary>
        [HttpPatch("{userId}")]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<User>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, Type = null)]
        public async Task<ActionResult<User>> PatchUserAsync(string userId, [FromBody]UserUpdateViewModel model) {

            if (!String.IsNullOrEmpty(model.Email)) {
                try {
                    var n = new MailAddress(model.Email);
                } catch (FormatException) {
                    return StatusCode(StatusCodes.Status406NotAcceptable, "invalid email address"); 
                }
            }

            var birthdayResult = _validationController.AtLeast18YearsOld(model.Birthdate);
            if (!birthdayResult.Value)
                return StatusCode(StatusCodes.Status406NotAcceptable, "too young");
            
            var user = await _context.Users
                .Include(x => x.GendersAttractedTo)
                .FirstOrDefaultAsync(x => x.FirebaseUserId == userId);
            if (user == null)
                return NotFound();

            if (!String.IsNullOrEmpty(userId) && user.FirebaseUserId != userId)
                user.FirebaseUserId = userId;
            
            if (!String.IsNullOrEmpty(model.FirstName) && user.FirstName != model.FirstName)
                user.FirstName = model.FirstName;
            
            if (!String.IsNullOrEmpty(model.PreferredName) && user.PreferredName != model.PreferredName)
                user.PreferredName = model.PreferredName;
            
            if (!String.IsNullOrEmpty(model.LastName) && user.LastName != model.LastName)
                user.LastName = model.LastName;
            
            if (!String.IsNullOrEmpty(model.ProfileIntro) && user.ProfileIntro != model.ProfileIntro)
                user.ProfileIntro = model.ProfileIntro;

            if (!String.IsNullOrEmpty(model.Email) && user.Email != model.Email) {
                user.Email = model.Email;
            }

            if (model.Birthdate != default && user.Birthdate != model.Birthdate)
                user.Birthdate = model.Birthdate;

            if (model.HeightInches != default && user.HeightInches != model.HeightInches)
                user.HeightInches = model.HeightInches;
            
            if (model.UserGenderId != default && user.UserGenderId != model.UserGenderId)
                user.UserGenderId = model.UserGenderId;

            if (model.UserAttractedGenderIds?.Count > 0) {
                user.GendersAttractedTo.Clear();
                user.GendersAttractedTo.AddRange(
                    _context.UserGenders
                        .Where(x => model.UserAttractedGenderIds
                        .Contains(x.Id))
                );
            }
            
            if (model.UserBodyTypeId != default && user.UserBodyTypeId != model.UserBodyTypeId)
                user.UserBodyTypeId = model.UserBodyTypeId ?? user.UserBodyTypeId;
            
            _context.Entry(user).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!UserExists(userId))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Updates property with given name for a user
        /// </summary>
        [Obsolete("Don't use this. It was a bad idea.")]
        [HttpPatch("{userId}/{propertyName}/{value}")]
        [Consumes("application/json")]
        [Produces("application/json", Type = null)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Database commit unsuccessful")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Property does not exist")]
        public async Task<ActionResult<User>> PatchUserPropertyAsync(string userId, string propertyName, string value) {
            
            throw new NotImplementedException("I really don't want to enable this. If you think we need it, let me know.");

            if (!_context.Entry<User>(new Model.User()).Properties.Any(x => x.Metadata.Name == propertyName))
                return BadRequest();

            // var user = await _context.Users
            //     .Where(x => x.FirebaseUserId == userId)
            //     .FirstOrDefaultAsync();

            // if (user == null)
            //     return NotFound();

            // var prop = _context.Entry(user).Property(propertyName);
            // var type = prop.Metadata.ClrType;
            // var updatedValue = Convert.ChangeType(value, type);

            // _context.Entry(user).Property(propertyName).CurrentValue = updatedValue;
            // _context.Entry(user).Property(propertyName).IsModified = true;
            // var result = await _context.SaveChangesAsync();

            // if (result == 0)
            //     return StatusCode(StatusCodes.Status500InternalServerError);

            // return NoContent();
        }

        /// <summary>
        /// Creates new user record
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<User>))]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ActionResult<User>))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, Type = null)]
        public async Task<ActionResult<Model.User>> PostUserAsync([FromBody]UserCreateViewModel model) {

            if (!String.IsNullOrEmpty(model.Email)) {
                try {
                    var n = new MailAddress(model.Email);
                } catch (FormatException) {
                    return StatusCode(StatusCodes.Status406NotAcceptable, "invalid email address"); 
                }
            }

            if (await _context.Users.AnyAsync(x => x.FirebaseUserId == model.FirebaseUserId))
                return StatusCode(StatusCodes.Status406NotAcceptable, "UID already exists");

            var user = new Model.User {
                FirebaseUserId = model.FirebaseUserId,
                FirstName = model.FirstName,
                PreferredName = model.PreferredName,
                LastName = model.LastName,
                Birthdate = model.Birthdate,
                UserGenderId = model.UserGenderId,
                UserBodyTypeId = model.UserBodyTypeId,
                HeightInches = model.HeightInches,
                ProfileIntro = model.ProfileIntro,
                Email = model.Email,

                Active = true,
                RegisteredTimestamp = DateTime.UtcNow,
                LastLoginTimestamp = DateTime.UtcNow,
                ProfileLastUpdatedTimestamp = DateTime.UtcNow,
            };

            if (model.UserAttractedGenderIds?.Count > 0) {
                var genders = await _context.UserGenders
                    .Where(x => model.UserAttractedGenderIds.Contains(x.Id))
                    .ToListAsync();
                user.GendersAttractedTo.AddRange(genders);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        /// <summary>
        /// Deletes a user
        /// </summary>
        [HttpDelete("{userId}")]
        [Consumes("application/json")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> DeleteUserAsync(string userId) {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == userId);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Users/5/Export
        /// <summary>
        /// Exports all data for user
        /// </summary>
        [HttpGet("{userId}/Export")]
        [Produces("application/json", Type = typeof(ActionResult<Model.User>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<Model.User>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<Model.User>> ExportUserInformationAsync(string userId) {
            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.UserGender)
                .Include(x => x.UserSurveyResponses)
                .Include(x => x.UserImages)
                .Where(x => x.FirebaseUserId == userId)
                .FirstOrDefaultAsync();
            
            if (user == null) 
                return NotFound();

            user.Locations = await _context.Locations
                .Where(x => x.FirebaseUserId == user.FirebaseUserId)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

            return user;
        }

        private bool UserExists(string id) {
            return _context.Users.Any(e => e.FirebaseUserId == id);
        }

        #endregion

        #region User Images

        // POST: api/Users/5/Image
        /// <summary>
        /// Uploads new profile photo
        /// </summary>
        /// <remarks>
        /// {
        ///     "FileName": "image.jpg"
        ///     "ContentType": "image/jpeg"
        ///     "Description": "a picture"
        ///     "Base64ImageString": "abcef0123456789"
        /// }
        /// </remarks>
        [HttpPost("{userId}/Image")]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        public async Task<ActionResult<Model.UserImage>> UploadImageAsync(string userId, [FromBody]UserProfileImageUploadViewModel model) {

            if (String.IsNullOrEmpty(userId))
                return BadRequest("UID is required");
            if (model is null)
                return BadRequest("model needs to be in the request body");

            var uid = await _context.Users
                .Where(x => x.FirebaseUserId == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (uid == default)
                return NotFound();

            var image = new Model.UserImage {
                UserId = uid,
                ContentType = model.ContentType,
                FileName = model.FileName,
                UploadDate = DateTime.UtcNow,
                Image = Convert.FromBase64String(model.Base64ImageString),
            };

            await _context.UserImages.AddAsync(image);
            await _context.SaveChangesAsync();

            image.Image = null;
            return image;
        }


        [Obsolete("Only used internally within the server code")]
        [HttpPost("{userId}/Image/FromFormFile")]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        public async Task<ActionResult<Model.UserImage>> AddImageAsync(string userId, [FromForm]IFormFile file) {
            var uid = await _context.Users
                .Where(x => x.FirebaseUserId == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (uid == default)
                return NotFound();

            byte[] fileBytes = null;
            using (var ms = new MemoryStream()) {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            var image = new Model.UserImage {
                UserId = uid,
                ContentType = file.ContentType,
                FileName = file.FileName,
                UploadDate = DateTime.UtcNow,
                Image = fileBytes,
            };

            await _context.UserImages.AddAsync(image);
            await _context.SaveChangesAsync();

            return image;
        }

        /// <summary>
        /// Gets most recent image for user
        /// </summary>
        [HttpGet("{userId}/Image")]
        [Produces("application/json", Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<Model.UserImage>> GetMostRecentImageAsync(string userId) {
            var user = await _context.Users
                .Where(x => x.FirebaseUserId == userId)
                .FirstOrDefaultAsync();

            if (user == null || !user.FirebaseUserId.Equals(userId))
                return BadRequest("user not found");

            var image = await _context.UserImages
                .OrderByDescending(x => x.UploadDate)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (image == null)
                return NotFound();

            return image;
        }

        // GET: api/Users/5/Image/1
        /// <summary>
        /// Gets image for user
        /// </summary>
        [HttpGet("{userId}/Image/{imageId}")]
        [Produces("application/json", Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<Model.UserImage>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<Model.UserImage>> GetImageByIdAsync(string userId, int imageId) {
            var uid = await _context.Users
                .Where(x => x.FirebaseUserId == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            if (uid == default)
                return BadRequest();

            var image = await _context.UserImages.FirstOrDefaultAsync(x => x.UserId == uid && x.Id == imageId);
            if (image == null)
                return NotFound();

            return image;
        }

        // DELETE: api/Users/5/Image/5
        /// <summary>
        /// Deletes image
        /// </summary>
        [HttpDelete("{userId}/Image/{imageId}")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        public async Task<ActionResult> DeleteImageAsync(string userId, int imageId) {
            var uid = (await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == userId)).Id;
            _context.UserImages.RemoveRange(_context.UserImages.Where(x => x.UserId == uid && x.Id == imageId));
            int rows = await _context.SaveChangesAsync();

            if (rows == 0)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return NoContent();
        }

        #endregion

        #region Profile

        /// <summary>
        /// Get a list of possible response weight values
        /// </summary>
        [HttpGet("responseweights")]
        [Produces("application/json", Type = typeof(IEnumerable<UserSurveyResponseWeight>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserSurveyResponseWeight>))]
        public ActionResult<IEnumerable<UserSurveyResponseWeight>> GetSurveyResponseWeightTypes() {
            var types = Enum.GetValues<UserSurveyResponseWeight>()
                .Cast<UserSurveyResponseWeight>()
                .Select(x => new {
                    Name = x.ToString(),
                    Id = (int)x
                })
                .ToList();
                // .ToDictionary(x => x.ToString(), x => (int)x);
            return Ok(types);
        }

        /// <summary>
        /// Identifies if a user's profile is complete
        /// </summary>
        [HttpGet("{userId}/Profile/Completed")]
        [Produces("application/json", Type = typeof(ActionResult<UserProfileCompletedViewModel>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<UserProfileCompletedViewModel>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<UserProfileCompletedViewModel>> GetUserProfileCompleteAsync(string userId) {
            var user = await _context.Users
                .Include(x => x.GendersAttractedTo)
                .FirstOrDefaultAsync(x => x.FirebaseUserId == userId);

            if (user == null)
                return NotFound();

             var properties = user.GetType()
                .GetProperties()
                .Where(x => x.CanWrite && x.MemberType == MemberTypes.Property)
                .Where(x => x.GetGetMethod()?.IsVirtual == false)
                .Where(x => !x.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute)))
                .Where(x => x.GetValue(user) == default)
                .Select(x => x.Name)
                .ToList();

            var hasImage = await _context.UserImages.AnyAsync(x => x.UserId == user.Id);

            var questions = await _context.SurveyQuestions
                .GroupJoin(
                    _context.UserSurveyResponses
                        .Include(x => x.SurveyAnswers)
                        .Where(x => x.UserId == user.Id),
                    x => x.Id,
                    y => y.SurveyQuestionId,
                    (sq, usr) => new { Question = sq, UserSurveyResponse = usr }
                )
                .SelectMany(
                    x => x.UserSurveyResponse.DefaultIfEmpty(),
                    (x, usr) => new {
                        Question = x.Question,
                        UserSurveyResponse = usr
                    }
                )                
                .ToListAsync();

            var unanswered = questions
                .Where(x => x.UserSurveyResponse == null)
                .Select(x => x.Question)
                .ToList();

            var result = new UserProfileCompletedViewModel {
                HasImage = hasImage,
                RemainingProfileItems = properties,
                RemainingSurveyQuestions = unanswered
            };

            return result;
        }

        /// <summary>
        /// Gets user profile
        /// </summary>
        [HttpGet("{userId}/Profile")]
        [Produces("application/json", Type = typeof(ActionResult<UserProfileViewModel>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<UserProfileViewModel>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<UserProfileViewModel>> GetProfileAsync(string userId) {

            var user = await _context.Users
                .Include(x => x.GendersAttractedTo)
                .Include(x => x.UserBodyType)
                .Include(x => x.UserGender)
                .FirstAsync(x => x.FirebaseUserId == userId);
            if (user == null)
                return NotFound();

            var model = new UserProfileViewModel {
                User = user,
                Genders = await _context.UserGenders.OrderBy(x => x.Id).ToListAsync(),
            };

            model.UserImage = await _context.UserImages
                .OrderBy(x => x.UploadDate)
                .Where(x => x.UserId == model.User.Id)
                .Select(x => new Model.UserImage {
                    Id = x.Id,
                    UserId = x.UserId,
                    UploadDate = x.UploadDate,
                })
                .FirstOrDefaultAsync();
            model.HasImage = model.UserImage != null;

            model.Questions = await _context.SurveyQuestions
                .Include(x => x.Answers.OrderBy(x => x.Id))
                .OrderBy(x => x.Order)
                .GroupJoin(
                    _context.UserSurveyResponses
                        .Include(x => x.SurveyAnswers)
                        .Where(x => x.UserId == model.User.Id),
                    sq => sq.Id,
                    usr => usr.SurveyQuestionId,
                    (sq, usr) => new {
                        sq,
                        usr,
                    }
                )
                .SelectMany(x => x.usr.DefaultIfEmpty(), (x, y) => new UserProfileViewModelQuestion {
                    SurveyQuestion = x.sq,
                    UserSurveyResponse = y,
                })
                .OrderBy(x => x.SurveyQuestion.Order)
                .ToListAsync();

            var index = 0;
            model.Questions.ForEach(x => { 
                x.Index = index++; 
                x.SurveyQuestion.Answers = x.SurveyQuestion.Answers.OrderBy(x => x.Id).ToList();
            });

            return model;
        }

        // TODO: use for test graph
        /// <summary>
        /// Updates the user's survey responses
        /// </summary>
        [HttpPut("{userId}/Profile")]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<UserProfileUpdateViewModel>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<UserProfileUpdateViewModel>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null, Description = "No changes made")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<UserProfileUpdateViewModel>> PutProfileAsync(string userId, UserProfileUpdateViewModel model) {

            if (String.IsNullOrEmpty(userId) || model == null)
                return BadRequest();

            var user = await _context.Users
                .Include(x => x.UserSurveyResponses)
                .ThenInclude(x => x.SurveyAnswers)
                .FirstOrDefaultAsync(x => x.FirebaseUserId.Equals(userId));

            if (user == null)
                return NotFound("User not found");

            model.FirebaseUserId = userId;

            foreach (var item in model.Questions) {
                var questionType = item.SurveyQuestion.QuestionType;
                var userResponse = item.UserSurveyResponse;

                var responseExists = user.UserSurveyResponses.Any(x => x.SurveyQuestionId == item.SurveyQuestion.Id);
                if (!responseExists) {

                    var answerIds = model.Questions
                        .Single(x => x.SurveyQuestion.Id == item.SurveyQuestion.Id)
                        .UserSurveyResponse
                        .SurveyAnswers
                        .Select(x => x.Id)
                        .OrderBy(x => x)
                        .ToList();

                    user.UserSurveyResponses.Add(new Model.UserSurveyResponse {
                        SurveyQuestionId = item.SurveyQuestion.Id,
                        UserId = user.Id,
                        SurveyAnswers = await _context.SurveyAnswers
                            .Where(x => answerIds.Contains(x.Id))
                            .ToListAsync(),
                        SurveyAnswerResponse = userResponse.SurveyAnswerResponse,
                        UserSurveyResponseWeight = userResponse.UserSurveyResponseWeight,

                    });
                } else {
                    var surveyResponse = user.UserSurveyResponses.Single(x => x.SurveyQuestionId == item.SurveyQuestion.Id);

                    surveyResponse.UserSurveyResponseWeight = item.UserSurveyResponse.UserSurveyResponseWeight;

                    switch (questionType) {
                        case Model.QuestionType.YesNo:
                        case Model.QuestionType.FreeForm:
                        case Model.QuestionType.FreeFormShort:
                        case Model.QuestionType.Numeric:
                            if (!surveyResponse.SurveyAnswerResponse?.Equals(userResponse.SurveyAnswerResponse) ?? true) {
                                surveyResponse.SurveyAnswerResponse = userResponse.SurveyAnswerResponse;
                                _context.Entry(surveyResponse).Property(x => x.SurveyAnswerResponse).IsModified = true;
                            }
                            break;

                        case Model.QuestionType.SingleChoice:
                            var updatedId = model.Questions
                                .Single(x => x.SurveyQuestion.Id == item.SurveyQuestion.Id)
                                .UserSurveyResponse
                                .SurveyAnswers
                                .SingleOrDefault()?
                                .Id;

                            if (!updatedId.HasValue)
                                break;

                            if (surveyResponse.SurveyAnswers == null)
                                surveyResponse.SurveyAnswers = new List<Model.SurveyAnswer>();
                            
                            if (surveyResponse.SurveyAnswers.Count == 0) {
                                surveyResponse.SurveyAnswers.Add(_context.SurveyAnswers.Single(x => x.Id == updatedId));
                            } else {
                                var currentId = surveyResponse.SurveyAnswers.Single().Id;
                                if (currentId != updatedId) {
                                    surveyResponse.SurveyAnswers.Clear();
                                    surveyResponse.SurveyAnswers.Add(_context.SurveyAnswers.Single(x => x.Id == updatedId));
                                }
                            }

                            break;

                        case Model.QuestionType.MultipleChoice:
                            var updatedIds = model.Questions
                                .Single(x => x.SurveyQuestion.Id == item.SurveyQuestion.Id)
                                .UserSurveyResponse
                                .SurveyAnswers
                                .Select(x => x.Id)
                                .OrderBy(x => x)
                                .ToList();
                            var currentIds = surveyResponse
                                .SurveyAnswers
                                .Select(x => x.Id)
                                .OrderBy(x => x)
                                .ToList();

                            if (surveyResponse.SurveyAnswers == null)
                                surveyResponse.SurveyAnswers = new List<Model.SurveyAnswer>();

                            if (updatedIds.Count != currentIds.Count || !Enumerable.SequenceEqual(updatedIds, currentIds)) {
                                var answers = await _context.SurveyAnswers.Where(x => updatedIds.Contains(x.Id)).ToListAsync();

                                surveyResponse.SurveyAnswers.Clear();
                                surveyResponse.SurveyAnswers.AddRange(answers);
                            }


                            break;
                        default:
                            throw new NotImplementedException($"Unknown question type: {questionType.ToString()}");
                    }
                }
            }

            model.Changes = _context.ChangeTracker.Entries().Any(x => x.State != EntityState.Unchanged);
            if (model.Changes) {
                user.ProfileLastUpdatedTimestamp = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _context.Database.ExecuteSqlRawAsync("call sp_RecalculateProfileMatch({0})", user.Id);

                return model;
            }

            return NoContent();
        }
        
        /// <summary>
        /// Updates user survey response for a single question
        /// </summary>
        /// <remarks>
        /// For a free-form or numeric field (cannot be combined with AnswerIds):
        /// 
        ///     {
        ///         "SurveyAnswerResponse": "I like coffee"
        ///     }
        /// 
        /// For a single or multiple choice, pass the IDs of the selected answers in an array (cannot be combined with SurveyAnswerResponse):
        /// 
        ///     {
        ///         "AnswerIds": [1,2]
        ///     }
        /// 
        /// To update the weight (this can be added to either of the two above examples):
        /// 
        ///     {
        ///         "UserSurveyResponseWeight": 1
        ///     }
        /// </remarks>
        [HttpPatch("{userId}/Profile/{questionId}")]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<UserSurveyResponse>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<UserSurveyResponse>))]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ActionResult<UserSurveyResponse>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        public async Task<ActionResult<UserSurveyResponse>> PatchProfileAsync(string userId, int questionId, [FromBody]UserSurveyResponseViewModel model) {

            // check for invalid parameters
            if (String.IsNullOrEmpty(userId) || questionId <= 0 || model == null)
                return BadRequest();
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUserId == userId);
            if (user == null)
                return NotFound("user not found");

            if (!await _context.SurveyQuestions.AnyAsync(x => x.Id == questionId))
                return NotFound($"QuestionId {questionId} not found");
            
            var changedMade = false;

            var response = await _context.UserSurveyResponses
                .Include(s => s.SurveyAnswers)
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.SurveyQuestionId == questionId);

            if (response == null) {
                response = new UserSurveyResponse {
                    UserId = user.Id,
                    SurveyQuestionId = questionId,
                };
                _context.Entry(response).State = EntityState.Added;
            }
            
            // update either string response or selected answers
            if (String.IsNullOrEmpty(model.SurveyAnswerResponse) == false && !response.SurveyAnswerResponse.Equals(model.SurveyAnswerResponse)) {
                response.SurveyAnswerResponse = model.SurveyAnswerResponse;
                changedMade = true;
            }

            if (model.UserSurveyResponseWeight.HasValue && response.UserSurveyResponseWeight != model.UserSurveyResponseWeight) {
                response.UserSurveyResponseWeight = model.UserSurveyResponseWeight.Value;
                changedMade = true;
            } else if (response.UserSurveyResponseWeight == 0) {
                response.UserSurveyResponseWeight = UserSurveyResponseWeight.Medium;
                changedMade = true;
            }

            var toRemove = response.SurveyAnswers
                .Select(x => x.Id)
                .Except(model.AnswerIds)
                .ToList();
            var toAdd = model.AnswerIds
                .Except(response.SurveyAnswers.Select(y => y.Id))
                .ToList();

            if (toRemove.Count > 0) {
                response.SurveyAnswers.RemoveAll(x => toRemove.Contains(x.Id));
                changedMade = true;
            }

            if (toAdd.Count > 0) {
                var items = await _context.SurveyAnswers.Where(x => toAdd.Contains(x.Id)).ToListAsync();
                response.SurveyAnswers.AddRange(items);
                changedMade = true;
            }

            if (changedMade == true) {
                var state = _context.Entry(response).State;

                try {
                    var result = await _context.SaveChangesAsync();

                    await _context.Database.ExecuteSqlRawAsync("call sp_RecalculateProfileMatch({0})", user.Id);


                    return result == 0 ? StatusCode(StatusCodes.Status500InternalServerError) :
                        state == EntityState.Added ? StatusCode(StatusCodes.Status201Created, response) :
                        response;
                } catch (Exception ex) {
                    throw;
                }
            }
                
            return Ok("No change made");
        }

        #endregion

    }
}
