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
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.DTOs;

namespace hms_tom_dev.Controllers
{
    public class MstTruckSealController : BaseController
    {
        private readonly IMasterTruckSealBLL _masterTruckSealBLL;

        public MstTruckSealController(IMasterTruckSealBLL masterTruckSealBLL)
        {
            _masterTruckSealBLL = masterTruckSealBLL;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllTruckSeals()
        {
            var masterTrucks = _masterTruckSealBLL.GetMasterTruckSeal();
            return Json(masterTrucks, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLastNumber()
        {
            int max = 0;
            try
            {
                var masterTrucks = _masterTruckSealBLL.GetMasterTruckSeal();
                foreach (var mt in masterTrucks)
                    max = int.Parse(mt.SealNumberTo);
            }
            catch { }
            return Json(max, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertTruckSeals(InsertUpdateData<MasterTruckSealViewModel> bulkData)
        {
            bool result = false;

            try
            {
                if (bulkData.New != null)
                {
                    var mstTruckSeal = Mapper.Map<MasterTruckSealDTO>(bulkData.New[0]);

                    mstTruckSeal.CreatedBy = GetUserId();
                    mstTruckSeal.CreatedDate = DateTime.Now;
                    mstTruckSeal.UpdatedBy = GetUserId();
                    mstTruckSeal.UpdatedDate = DateTime.Now;

                    result = _masterTruckSealBLL.SaveData(mstTruckSeal);
                }
            }
            catch (ExceptionBase ex)
            {
                result = false;
            }

            return Json(result);
        }

        public ActionResult DeleteTruckSeal(int keyID)
        {
            bool result = false;

            result = _masterTruckSealBLL.DelData(keyID);
            return Json(result);
        }
    }
}
