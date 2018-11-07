using System;

namespace DischargeTransportManagement.Models
{
    public class AuditLog
    {
        public int RequestId { get; set; }
        public string MrNumber { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedDateTime { get; set; }

    }
}