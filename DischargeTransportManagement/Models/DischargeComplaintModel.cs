using DischargeTransportManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class DischargeComplaintModel
    {
        public int RequestId { get; set; }
        public string Complaint { get; set; }
        public string ComplaintType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsAocNotified { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string DelayedReason { get; set; }
        public string PatientName { get; set; }
        public DateTime? RequestDate { get; set; }
        public string MRN { get; set; }
        public string Destination { get; set; }
        public string Agency { get; set; }
        public string Location { get; set; }

        public IEnumerable<SelectListItem> GetComplaintTypes()
        {
            IList<SelectListItem> complaintTypes = new List<SelectListItem>();
            using (lifeflightapps context = new lifeflightapps())
            {
                var complaintTypesFromDb = context.tblDischargeComplaintTypes.Select(e => new SelectListItem()
                {
                    Value = e.ComplaintTypeID.ToString(),
                    Text = e.ComplaintType
                });
                complaintTypes = complaintTypesFromDb.ToList();
            }
            return complaintTypes;
        }

        public IEnumerable<SelectListItem> GetDelayReasons()
        {
            IList<SelectListItem> delayReasons = new List<SelectListItem>();
            using (lifeflightapps context = new lifeflightapps())
            {
                var delayReasonsFromDb = context.tblDelayReasons.Select(e => new SelectListItem()
                {
                    Value = e.delayreasonID.ToString(),
                    Text = e.delayreason
                });
                delayReasons = delayReasonsFromDb.ToList();
            }
            return delayReasons;
        }
    }
}