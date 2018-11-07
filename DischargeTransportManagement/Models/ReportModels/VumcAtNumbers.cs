using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DischargeTransportManagement.Models.ReportModels
{
    public class VumcAtNumbers
    {
        public DateTime PatientPickupTime { get; set; }
        public string Type { get; set; }
        public string Patient { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string EmsAgency { get; set; }
        public int Miles { get; set; }
        public int Cost { get; set; }
        public string AtNumber { get; set; }
        public string Payor { get; set; }
        public string Reason { get; set; }
    }
}