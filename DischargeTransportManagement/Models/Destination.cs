using System.Collections.Generic;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class Destination
    {
        public int DestinationId { get; set; }
        public string DestinationType { get; set; }
        public string DestinationName { get; set; }
        public string DestInstructions { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string Phone { get; set; }
        public int? Miles { get; set; }
        public string TravelTime { get; set; }

        public List<SelectListItem> StatesList { get; set; }
    }
}