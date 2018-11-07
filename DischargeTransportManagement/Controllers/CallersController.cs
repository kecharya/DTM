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
    [Authorize]
    public class CallersController : Controller
    {

        public ActionResult LoadCallers()
        {
            using (var context = new lifeflightapps())
            {
                var data = context.tblCallers.OrderBy(c => c.CallerLastName).ThenBy(ca => ca.CallerFirstName).ToList();
                return Json(data: new {  data }, behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        // GET: Callers
        public ActionResult Index(int? page, string SearchString)
        {
            List<Caller> callers = new List<Caller>();
            using (var context = new lifeflightapps())
            {
                var dbCaller = context.tblCallers.Where(ca => ca.CallerFirstName != null && ca.CallerLastName != null)
                    .Select(c => new { c.ID, c.CallerFirstName, c.CallerLastName, c.CallerTitle, c.Assignment, c.OfficePhone, c.CallerPager, c.MobilePhone, c.CallerEmail })
                                            .OrderBy(l => l.CallerLastName).ThenBy(f => f.CallerFirstName)
                                            .AsEnumerable();

                if (!string.IsNullOrEmpty(SearchString))
                {

                    var cid = dbCaller.Where(c => c.CallerLastName + " " + c.CallerFirstName == SearchString).FirstOrDefault();

                    return RedirectToAction("Details", new { id = cid.ID });
                }

                foreach (var caller in dbCaller)
                {
                    Caller model = new Caller()
                    {
                        CallerId = caller.ID,
                        CallerFirstName = caller.CallerFirstName,
                        CallerLastName = caller.CallerLastName,
                        CallerTitle = caller.CallerTitle,
                        Assignment = caller.Assignment,
                        OfficePhone = caller.OfficePhone,
                        CallerPager = caller.CallerPager,
                        MobilePhone = caller.MobilePhone,
                        CallerEmail = caller.CallerEmail
                    };
                    callers.Add(model);
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(callers.ToPagedList(pageNumber, pageSize));
        }


        // GET: Callers/Details/5
        public ActionResult Details(int id)
        {
            using (var context = new lifeflightapps())
            {
                var caller = context.tblCallers.Select(c => new { c.ID, c.CallerFirstName, c.CallerLastName, c.CallerTitle, c.Assignment, c.OfficePhone, c.CallerPager, c.MobilePhone, c.CallerEmail })
                    .Where(i => i.ID == id)
                    .FirstOrDefault();
                Caller model = new Caller()
                {
                    CallerId = caller.ID,
                    CallerFirstName = caller.CallerFirstName,
                    CallerLastName = caller.CallerLastName,
                    CallerTitle = caller.CallerTitle,
                    Assignment = caller.Assignment,
                    OfficePhone = caller.OfficePhone,
                    CallerPager = caller.CallerPager,
                    MobilePhone = caller.MobilePhone,
                    CallerEmail = caller.CallerEmail
                };
                return View(model);
            }
        }

        // GET: Callers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Callers/Create
        [HttpPost]
        public ActionResult Create(Caller CallerModel)
        {
            int CallerId = 0;
            var preferred = CallerModel.PreferredNumberType;
            string preferredNumber = string.Empty;
            try
            {
                tblCaller tcaller = new tblCaller()
                {
                    Active = true,
                    Assignment = CallerModel.Assignment,
                    CallerEmail = CallerModel.CallerEmail,
                    OfficePhone = CallerModel.OfficePhone,
                    CallerFirstName = CallerModel.CallerFirstName,
                    CallerLastName = CallerModel.CallerLastName,
                    CallerPager = CallerModel.CallerPager,
                    MobilePhone = CallerModel.MobilePhone,
                    CallerTitle = CallerModel.CallerTitle
                };
                preferredNumber = GetPreferredNumber(CallerModel, preferred);
                tblCallerPreferredNumber callerPreferredNumber = new tblCallerPreferredNumber()
                {
                    PreferredNumber = preferredNumber,
                    PreferredType = CallerModel.PreferredNumberType
                };
                using (var context = new lifeflightapps())
                {
                    try
                    {
                        context.tblCallers.Add(tcaller);
                        context.SaveChanges();
                        CallerId = tcaller.ID;

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }


                    callerPreferredNumber.CallerId = CallerId;

                    context.tblCallerPreferredNumbers.Add(callerPreferredNumber);
                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetPreferredNumber(Caller CallerModel, string preferred)
        {
            string preferredNumber;
            switch (preferred)
            {
                case "Mobile":
                    preferredNumber = CallerModel.MobilePhone.ToString();
                    break;
                case "Pager":
                    preferredNumber = CallerModel.CallerPager.ToString();
                    break;
                default:
                    preferredNumber = CallerModel.CallerPager.ToString();
                    break;
            }

            return preferredNumber;
        }

        [HttpGet]
        // GET: Callers/Edit/5
        public ActionResult Edit(int id)
        {
            Caller model = new Caller();
            tblCaller dbcaller = new tblCaller();
            tblCallerPreferredNumber preferredNumber = new tblCallerPreferredNumber();
            using (var context =  new lifeflightapps())
            {
                dbcaller = context.tblCallers.Find(id);
                preferredNumber = context.tblCallerPreferredNumbers.Where(x => x.CallerId == id).OrderByDescending(x=>x.CallerPreferredID).FirstOrDefault();
            }
            
            model = new Caller()
            {
                Active = dbcaller.Active.HasValue?dbcaller.Active.Value:false,
                CallerFirstName = dbcaller.CallerFirstName,
                CallerLastName = dbcaller.CallerLastName,
                CallerTitle = dbcaller.CallerTitle,
                Assignment = dbcaller.Assignment,
                OfficePhone = dbcaller.OfficePhone,
                CallerPager = dbcaller.CallerPager,
                MobilePhone = dbcaller.MobilePhone,
                CallerEmail = dbcaller.CallerEmail,
                CallerId = dbcaller.ID
            };
            
            //    if (preferredNumber != null)
            //    {
            //        model.PreferredNumberType = preferredNumber.PreferredType;
            //        TempData["PreferredType"] = preferredNumber.PreferredType;
            //    }

            //    return View(model);
            //}
            model.PreferredNumberTypes = GetPreferredTypes();
            if (preferredNumber != null)
            {
                foreach (var item in model.PreferredNumberTypes)
                {
                    if (item.Value == preferredNumber.PreferredType)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            return View(model);
        }

        private List<SelectListItem> GetPreferredTypes()
        {
            List<SelectListItem> Types = new List<SelectListItem>
            {
                new SelectListItem() {Text="Mobile", Value="Mobile"},
                new SelectListItem() {Text="Pager", Value="Pager"},
            };
            return Types;
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditCaller(Caller callerModel)
        {
            if (ModelState.IsValid)
            {
                tblCaller caller = new tblCaller()
                {
                    Active = callerModel.Active,
                    Assignment = callerModel.Assignment,
                    CallerEmail = callerModel.CallerEmail,
                    MobilePhone = callerModel.MobilePhone,
                    CallerFirstName = callerModel.CallerFirstName,
                    CallerLastName = callerModel.CallerLastName,
                    CallerPager = callerModel.CallerPager,
                    OfficePhone = callerModel.OfficePhone,
                    CallerTitle = callerModel.CallerTitle,
                    ID = callerModel.CallerId
                };
                try
                {
                    using (var context = new lifeflightapps())
                    {
                        context.tblCallers.AddOrUpdate(caller);

                        //db.tblCallers.Attach(caller);
                        //var entry = db.Entry(caller);
                        //entry.State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        var preferredNumber = context.tblCallerPreferredNumbers.Where(x => x.CallerId == callerModel.CallerId).FirstOrDefault();
                        string number = GetPreferredNumber(callerModel, callerModel.PreferredNumberType);
                        if (preferredNumber != null)
                        {
                            preferredNumber.PreferredNumber = number;
                            preferredNumber.PreferredType = callerModel.PreferredNumberType;
                            context.SaveChanges();
                        }
                        else
                        {
                            tblCallerPreferredNumber cpn = new tblCallerPreferredNumber()
                            {
                                CallerId = callerModel.CallerId,
                                PreferredNumber = number,
                                PreferredType = callerModel.PreferredNumberType
                            };

                            context.tblCallerPreferredNumbers.Add(cpn);
                            context.SaveChanges();
                        }
                        return RedirectToAction("Index");
                    }
                }
                catch (DataException dx)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    throw dx;
                }
            }
            return View(callerModel);
        }

        // GET: Callers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Callers/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetFilteredCallers(string term)
        {
            List<string> callers = new List<string>();
            using (var context = new lifeflightapps())
            {
                callers = context.tblCallers.Where(s => s.CallerLastName.StartsWith(term) || s.CallerFirstName.StartsWith(term)).Select(x => x.CallerLastName + " " + x.CallerFirstName).ToList();
            }
            return Json(callers, JsonRequestBehavior.AllowGet);
        }

    }
}
