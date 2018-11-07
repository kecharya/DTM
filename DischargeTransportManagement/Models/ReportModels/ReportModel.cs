using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DischargeTransportManagement.Models.ReportModels
{
    public abstract class ReportModel
    {
        public DateTime RequestReceivedAt { get; set; }
        public DateTime PatientPickupTime { get; set; }
        public DateTime EMSAgencyAssignedAt { get; set; }
        public DateTime EMSAgencyBedSideAt { get; set; }
        public string Type { get; set; }
        public string CallerName { get; set; }
        public string CallerTitle { get; set; }
        public string RequestStatus { get; set; }
        public string TransportMode { get; set; }
        public string Patient { get; set; }
        public int PatientAge { get; set; }
        public int PatientWeight { get; set; }
        public string PatientAddress { get; set; }
        public string MrNumber { get; set; }
        public string From { get; set; }
        public string Destination { get; set; }
        public string EmsAgency { get; set; }
        public int Miles { get; set; }
        public int Cost { get; set; }
        public string AtNumber { get; set; }
        public string Payor { get; set; }
        public string PayingReason { get; set; }
    }
}