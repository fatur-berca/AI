using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;
using hms_tom_dev.Code;
using hms_tom_dev.Models.Account;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class LoginController : BaseController
    {
        private IMasterUserBLL _mstUserBLL;
        private IUtilitiesBLL _utilBLL;
        private IFormsAuthentication _formsAuthentication;
        private bool _debug;

        public LoginController(IMasterUserBLL mstUserBll, IUtilitiesBLL utilBLL,IFormsAuthentication formsAuthentication)
        {
            _mstUserBLL = mstUserBll;
            _utilBLL = utilBLL;
            _formsAuthentication = formsAuthentication;
            _debug = Convert.ToBoolean(WebConfigurationManager.AppSettings["debug"]);
        }
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserModel model)
        {
            //MasterUser user = _mstUserBLL.GetLogin(model.UserAD);

            //if (user != null)
            //{
            //    UserSession login = new UserSession();
            //    login.Name = user.IDUser.ToUpper();
            //    login.Username = user.FullName;
            //    List<MasterUserLocationMapping> listLocationMap = user.MasterUserLocationMappings.ToList();
            //    var listRole = user.MasterUserRoleMappings.Select(x => x.IDRole).ToList(); //_utilBLL.GetListRole(user.IDUser);
            //    if (listRole.Any())
            //    {
            //        var res = _utilBLL.GetResponsibilityPage(login.Name, listLocationMap);
            //        login.Page = res.Page;
            //        login.Location = res.Location;
            //        login.Button = res.Button;
            //        login.Role = res.Role;
            //    }
            //    else
            //    {
            //        login.Page = null;
            //        login.Location = null;
            //    }
            //    Session["CurrentUser"] = login;
            //    _formsAuthentication.SignIn(model.UserAD, true);
            //    return RedirectToAction("Index", "Home");
            //}

            //ViewBag.Error = "Username incorrect";
            //return PartialView("Index");

            MasterUser user = _mstUserBLL.GetLogin(model.UserAD);

           /* if (user != null)
            {
                UserSession login = new UserSession();
                login.Name = user.IDUser.ToLower();
                login.Username = user.FullName;
                var res = _utilBLL.GetResponsibilityPage(login.Name);
                login.Page = res.Page;
                login.Role = res.Role;
                login.Location = res.Location;
                login.Button = res.Button;
                //var pageId = res.Page.Where(x => x.FunctionName == Page).Select(x => x.IDFunction).FirstOrDefault();
                //var button = _utilBLL.GetResponsibilityButton(login.Name, pageId);
                //ViewBag.ButtonAccess = button;
                //ViewBag.PageAccess = login.Page.Select(x => x.FunctionName).ToList();
                Session["CurrentUser"] = login;
                _formsAuthentication.SignIn(model.UserAD, true);
                if (user.IsActive == true)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("UserNotRegistered", "Error");
                }
            }*/

            ViewBag.Error = "Username incorrect";
            return PartialView("Index");
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.Clear();

            if (_debug)
            {
                _formsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
	}
}