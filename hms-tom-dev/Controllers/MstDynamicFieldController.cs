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
    public class MstDynamicFieldController : BaseController
    {
        private readonly IMasterDynamicFieldBLL _masterDynamicFieldBLL;
        //
        // GET: /MstDynamicField/

        public MstDynamicFieldController(IMasterDynamicFieldBLL masterDynamicFieldBLL)
        {
            _masterDynamicFieldBLL = masterDynamicFieldBLL;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetDataById(MasterDynamicFieldInput criteria)
        {
            var masterDynamicFields = _masterDynamicFieldBLL.GetById(criteria.IDDynamicField);
            var viewModel = Mapper.Map<MasterDynamicFieldViewModel>(masterDynamicFields);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMasterDynamicField(MasterDynamicFieldInput criteria)
        {
            var masterDynamicFields = _masterDynamicFieldBLL.GetMasterDynamicFields(criteria);
            var viewModel = Mapper.Map<List<MasterDynamicFieldViewModel>>(masterDynamicFields);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //public ActionResult InsertList(MasterListInput criteria)
        public ActionResult InsertDynamicField(InsertUpdateData<MasterDynamicFieldViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstDynamicField = Mapper.Map<MasterDynamicFieldDTO>(bulkData.New[i]);

                    //set createdby and updatedby

                    mstDynamicField.CreatedBy = GetUserId();
                    mstDynamicField.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterDynamicFieldBLL.SaveData(mstDynamicField);
                        bulkData.New[i] = Mapper.Map<MasterDynamicFieldViewModel>(item);
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
                    var mstDynamicField = Mapper.Map<MasterDynamicFieldDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstDynamicField.CreatedBy = GetUserId();
                    mstDynamicField.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterDynamicFieldBLL.EditData(mstDynamicField);
                        bulkData.Edit[i] = Mapper.Map<MasterDynamicFieldViewModel>(item);
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
            //MasterListDTO newDataToInsert = Mapper.Map<List<MasterListDTO>>(criteria);
            //var newDataToInsert = Mapper.Map<MasterListDTO>(criteria);

            //var newData = new MasterListDTO()
            //{
            //    FieldName = criteria.FieldName,
            //    FieldValue = criteria.FieldValue,
            //    IsActive = criteria.IsActive
            //};

            //var dataInsertAbove = _masterListBLL.GetMasterLists(criteria);

            //if (dataInsertAbove == null)
            //{
            //// insert new
            //    _masterListBLL.SaveData(newData);

            //    // get data after insert
            //    dataInsertAbove = _masterListBLL.GetMasterLists(criteria);
            //}
            ////else
            ////{
            ////    // remove data
            ////    _genBuildingFacilityBLL.Remove(newDataToInsert.WarehouseID);
            ////}
            ////var masterLists = _masterListBLL.SaveData(criteria);

            //var viewModel = Mapper.Map<List<MasterListViewModel>>(dataInsertAbove);
            //return Json(viewModel);
        }
	}
}