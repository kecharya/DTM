using System;
using System.Collections.Generic;
using System.Web.Configuration;
using epi = VUMC.EnterpriseServices;


namespace DischargeTransportManagement.Models
{
    public class RetrievePatientInformation
    {
        //string patientProdurl = "https://epi.service.vumc.org/epi7_0/request";
        //string patientProdurl = "https://episervice.mc.vanderbilt.edu/epi6_1/request";
        string patientProdurl = WebConfigurationManager.AppSettings["EPIService"];//"https://episervice.mc.vanderbilt.edu/epi6_1/request";
        string patientInturl = WebConfigurationManager.AppSettings["EPIServiceInt"];//"https://epiint.service.vumc.org/epi7_0/request";

        /// <summary>
        /// Get the patient information from the GenServices
        /// </summary>
        /// <param name="mrNumber"></param>
        /// <returns></returns>
        public List<Patient> GetPatientInformation(string mrNumber)
        {
            List<epi.Patient> epiPatients = new List<epi.Patient>();
            epi.PersonService.ServiceUrl = patientProdurl;
            PatientDetails details = new PatientDetails();
            epiPatients = epi.PersonService.GetPatientInformation(details.ApplicationName, details.AppPassKeyPrd, details.VunetId, mrNumber, "", false, "", false, "", "", "", "", null, null, null, "", "", "", 0);
            List<Patient> patientListModel = new List<Patient>();
            Patient patientModel = new Patient();
            foreach (var patient in epiPatients)
            {
                patientModel = ConvertToPatientModel(patient);
                patientListModel.Add(patientModel);
            }
            return patientListModel;
        }

        /// <summary>
        /// Search for MRN using the patient name
        /// </summary>
        /// <param name="Fname"></param>
        /// <param name="Lname"></param>
        /// <returns></returns>
        public string GetMrnByName(string Fname, string Lname)
        {
            string MRNumber = string.Empty;
            List<epi.Patient> epiPatients = new List<epi.Patient>();
            epi.PersonService.ServiceUrl = patientProdurl;
            PatientDetails details = new PatientDetails();
            epiPatients = epi.PersonService.GetPatientInformation(details.ApplicationName, details.AppPassKeyPrd, details.VunetId, string.Empty, Lname, true, Fname, true, string.Empty, string.Empty, string.Empty, string.Empty, null, null, null, string.Empty, string.Empty, string.Empty, 0);
            List<string> mrnumbers = new List<string>();
            foreach (var p in epiPatients)
            {
                mrnumbers.Add(p.MedicalRecordNumber);
            }
            RetrieveCensusInformation rc = new RetrieveCensusInformation();
            MRNumber = rc.GetMrnFromCensus(mrnumbers);
            return MRNumber;//mrnumbers.FirstOrDefault();
        }

        /// <summary>
        /// Calls from the request Controller
        /// </summary>
        /// <param name="mrNumber"></param>
        /// <param name="vunetId"></param>
        /// <returns></returns>
        public Patient GetPatient(string mrNumber, string vunetId)
        {
            Patient patientModel = new Patient();
            try
            {

                List<epi.Patient> epipatients = new List<epi.Patient>();
                PatientDetails details = new PatientDetails();
                epi.PersonService.ServiceUrl = patientProdurl;
                try
                {
                    epipatients = epi.PersonService.GetPatientInformation(details.ApplicationName, details.AppPassKeyPrd, vunetId, mrNumber, "", false, "", false, "", "", "", "", null, null, null, "", "", "", 0);
                }
                catch (Exception ex)
                {
                    var exception = ex;
                    //throw ex.InnerException;
                    //patientModel.MRN = "";
                    return patientModel;
                }
                    if (epipatients.Count > 0 && epipatients.Count == 1)
                    patientModel = ConvertToPatientModel(epipatients[0]);
                return patientModel;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Convert the dto from service to model
        /// </summary>
        /// <param name="epipatient"></param>
        /// <returns></returns>
        private Patient ConvertToPatientModel(epi.Patient epipatient)
        {
            Patient myPatient = new Patient()
            {
                Address = epipatient.HomeAddress.AddressLine1 + ", " + epipatient.HomeAddress.AddressLine2,
                Age = epipatient.BirthDate.HasValue ? (DateTime.Today.Year - epipatient.BirthDate.Value.Year) : 0,
                City = epipatient.HomeAddress.City,
                County = epipatient.HomeAddress.LocationName,
                DOB = epipatient.BirthDate.HasValue ? epipatient.BirthDate.Value.ToShortDateString() : "N/A",
                FirstName = epipatient.FirstName,
                LastName = epipatient.LastName,
                MRN = epipatient.MedicalRecordNumber,
                Phone = epipatient.HomePhone,
                Social = epipatient.SSN,
                State = epipatient.HomeAddress.State,
                Zip = epipatient.HomeAddress.ZipCode
            };
            return myPatient;
        }
    }
}