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
    public class SurveyQuestionsController : Controller {
        private readonly CapstoneContext _context;

        public SurveyQuestionsController(CapstoneContext context) {
            _context = context;
        }

        // GET: api/SurveyQuestions/Types
        /// <summary>
        /// Get a list of possible question types
        /// </summary>
        [HttpGet("Types")]
        [Produces("application/json", Type = typeof(IEnumerable<QuestionType>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<QuestionType>))]
        public ActionResult<IEnumerable<QuestionType>> GetSurveyQuestionTypes() {
            var types = Enum.GetValues<QuestionType>()
                .Cast<QuestionType>()
                .Select(x => new {
                    Name = x.ToString(),
                    Id = (int)x
                })
                .ToList();
                // .ToDictionary(x => x.ToString(), x => (int)x);
            return Ok(types);
        }

        // GET: api/SurveyQuestions
        /// <summary>
        /// Gets a list of possible survey questions, with answers
        /// </summary>
        [HttpGet]
        [Produces("application/json", Type = typeof(ActionResult<IEnumerable<SurveyQuestion>>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<IEnumerable<SurveyQuestion>>))]
        public async Task<ActionResult<IEnumerable<SurveyQuestion>>> GetSurveyQuestions() {
            var query = _context.SurveyQuestions
                .Include(x => x.Answers)
                .OrderBy(x => x.Order);

            return Ok(await query.ToListAsync());
        }

        // GET: api/SurveyQuestions/5
        /// <summary>
        /// Get a single question with possible answers
        /// </summary>
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActionResult<SurveyQuestion>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<SurveyQuestion>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<ActionResult<SurveyQuestion>> GetSurveyQuestion(int id) {
            var surveyQuestion = await _context.SurveyQuestions
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (surveyQuestion == null)
                return NotFound();

            return Ok(surveyQuestion);
        }

        // PUT: api/SurveyQuestions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates a question
        /// </summary>
        [HttpPatch("{id}")]
        [Consumes("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = null)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> PatchSurveyQuestion(int id, SurveyQuestion surveyQuestion) {
            if (id != surveyQuestion.Id)
                return BadRequest();

            _context.Entry(surveyQuestion).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!SurveyQuestionExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/SurveyQuestions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new question
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<SurveyQuestion>))]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ActionResult<SurveyQuestion>))]
        public async Task<ActionResult<SurveyQuestion>> PostSurveyQuestion(SurveyQuestion surveyQuestion) {
            _context.SurveyQuestions.Add(surveyQuestion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSurveyQuestion", new { id = surveyQuestion.Id }, surveyQuestion);
        }

        // DELETE: api/SurveyQuestions/5
        /// <summary>
        /// Deletes a survey question
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = null)]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = null)]
        public async Task<IActionResult> DeleteSurveyQuestion(int id) {
            var surveyQuestion = await _context.SurveyQuestions.FindAsync(id);
            if (surveyQuestion == null)
                return NotFound();

            _context.SurveyQuestions.Remove(surveyQuestion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyQuestionExists(int id) {
            return _context.SurveyQuestions.Any(e => e.Id == id);
        }
    }
}
