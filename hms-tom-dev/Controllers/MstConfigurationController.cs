using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System.Linq;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class MstConfigurationController : BaseController
    {
        private readonly IMasterConfigurationBLL _masterConfigurationBll;

        public MstConfigurationController(IMasterConfigurationBLL masterConfigurationBll)
        {
            _masterConfigurationBll = masterConfigurationBll;
         }
        //
        // GET: /MstConfiguration/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDropDownPageName(string type)
        {
            return Json(_masterConfigurationBll.GetDistinctPageName(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterConfiguration(MasterConfigurationInput criteria)
        {
            var viewModel = Mapper.Map<List<MasterConfigurationViewModel>>(_masterConfigurationBll.GetAllMasterConfiguration());
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecords(MasterConfigurationInput criteria)
        {
            var masterConfiguration = _masterConfigurationBll.GetRecords(criteria);
            var viewModel = Mapper.Map<List<MasterConfigurationViewModel>>(masterConfiguration);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<MasterConfigurationViewModel> bulkData)
        {
            string result;
            MasterConfiguration checkAvailable;
            try
            {
                if (bulkData.New != null || bulkData.Edit != null)
                {
                    MasterConfiguration mstConfiguration = null;

                    if (bulkData.New != null)
                    {
                        mstConfiguration = Mapper.Map<MasterConfiguration>(bulkData.New[0]);
                    }
                    else if (bulkData.Edit != null)
                    {
                        mstConfiguration = Mapper.Map<MasterConfiguration>(bulkData.Edit[0]);
                    }
                    mstConfiguration.CreatedBy = GetUserId();
                    mstConfiguration.UpdatedBy = GetUserId();
                    //mstConfiguration.Value = mstConfiguration.Value.Replace(" ", "");
                    if (mstConfiguration.PageName == "TransportLostdamageClaim")
                    {
                        var listuser = mstConfiguration.Value.Split(';').ToList();

                        List<string> DoubleData = new List<string>();
                        foreach (var item in listuser)
                        {
                            DoubleData = listuser.Where(c => c.Contains(item)).ToList();
                        }

                        if (DoubleData.Count == 1)
                        {
                            var status = true;
                            foreach (var item in listuser)
                            {
                                var existuser = _masterConfigurationBll.GetUser(item);
                                if (existuser == null)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                checkAvailable = _masterConfigurationBll.SaveData(mstConfiguration, true);
                                result = checkAvailable == null ? Enums.ResponseType.Error.ToString() : Enums.ResponseType.Success.ToString();
                            }
                            else
                            {
                                result = Enums.ResponseType.NotExist.ToString();
                            }
                        }
                        else
                        {
                            result = Enums.ResponseType.DoubleData.ToString();
                        }

                    }
                    else
                    {
                        checkAvailable = _masterConfigurationBll.SaveData(mstConfiguration, true);
                        result = checkAvailable == null ? Enums.ResponseType.Error.ToString() : Enums.ResponseType.Success.ToString();
                    }
                }
                else
                {
                    var mstConfiguration = Mapper.Map<MasterConfiguration>(bulkData.Edit[0]);
                    mstConfiguration.CreatedBy = GetUserId();
                    mstConfiguration.UpdatedBy = GetUserId();
                    checkAvailable = _masterConfigurationBll.SaveData(mstConfiguration, false);
                    result = Enums.ResponseType.Success.ToString();
                }
            }
            catch (ExceptionBase ex)
            {
                result = Enums.ResponseType.Error.ToString();
            }
            return Json(result);
        }
    }
}