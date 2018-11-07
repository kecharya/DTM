using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class AgencyModel
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string AddressLn1 { get; set; }
        public string AddressLn2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string Phone { get; set; }
        public bool LocalUse { get; set; }
        public string  AgencyType { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        public decimal? BaseRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        public decimal? MileageRate { get; set; }
        public int TaxId { get; set; }
        public string Notes { get; set; }
        public List<SelectListItem> StatesList { get; set; }
    }
}