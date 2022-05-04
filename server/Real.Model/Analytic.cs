using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Real.Model {

    public class AnalyticDetail {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        internal Guid? _AnalyticId = null;
        public Guid AnalyticId {
            get {
                if (!_AnalyticId.HasValue)
                    _AnalyticId = Analytic.Id;
                return _AnalyticId.Value;
            }
            set => _AnalyticId = value;
        }
        public virtual Analytic Analytic { get; set; }

        public string Message { get; set; }
    }

    public class AnalyticError {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AnalyticId { get; set; }
        public virtual Analytic Analytic { get; set; }

        [StringLength(200)]
        public string TraceId { get; set; }

        [StringLength(200)]
        public string RequestId { get; set; }

        [StringLength(200)]
        public string TraceIdentifier { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }

    

    public class Analytic {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? AnalyticErrorId { get; set; }
        public virtual AnalyticError AnalyticError { get; set; }

        public Guid? AnalyticDetailId { get; set; }
        public virtual AnalyticDetail AnalyticDetail { get; set; }

        public DateTime StartTimestamp { get; set; } = DateTime.UtcNow;
        public DateTime EndTimestamp { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string Namespace { get; set; }

        [StringLength(200)]
        public string Area { get; set; }

        [StringLength(200)]
        public string Controller { get; set; }

        [StringLength(200)]
        public string Action { get; set; }

        [StringLength(200)]
        public string UserName { get; set; }

        [StringLength(36)]
        public string FirebaseUserId { get; set; }

        [StringLength(100)]
        public string IPv4 { get; set; }

        [StringLength(100)]
        public string IPv6 { get; set; }

        [StringLength(200)]
        public string Host { get; set; }

        [StringLength(200)]
        public string Path { get; set; }

        public string QueryString { get; set; }
        
    }
}
