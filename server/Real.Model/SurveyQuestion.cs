using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model
{

    public enum QuestionType {

        [Display(Name = "Free Form - Long", Description = "Multiline, free-form text input")]
        FreeForm = 1,

        [Display(Name = "Single Choice")]
        SingleChoice = 2,
        
        [Display(Name = "Multiple Choice")]
        MultipleChoice = 3,
        
        [Display(Name = "Numeric")]
        Numeric = 4,

        [Display(Name = "Yes/No")]
        YesNo = 5,

        [Display(Name = "Free Form - Short")]
        FreeFormShort = 6,

    }

    public class SurveyQuestion : EntityBase {

        public int Order { get; set; }

        [Display(Name = "Question type")]
        [EnumDataType(typeof(QuestionType))]
        public QuestionType QuestionType { get; set; }

        [NotMapped]
        public string QuestionTypeString { get => QuestionType.ToString(); }

        [Display(Name = "Question text")]
        [MaxLength(250)]
        public string QuestionText { get; set; }

        [Display(Name = "Possible Responses")]
        public virtual List<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
    }
}
