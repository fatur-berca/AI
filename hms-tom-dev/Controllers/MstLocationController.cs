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
    public class MstLocationController : BaseController
    {
        private readonly IMasterLocationBLL _masterLocationBLL;
        private readonly IMasterMappingLocationBLL _mstMapLocationBLL;

        public MstLocationController(IMasterLocationBLL masterLocationBLL, IMasterMappingLocationBLL mstMapLocationBLL)
        {
            _masterLocationBLL = masterLocationBLL;
            _mstMapLocationBLL = mstMapLocationBLL;
        }
        //
        // GET: /MstLocation/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataById(MasterLocationInput criteria)
        {
            var masterLocations = _masterLocationBLL.GetById(criteria.IDLocation);
            var viewModel = Mapper.Map<MasterLocationViewModel>(masterLocations);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterLocation(MasterLocationInput criteria)
        {
            var masterLocations = _masterLocationBLL.GetMasterLocations(criteria);
            var viewModel = Mapper.Map<List<MasterLocationViewModel>>(masterLocations);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterLocationTOM(MasterLocationInput criteria)
        {
            var masterLocations = _masterLocationBLL.GetMasterLocations(criteria);
            var viewModel = Mapper.Map<List<MasterLocationViewModel>>(masterLocations);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetMasterLocationTOM(string IDLocation, bool flag)
        {
            return Json(_masterLocationBLL.SetMasterLocationMapping(new MasterLocationDTO() { IDLocation = IDLocation }, flag), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterLocationDrop(MasterLocationInput criteria)
        {
            var masterLocations = _masterLocationBLL.GetMasterLocationDrops(criteria);
            var viewModel = Mapper.Map<List<MasterLocationViewModel>>(masterLocations);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartementForKPISS()
        {
            var masterLocations = _masterLocationBLL.GetDepartementForKPISSs();
            var viewModel = Mapper.Map<List<MasterLocationViewModel>>(masterLocations);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertLocation(InsertUpdateData<MasterLocationViewModel> bulkData)
        {
            /*
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstLocation = Mapper.Map<MasterLocationDTO>(bulkData.New[i]);


                    //set createdby and updatedby


                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi

                    mstLocation.CreatedBy = GetUserId();
                    mstLocation.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterLocationBLL.SaveData(mstLocation);
                        bulkData.New[i] = Mapper.Map<MasterLocationViewModel>(item);
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
                    var mstLocation = Mapper.Map<MasterLocationDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstLocation.CreatedBy = GetUserId();
                    mstLocation.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterLocationBLL.EditData(mstLocation);
                        bulkData.Edit[i] = Mapper.Map<MasterLocationViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                }
            }
            */


            var mstLocation = Mapper.Map<MasterLocationDTO>(bulkData.New != null ? bulkData.New[0] : bulkData.Edit[0]);
            if (bulkData.New != null)
            {
                mstLocation.CreatedBy = GetUserId();
                mstLocation.CreatedDate = DateTime.Now;
            }
            mstLocation.UpdatedBy = GetUserId();
            mstLocation.UpdatedDate = DateTime.Now;

            try
            {
                _masterLocationBLL.InsertOrUpdate(mstLocation);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
            // return Json(bulkData);
        }

        public JsonResult GetWarehouseResult()
        {
            var type = Request.Params["types"];
            var region = Request.Params["region"];

            var dbResult = _masterLocationBLL.GetWarehouseList(type, region);
            var viewResult = Mapper.Map<List<MasterLocationViewModel>>(dbResult);

            return Json(viewResult, JsonRequestBehavior.AllowGet);
        }
    }
}