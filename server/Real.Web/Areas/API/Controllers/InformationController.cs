using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("API/[controller]/[action]")]
    public class InformationController : Controller {
        private readonly CapstoneContext _db;

        public InformationController(CapstoneContext context) {
            _db = context;
        }


        /// <summary>
        /// Get information about database table structure and column information
        /// </summary>
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(IEnumerable<Model.InformationSchema.Table>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Model.InformationSchema.Table>))]
        [HttpGet]
        public async Task<IActionResult> DatabaseSchema() {
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

            return Ok(tables);
        }

    }
}
