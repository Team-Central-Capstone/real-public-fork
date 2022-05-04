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
    public class UserBodyTypesController : Controller {
        private readonly CapstoneContext _context;

        public UserBodyTypesController(CapstoneContext context) {
            _context = context;
        }

        // GET: api/UserBodyTypes
        /// <summary>
        /// Gets a list of possible user body types
        /// </summary>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<UserBodyType>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserBodyType>))]
        public async Task<ActionResult<IEnumerable<UserBodyType>>> GetUserBodyTypes() {
            var query = _context.UserBodyTypes.AsQueryable();
            
            return await query.ToListAsync();
        }

        // GET: api/UserBodyTypes/5
        /// <summary>
        /// Get a single body type by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActionResult<UserBodyType>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<UserBodyType>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<UserBodyType>> GetUserBodyType(int id) {
            var UserBodyType = await _context.UserBodyTypes.FindAsync(id);

            if (UserBodyType == null)
                return NotFound();

            return UserBodyType;
        }

        // PUT: api/UserBodyTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates existing item
        /// </summary>
        [HttpPatch("{id}")]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(UserBodyType))]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> PatchUserBodyType(int id, UserBodyType UserBodyType) {
            if (id != UserBodyType.Id)
                return BadRequest();

            _context.Entry(UserBodyType).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!UserBodyTypeExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/UserBodyTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create a new bdoy type
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /API/UserBodyTypes
        ///     {
        ///         "name": "",
        ///     }
        /// </remarks>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<UserBodyType>))]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ActionResult<UserBodyType>))]
        public async Task<ActionResult<UserBodyType>> PostUserBodyType(UserBodyType UserBodyType) {
            _context.UserBodyTypes.Add(UserBodyType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserBodyType", new { id = UserBodyType.Id }, UserBodyType);
        }

        // DELETE: api/UserBodyTypes/5
        /// <summary>
        /// Deletes an existing body type
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        public async Task<IActionResult> DeleteUserBodyType(int id) {
            var UserBodyType = await _context.UserBodyTypes.FindAsync(id);
            if (UserBodyType == null)
                return NotFound();

            _context.UserBodyTypes.Remove(UserBodyType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserBodyTypeExists(int id) {
            return _context.UserBodyTypes.Any(e => e.Id == id);
        }
    }
}
