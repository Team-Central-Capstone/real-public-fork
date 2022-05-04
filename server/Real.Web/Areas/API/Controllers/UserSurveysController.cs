// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Real.Data.Contexts;
// using Real.Model;

// namespace Real.Web.Areas.API.Controllers {

//     [Area("API")]
//     [Route("API/[controller]")]
//     [ApiController]
//     public class UserSurveysController : Controller {
//         private readonly CapstoneContext _context;

//         public UserSurveysController(CapstoneContext context) {
//             _context = context;
//         }

//         // GET: api/UserSurveys/ByUser/5
//         [HttpGet("ByUser/{userId}")]
//         public async Task<ActionResult<IEnumerable<UserSurveyResponse>>> GetUserSurveyResponsesForUser(int userId) {
//             return await _context.UserSurveyResponses
//                 .Include(x => x.SurveyQuestion)
//                 .Include(x => x.SurveyAnswers)
//                 .Where(x => x.UserId == userId)
//                 .ToListAsync();
//         }



//         // GET: api/UserSurveys
//         [HttpGet]
//         public async Task<ActionResult<IEnumerable<UserSurveyResponse>>> GetUserSurveyResponses()
//         {
//             return await _context.UserSurveyResponses.ToListAsync();
//         }

//         // GET: api/UserSurveys/5
//         [HttpGet("{id}")]
//         public async Task<ActionResult<UserSurveyResponse>> GetUserSurveyResponse(int id)
//         {
//             var userSurveyResponse = await _context.UserSurveyResponses.FindAsync(id);

//             if (userSurveyResponse == null)
//             {
//                 return NotFound();
//             }

//             return userSurveyResponse;
//         }

//         // PUT: api/UserSurveys/5
//         // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//         [HttpPut("{id}")]
//         public async Task<IActionResult> PutUserSurveyResponse(int id, UserSurveyResponse userSurveyResponse)
//         {
//             if (id != userSurveyResponse.Id)
//             {
//                 return BadRequest();
//             }

//             _context.Entry(userSurveyResponse).State = EntityState.Modified;

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!UserSurveyResponseExists(id))
//                 {
//                     return NotFound();
//                 }
//                 else
//                 {
//                     throw;
//                 }
//             }

//             return NoContent();
//         }

//         // POST: api/UserSurveys
//         // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//         [HttpPost]
//         public async Task<ActionResult<UserSurveyResponse>> PostUserSurveyResponse(UserSurveyResponse userSurveyResponse)
//         {
//             _context.UserSurveyResponses.Add(userSurveyResponse);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction("GetUserSurveyResponse", new { id = userSurveyResponse.Id }, userSurveyResponse);
//         }

//         // DELETE: api/UserSurveys/5
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteUserSurveyResponse(int id)
//         {
//             var userSurveyResponse = await _context.UserSurveyResponses.FindAsync(id);
//             if (userSurveyResponse == null)
//             {
//                 return NotFound();
//             }

//             _context.UserSurveyResponses.Remove(userSurveyResponse);
//             await _context.SaveChangesAsync();

//             return NoContent();
//         }

//         private bool UserSurveyResponseExists(int id)
//         {
//             return _context.UserSurveyResponses.Any(e => e.Id == id);
//         }
//     }
// }
