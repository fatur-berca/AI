using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class MstRoleFunctionController : BaseController
    {
        private readonly IMasterRoleFunctionBLL _masterRoleFuncionBll;

        public MstRoleFunctionController(IMasterRoleFunctionBLL masterRoleFunctionBll)
        {
            _masterRoleFuncionBll = masterRoleFunctionBll;
        }
        //
        // GET: /MstRoleFunction/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDropDownRole()
        {
            return Json(_masterRoleFuncionBll.GetRoleList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTreeListFunction()
        {
            return Json(_masterRoleFuncionBll.GetMasterRoleFunctionTreeList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIDFunctionList(int roleid)
        {
            return Json(_masterRoleFuncionBll.GetIDFunctionList(roleid), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterRoleFunction(MasterRoleFunctionInput criteria)
        {
            var viewModel = Mapper.Map<List<MasterRoleFunctionViewModel>>(_masterRoleFuncionBll.GetDistinctMasterRoleFunction());
            return Json(viewModel, JsonRequestBehavior.AllowGet);

        }
        
            //return Json(viewModel, JsonRequestBehavior.AllowGet);

            //var viewModel = Mapper.Map<List<MasterRoleFunctionViewModel>>(_masterRoleFuncionBll.GetMasterRoleFunctions(criteria));
        public ActionResult UpdateIsActive(int idrole, bool status)
        {
            _masterRoleFuncionBll.UpdateIsActiveByIDRole(idrole, status);
            return Json(new MasterRoleFunctionViewModel());
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<MasterRoleFunctionViewModel> bulkData)
        {
            try
            {
                MasterRolesFunctionMapping item;
                if (bulkData.New != null)
                {
                    MasterRoleFunctionDTO mstRoleFunction = Mapper.Map<MasterRoleFunctionDTO>(bulkData.New[0]);
                    mstRoleFunction.CreatedBy = GetUserId();
                    mstRoleFunction.UpdatedBy = GetUserId();
                    item = _masterRoleFuncionBll.SaveData(mstRoleFunction, true);
                }
                else
                {
                    MasterRoleFunctionDTO mstRoleFunction = Mapper.Map<MasterRoleFunctionDTO>(bulkData.Edit[0]);
                    mstRoleFunction.UpdatedBy = GetUserId();
                    item = _masterRoleFuncionBll.SaveData(mstRoleFunction, false);
                }
                //bulkData.Edit[0] = Mapper.Map<MasterFunctionViewModel>(item);
                //bulkData.Edit[0].ResponseType = Enums.ResponseType.Success.ToString();
            }
            catch (ExceptionBase ex)
            {
                //bulkData.Edit[0].ResponseType = Enums.ResponseType.Error.ToString();
                //bulkData.Edit[0].Message = ex.Message;
            }
            return Json(bulkData);
        }
	}
}