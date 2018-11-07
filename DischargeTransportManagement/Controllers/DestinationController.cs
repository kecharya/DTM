using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class DestinationController : Controller
    {
        // GET: Destination
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DestinationList(int? page, string SearchDestination)
        {
            List<Destination> destinations = new List<Destination>();
            //DestinationViewModel viewModel = new DestinationViewModel();
            //destinations = viewModel.RetrieveDestinations();
            using (var context = new lifeflightapps())
            {
                if (!string.IsNullOrWhiteSpace(SearchDestination))
                {
                    string[] splitTaskName = SearchDestination.Split(',');
                    string destinationName = splitTaskName[0].ToString();
                    string statecode = splitTaskName[2].ToString();
                    //Union_Facilities facilities = new Union_Facilities();

                    var facilities = context.Union_Facilities.Where(d => d.NAME == destinationName && d.STATE == statecode).FirstOrDefault();

                    if (facilities != null)
                    {
                        return RedirectToAction("GetDestinationDetails", new { DestinationId = facilities.id2 });
                    }

                    Destination destination = ConvertToDestination(facilities);
                    destinations.Add(destination);

                }
                else
                {
                    var des = context.Union_Facilities.OrderBy(x => x.NAME).ToList();
                    foreach (var item in des)
                    {
                        Destination destination = ConvertToDestination(item);
                        destinations.Add(destination);
                    }
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(destinations.OrderBy(x => x.DestinationName).ToPagedList(pageNumber, pageSize));
            //return View(destinations);
        }

        private static Destination ConvertToDestination(Union_Facilities facilities)
        {
            return new Destination()
            {
                DestinationId = facilities.id2,
                DestinationName = facilities.NAME,
                Address = facilities.ADDRESS,
                City = facilities.CITY,
                County = facilities.COUNTY,
                State = facilities.STATE,
                Zip = facilities.ZIP,
                Phone = facilities.PHONE
            };
        }

        public ActionResult GetDestinationDetails(int DestinationId)
        {
            Destination destination = new Destination();

            using (var context = new lifeflightapps())
            {
                Union_Facilities facilities = new Union_Facilities();

                facilities = context.Union_Facilities.Find(DestinationId);

                destination = ConvertToDestination(facilities);

            }
            return View(destination);
        }

        //[HttpPost]
        //public ActionResult DestinationList(string SearchDestination)
        //{
        //    List<Destination> destinations = new List<Destination>();
        //    DestinationViewModel viewModel = new DestinationViewModel();


        //    if (string.IsNullOrWhiteSpace(SearchDestination))
        //    {
        //        destinations = viewModel.RetrieveDestinations();
        //    }
        //    else
        //    {
        //        //destinations = viewModel.retrieveDestinations();
        //        try
        //        {
        //            string[] splitTaskName = SearchDestination.Split(',');
        //            string destinationName = splitTaskName[0].ToString();
        //            string statecode = splitTaskName[2].ToString();
        //            Union_Facilities facilities = new Union_Facilities();
        //            using (var context = new lifeflightapps())
        //            {
        //                facilities = context.Union_Facilities.Where(d => d.NAME == destinationName && d.STATE == statecode).FirstOrDefault();
        //                Destination destination = new Destination()
        //                {
        //                    DestinationName = facilities.NAME,
        //                    Address = facilities.ADDRESS,
        //                    City = facilities.CITY,
        //                    County = facilities.COUNTY,
        //                    State = facilities.STATE,
        //                    Zip = facilities.ZIP,
        //                    Phone = facilities.PHONE
        //                };
        //                destinations.Add(destination);
        //            }
        //            //destinations = viewModel.RetrieveDesiredDestinations(SearchDestination);
        //        }
        //        catch (Exception ex)
        //        {

        //            throw ex;
        //        }

        //    }

        //    return View(destinations);
        //}


        public ActionResult AddDestination(Destination destination)
        {
            if (string.IsNullOrEmpty(destination.DestinationName))
            {
                Destination model = new Destination();
                RequestModel requestModel = new RequestModel();
                model.StatesList = requestModel.GetStatesList().ToList();
                return View(model);
            }
            else
            {
                using (var context = new lifeflightapps())
                {
                    tblNursingHome nursingHome = new tblNursingHome()
                    {
                        Name = destination.DestinationName,
                        Address = destination.Address,
                        City = destination.City,
                        County = destination.County,
                        State = destination.State,
                        Zip = Convert.ToDouble(destination.Zip),
                        Phone = destination.Phone
                    };

                    context.tblNursingHomes.Add(nursingHome);
                    context.SaveChanges();
                    int id = nursingHome.NursingHomeID;
                    return RedirectToAction("GetDestinationDetails", new { DestinationId = id });
                }
            }

            
        }

        public JsonResult GetFilteredDestinations(string term)
        {
            List<Destination> destinations = new List<Destination>();
            DestinationViewModel viewModel = new DestinationViewModel();
            List<string> DestinationNames;
            DestinationNames = viewModel.RetrieveDesiredDestinations(term).Select(d => d.DestinationName).ToList();

            return Json(DestinationNames, JsonRequestBehavior.AllowGet);

        }

    }
}