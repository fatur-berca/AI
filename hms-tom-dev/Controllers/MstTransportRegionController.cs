using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Controllers
{
    public class MstTransportRegionController : BaseController
    {
        private readonly IMasterTransportRegionBLL _masterTransportRegionBLL;
        //
        // GET: /MstTransportRegion/
        public MstTransportRegionController(IMasterTransportRegionBLL masterTransportRegionBLL)
        {
            _masterTransportRegionBLL = masterTransportRegionBLL;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataById(MasterTransportRegionInput criteria)
        {
            var masterTransportRegions = _masterTransportRegionBLL.GetById(criteria.IDTransportRegion);
            var viewModel = Mapper.Map<MasterTransportRegionViewModel>(masterTransportRegions);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterTransportRegion(MasterTransportRegionInput criteria)
        {
            var masterTransportRegions = _masterTransportRegionBLL.GetMasterTransportRegions(criteria);
            var viewModel = Mapper.Map<List<MasterTransportRegionViewModel>>(masterTransportRegions);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //public ActionResult InsertList(MasterListInput criteria)
        public ActionResult InsertTransportRegion(InsertUpdateData<MasterTransportRegionViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstTransportRegion = Mapper.Map<MasterTransportRegionDTO>(bulkData.New[i]);

                    //set createdby and updatedby

                    mstTransportRegion.CreatedBy = GetUserId();
                    mstTransportRegion.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterTransportRegionBLL.SaveData(mstTransportRegion);
                        bulkData.New[i] = Mapper.Map<MasterTransportRegionViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                }
            }
            else
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    if (bulkData.Edit[i] == null) continue;
                    var mstTransportRegion = Mapper.Map<MasterTransportRegionDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstTransportRegion.CreatedBy = GetUserId();
                    mstTransportRegion.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterTransportRegionBLL.EditData(mstTransportRegion);
                        bulkData.Edit[i] = Mapper.Map<MasterTransportRegionViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                }
            }

            return Json(bulkData);
          
        }
	}
}