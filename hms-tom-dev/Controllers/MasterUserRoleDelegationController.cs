using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using DFIS.Universal.BusinessLogics;
using TOM.Master.BusinessLogics;
using DFIS.Universal.Domain.Inputs;
using hms_tom_dev.Models.Masters;
using DFIS.Universal.Domain.DTOs;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Controllers
{
    public class MasterUserRoleDelegationController : BaseController
    {
        private readonly IMasterUserBLL _masterUserBll;
        private readonly IMasterUserRoleBLL _masterUserRoleBll;
        private readonly IMasterLocationBLL _masterLocationBLL;
        private readonly IMasterUserRoleDelegationBLL _masterUserRoleDelegationBLL;
        private readonly IUtilitiesBLL _utilBLL;
        private readonly IMasterUserLocationMappingBLL _masterUserLocationMappingBLL;
        public MasterUserRoleDelegationController(IMasterUserBLL masterUserBll, IMasterUserRoleBLL masterUserRoleBll, IMasterLocationBLL masterLocationBLL, IMasterUserRoleDelegationBLL masterUserRoleDelegationBLL, IUtilitiesBLL utilBLL, IMasterUserLocationMappingBLL masterUserLocationMappingBLL)
        {
            _masterUserBll = masterUserBll;
            _masterUserRoleBll = masterUserRoleBll;
            _masterLocationBLL = masterLocationBLL;
            _masterUserRoleDelegationBLL = masterUserRoleDelegationBLL;
            _utilBLL = utilBLL;
            _masterUserLocationMappingBLL = masterUserLocationMappingBLL;
        }
        //
        // GET: /MasterUserRoleDelegation/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(MasterUserRoleDelegationViewInput criteria)
        {
            var dbResult = _masterUserRoleDelegationBLL.GetData(criteria);
            var viewModel = Mapper.Map<List<MasterUserRoleDelegationViewViewModel>>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserLocationById()
        {
            var id = Int32.Parse(Request.Params["IDUserDelegation"]);
            var dbResult = _masterUserRoleDelegationBLL.GetDataByIdParent(id);
            var viewModel = Mapper.Map<List<UserLocationDelegationDTO>>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDelegateTo(MasterUserInput criteria)
        {
           
            var dbResult = _masterUserBll.GetMasterUsers(criteria);
            var viewModel = Mapper.Map<List<MasteUserViewModel>>(dbResult);
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDelegateRole(MasterUserRoleMappingDTO criteria)
        {
            var dbResult = _masterUserRoleBll.GetUserRoleMapping(criteria);
            var viewModel = Mapper.Map<List<MasterUserRoleMappingViewModel>>(dbResult);
            return Json(
                from role in dbResult
                group role by new { role.IDRole } into resGroup
                select resGroup.First()
                , JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDelegateLocation()
        {
            string id = Request.Params["IdUser"];
            string type = Request.Params["Type"];
            var dbResult = _masterUserLocationMappingBLL.GetLocationsByIdUserAndWarehouseType(id);
            
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InputData(MasterUserRoleDelegationInput input)
        {
            //input.IDUserFrom = GetUserId();
            var inputData = Mapper.Map<MasterUserRoleDelegationDTO>(input);

            try
            {
                inputData.CurrentUser = GetUserId();
                inputData.UpdatedBy = GetUserId();

                if (inputData.CreatedBy == null)
                {
                    inputData.CreatedBy = GetUserId();
                    inputData.CreatedDate = DateTime.Now;
                }

                _masterUserRoleDelegationBLL.InsertOrUpdate(inputData);

                if (input.EffectiveStartDate.Date >= DateTime.Now.Date)
                {
                    MasterUserRoleMappingDTO dataIn = new MasterUserRoleMappingDTO()
                    {
                        IDRole = inputData.DelegationIDRole.Value,
                        IDUser = inputData.IDUserTo,
                        IsActive = inputData.IsActive.Value,
                        CreatedBy = GetUserId(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = GetUserId(),
                        UpdatedDate = DateTime.Now,
                        Remarks = "-"                        
                    };
                    _utilBLL.SaveMasterUserRoleMapping(dataIn);

                    MasterUserLocationMappingDTO mulInput = new MasterUserLocationMappingDTO()
                    {
                        IDUser = inputData.IDUserTo,
                        IsActive = inputData.IsActive.Value,
                        CreatedBy = GetUserId(),
                        UpdatedBy = GetUserId(),
                    };

                    string[] locationDelegation = input.DelegationIDLocation.Split(',');
                    foreach (var id in locationDelegation)
                    {
                        mulInput.IDLocation = id;
                        _utilBLL.SaveMasterUserLocationMapping(mulInput);
                    }
                }
                return Json(new NonQueryResult(true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(new NonQueryResult(false, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

    }
}