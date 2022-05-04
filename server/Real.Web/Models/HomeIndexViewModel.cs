using System;
using System.Collections.Generic;

namespace Real.Web.Models
{
    public class HomeIndexViewModel {
        public bool IsAuthenticated { get; set; } = false;
        public Model.User User { get; set; }

        public List<Model.User> NearbyUsers { get; set; } = new List<Model.User>();
    }
}
