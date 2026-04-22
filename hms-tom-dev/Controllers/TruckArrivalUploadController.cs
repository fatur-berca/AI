using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using TOM.Transport.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class TruckArrivalUploadController : Controller
    {
        private readonly ITransportTruckArrivalBLL _transportTruckArrivalBll;

        public TruckArrivalUploadController(ITransportTruckArrivalBLL transportTruckArrivalBll)
        {
            _transportTruckArrivalBll = transportTruckArrivalBll;
        }

        // GET: TruckArrivalUpload
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadTruckArrival(string locationFolder)
        {
            string[] fileEntries = Directory.GetFiles(locationFolder);
            string destinationFolder = ConfigurationManager.AppSettings["TruckArrivalTemp"];
            _transportTruckArrivalBll.DeleteTruckArrivalTemp();
            foreach (string fileName in fileEntries)
            {
                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension.ToLower() == ".xls")
                    _transportTruckArrivalBll.SaveUploadXLS(fileName);
                if (fileExtension.ToLower() == ".xlsx")
                    _transportTruckArrivalBll.SaveUpload(fileName);
                _transportTruckArrivalBll.MoveFile(fileName, destinationFolder);
            }
            return Json("",JsonRequestBehavior.AllowGet);
        }
    }
}