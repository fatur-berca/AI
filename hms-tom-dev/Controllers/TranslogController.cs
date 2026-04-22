//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using DFIS.Universal.BusinessLogics.TransactionLogBLL;
//using DFIS.Universal.Domain.DTOs;
//using DFIS.Universal.Domain.Inputs;
//using DFIS.Universal.Repositories.TransactionLogRepo;
//using AutoMapper;
//using hms_dfis_dev.Models.Common;
//using hms_dfis_dev.Models.Universal;
//using DFIS.Utils;
//using DFIS.Master.Domain.Outputs;

//namespace hms_dfis_dev.Controllers
//{
//    public class TranslogController : BaseController
//    {
//        private readonly TransactionLogBLL _transactionLogBLL;

//        public TranslogController(TransactionLogBLL transactionLogBLL)
//        {
//            _transactionLogBLL = transactionLogBLL;
//        }

//        // GET: Translog
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult GetLog()
//        {
//            //string ctr = Request.Params[0].ToString();
//            string ctr = "MstList";

//            var logList = _transactionLogBLL.GetLog(new TransactionLogInput { ctr = ctr });
//            var viewModel = Mapper.Map<List<TransactionLogViewModel>>(logList);
//            return Json(viewModel, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetNotification()
//        {
//            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
//            var notificationList = _transactionLogBLL.getNotification(session.Name);
//            var viewModel = Mapper.Map<List<NotificationViewModel>>(notificationList);
//            return Json(viewModel, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult SetNotificationRead(){
//            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
//            _transactionLogBLL.setNotificationRead(session.Name);
//            return Json("Succes", JsonRequestBehavior.AllowGet);
//        }
//    }
//}