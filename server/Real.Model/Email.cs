using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Real.Model {

    public class Email : EntityBase {

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // https://www.rfc-editor.org/errata_search.php?rfc=3696&eid=1690
        [StringLength(254)]
        public string From { get; set; }
        
        [StringLength(254)]
        public string To { get; set; }
        
        // http://www.faqs.org/rfcs/rfc2822.html
        [StringLength(998)]        
        public string Subject { get; set; }
        
        public string Body { get; set; }
    }
}
