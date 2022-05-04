using System;
using System.Linq;
using System.Collections.Generic;

namespace Real.Web.Areas.Admin.Models.Visualization {

    public class IndexViewModel {
        public Model.User User { get; set; }

        public List<(string, string)> Users { get; set; } = new List<(string, string)>();
    }

}