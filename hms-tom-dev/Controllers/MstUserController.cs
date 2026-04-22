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

namespace hms_tom_dev.Controllers
{
    public class MstUserController : BaseController
    {
        private readonly IMasterUserBLL _masterUserBLL;

        public MstUserController(IMasterUserBLL masterUserBLL)
        {
            _masterUserBLL = masterUserBLL;
         
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMasterUsers(MasterUserInput criteria)
        {
            var masterUsers = _masterUserBLL.GetMasterUsers(criteria);

            var viewModel = Mapper.Map<List<MasteUserViewModel>>(masterUsers);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getMasterUserViews(MasterUserInput criteria)
        {
            var masterUsers = _masterUserBLL.GetMasterUserViews(criteria);

            var viewModel = Mapper.Map<List<MasteUserViewModel>>(masterUsers);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getDataByID(MasterUserInput criteria)
        {
            var _criteria = new MasterUserInput()
            {
                 IDUser = criteria.IDUser.Replace("-", "\\")
            };
            var masterUsers = _masterUserBLL.GetMasterUserViews(_criteria);

            var viewModel = Mapper.Map<List<MasteUserViewModel>>(masterUsers);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertUser(InsertUpdateData<MasteUserViewModel> bulkData)
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

                    var mstVendor = Mapper.Map<MasterUserDTO>(bulkData.New[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstVendor.CreatedBy = GetUserId();
                    mstVendor.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterUserBLL.SaveData(mstVendor, controller, userid);
                        bulkData.New[i] = Mapper.Map<MasteUserViewModel>(item);
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

                    var mstVendor = Mapper.Map<MasterUserDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstVendor.CreatedBy = GetUserId();
                    mstVendor.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterUserBLL.EditData(mstVendor, controller, userid);
                        bulkData.Edit[i] = Mapper.Map<MasteUserViewModel>(item);
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
