using AutoMapper;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.DTOs;
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
using TOM.Master.Domain.Inputs;

namespace hms_tom_dev.Controllers
{
    public class MstUomController : BaseController
    {
        private readonly IMasterUomBLL _masterUomBll;
        private readonly IMasterListBLL _masterListBLL;

        public MstUomController(IMasterUomBLL masterUomBll, IMasterListBLL masterListBLL)
        {
            _masterUomBll = masterUomBll;
            _masterListBLL = masterListBLL;
        }

        //
        // GET: /MstUom/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataById(MasterUomInput criteria)
        {
            var masterLists = _masterUomBll.GetById(criteria.IDUoM);
            var viewModel = Mapper.Map<MasterUomViewModel>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterUom(MasterUomInput criteria)
        {
            var dbResult = _masterUomBll.GetMasterUom(criteria);
            var viewModel = Mapper.Map<List<MasterUomViewModel>>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterListType(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterListByFieldName(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterListUom(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterListByFieldName(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<MasterUomViewModel> bulkData)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = "1";

            var editMode = bulkData.New == null;
            try
            {
                foreach (var data in (!editMode ? bulkData.New : bulkData.Edit))
                {
                    var mstList = Mapper.Map<MasterUomDTO>(data);
                    if (!editMode)
                        mstList.CreatedBy = GetUserId();
                    mstList.UpdatedBy = GetUserId();
                    if (!editMode)
                    {
                        var val = _masterUomBll.GetMasterUom(new MasterUomInput() { UoM = mstList.UoM, MaterialType = mstList.MaterialType });
                        if (val != null && val.Count > 0)
                        {
                            if (val[0].IsActive)
                            {
                                return Json(
                                new NonQueryResult(false, "Data already exists!")
                                );
                            }
                            val[0].IsActive = true;
                            _masterUomBll.EditData(val[0], controller, userid);
                        }
                        else
                            _masterUomBll.SaveData(mstList, controller, userid);
                    }
                    else
                        _masterUomBll.EditData(mstList, controller, userid);
                }
            }
            catch (Exception ex) { return Json(new NonQueryResult(false, ex.Message)); }
            return Json(new NonQueryResult(true));

            /*
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstList = Mapper.Map<MasterUomDTO>(bulkData.New[i]);

                    mstList.CreatedBy = GetUserId();
                    mstList.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterUomBll.SaveData(mstList, controller, userid);
                        //var item = _masterListBLL.SaveData(mstList);
                        bulkData.New[i] = Mapper.Map<MasterUomViewModel>(item);
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
                    var mstList = Mapper.Map<MasterUomDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstList.CreatedBy = GetUserId();
                    mstList.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterUomBll.EditData(mstList, controller, userid);
                        bulkData.Edit[i] = Mapper.Map<MasterUomViewModel>(item);
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
            */
        }
    }
}