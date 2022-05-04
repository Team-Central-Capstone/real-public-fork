using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Real.Data.Contexts;
using Real.Model;

namespace Real.Web.Areas.Admin.Controllers {

    [Area("Admin")]
    public class DatabaseController : Controller {
        private readonly CapstoneContext _db;

        public DatabaseController(CapstoneContext context) {
            _db = context;
        }

        public async Task<IActionResult> Index() {
            var tables = await _db.InformationSchema_Tables
                .OrderBy(x => x.TABLE_TYPE)
                .ThenBy(x => x.TABLE_NAME)
                .ToListAsync();
            var columns = await _db.InformationSchema_Columns
                .OrderBy(x => x.TABLE_NAME)
                .ThenBy(x => x.ORDINAL_POSITION)
                .Where(x => tables.Select(y => y.TABLE_NAME).Contains(x.TABLE_NAME))
                .ToListAsync();
            tables.ForEach(x => x.Columns = columns.Where(y => y.TABLE_NAME == x.TABLE_NAME).ToList() );

            return View(tables);
        }

    }
}
