using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        // GET: Request
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This returns the new request view where a discharge request is placed
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult RequestView()
        {
            RequestModel model = new RequestModel();
            return View(model);
        }

        /// <summary>
        /// Saves the request to the DB
        /// </summary>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RequestView(RequestModel model, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                string vunetId = User.Identity.Name;
                string genResponse = string.Empty;
                GenServiceModel genServiceModel = new GenServiceModel();
                if (model.RequestType == "VHAN")
                {
                    genServiceModel.PatientDemographics = new Patient();
                    genServiceModel.Census = new CensusModel();
                    //genServiceModel.Coverages = new CoverageInfo();
                    genServiceModel.Insurances = new InsuranceInformation();
                }
                else
                {
                    try
                    {
                        genServiceModel = RetrieveDataFromGenServices(model.MrNumber, vunetId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    if (string.IsNullOrEmpty(genServiceModel.PatientDemographics.MRN))
                    {
                        model.IsPostSuccess = "failure";
                        return View(model);
                    }
                }

                genServiceModel.PatientDemographics.Weight = model.PatientWeight.ToString();
                genServiceModel.PatientDemographics.WeightUnit = model.WeightUnit.ToString();
                int callerId = GetCallerId(model.CallerName);
                string destinationNameToSave = string.IsNullOrEmpty(model.DestinationName) ? string.Empty : model.DestinationName.Split(',').FirstOrDefault();
                if (model.RequestType != "Discharge")
                    model.DestinationType = "Hospital/NursingHome";
                int destinationId = DestinationEntry(model.DestinationType, destinationNameToSave, model.DestinationInstructions, model.AddressLineOne, model.City, model.State, model.Zip, model.Phone, model.TravelTime, model.Miles);
                int pickupId = 0;
                if (!string.IsNullOrEmpty(model.PickupName) || !string.IsNullOrEmpty(model.PickupAddressLine1))
                {
                    string pickupNameToSave = model.PickupName.Split(',').FirstOrDefault();
                    pickupId = PickupEntry(pickupNameToSave, model.PickupInstructions, model.PickupAddressLine1, model.PickupCity, model.PickupState, model.PickupZip, model.Phone);
                }
                if (model.RequestType == "VHAN")
                {
                    string[] patientName = model.PatientName.Replace(" ", "").Split(',');
                    var patientFname = patientName[0].Trim();
                    var patientLname = patientName[1].Trim();

                    genServiceModel.PatientDemographics.FirstName = patientFname;
                    genServiceModel.PatientDemographics.LastName = patientLname;
                    genServiceModel.Insurances.InsuranceId = Convert.ToInt64(model.PatientInsuranceId);
                    genServiceModel.Insurances.Payorname = model.PatientInsuranceProvider;
                }
                int patientId = PatientEntry(genServiceModel.PatientDemographics);
                int censusId = CensusEntry(genServiceModel.Census, model.MrNumber);
                int coverageId = CoverageEntry(genServiceModel.Coverages, model.MrNumber);
                int insuranceId = InsuranceEntry(genServiceModel.Insurances, model.MrNumber);
                DischargeNeedsEntry(model.SpecialNeeds, patientId);

                string caseNumber = string.Empty;
                if (genServiceModel.Census != null)
                {
                    caseNumber = genServiceModel.Census.CaseNumber;
                }

                int dischargeRequestId = DischargeRequestEntry(callerId, destinationId, model.DischargeTime, insuranceId, censusId, model.SelectedEmsAgency, model.MrNumber, caseNumber, patientId, model.SpecialInstructions, model.LifeSupport, pickupId, model.RequestType, model.RequesStatus, model.Notes, model.AppointmentTime);
                if (dischargeRequestId != 0)
                {
                    model = new RequestModel
                    {
                        IsPostSuccess = "Success"
                    };
                    if (callerId ==0)
                    {
                        model.IsPostSuccess += " new caller";
                    }
                }
            }
            return View(model);
        }

        #region DischargeEntry

        /// <summary>
        ///
        /// </summary>
        /// <param name="census"></param>
        /// <returns></returns>
        private int CensusEntry(CensusModel census, string mrNumber)
        {
            int censusId;
            tblMbrCensu tcensus;
            if (census != null)
            {
                tcensus = new tblMbrCensu()
                {
                    Bed = census.Bed,
                    MrNumber = mrNumber,
                    PavillionCode = census.PavillionCode,
                    Unit = census.Unit,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };
            }
            else
            {
                tcensus = new tblMbrCensu()
                {
                    Bed = "N/A",
                    MrNumber = mrNumber,
                    PavillionCode = "N/A",
                    Unit = "N/A",
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };
            }

            using (var context = new lifeflightapps())
            {
                context.tblMbrCensus.Add(tcensus);
                context.SaveChanges();
                censusId = tcensus.MbrCensusID;
            }
            return censusId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="coverages"></param>
        /// <param name="mrNumber"></param>
        /// <returns></returns>
        private int CoverageEntry(CoverageInfo coverages, string mrNumber)
        {
            int coverageId;
            tblMbrCoverage tcoverage;
            if (coverages != null)
            {
                tcoverage = new tblMbrCoverage()
                {
                    CoverageTypeID = coverages.TypeId,
                    CoverageTypeName = coverages.Typename,
                    IsActive = coverages.IsActive,
                    MrNumber = mrNumber,
                    VerifiedByUser = coverages.VerifiedByUser,
                    VerifiedDate = coverages.VerifiedDate,
                    VerifiedStatus = coverages.VerifiedStatus,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
            }
            else
            {
                tcoverage = new tblMbrCoverage()
                {
                    CoverageTypeID = 999,
                    CoverageTypeName = "N/A",
                    IsActive = false,
                    MrNumber = mrNumber,
                    VerifiedByUser = "N/A",
                    VerifiedDate = DateTime.Now,
                    VerifiedStatus = "N/A",
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
            }
            using (var context = new lifeflightapps())
            {
                context.tblMbrCoverages.Add(tcoverage);
                context.SaveChanges();
                coverageId = tcoverage.MbrCoverageID;
            }
            return coverageId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="destinationType"></param>
        /// <param name="addressLineOne"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        private int DestinationEntry(string destinationType, string destinationName, string destinationInstructions, string addressLineOne, string city, string state, string zip, string phone, string travelTime, string miles)
        {
            int destinationId;
            int Miles = 0;
            if (!string.IsNullOrEmpty(miles))
            {
                if (miles.Contains("."))
                {
                    Miles = Convert.ToInt32(miles.Substring(0, miles.LastIndexOf(".")));
                }
                else
                {
                    Miles = Convert.ToInt32(miles);
                }
            }

            tblDischargeDestination tdestination = new tblDischargeDestination()
            {
                DestinationType = destinationType,
                DestinationName = destinationName,
                DestInstructions = destinationInstructions,
                AddressLineOne = addressLineOne,
                City = city,
                StateCode = state,
                Zip = zip,
                Miles = Miles,
                Phone = phone,
                TravelTime = Convert.ToDateTime(travelTime).TimeOfDay
            };
            using (var context = new lifeflightapps())
            {
                context.tblDischargeDestinations.Add(tdestination);
                context.SaveChanges();
                destinationId = tdestination.DestinationID;
            }
            return destinationId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="specialNeeds"></param>
        /// <param name="patientId"></param>
        private void DischargeNeedsEntry(List<SelectListItem> specialNeeds, int patientId)
        {
            specialNeeds = specialNeeds.Where(s => s.Selected == true).ToList();
            foreach (var item in specialNeeds)
            {
                tblDischargeNeed tdischargeneed = new tblDischargeNeed()
                {
                    patientid = patientId,
                    specialneedsid = Convert.ToInt32(item.Value),
                    active = true
                };

                using (var context = new lifeflightapps())
                {
                    context.tblDischargeNeeds.Add(tdischargeneed);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="callerId"></param>
        /// <param name="destinationId"></param>
        /// <param name="dischargeTime"></param>
        /// <param name="insuranceId"></param>
        /// <param name="censusId"></param>
        /// <param name="selectedEmsAgency"></param>
        /// <param name="mrNumber"></param>
        /// <param name="patientId"></param>
        /// <param name="specialInstructions"></param>
        /// <param name="lifeSupport"></param>
        /// <returns></returns>
        private int DischargeRequestEntry(int callerId, int destinationId, string dischargeTime, int insuranceId, int censusId, string selectedEmsAgency, string mrNumber, string CaseNumber, int patientId, string specialInstructions, string lifeSupport, int pickupId, string requestType, string requestStatus, string notes, string appointmentTime)
        {
            int dischargerequestId;
            DateTime? dischargeRequestTime = string.IsNullOrEmpty(dischargeTime) ? (DateTime?)null : Convert.ToDateTime(dischargeTime);
            tblDischargeRequest1 tdischargeRequest = new tblDischargeRequest1()
            {
                CallReceivedBy = User.Identity.Name, //Environment.UserName,
                CallReceivedDate = DateTime.Now,
                CallerID = callerId,
                DestinationID = destinationId,
                DischargeTime = dischargeRequestTime,
                InsuranceID = insuranceId,
                LocationID = censusId,
                ModeOfTransport = selectedEmsAgency,
                MrNumber = mrNumber,
                CaseNumber = CaseNumber,
                PatientID = patientId,
                SpecialInstructions = specialInstructions,
                Notes = notes,
                LifeSupport = lifeSupport,
                PickupID = pickupId,
                RequestType = requestType,
                RequestStatusID = Convert.ToInt32(requestStatus),
                AppointmentTime = string.IsNullOrEmpty(appointmentTime) ? (DateTime?)null : Convert.ToDateTime(appointmentTime)
            };

            using (var context = new lifeflightapps())
            {
                context.tblDischargeRequests1.Add(tdischargeRequest);
                context.SaveChanges();
                dischargerequestId = tdischargeRequest.RequestID;

                tblAuditLog auditLog = new tblAuditLog()
                {
                    APPNAME = "DTM",
                    IDFROMAPP = dischargerequestId,
                    MRNUMBER = mrNumber,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now,
                    UPDATENOTES = $"New request of type {requestType} has been created"
                };

                context.tblAuditLogs.Add(auditLog);
                context.SaveChanges();
            }
            return dischargerequestId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="callerName"></param>
        /// <returns></returns>
        private int GetCallerId(string callerName)
        {
            int callerid = 0;
            using (var context = new lifeflightapps())
            {
                callerid = context.tblCallers.Where(c => c.CallerLastName + " " + c.CallerFirstName == callerName).Select(i => i.ID).FirstOrDefault();

                //if (callerid == 0)
                //{
                //    string[] splitCallerName = callerName.Split(' ');
                //    string Ln = splitCallerName[0].ToString();
                //    string Fn = splitCallerName[1].ToString();
                //    tblCaller tcaller = new tblCaller()
                //    {
                //        Active = true,
                //        CallerFirstName = Fn,
                //        CallerLastName = Ln
                //    };
                //    context.tblCallers.Add(tcaller);
                //    context.SaveChanges();
                //    callerid = tcaller.ID;
                //}
            }
            return callerid;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="insurances"></param>
        /// <param name="mrNumber"></param>
        /// <returns></returns>
        private int InsuranceEntry(InsuranceInformation insurances, string mrNumber)
        {
            int insuranceId;
            tblMbrInsurance tinsurance;
            if (insurances != null)
            {
                tinsurance = new tblMbrInsurance()
                {
                    FinancialName = insurances.FinancialName,
                    PayorName = insurances.Payorname,
                    PayorID = Convert.ToInt32(insurances.PayorId),
                    PlanID = Convert.ToInt32(insurances.PlanId),
                    PlanName = insurances.PlanName,
                    PlanType = insurances.PlanType,
                    SubscriberName = insurances.SubscriberName,
                    MrNumber = mrNumber,
                    Effective = insurances.EffectiveDate,
                    Termination = insurances.TerminationDate,
                    FillingOrder = insurances.FilingOrder,
                    InsuranceID = insurances.InsuranceId,
                    MedipacPlanID = insurances.MedipacPlanId,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
            }
            else
            {
                tinsurance = new tblMbrInsurance()
                {
                    FinancialName = "N/A",
                    PayorName = "N/A",
                    PayorID = 999,
                    PlanID = 999,
                    PlanName = "N/A",
                    PlanType = "N/A",
                    SubscriberName = "N/A",
                    MrNumber = mrNumber,
                    Effective = DateTime.Now,
                    Termination = DateTime.Now,
                    FillingOrder = 999,
                    InsuranceID = 999,
                    MedipacPlanID = "N/A",
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
            }
            using (var context = new lifeflightapps())
            {
                context.tblMbrInsurances.Add(tinsurance);
                context.SaveChanges();
                insuranceId = tinsurance.MbrInsuranceID;
            }
            return insuranceId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="patientDemographics"></param>
        /// <returns></returns>
        private int PatientEntry(Patient patient)
        {
            int patientId;
            tblPatient tPatient = new tblPatient()
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Address = patient.Address ?? "N/A",
                Age = patient.Age ?? 0,
                City = patient.City ?? "N/A",
                DateOfBirth = (!string.IsNullOrEmpty(patient.DOB)) ? Convert.ToDateTime(patient.DOB) : Convert.ToDateTime("01/01/1900"),
                Phone = patient.Phone ?? "N/A",
                Social = patient.Social ?? "N/A",
                State = patient.State ?? "N/A",
                Weight = Convert.ToInt32(patient.Weight),
                WeightUnit = patient.WeightUnit,
                MrNumber = patient.MRN,
                Zip = patient.Zip ?? "N/A",
                CreatedBy = User.Identity.Name,
                CreatedDate = DateTime.Now
            };

            using (var context = new lifeflightapps())
            {
                context.tblPatients.Add(tPatient);
                context.SaveChanges();
                patientId = tPatient.PatientID;
            }
            return patientId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pickupNameToSave"></param>
        /// <param name="pickupInstructions"></param>
        /// <param name="pickupAddressLine1"></param>
        /// <param name="pickupCity"></param>
        /// <param name="pickupState"></param>
        /// <param name="pickupZip"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        private int PickupEntry(string pickupNameToSave, string pickupInstructions, string pickupAddressLine1, string pickupCity, string pickupState, string pickupZip, string phone)
        {
            int pickupId;
            tblDischargePickup tpickup = new tblDischargePickup()
            {
                PickupLocationName = pickupNameToSave,
                PickupInstructions = pickupInstructions,
                AddressLineOne = pickupAddressLine1,
                City = pickupCity,
                StateCode = pickupState,
                Zip = pickupZip,
                Phone = phone
            };
            using (var context = new lifeflightapps())
            {
                context.tblDischargePickups.Add(tpickup);
                context.SaveChanges();
                pickupId = tpickup.PickupID;
            }
            return pickupId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mrNumber"></param>
        /// <returns></returns>
        private GenServiceModel RetrieveDataFromGenServices(string mrNumber, string vunetId)
        {
            GenServiceModel gsm = new GenServiceModel();
            RetrieveCoverageAndInsurance coverageinfo = new RetrieveCoverageAndInsurance();
            RetrievePatientInformation patientinfo = new RetrievePatientInformation();
            RetrieveCensusInformation censusinfo = new RetrieveCensusInformation();
            CoverageInfo coverages = coverageinfo.GetCoverageAndInsurance(mrNumber, vunetId).Where(c => c.IsActive == true).FirstOrDefault();
            InsuranceInformation insurances = coverageinfo.GetInsurances(mrNumber, vunetId).Where(i => i.EffectiveDate <= DateTime.Today && (i.TerminationDate >= DateTime.Today || i.TerminationDate == null)).FirstOrDefault();
            Patient patient = patientinfo.GetPatient(mrNumber, vunetId);
            CensusModel census = censusinfo.GetCensusRecords(mrNumber, vunetId).FirstOrDefault();

            gsm.PatientDemographics = patient;
            gsm.Coverages = coverages;
            gsm.Insurances = insurances;
            gsm.Census = census;

            return gsm;
        }
        #endregion DischargeEntry

        #region Ajax calls

        /// <summary>
        /// Gets the city and state based on the zip code provided
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCityState(string zip)
        {
            tblZipCityState cityState = new tblZipCityState();
            using (var context = new lifeflightapps())
            {
                cityState = context.tblZipCityStates.Where(z => z.Zip == zip).FirstOrDefault();
            }
            AddressFill fill = new AddressFill()
            {
                City = cityState.City,
                State = cityState.StateCode
            };
            return Json(fill, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the destination address
        /// </summary>
        /// <param name="Destination"></param>
        /// <param name="Filter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDestinationAddress(string Destination, string Filter)
        {
            string[] splitTaskName = Destination.Split(',');
            string destinationName = splitTaskName[0].ToString();
            string statecode = splitTaskName[2].ToString();
            Union_Facilities facilities = new Union_Facilities();
            using (var context = new lifeflightapps())
            {
                facilities = context.Union_Facilities.Where(d => d.NAME == destinationName && d.STATE == statecode).FirstOrDefault();
            }
            AddressFill fill = new AddressFill()
            {
                AddressLineOne = facilities.ADDRESS,
                City = facilities.CITY,
                State = facilities.STATE,
                Zip = facilities.ZIP,
                Phone = facilities.PHONE
            };
            return Json(fill, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shows the callers names from the tblCallers table
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonResult GetFilteredCallers(string term)
        {
            List<string> callers = new List<string>();
            IQueryable<string> dbCallers;
            using (var context = new lifeflightapps())
            {
                dbCallers = context.tblCallers.Where(s => s.Active == true && (s.CallerLastName.Contains(term) || s.CallerFirstName.Contains(term))).Select(x => x.CallerLastName + " " + x.CallerFirstName);
                callers = dbCallers.ToList();
            }

            return Json(callers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the destinations as per the state and request type
        /// </summary>
        /// <param name="term"></param>
        /// <param name="Filter"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public JsonResult GetFilteredDestinations(string term, string Filter, string State)
        {
            List<string> DestinationNames;
            if (!string.IsNullOrEmpty(Filter) && (Filter == "ALT Funding"))
            {
                DestinationNames = RetrieveVanderbiltDestinations().Select(d => d.DestinationName + "," + d.City + "," + d.State).ToList();
            }
            else
            {
                DestinationNames = RetrieveDesiredDestinations(term, State).Select(d => d.DestinationName + "," + d.City + "," + d.State).ToList();
            }
            return Json(DestinationNames, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the patient's address
        /// </summary>
        /// <param name="MrNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPatientAddress(string MrNumber)
        {
            //string patientAddress = string.Empty;
            string vunetId = User.Identity.Name;
            RetrievePatientInformation patientinfo = new RetrievePatientInformation();
            Patient patient = patientinfo.GetPatient(MrNumber, vunetId);
            //patientAddress = patient.Address + ", " + patient.City + ", " + patient.State + ", " + patient.Zip;
            if (!string.IsNullOrEmpty(patient.MRN))
            {
                RequestModel requestModel = new RequestModel();
                if (patient.State.Length > 2)
                {
                    var states = requestModel.GetStatesList();
                    patient.State = states.Where(x => x.Text == patient.State).FirstOrDefault().Value;
                }
                AddressFill fill = new AddressFill()
                {
                    AddressLineOne = patient.Address.Replace(",", ""),
                    City = patient.City,
                    State = patient.State,
                    Zip = patient.Zip,
                    Phone = patient.Phone
                };
                return Json(fill, JsonRequestBehavior.AllowGet);
            }
            else return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieves a patient MRN by name
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RetrieveByName(string firstName, string lastName)
        {
            //Patient retrievedPatient = new Patient();
            RetrievePatientInformation patientinfo = new RetrievePatientInformation();

            //retrievedPatient = patientinfo.GetPatientByName(firstName, lastName);
            string MRNumber = patientinfo.GetMrnByName(firstName, lastName);

            return Json(MRNumber, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Validates the MRN
        /// </summary>
        /// <param name="mrn"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidateMrNumber(string mrn)
        {
            string vunetId = User.Identity.Name;
            PatientFill fill = null;
            RetrieveCensusInformation censusinfo = new RetrieveCensusInformation();
            int censusRecords = censusinfo.GetCensusCount(mrn);
            if (censusRecords > 0)
            {
                RetrieveCoverageAndInsurance covandins = new RetrieveCoverageAndInsurance();
                var ins = covandins.GetInsurances(mrn, vunetId).Where(i => i.EffectiveDate <= DateTime.Today && (i.TerminationDate >= DateTime.Today || i.TerminationDate == null)).FirstOrDefault();
                long insId = 0;
                if (ins != null)
                {
                    insId = ins.InsuranceId;
                }
                RetrievePatientInformation patientinfo = new RetrievePatientInformation();
                Patient patient = patientinfo.GetPatient(mrn, vunetId);

                fill = new PatientFill()
                {
                    MrNumberFromEpi = string.IsNullOrEmpty(patient.MRN) ? "0" : patient.MRN,
                    PatientFName = patient.FirstName,
                    PatientLName = patient.LastName,
                    InsuranceIdFromEpi = insId
                };
            }
            return Json(fill, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns all the destinations from the Union_Facilities
        /// </summary>
        /// <param name="searchDestination"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        private List<Destination> RetrieveDesiredDestinations(string searchDestination, string State)
        {
            List<Destination> destinationList = new List<Destination>();

            if (!string.IsNullOrEmpty(State))
            {
                using (var context = new lifeflightapps())
                {
                    var retrievedDestinations = context.Union_Facilities.Select(n => new { n.id2, n.NAME, n.ADDRESS, n.CITY, n.STATE, n.ZIP, n.COUNTY, n.PHONE })
                                                           .Where(nu => nu.NAME.Contains(searchDestination) && nu.STATE == State).Take(20).AsEnumerable().ToList();
                    foreach (var destination in retrievedDestinations)
                    {
                        Destination hospitalDestination = new Destination()
                        {
                            Address = destination.ADDRESS,
                            City = destination.CITY,
                            County = destination.COUNTY,
                            DestinationId = destination.id2,
                            DestinationName = destination.NAME,
                            Phone = destination.PHONE ?? string.Empty,
                            State = destination.STATE,
                            Zip = destination.ZIP
                        };
                        destinationList.Add(hospitalDestination);
                    }
                }
            }
            else
            {
                using (var context = new lifeflightapps())
                {
                    var retrievedDestinations = context.Union_Facilities.Select(n => new { n.id2, n.NAME, n.ADDRESS, n.CITY, n.STATE, n.ZIP, n.COUNTY, n.PHONE })
                                                          .Where(nu => nu.NAME.Contains(searchDestination)).Take(20).AsEnumerable();
                    foreach (var destination in retrievedDestinations)
                    {
                        Destination hospitalDestination = new Destination()
                        {
                            Address = destination.ADDRESS,
                            City = destination.CITY,
                            County = destination.COUNTY,
                            DestinationId = destination.id2,
                            DestinationName = destination.NAME,
                            Phone = destination.PHONE ?? string.Empty,
                            State = destination.STATE,
                            Zip = destination.ZIP
                        };
                        destinationList.Add(hospitalDestination);
                    }
                }
            }
            return destinationList;
        }

        /// <summary>
        /// Retruns only the Vanderbilt destination for request type ALT Funding
        /// </summary>
        /// <returns></returns>
        private List<Destination> RetrieveVanderbiltDestinations()
        {
            List<Destination> destinationList = new List<Destination>();
            using (var context = new lifeflightapps())
            {
                var destinationsView = context.Union_Facilities.Select(n => new { n.id2, n.NAME, n.ADDRESS, n.CITY, n.STATE, n.ZIP, n.COUNTY, n.PHONE }).Where(nu => nu.NAME.Contains("vanderbilt") && nu.STATE == "TN").AsEnumerable();
                foreach (var destination in destinationsView)
                {
                    Destination hospitalDestination = new Destination()
                    {
                        Address = destination.ADDRESS,
                        City = destination.CITY,
                        County = destination.COUNTY,
                        DestinationId = destination.id2,
                        DestinationName = destination.NAME,
                        Phone = destination.PHONE ?? string.Empty,
                        State = destination.STATE,
                        Zip = destination.ZIP
                    };
                    destinationList.Add(hospitalDestination);
                }
            }
            return destinationList;
        }
        #endregion Ajax calls
    }

    #region Helper Classes

    /// <summary>
    /// Helper class for the address
    /// </summary>
    public class AddressFill
    {
        public string AddressLineOne { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
    }

    /// <summary>
    /// Helper class for the patient information
    /// </summary>
    public class PatientFill
    {
        public long InsuranceIdFromEpi { get; set; }
        public string MrNumberFromEpi { get; set; }
        public string PatientFName { get; set; }
        public string PatientLName { get; set; }
    }
    #endregion Helper Classes
}