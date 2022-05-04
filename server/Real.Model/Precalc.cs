using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Real.Model {

    public class Precalc_Location : EntityBase, ICloneable {
        public string FirebaseUserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string trace { get; set; }

        public object Clone()
        {
            return new Precalc_Location {
                Id = this.Id,
                FirebaseUserId = this.FirebaseUserId,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                trace = this.trace,
            };
        }
    }

    public class Precalc_ProfileMatch : EntityBase {
        public int lUserId { get; set; }
        public int rUserId { get; set; }
        public long TotalPossibleQuestions { get; set; }
        public int MatchedQuestions { get; set; }
        public double RawMatchPercentage { get; set; }
        public double WeightedMatchPercentage { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
}
