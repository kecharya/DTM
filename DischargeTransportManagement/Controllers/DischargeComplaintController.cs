using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class DischargeComplaintController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Complaint()
        {
            DischargeComplaintModel model = new DischargeComplaintModel();

            return View(model);
        }

        /// <summary>
        /// Gets all the complaints entered.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult AllComplaints(int? page)
        {
            List<DischargeComplaintModel> complaintList = new List<DischargeComplaintModel>();
            ///TODO: Need to fix the logic of retrieving the complaints
            using (var context = new lifeflightapps())
            {
                var complaints = context.tblDischargeComplaints.ToList().OrderByDescending(x => x.ComplaintEntereddatetime).Where(x => x.ComplaintEntereddatetime > DateTime.Now.AddMonths(-2));

                foreach (var complaint in complaints)
                {
                    var discharge = context.tblDischargeRequests1.Find(complaint.RequestID);
                    if (discharge != null)
                    {
                        DischargeComplaintModel model = new DischargeComplaintModel();
                        model = ConvertToModel(complaint);
                        complaintList.Add(model);
                    }
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(complaintList.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RequestId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult FullDetails(int RequestId)
        {
            DischargeViewModel model = new DischargeViewModel();
            DischargeDetailsController dischargeDetails = new DischargeDetailsController();
            model.RequestModel = dischargeDetails.PopulateModel(RequestId, true);
            model.ComplaintModel = new DischargeComplaintModel();
            model.DischargeActions = GetDischargeActions(model.RequestModel);
            model.AuditLogs = GetRequestHistory(RequestId);
            List<tblDischargeComplaint> complaints = new List<tblDischargeComplaint>();
            DateTime dtmLive = Convert.ToDateTime("05/21/2018");
            using (var context = new lifeflightapps())
            {
                complaints = context.tblDischargeComplaints.Where(x => x.RequestID == RequestId && x.ComplaintEntereddatetime > dtmLive).ToList();
            }
                List<DischargeComplaintModel> cModels = new List<DischargeComplaintModel>();
            foreach (var entry in complaints)
            {
                DischargeComplaintModel complaintModel = new DischargeComplaintModel();

                complaintModel = ConvertToModel(entry);
                cModels.Add(complaintModel);
            }
            model.ComplaintModels = cModels;
            return View(model);
        }

        /// <summary>
        /// Get the history of the request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public List<AuditLog> GetRequestHistory(int requestId)
        {
            List<AuditLog> logs = new List<AuditLog>();
            List<tblAuditLog> tblAuditLogs = new List<tblAuditLog>();

            using (var context = new lifeflightapps())
            {
                tblAuditLogs = context.tblAuditLogs.Where(x => x.IDFROMAPP == requestId).OrderBy(x => x.CREATEDON).ToList();
            }
            if (tblAuditLogs.Count > 0)
            {
                logs = ConvertToAuditLogs(tblAuditLogs);
            }

            return logs;
        }

        /// <summary>
        /// COnverts the DB entities to Model
        /// </summary>
        /// <param name="tblAuditLogs"></param>
        /// <returns></returns>
        private List<AuditLog> ConvertToAuditLogs(List<tblAuditLog> tblAuditLogs)
        {
            List<AuditLog> logs = new List<AuditLog>();

            foreach (var log in tblAuditLogs)
            {
                AuditLog auditLog = new AuditLog()
                {
                    RequestId = log.IDFROMAPP,
                    MrNumber = log.MRNUMBER,
                    Notes = log.UPDATENOTES,
                    CreatedBy = log.CREATEDBY,
                    CreatedOn = log.CREATEDON,
                    CreatedDateTime = log.CREATEDON.ToString() // .ToString("yyyy-MM-ddTHH:mm:ss")
            };
                logs.Add(auditLog);
            }
            return logs;

        }

        /// <summary>
        /// Gets the Discharge Actions
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private List<DischargeAction> GetDischargeActions(SubmittedRequestModel requestModel)
        {
            List<DischargeAction> Actions = new List<DischargeAction>();
            //1. Discharge Requested
            if (requestModel.CallReceivedAt != null)
            {
                DischargeAction action = new DischargeAction()
                {
                    Action = "Discharge Requested",
                    ActionNotes = $"Discharge request has been placed for patient {requestModel.submittedPatient.FirstName + " " + requestModel.submittedPatient.LastName}",
                    ActionTime = requestModel.CallReceivedAt.Value
                };
                Actions.Add(action);
            }
            //2. Get the pagelogs
            List<tblPageLog> pagelogs = new List<tblPageLog>();
            using (var context = new lifeflightapps())
            {
              pagelogs = context.tblPageLogs.Where(x => x.DischargeRequestId == requestModel.submittedDischarge.DischargeRequestId).ToList();
            }
                foreach (var page in pagelogs)
            {
                DischargeAction action = new DischargeAction()
                {
                    Action = page.MessageType,
                    ActionNotes = page.Message,
                    ActionTime = page.SentOn
                };
                Actions.Add(action);
            }
            //3. Agency Contacted
            if (requestModel.AgencyContacted != null)
            {
                DischargeAction action = new DischargeAction()
                {
                    Action = "Agency Contacted",
                    ActionNotes = $"{requestModel.EmsAgency} has been contacted",
                    ActionTime = requestModel.AgencyContacted.Value
                };
                Actions.Add(action);
            }
            //4. Agency Responded
            if (requestModel.AgencyResponded != null)
            {
                DischargeAction action = new DischargeAction()
                {
                    Action = "Agency Responded",
                    ActionNotes = $"{requestModel.EmsAgency} has confirmed pickup",
                    ActionTime = requestModel.AgencyResponded.Value
                };
                Actions.Add(action);
            }
            //5. Agency Arrived
            if (requestModel.AgencyArrived != null)
            {
                DischargeAction action = new DischargeAction()
                {
                    Action = "Agency has Arrived for pickup",
                    ActionNotes = $"{requestModel.EmsAgency} has arrived for pickup",
                    ActionTime = requestModel.AgencyArrived.Value
                };
                Actions.Add(action);
            }
            return Actions;
        }

        /// <summary>
        /// Converts the dbentities to model
        /// </summary>
        /// <param name="complaint"></param>
        /// <returns></returns>
        private DischargeComplaintModel ConvertToModel(tblDischargeComplaint complaint)
        {
            DischargeComplaintModel model = new DischargeComplaintModel();
            using (var context = new lifeflightapps())
            {
                var complaintType = context.tblDischargeComplaintTypes.Find(complaint.ComplaintTypeID).ComplaintType;
                string delayedReason = string.Empty;
                if (complaint.DelayReasonID != null)
                {
                    delayedReason = context.tblDelayReasons.Find(complaint.DelayReasonID).delayreason;
                }
                //var patientID = db.tblDischargeRequests1.Find(complaint.RequestID).PatientID;
                var discharge = context.tblDischargeRequests1.Find(complaint.RequestID);
                var mrn = context.tblPatients.Where(x => x.PatientID == discharge.PatientID).Select(x => x.MrNumber).FirstOrDefault();
                var location = context.tblMbrCensus.Find(discharge.LocationID);
                var agency = context.tblEmsAgencyLocals.Find(discharge.EmsAgencyId).Name;
                var destination = context.tblDischargeDestinations.Find(discharge.DestinationID);
                string goingTo = (destination.DestinationType == "Hospital/NursingHome") ? destination.DestinationName : destination.AddressLineOne + " " + destination.City + " " + destination.StateCode;
                string from = string.Empty;
                if (discharge.RequestType == "Discharge")
                {
                    from = location.Unit + "-" + location.Bed;
                }
                var patientName = context.tblPatients.Where(x => x.PatientID == discharge.PatientID).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                model = new DischargeComplaintModel()
                {
                    Complaint = complaint.Complaint,
                    CreatedBy = complaint.EnteredBy,
                    CreatedDate = complaint.ComplaintEntereddatetime,
                    RequestId = complaint.RequestID,
                    IsAocNotified = complaint.HospitalAOCNotified,
                    IsResolved = complaint.Resolved,
                    ResolvedDate = complaint.ResolvedDateTime,
                    ComplaintType = complaintType,
                    DelayedReason = delayedReason,
                    PatientName = patientName,
                    RequestDate = discharge.CallReceivedDate,
                    Location = from,
                    Agency = agency,
                    Destination = goingTo,
                    MRN = mrn
                };
            }
            return model;
        }
    }
}