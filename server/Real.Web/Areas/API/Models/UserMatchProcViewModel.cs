
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using MySqlConnector;
using Real.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Models {

    public static partial class Extensions {
        public static UserMatchProcViewModel ToUserMatch(this MySqlDataReader reader) {
            return new UserMatchProcViewModel {
                RawMatchPercentage = double.Parse(reader["RawMatchPercentage"].ToString()),
                WeightedMatchPercentage = double.Parse(reader["WeightedMatchPercentage"].ToString()),
                MatchCalculcatedTimestamp = DateTime.Parse(reader["MatchCalculcatedTimestamp"].ToString()),
                lUserId = int.Parse(reader["lUserId"].ToString()),
                rUserId = int.Parse(reader["rUserId"].ToString()),
                start_time = DateTime.Parse(reader["start_time"].ToString()),
                end_time = DateTime.Parse(reader["end_time"].ToString()),
                overlaptime = DateTime.Parse(reader["overlaptime"].ToString()),
                miles = double.Parse(reader["miles"].ToString()),
                lLatitude = double.Parse(reader["lLatitude"].ToString()),
                lLongitude = double.Parse(reader["lLongitude"].ToString()),
                rLatitude = double.Parse(reader["rLatitude"].ToString()),
                rLongitude = double.Parse(reader["rLongitude"].ToString()),
                lFirebaseUserId = reader["lFirebaseUserId"].ToString(),
                rFirebaseUserId = reader["rFirebaseUserId"].ToString(),
                lStartTime = DateTime.Parse(reader["lStartTime"].ToString()),
                lEndTime = DateTime.Parse(reader["lEndTime"].ToString()),
                rStartTime = DateTime.Parse(reader["rStartTime"].ToString()),
                rEndTime = DateTime.Parse(reader["rEndTime"].ToString()),
            };
        }
    }

    public class UserMatchProcViewModel {
        public double RawMatchPercentage { get; set; }
        public double WeightedMatchPercentage { get; set; }
        public DateTime MatchCalculcatedTimestamp { get; set; }
        public int lUserId { get; set; }
        public int rUserId { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public DateTime overlaptime { get; set; }
        public double miles { get; set; }
        public double lLatitude { get; set; }
        public double lLongitude { get; set; }
        public double rLatitude { get; set; }
        public double rLongitude { get; set; }
        public string lFirebaseUserId { get; set; }
        public string rFirebaseUserId { get; set; }
        public DateTime lStartTime { get; set; }
        public DateTime lEndTime { get; set; }
        public DateTime rStartTime { get; set; }
        public DateTime rEndTime { get; set; }   
    }
}