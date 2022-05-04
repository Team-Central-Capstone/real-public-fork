// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
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
//     public class LocationControllerTests {
        
//         private static CapstoneContext _context;
//         private static LocationController _controller;
//         private static string _table => _context.Model.FindEntityType(typeof(Real.Model.Location)).GetTableName();

//         [ClassInitialize]
//         public static void _ClassInitialize(TestContext context) {
//             Console.WriteLine("_ClassInitialize");
//         }

//         [TestInitialize]
//         public void TestInitialize() {
//             Console.WriteLine("TestInitialize");
//             var cn = new SqliteConnectionStringBuilder {
//                 DataSource = ":memory:",
//                 Cache = SqliteCacheMode.Shared,
//             };
//             var connection = new SqliteConnection(cn.ToString());

//             connection.Open(); // inmemory database has to be opened

//             var builder = new DbContextOptionsBuilder<CapstoneContext>();
//             builder.UseSqlite(connection);


//             _context = new CapstoneContext(builder.Options);
//             _controller = new LocationController(_context);

//             Assert.IsNotNull(_context);
//             Assert.IsNotNull(_controller);
//         }

//         // [DataTestMethod]
//         // [DataRow]
//         [TestMethod]
//         public void CheckMPHCalculation() {
//             const double tolerance = 0.001;

//             var prevTimestamp = DateTime.Parse("2022-01-25 02:46:30.970550");
//             var prevLat = 41.784625149023825;
//             var prevLon = -72.51954092034133;
            
//             var timestamp = DateTime.Parse("2022-01-25 02:46:32.125792");
//             var lat = 41.7845295415311;
//             var lon = -72.52000800920989;

//             var dMi = DistanceBetweenPoints.Miles(lat, lon, prevLat, prevLon);
//             var mph = DistanceBetweenPoints.MPH(lat, lon, prevLat, prevLon, timestamp, prevTimestamp);

//             Assert.AreEqual(0.024954492147294685, dMi);
//             Assert.AreEqual(77.76394186695155, mph);
//         }

//         [TestMethod]
//         public async Task RecordLocation() {
//             const string id = "109409728062644297175";

//             var _startTime = DateTime.UtcNow;
//             var _data = new [] {
//                 (41.78119143588308, -72.57641584897699, _startTime),
//                 (41.78434353046944, -72.57624418760906, _startTime.AddSeconds(30)),
//                 (41.7886199044578, -72.57405741569956, _startTime.AddSeconds(60)),
//                 (41.78361617981973, -72.58320629119373, _startTime.AddSeconds(90))
//             };
//             var _locations = _data.Select(x => new Real.Model.Location {
//                 FirebaseUserId = id,
//                 Latitude = x.Item1,
//                 Longitude = x.Item2,
//                 Timestamp = x.Item3,
//             }).ToList();

//             for (var i=1; i<_locations.Count; ++i) {
//                 _locations[i].SpeedFromLast = DistanceBetweenPoints.MPH(_locations[i], _locations[i-1]);
//             }

//             Console.WriteLine($"Speeds = [{String.Join(", ", _locations.Select(x => Math.Round(x.SpeedFromLast, 2)))}]");

//             await _context.Locations.AddRangeAsync(_locations);
//             await _context.SaveChangesAsync();

            
//             var models = new List<Real.Web.Areas.API.Models.RecordLocationModel> {
//                 new Real.Web.Areas.API.Models.RecordLocationModel {
//                     id = id,
//                     latitude = 1,
//                     longitude = 1,
//                     deviceid = "test"
//                 }
//             };

//             var result = await _controller.RecordLocationAsync(models);

//             Assert.IsNotNull(result);
//             Assert.IsNotNull(result.Value);
//             Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<Real.Model.Location>));
//             Assert.AreEqual(1, (result.Value as IEnumerable<Real.Model.Location>).Count());
            
//         }

//         [TestMethod]
//         public async Task RecordLocationBadModal() {
//             var result = await _controller.RecordLocationAsync(null);

//             Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
//         }

//         [DataTestMethod]
//         [DataRow(1)]
//         [DataRow(2)]
//         public async Task GetSingleLocation(int id) {

//             await _context.Locations.AddAsync(new Real.Model.Location {
//                 Id = id,
//             });
//             await _context.SaveChangesAsync();

//             var result = await _controller.GetAsync(id);
            
//             Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//             Assert.AreEqual(id, ((result as OkObjectResult)?.Value as Real.Model.Location)?.Id);
//         }

//         [TestMethod]
//         public async Task GetAsyncWithBadId() {
//             var result = await _controller.GetAsync(-1);
            
//             Assert.IsInstanceOfType(result, typeof(NotFoundResult));
//         }
        

//     }
    
// }
