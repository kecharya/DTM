using System;

namespace DischargeTransportManagement.Models.ReportModels
{
    public class RequestsPerMonth
    {
        public DateTime? RequestDateTime { get; set; }
        public string Type { get; set; }
        public string Patient { get; set; }
        public string Location { get; set; }
        public string Destination { get; set; }
        public string EmsAgency { get; set; }
    }
}