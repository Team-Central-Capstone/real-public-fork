using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Real.Data.Contexts;
using Real.Model;
using Real.Web.Areas.API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Controllers {

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Area("API")]
    [Route("API/[controller]")]
    public class AnalyticsController : Controller {
        private readonly CapstoneContext _context;
        private readonly IConfiguration _config;

        public AnalyticsController(CapstoneContext context, IConfiguration config) {
            _context = context;
            _config = config;
        }        

        [HttpGet("errors/{page}")]
        [Produces("application/json", Type = typeof(IEnumerable<Analytic>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Analytic>))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, Type = null)]
        [SwaggerResponse(StatusCodes.Status416RequestedRangeNotSatisfiable, Type = null)]
        public async Task<ActionResult<IEnumerable<Analytic>>> GetErrorsAsync(int page = 0) {
            const int PageSize = 100;

            if (page < 0)
                return StatusCode(StatusCodes.Status406NotAcceptable, "page cannot be less than 0");
            
            var results = await _context.Analytics
                .Include(x => x.AnalyticDetail)
                .Include(x => x.AnalyticError)
                .Where(x => x.AnalyticError != null)
                .OrderByDescending(x => x.StartTimestamp)
                .Skip(page * PageSize)
                .Take(PageSize)
                .ToListAsync();
            
            if (results.Count == 0)
                return StatusCode(StatusCodes.Status416RequestedRangeNotSatisfiable, "page out of range");
            
            return results;
        }

        [HttpGet("{page}")]
        [Produces("application/json", Type = typeof(IEnumerable<Analytic>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Analytic>))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, Type = null)]
        [SwaggerResponse(StatusCodes.Status416RequestedRangeNotSatisfiable, Type = null)]
        public async Task<ActionResult<IEnumerable<Analytic>>> GetAsync(int page = 0) {
            const int PageSize = 100;

            if (page < 0)
                return StatusCode(StatusCodes.Status406NotAcceptable, "page cannot be less than 0");
            
            var results = await _context.Analytics
                .Include(x => x.AnalyticDetail)
                .Include(x => x.AnalyticError)
                .OrderByDescending(x => x.StartTimestamp)
                .Skip(page * PageSize)
                .Take(PageSize)
                .ToListAsync();
            
            if (results.Count == 0)
                return StatusCode(StatusCodes.Status416RequestedRangeNotSatisfiable, "page out of range");
            
            return results;
        }

    }
}
