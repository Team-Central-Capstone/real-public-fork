using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real.Data.Contexts;
using Real.Model;
using Real.Web.Areas.API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Controllers {

    [ApiController]
    [Area("API")]
    [Route("API/[controller]")]
    public class LocationController : Controller {
        private readonly CapstoneContext _context;

        public LocationController(CapstoneContext context) {
            _context = context;
        }
        

        /// <summary>
        /// Gets location record with given ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /API/Location/{id}
        /// 
        /// </remarks>
        [HttpGet("{id?}")]
        [Produces("application/json", Type = typeof(Location))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Location))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> GetAsync(int? id) {
            if (!id.HasValue)
                return BadRequest();

            var item = await _context.Locations.FindAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Get a list of possible location source types
        /// </summary>
        [HttpGet("locationsources")]
        [Produces("application/json", Type = typeof(IEnumerable<LocationSource>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<LocationSource>))]
        public ActionResult<IEnumerable<LocationSource>> GetLocationSourceTypes() {
            var types = Enum.GetValues<LocationSource>()
                .Cast<LocationSource>()
                .Select(x => new {
                    Name = x.ToString(),
                    Id = (int)x
                })
                .ToList();
                // .ToDictionary(x => x.ToString(), x => (int)x);
            return Ok(types);
        }

        /// <summary>
        /// Records 1 or more new location for user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /API/Location
        ///     [
        ///         {
        ///             "id": "firebase UID",
        ///             "latitude": 0.00,
        ///             "longitude": 0.00,
        ///             "deviceid": ""
        ///         },
        ///         {
        ///             "id": "firebase UID",
        ///             "latitude": 0.00,
        ///             "longitude": 0.00
        ///             "deviceid": ""
        ///         }    
        ///     ]
        /// </remarks>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(IEnumerable<Location>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Location>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        public async Task<ActionResult<IEnumerable<Location>>> RecordLocationAsync([FromBody, SwaggerRequestBody(Required = true)]IEnumerable<RecordLocationModel> models) {
            try {
                if (models == null)
                    return BadRequest("Models cannot be null");
                
                if (models.Any(x => String.IsNullOrEmpty(x.id)))
                    return BadRequest("id (firebase UID) is required");

                if (models.Any(x => String.IsNullOrEmpty(x.deviceid)))
                    return BadRequest("deviceid is required");

                var result = models
                    .Select(x => new Location {
                        FirebaseUserId = x.id,
                        Timestamp = x.time ?? DateTime.UtcNow,
                        Latitude = x.latitude,
                        Longitude = x.longitude,
                        DeviceID = x.deviceid,
                    })
                    .OrderBy(x => x.Timestamp)
                    .ToList();

                await _context.Locations.AddRangeAsync(result);
                await _context.SaveChangesAsync();

                return result;
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
