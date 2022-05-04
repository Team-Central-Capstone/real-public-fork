using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Real.Data.Contexts;
using Real.Model;

namespace Real.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AnalyticsController : Controller
    {
        private readonly CapstoneContext _context;

        public AnalyticsController(CapstoneContext context)
        {
            _context = context;
        }

        // GET: Admin/Analytics
        public async Task<IActionResult> Index()
        {
            return View(await _context.Analytics.ToListAsync());
        }

        // GET: Admin/Analytics/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analytic = await _context.Analytics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (analytic == null)
            {
                return NotFound();
            }

            return View(analytic);
        }

        // GET: Admin/Analytics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Analytics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartTimestamp,EndTimestamp,Namespace,Area,Controller,Action,UserName,FirebaseUserId,IPv4,IPv6,Host,Path,QueryString,Data")] Analytic analytic)
        {
            if (ModelState.IsValid)
            {
                analytic.Id = Guid.NewGuid();
                _context.Add(analytic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(analytic);
        }

        // GET: Admin/Analytics/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analytic = await _context.Analytics.FindAsync(id);
            if (analytic == null)
            {
                return NotFound();
            }
            return View(analytic);
        }

        // POST: Admin/Analytics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,StartTimestamp,EndTimestamp,Namespace,Area,Controller,Action,UserName,FirebaseUserId,IPv4,IPv6,Host,Path,QueryString,Data")] Analytic analytic)
        {
            if (id != analytic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(analytic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnalyticExists(analytic.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(analytic);
        }

        // GET: Admin/Analytics/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analytic = await _context.Analytics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (analytic == null)
            {
                return NotFound();
            }

            return View(analytic);
        }

        // POST: Admin/Analytics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var analytic = await _context.Analytics.FindAsync(id);
            _context.Analytics.Remove(analytic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnalyticExists(Guid id)
        {
            return _context.Analytics.Any(e => e.Id == id);
        }
    }
}
