using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class SubmittedRequestModel
    {
        public DischargeRequestModel submittedDischarge = new DischargeRequestModel();
        public Caller submittedCaller = new Caller();
        public Destination submittedDestination = new Destination();
        public PickupModel submittedPickup = new PickupModel();
        public InsuranceInformation submittedInsurance = new InsuranceInformation();
        public CensusModel submittedCensus = new CensusModel();
        public Patient submittedPatient = new Patient();
        public List<SpecialNeeds> submittedSplNeeds = new List<SpecialNeeds>();
        public InsuranceModel insuranceDetails = new InsuranceModel();
        public string SpecialInstructions { get; set; }
        public DateTime DischargeTime { get; set; }
        public string EmsAgency { get; set; }
        public string MOTSelected { get; set; }
        public string RequestStatus { get; set; }
        public int EmsAgencySelected { get; set; }
        public string LocationDetails { get; set; }
        public string RequestType { get; set; }
        public string ATNumber { get; set; }
        public string PayingReason { get; set; }
        public decimal? Cost { get; set; }
        public string AuthCode { get; set; }
        public bool EnableEdit { get; set; }
        public string CallReceivedBy { get; set; }
        public string InvoiceNumber { get; set; }
        public string PONumber { get; set; }
        #region Complaint Details
        public DateTime? AgencyContacted { get; set; }
        public DateTime? AgencyResponded { get; set; }
        public DateTime? AgencyArrived { get; set; }
        //public string CallReceivedBy { get; set; }
        public DateTime? CallReceivedAt { get; set; }

        #endregion

        public List<SelectListItem> EmsAgencies { get; set; }
        public List<SelectListItem> TransportModes { get; set; }
        public List<SelectListItem> Statuses { get; set; }
        public List<SelectListItem> SpecialNeedsSubmitted { get; set; }
        public List<SelectListItem> VumcPayorReasons { get; set; }
        public List<SelectListItem> Payors { get; set; }

        public IEnumerable<SelectListItem> GetStatesListFromRequestModel()
        {
            RequestModel rm = new RequestModel();
            return rm.GetStatesList();
        }

        public IEnumerable<SelectListItem> GetEmsAgenciesFromRequestModel()
        {
            RequestModel rm = new RequestModel();
            return rm.GetEmsAgencies();
        }

        public IEnumerable<SelectListItem> GetTransModesFromrequestModel()
        {
            RequestModel rm = new RequestModel();
            return rm.GetModesOfTransport();
        }
    }
}