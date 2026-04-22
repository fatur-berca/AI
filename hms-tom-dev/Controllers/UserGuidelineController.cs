using System;
using System.Web.Mvc;
using DFIS.Utils;

namespace hms_tom_dev.Controllers
{
    public class UserGuidelineController : BaseController
    {

        public UserGuidelineController()
        {
            SetPage(EnumHelper.GetDescription(Enums.PageName.UserGuideline));
        }

        // GET: UserGuideline
        public ActionResult Index()
        {
            ViewBag.Title = EnumHelper.GetDescription(Enums.PageName.UserGuideline);
            return View();
        }
    }
}