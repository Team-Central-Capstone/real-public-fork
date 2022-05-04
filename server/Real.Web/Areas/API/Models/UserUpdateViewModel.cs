using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Real.Model;
using Swashbuckle.AspNetCore.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Real.Web.Areas.API.Models {
    public class UserUpdateViewModel {

        [MaxLength(36)]
        public string FirebaseUserId { get; set; }

        [Display(Name = "Email Address")]
        [MaxLength(320)]
        public string Email { get; set; }

        [Display(Name = "First name")]
        [MaxLength(100)]
        public string FirstName { get; set; }
        
        [Display(Name = "Preferred name")]
        [MaxLength(30)]
        public string PreferredName { get; set; }
        
        [Display(Name = "Last name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        [Display(Name = "Gender")]
        public int UserGenderId { get; set; }

        [Display(Name = "Body Type")]
        public int? UserBodyTypeId { get; set; }

        [Display(Name = "Genders attracted to")]
        public List<int> UserAttractedGenderIds { get; set; }

        [Display(Name = "Height (inches)")]
        public int HeightInches { get; set; }

        [Display(Name = "Profile Summary")]
        [StringLength(1000)]
        public string ProfileIntro { get; set; }
    }
}
