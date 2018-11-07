using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class InsuranceAgencyController : Controller
    {
        // GET: InsuranceAgency
        public ActionResult InsuranceAgencyAdmin()
        {
            InsuranceAgencyViewModel model = new InsuranceAgencyViewModel();
            List<SelectListItem> ins = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                ins = context.tblInsuranceAgencies.Where(x => x.INSAGENCYNAME != null).Select(x => new SelectListItem()
                {
                    Value = x.INSURANCEID.ToString(),
                    Text = x.INSAGENCYNAME
                }).ToList();
            }
            model.Insurances = ins.ToList();
            return View(model);
        }

        /// <summary>
        /// Gets all the insurance agencies and displays them 10 per page
        /// and also has an ability to search 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="InsAgencyName"></param>
        /// <returns></returns>
        public ActionResult InsuranceAgencies(int? page, string InsAgencyName)
        {
            List<InsuranceModel> models = new List<InsuranceModel>();
            using (var context = new lifeflightapps())
            {
                if (!string.IsNullOrEmpty(InsAgencyName))
                {
                    var insAgencyQueried =
                        context.tblInsuranceAgencies.FirstOrDefault(c => c.INSAGENCYNAME == InsAgencyName);

                    if (insAgencyQueried != null)
                    {
                        return RedirectToAction("GetInsuranceAgencyDetails", new { insAgencyId = insAgencyQueried.INSURANCEID});
                    }
                }

                var insAgencies = context.tblInsuranceAgencies.ToList();
                models = ConvertToInsModel(insAgencies);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(models.OrderBy(x => x.InsuranceName).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Gets the insurance agency details
        /// </summary>
        /// <param name="insAgencyId"></param>
        /// <returns></returns>
        public ActionResult GetInsuranceAgencyDetails(int insAgencyId)
        {
            InsuranceModel model = GetInsuranceFromDB(insAgencyId);//new InsuranceModel();

            return View(model);
        }

        /// <summary>
        /// Make a DB call to get the insurance details
        /// </summary>
        /// <param name="insAgencyId"></param>
        /// <returns></returns>
        public InsuranceModel GetInsuranceFromDB(int insAgencyId)
        {
            InsuranceModel model = new InsuranceModel();
            tblInsuranceAgency insuranceAgency = new tblInsuranceAgency();

            using (var context = new lifeflightapps())
            {
                insuranceAgency = context.tblInsuranceAgencies.Find(insAgencyId);
                model = ConvertToInsuranceModel(insuranceAgency);
            }
            return model;
        }

        /// <summary>
        /// Convert the Insurance DB entity to Model
        /// </summary>
        /// <param name="insAgencies"></param>
        /// <returns></returns>
        private List<InsuranceModel> ConvertToInsModel(List<tblInsuranceAgency> insAgencies)
        {
            List<InsuranceModel> models = new List<InsuranceModel>();
            foreach (var i in insAgencies)
            {
                InsuranceModel model = new InsuranceModel()
                {
                    InsuranceId = i.INSURANCEID,
                    InsuranceName = i.INSAGENCYNAME,
                    Description = i.DESCRIPTION,
                    InsuranceFax = i.INSURANCEFAX,
                    InsurancePhone = i.INSURANCEPHONE,
                    Instructions = i.INSTRUCTIONS,
                    PreAuthRequired = i.PREAUTHREQUIRED.HasValue ? i.PREAUTHREQUIRED.Value : false,
                    HasOwnPaperWork = i.HASOWNPAPEROWORK.HasValue ? i.HASOWNPAPEROWORK.Value : false,
                    CreatedBy = i.CREATEDBY,
                    CreatedOn = i.CREATEDON
                };
                models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// HttpGet for adding new insurance agency
        /// </summary>
        /// <returns></returns>
        public ActionResult AddInsuranceAgency()
        {
            InsuranceModel model = new InsuranceModel();
            return View(model);
        }

        /// <summary>
        /// Converts the insurance entity to model
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        private InsuranceModel ConvertToInsuranceModel(tblInsuranceAgency ins)
        {
            InsuranceModel model = new InsuranceModel();

            using (var context = new lifeflightapps())
            {
                var contacts = context.tblInsuranceAgencyContacts.Where(x => x.InsuranceID == ins.INSURANCEID).ToList();
                List<InsuranceContactModel> contactModels = new List<InsuranceContactModel>();
                contactModels = ConvertToContactsModel(contacts);
                InsuranceModel insModel = new InsuranceModel()
                {
                    InsuranceId = ins.INSURANCEID,
                    InsuranceName = ins.INSAGENCYNAME,
                    Description = ins.DESCRIPTION,
                    Instructions = ins.INSTRUCTIONS,
                    InsurancePhone = ins.INSURANCEPHONE,
                    InsuranceFax = ins.INSURANCEFAX,
                    PreAuthRequired = ins.PREAUTHREQUIRED.HasValue ? ins.PREAUTHREQUIRED.Value : false,
                    HasOwnPaperWork = ins.HASOWNPAPEROWORK.HasValue ? ins.HASOWNPAPEROWORK.Value : false,
                    CreatedBy = ins.CREATEDBY,
                    CreatedOn = ins.CREATEDON,
                    contactModels = contactModels
                };
                model = insModel;
            }
            return model;
        }

        /// <summary>
        /// Converts the contact entity to model
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        private List<InsuranceContactModel> ConvertToContactsModel(List<tblInsuranceAgencyContact> contacts)
        {
            List<InsuranceContactModel> contactModels = new List<InsuranceContactModel>();
            foreach (var item in contacts)
            {
                InsuranceContactModel model = new InsuranceContactModel()
                {
                    ContactName = item.ContactName,
                    ContactPhone = item.ContactPhone,
                    ContactEmail = item.ContactEmail,
                    ContactFax = item.ContactFax,
                    ContactNote = item.ContactNote
                };

                contactModels.Add(model);
            }
            return contactModels;
        }

        /// <summary>
        /// Get the EMS Agencies for an insurance
        /// </summary>
        /// <param name="InsProvider"></param>
        /// <returns></returns>
        public JsonResult GetAgenciesByInsurance(int InsProvider)
        {
            IEnumerable<SelectListItem> allAgencies;// = new IEnumerable<SelectListItem>();
            IEnumerable<SelectListItem> selectedAgencies;//= new IEnumerable<SelectListItem>();
            //get all agencies
            List<SelectListItem> agns = new List<SelectListItem>();
            using (var context = new lifeflightapps())
            {
                agns = context.tblEmsAgencyLocals.Select(x => new SelectListItem()
                {
                    Text = x.Name + " " + x.City + " " + x.State,
                    Value = x.EMSID.ToString()
                }).ToList();


                allAgencies = agns.ToList().OrderBy(x => x.Text);
                selectedAgencies = new List<SelectListItem>();

                //get agencies assigned for the specific insurance provider
                var insagencies = from x in context.tblInsuranceEmsXrefs select new { x.AgencyId, x.InsuranceProviderID };
                // Where(x => x.InsuranceProviderID == Convert.ToInt32(InsProvider)).Select(x => x.AgencyId).ToList();
                var myAgencies = insagencies.ToList().Where(x => x.InsuranceProviderID == InsProvider).Select(x => x.AgencyId).ToList();

                //get the agencies from the all agencies
                if (myAgencies.Any())
                {
                    var agencies = context.tblEmsAgencyLocals.Where(x => myAgencies.Contains(x.EMSID));

                    var agenciesselected = agencies.Select(x => new SelectListItem()
                    {
                        Text = x.Name + " " + x.City + " " + x.State,
                        Value = x.EMSID.ToString()
                    });
                    selectedAgencies = agenciesselected.ToList().OrderBy(x => x.Text);
                    foreach (var item in selectedAgencies)
                    {
                        allAgencies = allAgencies.Where(x => x.Value != item.Value);
                    }

                }
            }   
            return Json(new {all = allAgencies, selectd = selectedAgencies }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add the insurance xref EMS reference
        /// </summary>
        /// <param name="InsuranceId"></param>
        /// <param name="Agencies"></param>
        /// <returns></returns>
        public JsonResult AddInsAgencies(int InsuranceId, string Agencies)
        {
            string[] selectedAgencies = Agencies.Split(',');
            //var insagencies = db.tblInsuranceEmsXrefs.Where(x => x.InsuranceProviderID == InsuranceId).Select(x => x.AgencyId);
            //if (insagencies.Any())
            //{

            //}
            //else
            //{
                foreach (var item in selectedAgencies)
                {
                    tblInsuranceEmsXref xref = new tblInsuranceEmsXref()
                    {
                        Active = true,
                        AgencyId = Convert.ToInt32(item),
                        InsuranceProviderID = InsuranceId
                    };

                    using (var context = new lifeflightapps())
                    {
                        context.tblInsuranceEmsXrefs.Add(xref);
                        context.SaveChanges();
                    }
                }
            //}
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Remove the insurance xref EMS reference
        /// </summary>
        /// <param name="InsuranceId"></param>
        /// <param name="Agencies"></param>
        /// <returns></returns>
        public JsonResult RemoveInsAgencies(int InsuranceId, string[] Agencies)
        {
            string[] selectedAgencies = Agencies;
            foreach (var item in selectedAgencies)
            {
                //tblInsuranceEmsXref xref = new tblInsuranceEmsXref()
                //{
                //    Active = true,
                //    AgencyId = Convert.ToInt32(item),
                //    InsuranceProviderID = InsuranceId
                //};
                int agencyid = Convert.ToInt32(item);
                //tblInsuranceEmsXref xref = db.tblInsuranceEmsXrefs.Where(x => x.AgencyId == agencyid && x.InsuranceProviderID == InsuranceId).FirstOrDefault();
                using (var context = new lifeflightapps())
                {
                    var xref = context.tblInsuranceEmsXrefs.Where(x => x.AgencyId == agencyid && x.InsuranceProviderID == InsuranceId).FirstOrDefault();
                    context.tblInsuranceEmsXrefs.Attach(xref);
                    context.tblInsuranceEmsXrefs.Remove(xref);
                    context.SaveChanges();
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AutoComplete for the insurance agency names
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonResult GetFilteredInsAgencies(string term)
        {
            List<string> InsuranceAgencies = new List<string>();
            using (var context = new lifeflightapps())
            {
                InsuranceAgencies = context.tblInsuranceAgencies.Where(s => s.INSAGENCYNAME.StartsWith(term)).Select(x => x.INSAGENCYNAME).ToList();
            }
            return Json(InsuranceAgencies, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Autocomplete for the insurance descriptions
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonResult GetInsDescriptions(string term)
        {
            List<string> InsDescriptions = new List<string>();
            using (var context = new lifeflightapps())
            {
                InsDescriptions = context.tblInsuranceAgencies.Select(x => x.DESCRIPTION).Distinct().ToList();
            }
            return Json(InsDescriptions, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add contacts for the insurance agency
        /// </summary>
        /// <param name="InsuranceID"></param>
        /// <param name="Name"></param>
        /// <param name="Email"></param>
        /// <param name="Phone"></param>
        /// <param name="Fax"></param>
        /// <param name="Note"></param>
        /// <returns></returns>
        public JsonResult AddInsuranceContact(int InsuranceID, string Name, string Email, string Phone, string Fax, string Note)
        {
            tblInsuranceAgencyContact contact = new tblInsuranceAgencyContact()
            {
                InsuranceID = InsuranceID,
                ContactName = Name,
                ContactEmail = Email,
                ContactPhone = Phone,
                ContactFax = Fax,
                ContactNote = Note,
                CreatedBy = User.Identity.Name,
                CreatedDate = DateTime.Now
            };
            using (var context = new lifeflightapps())
            {
                context.tblInsuranceAgencyContacts.Add(contact);
                context.SaveChanges();   
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update the insurance informaiton
        /// </summary>
        /// <param name="InsuranceID"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="Phone"></param>
        /// <param name="Fax"></param>
        /// <param name="PreAuth"></param>
        /// <param name="OwnPaper"></param>
        /// <param name="Instructions"></param>
        /// <returns></returns>
        public JsonResult UpdateInsuranceInfo(int InsuranceID, string Name, string Description, string Phone, string Fax, bool PreAuth, bool OwnPaper, string Instructions)
        {
            using (var context = new lifeflightapps())
            {
                var ins = context.tblInsuranceAgencies.Find(InsuranceID);
                ins.INSAGENCYNAME = Name;
                ins.DESCRIPTION = Description;
                ins.INSURANCEPHONE = Phone;
                ins.INSURANCEFAX = Fax;
                ins.HASOWNPAPEROWORK = OwnPaper;
                ins.PREAUTHREQUIRED = PreAuth;
                ins.INSTRUCTIONS = Instructions;
                ins.UPDATEDBY = User.Identity.Name;
                ins.UPDATEDON = DateTime.Now;
                context.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add new insurance agency
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="Phone"></param>
        /// <param name="Fax"></param>
        /// <param name="PreAuth"></param>
        /// <param name="OwnPaper"></param>
        /// <param name="Instructions"></param>
        /// <returns></returns>
        public JsonResult AddNewInsuranceAgency(string Name, string Description, string Phone, string Fax, bool PreAuth, bool OwnPaper, string Instructions)
        {
            int insuranceID = 0;
            using (var context = new lifeflightapps())
            {
                tblInsuranceAgency insAgency = new tblInsuranceAgency()
                {
                    INSAGENCYNAME = Name,
                    DESCRIPTION = Description,
                    INSURANCEPHONE = Phone,
                    INSURANCEFAX = Fax,
                    PREAUTHREQUIRED = PreAuth,
                    INSTRUCTIONS = Instructions,
                    CREATEDBY = User.Identity.Name,
                    CREATEDON = DateTime.Now
                };
                context.tblInsuranceAgencies.Add(insAgency);
                context.SaveChanges();
                insuranceID = insAgency.INSURANCEID;
            }
            return Json(insuranceID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// CHeck if the insurance name exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public JsonResult ValidateInsuranceName(string Name)
        {
            bool doesExist = false;
            using (var context = new lifeflightapps())
            {
                var fromDB = context.tblInsuranceAgencies.Where(x => x.INSAGENCYNAME == Name).FirstOrDefault();
                if (fromDB != null)
                {
                    if (!string.IsNullOrEmpty(fromDB.INSAGENCYNAME))
                    {
                        doesExist = true;
                    }
                }
            }
            return Json(doesExist, JsonRequestBehavior.AllowGet);
        }

    }
}