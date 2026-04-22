using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using TOM.Transport.BusinessLogics;
using TOM.Master.Domain.Inputs;
using TOM.Transport.Domain.Inputs;

namespace hms_tom_dev.Controllers
{
    public class TruckArrivalController : BaseController
    {
        private readonly ITransportTruckArrivalBLL _transportTruckArrivalBll;

        public TruckArrivalController(ITransportTruckArrivalBLL transportTruckArrivalBll)
        {
            _transportTruckArrivalBll = transportTruckArrivalBll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportTruckArrival));
        }

        // GET: TruckArrival
        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            return View();
        }

        public ActionResult GetETACategory()
        {
            return Json(_transportTruckArrivalBll.GetAllETACategory(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetALLTransportTruckArrival(TransportTruckArrivalInput filter)
        {
            return Json(_transportTruckArrivalBll.GetAllTransportTruckArrival(filter));
        }

        public ActionResult GetMaxSpeed()
        {
            return Json(_transportTruckArrivalBll.GetMaxSpeed(),JsonRequestBehavior.AllowGet);
        }

        public ActionResult Calculate(string nopol, DateTime eta, string etaCategory)
        {
            TransportTruckArrival save = new TransportTruckArrival();
            save.PoliceNumber = nopol;
            save.ETACalculate = eta;
            save.ETACategoryCalculate = etaCategory;
            _transportTruckArrivalBll.UpdateCalculate(save,GetUserId());
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ExportReport(string nopol, string gpno, string etacat)
        {
            TransportTruckArrivalInput filter = new TransportTruckArrivalInput();
            filter.PoliceNumber = nopol;
            filter.GPNo = gpno;
            filter.ETACategory = etacat;
            HttpResponse response = System.Web.HttpContext.Current.Response;
            HttpResponse resp = System.Web.HttpContext.Current.Response;
            var fileName = "TruckArrival" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            resp.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            resp.AppendHeader("content-disposition", "attachment; filename=" + fileName);
            response.BinaryWrite(_transportTruckArrivalBll.ReportExport(filter).ToArray());
            response.Flush();
            response.End();
        }
    }
}