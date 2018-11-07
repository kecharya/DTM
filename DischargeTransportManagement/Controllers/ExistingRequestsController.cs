using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using VUMC.EnterpriseServices;
using VUMC.Notification;

namespace DischargeTransportManagement.Controllers
{
    [Authorize]
    public class ExistingRequestsController : Controller
    {
        private DischargeDetailsController dischargeDetailsController = new DischargeDetailsController();

        [Authorize]
        public ActionResult ShowExistingRequests()
        {
            ExistingRequestsModel existingRequestModel = new ExistingRequestsModel();
            //Show all requests that are in a state of Pending or Scheduled or Confirmed.
            //And agency arrived time should be null
            List<tblDischargeRequest1> dischargeRequests = new List<tblDischargeRequest1>();
            using (var context = new lifeflightapps())
            {
                dischargeRequests = (from r in context.tblDischargeRequests1
                                     where
                                     r.EMSAgencyArrived == null
                                     && r.RequestStatusID != 2 && r.RequestStatusID != 4
                                     select r).OrderByDescending(x => x.RequestStatusID == 3).ThenBy(x => x.DischargeTime).ToList();
            }
            PopulateTheModel(existingRequestModel, dischargeRequests);
            return View(existingRequestModel);
        }

        
        /// <summary>
        /// Display all the requests that have been placed.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]

        public ActionResult AllRequests(int? page, string ddlSearchOption, string RequestSearch, DateTime? FromDate, DateTime? ToDate)
        {
            List<AllRequestsModel> allRequests = new List<AllRequestsModel>();

            if ((FromDate.HasValue && ToDate.HasValue) || (!string.IsNullOrEmpty(RequestSearch)))
            {
                using (var context = new lifeflightapps())
                {
                   var discharges = context.tblDischargeRequests1.Where(x => (x.EMSAgencyArrived != null) || (x.RequestStatusID == 2 || x.RequestStatusID == 4)).OrderByDescending(x => x.CallReceivedDate);

                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        discharges = discharges.Where(x => DbFunctions.TruncateTime(x.CallReceivedDate) >= DbFunctions.TruncateTime(FromDate)
                                                                        && DbFunctions.TruncateTime(x.CallReceivedDate) <= DbFunctions.TruncateTime(ToDate)).OrderByDescending(x => x.CallReceivedDate);
                    }

                    if (!string.IsNullOrEmpty(RequestSearch))
                    {
                        discharges = FilterDischarges(ddlSearchOption, RequestSearch, context, discharges);
                    }
                    
                    foreach (var item in discharges)
                    {
                        AllRequestsModel allRequestsModel = new AllRequestsModel();
                        allRequestsModel = GetRequestDetails(item);
                        allRequests.Add(allRequestsModel);
                    }
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(allRequests.ToPagedList(pageNumber, pageSize));

        }

        private static IOrderedQueryable<tblDischargeRequest1> FilterDischarges(string ddlSearchOption, string RequestSearch, lifeflightapps context, IOrderedQueryable<tblDischargeRequest1> discharges)
        {
            if (ddlSearchOption == "Name")
            {
                var pid = context.tblPatients.Where(p => p.FirstName + " " + p.LastName == RequestSearch || p.MrNumber == RequestSearch).Select(p => p.PatientID);
                discharges = discharges.Where(x => pid.Contains(x.PatientID)).OrderByDescending(x => x.CallReceivedDate);
            }
            else if (ddlSearchOption == "MRN")
            {
                int mrNumber = 0;
                var isMrNuumber = int.TryParse(RequestSearch, out mrNumber);
                if (mrNumber != 0)
                {
                    discharges = discharges.Where(x => x.MrNumber == RequestSearch).OrderByDescending(x => x.CallReceivedDate);
                }
            }
            else if (ddlSearchOption == "AT#")
            {
                var reqid = context.tblDischargeATNumbers.Where(x => x.ATNumber == RequestSearch).Select(x => x.RequestID).FirstOrDefault();
                if (reqid != 0)
                {
                    discharges = discharges.Where(x => x.RequestID == reqid).OrderByDescending(x => x.CallReceivedDate);
                }
            }

            return discharges;
        }

        /// <summary>
        /// Gets the request details from the tblDischargerequest table
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private AllRequestsModel GetRequestDetails(tblDischargeRequest1 item)
        {
            AllRequestsModel allRequestsModel = new AllRequestsModel();
            using (var context = new lifeflightapps())
            {
                var patient = context.tblPatients.Find(item.PatientID);
                var agency = context.tblEmsAgencyLocals.Find(item.EmsAgencyId);
                var status = context.tblRequestStatus.Find(item.RequestStatusID);
                var destination = context.tblDischargeDestinations.Find(item.DestinationID);
                var location = context.tblMbrCensus.Find(item.LocationID);
                allRequestsModel = new AllRequestsModel()
                {
                    RequestId = item.RequestID,
                    RequestType = item.RequestType,
                    ScheduledTime = item.DischargeTime.ToString(),
                    PatientName = patient.FirstName + " " + patient.LastName,
                    Agency = (agency != null) ? agency.Name : "",
                    Status = status.RequestStatus,
                    Destination = ((destination.DestinationType == "Address On File") || (destination.DestinationType == "Other")) ? destination.AddressLineOne + destination.City + ", " + destination.StateCode + ", " + destination.Zip : destination.DestinationName,
                    MRN = item.MrNumber,
                    Location = (location.PavillionCode == "N/A" ? location.PavillionCode : location.PavillionCode + ", " + location.Unit)
                };
            }
            return allRequestsModel;
        }

        /// <summary>
        /// Populates the ExistingRequestsModel
        /// </summary>
        /// <param name="existingRequestModel"></param>
        /// <param name="dischargeRequests"></param>
        private void PopulateTheModel(ExistingRequestsModel existingRequestModel, List<tblDischargeRequest1> dischargeRequests)
        {
            try
            {
                using (var context = new lifeflightapps())
                {
                    foreach (var drequest in dischargeRequests)
                    {
                        string CallerName = string.Empty;
                        if (drequest.CallerID != 0)
                        {
                            var caller = context.tblCallers.Find(drequest.CallerID);
                            CallerName = caller.CallerFirstName + " " + caller.CallerLastName;
                        }
                        else
                        {
                            CallerName = "Unknown";
                        }

                        var destination = context.tblDischargeDestinations.Find(drequest.DestinationID);
                        var location = context.tblMbrCensus.Find(drequest.LocationID);
                        var patient = context.tblPatients.Find(drequest.PatientID);
                        var status = context.tblRequestStatus.Find(drequest.RequestStatusID);//from s in db.tblRequestStatus select s;
                        string loc = (location.PavillionCode == "N/A" ? location.PavillionCode : location.Unit + ", " + location.Bed);
                        string agencyName = string.Empty;

                        if (drequest.EmsAgencyId.HasValue && drequest.EmsAgencyId.Value > 0)
                        {
                            agencyName = context.tblEmsAgencyLocals.Find(drequest.EmsAgencyId.Value).Name;
                        };
                        //split the requests based on the request type
                        if (drequest.RequestType == "Discharge")
                        {
                            ExistingRequests dischargeRequest = new ExistingRequests()
                            {
                                WarningMessage = CheckLocationUpdate(drequest.MrNumber, User.Identity.Name, location, drequest.RequestID),
                                DischargeRequestId = drequest.RequestID,
                                MrNumber = drequest.MrNumber,
                                CallerId = drequest.CallerID,
                                CallerName = CallerName,//caller.CallerFirstName + " " + caller.CallerLastName, //caller.Select(c => c.CallerLastName + " " + c.CallerFirstName).FirstOrDefault(),
                                                        //TODO: tblDischargeDestination
                                DestinationId = drequest.DestinationID,
                                DestinationName = ((destination.DestinationType == "Address On File") || (destination.DestinationType == "Other")) ? destination.AddressLineOne + destination.City + ", " + destination.StateCode + ", " + destination.Zip : destination.DestinationName,
                                //destination.Select(d => d.AddressLineOne + "," + d.City + "," + d.StateCode).FirstOrDefault(),

                                EmsAgency = (!string.IsNullOrEmpty(agencyName)) ? agencyName : drequest.ModeOfTransport,
                                PatientId = drequest.PatientID,

                                ScheduledTime = drequest.DischargeTime?.ToString("MM/dd/yyyy HH:mm") ?? "",
                                LocationId = drequest.LocationID,
                                Location = (location.PavillionCode == "N/A" ? location.PavillionCode : location.PavillionCode + ", " + location.Unit + ", " + location.Bed), // Removing the bed as requested by Elizabeth + ", " + p.Bed
                                PatientName = patient.FirstName + " " + patient.LastName + " - " + loc, //patient.Select(p => p.LastName + " " + p.FirstName).FirstOrDefault(),
                                Status = status.RequestStatus,//status.Where(i=> i.ReqStatusID == drequest.RequestStatusID).Select(s=>s.RequestStatus).FirstOrDefault(),
                                StatusColor = SetStatusColor(drequest.RequestStatusID),
                                RequestType = drequest.RequestType,
                                AgencyContacted = drequest.EMSAgencyContacted,
                                AgencyResponded = drequest.EMSAgencyResponded,
                                EmsUnitId = drequest.EmsUnitId
                            };
                            try
                            {
                                existingRequestModel.Discharges.Add(dischargeRequest);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else if (drequest.RequestType != null)
                        {
                            var pickup = (drequest.PickupID.HasValue && drequest.PickupID.Value > 0) ? context.tblDischargePickups.Find(drequest.PickupID) : null;
                            ExistingRequests otherRequests = new ExistingRequests()
                            {
                                DischargeRequestId = drequest.RequestID,
                                MrNumber = drequest.MrNumber,
                                CallerId = drequest.CallerID,
                                CallerName = CallerName,//caller.CallerFirstName + " " + caller.CallerLastName, //caller.Select(c => c.CallerLastName + " " + c.CallerFirstName).FirstOrDefault(),
                                                        //TODO: tblDischargeDestination
                                DestinationId = drequest.DestinationID,
                                DestinationName = destination.DestinationName,
                                //destination.Select(d => d.AddressLineOne + "," + d.City + "," + d.StateCode).FirstOrDefault(),

                                EmsAgency = (!string.IsNullOrEmpty(agencyName)) ? agencyName : drequest.ModeOfTransport,
                                PatientId = drequest.PatientID,
                                PatientName = patient.FirstName + " " + patient.LastName, //patient.Select(p => p.LastName + " " + p.FirstName).FirstOrDefault(),
                                ScheduledTime = drequest.DischargeTime?.ToString("MM/dd/yyyy HH:mm") ?? "",
                                LocationId = drequest.LocationID,
                                Location = (location.PavillionCode == "N/A" ? location.PavillionCode : location.PavillionCode + ", " + location.Unit), // Removing the bed as requested by Elizabeth + ", " + p.Bed
                                Status = status.RequestStatus,//status.Where(i=> i.ReqStatusID == drequest.RequestStatusID).Select(s=>s.RequestStatus).FirstOrDefault(),
                                StatusColor = SetStatusColor(drequest.RequestStatusID),
                                PickupId = drequest.PickupID ?? 0,
                                PickupName = pickup.PickupLocationName + ", " + pickup.City + ", " + pickup.StateCode, //Need to check if the ALT Fundings can have a request from home.
                                RequestType = drequest.RequestType,
                                AgencyContacted = drequest.EMSAgencyContacted,
                                AgencyResponded = drequest.EMSAgencyResponded
                            };

                            existingRequestModel.OtherRequests.Add(otherRequests);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         * 1. MrNumber as input parameter
         * 2. Call the gen services /census and get the location information
         * 3. Check if the location is same in tblMbrCensus
         * 4. If same do nothing
         * 5. Else make an entry and capture the existing censusID into the PreviousCensusID
         * 6. Return a string that has the new location and previous location
         */

        /// <summary>
        ///
        /// </summary>
        /// <param name="mrNumber"></param>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private string CheckLocationUpdate(string mrNumber, string VunetId, tblMbrCensu location, int dischargerequestId)
        {
            string response = string.Empty;
            RetrieveCensusInformation retrieveCensusInformation = new RetrieveCensusInformation();
            List<CensusModel> censusModels = new List<CensusModel>();
            censusModels = retrieveCensusInformation.GetCensusRecords(mrNumber, VunetId);
            response = InsertOrUpdateCensus(censusModels, mrNumber, location, dischargerequestId);
            return response;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="censusModels"></param>
        /// <param name="mrNumber"></param>
        /// <param name="location"></param>
        private string InsertOrUpdateCensus(List<CensusModel> censusModels, string mrNumber, tblMbrCensu location, int dischargerequestId)
        {
            CensusModel census = censusModels.FirstOrDefault();
            string censusUpdate = string.Empty;
            if (census != null)
            {
                if (!(census.Unit == location.Unit && census.PavillionCode == location.PavillionCode && census.Bed == location.Bed))
                {
                    census.PreviousCencusID = location.MbrCensusID;
                    tblMbrCensu tcensus = new tblMbrCensu()
                    {
                        Bed = census.Bed,
                        MrNumber = mrNumber,
                        PavillionCode = census.PavillionCode,
                        Unit = census.Unit,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = dischargerequestId,
                        MRNUMBER = mrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = $"Auto update of the patient location from {location.Unit},{location.PavillionCode},{location.Bed} to {census.Unit},{census.PavillionCode},{census.Bed}"
                    };

                    using (var context = new lifeflightapps())
                    {
                        context.tblMbrCensus.Add(tcensus);
                        context.tblAuditLogs.Add(auditLog);

                        int censusId = tcensus.MbrCensusID;
                        var drs = context.tblDischargeRequests1.Find(dischargerequestId);
                        drs.LocationID = censusId;
                        context.SaveChanges();
                    }
                    censusUpdate = $"The census information has changed from {location.Unit} bed {location.Bed} to new unit {census.Unit}, bed {census.Bed}";
                }
            }
            return censusUpdate;
        }

        //public ActionResult PrintReport(int DischargeRequestId)
        //{
        //    var report = new ActionAsPdf("GeneratePrintableReport", new { DischargeRequestId = DischargeRequestId });
        //    return report;
        //}

        public ActionResult GeneratePrintableReport(int DischargeRequestId)
        {
            PrintableDetails printdetails = GePrintDetails(DischargeRequestId);
            return View(printdetails);
        }

        /// <summary>
        /// Generates the PDF
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        //public void GeneratePdf(int DischargeRequestId)
        //{
        //    // Create a Document object
        //    var document = new Document(PageSize.A4, 50, 50, 25, 25);

        //    // Create a new PdfWrite object, writing the output to a MemoryStream
        //    var output = new MemoryStream();
        //    var writer = PdfWriter.GetInstance(document, output);

        //    // First, create our fonts... (For more on working w/fonts in iTextSharp, see: http://www.mikesdotnetting.com/Article/81/iTextSharp-Working-with-Fonts
        //    var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
        //    var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
        //    var boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
        //    var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
        //    var bodyFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
        //    document.Header = new HeaderFooter(new Phrase("Vanderbilt Discharge Transportation Management"), false);

        //    var firstPageFont = FontFactory.GetFont("Courier", 16, Font.BOLD);

        //    // Parameters passed on to the function that creates the PDF
        //    string headerText = "Vanderbilt Discharge Transportation Management";
        //    string footerText = "Page";

        //    // Define a font and font-size in points (plus f for float) and pick a color
        //    // This one is for both header and footer but you can also create seperate ones
        //    //Font fontHeaderFooter = FontFactory.GetFont("CenturySchoolbook", 20f);
        //    //fontHeaderFooter.Color = Color.GRAY;
        //    BaseFont BaseFontHeaderFooter = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
        //    Font fontHeaderFooter = new Font(BaseFontHeaderFooter, 20, Font.BOLDITALIC, Color.GRAY);

        //    // Apply the font to the headerText and create a Phrase with the result
        //    Chunk chkHeader = new Chunk(headerText, fontHeaderFooter);
        //    Phrase p1 = new Phrase(chkHeader);
        //    // create a HeaderFooter element for the header using the Phrase
        //    // The boolean turns numbering on or off
        //    HeaderFooter header = new HeaderFooter(p1, false)
        //    {
        //        // Remove the border that is set by default
        //        Border = Rectangle.NO_BORDER,
        //        // Align the text: 0 is left, 1 center and 2 right.
        //        Alignment = 1
        //    };

        //    // add the header to the document
        //    document.Header = header;

        //    // If you want to use numbering like in this example, add a whitespace to the
        //    // text because by default there's no space in between them
        //    if (footerText.Substring(footerText.Length - 1) != " ") footerText += " ";

        //    Chunk chkFooter = new Chunk(footerText, fontHeaderFooter);
        //    Phrase p2 = new Phrase(chkFooter);

        //    // Turn on numbering by setting the boolean to true
        //    HeaderFooter footer = new HeaderFooter(p2, true)
        //    {
        //        Border = Rectangle.NO_BORDER,
        //        Alignment = 1
        //    };

        //    document.Footer = footer;

        //    BaseFont bfTimes = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);

        //    Font times = new Font(bfTimes, 12, Font.BOLDITALIC, Color.RED);

        //    document.Open();
        //    //document.Add(new Paragraph("Vanderbilt Discharge Transportation Management", titleFont));
        //    //document.Add(new Phrase("\n"));
        //    //document.Add(new Paragraph(""));
        //    //document.Add(Chunk.NEWLINE);
        //    //document.Add(Chunk.NEWLINE);
        //    //document.Add(Chunk.NEWLINE);
        //    //document.Add(Chunk.NEWLINE);
        //    Paragraph para1 = new Paragraph("This fax contains a Discharge Transportation Request from Vanderbilt Medical Center.", firstPageFont);
        //    para1.SetAlignment("Justify");

        //    Paragraph para2 = new Paragraph("Please call 322 - 7433 to confirm receipt of this request and to confirm that you will be able to provide transport of this patient at the requested time.", firstPageFont);
        //    Paragraph para3 = new Paragraph("If you feel you have received this fax in error please discard and call 615 - 322 - 7433.", times);
        //    para1.SpacingBefore = 50f;
        //    para2.SpacingBefore = 100f;
        //    para3.SpacingBefore = 100f;
        //    document.Add(new Phrase("\n"));
        //    document.Add(para1);
        //    document.Add(para2);
        //    document.Add(para3);

        //    //document.Add(Chunk.NEWLINE);
        //    //document.Add(Chunk.NEWLINE);
        //    //document.Add(new Paragraph("Please call 322-7433 to confirm receipt of this request and to confirm that you will be able to provide transport of this patient at the requested time.", firstPageFont));
        //    //document.Add(Chunk.NEWLINE);
        //    //document.Add(Chunk.NEWLINE);
        //    //document.Add(new Paragraph("If you feel you have received this fax in error please discard and call 615 - 322 - 7433.", times));
        //    //document.Add(Chunk.NEWLINE);

        //    // Open the Document for writing
        //    document.NewPage();

        //    PrintableDetails printdetails = new PrintableDetails();
        //    printdetails = GePrintDetails(DischargeRequestId);

        //    var printableTable = new PdfPTable(2)
        //    {
        //        HorizontalAlignment = 0,
        //        SpacingBefore = 10,
        //        SpacingAfter = 10
        //    };
        //    printableTable.DefaultCell.Border = 1;
        //    printableTable.SetWidths(new int[] { 2, 4 });
        //    printableTable.SpacingAfter = 2;
        //    printableTable.WidthPercentage = 100;
        //    PdfPCell cell = new PdfPCell(new Phrase(string.Format("Transport Details for {0}", printdetails.PatientName.ToUpper())))
        //    {
        //        Colspan = 3,

        //        HorizontalAlignment = 1 //0=Left, 1=Centre, 2=Right
        //    };

        //    printableTable.AddCell(cell);

        //    printableTable.AddCell(new PdfPCell(new Phrase("Bedside Date/Time:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.BedsideDateTime)));

        //    printableTable.AddCell(new PdfPCell(new Phrase("Care Level:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.CareLevel)));

        //    printableTable.AddCell(new PdfPCell(new Phrase("Special Needs:", boldTableFont)));
        //    //Create a checkbox for each special need
        //    /*
        //     * Oxygen
        //        IVMeds
        //        CardiacMonitor
        //        Pacer
        //        Trach
        //        Ventilator
        //     */
        //     var rect = new Rectangle(180, 806, 200, 788);
        //    var checkbox = new RadioCheckField(writer, rect, "fieldVO.getNewName()", "on");

        //    if (printdetails.SpecialNeeds.Count() > 0)
        //    {
        //        string needs = string.Empty;
        //        for (var i = 0; i < printdetails.SpecialNeeds.Count(); i++)
        //        {
        //             needs += printdetails.SpecialNeeds[i].Selected ? printdetails.SpecialNeeds[i].Text + Environment.NewLine : "";

        //        }

        //        printableTable.AddCell(new PdfPCell(new Phrase(needs)));
        //    }
        //    else
        //    {
        //        printableTable.AddCell(new PdfPCell(new Phrase("")));
        //    }
        //    printableTable.AddCell(new PdfPCell(new Phrase("Patient Name:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PatientName)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Date Of Birth:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.DOB)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Social:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.SSN)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Weight:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.weight)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Patient Notes:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PatientNotes)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Patient Location:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PatientLocation)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Destination:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.Destination)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Destination Address:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.DestinationAddress)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Primary Payor:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PrimaryPayor)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Insurance Provider:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.InsuranceProvider)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Insurance ID:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.InsuranceId)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Pre-Auth Number:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PreAuthNumber)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("AT Number:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.AtNumber)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Cost:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.Cost)));
        //    printableTable.AddCell(new PdfPCell(new Phrase("Drive Mileage:", boldTableFont)));
        //    printableTable.AddCell(new PdfPCell(new Phrase(printdetails.DriveMileage)));

        //    document.Add(new Phrase("\n"));
        //    document.Add(new Phrase("\n"));

        //    document.Add(printableTable);
        //    // Add ending message
        //    //var endingMessage = new Paragraph("Thank you for your business! If you have any questions about your order, please contact us at 800-555-NORTH.", endingMessageFont);
        //    //endingMessage.SetAlignment("Center");
        //    //document.Add(endingMessage);
        //    document.NewPage();

        //    document.Close();

        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("Content-Disposition", string.Format("inline;filename=DischargeRequest-{0}.pdf", DischargeRequestId));
        //    Response.BinaryWrite(output.ToArray());
        //}

        public void GeneratePdf(int DischargeRequestId)
        {
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
            document.Header = new HeaderFooter(new Phrase("Vanderbilt Discharge Transportation Management"), false);

            var firstPageFont = FontFactory.GetFont("Arial", 16, Font.BOLD);

            // Parameters passed on to the function that creates the PDF
            string headerText = "Vanderbilt Discharge Transportation Management";
            string footerText = "Page";

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
            HeaderFooter footer = new HeaderFooter(p2, true)
            {
                Border = Rectangle.NO_BORDER,
                Alignment = 1
            };

            document.Footer = footer;

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
            Font times = new Font(bfTimes, 12, Font.BOLDITALIC, Color.RED);

            document.Open();
            Paragraph para1 = new Paragraph("This fax contains a Discharge Transportation Request from Vanderbilt Medical Center.", firstPageFont);
            para1.SetAlignment("Justify");

            Paragraph para2 = new Paragraph("Please call 322-7433 to confirm receipt of this request and to confirm that you will be able to provide transport of this patient at the requested time.", firstPageFont);
            Paragraph para3 = new Paragraph("If you feel you have received this fax in error please discard and call 615-322-7433.", receivedInError);
            para1.SpacingBefore = 50f;
            para2.SpacingBefore = 100f;
            para3.SpacingBefore = 100f;
            document.Add(new Phrase("\n"));
            document.Add(para1);
            document.Add(para2);
            document.Add(para3);

            // Open the Document for writing
            document.NewPage();

            PrintableDetails printdetails = new PrintableDetails();
            printdetails = GePrintDetails(DischargeRequestId);

            //Paragraph titlePara = new Paragraph(string.Format("Transportion details for: {0}", printdetails.PatientName.ToUpper()));
            //titlePara.Alignment = 1;
            //document.Add(titlePara);
            //document.Add(new Phrase("\n"));
            //Paragraph careLevelLblPara = new Paragraph(new Chunk("Bedside Date/Time:"));
            //Paragraph carLevelTextPara = new Paragraph(new Chunk(printdetails.BedsideDateTime));
            //document.Add(careLevelLblPara);
            //document.Add(carLevelTextPara);

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
            PdfPCell cell = new PdfPCell(new Phrase(string.Format("Transport Details for {0}", printdetails.PatientName.ToUpper())))
            {
                Colspan = 3,
                HorizontalAlignment = 1 //0=Left, 1=Centre, 2=Right
            };

            printableTable.AddCell(cell);

            printableTable.AddCell(new PdfPCell(new Phrase("Bedside Date/Time:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.BedsideDateTime)));

            printableTable.AddCell(new PdfPCell(new Phrase("Care Level:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.CareLevel)));

            printableTable.AddCell(new PdfPCell(new Phrase("Special Needs:", boldTableFont)));
            //Create a checkbox for each special need
            /*
             * Oxygen
                IVMeds
                CardiacMonitor
                Pacer
                Trach
                Ventilator
             */
            var rect = new Rectangle(180, 806, 200, 788);
            var checkbox = new RadioCheckField(writer, rect, "fieldVO.getNewName()", "on");

            if (printdetails.SpecialNeeds.Count() > 0)
            {
                string needs = string.Empty;
                for (var i = 0; i < printdetails.SpecialNeeds.Count(); i++)
                {
                    needs += printdetails.SpecialNeeds[i].Selected ? printdetails.SpecialNeeds[i].Text + Environment.NewLine : "";
                }

                printableTable.AddCell(new PdfPCell(new Phrase(needs)));
            }
            else
            {
                printableTable.AddCell(new PdfPCell(new Phrase("")));
            }
            printableTable.AddCell(new PdfPCell(new Phrase("Patient Name:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PatientName)));
            printableTable.AddCell(new PdfPCell(new Phrase("Date Of Birth:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.DOB)));
            printableTable.AddCell(new PdfPCell(new Phrase("Social:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.SSN)));
            printableTable.AddCell(new PdfPCell(new Phrase("Weight:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.Weight)));
            printableTable.AddCell(new PdfPCell(new Phrase("Patient Notes:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PatientNotes)));
            printableTable.AddCell(new PdfPCell(new Phrase("Patient Location:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PatientLocation)));
            printableTable.AddCell(new PdfPCell(new Phrase("Destination:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.Destination)));
            printableTable.AddCell(new PdfPCell(new Phrase("Destination Address:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.DestinationAddress)));
            printableTable.AddCell(new PdfPCell(new Phrase("Primary Payor:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PrimaryPayor)));
            printableTable.AddCell(new PdfPCell(new Phrase("Insurance Provider:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.InsuranceProvider)));
            printableTable.AddCell(new PdfPCell(new Phrase("Insurance ID:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.InsuranceId)));
            printableTable.AddCell(new PdfPCell(new Phrase("Pre-Auth Number:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.PreAuthNumber)));
            printableTable.AddCell(new PdfPCell(new Phrase("AT Number:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.AtNumber)));
            printableTable.AddCell(new PdfPCell(new Phrase("Cost:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.Cost)));
            printableTable.AddCell(new PdfPCell(new Phrase("Drive Mileage:", boldTableFont)));
            printableTable.AddCell(new PdfPCell(new Phrase(printdetails.DriveMileage)));

            document.Add(new Phrase("\n"));
            document.Add(new Phrase("\n"));

            document.Add(printableTable);
            // Add ending message
            //var endingMessage = new Paragraph("Thank you for your business! If you have any questions about your order, please contact us at 800-555-NORTH.", endingMessageFont);
            //endingMessage.SetAlignment("Center");
            //document.Add(endingMessage);
            document.NewPage();

            document.Close();

            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", string.Format("inline;filename=DischargeRequest-{0}.pdf", DischargeRequestId));
            Response.BinaryWrite(output.ToArray());
        }

        /// <summary>
        /// Gets the details that are needed to be displayed on the PDF taking dischargerequestID as input parameter
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <returns></returns>
        private PrintableDetails GePrintDetails(int dischargeRequestId)
        {
            PrintableDetails printdetails = new PrintableDetails();
            using (var context = new lifeflightapps())
            {
                var dr = context.tblDischargeRequests1.Find(dischargeRequestId);
                var patient = context.tblPatients.Find(dr.PatientID);
                var location = context.tblMbrCensus.Find(dr.LocationID);
                var destination = context.tblDischargeDestinations.Find(dr.DestinationID);
                var insurance = context.tblMbrInsurances.Find(dr.InsuranceID);
                var atn = context.tblDischargeATNumbers.Where(x => x.RequestID == dischargeRequestId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                printdetails.BedsideDateTime = dr.DischargeTime.ToString();
                printdetails.CareLevel = dr.LifeSupport;
                printdetails.SpecialNeeds = GetNeeds(dr.PatientID);
                printdetails.PatientName = patient.FirstName + " " + patient.LastName;
                printdetails.DOB = patient.DateOfBirth.ToString();
                printdetails.SSN = patient.Social;
                printdetails.Weight = patient.Weight.ToString();
                printdetails.PatientNotes = dr.SpecialInstructions;
                printdetails.PatientLocation = location.PavillionCode + " || " + location.Unit + " || " + location.Bed;
                printdetails.Destination = destination.DestinationType;
                printdetails.DestinationAddress = destination.AddressLineOne + " " + destination.City + " " + destination.StateCode + " " + destination.Zip;
                printdetails.DestinationRoom = string.Empty;
                printdetails.PrimaryPayor = insurance.PayorName;
                printdetails.InsuranceProvider = insurance.PlanName;
                printdetails.InsuranceId = insurance.InsuranceID.ToString();
                printdetails.PreAuthNumber = string.Empty;

                if (destination.DestinationType == "Hospital/NursingHome" && dr.RequestType == "Discharge")
                {
                    var facility = context.Union_Facilities.Where(x => x.NAME == destination.DestinationName).FirstOrDefault();
                    if (facility != null)
                    {
                        var mileage = context.tblDrivingInfoes.Where(x => x.FacilityID == facility.id2).FirstOrDefault();
                        if (mileage != null)
                        {
                            printdetails.DriveMileage = mileage.DriveMileage.ToString();
                        }
                    }
                }

                if (atn != null)
                {
                    printdetails.AtNumber = atn.ATNumber;
                    printdetails.Cost = atn.Cost.ToString();
                }

                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = dischargeRequestId,
                    MRNUMBER = dr.MrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = $"PDF / Printable report of the request was generated."
                };
                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }
            return printdetails;
        }

        /// <summary>
        /// Gets the patient needs taking patientID as input parameter
        /// </summary>
        /// <param name="patientID"></param>
        /// <returns></returns>
        private List<SelectListItem> GetNeeds(int patientID)
        {
            List<SpecialNeeds> needs = new List<SpecialNeeds>();
            List<SelectListItem> snss = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                var splNeed = context.tblSpecialNeeds.Select(i => new { i.SpecialNeed, i.SpecialNeedID }).ToList();

                var dischargeNeeds = context.tblDischargeNeeds.Where(i => i.patientid == patientID && i.active == true).ToList();
                foreach (var item in dischargeNeeds)
                {
                    var sn = splNeed.Where(i => i.SpecialNeedID == item.specialneedsid).First();

                    SpecialNeeds need = new SpecialNeeds()
                    {
                        SpecialNeed = sn.SpecialNeed,
                        SpecialNeedId = sn.SpecialNeedID
                    };
                    needs.Add(need);
                    SelectListItem sns = new SelectListItem()
                    {
                        Selected = true,
                        Text = sn.SpecialNeed,
                        Value = sn.SpecialNeedID.ToString()
                    };
                    snss.Add(sns);
                }
            }
            return snss;
        }

        /// <summary>
        /// Sets the color based on the request status
        /// </summary>
        /// <param name="requestStatusID"></param>
        /// <returns></returns>
        private string SetStatusColor(int requestStatusID)
        {
            string statusColor = string.Empty;
            if (requestStatusID == 3)
            {
                statusColor = "Goldenrod";
            }
            else
            {
                statusColor = "cornflowerblue";
            }
            return statusColor;
        }

        /// <summary>
        /// Gets the Ems agencies for the discharge request
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        private List<SelectListItem> GetEmsAgenciesForDischarge(int DischargeRequestId)
        {
            List<SelectListItem> agencies = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                var discharge = context.tblDischargeRequests1.Find(DischargeRequestId);
                var emsAgency = discharge.EmsAgencyId.HasValue ? context.tblEmsAgencyLocals.Find(discharge.EmsAgencyId.Value).Name : "N/A";

                var eagencies = context.tblEmsAgencyLocals.Where(x => x.LocalUse == true && x.Type == discharge.ModeOfTransport).ToList();
                foreach (var item in eagencies)
                {
                    SelectListItem eagency = new SelectListItem()
                    {
                        Value = item.Name,
                        Text = item.Name
                    };
                    agencies.Add(eagency);
                }
                if ((!string.IsNullOrEmpty(emsAgency)) && (emsAgency != "N/A"))
                {
                    foreach (var item in agencies)
                    {
                        if (item.Value == emsAgency)
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
            return agencies;
        }

        #region Re routes

        public ActionResult ShowRequestDetails(string mrN, int patId, int destId, int locId, int callId)
        {
            //Display the caller, patient, census, destination and insurance details. Also the special requests.
            return RedirectToAction("DischargeDetails", "DischargeDetails", new { MrNumber = mrN, PatientId = patId, DestinationId = destId, LocationId = locId, CallerId = callId });
            //return View();
        }

        public ActionResult RequestDetails(int id)
        {
            //Display the caller, patient, census, destination and insurance details. Also the special requests.
            return RedirectToAction("DischargeInfo", "DischargeDetails", new { DischargeRequestId = id });
            //return View();
        }

        public ActionResult NewDischarge()
        {
            return RedirectToAction("RequestView", "Request");
        }

        #endregion Re routes

        #region Ajax Calls

        /// <summary>
        /// Updates the entry from the Modal to the DB.
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <param name="scheduledTime"></param>
        /// <param name="status"></param>
        /// <param name="Mode"></param>
        /// <param name="Agency"></param>
        /// <param name="AgencyContacted"></param>
        /// <param name="AgencyResponded"></param>
        /// <returns></returns>
        public JsonResult UpdateTimeAndStatus(int dischargeRequestId, DateTime? scheduledTime, string status, string Mode, string Agency, DateTime? AgencyContacted, DateTime? AgencyResponded, DateTime? AgencyArrived, string SpecialInstructions, string Notes, string Unit)
        {
            using (var context = new lifeflightapps())
            {
                var drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                int statusid = context.tblRequestStatus.Where(x => x.RequestStatus == status).Select(i => i.ReqStatusID).First();

                int? agencyid = null;
                if ((!string.IsNullOrEmpty(Agency)) && (Agency != "-Choose Agency-"))
                {
                    agencyid = context.tblEmsAgencyLocals.Where(x => x.Name == Agency).Select(i => i.EMSID).First();
                }

                //Check for performance
                string updatenotes = "Updates: ";
                if (drs.ModeOfTransport != Mode)
                {
                    updatenotes += $"Mode of transport has been updated from {drs.ModeOfTransport} to {Mode} ";
                }
                if (drs.EmsAgencyId != agencyid)
                {
                    string agencyname = string.Empty;
                    if (drs.EmsAgencyId != null)
                    {
                        agencyname = context.tblEmsAgencyLocals.Find(drs.EmsAgencyId).Name;
                        updatenotes += $" Ems agency has been changed from {agencyname} to {Agency} ";
                    }
                    else
                    {
                        updatenotes += $" Ems agency {Agency} has been selected ";
                    }
                }
                if (drs.RequestStatusID != statusid)
                {
                    var requestStatus = context.tblRequestStatus.Find(drs.RequestStatusID).RequestStatus;
                    updatenotes += $" RequestStatus has been changed from {requestStatus} to {status} ";
                }
                if (drs.DischargeTime != scheduledTime)
                {
                    if (drs.DischargeTime.HasValue)
                    {
                        updatenotes += $" DischargeTime has been set to {scheduledTime} ";
                    }
                    else
                    {
                        updatenotes += $" DischargeTime has been changed from {drs.DischargeTime} to {scheduledTime} ";
                    }
                }
                if (drs.EMSAgencyContacted != AgencyContacted)
                {
                    if (drs.EMSAgencyContacted.HasValue)
                    {
                        var contact = AgencyContacted.HasValue ? AgencyContacted.Value.ToString() : "Null";
                        updatenotes += $" AgencyContacted time has been changed from {drs.EMSAgencyContacted} to {contact} ";
                    }
                    else
                    {
                        updatenotes += $" AgencyContacted time has been set to {AgencyContacted} ";
                    }
                }
                if (drs.EMSAgencyResponded != AgencyResponded)
                {
                    if (drs.EMSAgencyResponded.HasValue)
                    {
                        var respond = AgencyResponded.HasValue ? AgencyResponded.Value.ToString() : "Null";
                        updatenotes += $" AgencyResponded has been channged from {drs.EMSAgencyResponded} to {respond} ";
                    }
                    else
                    {
                        updatenotes += $" AgencyResponded time has been set to {AgencyResponded} ";
                    }
                }
                if (drs.EMSAgencyArrived != AgencyArrived)
                {
                    updatenotes += $" AgencyArrived has been changed from {drs.EMSAgencyArrived} to {AgencyArrived} ";
                }
                if (drs.SpecialInstructions != SpecialInstructions)
                {
                    if (string.IsNullOrEmpty(SpecialInstructions))
                    {
                        updatenotes += $" SpecialInstructions have been added: {SpecialInstructions}";
                    }
                    else
                    {
                        updatenotes += $" SpecialInstructions have been changed from {drs.SpecialInstructions} to {SpecialInstructions}";
                    }
                }
                if (drs.Notes != Notes)
                {
                    if (string.IsNullOrEmpty(Notes))
                    {
                        updatenotes += $" Notes have been added: {Notes}";
                    }
                    else
                    {
                        updatenotes += $" Notes have been changed from {drs.Notes} to {Notes}";
                    }
                }

                int id = context.tblEmsAgencyLocals.Where(x => x.Name == "Vanderbilt EMS").Select(i => i.EMSID).First();
                if (agencyid == id)
                {
                    if (Unit != "Choose One")
                    {
                        drs.EmsUnitId = Convert.ToInt32(Unit);
                    }
                }


                drs.ModeOfTransport = Mode;
                drs.EmsAgencyId = agencyid;
                drs.RequestStatusID = statusid;
                drs.DischargeTime = scheduledTime;
                drs.EMSAgencyContacted = AgencyContacted;
                drs.EMSAgencyResponded = AgencyResponded;
                drs.EMSAgencyArrived = AgencyArrived;
                drs.SpecialInstructions = SpecialInstructions;
                drs.Notes = Notes;
                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = dischargeRequestId,
                        MRNUMBER = drs.MrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };
                    context.tblAuditLogs.Add(auditLog);
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the entry from the modal and does the next action based on the Action Parameter
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <param name="scheduledTime"></param>
        /// <param name="status"></param>
        /// <param name="Mode"></param>
        /// <param name="Agency"></param>
        /// <param name="AgencyContacted"></param>
        /// <param name="AgencyResponded"></param>
        /// <param name="AgencyArrived"></param>
        /// <param name="Action"></param>
        /// <param name="ActionParms"></param>
        /// <returns></returns>
        public JsonResult UpdateAndDoNext(int dischargeRequestId, DateTime? scheduledTime, string status, string Mode, string Agency, DateTime? AgencyContacted, DateTime? AgencyResponded, DateTime? AgencyArrived, string Action, string ActionParms, string SpecialInstructions, string Notes, string Unit)
        {
            using (var context = new lifeflightapps())
            {
                var drs = context.tblDischargeRequests1.Find(dischargeRequestId);
                int statusid = context.tblRequestStatus.Where(x => x.RequestStatus == status).Select(i => i.ReqStatusID).First();

                int? agencyid = null;
                if ((!string.IsNullOrEmpty(Agency)) && (Agency != "-Choose Agency-"))
                {
                    agencyid = context.tblEmsAgencyLocals.Where(x => x.Name == Agency).Select(i => i.EMSID).First();
                }

                //Check for performance
                string updatenotes = "Updates: ";
                if (drs.ModeOfTransport != Mode)
                {
                    updatenotes += $"Mode of transport has been updated from {drs.ModeOfTransport} to {Mode} ";
                }
                if (drs.EmsAgencyId != agencyid)
                {
                    var agencyname = drs.EmsAgencyId.HasValue ? context.tblEmsAgencyLocals.Find(drs.EmsAgencyId).Name : "N/A";
                    updatenotes += $" Ems agency has been changed from {agencyname} to {Agency} ";
                }
                if (drs.RequestStatusID != statusid)
                {
                    var requestStatus = context.tblRequestStatus.Find(drs.RequestStatusID).RequestStatus;
                    updatenotes += $" RequestStatus has been changed from {requestStatus} to {status} ";
                }
                if (drs.DischargeTime != scheduledTime)
                {
                    updatenotes += $" DischargeTime has been changed from {drs.DischargeTime} to {scheduledTime} ";
                }
                if (drs.EMSAgencyContacted != AgencyContacted)
                {
                    updatenotes += $" AgencyContacted time has been changed from {drs.EMSAgencyContacted.HasValue} to {AgencyContacted} ";
                }
                if (drs.EMSAgencyResponded != AgencyResponded)
                {
                    updatenotes += $" AgencyResponded has been channged from {drs.EMSAgencyResponded} to {AgencyResponded} ";
                }
                if (drs.EMSAgencyArrived != AgencyArrived)
                {
                    updatenotes += $" AgencyArrived has been channged from {drs.EMSAgencyArrived} to {AgencyArrived} ";
                }
                if (drs.SpecialInstructions != SpecialInstructions)
                {
                    if (string.IsNullOrEmpty(SpecialInstructions))
                    {
                        updatenotes += $" SpecialInstructions have been added: {SpecialInstructions}";
                    }
                    else
                    {
                        updatenotes += $" SpecialInstructions have been changed from {drs.SpecialInstructions} to {SpecialInstructions}";
                    }
                }
                if (drs.Notes != Notes)
                {
                    if (string.IsNullOrEmpty(Notes))
                    {
                        updatenotes += $" Notes have been added: {Notes}";
                    }
                    else
                    {
                        updatenotes += $" Notes have been changed from {drs.Notes} to {Notes}";
                    }
                }
                int id = context.tblEmsAgencyLocals.Where(x => x.Name == "Vanderbilt EMS").Select(i => i.EMSID).First();
                if (agencyid == id)
                {
                    if (Unit != "Choose One" && !string.IsNullOrEmpty(Unit))
                    {
                        drs.EmsUnitId = Convert.ToInt32(Unit);
                    }
                }
                drs.ModeOfTransport = Mode;
                drs.EmsAgencyId = agencyid;
                drs.RequestStatusID = statusid;
                drs.DischargeTime = scheduledTime;
                drs.EMSAgencyContacted = AgencyContacted;
                drs.EMSAgencyResponded = AgencyResponded;
                drs.EMSAgencyArrived = AgencyArrived;
                drs.SpecialInstructions = SpecialInstructions;
                drs.Notes = Notes;
                if (updatenotes != "Updates: ")
                {
                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = dischargeRequestId,
                        MRNUMBER = drs.MrNumber,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = updatenotes
                    };

                    context.tblAuditLogs.Add(auditLog);
                }
                context.SaveChanges();
            }
            switch (Action)
            {
                case "Print":
                    //GeneratePdf(dischargeRequestId);
                    return Json("Print", JsonRequestBehavior.AllowGet);

                case "Preview":
                    var Resp = PreviewMessage(dischargeRequestId, ActionParms, Agency);
                    return Json(Resp.Data, JsonRequestBehavior.AllowGet);

                default:
                    break;
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the destinations as per the state and request type
        /// </summary>
        /// <param name="term"></param>
        /// <param name="Filter"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public JsonResult GetFilteredDischarges(string term)
        {
            List<string> patientNames = new List<string>();
            using (var context = new lifeflightapps())
            {
                var pnames = from p in context.tblPatients
                             join r in context.tblDischargeRequests1
                             on p.PatientID equals r.PatientID
                             where
                             (p.FirstName.StartsWith(term) || p.LastName.StartsWith(term))
                             && (r.EMSAgencyArrived != null
                             || (r.RequestStatusID == 2 || r.RequestStatusID == 4))
                             select p.FirstName + " " + p.LastName;
                patientNames = pnames.Distinct().ToList();
            }
            return Json(patientNames, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieves the TransportMode and Agency
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult GetModeAndAgency(int DischargeRequestId)
        {
            Trans transDetails = new Trans();
            tblEmsAgencyLocal tblEms;
            string agency = "N/A";
            bool LocalUse = true;
            DischargeDetailsController dischargeDetailsController = new DischargeDetailsController();
            tblDischargeRequest1 discharge = new tblDischargeRequest1();
            using (var context = new lifeflightapps())
            {
                discharge = context.tblDischargeRequests1.Find(DischargeRequestId);
                transDetails.Mode = discharge.ModeOfTransport;
                transDetails.TransportModes = dischargeDetailsController.GetTransportModes(discharge.ModeOfTransport);
                if (discharge.EmsAgencyId.HasValue)
                {
                    tblEms = context.tblEmsAgencyLocals.Find(discharge.EmsAgencyId.Value);
                    agency = tblEms.Name;
                    transDetails.IsLocal = tblEms.LocalUse;
                    LocalUse = tblEms.LocalUse;
                }

                transDetails.Agencies = dischargeDetailsController.GetEmsAgencies(discharge.ModeOfTransport, agency, LocalUse);
                transDetails.Agency = transDetails.Agencies.Where(s => s.Selected).Select(s => s.Value).FirstOrDefault();
                transDetails.AgencyContacted = discharge.EMSAgencyContacted?.ToString("MM/dd/yyyy HH:mm") ?? "";
                transDetails.AgencyResponded = discharge.EMSAgencyResponded?.ToString("MM/dd/yyyy HH:mm") ?? "";
                transDetails.AgencyArrived = discharge.EMSAgencyArrived?.ToString("MM/dd/yyyy HH:mm") ?? "";
                transDetails.IsLocal = LocalUse;
                transDetails.CareLevel = discharge.LifeSupport;
                transDetails.SpecialInstructions = discharge.SpecialInstructions;
                transDetails.Notes = discharge.Notes;
                transDetails.SpecialNeeds = dischargeDetailsController.GetNeeds(discharge.PatientID);
                transDetails.EmsUnitId = discharge.EmsUnitId;
                var msg = context.tblPageLogs.Where(x => x.DischargeRequestId == DischargeRequestId).OrderByDescending(x => x.SentOn).FirstOrDefault();
                if (msg != null)
                {
                    transDetails.MessageDetails = $"Message {msg.MessageType} has been sent";
                }
            }
            return Json(transDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FreeTextMessage(int DischargeRequestId)
        {
            string freeText = string.Empty;
            using (var context = new lifeflightapps())
            {
                var discharge = context.tblDischargeRequests1.Find(DischargeRequestId);
                var patient = context.tblPatients.Find(discharge.PatientID);
                var caller = context.tblCallers.Find(discharge.CallerID);
                var location = context.tblMbrCensus.Find(discharge.LocationID);
                var destination = context.tblDischargeDestinations.Find(discharge.DestinationID);
                var specialNeeds = context.tblDischargeNeeds.Where(x => x.patientid == patient.PatientID && x.active == true);
                freeText += $"Pickup Time: {(discharge.DischargeTime.HasValue ? discharge.DischargeTime.Value.ToString() : string.Empty)},";
                freeText += $"Appointment Time: ";
                if (location.PavillionCode != "N/A")
                {
                    freeText += $", Location: {location.PavillionCode} - {location.Unit} - {location.Bed} ";
                }
                if (destination.DestinationType == "Hospital/NursingHome")
                {
                    freeText += $", Destination: {destination.DestinationName} ";
                }

                freeText += $" ,Pt: {patient.LastName} ";
                if (patient.Weight.HasValue && patient.Weight.Value > 0)
                {
                    freeText += $" { patient.Weight} { patient.WeightUnit}";
                }
                if (discharge.LifeSupport != "N/A")
                {
                    freeText += $" Level: {discharge.LifeSupport}";
                }
                if (specialNeeds.Count() > 0)
                {
                    freeText += " SpecialNeeds: ";
                    foreach (var item in specialNeeds)
                    {
                        string need = context.tblSpecialNeeds.Find(item.specialneedsid).SpecialNeed;
                        freeText += $" {need} ";
                    }
                }
            }
            return Json(freeText, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieves the message and the receipents based on the message type
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        public JsonResult PreviewMessage(int DischargeRequestId, string MessageType, string Agency)
        {
            List<string> Preview = new List<string>();
            Preview = ComposeMessage(DischargeRequestId, MessageType, Agency);

            return Json(Preview, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Composes the message based on the Type
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        private List<string> ComposeMessage(int DischargeRequestId, string MessageType, string Agency)
        {
            List<string> MessageAndPagerNumbers = new List<string>();
            string Message = string.Empty;
            string Receipents = string.Empty;
            List<string> recipients = new List<string>();
            using (var context = new lifeflightapps())
            {
                var discharge = context.tblDischargeRequests1.Find(DischargeRequestId);
                var patient = context.tblPatients.Find(discharge.PatientID);
                var caller = context.tblCallers.Find(discharge.CallerID);
                var location = context.tblMbrCensus.Find(discharge.LocationID);
                tblEmsAgencyLocal agency;
                if (discharge.EmsAgencyId.HasValue)
                {
                    agency = context.tblEmsAgencyLocals.Find(discharge.EmsAgencyId);
                }
                else
                {
                    agency = context.tblEmsAgencyLocals.Where(x => x.Name == Agency && x.Type == discharge.ModeOfTransport).FirstOrDefault();
                }
                var destination = context.tblDischargeDestinations.Find(discharge.DestinationID);
                string FloorNurse = context.tblDischargeRequestUnits.Where(c => c.ReqUnit.StartsWith(location.Unit)).Select(c => c.CNPager).FirstOrDefault();

                string MrNumber = discharge.MrNumber;
                string PatientName = patient.FirstName + " " + patient.LastName;
                string CallerName = caller.CallerFirstName + " " + caller.CallerLastName;
                string PickupTime = discharge.DischargeTime?.ToString("MM/dd/yyyy HH:mm") ?? "";
                string AgencyName = agency.Name;
                string AgencyContact = agency.Phones;
                string CalledAt = discharge.CallReceivedDate.Value.ToString("MM/dd/yyyy HH:mm");
                string callerContact = string.Empty;
                if (string.IsNullOrEmpty(caller.CallerPager))
                {
                    callerContact = caller.CallerEmail;
                }
                else
                {
                    callerContact = caller.CallerPager;
                }

                string Goingto = (destination.DestinationType == "Hospital/NursingHome") ? destination.DestinationName : destination.AddressLineOne + " " + destination.City + " " + destination.StateCode;

                if (discharge.RequestType == "Discharge")
                {
                    if (MessageType == "Confirmation")
                    {
                        if (location.Unit == "N/A")
                        {
                            Message = $"At {PickupTime} Patient {PatientName} is scheduled to be picked up by {AgencyName} and is going to {Goingto}";
                        }
                        else
                        {
                            Message = $"At {PickupTime} Patient {PatientName} in {location.Unit} - {location.Bed} is scheduled to be picked up by {AgencyName} and is going to {Goingto}";
                        }
                    }
                    else if (MessageType == "Arrival")
                    {
                        Message = $"EMS Agency {AgencyName} has arrived for pickup of patient {PatientName} from {location.Unit} - {location.Bed}";
                    }
                    if (!string.IsNullOrEmpty(FloorNurse))
                    {
                        char[] delimiters = new char[] { ',', ';' };
                        List<string> multipleNumbers = FloorNurse.Split(delimiters).ToList();

                        foreach (var item in multipleNumbers)
                        {
                            if (item.Contains("@"))
                            {
                                recipients.Add(item.Substring(0, item.LastIndexOf("@")));
                            }
                            else
                            {
                                recipients.Add(item);
                            }
                        }
                    }
                    recipients.Add(callerContact);
                    Receipents = string.Join(",", recipients.ToArray());
                    //Receipents =  !string.IsNullOrEmpty(FloorNurse) ? FloorNurse + ";" + callerContact : callerContact;
                    //Receipents = FloorNurse + "," + caller.MobilePhone.ToString();
                    MessageAndPagerNumbers.Add(Message);
                    MessageAndPagerNumbers.Add(Receipents);
                }
                else if (discharge.RequestType == "ALT Funding")
                {
                    if (MessageType == "Confirmation")
                    {
                        Message = $"{PatientName} is scheduled to be picked up by {AgencyName} at {PickupTime} and be transported to {destination.DestinationName}. This was arranged by {CallerName} on {CalledAt}. Please contact DTM at 2-7433 for any changes or questions.";
                    }
                    else if (MessageType == "Arrival")
                    {
                        Message = $"EMS Agency {AgencyName} has arrived for pickup of patient {PatientName}";
                    }
                    Receipents = callerContact;//caller.CallerPager.ToString();
                    MessageAndPagerNumbers.Add(Message);
                    MessageAndPagerNumbers.Add(Receipents);
                }
            }
            return MessageAndPagerNumbers;
        }

        /// <summary>
        /// Retrieves the caller contact for the specific discharge
        /// </summary>
        /// <param name="DischargeRequestId"></param>
        /// <returns></returns>
        public JsonResult GetCallerContact(int DischargeRequestId)
        {
            string callerDetails = string.Empty;
            using (var context = new lifeflightapps())
            {
                var discharge = context.tblDischargeRequests1.Find(DischargeRequestId);
                var caller = context.tblCallers.Find(discharge.CallerID);
                //Change this to preferred number.
                //callerDetails = "This page will be sent to " + caller.CallerFirstName + " " + caller.CallerLastName + " @ "+ caller.CallerPhone.ToString();
                callerDetails = caller.CallerPager.ToString();
                if (string.IsNullOrEmpty(callerDetails))
                {
                    callerDetails = caller.CallerEmail.ToString();
                }
            }
            return Json(callerDetails, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Send page using the VUMC Notify
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Receipents"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        public JsonResult SendPage(int DischargeRequestId, string Message, string Receipents, string MessageType)
        {
            string PagerResponseID = string.Empty;
            string env = Dns.GetHostName();
            if (env == "ABO118WT-LFTWEB" || env.StartsWith("N"))
            {
                PagerResponseID = SendEmail(Message);
                LogThePage(DischargeRequestId, Message, Receipents, MessageType, PagerResponseID);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                string AppPassKeyPrd = "r#A0hl7Q";
                string ApplicationName = "LifeFlightDTM";
                string VunetId = User.Identity.Name;
                //ComposedMessage = ComposeMessage(DischargeRequestId);

                VUMC.Notification.Email emailService = new VUMC.Notification.Email("srfs.vumc.org");

                //VUMC.EnterpriseServices.NotificationService pagerService = new NotificationService();
                Pager pagerService = new Pager();
                //NotificationService.ServiceUrl = "https://vumcnotify.mc.vanderbilt.edu/notif-webapp/";
                Pager.ServiceUrl = "https://vumcnotify.mc.vanderbilt.edu/notif-webapp/";

                string message = Message;
                string phoneNumbers = Receipents.Replace(" ", string.Empty);
                try
                {
                    Dictionary<string, string> responses = new Dictionary<string, string>();
                    Receipents = Receipents.Replace(" ", string.Empty);
                    List<string> recipients = new List<string>();
                    List<string> pageTo = new List<string>();
                    List<string> emailTo = new List<string>();

                    if (Receipents.Contains(',') || Receipents.Contains(';') || Receipents.Contains('@'))
                    {
                        char[] delimiters = new char[] { ',', ';' };
                        recipients = Receipents.Split(delimiters).ToList();
                        recipients = recipients.Distinct().ToList();
                        foreach (var item in recipients)
                        {
                            if (item.Contains('@'))
                            {
                                emailTo.Add(item);
                            }
                            else
                            {
                                pageTo.Add(item);
                            }
                        }
                        if (emailTo.Count() > 0)
                        {
                            string emailList = string.Join(";", emailTo.ToArray());
                            string emailUrl = "https://ss_services.mc.vanderbilt.edu/HttpEmail/";
                            string emailFrom = "LifeFlight.Coordinators@Vanderbilt.edu";
                            string emailSubject = "Transport Communication";
                            var emailResponse = emailService.SendHTTP(emailUrl, emailFrom, emailList, "", "", "", emailSubject, message, false, new PostFile[0]);
                            PagerResponseID = emailResponse.Message;
                            //emailService.Send("LifeFlight.Coordinators@Vanderbilt.edu", emailList, "Transport Communication", message);
                        }
                        if (pageTo.Count() > 0)
                        {
                            responses = Pager.SendPageSecure(ApplicationName, AppPassKeyPrd, ApplicationName, pageTo, message, 0);
                            foreach (var item in responses)
                            {
                                PagerResponseID += item;
                            }
                        }
                    }
                    else
                    {
                        //since the receipents does not have @ or , or ; it should be just one phone number
                        PagerResponseID = $"[{Receipents}, {Pager.SendPageSecure(ApplicationName, AppPassKeyPrd, ApplicationName, Receipents, message, 0)}]";
                    }
                    LogThePage(DischargeRequestId, Message, Receipents, MessageType, PagerResponseID);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(ex, JsonRequestBehavior.AllowGet);
                }
                /*
                try
                {
                    if (phoneNumbers.Contains(',') || phoneNumbers.Contains(';'))
                    {
                        Dictionary<string, string> responses = new Dictionary<string, string>();
                        List<string> multipleNumbers = new List<string>();
                        //char[] delimiters = new char[] { ',', ';' };
                        multipleNumbers = phoneNumbers.Split(delimiters).ToList();
                        //responses = Pager.SendPageSecure(, multipleNumbers, message, 0);

                        //foreach (var item in multipleNumbers)
                        //{
                        //    Pager.ServiceUrl = "https://vumcnotify.mc.vanderbilt.edu/notif-webapp/";
                        //    string resp = Pager.SendPageSecure(ApplicationName, AppPassKeyPrd, ApplicationName, item, message, 0);
                        //    PagerResponseID += $"[{item}, {resp}]";

                        //}

                        responses = Pager.SendPageSecure(ApplicationName, AppPassKeyPrd, ApplicationName, multipleNumbers, message, 0);
                        foreach (var item in responses)
                        {
                            PagerResponseID += item;
                        }
                    }
                    else
                    {
                        PagerResponseID = $"[{phoneNumbers}, {Pager.SendPageSecure(ApplicationName, AppPassKeyPrd, ApplicationName, phoneNumbers, message, 0)}]";
                    }
                    LogThePage(DischargeRequestId, Message, Receipents, MessageType, PagerResponseID);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(ex, JsonRequestBehavior.AllowGet);
                }
                */
            }
        }

        //public JsonResult GeneratePDF(int DischargeRequestId)
        //{
        //    Report report = new Report(new PdfFormatter());
        //    FontDef fontDef = new FontDef(report, "Calibri ");

        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetEmsAgenciesByTransMode(string transMode, bool LocalUse)
        {
            List<SelectListItem> emsAgencies = new List<SelectListItem>();
            emsAgencies = dischargeDetailsController.GetAgencies(transMode, LocalUse);

            return Json(emsAgencies.OrderBy(x => x.Value), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the history on the request
        /// </summary>
        /// <param name="RequestId"></param>
        /// <returns></returns>
        public JsonResult GetRequestHistory(int RequestId)
        {
            DischargeComplaintController dischargeComplaintController = new DischargeComplaintController();
            List<AuditLog> auditLogs = new List<AuditLog>();
            auditLogs = dischargeComplaintController.GetRequestHistory(RequestId);

            return Json(auditLogs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// retrieves the complaints for a specific discharge request and user.
        /// </summary>
        /// <param name="User"></param>
        /// <param name="RequestId"></param>
        /// <returns></returns>
        public JsonResult GetComplaintsForUser(int RequestId)
        {
            string user = User.Identity.Name;
            UserComplaints userComplaints = new UserComplaints();
            using (var context = new lifeflightapps())
            {
                var complaints = context.tblDischargeComplaints.Where(x => x.RequestID == RequestId && x.EnteredBy == user).OrderByDescending(x => x.ComplaintEntereddatetime).FirstOrDefault();

                if (complaints != null)
                {
                    userComplaints.AocNotified = complaints.HospitalAOCNotified;
                    userComplaints.Complaint = complaints.Complaint;
                    userComplaints.ComplaintType = complaints.ComplaintTypeID ?? 0;
                }
            }
            return Json(userComplaints, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert or update the complaint.
        /// </summary>
        /// <param name="RequestId"></param>
        /// <param name="Complaint"></param>
        /// <param name="ComplaintType"></param>
        /// <param name="IsAocNotified"></param>
        /// <returns></returns>
        public JsonResult InsertOrUpdateComplaint(int RequestId, string Complaint, int ComplaintType, int? DelayReason, bool IsAocNotified)
        {
            string user = User.Identity.Name;
            DateTime now = DateTime.Now;
            string mrn = string.Empty;
            using (var context = new lifeflightapps())
            {
                mrn = context.tblDischargeRequests1.Find(RequestId).MrNumber;
                var dbComplaint = context.tblDischargeComplaints.Where(x => x.RequestID == RequestId && x.EnteredBy == user).OrderByDescending(x => x.ComplaintEntereddatetime).FirstOrDefault();
                if (dbComplaint != null)
                {
                    //Check for performance
                    string updatenotes = "Complaint Change: ";
                    if (dbComplaint.Complaint != Complaint)
                    {
                        updatenotes += $"Complaint has been changed from {dbComplaint.Complaint} to {Complaint} ";
                    }
                    if (dbComplaint.ComplaintTypeID != ComplaintType)
                    {
                        var oldtype = context.tblDischargeComplaintTypes.Where(x => x.ComplaintTypeID == dbComplaint.ComplaintTypeID).Select(x => x.ComplaintType).FirstOrDefault();
                        var newtype = context.tblDischargeComplaintTypes.Where(x => x.ComplaintTypeID == ComplaintType).Select(x => x.ComplaintType).FirstOrDefault();
                        updatenotes += $"ComplaintTypeID has been changed from {oldtype} to {newtype} ";
                    }
                    if (dbComplaint.HospitalAOCNotified != IsAocNotified)
                    {
                        updatenotes += $"HospitalAOCNotified has been changed from {dbComplaint.HospitalAOCNotified} to {IsAocNotified}";
                    }
                    if (dbComplaint.DelayReasonID.HasValue && DelayReason.HasValue)
                    {
                        if (dbComplaint.DelayReasonID != DelayReason)
                        {
                            var oldreason = context.tblDelayReasons.Where(x => x.delayreasonID == dbComplaint.DelayReasonID).Select(x => x.delayreason).FirstOrDefault();
                            var newreason = context.tblDelayReasons.Where(x => x.delayreasonID == DelayReason).Select(x => x.delayreason).FirstOrDefault();
                            updatenotes += $" DelayReason has been changed form {oldreason} to {newreason}";
                        }
                    }
                    if (updatenotes != "Complaint Change: ")
                    {
                        tblAuditLog auditLog = new tblAuditLog()
                        {
                            APPNAME = "DTM",
                            IDFROMAPP = RequestId,
                            MRNUMBER = mrn,
                            CREATEDBY = User.Identity.Name,
                            CREATEDON = DateTime.Now,
                            UPDATENOTES = updatenotes
                        };
                        context.tblAuditLogs.Add(auditLog);
                    }
                    dbComplaint.Complaint = Complaint;
                    dbComplaint.ComplaintTypeID = ComplaintType;
                    dbComplaint.HospitalAOCNotified = IsAocNotified;
                    dbComplaint.DelayReasonID = DelayReason;

                    context.SaveChanges();
                }
                else
                {
                    tblDischargeComplaint tblComplaint = new tblDischargeComplaint()
                    {
                        Complaint = Complaint,
                        RequestID = RequestId,
                        ComplaintTypeID = ComplaintType,
                        DelayReasonID = DelayReason,
                        HospitalAOCNotified = IsAocNotified,
                        ComplaintEntereddatetime = now,
                        EnteredBy = user
                    };

                    tblAuditLog auditLog = new tblAuditLog()
                    {
                        APPNAME = "DTM",
                        IDFROMAPP = RequestId,
                        MRNUMBER = mrn,
                        CREATEDBY = User.Identity.Name,
                        CREATEDON = DateTime.Now,
                        UPDATENOTES = $"Complaint was logged. Compalint: {Complaint}"
                    };
                    context.tblDischargeComplaints.Add(tblComplaint);
                    context.tblAuditLogs.Add(auditLog);
                    context.SaveChanges();
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Logs the information
        /// </summary>
        /// <param name="dischargeRequestId"></param>
        /// <param name="message"></param>
        /// <param name="receipents"></param>
        /// <param name="messageType"></param>
        /// <param name="pagerResponseID"></param>
        public void LogThePage(int dischargeRequestId, string message, string receipents, string messageType, string pagerResponseID)
        {
            tblPageLog pageLog = new tblPageLog()
            {
                DischargeRequestId = dischargeRequestId,
                Message = message,
                MessageType = messageType,
                Receipents = receipents,
                SentOn = DateTime.Now,
                Response = pagerResponseID
            };
            string mrn = string.Empty;
            using (var context = new lifeflightapps())
            {
                mrn = context.tblDischargeRequests1.Find(dischargeRequestId).MrNumber;

                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = dischargeRequestId,
                    MRNUMBER = mrn,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = $"Page was sent. Type:{messageType}, Message:{message}, Receipents:{receipents}"
                };
                context.tblPageLogs.Add(pageLog);
                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Sends out an email
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string SendEmail(string message)
        {
            string user = User.Identity.Name;
            var emailClass = new EmailClass
            {
                Client = new System.Net.Mail.SmtpClient(Email.VUMC_SMTP_SERVER),
                Url = "https://ss_services.mc.vanderbilt.edu/HttpEmail/",
                Subject = "Vanderbilt Transport Management [TEST]",
                To = user + "@intemail.email.vanderbilt.edu",
                Body = message,
                From = "DoNotReply@vanderbilt.edu"
            };

            Email email = new Email(Email.VUMC_SMTP_SERVER);
            ServiceResponse response = new ServiceResponse();
            response = email.SendHTTP(emailClass.Url, emailClass.From, emailClass.To, "", "", "", emailClass.Subject, emailClass.Body, true, emailClass.Files);
            return response.Message;
        }

        #endregion Ajax Calls
    }

    #region Helper Classes

    /// <summary>
    /// Helper class for the Existing Requests
    /// </summary>
    public class ExistingRequestDetails
    {
        public string CallerName { get; set; }
        public int CallerId { get; set; }
        public string PatientName { get; set; }
        public string DestinationName { get; set; }
        public string DestinationType { get; set; }
        public string DestinationAddressLineOne { get; set; }
        public string DestinationState { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationZip { get; set; }
        public string LocationPavillionCode { get; set; }
        public string LocationUnit { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }

    /// <summary>
    /// Helper Class for the complaints
    /// </summary>
    public class UserComplaints
    {
        public int ComplaintType { get; set; }
        public string Complaint { get; set; }
        public bool AocNotified { get; set; }
    }

    /// <summary>
    /// A basic class for returning the details on modal load.
    /// </summary>
    public class Trans
    {
        public List<SelectListItem> TransportModes { get; set; }
        public List<SelectListItem> Agencies { get; set; }
        public string Mode { get; set; }
        public string Agency { get; set; }
        public string AgencyContacted { get; set; }
        public string AgencyResponded { get; set; }
        public string AgencyArrived { get; set; }
        public bool IsLocal { get; set; }
        public string SpecialInstructions { get; set; }
        public string Notes { get; set; }
        public string CareLevel { get; set; }
        public List<SelectListItem> SpecialNeeds { get; set; }
        public string MessageDetails { get; set; }
        public int? EmsUnitId { get; set; }
    }

    /// <summary>
    /// Helper class for the Email
    /// </summary>
    public class EmailClass
    {
        public SmtpClient Client { get; set; }
        public PostFile[] Files { get; set; }
        public string Url { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Cc { get; set; }
        public string Body { get; set; }

        public EmailClass()
        {
            Files = new PostFile[0];
        }
    }

    #endregion Helper Classes
}