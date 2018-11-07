using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class DischargeDetailsController : Controller
    {
        // GET: DischargeDetails

        /// <summary>
        /// Load the discharge info 
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        public ActionResult DischargeInfo(int DischargeRequestId)
        {
            bool Editable = true;//User.IsInRole("Admin");

            SubmittedRequestModel srModel = PopulateModel(DischargeRequestId, Editable);

            return View(srModel);
        }

        /// <summary>
        /// Populates the Model for the DischargeInfo page
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <param name="editable"></param>
        /// <returns></returns>
        public SubmittedRequestModel PopulateModel(int dischargeRequestId, bool editable)
        {
            SubmittedRequestModel model = new SubmittedRequestModel();

            tblDischargeRequest1 discharge = new tblDischargeRequest1();
            List<tblRequestStatu> status = new List<tblRequestStatu>();
            tblDischargeATNumber atN = new tblDischargeATNumber();

            string agencySelected = string.Empty;
            using (var context = new lifeflightapps())
            {
                discharge = context.tblDischargeRequests1.Find(dischargeRequestId);
                status = (from s in context.tblRequestStatus select s).ToList();
                atN = context.tblDischargeATNumbers.Where(a => a.RequestID == dischargeRequestId && a.IsActive == true).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (discharge.EmsAgencyId.HasValue)
                {
                    var ems = context.tblEmsAgencyLocals.Find(discharge.EmsAgencyId.Value);
                    agencySelected = ems.Name;
                    agencySelected += string.IsNullOrEmpty(ems.Phones) ? string.Empty : ", " + ems.Phones;
                }
            }
            model.RequestStatus = status.Where(i => i.ReqStatusID == discharge.RequestStatusID).Select(s => s.RequestStatus).FirstOrDefault();
            model.EnableEdit = editable;
            model.RequestType = discharge.RequestType;
            model.MOTSelected = discharge.ModeOfTransport;
            model.AgencyArrived = discharge.EMSAgencyArrived;
            model.AgencyContacted = discharge.EMSAgencyContacted;
            model.AgencyResponded = discharge.EMSAgencyResponded;
            model.CallReceivedBy = discharge.CallReceivedBy;
            model.CallReceivedAt = discharge.CallReceivedDate;
            model.VumcPayorReasons = GetVumcPayorReasons();
            model.Payors = GetPayors();

            model.CallReceivedBy = discharge.CallReceivedBy;
            if (atN != null)
            {
                model.ATNumber = atN.ATNumber;
                model.Cost = atN.Cost.HasValue?decimal.Round(atN.Cost.Value,2):0;
                model.PayingReason = atN.PayingReason;
                model.PONumber = atN.PONumber;
                model.InvoiceNumber = atN.InvoiceNumber;

                //var reason = model.VumcPayorReasons.Where(x => x.Text == atN.PayingReason).First();
                if (model.VumcPayorReasons.Where(x => x.Text == atN.PayingReason).Any())
                {
                    model.VumcPayorReasons.Where(x => x.Text == atN.PayingReason).First().Selected = true;
                }
                //model.VumcPayorReasons.Where(x => x.Text == atN.PayingReason).First().Selected = true;
                //model.VumcPayorReasons.First(x => x.Text == atN.PayingReason).Selected = true;
                if (!model.VumcPayorReasons.Where(x => x.Selected == true).Any())
                {
                    model.VumcPayorReasons.Add(new SelectListItem { Text = atN.PayingReason, Value = atN.PayingReason, Selected = true });
                }
                //foreach (var item in model.VumcPayorReasons)
                //{
                //    if (item.Text == atN.PayingReason)
                //    {
                //        item.Selected = true;
                //    }
                //}


                foreach (var item in model.Payors)
                {
                    if (item.Text == atN.Payor)
                    {
                        item.Selected = true;
                    }
                }
            }
            //srModel.ATNumber = atN.ATNumber;


            if (discharge.PickupID > 0)
            {
                model.submittedPickup = GetPickup(discharge.PickupID);
            }

            //srModel.TransportModes = GetTransportModes(discharge.ModeOfTransport);



            model.EmsAgency = agencySelected;

            //srModel.EmsAgencies = new List<SelectListItem>(GetEmsAgencies(discharge.ModeOfTransport, srModel.EmsAgency).OrderBy(x=>x.Value));

            model.Statuses = GetStatus(model.RequestStatus);

            model.SpecialNeedsSubmitted = GetNeeds(discharge.PatientID);

            model.submittedDischarge = GetDischarge(dischargeRequestId);


            model.submittedPatient = GetPatient(discharge.PatientID);

            model.submittedCaller = GetCaller(discharge.CallerID);

            model.submittedDestination = GetDestination(discharge.DestinationID);
            model.submittedCensus = GetCensus(discharge.LocationID);

            if (model.submittedCensus.PavillionCode != "N/A")
            {
                model.LocationDetails = model.submittedCensus.PavillionCode + "||" + model.submittedCensus.Unit + "||" + model.submittedCensus.Bed;
                model.LocationDetails += string.IsNullOrEmpty(model.submittedCensus.LocationPhone) ? "" : " - Ph#: " + model.submittedCensus.LocationPhone;
            }
            else
            {
                model.LocationDetails = "No Census Info available for this patient";
            }

            model.submittedInsurance = GetInsurance(discharge.InsuranceID);
            string insuranceName = model.submittedInsurance.Payorname;
            if ((!string.IsNullOrEmpty(insuranceName)) && (insuranceName != "N/A"))
            {
                int insuranceId = 0;
                using (var context = new lifeflightapps())
                {
                    var ins = context.tblInsuranceAgencies.Where(x => x.INSAGENCYNAME == insuranceName);
                    if (ins != null && ins.Count() > 0)
                    {
                        insuranceId = ins.FirstOrDefault().INSURANCEID;
                    }
                }
                if (insuranceId != 0)
                {
                    model.insuranceDetails = GetInsuranceDetails(insuranceId);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the insurance details for the request
        /// </summary>
        /// <param name="insuranceId"></param>
        /// <returns></returns>
        private InsuranceModel GetInsuranceDetails(int insuranceId)
        {
            InsuranceModel insuranceModel = new InsuranceModel();
            InsuranceAgencyController insuranceAgency = new InsuranceAgencyController();
            insuranceModel = insuranceAgency.GetInsuranceFromDB(insuranceId);
            return insuranceModel;
        }

        /// <summary>
        /// Gets the VUMC Payor Reasons
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetVumcPayorReasons()
        {
            List<SelectListItem> Reasons = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                var payorReasons = context.tblVUMCPayorReasons.ToList();
                foreach (var item in payorReasons)
                {
                    SelectListItem reasonsToView = new SelectListItem()
                    {
                        Value = item.Reason,
                        Text = item.Reason
                    };
                    Reasons.Add(reasonsToView);
                }
            }
            return Reasons;
        }

        /// <summary>
        /// Gets the Payors
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetPayors()
        {
            List<SelectListItem> Payors = new List<SelectListItem>
            {
                new SelectListItem() { Value = "VUMC", Text = "VUMC" },
                new SelectListItem() { Value = "VCH", Text = "VCH" },
                new SelectListItem() { Value = "VPH", Text = "VPH" },
                new SelectListItem() { Value = "Complex Care", Text = "Complex Care" },
                new SelectListItem() { Value = "Other", Text = "Other" },
                new SelectListItem() { Value = "Stallworth", Text = "Stallworth" }
            };
            return Payors.OrderBy(x => x.Text).ToList();
        }

        /// <summary>
        /// Gets the insurance information
        /// </summary>
        /// <param name="insuranceID"></param>
        /// <returns></returns>
        private InsuranceInformation GetInsurance(int insuranceID)
        {
            tblMbrInsurance insurance = new tblMbrInsurance();
            using (var context = new lifeflightapps())
            {
                insurance = context.tblMbrInsurances.Find(insuranceID);
            }
            InsuranceInformation submittedInsurance = new InsuranceInformation()
            {
                MbrInsuranceId = insurance.MbrInsuranceID,
                InsuranceId = insurance.InsuranceID,
                SubscriberName = insurance.SubscriberName,
                PlanId = insurance.PlanID.ToString(),
                PlanName = insurance.PlanName,
                PlanType = insurance.PlanType,
                PayorId = insurance.PayorID.ToString(),
                Payorname = insurance.PayorName,
                MedipacPlanId = insurance.MedipacPlanID,
                FilingOrder = insurance.FillingOrder,
                FinancialName = insurance.FinancialName,
                EffectiveDate = insurance.Effective,
                TerminationDate = insurance.Termination,
                AuthCode = insurance.AuthCode,
                ICDCode = insurance.ICDCode
            };
            return submittedInsurance;
        }

        /// <summary>
        /// Gets the census details
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        private CensusModel GetCensus(int locationID)
        {
            tblMbrCensu location = new tblMbrCensu();
            string locationPhone = string.Empty;
            using (var context = new lifeflightapps())
            {
                location = context.tblMbrCensus.Find(locationID);
                if (location.PavillionCode != "N/A")
                {
                    locationPhone = context.tblDischargeRequestUnits.Where(c => c.ReqUnit.StartsWith(location.Unit)).Select(c => c.UnitPhone).FirstOrDefault().ToString();
                }
            }
            CensusModel submittedCensus = new CensusModel()
            {
                PavillionCode = location.PavillionCode,
                Unit = location.Unit,
                Bed = location.Bed,
                LocationPhone = locationPhone
            };

            return submittedCensus;
        }

        /// <summary>
        /// Gets the destination information
        /// </summary>
        /// <param name="destinationID"></param>
        /// <returns></returns>
        private Destination GetDestination(int destinationID)
        {
            tblDischargeDestination destination = new tblDischargeDestination();
            using (var context = new lifeflightapps())
            {
                destination = context.tblDischargeDestinations.Find(destinationID);
            }

            Destination submittedDestination = new Destination()
            {
                DestinationType = destination.DestinationType,
                DestinationName = destination.DestinationName,
                DestInstructions = destination.DestInstructions,
                Address = destination.AddressLineOne,
                City = destination.City,
                State = destination.StateCode,
                Zip = destination.Zip,
                Phone = destination.Phone,
                Miles = destination.Miles,
                TravelTime = destination.TravelTime.HasValue ? destination.TravelTime.Value.ToString(@"hh\:mm") : string.Empty
            };

            return submittedDestination;
        }

        /// <summary>
        /// Gets the caller information
        /// </summary>
        /// <param name="callerID"></param>
        /// <returns></returns>
        private Caller GetCaller(int? callerID)
        {
            if (callerID.HasValue && callerID.Value != 0)
            {
                tblCaller caller = new tblCaller();
                using (var context = new lifeflightapps())
                {
                    caller = context.tblCallers.Find(callerID);
                }

                Caller submittedCaller = new Caller()
                {
                    CallerId = caller.ID,
                    CallerTitle = caller.CallerTitle,
                    CallerFirstName = caller.CallerFirstName,
                    CallerLastName = caller.CallerLastName,
                    Assignment = caller.Assignment,
                    OfficePhone = caller.OfficePhone,
                    CallerEmail = caller.CallerEmail,
                    CallerPager = caller.CallerPager,
                    MobilePhone = caller.MobilePhone
                };

                return submittedCaller;
            }
            else
            {
                Caller caller = new Caller();
                return caller;
            }
            
        }

        /// <summary>
        /// Gets the patient information
        /// </summary>
        /// <param name="patientID"></param>
        /// <returns></returns>
        private Patient GetPatient(int patientID)
        {
            tblPatient patient = new tblPatient();
            using (var context = new lifeflightapps())
            {
                patient = context.tblPatients.Find(patientID);
            }
            Patient submittedPatient = new Patient()
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DOB = patient.DateOfBirth.HasValue ? patient.DateOfBirth.Value.ToShortDateString() : "N/A",
                Age = patient.Age,
                Weight = patient.Weight.HasValue ? patient.Weight.Value.ToString() : "N/A",
                Social = patient.Social,
                MRN = patient.MrNumber,
                Phone = patient.Phone,
                Address = patient.Address.Replace(",", ""),
                City = patient.City,
                State = patient.State,
                Zip = patient.Zip,
                PatientId = patient.PatientID
            };
            return submittedPatient;
        }

        /// <summary>
        /// Gets the discharge details 
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <returns></returns>
        private DischargeRequestModel GetDischarge(int dischargeRequestId)
        {
            tblDischargeRequest1 discharge = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                discharge = context.tblDischargeRequests1.Find(dischargeRequestId);
            }

            DischargeRequestModel dischargeRequestModel = new DischargeRequestModel()
            {
                DischargeRequestId = discharge.RequestID,
                DischargeTime = discharge.DischargeTime?.ToString("MM/dd/yyyy HH:mm") ?? "",
                AgencyArrivalTime = discharge.EMSAgencyArrived?.ToString("MM/dd/yyyy HH:mm") ??"",
                ModeOfTransport = discharge.ModeOfTransport,
                SpecialInstructions = discharge.SpecialInstructions,
                Notes = discharge.Notes,
                RequestStatusID = discharge.RequestStatusID,
                LifeSupport = string.IsNullOrEmpty(discharge.LifeSupport) ? "N/A" : discharge.LifeSupport,
                MrNumber = discharge.MrNumber,
                CaseNumber = discharge.CaseNumber
            };

            return dischargeRequestModel;
        }

        /// <summary>
        /// Get the special needs selected for the patient
        /// </summary>
        /// <param name="patientID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetNeeds(int patientID)
        {

            List<SelectListItem> snss = new List<SelectListItem>();
            List<SelectListItem> sneeds = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                var splNeed = context.tblSpecialNeeds.Select(i => new { i.SpecialNeed, i.SpecialNeedID }).ToList();
                foreach (var item in splNeed)
                {
                    SelectListItem sneed = new SelectListItem()
                    {
                        Text = item.SpecialNeed,
                        Value = item.SpecialNeedID.ToString()
                    };
                    sneeds.Add(sneed);
                }
                var dischargeNeeds = context.tblDischargeNeeds.Where(i => i.patientid == patientID && i.active == true).ToList();
                foreach (var item in dischargeNeeds)
                {
                    foreach (var need in sneeds)
                    {
                        if (item.specialneedsid.ToString() == need.Value)
                        {
                            need.Selected = true;
                        }
                    }
                }
            }
            return sneeds;
        }

        /// <summary>
        /// Gets the request statuses
        /// </summary>
        /// <param name="requestStatus"></param>
        /// <returns></returns>
        public List<SelectListItem> GetStatus(string requestStatus)
        {
            List<SelectListItem> Status = new List<SelectListItem>();
            List<tblRequestStatu> statusFromDb = new List<tblRequestStatu>();
            using (var context = new lifeflightapps())
            {
                statusFromDb = context.tblRequestStatus.ToList();
            }
            foreach (var item in statusFromDb)
            {
                SelectListItem statusToView = new SelectListItem()
                {
                    Value = item.RequestStatus,
                    Text = item.RequestStatus
                };
                Status.Add(statusToView);
            }

            if (!string.IsNullOrEmpty(requestStatus))
            {
                foreach (var item in Status)
                {
                    if (item.Value == requestStatus)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            return Status;
        }

        /// <summary>
        /// Gets the EMS Agencies
        /// </summary>
        /// <param name="modeOfTransport"></param>
        /// <param name="emsAgency"></param>
        /// <param name="isLocal"></param>
        /// <returns></returns>
        public List<SelectListItem> GetEmsAgencies(string modeOfTransport, string emsAgency, bool isLocal)
        {
            List<SelectListItem> agencies = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                //1. Get all agencies
                var ems = context.tblEmsAgencyLocals.Where(x => x.Type == modeOfTransport && x.LocalUse == isLocal).ToList();

                if (isLocal)
                {
                    ems.ForEach(x =>
                    {
                        agencies.Add(new SelectListItem { Text = x.Name, Value = x.Name });
                    });
                }
                else
                {
                    // Local is false so append state code to the name of agency
                    ems.ForEach(x =>
                    {
                        agencies.Add(new SelectListItem { Text = x.Name + " - " + x.State, Value = x.Name });
                    });
                }
                agencies.OrderBy(x => x.Value).ToList();
                if ((!string.IsNullOrEmpty(emsAgency)) && (emsAgency != "N/A"))
                {
                    var emsSelected = context.tblEmsAgencyLocals.FirstOrDefault(x => x.Name == emsAgency);
                    if (emsSelected != null)
                    {
                        agencies.FirstOrDefault(x => x.Value == emsAgency).Selected = true;
                    }
                }

                else
                {
                    SelectListItem dummy = new SelectListItem()
                    {
                        Value = "-Choose Agency-",
                        Text = "-Choose Agency-",
                        Selected = true
                    };
                    agencies.Add(dummy);
                }
            }
            return agencies;
        }

        /// <summary>
        /// Get the EMS agencies for the mode of transport
        /// </summary>
        /// <param name="transMode"></param>
        /// <param name="LocalUse"></param>
        /// <returns></returns>
        public List<SelectListItem> GetAgencies(string transMode, bool LocalUse)
        {
            List<SelectListItem> emsAgencies = new List<SelectListItem>();
            List<tblEmsAgencyLocal> ems = new List<tblEmsAgencyLocal>();
            using (var context = new lifeflightapps())
            {


                ems = context.tblEmsAgencyLocals.Where(x => x.Type == transMode && x.LocalUse == LocalUse).ToList();
            }
            if (!LocalUse)
            {
                ems.ForEach(x =>
                {
                    emsAgencies.Add(new SelectListItem { Text = x.Name + " - " + x.State, Value = x.Name });
                });
            }
            else
            {
                ems.ForEach(x =>
                {
                    emsAgencies.Add(new SelectListItem { Text = x.Name, Value = x.Name });
                });
            }

            return emsAgencies;
        }

        /// <summary>
        /// Gets the transport modes
        /// </summary>
        /// <param name="modeOfTransport"></param>
        /// <returns></returns>
        public List<SelectListItem> GetTransportModes(string modeOfTransport)
        {
            //srModel.EmsAgencySelected = if(discharge.EmsAgencyId.HasValue)
            List<SelectListItem> modes = new List<SelectListItem>();
            List<tblTransportMode> tmodes = new List<tblTransportMode>();
            using (var context = new lifeflightapps())
            {
                //fetch the transportmodes and display the selected one.
                tmodes = context.tblTransportModes.ToList();
            }

            foreach (var item in tmodes)
            {
                SelectListItem transmodeitem = new SelectListItem()
                {
                    Text = item.TransportMode,
                    Value = item.TransportMode
                };

                modes.Add(transmodeitem);
            }

            foreach (var item in modes)
            {
                if (item.Value == modeOfTransport)
                {
                    item.Selected = true;
                    break;
                }
            }
            return modes;
        }

        /// <summary>
        /// Gets the pickup information
        /// </summary>
        /// <param name="pickupID"></param>
        /// <returns></returns>
        private PickupModel GetPickup(int? pickupID)
        {
            tblDischargePickup pickup = new tblDischargePickup();
            using (var context = new lifeflightapps())
            {
                pickup = context.tblDischargePickups.Find(pickupID);
            }
            PickupModel pickupModel = new PickupModel()
            {
                PickupId = pickup.PickupID,
                PickupLocationName = pickup.PickupLocationName,
                PickupInstructions = pickup.PickupInstructions,
                AddressLineOne = pickup.AddressLineOne,
                City = pickup.City,
                State = pickup.StateCode,
                Zip = pickup.Zip
            };
            return pickupModel;

        }

        /// <summary>
        /// Generates the Windsor PDF Document
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        public void GenerateWindsorPdf(int DischargeRequestId)
        {
            tblDischargeRequest1 dr = new tblDischargeRequest1();
            tblPatient patient = new tblPatient();
            tblMbrCensu location = new tblMbrCensu();
            tblDischargeDestination destination = new tblDischargeDestination();
            tblMbrInsurance insurance = new tblMbrInsurance();
            tblEmsAgencyLocal agency = new tblEmsAgencyLocal();
            using (var context = new lifeflightapps())
            {


                //Get the data:
                dr = context.tblDischargeRequests1.Find(DischargeRequestId);
                patient = context.tblPatients.Find(dr.PatientID);
                location = context.tblMbrCensus.Find(dr.LocationID);
                destination = context.tblDischargeDestinations.Find(dr.DestinationID);
                insurance = context.tblMbrInsurances.Find(dr.InsuranceID);
                agency = context.tblEmsAgencyLocals.Find(dr.EmsAgencyId);
                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = DischargeRequestId,
                    MRNUMBER = dr.MrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = "Windsor PDF document has been generated"
                };

                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }
            // Create a Document object
            var document = new Document(PageSize.A4, 50, 50, 25, 25);

            // Create a new PdfWrite object, writing the output to a MemoryStream
            var output = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, output);

            // First, create our fonts... (For more on working w/fonts in iTextSharp, see: http://www.mikesdotnetting.com/Article/81/iTextSharp-Working-with-Fonts
            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
            var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
            //document.Header = new HeaderFooter(new Phrase("Vanderbilt University Medical Center"), false);

            var firstPageFont = FontFactory.GetFont("Arial", 16, Font.BOLD);

            // Parameters passed on to the function that creates the PDF 
            string headerText = "Vanderbilt University Medical Center";
            string footerText = $"Faxed by Vanderbilt DischargeTransportation Coordinator Office : {DateTime.Now}";

            // Define a font and font-size in points (plus f for float) and pick a color
            // This one is for both header and footer but you can also create seperate ones
            //Font fontHeaderFooter = FontFactory.GetFont("CenturySchoolbook", 20f);
            //fontHeaderFooter.Color = Color.GRAY;
            BaseFont BaseFontHeaderFooter = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
            //Font fontHeaderFooter = new Font(BaseFontHeaderFooter, 20, Font.BOLDITALIC, Color.GRAY);
            Font fontHeaderFooter = FontFactory.GetFont("Arial", 20, Font.BOLDITALIC, Color.GRAY);
            Font receivedInError = FontFactory.GetFont("Arial", 16, Font.ITALIC, Color.RED);

            // Apply the font to the headerText and create a Phrase with the result
            Chunk chkHeader = new Chunk(headerText, fontHeaderFooter);
            Phrase p1 = new Phrase(chkHeader);
            // create a HeaderFooter element for the header using the Phrase
            // The boolean turns numbering on or off
            HeaderFooter header = new HeaderFooter(p1, false)
            {

                // Remove the border that is set by default
                Border = Rectangle.NO_BORDER,
                // Align the text: 0 is left, 1 center and 2 right.
                Alignment = 1
            };

            // add the header to the document
            document.Header = header;

            // If you want to use numbering like in this example, add a whitespace to the
            // text because by default there's no space in between them
            if (footerText.Substring(footerText.Length - 1) != " ") footerText += " ";

            Chunk chkFooter = new Chunk(footerText, fontHeaderFooter);
            Phrase p2 = new Phrase(chkFooter);

            // Turn on numbering by setting the boolean to true
            HeaderFooter footer = new HeaderFooter(p2, false)
            {
                Border = Rectangle.NO_BORDER,
                Alignment = 1
            };

            document.Footer = footer;

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
            Font times = new Font(bfTimes, 12, Font.BOLDITALIC, Color.RED);
            Font details = FontFactory.GetFont("CenturySchoolbook", 20, Font.ITALIC);
            document.Open();
            /* this shows the details as a list
             * 
            List list = new List(List.UNORDERED);
            list.Add(new ListItem($"Patient's Name: (Last, First): {patient.LastName + ", " + patient.FirstName}", details));
            list.Add(new ListItem($"Patient's DOB: {patient.DateOfBirth}", details));
            list.Add(new ListItem($"Member ID: {insurance.InsuranceID}", details));
            list.Add(new ListItem("Ordering Physician: ", details));
            list.Add(new ListItem($"Pickup Location: Vanderbilt Medical Center, 1211 Medical Center Dr, Nashville, TN 37232 {location.Unit + " " + location.Bed}", details));
            list.Add(new ListItem($"Destination: {destination.AddressLineOne + " " + destination.City + " " + destination.StateCode + " " + destination.Zip}", details));
            list.Add(new ListItem($"Date Of Transport: {dr.DischargeTime}", details));
            list.Add(new ListItem("ICD-9 Code:", details));
            list.Add(new ListItem("Diagnosis:", details));
            list.Add(new ListItem($"Ambulance Agency: {agency.Name}", details));
            list.Add(new ListItem("Completed By:", details));
            
            *
            */
            Paragraph paraTitle = new Paragraph("Windsor Medicare Non-Emergent Ambulance Authorization Request", details);
            paraTitle.SetAlignment("Center");

            document.Add(paraTitle);
            document.Add(new Phrase("\n"));

            var printableTable = new PdfPTable(2)
            {
                HorizontalAlignment = 0,
                SpacingBefore = 10,
                SpacingAfter = 10
            };
            printableTable.DefaultCell.Border = 1;
            printableTable.SetWidths(new int[] { 2, 4 });
            printableTable.SpacingAfter = 2;
            printableTable.WidthPercentage = 100;

            PdfPCell cell = new PdfPCell(new Phrase(string.Format("Authorization request for {0}", patient.LastName.ToUpper() + ", " + patient.FirstName.ToUpper())))
            {
                Colspan = 3,
                HorizontalAlignment = 1 //0=Left, 1=Centre, 2=Right
            };


            printableTable.AddCell(cell);

            printableTable.AddCell(new PdfPCell(new Phrase("Patient's Name: {last, First}", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(patient.LastName + ", " + patient.FirstName)));

            printableTable.AddCell(new PdfPCell(new Phrase("Patient's DOB:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(patient.DateOfBirth.Value.ToShortDateString())));

            printableTable.AddCell(new PdfPCell(new Phrase("MemberID", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(insurance.InsuranceID.ToString())));

            printableTable.AddCell(new PdfPCell(new Phrase("Ordering Physician", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase()));

            printableTable.AddCell(new PdfPCell(new Phrase("Pickup Location", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase($"Vanderbilt Medical Center, 1211 Medical Center Dr, Nashville, TN 37232 { location.Unit + " " + location.Bed }")));

            printableTable.AddCell(new PdfPCell(new Phrase("Destination", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(destination.AddressLineOne + " " + destination.City + " " + destination.StateCode + " " + destination.Zip)));

            printableTable.AddCell(new PdfPCell(new Phrase("ICD-9 Code:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase()));

            printableTable.AddCell(new PdfPCell(new Phrase("Diagnosis", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase()));

            printableTable.AddCell(new PdfPCell(new Phrase("Ambulance Agency", boldTableFont)));
            if (agency != null)
            {
                printableTable.AddCell(new PdfPCell(new Phrase(agency.Name)));
            }
            else
            {
                printableTable.AddCell(new PdfPCell(new Phrase("Not Selected Yet")));
            }

            printableTable.AddCell(new PdfPCell(new Phrase("Completed By", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase()));

            document.Add(printableTable);

            document.Add(new Phrase("\n"));
            document.Add(new Phrase("\n"));


            Paragraph paraEnd = new Paragraph("Please contact the Vanderbilt Discharge Transportation Coordinator with questions 615-322-7433 or fax: 615-343-5737", details);
            paraEnd.SetAlignment("Justify");

            document.Add(paraEnd);



            document.Close();

            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", string.Format("inline;filename=DischargeRequest-{0}.pdf", DischargeRequestId));
            Response.BinaryWrite(output.ToArray());
        }

        /// <summary>
        /// Creates the WellCare PDF document
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        public void GenerateWellCarePdf(int DischargeRequestId)
        {
            tblDischargeRequest1 dr = new tblDischargeRequest1();
            tblPatient patient = new tblPatient();
            tblMbrCensu location = new tblMbrCensu();
            tblDischargeDestination destination = new tblDischargeDestination();
            tblMbrInsurance insurance = new tblMbrInsurance();
            tblEmsAgencyLocal agency = new tblEmsAgencyLocal();
            using (var context = new lifeflightapps())
            {


                //Get the data:
                dr = context.tblDischargeRequests1.Find(DischargeRequestId);
                patient = context.tblPatients.Find(dr.PatientID);
                location = context.tblMbrCensus.Find(dr.LocationID);
                destination = context.tblDischargeDestinations.Find(dr.DestinationID);
                insurance = context.tblMbrInsurances.Find(dr.InsuranceID);
                agency = context.tblEmsAgencyLocals.Find(dr.EmsAgencyId);
                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = DischargeRequestId,
                    MRNUMBER = dr.MrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = "WellCare PDF document has been generated"
                };

                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }
            // Create a Document object
            var document = new Document(PageSize.A4, 50, 50, 25, 25);

            // Create a new PdfWrite object, writing the output to a MemoryStream
            var output = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, output);

            // First, create our fonts... (For more on working w/fonts in iTextSharp, see: http://www.mikesdotnetting.com/Article/81/iTextSharp-Working-with-Fonts
            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
            var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
            //document.Header = new HeaderFooter(new Phrase("Vanderbilt University Medical Center"), false);

            var firstPageFont = FontFactory.GetFont("Arial", 16, Font.BOLD);

            // Parameters passed on to the function that creates the PDF 
            string headerText = "Vanderbilt University Medical Center";
            string footerText = $"Faxed by Vanderbilt DischargeTransportation Coordinator Office : {DateTime.Now}";

            // Define a font and font-size in points (plus f for float) and pick a color
            // This one is for both header and footer but you can also create seperate ones
            //Font fontHeaderFooter = FontFactory.GetFont("CenturySchoolbook", 20f);
            //fontHeaderFooter.Color = Color.GRAY;
            BaseFont BaseFontHeaderFooter = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
            //Font fontHeaderFooter = new Font(BaseFontHeaderFooter, 20, Font.BOLDITALIC, Color.GRAY);
            Font fontHeaderFooter = FontFactory.GetFont("Arial", 20, Font.BOLDITALIC, Color.GRAY);
            Font receivedInError = FontFactory.GetFont("Arial", 16, Font.ITALIC, Color.RED);

            // Apply the font to the headerText and create a Phrase with the result
            Chunk chkHeader = new Chunk(headerText, fontHeaderFooter);
            Phrase p1 = new Phrase(chkHeader);
            // create a HeaderFooter element for the header using the Phrase
            // The boolean turns numbering on or off
            HeaderFooter header = new HeaderFooter(p1, false)
            {

                // Remove the border that is set by default
                Border = Rectangle.NO_BORDER,
                // Align the text: 0 is left, 1 center and 2 right.
                Alignment = 1
            };

            // add the header to the document
            document.Header = header;

            // If you want to use numbering like in this example, add a whitespace to the
            // text because by default there's no space in between them
            if (footerText.Substring(footerText.Length - 1) != " ") footerText += " ";

            Chunk chkFooter = new Chunk(footerText, fontHeaderFooter);
            Phrase p2 = new Phrase(chkFooter);

            // Turn on numbering by setting the boolean to true
            HeaderFooter footer = new HeaderFooter(p2, false)
            {
                Border = Rectangle.NO_BORDER,
                Alignment = 1
            };

            document.Footer = footer;

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
            Font times = new Font(bfTimes, 12, Font.BOLDITALIC, Color.RED);
            Font details = FontFactory.GetFont("CenturySchoolbook", 20, Font.ITALIC);
            document.Open();
            /* this shows the details as a list
             * 
            List list = new List(List.UNORDERED);
            list.Add(new ListItem($"Patient's Name: (Last, First): {patient.LastName + ", " + patient.FirstName}", details));
            list.Add(new ListItem($"Patient's DOB: {patient.DateOfBirth}", details));
            list.Add(new ListItem($"Member ID: {insurance.InsuranceID}", details));
            list.Add(new ListItem("Ordering Physician: ", details));
            list.Add(new ListItem($"Pickup Location: Vanderbilt Medical Center, 1211 Medical Center Dr, Nashville, TN 37232 {location.Unit + " " + location.Bed}", details));
            list.Add(new ListItem($"Destination: {destination.AddressLineOne + " " + destination.City + " " + destination.StateCode + " " + destination.Zip}", details));
            list.Add(new ListItem($"Date Of Transport: {dr.DischargeTime}", details));
            list.Add(new ListItem("ICD-9 Code:", details));
            list.Add(new ListItem("Diagnosis:", details));
            list.Add(new ListItem($"Ambulance Agency: {agency.Name}", details));
            list.Add(new ListItem("Completed By:", details));
            
            *
            */
            Paragraph paraTitle = new Paragraph("WellCare - Kentucky Medicaid", details);
            paraTitle.SetAlignment("Center");
            Paragraph paraTitle1 = new Paragraph("Non-Emergency Ambulance Request", details);
            paraTitle1.SetAlignment("Center");

            document.Add(paraTitle);
            document.Add(paraTitle1);
            document.Add(new Phrase("\n"));

            var printableTable = new PdfPTable(2)
            {
                HorizontalAlignment = 0,
                SpacingBefore = 10,
                SpacingAfter = 10
            };
            printableTable.DefaultCell.Border = 1;
            printableTable.SetWidths(new int[] { 2, 4 });
            printableTable.SpacingAfter = 2;
            printableTable.WidthPercentage = 100;

            PdfPCell cell = new PdfPCell(new Phrase(string.Format("Authorization request for {0}", patient.LastName.ToUpper() + ", " + patient.FirstName.ToUpper())))
            {
                Colspan = 3,
                HorizontalAlignment = 1 //0=Left, 1=Centre, 2=Right
            };
            string DateOfTransport = string.Empty;
            string TimeOfTransport = string.Empty;

            if (dr.DischargeTime.HasValue)
            {
                DateOfTransport = dr.DischargeTime.Value.ToShortDateString();
                TimeOfTransport = dr.DischargeTime.Value.ToShortTimeString();
            }
            printableTable.AddCell(cell);
            printableTable.AddCell(new PdfPCell(new Phrase("Today's Date", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToShortDateString())));

            printableTable.AddCell(new PdfPCell(new Phrase("Patient's Name: {last, First}", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(patient.LastName + ", " + patient.FirstName)));

            printableTable.AddCell(new PdfPCell(new Phrase("Patient's DOB:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(patient.DateOfBirth.Value.ToShortDateString())));

            printableTable.AddCell(new PdfPCell(new Phrase("WellCare ID", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(insurance.InsuranceID.ToString())));

            printableTable.AddCell(new PdfPCell(new Phrase("Date of Transport", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(DateOfTransport)));

            printableTable.AddCell(new PdfPCell(new Phrase("Time of Transport", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(TimeOfTransport)));

            printableTable.AddCell(new PdfPCell(new Phrase("Ordering Physician", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase()));

            printableTable.AddCell(new PdfPCell(new Phrase("Pickup Location", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase($"Vanderbilt Medical Center, 1211 Medical Center Dr, Nashville, TN 37232 { location.Unit + " " + location.Bed }")));

            printableTable.AddCell(new PdfPCell(new Phrase("Destination", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(destination.AddressLineOne + " " + destination.City + " " + destination.StateCode + " " + destination.Zip)));

            printableTable.AddCell(new PdfPCell(new Phrase("ICD-9 Code:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase()));

            printableTable.AddCell(new PdfPCell(new Phrase("Ambulance Service", boldTableFont)));
            if (agency != null)
            {
                printableTable.AddCell(new PdfPCell(new Phrase(agency.Name)));
            }
            else
            {
                printableTable.AddCell(new PdfPCell(new Phrase("Not Selected Yet")));
            }

            printableTable.AddCell(new PdfPCell(new Phrase("HCPCS", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase()));

            document.Add(printableTable);

            document.Add(new Phrase("\n"));
            document.Add(new Phrase("\n"));

            Paragraph paraMiddle = new Paragraph("Please return authorization via FAX to 615-343-5737", details);
            paraMiddle.SetAlignment("Justify");
            document.Add(paraMiddle);

            document.Add(new Phrase("\n"));
            document.Add(new Phrase("\n"));

            Paragraph paraEnd = new Paragraph("Please contact the Vanderbilt Discharge Transportation Coordinator with questions 615-322-7433 or fax: 615-343-5737", details);
            paraEnd.SetAlignment("Justify");

            document.Add(paraEnd);



            document.Close();

            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", string.Format("inline;filename=DischargeRequest-{0}.pdf", DischargeRequestId));
            Response.BinaryWrite(output.ToArray());
        }

        //private IEnumerable<SelectListItem> GetEmsAgencies(string modeOfTransport)
        //{
        //    IEnumerable<SelectListItem> emsAgencies;
        //    using (lifeflightapps db = new lifeflightapps())
        //    {
        //        var ems = db.tblEmsAgencyLocals.Where(x => x.Type != null && x.Type == modeOfTransport).Select(e => new SelectListItem()
        //        {
        //            Value = e.EMSID.ToString(),
        //            Text = e.Name
        //        });
        //        emsAgencies = ems;
        //    }
        //    return emsAgencies;
        //}

        #region Ajax Calls
        /// <summary>
        /// Gets the caller information
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCallerDetails(string name)
        {
            CallerDetails fill = new CallerDetails();
            using (var context = new lifeflightapps())
            {
                var callers = context.tblCallers.Select(c => new { c.ID, c.CallerFirstName, c.CallerLastName, c.CallerTitle, c.Assignment, c.OfficePhone, c.CallerPager, c.MobilePhone, c.CallerEmail }).ToList();

                var caller = callers.Where(c => c.CallerLastName + " " + c.CallerFirstName == name).FirstOrDefault();
                fill = new CallerDetails()
                {
                    CallerId = caller.ID,
                    Name = caller.CallerLastName + " " + caller.CallerFirstName,
                    PhoneNumber = caller.MobilePhone.HasValue ? caller.MobilePhone.ToString() : "N/A",
                    PagerNumber = string.IsNullOrEmpty(caller.CallerPager) ? "N/A" : caller.CallerPager,
                    Email = string.IsNullOrEmpty(caller.CallerEmail) ? "N/A" : caller.CallerEmail,
                    Title = string.IsNullOrEmpty(caller.CallerTitle) ? "N/A" : caller.CallerTitle,
                    Assignment = string.IsNullOrEmpty(caller.Assignment) ? "N/A" : caller.Assignment
                };
            }
            return Json(fill, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update the patient information
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="DOB"></param>
        /// <param name="Age"></param>
        /// <param name="Weight"></param>
        /// <param name="Phone"></param>
        /// <param name="Social"></param>
        /// <param name="Address"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Zip"></param>
        /// <param name="Mrn"></param>
        /// <param name="Csn"></param>
        /// <param name="Pid"></param>
        /// <returns></returns>
        public JsonResult UpdatePatientInfo(string FirstName, string LastName, DateTime DOB, int Age, int Weight, string Phone, string Social, string Address, string City, string State, string Zip, string Mrn, string Csn, int Pid)
        {
            tblPatient patient = new tblPatient();
            tblDischargeRequest1 drs = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                patient = context.tblPatients.Find(Pid);
                drs = context.tblDischargeRequests1.Where(x => x.PatientID == Pid).FirstOrDefault();


                string updatenotes = "Updates: ";

                if (patient.FirstName != FirstName)
                {
                    updatenotes += $"Patient first name has been updated from {patient.FirstName} to {FirstName}";
                }
                if (patient.LastName != LastName)
                {
                    updatenotes += $" Patient last name has been updated from {patient.LastName} to {LastName}";
                }
                if (patient.DateOfBirth != DOB)
                {
                    updatenotes += $" Patient DOB has been updated from {patient.DateOfBirth} to {DOB}";
                }
                if (patient.Age != Age)
                {
                    updatenotes += $" Patient age has been updated from {patient.Age} to {Age}";
                }
                if (patient.Weight != Weight)
                {
                    updatenotes += $" Patient weight has been updated from {patient.Weight} to {Weight}";
                }
                if (patient.Phone != Phone)
                {
                    updatenotes += $" Patient Phone# has been updated from {patient.Phone} to {Phone}";
                }
                if (patient.Social != Social)
                {
                    updatenotes += $" Patient social has been updated from {patient.Social} to {Social}";
                }
                if (patient.Address != Address)
                {
                    updatenotes += $" Patient Address has been updated from {patient.Address} to {Address}";
                }
                if (patient.City != City)
                {
                    updatenotes += $" Patient city has been updated from {patient.City} to {City}";
                }
                if (patient.State != State)
                {
                    updatenotes += $" Patient state has been updated from {patient.State} to {State}";
                }
                if (patient.Zip != Zip)
                {
                    updatenotes += $" Patient zip has been updated from {patient.Zip} to {Zip}";
                }
                if (patient.MrNumber != Mrn)
                {
                    updatenotes += $" Patient MR# has been updated from {patient.MrNumber} to {Mrn}";
                }
                if (drs.CaseNumber != Csn)
                {
                    updatenotes += $" Patient CSN has been updated from {drs.CaseNumber} to {Csn}";
                }
                patient.FirstName = FirstName;
                patient.LastName = LastName;
                patient.DateOfBirth = DOB;
                patient.Age = Age;
                patient.Weight = Weight;
                patient.Phone = Phone;
                patient.Social = Social;
                patient.Address = Address;
                patient.City = City;
                patient.State = State;
                patient.Zip = Zip;
                patient.MrNumber = Mrn;

                drs.MrNumber = Mrn;
                drs.CaseNumber = Csn;
                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = drs.RequestID,
                        MRNUMBER = drs.MrNumber,
                        FIRSTNAME = patient.FirstName,
                        LASTNAME = patient.LastName,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };

                    context.tblAuditLogs.Add(auditLog);


                }
                context.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update the caller details
        /// </summary>
        /// <param name="callerId"></param>
        /// <param name="dischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult UpdateCallerInfo(int callerId, int dischargeRequestId)
        {
            tblDischargeRequest1 drs = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                string updatenotes = "Updates: ";
                var newCaller = context.tblCallers.Find(callerId);
                if (drs.CallerID != null && drs.CallerID != 0)
                {
                    var oldCaller = context.tblCallers.Find(drs.CallerID);
                    

                    if (drs.CallerID != callerId)
                    {
                        updatenotes += $"The caller has been updated from {oldCaller.CallerFirstName} {oldCaller.CallerLastName} to {newCaller.CallerFirstName} {newCaller.CallerLastName}";
                    }
                }
                else
                {
                    updatenotes += $"The caller has been updated from Unknown to {newCaller.CallerFirstName} {newCaller.CallerLastName}";
                }


                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = drs.RequestID,
                        MRNUMBER = drs.MrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };
                    context.tblAuditLogs.Add(auditLog);
                }
                drs.CallerID = callerId;

                context.SaveChanges();
            }
            return Json(drs.RequestID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update the destination information
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Zip"></param>
        /// <param name="DestinationName"></param>
        /// <param name="DestinationType"></param>
        /// <param name="DestinationInstructions"></param>
        /// <param name="Miles"></param>
        /// <param name="Phone"></param>
        /// <param name="TravelTime"></param>
        /// <param name="dischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult UpdateDestinationInfo(string Address, string City, String State, String Zip, string DestinationName, string DestinationType, string DestinationInstructions, string Miles, string Phone, string TravelTime, int dischargeRequestId)
        {
            tblDischargeRequest1 drs = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                //get the dischargeInfo
                drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                //get the destinationInfo
                var des = context.tblDischargeDestinations.Find(drs.DestinationID);
                string destinationName = DestinationName.Split(',')[0].Trim();
                string updatenotes = "Updates: ";

                if (des.DestinationType != DestinationType)
                {
                    updatenotes += $"The destination type has been updated from {des.DestinationType} to {DestinationType}";
                }
                if (des.DestInstructions != DestinationInstructions)
                {
                    updatenotes += $"The destination instructions have been updated from {des.DestInstructions} to {DestinationInstructions}";
                }
                if (des.AddressLineOne != Address)
                {
                    updatenotes += $"The destination address has been updated from {des.AddressLineOne} to {Address}";
                }
                if (des.City != City)
                {
                    updatenotes += $"The destination city has been updated from {des.City} to {City}";
                }
                if (des.StateCode != State)
                {
                    updatenotes += $"The destination state has been updated from {des.StateCode} to {State}";
                }
                if (des.Zip != Zip)
                {
                    updatenotes += $"The destination zip has been updated from {des.Zip} to {Zip}";
                }
                if (des.Phone != Phone)
                {
                    updatenotes += $"The destination Phone has been updated from {des.Phone} to {Phone}";
                }
                if (des.TravelTime != Convert.ToDateTime(TravelTime).TimeOfDay)
                {
                    updatenotes += $"The destination type has been updated from {des.TravelTime} to {Convert.ToDateTime(TravelTime).TimeOfDay}";
                }

                int miles = 0;
                if(!string.IsNullOrEmpty(Miles))
                {
                    if (Miles.Contains("."))
                    {
                        miles = Convert.ToInt32(Miles.Substring(0, Miles.LastIndexOf(".")));
                    }
                    else
                    {
                        miles = Convert.ToInt32(Miles);
                    }
                }

                if (des.Miles != miles)
                {
                    updatenotes += $"The destination miles has been updated from {des.Miles} to {Miles}";
                }
                des.DestinationType = DestinationType;
                des.AddressLineOne = Address;
                des.City = City;
                des.StateCode = State;
                des.Zip = Zip;
                des.DestinationName = destinationName;
                des.DestInstructions = DestinationInstructions;
                des.Miles = miles;
                des.Phone = Phone;
                des.TravelTime = Convert.ToDateTime(TravelTime).TimeOfDay;
                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = drs.RequestID,
                        MRNUMBER = drs.MrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };
                    context.tblAuditLogs.Add(auditLog);
                }
                context.SaveChanges();
            }
            return Json(drs.RequestID, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Update the agency/ transport details
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="Agency"></param>
        /// <param name="DischargeTime"></param>
        /// <param name="RequestStatus"></param>
        /// <param name="dischargeRequestId"></param>
        /// <param name="specialInstructions"></param>
        /// <param name="lifeSupport"></param>
        /// <param name="specialNeeds"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public JsonResult UpdateTransportInformation(string Mode, string Agency, DateTime? DischargeTime, string RequestStatus, DateTime? AgencyArrival, int dischargeRequestId, string specialInstructions, string lifeSupport, string[] specialNeeds, string notes)
        {
            tblDischargeRequest1 drs = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                int agencyid = 0;
                bool isValidAgency = (Agency != "-Choose Agency-") && !string.IsNullOrEmpty(Agency);
                if (isValidAgency)
                {
                    agencyid = context.tblEmsAgencyLocals.Where(x => x.Name == Agency).Select(i => i.EMSID).First();
                }

                int statusid = context.tblRequestStatus.Where(x => x.RequestStatus == RequestStatus).Select(i => i.ReqStatusID).First();
                drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                var splNeeds = context.tblDischargeNeeds.Where(x => x.patientid == drs.PatientID);
                string updatenotes = "Updates: ";
                //get every entry form DB
                var fromdb = splNeeds.Where(x => x.active == true).Select(x => x.specialneedsid).ToList();
                //get every entry from UI
                List<int> fromui = new List<int>();
                //make sure the specialneeds is not null
                if (specialNeeds != null && specialNeeds.Count() > 0)
                {
                    fromui = Array.ConvertAll(specialNeeds, int.Parse).ToList();
                }

                //get the different items to inactivate in DB
                var diff = fromdb.Except(fromui).Concat(fromui.Except(fromdb));
                foreach (var item in diff)
                {
                    string needname = string.Empty;
                    //check if the need exists for the patient in db
                    var tobeupdated = splNeeds.Where(x => x.specialneedsid == item).FirstOrDefault();
                    if (tobeupdated != null)
                    {
                        needname = context.tblSpecialNeeds.Find(tobeupdated.specialneedsid).SpecialNeed;
                        if (tobeupdated.active == false)
                        {
                            updatenotes += $" Specialneed {needname} has been activated ";
                            tobeupdated.active = true;
                        }
                        else if (tobeupdated.active == true)
                        {
                            updatenotes += $" Specialneed {needname} has been deactivated ";
                            //if exists udpate it to inactive
                            tobeupdated.active = false;
                        }

                    }
                    else
                    {
                        needname = context.tblSpecialNeeds.Find(item).SpecialNeed;
                        //if not exists insert a new entry
                        tblDischargeNeed tdischargeneed = new tblDischargeNeed()
                        {
                            patientid = drs.PatientID,
                            specialneedsid = item,
                            active = true
                        };
                        context.tblDischargeNeeds.Add(tdischargeneed);
                        updatenotes += $" specialneed {needname} has been added ";
                    }
                }

                if (drs.ModeOfTransport != Mode)
                {
                    updatenotes += $"The mode of transport has been updated from {drs.ModeOfTransport} to {Mode}";
                }
                if (drs.EmsAgencyId != agencyid)
                {
                    string agencyname = string.Empty;
                    if (drs.EmsAgencyId.HasValue)
                    {
                        agencyname = context.tblEmsAgencyLocals.Find(drs.EmsAgencyId).Name;
                        updatenotes += $" The agency has been updated from {agencyname} to {Agency}";
                        drs.EmsAgencyId = agencyid;
                    }
                    else
                    {
                        if (isValidAgency)
                        {
                            updatenotes += $"Agency {Agency} has been selected";
                            drs.EmsAgencyId = agencyid;
                        }
                    }

                }
                if (drs.DischargeTime != DischargeTime)
                {
                    updatenotes += $" The discharge time has been updated from {drs.DischargeTime} to {DischargeTime}";
                }
                if (drs.EMSAgencyArrived != AgencyArrival)
                {
                    updatenotes += $" The discharge time has been updated from {drs.EMSAgencyArrived} to {AgencyArrival}";
                }
                if (drs.LifeSupport != lifeSupport)
                {
                    updatenotes += $" The life support has been updated from {drs.LifeSupport} to {lifeSupport}";
                }
                if (drs.SpecialInstructions != specialInstructions)
                {
                    if (string.IsNullOrEmpty(specialInstructions))
                    {
                        updatenotes += $" SpecialInstructions have been added: {specialInstructions}";
                    }
                    else
                    {
                        updatenotes += $" SpecialInstructions have been changed from {drs.SpecialInstructions} to {specialInstructions}";
                    }
                }
                if (drs.Notes != notes)
                {
                    if (string.IsNullOrEmpty(notes))
                    {
                        updatenotes += $" Notes have been added: {notes}";
                    }
                    else
                    {
                        updatenotes += $" Notes have been changed from {drs.Notes} to {notes}";
                    }
                }
                if (drs.RequestStatusID != statusid)
                {
                    var oldRequest = context.tblRequestStatus.Find(drs.RequestStatusID).RequestStatus;
                    updatenotes += $" The request status has been updated from {oldRequest} to {RequestStatus}";
                }
                drs.ModeOfTransport = Mode;

                drs.DischargeTime = DischargeTime;
                drs.EMSAgencyArrived = AgencyArrival;
                drs.LifeSupport = lifeSupport;
                drs.SpecialInstructions = specialInstructions;
                drs.RequestStatusID = statusid;
                drs.Notes = notes;
                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = drs.RequestID,
                        MRNUMBER = drs.MrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };
                    context.tblAuditLogs.Add(auditLog);

                }
                context.SaveChanges();
            }
            return Json(drs.RequestID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update the insurance details
        /// </summary>
        /// <param name="MbrInsuranceId"></param>
        /// <param name="PayorName"></param>
        /// <param name="InsuranceId"></param>
        /// <param name="PlanName"></param>
        /// <param name="PlanType"></param>
        /// <param name="DischargeRequestId"></param>
        /// <param name="AuthCode"></param>
        /// <returns></returns>
        public JsonResult UpdateInsuranceInfo(int MbrInsuranceId, string PayorName, long InsuranceId, string PlanName, string PlanType, int DischargeRequestId, string AuthCode, string ICDCode)
        {
            tblDischargeRequest1 drs = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                drs = context.tblDischargeRequests1.Find(DischargeRequestId);
                var ins = context.tblMbrInsurances.Find(drs.InsuranceID);
               string updatenotes = "Updates: ";
                if (ins.InsuranceID == 999)
                {
                    if (ins.AuthCode != AuthCode)
                    {
                        updatenotes += $" The AuthCode has been updated from {ins.AuthCode} to {AuthCode}";
                    }
                    ins.AuthCode = AuthCode;
                }
                else
                {
                    if (ins.AuthCode != AuthCode)
                    {
                        updatenotes += $" The AuthCode has been updated from {ins.AuthCode} to {AuthCode}";
                    }
                    if (ins.PayorName != PayorName)
                    {
                        updatenotes += $" The payor name has been updated from {ins.PayorName} to {PayorName}";
                    }
                    if (ins.InsuranceID != InsuranceId)
                    {
                        updatenotes += $" The insuranceID has been updated from {ins.InsuranceID} to {InsuranceId}";
                    }
                    if (ins.PlanName != PlanName)
                    {
                        updatenotes += $" The plan name has been updated from {ins.PlanName} to {PlanName}";
                    }
                    if (ins.PlanType != PlanType)
                    {
                        updatenotes += $" The plan type has been updated from {ins.PlanType} to {PlanType}";
                    }
                    if (ins.ICDCode != ICDCode)
                    {
                        updatenotes += $" The plan type has been updated from {ins.ICDCode} to {ICDCode}";
                    }
                    ins.AuthCode = AuthCode;
                    ins.PayorName = PayorName;
                    ins.InsuranceID = InsuranceId;
                    ins.PlanName = PlanName;
                    ins.PlanType = PlanType;
                    ins.ICDCode = ICDCode;
                }

                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = drs.RequestID,
                        MRNUMBER = drs.MrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };
                    context.tblAuditLogs.Add(auditLog);
                }

                context.SaveChanges();
            }
            return Json(drs.RequestID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get the EMS agencies for the mode of transport
        /// </summary>
        /// <param name="transMode"></param>
        /// <param name="LocalUse"></param>
        /// <returns></returns>
        public JsonResult GetEmsAgenciesByTransMode(string transMode, bool LocalUse)
        {
            List<SelectListItem> emsAgencies = GetAgencies(transMode, LocalUse);

            return Json(emsAgencies.OrderBy(x => x.Value), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the pavillion codes and units
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLocations()
        {
            List<SelectListItem> pavillionCodes = new List<SelectListItem>();

            using (var context = new lifeflightapps())
            {
                var pav = context.tblDischargeRequestUnits.Select(x => x.ReqBuilding).Distinct().ToList();
                foreach (var p in pav)
                {
                    pavillionCodes.Add(new SelectListItem { Text = p, Value = p});
                }
                pavillionCodes.Add(new SelectListItem { Text = "-Choose Building-", Value = "-Choose Building-", Selected = true });
                
            }
            return Json(pavillionCodes, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pavillion"></param>
        /// <returns></returns>
        public JsonResult GetUnitsByPavillion(string Pavillion)
        {
            List<SelectListItem> unitList = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                var units = context.tblDischargeRequestUnits.Where(x => x.ReqBuilding == Pavillion).Select(x => x.ReqUnit).Distinct().ToList();

                foreach (var u in units)
                {
                    unitList.Add(new SelectListItem { Text = u, Value = u });
                }
            }

            return Json(unitList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <param name="Pavillion"></param>
        /// <param name="Unit"></param>
        /// <param name="Bed"></param>
        /// <returns></returns>
        public JsonResult UpdateLocation(int DischargeRequestId, string Pavillion, string Unit, string Bed)
        {
            using (var context = new lifeflightapps())
            {
                var req = context.tblDischargeRequests1.Find(DischargeRequestId);
                var cen = context.tblMbrCensus.Find(req.LocationID);

                cen.PavillionCode = Pavillion;
                cen.Unit = Unit;
                cen.Bed = Bed;

                context.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update the pick up details
        /// </summary>
        /// <param name="PickupLocationName"></param>
        /// <param name="PickupInstructions"></param>
        /// <param name="PickupAddress"></param>
        /// <param name="PickupCity"></param>
        /// <param name="PickupState"></param>
        /// <param name="PickupZip"></param>
        /// <param name="dischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult UpdatePickupInformation(string PickupLocationName, string PickupInstructions, string PickupAddress, string PickupCity, string PickupState, string PickupZip, int dischargeRequestId)
        {
            tblDischargeRequest1 drs = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                //get the dischargeinfo
                drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                //get pickupInfo
                var pic = context.tblDischargePickups.Find(drs.PickupID);
                string pickuplocationname = PickupLocationName.Split(',')[0].Trim();
                string updatenotes = "Updates: ";
                if (pic.PickupLocationName != pickuplocationname)
                {
                    updatenotes += $" The pickup locatio name has been updated from {pic.PickupLocationName} to {pickuplocationname}";
                }
                if (pic.PickupInstructions != PickupInstructions)
                {
                    updatenotes += $" The pickup instructions have been updated from {pic.PickupInstructions} to {PickupInstructions}";
                }
                if (pic.AddressLineOne != PickupAddress)
                {
                    updatenotes += $" The pickup address has been updated from {pic.AddressLineOne} to {PickupAddress}";
                }
                if (pic.City != PickupCity)
                {
                    updatenotes += $" The pickup city has been  updated from {pic.City} to {PickupCity}";
                }
                if (pic.StateCode != PickupState)
                {
                    updatenotes += $" The pickup state has been updated from {pic.StateCode} to {PickupState}";
                }
                if (pic.Zip != PickupZip)
                {
                    updatenotes += $" The pickup zip has been updated from {pic.Zip} to {PickupZip}";
                }
                pic.PickupLocationName = PickupLocationName.Split(',')[0].Trim();
                pic.PickupInstructions = PickupInstructions;
                pic.AddressLineOne = PickupAddress;
                pic.City = PickupCity;
                pic.StateCode = PickupState;
                pic.Zip = PickupZip;
                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = drs.RequestID,
                        MRNUMBER = drs.MrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };
                    context.tblAuditLogs.Add(auditLog);
                }
                context.SaveChanges();
            }
            return Json(drs.RequestID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save the ATNumber to the DB
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <param name="Payor"></param>
        /// <param name="Reason"></param>
        /// <param name="ATNumber"></param>
        /// <param name="Cost"></param>
        /// <returns></returns>
        public JsonResult SaveATNumber(int dischargeRequestId, string Payor, string Reason, string ATNumber, string Cost)
        {
            tblDischargeATNumber aTNumber = new tblDischargeATNumber()
            {
                ATNumber = ATNumber,
                PayingReason = Reason,
                RequestID = dischargeRequestId,
                CreatedBy = User.Identity.Name,
                IsActive = true,
                Cost = string.IsNullOrEmpty(Cost) ? 0 : Convert.ToInt32(Cost),
                CreatedDate = DateTime.Now,
                Payor = Payor
            };
            string updatenotes = $"AT# has been generated with {Payor}, {Reason}, {ATNumber}, {Cost}";
            using (var context = new lifeflightapps())
            {
                var drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = dischargeRequestId,
                    MRNUMBER = drs.MrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = updatenotes
                };


                context.tblDischargeATNumbers.Add(aTNumber);
                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save the ATNumber, Invoice and PO to the DB
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <param name="Payor"></param>
        /// <param name="Reason"></param>
        /// <param name="ATNumber"></param>
        /// <param name="Cost"></param>
        /// <param name="Invoice"></param>
        /// <param name="PO"></param>
        /// <returns></returns>
        public JsonResult SaveNumbers(int DischargeRequestId, string Payor, string Reason, string ATNumber, decimal? Cost, string Invoice, string PO)
        {
            tblDischargeATNumber tblDischargeATNumber = null;
            string InvoiceCreatedBy = string.Empty;
            DateTime? InvoiceCreatedOn = null;
            string PoCreatedBy = string.Empty;
            DateTime? PoPaidOn = null;

            if (!string.IsNullOrEmpty(Invoice))
            {
                InvoiceCreatedBy = User.Identity.Name;
                InvoiceCreatedOn = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(PO))
            {
                PoCreatedBy = User.Identity.Name;
                PoPaidOn = DateTime.Now;
            }

            using (var context = new lifeflightapps())
            {
                tblDischargeATNumber = context.tblDischargeATNumbers.Where(a => a.RequestID == DischargeRequestId && a.IsActive == true).FirstOrDefault();

                if (tblDischargeATNumber != null)
                {
                    tblDischargeATNumber.PayingReason = Reason;
                    tblDischargeATNumber.Payor = Payor;
                    tblDischargeATNumber.ATNumber = ATNumber;
                    tblDischargeATNumber.Cost = Cost.HasValue? Cost.Value:0;//string.IsNullOrEmpty(Cost) ? 0 : Convert.ToInt32(Cost);
                    tblDischargeATNumber.InvoiceNumber = Invoice;
                    tblDischargeATNumber.InvoiceCreatedBy = InvoiceCreatedBy;
                    tblDischargeATNumber.InvoiceCreatedOn = InvoiceCreatedOn;
                    tblDischargeATNumber.PONumber = PO;
                    tblDischargeATNumber.POCreatedBy = PoCreatedBy;
                    tblDischargeATNumber.POPaidOn = PoPaidOn; 
                }
                else
                {
                    tblDischargeATNumber aTNumber = new tblDischargeATNumber()
                    {
                        ATNumber = ATNumber,
                        PayingReason = Reason,
                        RequestID = DischargeRequestId,
                        CreatedBy = User.Identity.Name,
                        IsActive = true,
                        Cost = Cost.HasValue ? Cost.Value : 0,//string.IsNullOrEmpty(Cost) ? 0 : Convert.ToInt32(Cost),
                        CreatedDate = DateTime.Now,
                        Payor = Payor,
                        InvoiceNumber = Invoice,
                        InvoiceCreatedBy = InvoiceCreatedBy,
                        InvoiceCreatedOn = InvoiceCreatedOn,
                        PONumber = PO,
                        POCreatedBy = PoCreatedBy,
                        POPaidOn = PoPaidOn
                    };

                    context.tblDischargeATNumbers.Add(aTNumber);
                }
                context.SaveChanges();
            }
                //dischargeRequestId, Payor, Reason, ATNumber, Cost, Invoice, PO 
                return Json("success", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Makes the entry to tblDischargeAtNumbers as inactive so that they don't show up on the page reload.
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult ClearNumbers(int DischargeRequestId)
        {
            using (var context = new lifeflightapps())
            {
                var atn = context.tblDischargeATNumbers.Where(a => a.RequestID == DischargeRequestId && a.IsActive == true).FirstOrDefault();
                atn.IsActive = false;

                context.SaveChanges();
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// soft delete the AT Number in the DB
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult ClearATNumber(int dischargeRequestId)
        {
            string updatenotes = $"AT# has been cleared for RequestID {dischargeRequestId}";
            using (var context = new lifeflightapps())
            {
                var drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = dischargeRequestId,
                    MRNUMBER = drs.MrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = updatenotes
                };

                var atn = context.tblDischargeATNumbers.Where(a => a.RequestID == dischargeRequestId);
                foreach (var at in atn)
                {
                    at.IsActive = false;
                }
                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save the Invoice (or) PO Number to the DB
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <param name="Name"></param>
        /// <param name="Number"></param>
        /// <returns></returns>
        public JsonResult SaveInvoiceOrPO(int dischargeRequestId, string Name, string Number)
        {
            using (var context = new lifeflightapps())
            {
                //Check if an entry exists in atnumber table
                var atn = context.tblDischargeATNumbers.Where(a => a.RequestID == dischargeRequestId && a.IsActive == true).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                var drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = dischargeRequestId,
                    MRNUMBER = drs.MrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = string.Empty
                };
                //If exists update 
                if (atn != null)
                {
                    if (Name == "Invoice")
                    {
                        auditLog.UPDATENOTES = $"Invoice has been updated with invoice# {Number}";
                        atn.InvoiceCreatedBy = User.Identity.Name;
                        atn.InvoiceCreatedOn = DateTime.Now;
                        atn.InvoiceNumber = Number;
                        context.tblAuditLogs.Add(auditLog);
                        context.SaveChanges();
                    }
                    else if (Name == "PO")
                    {
                        auditLog.UPDATENOTES = $"PO has been updated with PO# {Number}";
                        atn.POCreatedBy = User.Identity.Name;
                        atn.PONumber = Number;
                        atn.POPaidOn = DateTime.Now;
                        context.tblAuditLogs.Add(auditLog);
                        context.SaveChanges();
                    }
                }
                //else insert
                else
                {
                    if (Name == "Invoice")
                    {
                        tblDischargeATNumber aTNumber = new tblDischargeATNumber()
                        {
                            InvoiceCreatedBy = User.Identity.Name,
                            InvoiceCreatedOn = DateTime.Now,
                            InvoiceNumber = Number
                        };
                        auditLog.UPDATENOTES = $"Invoice has been created with invoice# {Number}";
                        context.tblDischargeATNumbers.Add(aTNumber);
                        context.tblAuditLogs.Add(auditLog);
                        context.SaveChanges();
                    }
                    else if (Name == "PO")
                    {
                        tblDischargeATNumber aTNumber = new tblDischargeATNumber()
                        {
                            POCreatedBy = User.Identity.Name,
                            PONumber = Number,
                            POPaidOn = DateTime.Now
                        };
                        auditLog.UPDATENOTES = $"PO has been created with PO# {Number}";
                        context.tblDischargeATNumbers.Add(aTNumber);
                        context.tblAuditLogs.Add(auditLog);
                        context.SaveChanges();
                    }
                }
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Soft delete Invoice (or) PO Number
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public JsonResult ClearInvoiceOrPO(int dischargeRequestId, string Name)
        {
            using (var context = new lifeflightapps())
            {
                var atn = context.tblDischargeATNumbers.Where(a => a.RequestID == dischargeRequestId);
                var drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = dischargeRequestId,
                    MRNUMBER = drs.MrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = string.Empty
                };
                foreach (var at in atn)
                {
                    if (Name == "Invoice")
                    {
                        at.InvoiceCreatedBy = "";
                        at.InvoiceNumber = "";
                        at.InvoiceCreatedOn = null;
                        auditLog.UPDATENOTES = "Invoice has been cleared";
                    }
                    else if (Name == "PO")
                    {
                        at.POCreatedBy = "";
                        at.PONumber = "";
                        at.POPaidOn = null;
                        auditLog.UPDATENOTES = "PO has been cleared";
                    }
                }
                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the destination address 
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult GetAddresses(int DischargeRequestId)
        {
            List<string> FromAndTo = new List<string>();
            using (var context = new lifeflightapps())
            {
                var dr = context.tblDischargeRequests1.Find(DischargeRequestId);
                var des = context.tblDischargeDestinations.Find(dr.DestinationID);
                if (dr.RequestType == "Discharge")
                {
                    FromAndTo.Add("1211 Medical Center Drive, Nashville, TN 37232");
                }
                else
                {
                    var pickup = context.tblDischargePickups.Find(dr.PickupID);
                    FromAndTo.Add($"{pickup.AddressLineOne}, {pickup.City}, {pickup.StateCode}, {pickup.Zip}");
                }
                FromAndTo.Add($"{des.AddressLineOne},{des.City},{des.StateCode}, {des.Zip}");
            }
            return Json(FromAndTo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Calculate the Medicare Allowable charges
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult CalculateMedicareCharges(int DischargeRequestId)
        {
            List<decimal> responses = new List<decimal>();
            decimal response = 0;
            int? miles = 0;
            using (var context = new lifeflightapps())
            {
                var drs = context.tblDischargeRequests1.Find(DischargeRequestId);
                miles = context.tblDischargeDestinations.Find(drs.DestinationID).Miles;
                var medicare = context.tblMedicareCodes;
                if (miles > 0)
                {
                    decimal? perMile = medicare.Where(x => x.HCPCSCode == "A0425").FirstOrDefault().MedicareAllowable;
                    perMile = miles * perMile;
                    decimal? perMileFW = medicare.Where(x => x.HCPCSCode == "A0435").FirstOrDefault().MedicareAllowable;
                    perMileFW = miles * perMileFW;

                    decimal? als = medicare.Where(x => x.HCPCSCode == "A0426").FirstOrDefault().MedicareAllowable;
                    als = perMile + als;
                    decimal? bls = medicare.Where(x => x.HCPCSCode == "A0428").FirstOrDefault().MedicareAllowable;
                    bls = perMile + bls;
                    decimal? fwt = medicare.Where(x => x.HCPCSCode == "A0430").FirstOrDefault().MedicareAllowable;
                    fwt = perMileFW + fwt;
                    decimal? cct = medicare.Where(x => x.HCPCSCode == "A0434").FirstOrDefault().MedicareAllowable;
                    cct = perMile + cct;

                    responses.Add(als.Value);
                    responses.Add(bls.Value);
                    responses.Add(cct.Value);
                    responses.Add(fwt.Value);
                }
                else
                {
                    response = 0;
                    responses.Add(response);
                }
            }
            return Json(responses, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Calculate the Cab charges
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult CalculateCabCharges(int DischargeRequestId)
        {
            //List<TransportCost> costs = new List<TransportCost>();
            List<decimal> responses = new List<decimal>();
            decimal response = 0;
            int? miles = 0;
            using (var context = new lifeflightapps())
            {
                var drs = context.tblDischargeRequests1.Find(DischargeRequestId);
                miles = context.tblDischargeDestinations.Find(drs.DestinationID).Miles;
                //[BaseRate]+([GroundMiles]*[MileageRate])
                var cabs = context.tblEmsAgencyLocals.Where(x => x.Type == "Taxi").OrderByDescending(x => x.Name).ToList();
                var medicare = context.tblMedicareCodes;
                if (miles > 0)
                {
                    foreach (var cab in cabs)
                    {

                        decimal? baseRate = cab.BaseRate;
                        decimal? mileageRate = cab.MileageRate;
                        var total = baseRate + (miles * mileageRate);
                        responses.Add(total.Value);
                        //TransportCost cost = new TransportCost()
                        //{
                        //    TransportType = cab.Name,
                        //    Cost = Convert.ToInt32(total)
                        //};
                    }
                }
                else
                {
                    response = 0;
                    responses.Add(response);
                }
            }
            return Json(responses, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the ICD Code and the diagnosis information
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonResult GetICDCodes(string term)
        {
            List<string> codes = new List<string>();
            using (var context = new lifeflightapps())
            {
                IQueryable<string> icd = context.tblICDCodes.Where(c => c.Diagnosis.Contains(term) || c.ICDCode.Contains(term)).Select(c => c.ICDCode + " - " + c.Diagnosis).Take(10);
                codes = icd.ToList();
            }
            return Json(codes, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Helper Classes
        //public class TransportCost
        //{
        //    public string TransportType { get; set; }
        //    public int Cost { get; set; }
        //}

        /// <summary>
        /// Helper class for caller information
        /// </summary>
        public class CallerDetails
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string PagerNumber { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            public int CallerId { get; set; }
            public string Assignment { get; set; }
        }

        #endregion

    }
}