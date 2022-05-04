using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
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
using Microsoft.Extensions.Logging;
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
    public class ExportController : Controller {
        private readonly CapstoneContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<ExportController> _logger;

        public ExportController(CapstoneContext context, IConfiguration config, ILogger<ExportController> logger) {
            _context = context;
            _config = config;
            _logger = logger;
        }        

        internal string _ScrubFileName(string file) {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (var c in invalidChars)
                file = file.Replace(c, '_');
            return file;
        }

        [HttpGet("SurveyResponses")]
        public async Task<IActionResult> ExportSurveyQuestionResponsesAsync() {

            var dt = new DataTable();

            using (var cn = _context.Database.GetDbConnection())
            using (var cmd = cn.CreateCommand()) {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                select
                    u.Id as 'UserId'
                    -- ,u.Birthdate
                    ,timestampdiff(year, u.Birthdate, current_timestamp()) as 'Age'
                    ,u.PreferredName
                    ,u.LastName
                    ,ug.Name as 'UserGender'
                    ,group_concat(distinct uag_ug.Name order by uag_ug.Id separator '|') as GendersAttractedTo
                    ,ubt.Name as 'UserBodyType'
                    ,u.HeightInches
                    ,sq.Id as 'QuestionId'
                    ,sq.QuestionText
                    ,usr.UserSurveyResponseWeight
                    ,group_concat(distinct coalesce(usr.SurveyAnswerResponse, sa.AnswerText) separator '|') as SurveyAnswerResponse
                from
                    Users u
                    left join UserGenders ug on u.UserGenderId = ug.Id
                    left join UserAttractedGenders uag on u.Id = uag.UserAttractedToId
                    left join UserGenders uag_ug on uag.GendersAttractedToId = uag_ug.Id
                    left join UserBodyTypes ubt on u.UserBodyTypeId = ubt.Id
                    inner join UserSurveyResponses usr on u.Id = usr.UserId
                    inner join SurveyQuestions sq on usr.SurveyQuestionId = sq.Id
                    left join SurveyAnswerUserSurveyResponse sausr on usr.Id = sausr.UserSurveyResponsesId
                    left join SurveyAnswers sa on sausr.SurveyAnswersId = sa.Id
                -- where u.Id in (12, 15)
                group by
                    u.Id
                    ,u.Birthdate
                    ,u.PreferredName
                    ,u.LastName
                    ,ug.Name
                    ,ubt.Name
                    ,u.HeightInches
                    ,sq.Id
                    ,sq.QuestionText
                    ,usr.UserSurveyResponseWeight
                order by sq.Id, u.Id;";

                if (cn.State != ConnectionState.Open)
                    await cn.OpenAsync();
                
                dt.Load(await cmd.ExecuteReaderAsync());
            }

            var questions = dt
                .AsEnumerable()
                .Select(x => new {
                    Id = x.Field<int>("QuestionId"),
                    Text = x.Field<string>("QuestionText")
                })
                .Distinct()
                .ToList();


            var files = new List<(string Name, string Content)>(); 
            
            foreach (var q in questions) {
                var b = new StringBuilder();

                b.AppendLine(String.Join(",", dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName)));
            
                foreach (DataRow r in dt.AsEnumerable().Where(x => x.Field<int>("QuestionId") == q.Id))
                    b.AppendLine(String.Join(",", r.ItemArray.Select(x => x is string ? $"\"{x}\"" : x)  ));
                files.Add((_ScrubFileName(q.Text) + ".csv", b.ToString()));
            }

            var bytes = (byte[])null;

            using (var ms = new MemoryStream()) {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true)) {
                    foreach (var file in files) {
                        using (var entry = archive.CreateEntry(file.Name, CompressionLevel.Fastest).Open())
                        using (var writer = new StreamWriter(entry))
                            writer.Write(file.Content);
                    }
                }

                ms.Seek(0, SeekOrigin.Begin);
                bytes = ms.ToArray();
            }
            
            Response.Headers.Add("Content-Disposition", "attachment;filename=export.zip");
            return new FileContentResult(bytes, "application/zip"); // , application/octet-stream


            // var b = new StringBuilder();
            // foreach (var q in questions) {
            //     b.AppendLine(String.Join(",", dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName)));
            //     b.AppendLine();

            //     foreach (DataRow r in dt.AsEnumerable().Where(x => x.Field<int>("QuestionId") == q.Id))
            //         b.AppendLine(String.Join(",", r.ItemArray));
            //     b.AppendLine();
            //     b.AppendLine();
            // }

            // var bytes = (byte[])null;
            // var filename = _ScrubFileName(questions.First().Text) + ".csv";

            // using (var ms = new MemoryStream()) {
            //     using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true)) {

            //         using (var entry = archive.CreateEntry(filename, CompressionLevel.Fastest).Open())
            //         using (var writer = new StreamWriter(entry))
            //             writer.Write(b.ToString());
                    

            //         // using (var es = entry.Open())
            //         // using (var sw = new StreamWriter(es)) {
            //         //     sw.Write(b.ToString());
            //         //     sw.Flush();
            //         // }
            //     }

            //     ms.Seek(0, SeekOrigin.Begin);
            //     bytes = ms.ToArray();
            // }
            
            // Response.Headers.Add("Content-Disposition", "attachment;filename=export.zip");
            // return new FileContentResult(bytes, "application/zip"); // , application/octet-stream

            // using (var ms = new MemoryStream())
            // using (var w = new StreamWriter(ms) as TextWriter) {
            //     w.Write(b.ToString());
            //     w.Flush();
            //     ms.Position = 0;
            //     bytes = ms.ToArray();
            // }

            // Response.Headers.Add("Content-Disposition", "attachment;filename=export.csv");
            // return new FileContentResult(bytes, "text/csv");

        }
    }
}
