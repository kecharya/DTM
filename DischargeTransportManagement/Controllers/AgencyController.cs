using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class AgencyController : Controller
    {
        /// <summary>
        /// Get all the agencies, 10 per page and ability to search on agency name
        /// </summary>
        /// <param name="page"></param>
        /// <param name="agencyName"></param>
        /// <returns></returns>
        public ActionResult AllAgencies(int? page, string agencyName)
        {
            List<AgencyModel> agencyModels = new List<AgencyModel>();

            using (var context = new lifeflightapps())
            {
                if (!string.IsNullOrEmpty(agencyName))
                {
                    string[] agencyDetails = agencyName.Split('-');
                    string name = agencyDetails[0].Trim();
                    string city = agencyDetails[1].Trim();
                    string state = agencyDetails[2].Trim();
                    var agencyQueried =
                        context.tblEmsAgencyLocals.FirstOrDefault(c => c.Name == name && c.City == city && c.State == state);

                    if (agencyQueried != null)
                    {
                        return RedirectToAction("GetAgencyDetails", new { agencyId = agencyQueried.EMSID });
                    }
                }
                var agencies = context.tblEmsAgencyLocals.ToList();
                foreach (var agency in agencies)
                {
                    AgencyModel model = new AgencyModel()
                    {
                        AddressLn1 = agency.Address,
                        AddressLn2 = agency.Address2,
                        LocalUse = agency.LocalUse,
                        AgencyId = agency.EMSID,
                        AgencyName = agency.Name,
                        City = agency.City,
                        StateCode = agency.State,
                        Zip = agency.Zip,
                        Phone = agency.Phones,
                        AgencyType = agency.Type,
                        County = agency.County
                    };
                    agencyModels.Add(model);
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(agencyModels.OrderBy(x=>x.AgencyName).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Get the agency details
        /// </summary>
        /// <param name="agencyId"></param>
        /// <returns></returns>
        public ActionResult GetAgencyDetails(int agencyId)
        {
            tblEmsAgencyLocal agency = new tblEmsAgencyLocal();
            using (var context = new lifeflightapps())
            {
                agency = context.tblEmsAgencyLocals.Find(agencyId);
            }
            AgencyModel agencyModel = new AgencyModel()
            {
                AgencyId =  agency.EMSID,
                AddressLn1 = agency.Address,
                AddressLn2 =  agency.Address2,
                City = agency.City,
                StateCode = agency.State,
                AgencyName = agency.Name,
                LocalUse = agency.LocalUse,
                AgencyType = agency.Type,
                Zip = agency.Zip,
                County = agency.County,
                Phone = agency.Phones,
                BaseRate = agency.BaseRate,
                MileageRate = agency.MileageRate,
                Notes = agency.Notes,
                TaxId = Convert.ToInt32(agency.taxID)
            };

            return View(agencyModel);
        }

        /// <summary>
        /// HTTPGet for the agency EDIT
        /// </summary>
        /// <param name="agencyId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditAgency(int agencyId)
        {
            tblEmsAgencyLocal agency = new tblEmsAgencyLocal();
            using (var context = new lifeflightapps())
            {
                agency = context.tblEmsAgencyLocals.Find(agencyId);
            }
            AgencyModel agencyModel = new AgencyModel()
            {
                AgencyId = agency.EMSID,
                AddressLn1 = agency.Address,
                AddressLn2 = agency.Address2,
                City = agency.City,
                StateCode = agency.State,
                AgencyName = agency.Name,
                LocalUse = agency.LocalUse,
                AgencyType = agency.Type,
                Zip = agency.Zip,
                County = agency.County,
                Phone = agency.Phones,
                BaseRate = agency.BaseRate,
                MileageRate = agency.MileageRate,
                TaxId = Convert.ToInt32(agency.taxID)
            };
            return View(agencyModel);
        }

        /// <summary>
        /// HTTPPost for the Agency EDIT
        /// </summary>
        /// <param name="agency"></param>
        /// <returns></returns>
        [HttpPost, ActionName("EditAgency")]
        public ActionResult EditAgencyDetails(AgencyModel agency)
        {
            if (ModelState.IsValid)
            {
                tblEmsAgencyLocal dbAgencyLocal = new tblEmsAgencyLocal()
                {
                    EMSID = agency.AgencyId,
                    Address = agency.AddressLn1,
                    Address2 = agency.AddressLn2,
                    City = agency.City,
                    State = agency.StateCode,
                    Name = agency.AgencyName,
                    LocalUse = agency.LocalUse,
                    Type = agency.AgencyType,
                    Zip = agency.Zip,
                    County = agency.County,
                    Phones = agency.Phone,
                    BaseRate = agency.BaseRate,
                    MileageRate = agency.MileageRate,
                    taxID = agency.TaxId.ToString()
                };
                try
                {
                    using (var context = new lifeflightapps())
                    {
                        context.tblEmsAgencyLocals.AddOrUpdate(dbAgencyLocal);
                        context.SaveChanges();
                    }
                    return RedirectToAction("GetAgencyDetails", new { agencyId = agency.AgencyId });
                }
                catch (DataException dx)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    throw dx;
                }
            }
            else
            {
                return View(agency);
            }
        }

        public ActionResult AddAgency(AgencyModel agency)
        {
            if (string.IsNullOrEmpty(agency.AgencyName))
            {
                AgencyModel model = new AgencyModel();
                RequestModel requestModel = new RequestModel();
                model.StatesList = requestModel.GetStatesList().ToList();
                return View(model);
            }
            else
            {
                using (var context = new lifeflightapps())
                {
                    tblEmsAgencyLocal emsAgency = new tblEmsAgencyLocal()
                    {
                        Name = agency.AgencyName,
                        Address = agency.AddressLn1,
                        City = agency.City,
                        County = agency.County,
                        State = agency.StateCode,
                        Zip = agency.Zip,
                        Phones = agency.Phone,
                        LocalUse = agency.LocalUse,
                        Type = agency.AgencyType,
                        BaseRate = agency.BaseRate,
                        MileageRate = agency.MileageRate,
                        Notes = agency.Notes
                    };

                    context.tblEmsAgencyLocals.Add(emsAgency);
                    context.SaveChanges();
                    int id = emsAgency.EMSID;

                    return RedirectToAction("GetAgencyDetails", new { agencyId = id });
                }
                
            }
        }

        /// <summary>
        /// Autocomplete for the agency search
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonResult GetFilteredAgencies(string term)
        {
            List<string> agencies = new List<string>();
            using (var context = new lifeflightapps())
            {
                agencies = context.tblEmsAgencyLocals.Where(s => s.Name.StartsWith(term)).Select(x => x.Name + " - " + x.City + " - " + x.State).ToList();
            }
            return Json(agencies, JsonRequestBehavior.AllowGet);
        }
    }
}