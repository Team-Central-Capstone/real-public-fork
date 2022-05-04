using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Real.Model;

namespace Real.Web.Areas.API.Models {
    public class UserProfileCompletedViewModel {
        public bool IsComplete => HasImage && RemainingProfileItems.Count == 0 && RemainingSurveyQuestions.Count == 0;
        public bool HasImage { get; set; }
        public List<string> RemainingProfileItems { get; set; } = new List<string>();
        public List<SurveyQuestion> RemainingSurveyQuestions { get; set; } = new List<SurveyQuestion>();
    }
}