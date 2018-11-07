using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class SubmittedRequestController : Controller
    {
        lifeflightapps db = new lifeflightapps();
        // GET: SubmittedRequest
        public ActionResult Index()
        {
            List<SubmittedRequestModel> submittedrequests = new List<SubmittedRequestModel>();
            //get the requests that were placed in the past 24 hours
            List<DischargeRequestModel> drm = new List<DischargeRequestModel>();
            List<tblDischargeRequest1> tdr = new List<tblDischargeRequest1>();
            tdr = db.tblDischargeRequests1.Where(d => d.CallReceivedDate >= DateTime.Now.AddHours(-24)).ToList();

            return View(tdr);
        }
    }
}