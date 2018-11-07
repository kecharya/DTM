using System;
using System.Collections.Generic;

namespace DischargeTransportManagement.Models
{
    public class InsuranceModel
    {
        public int InsuranceId { get; set; }
        public string InsuranceName { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string InsurancePhone { get; set; }
        public string InsuranceFax { get; set; }
        public bool PreAuthRequired { get; set; }
        public bool HasOwnPaperWork { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public List<InsuranceContactModel> contactModels { get; set; }
    }

    public class InsuranceContactModel
    {
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNote { get; set; }
    }
}