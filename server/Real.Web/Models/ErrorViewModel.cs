using System;

namespace Real.Web.Models {
    public class ErrorViewModel {
        public string RequestId { get; set; }

        public Exception Exception { get; set; }

        public string Path { get; set; }
        public string UserName { get; set; }
        public string FirebaseUserId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
