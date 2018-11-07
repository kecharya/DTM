using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DischargeTransportManagement.Models.ReportModels
{
    public class VanderbiltEMS
    {
        public DateTime PatientPickupTime { get; set; }
        public string CallerName { get; set; }
        public string CallerTitle { get; set; }
        public string RequestStatus { get; set; }
        public string Type { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string EmsAgency { get; set; }
        public string Patient { get; set; }
        public string PatientMRN { get; set; }
    }
}