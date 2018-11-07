using DischargeTransportManagement.AppSecurityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VUMC.Authentication;

namespace DischargeTransportManagement.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(FormCollection collection)
        {
            try
            {


                ActiveDirectoryClient client = new ActiveDirectoryClient();

                string vunetId = collection["Username"];
                string ePass = collection["Password"];
                string name = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                bool isValidUser = client.ValidateVUnetIDePassword(vunetId, ePass);
                if (isValidUser)
                {
                    ApplicationSecurityClient securityClient = new ApplicationSecurityClient();
                    List<UserDTO> users = new List<UserDTO>();
                    using (securityClient)
                    {
                        users = securityClient.GetAllUsersInApplication("DTM").ToList();

                        if (!string.IsNullOrEmpty(users.Where(x => x.IsActive == true && x.VunetId == vunetId).Select(x => x.VunetId).FirstOrDefault()))
                        {
                            System.Web.Security.FormsAuthentication.SetAuthCookie(vunetId, true);
                            HttpContext.Session.Timeout = 90;
                            return RedirectToAction("ShowExistingRequests", "ExistingRequests");
                        }
                        else
                        {
                            return View("Unauthorized");
                        }
                    }
                }
                else
                {
                    return View("Unauthorized");
                }
            }
            catch (Exception)
            {

                return RedirectToAction("ErrorLanding", "Login");
            }
        }

        public ActionResult SingOut()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("LoginPage", "Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Unauthorized()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ErrorLanding()
        {
            return View();
        }
    }
}