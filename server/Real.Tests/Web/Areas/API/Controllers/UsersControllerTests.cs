// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.Json;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Abstractions;
// using Microsoft.AspNetCore.Mvc.Routing;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Real.Data.Contexts;
// using Real.Web.Areas.API.Controllers;

// namespace Real.Tests.Web.Areas.API.Controllers {

//     [TestClass]
//     public class UsersControllerTests {
        
//         private static CapstoneContext _context;
//         private static ValidationController _validationController;
//         private static UsersController _controller;
//         private static string _table => _context.Model.FindEntityType(typeof(Real.Model.User)).GetTableName();

//         [ClassInitialize]
//         public static void ClassInitialize(TestContext context) {
            
//         }

//         [TestInitialize]
//         public void TestInitialize() {
//             var cn = new SqliteConnectionStringBuilder {
//                 DataSource = ":memory:",
//                 Cache = SqliteCacheMode.Shared,
//             };
//             var connection = new SqliteConnection(cn.ToString());

//             connection.Open(); // inmemory database has to be opened

//             var builder = new DbContextOptionsBuilder<CapstoneContext>();
//             builder.UseSqlite(connection);

//             _context = new CapstoneContext(builder.Options);
//             _validationController = new ValidationController();
//             _controller = new UsersController(_context, _validationController);

//             Assert.IsNotNull(_context);
//             Assert.IsNotNull(_controller);
//         }

//         [DataTestMethod]
//         [DataRow("1983-08-19", StatusCodes.Status204NoContent)]
//         [DataRow("2005-01-01", StatusCodes.Status406NotAcceptable)]
//         public async Task ValidateAge(string birthdate, int expected) {
//             var user = new Real.Model.User {
//                 FirebaseUserId = Guid.NewGuid().ToString(),
//             };
//             _context.Users.Add(user);
//             await _context.SaveChangesAsync();

//             var model = new Real.Web.Areas.API.Models.UserCreateViewModel {
//                 FirebaseUserId = user.FirebaseUserId,
//                 Birthdate = DateTime.Parse(birthdate),
//             };

//             var rawResult = await _controller.PatchUserAsync(model.FirebaseUserId, model);
//             var actual =
//                 (rawResult.Result as StatusCodeResult)?.StatusCode ??
//                 (rawResult.Result as ObjectResult)?.StatusCode ?? -1;

//             Assert.AreEqual(expected, actual);
//         }
        

//     }
    
// }
