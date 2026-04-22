
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.Inputs;
using TOM.Master.Domain.DTOs;

namespace hms_tom_dev.Controllers
{
	public class MstDataLoadFactorCFPController : BaseController
	{
		private readonly IMasterLoadFactorCFPBLL _masterLoadFactorCFPBLL;
		private readonly IMasterListBLL _masterListBLL;
		private readonly IMasterFABrandBLL _masterFABLL;
		//
		// GET: /MstDataLoadFactorCFP/

		public MstDataLoadFactorCFPController(IMasterLoadFactorCFPBLL masterLoadFactorCFPBLL, IMasterListBLL masterListBLL, IMasterFABrandBLL masterFABLL)
		{
			_masterLoadFactorCFPBLL = masterLoadFactorCFPBLL;
		   
			_masterListBLL = masterListBLL;
			_masterFABLL = masterFABLL;
		}
		public ActionResult PartialViewUploadFile()
		{
			return View("_UploadFile");
		}
		public ActionResult Index()
		{
			MasterListInput criteria = new MasterListInput();           
			var dropbrandcategory = _masterListBLL.GetListForBrandCategorys(criteria);                     
			var dropmodel = _masterLoadFactorCFPBLL.GetListForModel(criteria);
			/*
			var data = _masterFABLL.GetAllMasterFABrands().Select(x => x.SpeakingCode.Substring(0, 5))
				.GroupBy(x=>x)
				.Select(x=>x.FirstOrDefault())
				.ToList();
			*/
			var data = _masterFABLL.GetAllMasterFABrands()
				.Select(x => x.SpeakingCode.Substring(0,5))
				.Distinct()
				.OrderBy(x => x);
			var dropvehicle = _masterLoadFactorCFPBLL.GetListForCPFs(criteria);
			ViewBag.ListBrandCategory = new SelectList(data);
			ViewBag.ListModel = new SelectList(dropmodel, "FieldValue", "FieldValue");
			ViewBag.ListVehicle = new SelectList(dropvehicle, "FieldValue", "FieldValue");
			return View();
		}
		public ActionResult GetListForCPF(MasterListInput criteria)
		{
			var masterLists = _masterLoadFactorCFPBLL.GetListForCPFs(criteria);
			var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}
		public ActionResult GetListForModel(MasterListInput criteria)
		{
			var masterLists = _masterLoadFactorCFPBLL.GetListForModel(criteria);
			var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}
		public ActionResult GetDataById(MasterLoadFactorCFPInput criteria)
		{
			var masterLoadFactorCFPBLL = _masterLoadFactorCFPBLL.GetById(criteria.IDLoadFactorCFP);
			var viewModel = Mapper.Map<MasterLoadFactorCFPViewModel>(masterLoadFactorCFPBLL);
			viewModel._WeightperStick = viewModel.WeightperStick.ToString().Replace(".", ",");
			viewModel._KgCO2perLiter = viewModel.KgCO2perLiter.ToString().Replace(".", ",");
			viewModel._KMperLiter = viewModel.KMperLiter.ToString().Replace(".", ",");          
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetMasterLoadFactorCFP(MasterLoadFactorCFPInput criteria)
		{
			var masterLoadFactorCFPBLL = _masterLoadFactorCFPBLL.GetMasterLoadFactorCFPs(criteria);
			var viewModel = Mapper.Map<List<MasterLoadFactorCFPViewModel>>(masterLoadFactorCFPBLL);
			
			for (int i = 0; i < viewModel.Count(); i ++) {
				viewModel[i]._WeightperStick = viewModel[i].WeightperStick.ToString().Replace(".", ",");
				viewModel[i]._KgCO2perLiter = viewModel[i].KgCO2perLiter.ToString().Replace(".", ",");
				viewModel[i]._KMperLiter = viewModel[i].KMperLiter.ToString().Replace(".", ",");
			}
			return Json(viewModel);
		}

		public ActionResult UploadFileManual()
		{
			int row = 0;
			string message = "";
			DataSet result = null;
			Excel.IExcelDataReader reader = null;
			List<MasterLoadFactorCFPViewModel> model = new List<MasterLoadFactorCFPViewModel>();
			if (Request.Files.Count > 0)
			{
				HttpPostedFileBase uploadFile = Request.Files.Get(0);

				if (uploadFile != null && uploadFile.ContentLength > 0)
				{
					var filename = uploadFile.FileName;
					if (filename.EndsWith(".xls"))
					{
						reader = Excel.ExcelReaderFactory.CreateBinaryReader(uploadFile.InputStream);
					}
					else if (filename.EndsWith(".xlsx"))
					{
						reader = Excel.ExcelReaderFactory.CreateOpenXmlReader(uploadFile.InputStream);
					}
					else
					{
						message = "This file format is not supported";
					}

					reader.IsFirstRowAsColumnNames = false;
					result = reader.AsDataSet();
					reader.Close();
					if (result.Tables.Count > 0)
					{
						bool hasError = false;                  
						foreach (DataRow r in result.Tables[0].Rows)
						{
							if (row == 0)
							{
								if (r.ItemArray.Length != 8)
								{
									message = message + "Excel column format not correct, please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[0].ToString() != "Vehicle Type")
								{
									message = message + "Excel first column must be Vehicle Type, please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[1].ToString() != "Mode")
								{
									message = message + "Excel second column must be Mode, please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[2].ToString() != "Fuel Ratio")
								{
									message = message + "Excel third column must be Fuel Ratio, please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[3].ToString() != "KgCO2 / (liter/TKM)")
								{
									message = message + "Excel fourth column must be KgCO2 / (liter/TKM), please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[4].ToString() != "Brand")
								{
									message = message + "Excel fifth column must be Brand, please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[5].ToString() != "Max Capacity")
								{
									message = message + "Excel sixth column must be Max Capacity, please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[6].ToString() != "Weight/Stick")
								{
									message = message + "Excel seventh column must be Weight/Stick, please see template for the correct format<br/>";
									hasError = true;
								}
								if (r[7].ToString() != "Effective Date")
								{
									message = message + "Excel eighth column must be Effective Date, please see template for the correct format<br/>";
									hasError = true;
								}
								if (hasError)
									break;
							}
							else { 
								var newRow = new MasterLoadFactorCFPDTO();
								newRow.IDLoadFactorCFP = 0;
								newRow.VehicleType = r[0].ToString();
								newRow.Mode = r[1].ToString();
								newRow.KMperLiter = String.IsNullOrEmpty(r[2].ToString()) ? 0 : Convert.ToDecimal(r[2]);
								newRow.KgCO2perLiter = Convert.ToDouble(r[3]);
								newRow.BrandCategory = r[4].ToString();
								newRow.MaxQty = Convert.ToInt32(r[5]);
								newRow.WeightperStick = Convert.ToDouble(r[6]);
								newRow.StartDate = Convert.ToDateTime(r[7]);
								newRow.EndDate = new DateTime(2999, 12, 31);
								newRow.IsActive = true;
								newRow.CreatedBy = GetUserId();
								newRow.UpdatedBy = GetUserId();
								var _uploadsaved = _masterLoadFactorCFPBLL.SaveData(newRow);
							}
							row++;
							//model.Add(newRow);
						}
					}
				}
			}
			return Json(message, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetFABrands()
		{
			var data = _masterFABLL.GetAllMasterFABrands()
				.Select(x => x.SpeakingCode.Substring(0, 5))
				.Distinct()
				.OrderBy(x => x);
			// var data = _masterFABLL.GetAllMasterFABrands().Select(x => x.SpeakingCode.Substring(0, 5)).ToList();
			return Json(data,JsonRequestBehavior.AllowGet);
		}

		[HttpPost]       
		
		public ActionResult InsertLoadFactorCFP(InsertUpdateData<MasterLoadFactorCFPViewModel> bulkData)
		{

			if (bulkData.New != null)
			{
				for (var i = 0; i < bulkData.New.Count; i++)
				{
					if (bulkData.New[i] == null) continue;
					var mstLoadFactorCFP = Mapper.Map<MasterLoadFactorCFPDTO>(bulkData.New[i]);

				   
					//set createdby and updatedby

					mstLoadFactorCFP.CreatedBy = GetUserId();
					mstLoadFactorCFP.UpdatedBy = GetUserId();

					try
					{
						var item = _masterLoadFactorCFPBLL.SaveData(mstLoadFactorCFP);
						bulkData.New[i] = Mapper.Map<MasterLoadFactorCFPViewModel>(item);
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
					
					var mstLoadFactorCFP = Mapper.Map<MasterLoadFactorCFPDTO>(bulkData.Edit[i]);

					//set createdby and updatedby, nanti ditambah SSO pas udh jadi
					mstLoadFactorCFP.CreatedBy = GetUserId();
					mstLoadFactorCFP.UpdatedBy = GetUserId();
					try
					{
						var item = _masterLoadFactorCFPBLL.EditData(mstLoadFactorCFP);
						bulkData.Edit[i] = Mapper.Map<MasterLoadFactorCFPViewModel>(item);
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