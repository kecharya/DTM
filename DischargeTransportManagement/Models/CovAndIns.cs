using System.Collections.Generic;

namespace DischargeTransportManagement.Models
{
    public class CovAndIns
    {
        public IEnumerable<CoverageInfo> Coverages { get; set; }
        public IEnumerable<InsuranceInformation> Insurances { get; set; }
        public IEnumerable<Patient> PatientDemographics { get; set; }
        public IEnumerable<CensusModel> Census { get; set; }
        //public tblCaller Caller { get; set; }
    }
}