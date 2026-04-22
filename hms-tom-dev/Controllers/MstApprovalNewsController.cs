using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.BusinessLogics;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.BusinessLogics;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Controllers
{
    public class MstApprovalNewsController : BaseController
    {
        private readonly IMasterApprovalNewsBLL _masterApprovalNewsBll;

        public MstApprovalNewsController(IMasterApprovalNewsBLL masterApprovalNewsBll)
        {
            _masterApprovalNewsBll = masterApprovalNewsBll;
        }
        //
        // GET: /MstFunction/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMasterApprovalNews(int status)
        {
            var viewModel = Mapper.Map<List<MasterApprovalNewsViewModel>>(_masterApprovalNewsBll.GetAllMasterApprovalNewsByStatus(status));
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertList(int id, int status)
        {
            try
            {
                _masterApprovalNewsBll.SaveData(id,status,GetUserId());
            }
            catch (ExceptionBase ex)
            {
                return Json(Enums.ResponseType.Error.ToString());
            }
            return Json(Enums.ResponseType.Success.ToString());
        }

        [HttpPost]
        public ActionResult UpdateSeen(MasterApprovalNewsInput input)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();

            var inputData = Mapper.Map<MasterApprovalNewsDTO>(input);

            try
            {
                _masterApprovalNewsBll.UpdateSeen(inputData, controller, GetUserId());

                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
