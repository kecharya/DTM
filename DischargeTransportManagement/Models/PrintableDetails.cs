using System.Collections.Generic;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class PrintableDetails
    {
        public string BedsideDateTime { get; set; }
        public string CareLevel { get; set; }
        public string PatientName { get; set; }
        public List<SelectListItem> SpecialNeeds { get; set; }
        public string DOB { get; set; }
        public string SSN { get; set; }
        public string Weight { get; set; }
        public string PatientNotes { get; set; }
        public string PatientLocation { get; set; }
        public string Destination { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationRoom { get; set; }
        public string PrimaryPayor { get; set; }
        public string InsuranceProvider { get; set; }
        public string InsuranceId { get; set; }
        public string PreAuthNumber { get; set; }
        public string AtNumber { get; set; }
        public string Cost { get; set; }
        public string DriveMileage { get; set; }

    }
}