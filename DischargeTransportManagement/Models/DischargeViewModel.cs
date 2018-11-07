using System.Collections.Generic;

namespace DischargeTransportManagement.Models
{
    public class DischargeViewModel
    {
        public List<DischargeComplaintModel> ComplaintModels {get; set;}
        public DischargeComplaintModel ComplaintModel { get; set; }
        public SubmittedRequestModel RequestModel { get; set; }

        public List<DischargeAction> DischargeActions { get; set; }

        public List<AuditLog> AuditLogs { get; set; }
    }
}