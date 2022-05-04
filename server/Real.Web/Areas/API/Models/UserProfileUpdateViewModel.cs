using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Real.Model;

namespace Real.Web.Areas.API.Models
{
    public class UserProfileUpdateViewModel {
        public List<UserProfileUpdateViewModelQuestion> Questions { get; set; } = new List<UserProfileUpdateViewModelQuestion>();

        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool Changes { get; set; } = false;

        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        public string FirebaseUserId { get; set; }
    }

    public class UserProfileUpdateViewModelQuestion {
        public int Index { get; set; } = 1;
        public SurveyQuestion SurveyQuestion { get; set; }
        public UserSurveyResponse UserSurveyResponse { get; set; }
    }
}
