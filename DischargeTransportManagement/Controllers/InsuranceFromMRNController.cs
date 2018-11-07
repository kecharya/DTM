using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DTM_Preview.Controllers
{
    public class DTM_PreviewController : Controller
    {
        // GET: DTM_Preview
        public ActionResult Index(string MRN)
        {
            return View();
        }
    }
}