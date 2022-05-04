using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model
{
    public class SurveyAnswer : EntityBase {

        public int SurveyQuestionId { get; set; }
        // public virtual SurveyQuestion SurveyQuestion { get; set; }

        [MaxLength(250)]
        public string AnswerText { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual List<UserSurveyResponse> UserSurveyResponses { get; set; } = new List<UserSurveyResponse>();

        [NotMapped]
        public bool Deleted { get; set; } = false;
    }
}
