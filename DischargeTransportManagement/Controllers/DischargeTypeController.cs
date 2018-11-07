using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class DischargeTypeController : Controller
    {
        // GET: DischargeType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DischargeRequest(tblCaller caller, CovAndIns coverages)
        {
            TransportRequestViewModel transRequest = new TransportRequestViewModel();

            return View(transRequest);
        }

        [HttpPost]
        public ActionResult DischargeRequest(tblCaller caller, CovAndIns coverages, FormCollection form)
        {
            string state = form["States"].ToString();
            string ems = form["EmsAgencies"].ToString();
            string tansportMode = form["TransportModes"].ToString();
            TransportRequestViewModel transRequest = new TransportRequestViewModel(state, ems, tansportMode);
            ViewData["CallerInfo"] = caller;
            ViewData["CovInfo"] = coverages;
            ViewData["TransInfo"] = transRequest;

            return View(transRequest);
        }
    }
}