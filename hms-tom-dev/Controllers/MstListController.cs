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
    public class MstListController : BaseController
    {
        private readonly IMasterListBLL _masterListBLL;

        public MstListController(IMasterListBLL masterListBLL)
        {
            _masterListBLL = masterListBLL;
            SetPage(EnumHelper.GetDescription(Enums.PageName.MasterList));
        }
        //
        // GET: /MstList/
        public ActionResult Index()
        {           
            return View();
        }

        public ActionResult GetDataById(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetById(criteria.IDList);
            var viewModel = Mapper.Map<MasterListViewModel>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterListByFieldName(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterListByFieldName(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterList(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterLists(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterListValue(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterListValues(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListForBrandCategory(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetListForBrandCategorys(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterListDrop(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterListDrops(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInputSvDataType(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetInputSvListView(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListForEmployee(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetListForEmployees(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListForFABrand(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetListForFABrands(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListForLocation(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetListForLocations(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public ActionResult InsertList(MasterListInput criteria)
        public ActionResult InsertList(InsertUpdateData<MasterListViewModel> bulkData)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = "1";

            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstList = Mapper.Map<MasterListDTO>(bulkData.New[i]);


                    //set createdby and updatedby
                   

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi

                    mstList.CreatedBy = GetUserId();
                    mstList.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterListBLL.SaveData(mstList, controller, userid);
                        //var item = _masterListBLL.SaveData(mstList);
                        bulkData.New[i] = Mapper.Map<MasterListViewModel>(item);
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
                    var mstList = Mapper.Map<MasterListDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstList.CreatedBy = GetUserId();
                    mstList.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterListBLL.EditData(mstList, controller, userid);
                        bulkData.Edit[i] = Mapper.Map<MasterListViewModel>(item);
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