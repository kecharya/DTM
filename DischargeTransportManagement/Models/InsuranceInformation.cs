using System;

namespace DischargeTransportManagement.Models
{
    public class InsuranceInformation
    {
        public int MbrInsuranceId { get; set; }
        public int FilingOrder { get; set; }
        public string FinancialName { get; set; }
        public string Payorname { get; set; }
        public string MedipacPlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public string SubscriberName { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string PayorId { get; set; }
        public long InsuranceId { get; set; }
        public string PlanId { get; set; }
        public string AuthCode { get; set; }
        public string Phone { get; set; }
        public string ICDCode { get; set; }
    }
}