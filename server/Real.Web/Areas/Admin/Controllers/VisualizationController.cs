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
using Real.Web.Areas.Admin.Models.Visualization;

namespace Real.Web.Areas.Admin.Controllers {

    [Area("Admin")]
    public class VisualizationController : Controller {
        private readonly CapstoneContext _context;

        public VisualizationController(CapstoneContext context)
        {
            _context = context;
        }

        // GET: Admin/SurveyQuestions
        public async Task<IActionResult> Index() {

            var uid = this.GetUserFirebaseId() ?? "109409728062644297175";
            var model = new IndexViewModel {
                User = await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUserId == uid),
            };

            var realUserNames = await _context.Users
                .Where(x => !x.FirebaseUserId.StartsWith("testuser"))
                .Select(x => new {
                    x.FirebaseUserId,
                    x.FirstName,
                    x.LastName,
                })
                .ToListAsync();
            // var fakeUserNames = await _context.Locations
            //     .Where(x => x.FirebaseUserId.StartsWith("testuser"))
            //     .Select(x => x.FirebaseUserId)
            //     .Distinct()
            //     .Where(x => CapstoneContext.Rand() < 0.05)
            //     .OrderByDescending(x => x)
            //     .ToListAsync();

            model.Users.AddRange(realUserNames.Select(x => (x.FirebaseUserId, x.FirstName + " " + x.LastName)));
            // model.Users.AddRange(fakeUserNames.Select(x => (x, x)));

            return View(model);
        }

    }
}
