using System;
using System.Collections.Generic;

namespace DischargeTransportManagement.Models
{
    public class CoverageInfo
    {
        public bool IsActive { get; set; }
        public int TypeId { get; set; }
        public string Typename { get; set; }
        public string VerifiedStatus { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string VerifiedByUser { get; set; }

        public ICollection<InsuranceInformation> InsuranceDetails { get; set; }
    }
}