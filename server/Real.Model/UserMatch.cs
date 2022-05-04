using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model {

    public class UserMatch : EntityBase {

        [ForeignKey("User1")]
        public int UserId1 { get; set; }
        public User User1 { get; set; }

        [ForeignKey("User2")]
        public int UserId2 { get; set; }
        public User User2 { get; set; }


        public DateTime MatchedOnDate { get; set; } = DateTime.UtcNow;
        public DateTime? User1AcceptedDate { get; set; }
        public DateTime? User2AcceptedDate { get; set; }


        public double MatchedLatitude { get; set; }
        public double MatchedLongitude { get; set; }

        public double RawMatchPercentage { get; set; }
        public double WeightedMatchPercentage { get; set; }

        

        [NotMapped]
        public string MatchedLocationName { get; set; }
    }
    
}
