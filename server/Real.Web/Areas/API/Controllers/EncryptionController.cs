using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    public class EncryptionController : Controller {

        internal readonly IConfiguration Configuration;
        internal readonly SymmetricAlgorithm algorithm;

        public EncryptionController(IConfiguration config) {
            Configuration = config;

            var key = Configuration["encryption:key"];
            var iv = Configuration["encryption:iv"];

            algorithm = Aes.Create();
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;
            algorithm.Key = Convert.FromBase64String(key);
            algorithm.IV = Convert.FromBase64String(iv);
        }

        internal SymmetricAlgorithm GetAlgorithm() {
            return algorithm;
        }

        /// <summary>
        /// Encrypts a plain-text string to a base-64 string
        /// </summary>
        [HttpGet("encrypt")]
        [Consumes("text/plain")]
        [Produces("text/plain", Type = typeof(ActionResult<string>))]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public ActionResult<string> EncryptString(string value) {
            if (String.IsNullOrEmpty(value))
                return BadRequest();
            
            using (var encryptor = algorithm.CreateEncryptor()) // algorithm.Key, algorithm.IV
            using (var ms = new MemoryStream()) {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                    writer.Write(value);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Decrypts a base-64 string to a plain-text string
        /// </summary>
        [HttpGet("decrypt")]
        [Consumes("text/plain")]
        [Produces("text/plain", Type = typeof(ActionResult<string>))]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Decrypt(string value) {
            if (String.IsNullOrEmpty(value))
                return BadRequest();
                
            using (var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            using (var ms = new MemoryStream(Convert.FromBase64String(value)))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
                return reader.ReadToEnd();
        }

    }
}
