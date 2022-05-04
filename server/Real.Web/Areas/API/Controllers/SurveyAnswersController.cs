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
    public class SurveyAnswersController : Controller {
        private readonly CapstoneContext _context;

        public SurveyAnswersController(CapstoneContext context) {
            _context = context;
        }

        // GET: api/SurveyAnswers
        /// <summary>
        /// Gets all survey answers
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyAnswer>>> GetSurveyAnswers() {
            var query = _context.SurveyAnswers.AsQueryable();
            
            return Ok(await query.ToListAsync());
        }

        // GET: api/SurveyAnswers/5
        /// <summary>
        /// Get answer by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActionResult<SurveyAnswer>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<SurveyAnswer>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<SurveyAnswer>> GetSurveyAnswer(int id) {
            var surveyAnswer = await _context.SurveyAnswers.FindAsync(id);

            if (surveyAnswer == null)
                return NotFound();

            return Ok(surveyAnswer);
        }

        // GET: api/SurveyAnswers/ForQuestion/5
        /// <summary>
        /// Get answers by question ID
        /// </summary>
        [HttpGet("ForQuestion/{id}")]
        [Produces("application/json", Type = typeof(ActionResult<IEnumerable<SurveyAnswer>>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<IEnumerable<SurveyAnswer>>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<IEnumerable<SurveyAnswer>>> GetSurveyAnswersForQuestion(int id) {
            var surveyAnswers = await _context.SurveyAnswers
                .Where(x => x.SurveyQuestionId == id)
                .ToListAsync();

            if (surveyAnswers == null)
                return NotFound();

            return Ok(surveyAnswers);
        }

        // PUT: api/SurveyAnswers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update survey question
        /// </summary>
        [HttpPatch("{id}")]
        [Consumes("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = null)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> PatchSurveyAnswer(int id, SurveyAnswer surveyAnswer) {
            if (id != surveyAnswer.Id)
                return BadRequest();

            _context.Entry(surveyAnswer).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!SurveyAnswerExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/SurveyAnswers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create new survey question
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<SurveyAnswer>))]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ActionResult<SurveyAnswer>))]
        public async Task<ActionResult<SurveyAnswer>> PostSurveyAnswer(SurveyAnswer surveyAnswer) {
            _context.SurveyAnswers.Add(surveyAnswer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSurveyAnswer", new { id = surveyAnswer.Id }, surveyAnswer);
        }

        // DELETE: api/SurveyAnswers/5
        /// <summary>
        /// Deletes survey answer
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> DeleteSurveyAnswer(int id) {
            var surveyAnswer = await _context.SurveyAnswers.FindAsync(id);
            if (surveyAnswer == null)
                return NotFound();

            _context.SurveyAnswers.Remove(surveyAnswer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyAnswerExists(int id) {
            return _context.SurveyAnswers.Any(e => e.Id == id);
        }
    }
}
