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

    [ApiController]
    [Area("API")]
    [Route("API/[controller]")]
    public class EmailController : Controller {
        private readonly CapstoneContext _db;
        private readonly IConfiguration _config;

        public EmailController(CapstoneContext context, IConfiguration config) {
            _db = context;
            _config = config;
        }        

        /// <summary>
        /// Sends an email
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json", Type = typeof(ActionResult<bool>))]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> SendEmailAsync([FromBody]EmailViewModel model) {
            var host = _config["google:email:host"].ToString();
            var port = Int32.Parse(_config["google:email:port"].ToString());
            var pwd = _config["google:email:pwd"].ToString();
            var fromAddress = _config["google:email:fromAddress"].ToString();
            var fromName = _config["google:email:fromName"].ToString();
            var from = new MailAddress(fromAddress, fromName);
            var to = new MailAddress(model.Recipient);

            var smtp = new SmtpClient {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress, pwd),
            };

            using (var msg = new MailMessage(from, to)) {
                msg.Subject = model.Subject;
                msg.Body = model.Body;
                msg.IsBodyHtml = true;
                msg.Priority = model.Priority;

                try {
                    smtp.Send(msg);
                    await _db.Emails.AddAsync(new Email {
                        From = from.Address,
                        To = to.Address,
                        Subject = model.Subject,
                        Body = model.Body,
                    });
                    await _db.SaveChangesAsync();
                } catch (Exception ex) {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

            return true;
        }

    }
}
