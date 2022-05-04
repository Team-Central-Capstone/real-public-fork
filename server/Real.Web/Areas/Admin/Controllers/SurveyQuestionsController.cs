using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Real.Data.Contexts;
using Real.Model;

namespace Real.Web.Areas.Admin.Controllers {

    [Area("Admin")]
    public class SurveyQuestionsController : Controller {
        private readonly CapstoneContext _context;

        public SurveyQuestionsController(CapstoneContext context)
        {
            _context = context;
        }

        // GET: Admin/SurveyQuestions
        public async Task<IActionResult> Index()
        {
            var results = await _context.SurveyQuestions
                .Include(x => x.Answers)
                .OrderBy(x => x.Order)
                .ToListAsync();
            results.ForEach(x => x.Answers = x.Answers.OrderBy(x => x.Id).ToList());
            return View(results);
        }

        // GET: Admin/SurveyQuestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surveyQuestion == null)
            {
                return NotFound();
            }

            return View(surveyQuestion);
        }

        // GET: Admin/SurveyQuestions/Create
        public IActionResult Create()
        {
            return View("Edit", new SurveyQuestion());
        }

        // POST: Admin/SurveyQuestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SurveyQuestion surveyQuestion) {
            return RedirectToAction("Edit");
        }

        // GET: Admin/SurveyQuestions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestions
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (surveyQuestion == null)
            {
                return NotFound();
            }
            return View(surveyQuestion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder(Dictionary<int,int> items) {
            var sql = new StringBuilder();

            foreach (var item in items) {
                sql.AppendLine($"update `SurveyQuestions` set `Order`={item.Value} where `Id`={item.Key};");
            }

            var result = await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return Ok();
        }

        // POST: Admin/SurveyQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SurveyQuestion surveyQuestion)
        {
            if (id != surveyQuestion.Id) {
                return NotFound();
            }

            // if the question type has changed, we need to remove answers and revalidate
            if (surveyQuestion.QuestionType != QuestionType.SingleChoice && surveyQuestion.QuestionType != QuestionType.MultipleChoice) {
                if (surveyQuestion.Answers.Any()) {
                    foreach (var answer in surveyQuestion.Answers) {
                        _context.Entry(answer).State = EntityState.Deleted;
                    }
                    surveyQuestion.Answers.Clear();

                    ModelState.Clear();
                    TryValidateModel(surveyQuestion);
                }
            }

            if (ModelState.IsValid) {
                
                // check for answers to remove
                if (surveyQuestion.Answers.Any(x => x.Deleted)) {
                    foreach (var deletedAnswer in surveyQuestion.Answers.Where(x => x.Deleted)) {
                        _context.Entry(deletedAnswer).State = EntityState.Deleted;
                    }
                }

                if (surveyQuestion.Order == 0) {
                    var max = await _context.SurveyQuestions.MaxAsync(x => (int?)x.Order);
                    surveyQuestion.Order = 1 + (max ?? 0);
                }

                try {
                    _context.Update(surveyQuestion);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!SurveyQuestionExists(surveyQuestion.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            } else  {

                var errors = ModelState
                    .Where(x => x.Value.Errors.Any())
                    .Select(x => new { 
                        x.Key, 
                        x.Value.Errors,
                        ErrorMessage = x.Value.Errors.First().ErrorMessage,
                    });

                foreach (var error in errors) {
                    ModelState.AddModelError(error.Key, error.ErrorMessage);
                }
            }


            return View(surveyQuestion);
        }

        // GET: Admin/SurveyQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surveyQuestion == null)
            {
                return NotFound();
            }

            return View(surveyQuestion);
        }

        // POST: Admin/SurveyQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surveyQuestion = await _context.SurveyQuestions.FindAsync(id);
            _context.SurveyQuestions.Remove(surveyQuestion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyQuestionExists(int id)
        {
            return _context.SurveyQuestions.Any(e => e.Id == id);
        }
    }
}
