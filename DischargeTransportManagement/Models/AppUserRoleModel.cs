using DischargeTransportManagement.AppSecurityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class AppUserRoleModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public bool IsApplicationActive { get; set; }
        public int UserId { get; set; }
        public string VunetId { get; set; }
        public bool IsUserActive { get; set; }
        public bool ShowUser { get; set; }
        public string UserCreatedBy { get; set; }
        public DateTime UserCreatedOnDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsRoleActive { get; set; }
        public bool ShowRole { get; set; }
        public string RoleCreatedBy { get; set; }
        public DateTime RoleCreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        //public List<string> Roles { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public IEnumerable<SelectListItem> GetRoles()
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            ApplicationSecurityClient securityClient = new ApplicationSecurityClient();
            using (securityClient)
            {
                var rolesList = securityClient.GetAllRolesInApp("DTM").ToList();
                
                foreach (var role in rolesList)
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = role,
                        Value = role
                    };

                    roles.Add(item);

                    //model.Roles.Add(item);
                }

                //model.Roles = roles;
            }
            return roles;
        }

    }
}