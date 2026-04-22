using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.Outputs;
using System.Linq;
using System.Diagnostics;
using DFIS.Universal.Domain.Inputs;
using System;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.Inputs;
using TOM.Master.Domain.DTOs;

namespace hms_tom_dev.Controllers
{
    public class MstCostCenterAccountController : BaseController
    {
        private readonly IMasterCostCenterAccountBLL _masterCostCenterAccountBll;
        private readonly IMasterDistanceBLL _masterDistanceBLL;
        private readonly IMasterLocationBLL _masterLocationBLL;

        public MstCostCenterAccountController(IMasterLocationBLL masterLocationBLL, IMasterCostCenterAccountBLL masterCostCenterAccountBll, IMasterDistanceBLL masterDistanceBLL)
        {
            _masterCostCenterAccountBll = masterCostCenterAccountBll;
            _masterDistanceBLL = masterDistanceBLL;
            _masterLocationBLL = masterLocationBLL;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDropDown()
        {
            return Json(_masterCostCenterAccountBll.GetList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserWarehouse()
        {
            var getdata = _masterLocationBLL.GetByConfig("TOMLocationFilter");
            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterCostCenterAccount(MasterCostCenterAccountInput criteria)
        {
            var dataTable = _masterCostCenterAccountBll.GetALLMasterCostCenterAccount(criteria).ToArray();
            return Json(dataTable, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertCostCenterAccount(InsertUpdateData<MasterCostCenterAccountViewModel> bulkData)
        {

            try
            {
                if (bulkData.New != null)
                {
                    var mstDistance = Mapper.Map<MasterCostCenterAccountDTO>(bulkData.New[0]);
                    mstDistance.CreatedBy = GetUserId();
                    mstDistance.CreatedDate = DateTime.Today;
                    mstDistance.UpdatedBy = GetUserId();
                    mstDistance.Sender = bulkData.New[0].Sender;
                    mstDistance.Receiver = bulkData.New[0].Receiver;
                    mstDistance.Description = bulkData.New[0].Description == null ? "" : bulkData.New[0].Description; 
                    _masterCostCenterAccountBll.InsertOrUpdate(mstDistance);
                }
                else
                {
                    var mstDistance = Mapper.Map<MasterCostCenterAccountDTO>(bulkData.Edit[0]);
                    mstDistance.UpdatedBy = GetUserId();
                    mstDistance.Sender = bulkData.Edit[0].Sender;
                    mstDistance.Receiver = bulkData.Edit[0].Receiver;
                    mstDistance.Description = bulkData.Edit[0].Description == null ? "" : bulkData.Edit[0].Description;
                    _masterCostCenterAccountBll.InsertOrUpdate(mstDistance);
                }
            }
            catch (Exception ex)
            {
                return Json(
                    new NonQueryResult(false, ex.Message)
                );
            }
            return Json(
                new NonQueryResult(true)
                );
            #region old code
            return Json(null);
            int result = 0;
            try
            {                
                if (bulkData.New != null)
                {
                    var mstCostCenterAccount = Mapper.Map<MasterCostCenterAccountDTO>(bulkData.New[0]);
                    mstCostCenterAccount.CreatedBy = GetUserId();
                    mstCostCenterAccount.UpdatedBy = GetUserId();
                    result = _masterCostCenterAccountBll.SaveData(mstCostCenterAccount, true);
                    //Debug.WriteLine("Insert");
                }
                else
                {
                    var mstCostCenterAccount = Mapper.Map<MasterCostCenterAccountDTO>(bulkData.Edit[0]);
                    mstCostCenterAccount.UpdatedBy = GetUserId();
                    result = _masterCostCenterAccountBll.SaveData(mstCostCenterAccount, false);
                    //Debug.WriteLine("Update");
                }
            }
            catch (ExceptionBase ex)
            {
                result = 3;
            }
            return Json(result);
            #endregion
        }
        public ActionResult Delete(int keyID)
        {
            return Json(_masterCostCenterAccountBll.DelData(keyID));
        }
    }
}