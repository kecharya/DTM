using DischargeTransportManagement.AppSecurityService;
using DischargeTransportManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class AppUserRoleController : Controller
    {
        private string applicationName = "DTM";

        /// <summary>
        /// Gets all the DTM Users and displays them 10 per page 
        /// and also the ability to search for the users
        /// </summary>
        /// <param name="page"></param>
        /// <param name="SearchUser"></param>
        /// <returns></returns>
        public ActionResult DTMUsers(int? page, string SearchUser)
        {
            List<AppUserRoleModel> appUserRoleModels = new List<AppUserRoleModel>();
            appUserRoleModels = GetAllUsers();

            if (!string.IsNullOrEmpty(SearchUser))
            {
                AppUserRoleModel model = new AppUserRoleModel();
                model = appUserRoleModels.Where(x => x.FirstName + " " + x.LastName == SearchUser).FirstOrDefault();
                return RedirectToAction("EditUser", model) ;
                //return RedirectToAction("EditUser", new { model = model });
            }
            

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(appUserRoleModels.ToPagedList(pageNumber, pageSize));
            
        }
        
        /// <summary>
        /// Populates the AppUserRoleModel
        /// </summary>
        /// <returns></returns>
        private List<AppUserRoleModel> GetAllUsers()
        {
            List<AppUserRoleModel> models = new List<AppUserRoleModel>();
            ApplicationSecurityClient securityClient = new ApplicationSecurityClient();
            List<AppUserRoleDTO> dto = new List<AppUserRoleDTO>();
            using (securityClient)
            {

                dto = securityClient.GetAppUserWithRoles(applicationName).ToList();
                
                models = ConvertToModel(dto);
                
            }
            return models.OrderBy(x=>x.FirstName).ThenBy(x=>x.LastName).ToList();
        }

        /// <summary>
        /// Converts the DTO to Model
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        private List<AppUserRoleModel> ConvertToModel(List<AppUserRoleDTO> dtos)
        {
            List<AppUserRoleModel> models = new List<AppUserRoleModel>();
            foreach (var dto in dtos)
            {
                AppUserRoleModel model = new AppUserRoleModel()
                {
                    ApplicationName = dto.ApplicationName,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    IsUserActive = dto.IsUserActive,
                    VunetId = dto.VunetId,
                    RoleName = dto.RoleName
                };
                models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Adds user to a role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddUserOrRole()
        {
            AppUserRoleModel model = new AppUserRoleModel();
            return View(model);
        }

        /// <summary>
        /// HttpGet for the user EDIT
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditUser(AppUserRoleModel model)
        {
            model.Roles = model.GetRoles().ToList();
            model.Roles.Where(x => x.Text == model.RoleName).First().Selected = true;
            return View(model);
        }

        /// <summary>
        /// HttpPost for the user Edit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("EditUser")]
        public ActionResult ConfirmEditUser(AppUserRoleModel model)
        {
            ApplicationSecurityClient securityClient = new ApplicationSecurityClient();
            AppUserRoleDTO dto = new AppUserRoleDTO()
            {
                ApplicationName = model.ApplicationName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsUserActive = model.IsUserActive,
                RoleName = model.RoleName,
                ShowUser = true,
                VunetId = model.VunetId,
                UpdatedBy = User.Identity.Name,
                UpdatedOn = DateTime.Now
            };
            using (securityClient)
            {
                securityClient.UpdateAppUserRole(dto);
            }
            return RedirectToAction("DTMUsers");
        }

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="RoleName"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public JsonResult AddNewRole(string RoleName, string Description)
        {
            string response = string.Empty;
            ApplicationSecurityClient securityClient = new ApplicationSecurityClient();
            RolesDTO dto = new RolesDTO()
            {
                ApplicationName = applicationName,
                RoleName = RoleName,
                RoleDescription = Description,
                CreatedBy = User.Identity.Name,
                CreatedOn = DateTime.Now,
                IsActive = true,
                Show = true
            };
            using (securityClient)
            {
                response = securityClient.AddRoleToApplication(dto);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="VunetId"></param>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        public JsonResult AddNewUser(string FirstName, string LastName, string VunetId, string RoleName)
        {
            string response = string.Empty;
            ApplicationSecurityClient securityClient = new ApplicationSecurityClient();

            UserDTO dto = new UserDTO()
            {
                ApplicationName = applicationName,
                CreatedBy = User.Identity.Name,
                CreatedOn = DateTime.Now,
                FirstName = FirstName,
                LastName = LastName,
                IsActive = true,
                Show = true,
                VunetId = VunetId
            };

            using (securityClient)
            {
               response =  securityClient.AddUserToApplication(dto);
                if (response.Contains("Success"))
                {
                   response += Environment.NewLine + securityClient.AddUserToAppRole(VunetId, applicationName, RoleName);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Autocomplete for the user search
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonResult GetFilteredUsers(string term)
        {
            List<string> users = new List<string>();
            var userss = GetAllUsers();
            users = userss.Where(x => x.FirstName.ToLower().StartsWith(term.ToLower()) 
                        || x.LastName.ToLower().StartsWith(term.ToLower())).Select(x => x.FirstName + " " + x.LastName).ToList();
            //ApplicationSecurityClient securityClient = new ApplicationSecurityClient();
            //using (securityClient)
            //{
            //    users = securityClient.GetAllUsersInApplication(applicationName).Where(x => x.FirstName.StartsWith(term) || x.LastName.StartsWith(term)).Select(x => x.FirstName + " " + x.LastName).ToList();
            //}
            return Json(users, JsonRequestBehavior.AllowGet);
        }

    }
}