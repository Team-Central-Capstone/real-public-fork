using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Real.Model {
    
    
    public static class DistanceBetweenPoints {

        public static double MPH(Model.Location a, Model.Location b) {
            return MPH(a.Latitude, a.Longitude, b.Latitude, b.Longitude, a.Timestamp, b.Timestamp);
        }

        public static double MPH(double lat1, double lon1, double lat2, double lon2, DateTime timestamp, DateTime previousTimestamp) {
            var miles = Meters(lat1, lon1, lat2, lon2) / 1000.0 * 0.62137;
            var hours = (timestamp - previousTimestamp).TotalSeconds / 3600.0;
            return Math.Abs(miles / hours);
        }

        public static double Miles(Model.Location a, Model.Location b) {
            return Miles(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        public static double Miles(double lat1, double lon1, double lat2, double lon2) {
            return Meters(lat1, lon1, lat2, lon2) / 1000.0 * 0.62137;
        }

        public static double Meters(Model.Location a, Model.Location b) {
            return Meters(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        public static double Meters(double lat1, double lon1, double lat2, double lon2) {
            // https://www.movable-type.co.uk/scripts/latlong.html

            var @R = 6371e3; // meters
            var @PI = 3.141592653589793;
            var @phi1 = lat1 * @PI / 180.0;
            var @phi2 = lat2 * @PI / 180.0;
            var @dPhi = (lat2-lat1) * @PI / 180.0;
            var @dLambda = (lon2-lon1) * @PI / 180.0;

            var @a = Math.Sin(@dPhi/2) * Math.Sin(@dPhi/2) +
                     Math.Cos(@phi1) * Math.Cos(@phi2) *
                     Math.Sin(@dLambda / 2.0) * Math.Sin(@dLambda / 2.0);
            var @c = 2.0 * Math.Atan2(Math.Sqrt(@a), Math.Sqrt(1.0 - @a));
            var @d = @R * @c;
            
            return @d;
        }
    }



    public enum LocationSource {
        Mobile,
        Web,
    }

    public class Location : EntityBase {

        [IndexColumn("IX_Locations_FirebaseUserId")]
        [IndexColumn("IX_Locations", 0)]
        [StringLength(36)]
        public string FirebaseUserId { get; set; }

        [StringLength(64)]
        public string DeviceID { get; set; }

        public LocationSource Source { get; set; } = LocationSource.Mobile;

        [IndexColumn("IX_Locations", 1)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        
        [IndexColumn("IX_Locations", 2)]
        public double Latitude { get; set; } = 0;

        [IndexColumn("IX_Locations", 3)]
        public double Longitude { get; set; } = 0;

        public double SpeedFromLast { get; set; } = 0;
        public double RollingAverageSpeed { get; set; } = 0;


        [NotMapped]
        public double Distance { get; set; }

        public override string ToString() {
            return $"{FirebaseUserId}-({Latitude}.{Longitude}) {Timestamp}";
        }

    }
}
