using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class MstFunctionController : BaseController
    {
        private readonly IMasterFunctionBLL _masterFunctionBll;

        public MstFunctionController(IMasterFunctionBLL masterFunctionBll)
        {
            _masterFunctionBll = masterFunctionBll;
        }
        //
        // GET: /MstFunction/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMasterDataFunction(MasterFunctionInput criteria)
        {
            var masterFunctionBll = _masterFunctionBll.GetMasterFunctions(criteria);
            //var viewModel = Mapper.Map<List<MasterFunctionViewModel>>(_masterFunctionBll);
            return Json(masterFunctionBll, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterDataFunctionType(MasterFunctionInput criteria)
        {
            var masterFunctionBll = _masterFunctionBll.GetMasterFunctionTypes(criteria);
            //var viewModel = Mapper.Map<List<MasterFunctionViewModel>>(_masterFunctionBll);
            return Json(masterFunctionBll, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDropDownType()
        {
            return Json(_masterFunctionBll.GetTypeList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownParentId(string type)
        {
            return Json(_masterFunctionBll.GetMasterFunctionTypeMenu(type), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterFunction(MasterFunctionInput criteria)
        {
            var viewModel = Mapper.Map<List<MasterFunctionViewModel>>(_masterFunctionBll.GetAllMasterFunction());
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterFunctionForDynamic(MasterFunctionInput criteria)
        {
            var viewModel = Mapper.Map<List<MasterFunctionViewModel>>(_masterFunctionBll.GetMasterFunctionForDynamics(criteria));
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public ActionResult InsertList(MasterListInput criteria)
        public ActionResult InsertList(InsertUpdateData<MasterFunctionViewModel> bulkData)
        { 
            try
            {
                MasterFunction item;
                if (bulkData.New != null)
                {
                    var mstFunction = Mapper.Map<MasterFunction>(bulkData.New[0]);
                    mstFunction.CreatedBy = GetUserId();
                    mstFunction.UpdatedBy = GetUserId();
                    item = _masterFunctionBll.SaveData(mstFunction,true);
                }
                else
                {
                    var mstFunction = Mapper.Map<MasterFunction>(bulkData.Edit[0]);
                    mstFunction.UpdatedBy = GetUserId();
                    item = _masterFunctionBll.SaveData(mstFunction,false);
                }
                //bulkData.Edit[0] = Mapper.Map<MasterFunctionViewModel>(item);
                //bulkData.Edit[0].ResponseType = Enums.ResponseType.Success.ToString();
            }
            catch (ExceptionBase ex)
            {
                //bulkData.Edit[0].ResponseType = Enums.ResponseType.Error.ToString();
                //bulkData.Edit[0].Message = ex.Message;
            }
            return Json(bulkData);
        }
    }
}