using System.Collections.Generic;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class InsuranceAgencyViewModel
    {
        public IEnumerable<SelectListItem> Insurances { get; set; }
        public IEnumerable<SelectListItem> Agencies { get; set; }

        public IEnumerable<SelectListItem> SelectedAgencies { get; set; }
    }
}