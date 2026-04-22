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
    public class MasterIMDLRoleController : BaseController
    {
        private readonly IMasterIMDLRoleBLL _masterIMDLRoleBLL;
        // GET: MasterIMDLRole

        public MasterIMDLRoleController(IMasterIMDLRoleBLL masterIMDLRoleBLL)
        {
            _masterIMDLRoleBLL = masterIMDLRoleBLL;
        }
        public ActionResult Index()
        {
            var ListGetRole = _masterIMDLRoleBLL.GetDataRole();
            ViewBag.ListGetRole = new SelectList(ListGetRole, "IDRole", "RoleName");
            return View();
        }

        public ActionResult GetMasterIMDLRole(MasterIMDLRoleInput criteria)
        {
            var masterIMDLRoles = _masterIMDLRoleBLL.GetMasterIMDLRoles(criteria);

            return Json(masterIMDLRoles, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataById(MasterIMDLRoleInput criteria)
        {
            var masterIMDLRoles = _masterIMDLRoleBLL.GetById(criteria.IMDLRole);
            var viewModel = Mapper.Map<MasterIMDLRoleViewModel>(masterIMDLRoles);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertIMDLRole(InsertUpdateData<MasterIMDLRoleViewModel> bulkData)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = "1";
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;

                    var mstIMDLRole = Mapper.Map<MasterIMDLRoleDTO>(bulkData.New[i]);

                    mstIMDLRole.CreatedBy = GetUserId();
                    mstIMDLRole.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterIMDLRoleBLL.SaveData(mstIMDLRole, controller, userid);
                        bulkData.New[i] = Mapper.Map<MasterIMDLRoleViewModel>(item);
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

                    var mstIMDLRole = Mapper.Map<MasterIMDLRoleDTO>(bulkData.Edit[i]);

                    mstIMDLRole.CreatedBy = GetUserId();
                    mstIMDLRole.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterIMDLRoleBLL.EditData(mstIMDLRole, controller, userid);
                        bulkData.Edit[i] = Mapper.Map<MasterIMDLRoleViewModel>(item);
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