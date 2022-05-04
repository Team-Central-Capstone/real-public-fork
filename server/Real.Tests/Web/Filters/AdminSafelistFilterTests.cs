// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Reflection.Emit;
// using System.Text.Json;
// using System.Threading.Tasks;
// using ICSharpCode.Decompiler.CSharp;
// using ICSharpCode.Decompiler.Metadata;
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
// using Real.Web.Filters;

// namespace Real.Tests.Web.Filters {

//     [TestClass]
//     public class AdminSafelistFilterTests {
        
//         // private static CapstoneContext _context;
//         // private static ValidationController _validationController;
//         // private static UsersController _controller;

//         [ClassInitialize]
//         public static void ClassInitialize(TestContext context) {
            
//         }

//         [TestInitialize]
//         public void TestInitialize() {
//             // var cn = new SqliteConnectionStringBuilder {
//             //     DataSource = ":memory:",
//             //     Cache = SqliteCacheMode.Shared,
//             // };
//             // var connection = new SqliteConnection(cn.ToString());

//             // connection.Open(); // inmemory database has to be opened

//             // var builder = new DbContextOptionsBuilder<CapstoneContext>();
//             // builder.UseSqlite(connection);

//             // _context = new CapstoneContext(builder.Options);
//             // _validationController = new ValidationController();
//             // _controller = new UsersController(_context, _validationController);

//             // Assert.IsNotNull(_context);
//             // Assert.IsNotNull(_controller);
//         }

//         [TestMethod]
//         public void OnActionExecutingDoesNotContainAwait() {
//             var filter = new AdminSafelistFilter(new List<string>{}, null, null);
//             var methodInfo = filter.GetType().GetMethod(nameof(AdminSafelistFilter.OnActionExecuting));
            
//             // var file = Assembly.GetEntryAssembly().Location;
            
//             // var decompiler = new ICSharpCode.Decompiler.CSharp.CSharpDecompiler(file,
//             //     new UniversalAssemblyResolver(file, true, ".NETCOREAPP"),
//             //     new ICSharpCode.Decompiler.DecompilerSettings {

//             //     }
//             // );
            
//             Assert.Fail();
//         }
        

//     }
    
// }
