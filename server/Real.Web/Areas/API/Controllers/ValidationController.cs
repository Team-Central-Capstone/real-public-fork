using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Real.Data.Contexts;
using Real.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Controllers {

    [ApiController]
    [Area("API")]
    [Route("API/[controller]/[action]")]
    public class ValidationController : Controller {
        // private readonly CapstoneContext _context;
        private readonly DateTime _MinimumBirthdate = DateTime.Now.AddYears(-18);

        public ValidationController(/*CapstoneContext context*/) {
            // _context = context;
        }

        /// <summary>
        /// Verifies that the given string is not null or empty
        /// </summary>
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<bool>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<bool>))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, Type = typeof(ActionResult<bool>))]
        public IActionResult StringNotNull(string value) {
            var result = !String.IsNullOrEmpty(value);
            return result ? Ok(result) : StatusCode(StatusCodes.Status406NotAcceptable, result);
        }

        /// <summary>
        /// Verifies that the age of the person with the given date is 18 or older
        /// </summary>
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<bool>))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ActionResult<bool>))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, Type = typeof(ActionResult<bool>))]
        public ActionResult<bool> AtLeast18YearsOld(DateTime? value) {
            if (!value.HasValue) {
                var param = Request.Query.Single().Value;
                if (!DateTime.TryParse(param.ToString(), out DateTime parsed))
                    parsed = DateTime.Now;
                value = parsed;
            }

            var result = value < _MinimumBirthdate;

            return result ? result : StatusCode(StatusCodes.Status406NotAcceptable, result);
        }

    }
}
