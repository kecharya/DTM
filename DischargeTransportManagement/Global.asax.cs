using DischargeTransportManagement.AppSecurityService;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DischargeTransportManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    if (HttpContext.Current.Request.IsAuthenticated)
        //    {
        //        List<string> roles = new List<string>();
        //        var identity = HttpContext.Current.User.Identity;

        //        //set up domain context
        //        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "Vanderbilt");

        //        //find the user
        //        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, identity.Name);

        //        //find the AD groups
        //        GroupPrincipal adminGroup = GroupPrincipal.FindByIdentity(ctx, "DTM Admin Access");
        //        GroupPrincipal userGroup = GroupPrincipal.FindByIdentity(ctx, "DTM User Access");
        //        GroupPrincipal devGroup = GroupPrincipal.FindByIdentity(ctx, "TVPG Members");

        //        if (user != null)
        //        {
        //            //check if user is a member of AD group
        //            if (user.IsMemberOf(adminGroup))
        //            {
        //                roles.Add("admin");
        //            }
        //            else if (user.IsMemberOf(userGroup))
        //            {
        //                roles.Add("user");
        //            }
        //            else if (user.IsMemberOf(devGroup))
        //            {
        //                roles.Add("dev");
        //            }
        //        }
        //        HttpContext.Current.User = new GenericPrincipal(identity, roles.ToArray());
        //    }
        //}

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.AuthenticationType == "Forms" && HttpContext.Current.Request.IsAuthenticated)
                {

                    var identity = HttpContext.Current.User.Identity;
                    PrincipalContext context = new PrincipalContext(ContextType.Domain);
                    UserPrincipal user = UserPrincipal.FindByIdentity(context, identity.Name);

                    #region ApplicaionSecurityService Call
                    ////Call the ApplicationSecurity Service for roles 
                    ApplicationSecurityClient securityClient = new ApplicationSecurityClient();
                    string[] userRoles;


                    using (securityClient)
                    {
                        try
                        {
                            userRoles = securityClient.GetUserRolesInApp("DTM", identity.Name);

                            HttpContext.Current.User = new GenericPrincipal(identity, userRoles);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    #endregion
                }
            }

        }
    }
}

