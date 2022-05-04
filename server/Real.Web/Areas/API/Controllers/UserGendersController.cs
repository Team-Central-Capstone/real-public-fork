using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real.Data.Contexts;
using Real.Model;
using Real.Web.Areas.API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Controllers {

    [Area("API")]
    [Route("API/[controller]")]
    [ApiController]
    public class UserGendersController : Controller {
        private readonly CapstoneContext _context;

        public UserGendersController(CapstoneContext context) {
            _context = context;
        }

        // GET: api/UserGenders
        /// <summary>
        /// Gets a list of possible user genders
        /// </summary>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<UserGender>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserGender>))]
        public async Task<ActionResult<IEnumerable<UserGender>>> GetUserGenders() {
            var query = _context.UserGenders.AsQueryable();
            
            return await query.ToListAsync();
        }

        // GET: api/UserGenders/5
        /// <summary>
        /// Get a single gender by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActionResult<UserGender>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<UserGender>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<UserGender>> GetUserGender(int id) {
            var userGender = await _context.UserGenders.FindAsync(id);

            if (userGender == null)
                return NotFound();

            return userGender;
        }

        // PUT: api/UserGenders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates existing item
        /// </summary>
        [HttpPatch("{id}")]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(UserGender))]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> PatchUserGender(int id, UserGender userGender) {
            if (id != userGender.Id)
                return BadRequest();

            _context.Entry(userGender).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!UserGenderExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/UserGenders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create a new gender
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /API/UserGenders
        ///     {
        ///         "id": 1,
        ///         "name": "",
        ///     }
        /// </remarks>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<UserGender>))]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ActionResult<UserGender>))]
        public async Task<ActionResult<UserGender>> PostUserGender(UserGender userGender) {
            _context.UserGenders.Add(userGender);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserGender", new { id = userGender.Id }, userGender);
        }

        // DELETE: api/UserGenders/5
        /// <summary>
        /// Deletes an existing gender
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        public async Task<IActionResult> DeleteUserGender(int id) {
            var userGender = await _context.UserGenders.FindAsync(id);
            if (userGender == null)
                return NotFound();

            _context.UserGenders.Remove(userGender);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserGenderExists(int id) {
            return _context.UserGenders.Any(e => e.Id == id);
        }
    }
}
