using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class Caller
    {
        public int CallerId { get; set; }
        public string CallerLastName { get; set; }
        public string CallerFirstName { get; set; }
        public string CallerTitle { get; set; }
        public Nullable<double> OfficePhone { get; set; }
        public string CallerPager { get; set; }
        public Nullable<double> MobilePhone { get; set; }
        public string CallerEmail { get; set; }
        public string Assignment { get; set; }
        public bool Active { get; set; }
        public string PreferredNumberType { get; set; }
        public List<SelectListItem> PreferredNumberTypes { get; set; }
    }
}