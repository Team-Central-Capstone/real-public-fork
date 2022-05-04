using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Real.Model;

namespace Real.Web.Areas.API.Models
{
    public class UserSurveyResponseViewModel {
        public string SurveyAnswerResponse { get; set; }
        public List<int> AnswerIds { get; set; }
        public UserSurveyResponseWeight? UserSurveyResponseWeight { get; set; }
        
    }

}
