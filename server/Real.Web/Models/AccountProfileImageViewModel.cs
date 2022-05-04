using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Real.Model;

namespace Real.Web.Models
{
    public class AccountProfileImageViewModel {
        public int UserId { get; set; }
        public int ImageId { get; set; }
    }

}
