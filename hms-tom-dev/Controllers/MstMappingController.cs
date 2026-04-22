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
using System.IO;
using System.Data.OleDb;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class MstMappingController : BaseController
    {
        private readonly IMasterMappingBLL _masterMappingBLL;       
      
        //
        // GET: /MstMapping/

        public MstMappingController(IMasterMappingBLL masterMappingBLL)
        {
            _masterMappingBLL = masterMappingBLL;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataById(MasterMappingInput criteria)
        {
            var masterFABrands = _masterMappingBLL.GetById(criteria.IDMasterMapping);
            var viewModel = Mapper.Map<MasterMappingViewModel>(masterFABrands);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterMapping(MasterMappingInput criteria)
        {
            var masterFABrandBLL = _masterMappingBLL.GetMasterMappings(criteria);
            var viewModel = Mapper.Map<List<MasterMappingViewModel>>(masterFABrandBLL);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertMapping(InsertUpdateData<MasterMappingViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstFABrand = Mapper.Map<MasterMappingDTO>(bulkData.New[i]);

                    //set createdby and updatedby

                    mstFABrand.CreatedBy = GetUserId();
                    mstFABrand.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterMappingBLL.SaveData(mstFABrand);
                        bulkData.New[i] = Mapper.Map<MasterMappingViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        bulkData.New[i].Message = Enums.ResponseType.Success.ToString();

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
                    var mstFABrand = Mapper.Map<MasterMappingDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstFABrand.CreatedBy = GetUserId();
                    mstFABrand.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterMappingBLL.EditData(mstFABrand);
                        bulkData.Edit[i] = Mapper.Map<MasterMappingViewModel >(item);
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

        [HttpPost]
        public ActionResult UploadFile()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Assets/Uploads/MstMapping/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public ActionResult Import(string FileName)
        {
            List<MasterMappingDTO> input = new List<MasterMappingDTO>();

            string tes = Path.Combine(Server.MapPath("~/Assets/Uploads/MstMapping/"), FileName);
            string con = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + tes + ";Extended Properties=Excel 12.0;";

            using (OleDbConnection connection = new OleDbConnection(con))
            {
                connection.Open();

                OleDbCommand command = new OleDbCommand("select * from [Sheet1$]", connection);

                using (OleDbDataReader dr = command.ExecuteReader())
                {
                    string state = "reject";

                    while (dr.Read())
                    {
                        var mstMappingData = _masterMappingBLL.GetMasterMappings(new MasterMappingInput()
                        {
                            MapFrom = Convert.ToString(dr[0]),
                            MapTo = Convert.ToString(dr[1])
                        });

                        if (mstMappingData.Count() > 0)
                        {
                            // do nothing
                        }
                        else
                        {
                            var inputs = new MasterMappingDTO();

                            inputs.MapFrom = Convert.ToString(dr[0]);
                            inputs.MapTo = Convert.ToString(dr[1]);
                            inputs.IsActive = true;
                            inputs.CreatedBy = GetUserId();
                            inputs.CreatedDate = DateTime.Now;
                            inputs.UpdatedBy = GetUserId();
                            inputs.UpdatedDate = DateTime.Now;

                            input.Add(inputs);

                            state = "accept";
                        }
                    }

                    if (state == "accept")
                    {
                        for (var i = 0; i < input.Count; i++)
                        {
                            var masterMapping = Mapper.Map<MasterMappingDTO>(input[i]);

                            var item = _masterMappingBLL.Import(masterMapping);
                        }
                    }
                }
            }

            return Json(input, JsonRequestBehavior.AllowGet);
        }

	}
}