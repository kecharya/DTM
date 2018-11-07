using DischargeTransportManagement.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Configuration;
using epi = VUMC.EnterpriseServices;

namespace DischargeTransportManagement.Models
{
    public class RetrieveCensusInformation
    {
        string censusUrl = WebConfigurationManager.AppSettings["CensusService"]; //"https://census.service.vumc.org/census7_0/request";
        //string censusUrl = "https://censusservice.mc.vanderbilt.edu/census6_0/request";
        
        Constants cons = new Constants();
        public List<CensusModel> GetCensusRecords(string mrNumber, string vunetId)
        {
            List<epi.CensusRecord> censusRecords = new List<epi.CensusRecord>();
            Censusrequest req = new Censusrequest();
            epi.CensusService.ServiceUrl = censusUrl;
            int i;
            censusRecords = epi.CensusService.GetCensusRecords(req.ApplicationName, req.AppPassKey, vunetId, mrNumber);
            //,null,null,null,null,null,null,null,null,null
            i = epi.CensusService.GetCensusCount(req.ApplicationName, req.AppPassKey, req.VunetId, mrNumber);
            Debug.WriteLine(i);
            List<CensusModel> censusModel = new List<CensusModel>();
            foreach (var item in censusRecords)
            {
                censusModel.Add( CovertToModel(item));

            }
            return censusModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mrNumber"></param>
        /// <returns></returns>
        public int GetCensusCount(string mrNumber)
        {
            int censusCount = 0;
            epi.CensusService.ServiceUrl = censusUrl;
            Censusrequest req = new Censusrequest();
            censusCount = epi.CensusService.GetCensusCount(req.ApplicationName, req.AppPassKey, req.VunetId, mrNumber);
            return censusCount;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mrns"></param>
        /// <returns></returns>
        public string GetMrnFromCensus(List<string> mrns)
        {
            string mrNumber = string.Empty;

            foreach (var mr in mrns)
            {
                int censusCount = GetCensusCount(mr);
                if (censusCount > 0)
                {
                    mrNumber = mr;
                    break;
                }
            }
            return mrNumber;
        }

        private CensusModel CovertToModel(epi.CensusRecord item)
        {
            try
            {
                CensusModel cm = new CensusModel()
                {
                    Bed = item.Bed,
                    PavillionCode = item.PavilionCode,
                    Unit = item.Unit,
                    CaseNumber = item.CaseNumber
                };

                return cm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    public class Censusrequest
    {
        public Censusrequest()
        {
            ApplicationName = "LifeFlightDTM";

            AppPassKey = "r#A0hl7Q";

            VunetId = "tatiner";
        }
        public string ApplicationName { get; set; }
        public string AppPassKey { get; set; }

        public string VunetId { get; set; }
    }
}