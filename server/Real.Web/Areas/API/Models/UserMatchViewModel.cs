
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using MySqlConnector;
using Real.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Models {

    public class UserMatchViewModel {
        public double RawMatchPercentage { get; set; }
        public double WeightedMatchPercentage { get; set; }
        public DateTime MatchCalculcatedTimestamp { get; set; }
        
        public string lFirebaseUserId { get; set; }
        public string rFirebaseUserId { get; set; }

        [NotMapped]
        public string MatchedLocationName { get; set; }
    }
}