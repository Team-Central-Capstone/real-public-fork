using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model {
    
    public enum UserSurveyResponseWeight {
        [Display(Name = "Not important")]
        Low = 1,
        
        [Display(Name = "Average")]
        Medium = 2,
        
        [Display(Name = "Very important")]
        High = 3,
    }

    public class UserSurveyResponse : EntityBase {
        public int UserId { get; set; }
        public User User { get; set; }

        public int SurveyQuestionId { get; set; }
        public virtual SurveyQuestion SurveyQuestion { get; set; }

        // multiple selections
        public virtual List<SurveyAnswer> SurveyAnswers { get; set; } = new List<SurveyAnswer>();

        // free form entry
        [MaxLength(250)]
        public string SurveyAnswerResponse { get; set; }

        [Display(Name = "Importance")]
        public UserSurveyResponseWeight UserSurveyResponseWeight { get; set; } = UserSurveyResponseWeight.Medium;

    }
}
