using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model {

    public class UserBlock : EntityBase {

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("BlockedUser")]
        public int BlockedUserId { get; set; }
        public User BlockedUser { get; set; }


        public DateTime BlockedOnDate { get; set; } = DateTime.UtcNow;

    }
    
}
