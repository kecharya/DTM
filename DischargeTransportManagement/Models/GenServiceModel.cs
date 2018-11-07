namespace DischargeTransportManagement.Models
{
    public class GenServiceModel
    {
        public CoverageInfo Coverages { get; set; }
        public InsuranceInformation Insurances { get; set; }
        public Patient PatientDemographics { get; set; }
        public CensusModel Census { get; set; }
    }
}