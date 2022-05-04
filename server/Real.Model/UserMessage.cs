using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model {

    public class UserMessage : EntityBase {

        [ForeignKey("User1")]
        public int UserId1 { get; set; }
        public User User1 { get; set; }

        [ForeignKey("User2")]
        public int UserId2 { get; set; }
        public User User2 { get; set; }


        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Message { get; set; }
        public bool MessageRead { get; set; } = false;

    }
    
}
