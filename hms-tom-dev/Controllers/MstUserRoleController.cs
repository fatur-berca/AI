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

namespace hms_tom_dev.Controllers
{
    public class MstUserRoleController : BaseController
    {
        //
        // GET: /MstUserRole/

          private readonly IMasterUserRoleBLL _masterUserRoleBLL;


          public MstUserRoleController(IMasterUserRoleBLL masterUserRoleBLL)
        {
            _masterUserRoleBLL = masterUserRoleBLL;
         
        }
        public ActionResult Index()
        {
            var ListMasterUser = _masterUserRoleBLL.GetMasterUsers();
            var ListGetRole = _masterUserRoleBLL.GetMasterRoles();

            ViewBag.ListMasterUser = new SelectList(ListMasterUser, "IDUser", "IDUser");
            ViewBag.ListGetRole = new SelectList(ListGetRole, "IDRole", "RoleName");

            return View();
        }

        public ActionResult GetMasterUserRole(MasterUserRoleMappingDTO criteria)
        {
            var masterUsers = _masterUserRoleBLL.GetUserRoleMapping(criteria);

            return Json(masterUsers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataByID(MasterUserRoleMappingDTO criteria)
        {
            var masterUsers = _masterUserRoleBLL.GetDataByIDs(criteria);

            return Json(masterUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertData(InsertUpdateData<MasterUserRoleMappingViewModel> bulkData)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = "1";
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null)
                    {
                        continue;
                    }

                    var mstVendor = Mapper.Map<MasterUserRoleMappingDTO>(bulkData.New[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstVendor.CreatedBy = GetUserId();
                    mstVendor.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterUserRoleBLL.SaveData(mstVendor, controller, userid);
                        bulkData.New[i] = Mapper.Map<MasterUserRoleMappingViewModel>(item);
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
                    if (bulkData.Edit[i] == null)
                    {
                        continue;
                    }

                    var mstVendor = Mapper.Map<MasterUserRoleMappingDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstVendor.CreatedBy = GetUserId();
                    mstVendor.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterUserRoleBLL.EditData(mstVendor, controller, userid);
                        bulkData.Edit[i] = Mapper.Map<MasterUserRoleMappingViewModel>(item);
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