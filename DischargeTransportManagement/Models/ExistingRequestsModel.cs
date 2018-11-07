using DischargeTransportManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class ExistingRequestsModel
    {
        public List<ExistingRequests> Discharges = new List<ExistingRequests>();
        public List<ExistingRequests> AltFundings = new List<ExistingRequests>();
        public List<ExistingRequests> VhanRequests = new List<ExistingRequests>();
        public List<ExistingRequests> OtherRequests = new List<ExistingRequests>();
        public IEnumerable<SelectListItem> GetStatuses()
        {
            IList<SelectListItem> statuses = new List<SelectListItem>();
            using (lifeflightapps context = new lifeflightapps())
            {
                var statusFromDb = context.tblRequestStatus.Select(e => new SelectListItem()
                {
                    Value = e.RequestStatus.ToString(),
                    Text = e.RequestStatus
                });
                statuses = statusFromDb.ToList();
            }
            return statuses;
            //RequestModel rm = new RequestModel();
            //return rm.GetStatuses();
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

        public DischargeComplaintModel dischargeComplaintModel = new DischargeComplaintModel();
        public List<AuditLog> AuditLogs = new List<AuditLog>();
    }

    public class ExistingRequests
    {
        public string MrNumber { get; set; }
        public string PatientName { get; set; }
        public int PatientId { get; set; }
        public string DestinationName { get; set; }
        public int DestinationId { get; set; }
        public string EmsAgency { get; set; }
        public string ScheduledTime { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public string Bed { get; set; }
        public string Unit { get; set; }
        public string PavillionCode { get; set; }
        public string CallerName { get; set; }
        public int? CallerId { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public int DischargeRequestId { get; set; }
        public int PickupId { get; set; }
        public string PickupName { get; set; }
        public string RequestType { get; set; }
        public DateTime? AgencyResponded { get; set; }
        public DateTime? AgencyContacted { get; set; }
        public string WarningMessage { get; set; }
        public int? EmsUnitId { get; set; }
    }
}