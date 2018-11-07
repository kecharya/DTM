using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using epi = VUMC.EnterpriseServices;
//using System.Configuration;
using System.Web.Configuration;

namespace DischargeTransportManagement.Models
{
    public class RetrieveCoverageAndInsurance
    {
        string patientProdurl = WebConfigurationManager.AppSettings["EPIService"];//"https://episervice.mc.vanderbilt.edu/epi6_1/request";
        string patientInturl = WebConfigurationManager.AppSettings["EPIServiceInt"];//"https://epiint.service.vumc.org/epi7_0/request";

        /// <summary>
        /// Get Converage and insurance details for the MRN from the GenServices
        /// </summary>
        /// <param name="MRNumber"></param>
        /// <param name="vunetId"></param>
        /// <returns></returns>
        public List<CoverageInfo> GetCoverageAndInsurance(string MRNumber, string vunetId)
        {
            List<CoverageInfo> coverageresult = new List<CoverageInfo>();
            CoverageInfo info = new CoverageInfo();
            
            
            List<string> mrns = new List<string>
            {
                MRNumber
            };
            List<epi.PatientInsuranceRecord> insuranceRecords = new List<epi.PatientInsuranceRecord>();
            try
            {
                insuranceRecords = GetInsuranceList(mrns, vunetId, patientProdurl);
            }
            catch (Exception ex)
            {

                throw ex.InnerException;
            }

            List<epi.AccountRecord> accounts = new List<epi.AccountRecord>();

            foreach (var pir in insuranceRecords)
            {
                accounts = pir.AccountRecords.Where(p=>p.TypeName == "Personal/Family").ToList();

            }
            foreach (var item in accounts)
            {
                info = ConvertToCoverageInfo(item);
                coverageresult.Add(info);
            }

            return coverageresult;
        }

        public List<epi.PatientInsuranceRecord> GetInsuranceListInt(List<string> mrns, string vunetId, string serviceUrl)
        {
            epi.PersonService.ServiceUrl = serviceUrl;
            PatientDetails patientdetails = new PatientDetails();
            List<epi.PatientInsuranceRecord> insuranceRecords = new List<epi.PatientInsuranceRecord>();
            insuranceRecords = epi.PersonService.GetPatientInsuranceInformation(patientdetails.ApplicationName, patientdetails.AppPassKeyDev, vunetId, mrns, true);
            return insuranceRecords;
        }

        public List<epi.PatientInsuranceRecord> GetInsuranceList(List<string> mrns, string vunetId, string serviceUrl)
        {
            epi.PersonService.ServiceUrl = serviceUrl;
            PatientDetails patientdetails = new PatientDetails();
            List<epi.PatientInsuranceRecord> insuranceRecords = new List<epi.PatientInsuranceRecord>();
            insuranceRecords = epi.PersonService.GetPatientInsuranceInformation(patientdetails.ApplicationName, patientdetails.AppPassKeyPrd, vunetId, mrns, true);
            return insuranceRecords;
        }

        /// <summary>
        /// Get all the insurance information for the MRN
        /// </summary>
        /// <param name="mrNumber"></param>
        /// <param name="vunetId"></param>
        /// <returns></returns>
        public List<InsuranceInformation> GetInsurances(string mrNumber, string vunetId)
        {
            List<InsuranceInformation> insuranceList = new List<InsuranceInformation>();
            List<CoverageInfo> coverageresult = new List<CoverageInfo>();
            coverageresult = GetCoverageAndInsurance(mrNumber, vunetId);
            InsuranceInformation insinfo = new InsuranceInformation();
            foreach (var item in coverageresult)
            {
                foreach (var ins in item.InsuranceDetails)
                {
                    insuranceList.Add(ins);
                }
            }

            return insuranceList;

        }

        /// <summary>
        /// Convert the DTO to coverageinfo
        /// </summary>
        /// <param name="accountRecord"></param>
        /// <returns></returns>
        private CoverageInfo ConvertToCoverageInfo(epi.AccountRecord accountRecord)
        {
            CoverageInfo coverage = new CoverageInfo()
            {
                IsActive = accountRecord.IsActive,
                TypeId = accountRecord.TypeId,
                Typename = accountRecord.TypeName,
                VerifiedByUser = accountRecord.VerifiedByUser,
                VerifiedDate = accountRecord.VerifiedDate,
                VerifiedStatus = accountRecord.VerifiedStatus
            };
            List<InsuranceInformation> insuranceList = new List<InsuranceInformation>();
            foreach (var coveragerec in accountRecord.CoverageRecords)
            {
                InsuranceInformation insuranceInfo = new InsuranceInformation()
                {
                    EffectiveDate = coveragerec.EffectiveDate,
                    FilingOrder = coveragerec.FilingOrder,
                    FinancialName = coveragerec.FinancialName,
                    Payorname = coveragerec.PayorName,
                    MedipacPlanId = coveragerec.MedipacPlanId,
                    PlanName = coveragerec.PlanName,
                    PlanType = coveragerec.PayorName,
                    SubscriberName = coveragerec.SubscriberName,
                    TerminationDate = coveragerec.TerminationDate,
                    PayorId = coveragerec.PayorId,
                    PlanId = coveragerec.PlanId,
                    InsuranceId = Convert.ToInt32(coveragerec.ID)
                };
                insuranceList.Add(insuranceInfo);
            }
            coverage.InsuranceDetails = insuranceList;

            return coverage;
        }
    }

    
}