using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Real.Web.Areas.API.Models {
    public class EmailViewModel {

        [Required]
        public string Recipient { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public MailPriority Priority { get; set; } = MailPriority.Normal;
    }
}