
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySqlConnector;

namespace Real.Data.Contexts {

    public static partial class Extensions {

        public static IEnumerable<double> RollingAverage(this IEnumerable<double> source, int windowSize) {
            var queue = new Queue<double>(windowSize);

            foreach (double d in source) {
                if (queue.Count == windowSize)
                    queue.Dequeue();
                queue.Enqueue(d);
                yield return queue.Average();
            }
        }

        public static async Task<object> GenerateSeedLocationDataForExistingUsersAsync(this CapstoneContext _db) {
            const int MaxLocations = 10000;
            const int TimeVariabilityMinutes = 1;
            (double min, double max) Latitude = (41.383920730000604, 42.0420540561665);
            (double min, double max) Longitude = (-71.90395649891572, -73.48324099819017);
            (double min, double max) PositionVariablility = (0.000001, 0.01);
            
            var output = new StringBuilder();
            var log = (string x) => {
                output.AppendLine(x);
                Console.WriteLine(x);
            };

            var items = new List<Real.Model.Location>();

            var originalStartTime = DateTime.Now;

            // var nextId = await _db.Locations
            //     .FromSqlRaw("SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'capstone' AND TABLE_NAME = 'Locations';")
            //     .Select(x => x.Id)
            //     .FirstOrDefaultAsync();

            // var nextId = default(int);

            // var cn = _db.Database.GetDbConnection() as MySqlConnection;
            // using (var cmd = cn.CreateCommand()) {
            //     cmd.CommandType = CommandType.Text;
            //     cmd.CommandText = "SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'capstone' AND TABLE_NAME = 'Locations';";

            //     if (cn.State == ConnectionState.Closed)
            //         await cn.OpenAsync();

            //     var result = (ulong)(await cmd.ExecuteScalarAsync());

            //     if (result >= Int32.MaxValue)
            //         throw new OverflowException();

            //     nextId = (int)result;

            // }
            
            var userIds = await _db.Users
                .Where(x => x.RegisteredTimestamp == DateTime.Parse("2022-02-24 02:36:08"))
                .Where(x => !_db.Locations.Any(y => y.FirebaseUserId == x.FirebaseUserId))
                .OrderBy(x => x.Id)
                // .Select(x => x.FirebaseUserId)
                .ToListAsync();

            var stringFormat = $"#,{new String('0', ((long)userIds.Count * MaxLocations).ToString().Length)}";
            var r = new Random(Guid.NewGuid().GetHashCode());
            var startTime = DateTime.UtcNow;

            log($"GenerateSeedLocationDataForExistingUsersAsync: generating {MaxLocations} records each for {userIds.Count} users ({userIds.Count * MaxLocations} total rows)...");
            // log($"GenerateSeedLocationDataForExistingUsersAsync: Starting with location ID: {nextId}\n\n");

            foreach (var user in userIds) {
                var userId = user.FirebaseUserId;
                var t = startTime;
                (double lat, double lon) position = (0,0);
                var locations = new List<Real.Model.Location>();

                log($"GenerateSeedLocationDataForExistingUsersAsync: {(DateTime.UtcNow - startTime)} Inserting {MaxLocations} locations for {user.PreferredName} {user.LastName} (ID {user.Id})");

                items.Clear();

                for (var i=0; i<MaxLocations; ++i) {
                    if (position.lat == 0) {
                        var lat = r.NextDouble() * (Latitude.max - Latitude.min) + Latitude.min;
                        var lon = r.NextDouble() * (Longitude.max - Longitude.min) + Longitude.min;
                        position = (lat, lon);
                    } else {
                        if (r.Next()%2 == 0) {
                            var v = (
                                r.NextDouble() * (PositionVariablility.max - PositionVariablility.min) + PositionVariablility.min,
                                r.NextDouble() * (PositionVariablility.max - PositionVariablility.min) + PositionVariablility.min
                            );

                            position.lat += (r.Next()%2 == 0) ? v.Item1 : -v.Item1;
                            position.lon += (r.Next()%2 == 0) ? v.Item2 : -v.Item2;
                        }
                    }

                    var l = new Model.Location {
                        // Id = ++nextId,
                        FirebaseUserId = userId,
                        Latitude = position.lat,
                        Longitude = position.lon,
                        Timestamp = t,
                    };
                    locations.Add(l);

                    t = t.AddMinutes(TimeVariabilityMinutes);
                }

                for (var i=1; i<locations.Count; ++i) {
                    var skip = i - 10 < 0 ? 0 : i - 10;
                    var take = skip > 0 ? 10 : i;
                    var speed = Model.DistanceBetweenPoints.MPH(locations[i], locations[i-1]);
                    var speeds = locations
                        .Select(x => x.SpeedFromLast)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                    var average = speeds.Average();

                    locations[i].SpeedFromLast = speed;
                    locations[i].RollingAverageSpeed = average;
                }

                items.AddRange(locations);

                await _db.Locations.AddRangeAsync(items);
                await _db.SaveChangesAsync();
            }

            // log("inserting data through EF...");
            // await _db.Locations.AddRangeAsync(items);
            // await _db.SaveChangesAsync();

            // log("GenerateSeedLocationDataForExistingUsersAsync: generating DataTable...");

            // var dt = new DataTable();
            // var properties = typeof(Model.Location).GetProperties();
            // foreach (var prop in properties) {
            //     if (!Attribute.IsDefined(prop, typeof(NotMappedAttribute))) {
            //         dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            //     }
            // }

            // foreach (var item in items) {
            //     var row = dt.NewRow();
            //     foreach (var prop in properties) {
            //         if (!Attribute.IsDefined(prop, typeof(NotMappedAttribute))) {
            //             row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            //         }
            //     }
            //     dt.Rows.Add(row);
            // }


            // var connectionString = _db.Database.GetConnectionString();
            // var csItems = connectionString.Split(';').ToList();
            // csItems.RemoveRange(4, csItems.Count - 4);
            // csItems.Add("AllowLoadLocalInfile=true");
            // connectionString = String.Join(';', csItems);

            // var expected = (long)userIds.Count * MaxLocations;
            // if (expected != items.LongCount() || expected != dt.Rows.Count) {
            //     throw new InvalidProgramException();
            // }
            
            // using (var connection = new MySqlConnection(connectionString)) {
            //     var bulk = new MySqlBulkCopy(connection);
            //     bulk.BulkCopyTimeout = 0;
            //     bulk.DestinationTableName = _db.Model.FindEntityType(typeof(Model.Location)).GetTableName();

            //     bulk.NotifyAfter = MaxLocations;
            //     bulk.MySqlRowsCopied += (sender, e) => {
            //         log($"GenerateSeedLocationDataForExistingUsersAsync: Copied {e.RowsCopied.ToString(stringFormat)} of {((long)userIds.Count * MaxLocations).ToString(stringFormat)} rows");
            //     };

            //     for (int col=0, j=0; col<properties.Length; ++col)
            //         if (!Attribute.IsDefined(properties[col], typeof(NotMappedAttribute))) {
            //             bulk.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(j++, properties[col].Name));
            //         }
                
            //     log("GenerateSeedLocationDataForExistingUsersAsync: beginning bulk copy...");
            //     await bulk.WriteToServerAsync(dt);
            // }
            
            var runTime = (DateTime.Now - originalStartTime);
            log($"GenerateSeedLocationDataForExistingUsersAsync: Finished in {runTime}");
            


            return output.ToString();
        }

        public static async Task<object> GenerateSeedLocationDataAsync(this CapstoneContext _db) {
            int nextId = 0;
            int nextUserId = 0;

            var output = new StringBuilder();

            var log = (string x) => {
                output.AppendLine(x);
                Console.WriteLine(x);
            };

            nextId = await _db.Locations
                .MaxAsync(x => (int?)x.Id) ?? 0;

            nextUserId = await _db.Locations
                .Where(x => x.FirebaseUserId.StartsWith("testuser-"))
                .Select(x => x.FirebaseUserId)
                .Distinct()
                .CountAsync();

            (double min, double max) Latitude = (41.383920730000604, 42.0420540561665);
            (double min, double max) Longitude = (-71.90395649891572, -73.48324099819017);
            const int MaxUsers = 250;
            const int MaxLocations = 10000;
            (double min, double max) PositionVariablility = (0.000001, 0.01);
            // const double PositionVariablility = 0.000001; // 0.111m
            // const double PositionVariablility = 0.01; // 11.1m
            const int TimeVariabilityMinutes = 1;
            var NumberOfGenders = await _db.UserGenders.CountAsync();
            var stringFormat = $"#,{new String('0', ((long)MaxUsers * MaxLocations).ToString().Length)}";

            log($"GenerateSeedLocationDataAsync: generating {MaxUsers} users with {MaxLocations} records each ({((long)MaxUsers * MaxLocations).ToString(stringFormat)} total rows)...");
            log($"Starting UserId: testuser-{(nextUserId+1).ToString("0000000")}");
            log($"Starting with location ID: {nextId}\n\n");

            var r = new Random(Guid.NewGuid().GetHashCode());
            var startTime = DateTime.UtcNow;
            var originalStartTime = DateTime.Now;
            var items = new List<Real.Model.Location>();

            for (int i=0; i<MaxUsers; ++i, ++nextUserId) {
                var userId = $"testuser-{(nextUserId+1).ToString("0000000")}";
                (double lat, double lon) position = (0,0);
                var t = startTime;

                await _db.Users.AddAsync(new Model.User {
                    RegisteredTimestamp = DateTime.UtcNow,
                    FirebaseUserId = userId,
                    Active = false,
                    Birthdate = DateTime.UtcNow,
                    FirstName = (nextUserId+1).ToString("0000000"),
                    LastLoginTimestamp = DateTime.UtcNow,
                    LastName = "testuser",
                    PreferredName = "testuser",
                    ProfileLastUpdatedTimestamp = DateTime.UtcNow,
                    UserGenderId = 1
                });
                await _db.SaveChangesAsync();

                for (int j=0; j<MaxLocations; ++j) {
                    if (position.lat == 0) {
                        var lat = r.NextDouble() * (Latitude.max - Latitude.min) + Latitude.min;
                        var lon = r.NextDouble() * (Longitude.max - Longitude.min) + Longitude.min;
                        position = (lat, lon);
                    } else {
                        if (r.Next()%2 == 0) {
                            var v = (
                                r.NextDouble() * (PositionVariablility.max - PositionVariablility.min) + PositionVariablility.min,
                                r.NextDouble() * (PositionVariablility.max - PositionVariablility.min) + PositionVariablility.min
                            );

                            position.lat += (r.Next()%2 == 0) ? v.Item1 : -v.Item1;
                            position.lon += (r.Next()%2 == 0) ? v.Item2 : -v.Item2;
                        }
                    }

                    var l = new Model.Location {
                        Id = ++nextId,
                        FirebaseUserId = userId,
                        Latitude = position.lat,
                        Longitude = position.lon,
                        Timestamp = t,
                    };
                    items.Add(l);

                    t = t.AddMinutes(TimeVariabilityMinutes);
                }

            }

            log("GenerateSeedLocationDataAsync: generating DataTable...");

            var dt = new DataTable();
            var properties = typeof(Model.Location).GetProperties();
            foreach (var prop in properties) {
                if (!Attribute.IsDefined(prop, typeof(NotMappedAttribute))) {
                    dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }

            foreach (var item in items) {
                var row = dt.NewRow();
                foreach (var prop in properties) {
                    if (!Attribute.IsDefined(prop, typeof(NotMappedAttribute))) {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(row);
            }


            var connectionString = _db.Database.GetConnectionString();
            var csItems = connectionString.Split(';').ToList();
            csItems.RemoveRange(4, csItems.Count - 4);
            csItems.Add("AllowLoadLocalInfile=true");
            connectionString = String.Join(';', csItems);

            var expected = (long)MaxUsers*MaxLocations;
            if (expected != items.LongCount() || expected != dt.Rows.Count) {
                throw new InvalidProgramException();
            }
            
            using (var connection = new MySqlConnection(connectionString)) {
                var bulk = new MySqlBulkCopy(connection);
                bulk.BulkCopyTimeout = 0;
                bulk.DestinationTableName = _db.Model.FindEntityType(typeof(Model.Location)).GetTableName();

                bulk.NotifyAfter = MaxLocations;
                bulk.MySqlRowsCopied += (sender, e) => {
                    log($"GenerateSeedLocationDataAsync: Copied {e.RowsCopied.ToString(stringFormat)} of {((long)MaxUsers * MaxLocations).ToString(stringFormat)} rows");
                };

                for (int col=0, j=0; col<properties.Length; ++col)
                    if (!Attribute.IsDefined(properties[col], typeof(NotMappedAttribute))) {
                        bulk.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(j++, properties[col].Name));
                    }
                
                log("GenerateSeedLocationDataAsync: beginning bulk copy...");
                await bulk.WriteToServerAsync(dt);
            }
            
            var runTime = (DateTime.Now - originalStartTime);
            log($"GenerateSeedLocationDataAsync: Finished in {runTime}");

            return output.ToString();
        }
    }

}