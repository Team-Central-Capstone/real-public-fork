using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model {

    public class UserBodyType : EntityBase {
        public string Name { get; set; }
    }
    
}
