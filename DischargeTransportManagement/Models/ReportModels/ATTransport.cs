using System;

namespace DischargeTransportManagement.Models.ReportModels
{
    public class ATTransport
    {
        public DateTime? PatientPickupTime { get; set; }
        public string Type { get; set; }
        public string Patient { get; set; }
        public string Location { get; set; }
        public string Destination { get; set; }
        public string EmsAgency { get; set; }
        public int Miles { get; set; }
        public int Cost { get; set; }
        public string ATNumber { get; set; }
        public string Payor { get; set; }
        public string Reason { get; set; }
        public string RequestStatus { get; set; }

    }
}