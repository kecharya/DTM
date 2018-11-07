using System;

namespace DischargeTransportManagement.Models
{
    public class DischargeRequestModel
    {
        public int DischargeRequestId { get; set; }
        public int CallerID { get; set; }
        public int DestinationID { get; set; }
        public string MrNumber { get; set; }
        public string ModeOfTransport { get; set; }
        public string SpecialInstructions { get; set; }
        public string DischargeTime { get; set; }
        public string AgencyArrivalTime { get; set; }
        public int InsuranceID { get; set; }
        public int PatientID { get; set; }
        public int LocationID { get; set; }
        public int RequestStatusID { get; set; }
        public string LifeSupport { get; set; }
        public string ReceivedByUser { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string CaseNumber { get; set; }
        public string Notes { get; set; }
    }
}