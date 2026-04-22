using DFIS.Universal.Domain.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace hms_tom_dev.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult UserNotRegistered()
        {
            return View("UserNotRegistered");
        }

        public ActionResult UserNotAuthorized()
        {
            return View("UserNotAuthorized");
        }
	}
}