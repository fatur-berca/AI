using System.Collections.Generic;
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

namespace hms_tom_dev.Controllers
{
    public class MstUserLocationMapController : BaseController
    {
        private readonly IMasterUserLocationMappingBLL _masterUserLocationMappingBLL;
        private readonly IMasterLocationBLL _masterLocationBLL;

        public MstUserLocationMapController(IMasterUserLocationMappingBLL masterUserLocationMappingBLL,
            IMasterLocationBLL masterLocationBLL)
        {
            _masterUserLocationMappingBLL = masterUserLocationMappingBLL;
            _masterLocationBLL = masterLocationBLL;
        }

        //
        // GET: /MstUserLocationMap/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataById(MasterUserLocationMappingInput criteria)
        {
            var masterUserLocationMappings = _masterUserLocationMappingBLL.GetById(criteria.IDUser);
            var viewModel = Mapper.Map<MasterUserLocationMappingViewModel>(masterUserLocationMappings);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterUserLocationMapping(MasterUserLocationMappingInput criteria)
        {
            var masterUserLocationMappings = _masterUserLocationMappingBLL.GetDistinctLocationMasterUserLocation();
            var viewModel = Mapper.Map<List<MasterUserLocationMappingViewModel>>(masterUserLocationMappings);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserByLocations(List<string> IDLocations)
        {
            var masterUserLocationMappings = _masterUserLocationMappingBLL.GetMasterUserByLocations(IDLocations);
            var viewModel = Mapper.Map<List<MasterUserLocationMappingViewModel>>(masterUserLocationMappings);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTreeListLocation(MasterLocationInput criteria)
        {
            return Json(_masterUserLocationMappingBLL.GetMasterUserLocationMapTreeList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterUser(MasterUserInput criteria)
        {
            var masterUsers = _masterUserLocationMappingBLL.GetMasterUserLists(criteria);
            var viewModel = Mapper.Map<List<MasteUserViewModel>>(masterUsers);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWarehouseNameById(string id)
        {
            var masterUsers = _masterUserLocationMappingBLL.GetWarehouseNameByIdUser(id);
            //var viewModel = Mapper.Map<List<MasteUserViewModel>>(masterUsers);
            return Json(masterUsers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterFunctionByIdUser(string id)
        {
            var masterUsers = _masterUserLocationMappingBLL.GetMasterFunctionByIdUser(id);
            return Json(masterUsers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIDLocationList(string roleid)
        {
            return Json(_masterUserLocationMappingBLL.GetListIDLocationByIdUser(roleid), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateIsActive(string iduser, bool status)
        {
            _masterUserLocationMappingBLL.UpdateIsActiveByIDUser(iduser, status);
            return Json(new MasterRoleFunctionViewModel());
        }

        [HttpPost]
        public ActionResult InsertUserLocationMapping(InsertUpdateData<MasterUserLocationMappingViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstUserLocationMapping = Mapper.Map<MasterUserLocationMappingDTO>(bulkData.New[i]);
                    mstUserLocationMapping.CreatedBy = GetUserId();
                    mstUserLocationMapping.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterUserLocationMappingBLL.SaveData(mstUserLocationMapping);
                        bulkData.New[i] = Mapper.Map<MasterUserLocationMappingViewModel>(item);
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
                    var mstUserLocationMapping = Mapper.Map<MasterUserLocationMappingDTO>(bulkData.Edit[i]);
                    mstUserLocationMapping.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterUserLocationMappingBLL.EditData(mstUserLocationMapping);
                        bulkData.Edit[i] = Mapper.Map<MasterUserLocationMappingViewModel>(item);
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