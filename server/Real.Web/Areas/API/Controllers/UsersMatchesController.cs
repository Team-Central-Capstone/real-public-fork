using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Real.Data.Contexts;
using Real.Model;
using Real.Web.Areas.API.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;
using System.Net.Http.Headers;

namespace Real.Web.Areas.API.Controllers {

    /// <summary>
    /// Handles matches between users
    /// </summary>
    [Area("API")]
    [Route("API/Matches")]
    [ApiController]
    public class UserMatchesController : Controller {
        private readonly CapstoneContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;


        #region Constructor

        public UserMatchesController(CapstoneContext context, IConfiguration config, IWebHostEnvironment env) {
            _context = context;
            _config = config;
            _env = env;
        }

        #endregion

        public class GetAllMatchesModel {

            [Required]
            public DateTime StartTime { get; set; }// = DateTime.UtcNow.AddHours(-1);

            [Required]
            public DateTime EndTime { get; set; }// = DateTime.UtcNow;

            [Required]
            public double MinMiles { get; set; }// = 100;

            [Required]
            public TimeSpan MinTime { get; set; }// = new TimeSpan(00, 05, 00);
        }


        /// <summary>
        /// Gets pending matches for the given user
        /// </summary>        
        [HttpGet("{userId}")]
        [Produces("application/json", Type = typeof(ActionResult<List<UserMatch>>))]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserMatch>>> GetAllMatchesAsync(string userId, [FromQuery]GetAllMatchesModel model) {
            
            if (String.IsNullOrEmpty(userId))
                return BadRequest("userId cannot be null");

            if (model.MinMiles <= 0)
                return BadRequest("MinMiles must be greater than 0");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == userId);
            if (user == null)
                return NotFound("User does not exist");

            var cn = _context.Database.GetDbConnection() as MySqlConnection;
            using (var cmd = cn.CreateCommand()) {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "call sp_CalculateMatches(@userId, @startTime, @endTime, @minMiles, @minTime);";
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@startTime", model.StartTime);
                cmd.Parameters.AddWithValue("@endTime", model.EndTime);
                cmd.Parameters.AddWithValue("@minMiles", model.MinMiles);
                cmd.Parameters.AddWithValue("@minTime", model.MinTime);

                try {
                    if (cn.State == System.Data.ConnectionState.Closed)
                        await cn.OpenAsync();
                } catch (Exception ex) {
                    if (cn.State != System.Data.ConnectionState.Open)
                        return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                }

                await cmd.ExecuteNonQueryAsync();
            }

            var matches = await _context.UserMatches
                .Include(x => x.User2)
                .Where(x => x.UserId1 == user.Id && !x.User1AcceptedDate.HasValue)
                .ToListAsync();            

            if (matches.Count > 0) {
                var env = _env.EnvironmentName.ToLower() == "development" ? "development" : "production";
                var key = _config[$"google:maps:{env}"];
                var client = new HttpClient();

                matches.ForEach(async x => {
                    x.User2 = new User {
                        FirebaseUserId = x.User2.FirebaseUserId
                    };

                    var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={x.MatchedLatitude},{x.MatchedLongitude}&key={key}";
                    try {
                        var response = await client.GetAsync(url);
                        var json = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(json);
                        var location = jsonObject["results"]
                            .OfType<JObject>()
                            .Where(x => x["geometry"]["location_type"].Value<string>() == "APPROXIMATE")
                            .Select(x => x["formatted_address"].Value<string>())
                            .FirstOrDefault();
                        x.MatchedLocationName = location;
                    } catch (Exception ex) {
                        x.MatchedLocationName = ex.Message;
                    }
                });
            }

            return matches;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost("{userId}/ApproveMatchWith/{secondId}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserMatchViewModel>> ApproveMatchAsync(string userId, string secondId) {

            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(secondId))
                return BadRequest("Both userIDs are requried");

            var user1 = await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == userId);
            if (user1 == null)
                return NotFound();
            
            var user2 = await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == secondId);
            if (user2 == null)
                return NotFound();

            
            var match = await _context.UserMatches
                .Where(x => (x.UserId1 == user1.Id && x.UserId2 == user2.Id))
                .SingleOrDefaultAsync();

            if (match != null)
                match.User1AcceptedDate = DateTime.UtcNow;
            
            if (match == null) {
                match = await _context.UserMatches
                    .Where(x => (x.UserId1 == user2.Id && x.UserId2 == user1.Id))
                    .SingleOrDefaultAsync();

                if (match != null)
                    match.User2AcceptedDate = DateTime.UtcNow;
            }

            if (match == null)
                return NotFound("users have not matched");

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            if (match.User1AcceptedDate.HasValue && match.User2AcceptedDate.HasValue) {
                await NotifyOfNewMatchAsync(user1.FirebaseUserId, user2.FirebaseUserId);
            }

            return NoContent();
        }


        /// <summary>
        /// Gets previously approved matches
        /// </summary>        
        [HttpGet("{userId}/AllApproved")]
        [Produces("application/json", Type = typeof(ActionResult<List<UserMatch>>))]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserMatch>>> GetApprovedMatches(string userId) {
            
            if (String.IsNullOrEmpty(userId))
                return BadRequest("userId cannot be null");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == userId);
            if (user == null)
                return NotFound("User does not exist");

            
            var matches = await _context.UserMatches
                .Where(x => x.UserId1 == user.Id)
                .OrderByDescending(x => x.User1AcceptedDate)
                .ToListAsync();            

            return matches;
        }


        /// <summary>
        /// 
        /// </summary>
        [HttpPost("Notify/{firstId}/{secondId}")]
        [Produces("application/json", Type = typeof(ActionResult<object>))]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> NotifyOfNewMatchAsync(string firstId, string secondId) {

            var client = new HttpClient();
            var key = _config["firebase:fcm"];
            var url = $"https://fcm.googleapis.com/fcm/send";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try {
                var body = $@"{{
                    ""to"": ""/topics/{firstId}"",
                    ""priority"": ""high"",
                    ""notification"": {{
                        ""title"": ""New match!"",
                        ""body"": ""You have a new match! Check the app to see more!"",
                        ""data"": {{
                            ""firstId"": ""{firstId}"",
                            ""secondId"": ""{secondId}""
                        }}
                    }}
                }}";

                var response = await client.PostAsync(url, new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
                var json = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(json);

                return json;

            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }

        

    }
}
