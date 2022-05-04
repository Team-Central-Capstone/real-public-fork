using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Real.Model;

namespace Real.Web.Areas.API.Models
{
    public class UserProfileViewModel {
        public User User { get; set; }
        public UserImage UserImage { get; set; }
        public bool Changes { get; set; } = false;

        public bool HasImage { get; set; } = false;
        public IFormFile File { get; set; }

        public List<UserGender> Genders { get; set; }
        public List<UserProfileViewModelQuestion> Questions { get; set; }
    }

    public class UserProfileViewModelQuestion {
        public int Index { get; set; } = 1;
        public SurveyQuestion SurveyQuestion { get; set; }
        public UserSurveyResponse UserSurveyResponse { get; set; }
    }
}
