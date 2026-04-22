using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.BusinessLogics.UtilitiesBLL;

namespace hms_tom_dev.Code
{
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentException("httpContext");
            }

            MasterUserBLL bll = DependencyResolver.Current.GetService<MasterUserBLL>();
            MasterUser login = bll.GetLogin(httpContext.User.Identity.Name);
            if (login == null)
            {
                return false;
            }

            CustomPrincipal principal = new CustomPrincipal(httpContext.User.Identity.Name);
            principal.Username = login.IDUser;
            HttpContext.Current.User = principal;
            //var username = httpContext.User.Identity.Name;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Error", action = "UserNotAuthorized" }));
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "UserNotRegistered" }));
            }
        }
    }
}