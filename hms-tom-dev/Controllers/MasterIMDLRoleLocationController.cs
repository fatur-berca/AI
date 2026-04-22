using AutoMapper;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class MasterIMDLRoleLocationController : BaseController
    {
        private readonly IMasterIMDLRoleLocationBLL _masterIMDLRoleLocationBLL;
        // GET: MasterIMDLRole

        public MasterIMDLRoleLocationController(IMasterIMDLRoleLocationBLL masterIMDLRoleLocationBLL)
        {
            _masterIMDLRoleLocationBLL = masterIMDLRoleLocationBLL;
        }
        public ActionResult Index()
        {
            //var ListGetLocation = _masterIMDLRoleLocationBLL.GetDataLocation();
            var ListGetIMDLRole = _masterIMDLRoleLocationBLL.GetDataIMDLRole();
            //ViewBag.ListGetLocation = new SelectList(ListGetLocation, "IDLocation", "LocationName");
            ViewBag.ListGetIMDLRole = new SelectList(ListGetIMDLRole, "IMDLRole", "IMDLRole");
            return View();
        }

        public ActionResult GetMasterIMDLRoleLocation(MasterIMDLRoleLocationInput criteria)
        {
            var masterIMDLRoleLocations = _masterIMDLRoleLocationBLL.GetDistinctLocationMasterUserLocation();
            var viewModel = Mapper.Map<List<MasterIMDLRoleLocationViewModel>>(masterIMDLRoleLocations);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTreeListLocation(MasterLocationInput criteria)
        {
            return Json(_masterIMDLRoleLocationBLL.GetTreeList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIDLocationList(string roleid)
        {
            return Json(_masterIMDLRoleLocationBLL.GetListIDLocationByIdUser(roleid), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataById(MasterIMDLRoleLocationInput criteria)
        {
            var masterIMDLRoleLocations = _masterIMDLRoleLocationBLL.GetById(criteria.IDIMDLRoleLocation);
            var viewModel = Mapper.Map<MasterIMDLRoleLocationViewModel>(masterIMDLRoleLocations);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWarehouseNameById(string id)
        {
            var masterUsers = _masterIMDLRoleLocationBLL.GetWarehouseNameByIdUser(id);
            return Json(masterUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertIMDLRoleLocation(InsertUpdateData<MasterIMDLRoleLocationViewModel> bulkData)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = "1";
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;

                    var mstIMDLRoleLocation = Mapper.Map<MasterIMDLRoleLocationDTO>(bulkData.New[i]);

                    mstIMDLRoleLocation.CreatedBy = GetUserId();
                    mstIMDLRoleLocation.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterIMDLRoleLocationBLL.SaveData(mstIMDLRoleLocation, controller, userid);
                        bulkData.New[i] = Mapper.Map<MasterIMDLRoleLocationViewModel>(item);
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

                    var mstIMDLRoleLocation = Mapper.Map<MasterIMDLRoleLocationDTO>(bulkData.Edit[i]);

                    mstIMDLRoleLocation.CreatedBy = GetUserId();
                    mstIMDLRoleLocation.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterIMDLRoleLocationBLL.EditData(mstIMDLRoleLocation, controller, userid);
                        bulkData.Edit[i] = Mapper.Map<MasterIMDLRoleLocationViewModel>(item);
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