using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Real.Model;

namespace Real.Web.Areas.API.Models {
    public class UserProfileImageUploadViewModel {

        [StringLength(255)]
        [Required]
        public string FileName { get; set; }

        [StringLength(32)]
        [Required]
        public string ContentType { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public string Base64ImageString { get; set; }

    }
}