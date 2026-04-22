using AutoMapper;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.Repositories;

namespace hms_tom_dev.Controllers
{
    public class MstServicePoTomController : BaseController
    {
        private readonly IMasterServicePoTomBLL _masterServicePoTomBLL;
        private readonly IMasterVendorTOMBLL _masterVendorTOMBll;
        private readonly IMasterVendorTOMRepo _masterVendorTOMRepo;

        public MstServicePoTomController(IMasterServicePoTomBLL masterServicePoTomBLL,IMasterVendorTOMBLL masterVendorTOMBll, IMasterVendorTOMRepo masterVendorTOMRepo)
        {
            _masterServicePoTomBLL = masterServicePoTomBLL;
            _masterVendorTOMBll = masterVendorTOMBll;
            _masterVendorTOMRepo = masterVendorTOMRepo;
        }
        // GET: MstServicePoTom
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMasterServicePoTom(MasterServicePoTomInput criteria)
        {
            var dbResult = _masterServicePoTomBLL.GetData(criteria);
            var viewModel = Mapper.Map<List<MasterServicePoTomViewModel>>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);            
        }

        public ActionResult GetMasterVendorTOM()
        {
            var viewModel = Mapper.Map<List<MasterVendorTOMViewModel>>(_masterVendorTOMBll.GetALLMasterVendorsNoChild());
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<MasterServicePoTomViewModel> bulkData)
        {
            int result = 0;
            try
            {
                if (bulkData.New != null)
                {
                    Debug.WriteLine("Insert");
                    var mstService = Mapper.Map<MasterServicePoTomDTO>(bulkData.New[0]);
                    mstService.CreatedBy = GetUserId();
                    mstService.UpdatedBy = GetUserId();
                    mstService.CreatedDate = DateTime.Now;
                    //result = _masterServicePoTomBLL.SaveData(mstService);
                    _masterServicePoTomBLL.InsertOrUpdate(mstService);
                }
                else
                {
                    Debug.WriteLine("Update");
                    var mstService = Mapper.Map<MasterServicePoTomDTO>(bulkData.Edit[0]);
                    mstService.CreatedBy = GetUserId();
                    mstService.UpdatedBy = GetUserId();
                    //result = _masterServicePoTomBLL.EditData(mstService);
                    _masterServicePoTomBLL.InsertOrUpdate(mstService);
                }
            }
            catch (Exception ex)
            {
                // return Json(3);
                return Json(new NonQueryResult(false, ex));
            }
            // return Json(true);
            return Json(new NonQueryResult(true));            
        }
        public ActionResult Delete(int keyID)
        {
            try
            {
                _masterServicePoTomBLL.DelData(keyID);
                return Json(new NonQueryResult(true));
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex.Message));
            }
        }
    }
}