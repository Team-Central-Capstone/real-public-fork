using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;


namespace Real.Model {

    public class UserImage : EntityBase {
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public bool IsProfilePhoto { get; set; } = false;

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(32)]
        public string ContentType { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public byte[] Image { get; set; }

        public string GetImageString() {
            return $"data:{ContentType};base64,${Convert.ToBase64String(Image)}";
        }
    }
}
