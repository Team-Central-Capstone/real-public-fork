using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model {

    public class UserGender : EntityBase {
        [MaxLength(30)]
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual List<User> UserGenders { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual List<User> UserAttractedTo { get; set; }
    }
    
}
