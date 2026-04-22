using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using DFIS.Universal.BusinessLogics;
using hms_tom_dev.Models.Home;

namespace hms_tom_dev.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHomeBLL _homeBll;
        private static string userid = "";

        public HomeController(IHomeBLL homeBll)
        {
            _homeBll = homeBll;
        }

        // GET: Login
        public ActionResult Index(InitHomeViewModel init)
        {
            init.WebRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var unAuthorizedUser = session.Location;
            var _getUserRole = GetListUserRole().FirstOrDefault();
            var userRole = _getUserRole == null ? "" : _getUserRole.RoleName;
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.IsSuperAdmin = session.Role.Where(f => f.RoleName.Equals(EnumHelper.GetDescription(DFIS.Utils.Enums.UserRole.SuperAdmin))).FirstOrDefault() != null;
            ViewBag.IsContracted = session.Email.Contains("contracted") && !ViewBag.IsSuperAdmin && !userRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]);

            if (unAuthorizedUser == null) return View("Index", init);

            var count = unAuthorizedUser.Count;

            if (count == 0)
            {
                Session.Clear();
                init.IsRuleEmpty = true;
                return View("index", init);
            }

            var listNewsData = GetNews();
            var listNews = listNewsData.Any() ? listNewsData : null;

            ViewBag.ListNews = listNews;

            return View("index", init);
        }


        public ActionResult PartialViewAllNewsModal()
        {
            return View("_PartialViewAllNewsModal");
        }

        public ActionResult PartialViewAllNotificationsModal()
        {
            return View("_PartialViewAllNotificationModal");
        }

        public ActionResult GetDisplayUsername()
        {
            return Json(GetUserId(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDisplayFullname()
        {
            return Json(GetUserName(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetURL()
        {
            string geturl = _homeBll.GetUrlBestPracticeToolbox();
            return Json(geturl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadFile(string title)
        {
            string lokasiFile = "";
            string lokasiImage = "";
            try
            {
                var strFile = "./Assets/Uploads/NewsHighlight/File/";
                var strImg = "~/Assets/Uploads/NewsHighlight/Image/";
                var pathFile = Server.MapPath(strFile);
                var pathImage = Server.MapPath(strImg);
                if (!Directory.Exists(pathFile))
                {
                    Directory.CreateDirectory(pathFile);
                }

                if (!Directory.Exists(pathImage))
                {
                    Directory.CreateDirectory(pathImage);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    //Use the following properties to get file's name, size and MIMEType
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    string fileNameSave = DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + i.ToString();
                    System.IO.Stream fileContent = file.InputStream;
                    //To save file, use SaveAs method
                    if (fileName.Contains("\\"))
                    {
                        fileName = fileName.Split('\\')[fileName.Split('\\').Length - 1];
                    }
                    string extension = Path.GetExtension(fileName);

                    if (extension == ".pdf")
                    {
                        lokasiFile = strFile + fileNameSave + extension;
                        file.SaveAs(pathFile + fileNameSave + extension); //File will be saved in application root
                    }
                    else if (extension == ".png" || extension == ".jpg")
                    {
                        lokasiImage = strImg + fileNameSave + extension;
                        file.SaveAs(pathImage + fileNameSave + extension); //File will be saved in application root
                    }
                }

                _homeBll.SaveData(title, lokasiFile, lokasiImage, GetUserId());
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(Enums.ResponseType.Error.ToString());
            }
            return Json(Enums.ResponseType.Success.ToString());
        }

        public ActionResult ViewAllNotification()
        {
            return Json(_homeBll.ViewALLNotificationByUserId(GetUserId()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadNotification(List<int> idnotif, bool reload)
        {
            //userid = GetUserId();
            return Json(_homeBll.GetNotificationByUserId(idnotif, reload, GetUserId()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateNotificationIsOpen(List<int> idnotif)
        {
            _homeBll.UpdateNotification(idnotif, true, GetUserId());
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateNotificationIsRead(List<int> idnotif)
        {
            _homeBll.UpdateNotification(idnotif, false, GetUserId());
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}