using AutoMapper;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SelectPdf;
using System.Data.Entity.Validation;
using TOM.Transport.BusinessLogics.TransportLostClaimDamageBLL;
using TOM.Master.BusinessLogics;
using hms_tom_dev.Controllers;
using DFIS.Utils;
using DFIS.Universal.Domain.Outputs;
using hms_tom_dev.Models.Transport;
using TOM.Transport.Domain.Inputs;
using DFIS.Utils.Exceptions;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Util;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Enums;
using System.Configuration;

namespace hms_dfis_dev.Controllers.Transport
{
    public class TransportLostDamageClaimController : BaseController
    {
        private readonly ITransportLostClaimDamageBLL _bll;
        private readonly IMasterConfigurationBLL _configurationbll;

        public TransportLostDamageClaimController(ITransportLostClaimDamageBLL bll, IMasterConfigurationBLL configurationbll)
        {
            _bll = bll;
            _configurationbll = configurationbll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportLostDamageClaim));
        }

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList(); 
            ViewBag.ListUserRole = GetListUserRole();
            return View();
        }

        public ActionResult Add()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            if (GetListUserRole().Any())
            {
                ViewBag.ListCurrRoles = GetListUserRole().Select(c => c.RoleName).Distinct().ToList();
            }

            ViewBag.IsSuperUserOrTransport = session.Role.Where(f => f.IDRole == 1 || f.IDRole == 5).ToList().Count >= 1;
            var viewModel = new TransportLostClaimDamageViewModel();

            viewModel.ClaimTypeDropDown = GetClaimTypeDropDown();
            viewModel.SJNumberDropDown = GetSJNumberDropDown();
            viewModel.ClaimExpenseDropDown = GetClaimExpenseDropDown();
            viewModel.ClaimCategoryDropDown = GetClaimCategoryDropDown();
            //viewModel.GPNumberDropDown = GetGPNumberDropDown();

            return View("Add", viewModel);
        }

        [HttpPost]
        public ActionResult SetStatusResult(TransportLostClaimDamageInput filter)
        {
            try
            {
                _bll.SetActive(filter.ListIsActiveIdString, filter.IsActive);

                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddInitiation(TransportLostClaimDamageViewModel viewModel)
        {
            try
            {
                // assign value disabled input from hidden field
                viewModel.GPNumber = viewModel.GPNumberHidden;
                viewModel.SJDate = viewModel.SJDateHidden;
                viewModel.DateOfDelivery = viewModel.DateOfDeliveryHidden;

                if (viewModel.ClaimType != "Other" && viewModel.ClaimType != "Misshandling") {
                    viewModel.DestinationWarehouse = viewModel.DestinationWarehouseHidden;
                    viewModel.OriginWarehouse = viewModel.OriginWarehouseHidden;
                    viewModel.PoliceRegNumber = viewModel.PoliceRegNumberHidden;
                }
                
                viewModel.TransportationVendor = Convert.ToInt32(viewModel.TransportationVendorHidden);
                viewModel.STONumber = viewModel.STONumberHidden;

                if (viewModel.ClaimType == "Misshandling" || viewModel.ClaimType == "Other")
                {
                    viewModel.SJNumber = viewModel.SJNumberHidden;
                    viewModel.GPNumber = viewModel.GPNumberHidden;
                }

                // Removing list FA Code null
                viewModel.ListFACode.RemoveAll(c => (c.FACode == null || c.FACode == ""));

                var listFileName = Directory.EnumerateFiles(Server.MapPath("~/Assets/Uploads/TransportLostClaimDamage"));

                var uploadBoxCondition1FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-1-UploadBoxCondition.jpg")).FirstOrDefault();
                var uploadBoxCondition2FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-2-UploadBoxCondition.jpg")).FirstOrDefault();
                var uploadBoxCondition3FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-3-UploadBoxCondition.jpg")).FirstOrDefault();

                var uploadPositionOnTruck1FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-1-UploadPositionOnTruck.jpg")).FirstOrDefault();
                var uploadPositionOnTruck2FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-2-UploadPositionOnTruck.jpg")).FirstOrDefault();
                var uploadPositionOnTruck3FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-3-UploadPositionOnTruck.jpg")).FirstOrDefault();

                var uploadFinalResult1FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-1-UploadFinalResult.jpg")).FirstOrDefault();
                var uploadFinalResult2FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-2-UploadFinalResult.jpg")).FirstOrDefault();
                var uploadFinalResult3FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-3-UploadFinalResult.jpg")).FirstOrDefault();

                if (viewModel.IsAnyAttachment)
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.VERIFICATION.ToString();
                    //dataReturn.RealStatus = TransportEnums.LostClaimProgress.VERIFICATION.ToString();
                }
                else
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.INITIATION.ToString();
                    // dataReturn.TransportationMode = TransportEnums.LostClaimProgress.INITIATION.ToString();
                }

                if (String.IsNullOrEmpty(uploadBoxCondition1FileName))
                    viewModel.ImageUploadConditionBoxPath1 = string.Empty;
                else
                    viewModel.ImageUploadConditionBoxPath1 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadBoxCondition1FileName);

                if (String.IsNullOrEmpty(uploadBoxCondition2FileName))
                    viewModel.ImageUploadConditionBoxPath2 = string.Empty;
                else
                    viewModel.ImageUploadConditionBoxPath2 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadBoxCondition2FileName);

                if (String.IsNullOrEmpty(uploadBoxCondition3FileName))
                    viewModel.ImageUploadConditionBoxPath3 = string.Empty;
                else
                    viewModel.ImageUploadConditionBoxPath3 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadBoxCondition3FileName);

                // Position Truck
                if (String.IsNullOrEmpty(uploadPositionOnTruck1FileName))
                    viewModel.ImageUploadPositionTruckPath1 = string.Empty;
                else
                    viewModel.ImageUploadPositionTruckPath1 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadPositionOnTruck1FileName);

                if (String.IsNullOrEmpty(uploadPositionOnTruck2FileName))
                    viewModel.ImageUploadPositionTruckPath2 = string.Empty;
                else
                    viewModel.ImageUploadPositionTruckPath2 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadPositionOnTruck2FileName);

                if (String.IsNullOrEmpty(uploadPositionOnTruck3FileName))
                    viewModel.ImageUploadPositionTruckPath3 = string.Empty;
                else
                    viewModel.ImageUploadPositionTruckPath3 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadPositionOnTruck3FileName);

                // Final Result
                if (String.IsNullOrEmpty(uploadFinalResult1FileName))
                    viewModel.ImageUploadFinalResultPath1 = string.Empty;
                else
                    viewModel.ImageUploadFinalResultPath1 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadFinalResult1FileName);

                if (String.IsNullOrEmpty(uploadFinalResult2FileName))
                    viewModel.ImageUploadFinalResultPath2 = string.Empty;
                else
                    viewModel.ImageUploadFinalResultPath2 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadFinalResult2FileName);

                if (String.IsNullOrEmpty(uploadFinalResult3FileName))
                    viewModel.ImageUploadFinalResultPath3 = string.Empty;
                else
                    viewModel.ImageUploadFinalResultPath3 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadFinalResult3FileName);

                var dto = Mapper.Map<TransportLostClaimDamageDTO>(viewModel);
                var returnData = _bll.SaveInitiation(dto, GetUserId());
                UserSession currentLoginSession = Session["CurrentUser"] as UserSession;
                string loginName = "";
                if (currentLoginSession != null)
                {
                    loginName = currentLoginSession.Name;
                }
                returnData.CreatedBy = loginName;
                //var getValueInitiation = _configurationbll.getValue("TransportLostDamageClaim", "Initiation");
                //var listuser = getValueInitiation[0].Split(';').ToList();
                //_bll.SaveNotification(returnData, listuser, GetUserName());

                return Json(new
                {
                    success = true,
                    gpNumber = returnData.GPNumber,
                    sjNumber = returnData.SJNumber,
                    totalClaimPack = returnData.TotalClaimInPack,
                    totalClaimStick = returnData.TotalClaimInStick,
                    status = viewModel.Status,
                    message = "Saved Initiation Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    success = false,
                    status = viewModel.Status,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }

        [HttpPost]
        public ActionResult EditInitiation(TransportLostClaimDamageViewModel viewModel)
        {
            try
            {
                viewModel.SJDate = viewModel.SJDateHidden;
                viewModel.DateOfDelivery = viewModel.DateOfDeliveryHidden;
                viewModel.DestinationWarehouse = viewModel.DestinationWarehouseHidden;
                viewModel.OriginWarehouse = viewModel.OriginWarehouseHidden;
                viewModel.STONumber = viewModel.STONumberHidden;
                viewModel.GPNumber = viewModel.GPNumberHidden;
                viewModel.TransportationVendor = Convert.ToInt32(viewModel.TransportationVendorHidden);
                viewModel.PoliceRegNumber = viewModel.PoliceRegNumberHidden;
                if (String.IsNullOrEmpty(viewModel.GPNumber))
                    viewModel.GPNumber = viewModel.GPNumberOld;

                if (viewModel.ClaimType == "Misshandling" || viewModel.ClaimType == "Other")
                {
                    viewModel.GPNumber = viewModel.GPNumberHidden;
                    viewModel.SJNumber = viewModel.SJNumberHidden;
                }
                else
                {
                    if (viewModel.SJNumber.Contains("MSH") || viewModel.SJNumber.Contains("OTH"))
                    {
                        return Json(new
                        {
                            success = false,
                            status = viewModel.Status,
                            message = "Format Order Number or Transportation Number invalid for claim type " + viewModel.ClaimType
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                var listFileName = Directory.EnumerateFiles(Server.MapPath("~/Assets/Uploads/TransportLostClaimDamage"));

                var uploadBoxCondition1FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-1-UploadBoxCondition.jpg")).FirstOrDefault();
                var uploadBoxCondition2FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-2-UploadBoxCondition.jpg")).FirstOrDefault();
                var uploadBoxCondition3FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-3-UploadBoxCondition.jpg")).FirstOrDefault();

                var uploadPositionOnTruck1FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-1-UploadPositionOnTruck.jpg")).FirstOrDefault();
                var uploadPositionOnTruck2FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-2-UploadPositionOnTruck.jpg")).FirstOrDefault();
                var uploadPositionOnTruck3FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-3-UploadPositionOnTruck.jpg")).FirstOrDefault();

                var uploadFinalResult1FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-1-UploadFinalResult.jpg")).FirstOrDefault();
                var uploadFinalResult2FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-2-UploadFinalResult.jpg")).FirstOrDefault();
                var uploadFinalResult3FileName = listFileName.Where(c => c.Contains(viewModel.GPNumber.Replace("/", "~") + "-" + viewModel.SJNumber.Replace("/", "~") + "-3-UploadFinalResult.jpg")).FirstOrDefault();

                if (String.IsNullOrEmpty(uploadBoxCondition1FileName))
                    viewModel.ImageUploadConditionBoxPath1 = string.Empty;
                else
                    viewModel.ImageUploadConditionBoxPath1 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadBoxCondition1FileName);
                if (String.IsNullOrEmpty(uploadBoxCondition2FileName))
                    viewModel.ImageUploadConditionBoxPath2 = string.Empty;
                else
                    viewModel.ImageUploadConditionBoxPath2 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadBoxCondition2FileName);
                if (String.IsNullOrEmpty(uploadBoxCondition3FileName))
                    viewModel.ImageUploadConditionBoxPath3 = string.Empty;
                else
                    viewModel.ImageUploadConditionBoxPath3 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadBoxCondition3FileName);

                if (viewModel.IsAnyAttachment)
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.VERIFICATION.ToString();
                    //dataReturn.RealStatus = TransportEnums.LostClaimProgress.VERIFICATION.ToString();
                }
                else
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.INITIATION.ToString();
                    // dataReturn.TransportationMode = TransportEnums.LostClaimProgress.INITIATION.ToString();
                }
                // Position Truck
                if (String.IsNullOrEmpty(uploadPositionOnTruck1FileName))
                    viewModel.ImageUploadPositionTruckPath1 = string.Empty;
                else
                    viewModel.ImageUploadPositionTruckPath1 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadPositionOnTruck1FileName);
                if (String.IsNullOrEmpty(uploadPositionOnTruck2FileName))
                    viewModel.ImageUploadPositionTruckPath2 = string.Empty;
                else
                    viewModel.ImageUploadPositionTruckPath2 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadPositionOnTruck2FileName);
                if (String.IsNullOrEmpty(uploadPositionOnTruck3FileName))
                    viewModel.ImageUploadPositionTruckPath3 = string.Empty;
                else
                    viewModel.ImageUploadPositionTruckPath3 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadPositionOnTruck3FileName);

                // Final Result
                if (String.IsNullOrEmpty(uploadFinalResult1FileName))
                    viewModel.ImageUploadFinalResultPath1 = string.Empty;
                else
                    viewModel.ImageUploadFinalResultPath1 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadFinalResult1FileName);
                if (String.IsNullOrEmpty(uploadFinalResult2FileName))
                    viewModel.ImageUploadFinalResultPath2 = string.Empty;
                else
                    viewModel.ImageUploadFinalResultPath2 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadFinalResult2FileName);
                if (String.IsNullOrEmpty(uploadFinalResult3FileName))
                    viewModel.ImageUploadFinalResultPath3 = string.Empty;
                else
                    viewModel.ImageUploadFinalResultPath3 = "~/Assets/Uploads/TransportLostClaimDamage/" + Path.GetFileName(uploadFinalResult3FileName);

                // Removing list FA Code null
                viewModel.ListFACode.RemoveAll(c => (c.FACode == null || c.FACode == ""));
                viewModel.ListFACode.ForEach(x => x.CategoryTemplates = new SelectList(GetClaimTypeDropDown(),"","",x.LostOrDamage));

                var dto = Mapper.Map<TransportLostClaimDamageDTO>(viewModel);
                var returnData = _bll.EditInitiation(dto, GetUserId());

                return Json(new
                {
                    success = true,
                    status = viewModel.Status,
                    totalClaimPack = returnData.TotalClaimInPack,
                    totalClaimStick = returnData.TotalClaimInStick,
                    message = "Saved Initiation Successfully"
                });
                //var returnData = "";//_bll.EditInitiation(dto, GetUserId());

                //return Json(new
                //{
                //    success = true,
                //    status = viewModel.Status,
                //    totalClaimPack = "",//returnData.TotalClaimInPack,
                //    totalClaimStick = "",//returnData.TotalClaimInStick,
                //    message = "Saved Initiation Successfully"
                //}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    success = false,
                    status = viewModel.Status,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveInitiationAttachment()
        {
            try
            {
                string gpNumber = Request.Headers["X-GPNumber"].ToString();
                string sjNumber = Request.Headers["X-SJNumber"].ToString();

                var path = Server.MapPath("~/Assets/Uploads/TransportLostClaimDamage/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    //Use the following properties to get file's name, size and MIMEType
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;
                    //To save file, use SaveAs method
                    if (fileName.Contains("\\"))
                    {
                        fileName = fileName.Split('\\')[fileName.Split('\\').Length - 1];
                    }
                    file.SaveAs(path + fileName); //File will be saved in application root
                    _bll.SaveInitiationAttachment(fileName, sjNumber, gpNumber);
                }

                return Json(new
                {
                    success = true,
                    status = TransportEnums.LostClaimProgress.INITIATION.ToString(),
                    message = "Saved Initiation Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.StatusDescription = ExceptionExtensions.GetFullMessage(ex);
                return Json(ExceptionExtensions.GetFullMessage(ex));
            }

        }

        [HttpPost]
        public ActionResult SaveFile()
        {
            try
            {
                string gpNumber = Request.Headers["X-GPNumber"].ToString();
                string sjNumber = Request.Headers["X-SJNumber"].ToString();
                string status = Request.Headers["X-Status"].ToString();

                var path = Server.MapPath("~/Assets/Uploads/TransportLostClaimDamage/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    //Use the following properties to get file's name, size and MIMEType
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;
                    //To save file, use SaveAs method
                    if (fileName.Contains("\\"))
                    {
                        fileName = fileName.Split('\\')[fileName.Split('\\').Length - 1];
                    }
                    file.SaveAs(path + fileName); //File will be saved in application root
                    _bll.SaveFile(fileName, sjNumber, gpNumber, status, GetUserId());
                }

                return Json(new
                {
                    success = true,
                    status = TransportEnums.LostClaimProgress.INITIATION.ToString(),
                    message = "Saved Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.StatusDescription = ExceptionExtensions.GetFullMessage(ex);
                return Json(ExceptionExtensions.GetFullMessage(ex));
            }

        }

        [HttpPost]
        public ActionResult SaveInitiationUploadBoxes()
        {
            try
            {
                string gpNumber = Request.Headers["X-GPNumber"].ToString();
                string sjNumber = Request.Headers["X-SJNumber"].ToString();

                var path = Server.MapPath("~/Assets/Uploads/TransportLostClaimDamage/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    //Use the following properties to get file's name, size and MIMEType
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;
                    //To save file, use SaveAs method
                    if (fileName.Contains("\\"))
                    {
                        fileName = fileName.Split('\\')[fileName.Split('\\').Length - 1];
                    }

                    if (!String.IsNullOrEmpty(fileName) && fileName != "blob")
                    {
                        file.SaveAs(path + fileName); //File will be saved in application root
                    }
                }

                return Json(new
                {
                    success = true,
                    status = TransportEnums.LostClaimProgress.INITIATION.ToString(),
                    message = "Saved Upload Boxes Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.StatusDescription = ExceptionExtensions.GetFullMessage(ex);
                return Json(ExceptionExtensions.GetFullMessage(ex));
            }

        }

        [HttpPost]
        public ActionResult SaveVerification(TransportLostClaimDamageViewModel viewModel)
        {
            try
            {
                //viewModel.TotalClaimInPack = viewModel.TotalClaimInPackHidden;
                //viewModel.TotalClaimInStick = viewModel.TotalClaimInStickHidden;

                var dto = Mapper.Map<TransportLostClaimDamageDTO>(viewModel);
                //var getValueVerification = _configurationbll.getValue("TransportLostDamageClaim", "Verification");
                //var listuser = getValueVerification[0].Split(';').ToList();

                UserSession currentLoginSession = Session["CurrentUser"] as UserSession;
                string loginName = "";
                if (currentLoginSession != null)
                {
                    loginName = currentLoginSession.Name;
                }
                dto.CreatedBy = loginName;
                dto.RealStatus = TransportEnums.LostClaimProgress.PROCESS.ToString();
                if(viewModel.TransportationMode == null)
                    dto.TransportationMode = viewModel.TransportationModeHidden;
                if(viewModel.InitClaimedAmount == null)
                    dto.InitClaimedAmount = Int32.Parse(viewModel.InitClaimedAmountHidden);
                if(viewModel.TotalClaimInPack == null)
                    dto.TotalClaimInPack = Int32.Parse(viewModel.TotalClaimInPackHidden);
                if(viewModel.TotalClaimInStick == null)
                    dto.TotalClaimInStick = Int32.Parse(viewModel.TotalClaimInStickHidden);
                _bll.SaveVerification(dto, GetUserId());

                dto.Status = TransportEnums.LostClaimProgress.VERIFICATION.ToString();
                //_bll.SaveNotification(dto, listuser, GetUserName());

                return Json(new
                {
                    success = true,
                    status = viewModel.Status,
                    message = "Saved Verification Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    success = false,
                    status = viewModel.Status,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveProcess(TransportLostClaimDamageViewModel viewModel)
        {
            try
            {
                //if (String.IsNullOrEmpty(viewModel.BSWarehouseReceiveDate)) dto.BSWarehouseReceiveDate = null;
                //else dto.BSWarehouseReceiveDate = DateTime.ParseExact(viewModel.BSWarehouseReceiveDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                //if (String.IsNullOrEmpty(viewModel.HMSMemoDate)) dto.HMSMemoDate = null;
                //else dto.HMSMemoDate = DateTime.ParseExact(viewModel.HMSMemoDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var dto = Mapper.Map<TransportLostClaimDamageDTO>(viewModel);
                //var getValueVerification = _configurationbll.getValue("TransportLostDamageClaim", "Process");
                //var listuser = getValueVerification[0].Split(';').ToList();

                UserSession currentLoginSession = Session["CurrentUser"] as UserSession;
                string loginName = "";
                if (currentLoginSession != null)
                {
                    loginName = currentLoginSession.Name;
                }
                if (dto.HMSMemoNumber != null)
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.INVOICING.ToString();
                }
                else
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.PROCESS.ToString();
                }
                dto.CreatedBy = loginName;
                _bll.SaveProcess(dto, GetUserId());

                if (dto.HMSMemoNumber != null)
                {
                    dto.Status = TransportEnums.LostClaimProgress.PROCESS.ToString();
                    //_bll.SaveNotification(dto, listuser, GetUserName());
                }

                return Json(new
                {
                    success = true,
                    status = viewModel.Status,
                    message = "Saved Process Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    success = false,
                    status = viewModel.Status,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveInvoicing(TransportLostClaimDamageViewModel viewModel)
        {
            try
            {
                //if (String.IsNullOrEmpty(viewModel.InvoiceDate)) dto.InvoiceDate = null;
                //else dto.InvoiceDate = DateTime.ParseExact(viewModel.InvoiceDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                //if (String.IsNullOrEmpty(viewModel.VendorPaymentDate)) dto.VendorPaymentDate = null;
                //else dto.VendorPaymentDate = DateTime.ParseExact(viewModel.VendorPaymentDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var dto = Mapper.Map<TransportLostClaimDamageDTO>(viewModel);
                //var getValueVerification = _configurationbll.getValue("TransportLostDamageClaim", "Invoicing");
                //var listuser = getValueVerification[0].Split(';').ToList();

                UserSession currentLoginSession = Session["CurrentUser"] as UserSession;
                string loginName = "";
                if (currentLoginSession != null)
                {
                    loginName = currentLoginSession.Name;
                }
                dto.CreatedBy = loginName;

                _bll.SaveInvoicing(dto, GetUserId());

                if (viewModel.ClaimType == "Product Lost" && viewModel.VendorPaymentDate != null)
                {
                    dto.EDPSNumber = "0";
                    dto.EDPSDate = new DateTime(0001, 01, 01);
                    dto.IncinerationReportFile = "-";
                    dto.Status = "DISPOSE";

                    _bll.SaveDispose(dto, GetUserId());
                }

                if (dto.VendorPaymentDate != null && dto.VendorPaymentDate > new DateTime(0001, 01, 01))
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.DISPOSE.ToString();
                }
                else
                {
                    viewModel.RealStatus = TransportEnums.LostClaimProgress.INVOICING.ToString();
                }

                if (dto.VendorPaymentDate != null)
                {
                    dto.Status = TransportEnums.LostClaimProgress.INVOICING.ToString();
                    //_bll.SaveNotification(dto, listuser, GetUserName());
                }

                return Json(new
                {
                    success = true,
                    status = viewModel.Status,
                    message = "Saved Invoicing Successfully"
                }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    success = false,
                    status = viewModel.Status,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveDispose(TransportLostClaimDamageViewModel viewModel)
        {
            try
            {
                //if (String.IsNullOrEmpty(viewModel.EDPSDate)) dto.EDPSDate = null;
                //else dto.EDPSDate = DateTime.ParseExact(viewModel.EDPSDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                //if (String.IsNullOrEmpty(viewModel.IncinerationReportDate)) dto.IncinerationReportDate = null;
                //else dto.IncinerationReportDate = DateTime.ParseExact(viewModel.IncinerationReportDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                var dto = Mapper.Map<TransportLostClaimDamageDTO>(viewModel);
                //var getValueVerification = _configurationbll.getValue("TransportLostDamageClaim", "Dispose");
                //var listuser = getValueVerification[0].Split(';').ToList();

                UserSession currentLoginSession = Session["CurrentUser"] as UserSession;
                string loginName = "";
                if (currentLoginSession != null)
                {
                    loginName = currentLoginSession.Name;
                }
                dto.CreatedBy = loginName;
                dto.RealStatus = TransportEnums.LostClaimProgress.COMPLETED.ToString();
                _bll.SaveDispose(dto, GetUserId());

                dto.Status = TransportEnums.LostClaimProgress.DISPOSE.ToString();
                //_bll.SaveNotification(dto, listuser, GetUserName());

                return Json(new
                {
                    success = true,
                    status = viewModel.Status,
                    message = "Saved Dispose Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    success = false,
                    status = viewModel.Status,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Revoke(TransportLostClaimDamageViewModel viewModel)
        {
            try
            {
                var dto = Mapper.Map<TransportLostClaimDamageDTO>(viewModel);

                if (viewModel.Status == TransportEnums.LostClaimProgress.VERIFICATION.ToString())
                    _bll.RevokeVerification(dto, GetUserId());
                else if (viewModel.Status == TransportEnums.LostClaimProgress.PROCESS.ToString())
                    _bll.RevokeProcess(dto, GetUserId());
                else if (viewModel.Status == TransportEnums.LostClaimProgress.INVOICING.ToString())
                    _bll.RevokeInvoicing(dto, GetUserId());
                else if (viewModel.Status == TransportEnums.LostClaimProgress.DISPOSE.ToString())
                    _bll.RevokeDispose(dto, GetUserId());

                return Json(new
                {
                    success = true,
                    status = viewModel.Status,
                    message = "Revoke Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    success = false,
                    status = viewModel.Status,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Upload Marker
        public ActionResult PartialViewUploadBoxCondition1()
        {
            return View("UploadBoxCondition1");
        }

        public ActionResult PartialViewUploadBoxCondition2()
        {
            return View("UploadBoxCondition2");
        }

        public ActionResult PartialViewUploadBoxCondition3()
        {
            return View("UploadBoxCondition3");
        }

        public ActionResult PartialViewUploadPositionOnTruk1()
        {
            return View("UploadPositionOnTruk1");
        }

        public ActionResult PartialViewUploadPositionOnTruk2()
        {
            return View("UploadPositionOnTruk2");
        }

        public ActionResult PartialViewUploadPositionOnTruk3()
        {
            return View("UploadPositionOnTruk3");
        }

        public ActionResult PartialViewUploadFinalResult1()
        {
            return View("UploadFinalResult1");
        }

        public ActionResult PartialViewUploadFinalResult2()
        {
            return View("UploadFinalResult2");
        }

        public ActionResult PartialViewUploadFinalResult3()
        {
            return View("UploadFinalResult3");
        }
        #endregion

        #region Edit Marker
        public ActionResult EditPartialViewUploadBoxCondition1(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EdituploadBoxCondition1", viewModel);
        }

        public ActionResult EditPartialViewUploadBoxCondition2(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EdituploadBoxCondition2", viewModel);
        }

        public ActionResult EditPartialViewUploadBoxCondition3(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EdituploadBoxCondition3", viewModel);
        }

        public ActionResult EditPartialViewPositionOnTruck1(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EditPositionOnTruk1", viewModel);
        }

        public ActionResult EditPartialViewPositionOnTruck2(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EditPositionOnTruk2", viewModel);
        }

        public ActionResult EditPartialViewPositionOnTruck3(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EditPositionOnTruk3", viewModel);
        }

        public ActionResult EditPartialViewUploadFinalResult1(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EditFinalResult1", viewModel);
        }

        public ActionResult EditPartialViewUploadFinalResult2(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EditFinalResult2", viewModel);
        }

        public ActionResult EditPartialViewUploadFinalResult3(TransportLostClaimDamageViewModel viewModel)
        {
            return View("EditFinalResult3", viewModel);
        }
        #endregion

        [HttpPost]
        public JsonResult UploadPic()
        {
            var fileName = string.Empty;

            var sjNumber = string.Empty;
            var gpNumber = string.Empty;

            var filePath = string.Empty;
            var path = Server.MapPath("~/Assets/Uploads/TransportLostClaimDamage/");
            try
            {
                sjNumber = Request.Headers["X-SJNumber"];
                gpNumber = Request.Headers["X-GPNumber"];

                fileName = Request.Headers["X-File-Name-Pic"];


                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file

                    fileName = sjNumber + "_" + gpNumber + "_" + file.FileName;

                    //To save file, use SaveAs method
                    if (fileName.Contains("\\"))
                    {
                        fileName = fileName.Split('\\')[fileName.Split('\\').Length - 1];
                    }
                    file.SaveAs(path + fileName); //File will be saved in application root

                    //resize image
                    ResizeSettings resizeSetting = new ResizeSettings
                    {
                        Width = 400,
                        Height = 400,
                        Format = "jpg"
                    };
                    ImageBuilder.Current.Build(path + fileName, path + fileName, resizeSetting);
                }

                Response.StatusDescription = "/Assets/Uploads/TransportLostClaimDamage/" + fileName;
                return Json(new { success = true, picPath = path + fileName, message = "Upload Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.StatusDescription = ExceptionExtensions.GetFullMessage(ex);
                return Json(ExceptionExtensions.GetFullMessage(ex));
            }
        }

        public JsonResult LoadDataGrid(TransportLostClaimDamageInput inputParam)
        {
            var listResult = _bll.GetListLostClaimDamage(inputParam);
            var viewModel = Mapper.Map<List<TransportLostClaimDamageViewModel>>(listResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadGridDefault()
        {
            var listDBData = _bll.GetListDefault();
            var viewModel = Mapper.Map<List<TransportLostClaimDamageViewModel>>(listDBData);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListOriginWarehouse()
        {
            var listOriginWarehouse = _bll.GetDropDownOriginWarehouse();
            return Json(listOriginWarehouse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListDestWarehouse()
        {
            var listDestWarehouse = _bll.GetDropDownDestWarehouse();
            return Json(listDestWarehouse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListSTONumber(string stono)
        {
            return Json(_bll.GetTransportationNumberFilter(stono), JsonRequestBehavior.AllowGet);
        }
        /*
        public JsonResult GetListSTONumber()
        {
            var listSTONumber = _bll.GetDropDownSTONumber();
            return Json(listSTONumber, JsonRequestBehavior.AllowGet);
        }
        */

        public JsonResult GetListDeliveryNoteNumber()
        {
            var listDeliveryNoteNumber = _bll.GetDropDownDeliveryNoteNumber();
            return Json(listDeliveryNoteNumber, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListPoliceRegNumber()
        {
            var listPoliceRegNumber = _bll.GetDropDownPoliceRegNumber();
            return Json(listPoliceRegNumber, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListVendor()
        {
            var listVendor = _bll.GetDropDownVendor();
            return Json(listVendor, JsonRequestBehavior.AllowGet);
        }

        ////public JsonResult GetListClaimType()
        ////{
        ////    var listClaimType = _bll.GetDropDownClaimType();
        ////    return Json(listClaimType, JsonRequestBehavior.AllowGet);
        ////}

        public IList<SelectListItem> GetClaimTypeDropDown()
        {
            var listClaimType = _bll.GetDropDownClaimType();
            var selectLisItem = listClaimType.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
            selectLisItem.Insert(0, new SelectListItem { Text = "-- Please Select --", Value = "" });

            return selectLisItem;
        }

        public IList<SelectListItem> GetClaimExpenseDropDown()
        {
            var listClaimType = _bll.GetDropDownClaimExpense();
            var selectLisItem = listClaimType.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
            selectLisItem.Insert(0, new SelectListItem { Text = "-- Please Select --", Value = "" });

            return selectLisItem;
        }

        public IList<SelectListItem> GetClaimCategoryDropDown()
        {
            var listClaimType = _bll.GetDropDownClaimCategory();
            var selectLisItem = listClaimType.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
            selectLisItem.Insert(0, new SelectListItem { Text = "-- Please Select --", Value = "" });

            return selectLisItem;
        }

        public IList<SelectListItem> GetSJNumberDropDown()
        {
            var listSJNumber = _bll.GetDropDownSJNumber();
            var selectLisItem = listSJNumber.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
            selectLisItem.Insert(0, new SelectListItem { Text = "", Value = "" });

            return selectLisItem;
        }

        public JsonResult GetSJNumberAutoComplete()
        {
            var listSJNumber = _bll.GetDropDownSJNumber();

            return Json(listSJNumber, JsonRequestBehavior.AllowGet); ;
        }

        ////public IList<SelectListItem> GetGPNumberDropDown()
        ////{
        ////    var listGPNumber = _bll.GetDropDownGPNumber();
        ////    var selectLisItem = listGPNumber.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
        ////    selectLisItem.Insert(0, new SelectListItem { Text = "", Value = "" });

        ////    return selectLisItem;
        ////}

        public ActionResult Edit(string id)
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            var listCurrRoles = session.Role.Select(x => x.IDRole).ToList();
            ViewBag.ListCurrRoles = listCurrRoles;

            var gpNumber = id.Replace("~", "/").Split('$')[0];
            var sjNumber = id.Replace("~", "/").Split('$')[1];
            var dto = _bll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);
            var viewModel = Mapper.Map<TransportLostClaimDamageViewModel>(dto);

            //viewModel.BSWarehouseReceiveDate = dto.BSWarehouseReceiveDate == null ? "" : dto.BSWarehouseReceiveDate.ToString("dd-MMM-yyyy");
            //viewModel.HMSMemoDate = dto.HMSMemoDate == null ? "" : dto.HMSMemoDate.Value.ToString("dd/MM/yyyy");
            //viewModel.InvoiceDate = dto.InvoiceDate == null ? "" : dto.InvoiceDate.Value.ToString("dd/MM/yyyy");
            //viewModel.VendorPaymentDate = dto.VendorPaymentDate == null ? "" : dto.VendorPaymentDate.Value.ToString("dd/MM/yyyy");
            //viewModel.EDPSDate = dto.EDPSDate == null ? "" : dto.EDPSDate.Value.ToString("dd/MM/yyyy");
            //viewModel.IncinerationReportDate = dto.IncinerationReportDate == null ? "" : dto.IncinerationReportDate.Value.ToString("dd/MM/yyyy");

            if (dto.InvoiceAmount == null)
            {
                viewModel.InvoiceAmount = "";
            }
            else
            {
                viewModel.InvoiceAmount = (Convert.ToDouble(viewModel.InvoiceAmount)).ToString("0.#");
            }
            if (dto.InitClaimedAmount == null)
            {
                viewModel.InitClaimedAmount = "";
            }
            else
            {
                viewModel.InitClaimedAmount = (Convert.ToDouble(viewModel.InitClaimedAmount)).ToString("0.#");
            }
            if (dto.GrossLossAmount == null)
            {
                viewModel.GrossLossAmount = "";
            }
            else
            {
                viewModel.GrossLossAmount = (Convert.ToDouble(viewModel.GrossLossAmount)).ToString("0.#");
            }
            viewModel.NetClaim = (Convert.ToDouble(viewModel.NetClaim)).ToString("0.#");

            //foreach (var brand in dto.ListFACode)
            //{
            //    var brandViewModel = new TransportLostClaimFACodeViewModel();
            //    brandViewModel.Description = brand.Description;
            //    brandViewModel.FACode = brand.FACode;
            //    brandViewModel.LostOrDamage = brand.LostOrDamage;
            //    brandViewModel.Pack = brand.Pack;
            //    brandViewModel.SpeakingCode = brand.SpeakingCode;

            //    viewModel.ListFACode.Add(brandViewModel);
            //}

            viewModel.SJNumberOld = dto.SJNumber;
            viewModel.GPNumberOld = dto.GPNumber;

            viewModel.TransportationModeHidden = viewModel.TransportationMode;
            viewModel.InitClaimedAmountHidden = viewModel.InitClaimedAmount;
            viewModel.TotalClaimInPackHidden = viewModel.TotalClaimInPack;
            viewModel.TotalClaimInStickHidden = viewModel.TotalClaimInStick;

            viewModel.GPNumberHidden = viewModel.GPNumber;
            viewModel.SJNumberHidden = viewModel.SJNumber;

            if (viewModel.BSWarehouseReceiveDate == "01-Jan-0001") viewModel.BSWarehouseReceiveDate = "";
            if (viewModel.HMSMemoDate == "01-Jan-0001") viewModel.HMSMemoDate = "";
            if (viewModel.InvoiceDate == "01-Jan-0001") viewModel.InvoiceDate = "";
            if (viewModel.VendorPaymentDate == "01-Jan-0001") viewModel.VendorPaymentDate = "";
            if (viewModel.EDPSDate == "01-Jan-0001") viewModel.EDPSDate = "";
            if (viewModel.IncinerationReportDate == "01-Jan-0001") viewModel.IncinerationReportDate = "";

            viewModel.DateOfDelivery = (dto.DateOfDelivery > DateTime.MinValue) ? dto.DateOfDelivery.ToString("dd-MMM-yyyy") : "";
            viewModel.DateOfIncident = (dto.DateOfIncident > DateTime.MinValue) ? dto.DateOfIncident.ToString("dd-MMM-yyyy") : "";
            // https://stackoverflow.com/questions/33371527/no-overload-for-method-tostring-takes-1-arguments-when-casting-date
            viewModel.SJDate = (dto.SJDate.HasValue) ? dto.SJDate.Value.ToString("dd-MMM-yyyy") : "";

            viewModel.ClaimTypeDropDown = GetClaimTypeDropDown();
            viewModel.ClaimExpenseDropDown = GetClaimExpenseDropDown();
            viewModel.ClaimCategoryDropDown = GetClaimCategoryDropDown();

            if (GetListUserRole().Any())
            {
                ViewBag.ListCurrRoles = GetListUserRole().Select(c => c.RoleName).Distinct().ToList();
            }

            // add vendor name
            viewModel.TransportationVendorName = dto.TransportationVendorName;//viewModel.TransportationVendor = dto.TransportationVendor + " - " + dto.TransportationVendorName;
            viewModel.TransportationVendorHidden = dto.TransportationVendor.ToString();

            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            return View("Edit", viewModel);
        }

        [AllowAnonymous]
        public ActionResult PrintDocument(string id)
        {
            var gpNumber = id.Replace("~", "/").Split('-')[0];
            var sjNumber = id.Replace("~", "/").Split('-')[1];
            var dto = _bll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);

            var viewModel = Mapper.Map<TransportLostClaimDamageViewModel>(dto);

            //viewModel.BSWarehouseReceiveDate = dto.BSWarehouseReceiveDate == null ? "" : dto.BSWarehouseReceiveDate.Value.ToString("dd/MM/yyyy");
            //viewModel.HMSMemoDate = dto.HMSMemoDate == null ? "" : dto.HMSMemoDate.Value.ToString("dd/MM/yyyy");
            //viewModel.InvoiceDate = dto.InvoiceDate == null ? "" : dto.InvoiceDate.Value.ToString("dd/MM/yyyy");
            //viewModel.VendorPaymentDate = dto.VendorPaymentDate == null ? "" : dto.VendorPaymentDate.Value.ToString("dd/MM/yyyy");
            //viewModel.EDPSDate = dto.EDPSDate == null ? "" : dto.EDPSDate.Value.ToString("dd/MM/yyyy");
            //viewModel.IncinerationReportDate = dto.IncinerationReportDate == null ? "" : dto.IncinerationReportDate.Value.ToString("dd/MM/yyyy");

            viewModel.SJNumberOld = dto.SJNumber;
            viewModel.GPNumberOld = dto.GPNumber;

            if (!String.IsNullOrEmpty(viewModel.ImageUploadPositionTruckPath1))
            {
                string pathImgTruck1 = Server.MapPath(viewModel.ImageUploadPositionTruckPath1);
                byte[] imageByteData = System.IO.File.ReadAllBytes(pathImgTruck1);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                viewModel.ImageUploadPositionTruckPath1 = imageDataURL;
            }

            //return new Rotativa.MVC.PartialViewAsPdf("PrintDocument", viewModel) { FileName = viewModel.GPNumber + "_" + viewModel.SJNumber + "_" + DateTime.Now.ToShortDateString() + ".pdf" };
            //return PartialView("PrintDocument", viewModel);
            return View("PrintDocument", viewModel);
        }


        public ActionResult GetDocument(string strUrl, string id)
        {
            // instantiate a html to pdf converter object 

            HtmlToPdf converter = new HtmlToPdf();
            var username = GetUserName();
            string url = strUrl + "/PrintDocument/PrintDocumentLostClaim?id=" + id + "&uName=" + username;

            PdfDocument doc = converter.ConvertUrl(url);

            // save pdf document 
            byte[] pdf = doc.Save();

            // close pdf document 
            doc.Close();

            // return resulted pdf document 
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "TransportLostClaimDamage" + DateTime.Now.ToString("yyyyMMddHHmm") + ".pdf";
            return fileResult;
        }

        public JsonResult ExportToExcel(TransportLostClaimDamageInput inputParam)
        {
            IEnumerable<TransportLostClaimDamageDTO> listResult = null;

            if (inputParam.DateReceiptFrom == null && inputParam.DateReceiptTo == null
                && inputParam.OriginWarehouse == null && inputParam.DestinationWarehouse == null
                && inputParam.STONumber == null && inputParam.DeliveryNoteNumber == null
                && inputParam.PoliceRegNumber == null && inputParam.Vendor == null)
            {
                listResult = _bll.GetListDefault();
            }
            else
            {
                listResult = _bll.GetListLostClaimDamage(inputParam);
            }
            var viewModel = Mapper.Map<List<TransportLostClaimDamageViewModel>>(listResult);

            DataTable boundTable = new DataTable();
            boundTable.Columns.Add("Order Number/STO", typeof(string));
            boundTable.Columns.Add("Transportation Number", typeof(string));
            boundTable.Columns.Add("Origin", typeof(string));
            boundTable.Columns.Add("Destination", typeof(string));
            boundTable.Columns.Add("DNNumber", typeof(string));
            boundTable.Columns.Add("PoliceRegNumber", typeof(string));
            boundTable.Columns.Add("Vendor", typeof(string));

            foreach (var record in viewModel)
            {
                dynamic dr = boundTable.NewRow();

                dr["Order Number/STO"] = record.SJNumber;
                dr["Transportation Number"] = record.GPNumber;
                dr["Origin"] = record.OriginWarehouse;
                dr["Destination"] = record.DestinationWarehouse;
                dr["DNNumber"] = record.DeliveryNumberNote;
                dr["PoliceRegNumber"] = record.PoliceRegNumber;
                dr["Vendor"] = record.TransportationVendorName;
                boundTable.Rows.Add(dr);
            }

            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "TransportLostClaimDamage" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportToExcelCustom()
        {
            string columns = Request.Params["Columns"];
            string dateReceiptFrom = Request.Params["DateReceiptFrom"];
            string dateReceiptTo = Request.Params["DateReceiptTo"];
            string originWarehouse = Request.Params["OriginWarehouse"];
            string destinationWarehouse = Request.Params["DestinationWarehouse"];
            string sTONumber = Request.Params["STONumber"];
            string deliveryNoteNumber = Request.Params["DeliveryNoteNumber"];
            string policeRegNumber = Request.Params["PoliceRegNumber"];
            string vendor = Request.Params["Vendor"];

            bool brandRepeat = false;//penanda untuk pengulangan, kalau ada kolom brand untuk eksport

            var inputParam = new TransportLostClaimDamageInput();
            inputParam.DateReceiptFrom = dateReceiptFrom;
            inputParam.DateReceiptTo = dateReceiptTo;
            inputParam.OriginWarehouse = originWarehouse;
            inputParam.DestinationWarehouse = destinationWarehouse;
            inputParam.STONumber = sTONumber;
            inputParam.DeliveryNoteNumber = deliveryNoteNumber;
            inputParam.PoliceRegNumber = policeRegNumber;
            inputParam.Vendor = vendor;

            var viewModel = new List<TransportLostClaimDamageViewModel>();

            if ((inputParam.DateReceiptFrom == null || inputParam.DateReceiptFrom == "") &&
                (inputParam.DateReceiptTo == null || inputParam.DateReceiptTo == "") &&
                (inputParam.OriginWarehouse == null || inputParam.OriginWarehouse == "") &&
                (inputParam.DestinationWarehouse == null || inputParam.DestinationWarehouse == "") &&
                (inputParam.STONumber == null || inputParam.STONumber == "") &&
                (inputParam.DeliveryNoteNumber == null || inputParam.DeliveryNoteNumber == "") &&
                (inputParam.PoliceRegNumber == null || inputParam.PoliceRegNumber == "") &&
                (inputParam.Vendor == null || inputParam.Vendor == ""))
            {
                var listResult = _bll.GetListDefault();
                viewModel = Mapper.Map<List<TransportLostClaimDamageViewModel>>(listResult);
            }
            else
            {
                var listResult = _bll.GetListLostClaimDamage(inputParam);
                viewModel = Mapper.Map<List<TransportLostClaimDamageViewModel>>(listResult);
            }


            DataTable boundTable = new DataTable();

            if (string.IsNullOrEmpty(columns))
            {
                columns = "Origin,Destination,STO Number,DN Number,Police Reg. Number,Vendor Name,GPNumber,Status,Claim Type,Delivery Date,Incident Date,Driver 1,Driver 2,CoDriver,Receiver,Witness,Claim Expense,DN TO Reserve Logistics,Supervisor,Transportation Mode,Attachment,Material Code,Material Description,Category,Pack,Remark,Goods Category,Initial Claimed mount (IDR),Claim Category,Total Claim In Pack,Total Claim In Stick,BS Warehouse Receive Date,HMS Memo Number,HMS Memo Date,HMS Memo File,Invoice Number,Invoice Date,Invoice Amount,Vendor Payment Date,Gross Loss Amount,Net Claim (IDR),Insurance Payment Proof,eDPS Number,eDPS Date,Incineration eport Date,Incineration Report File,Remarks";
            }

            string[] Fields = columns.Split(',');
            foreach (string Field in Fields)
            {
                if (Field == "Material Code" || Field == "Material Description" || Field == "Category" || Field == "Pack" || Field == "Remark")
                    brandRepeat = true;
                boundTable.Columns.Add(Field, typeof(string));
            }

            foreach (var record in viewModel)
            {
                var dict = new Dictionary<string, string>();
                string dateOfReceipt = "";
                string deliveryDate = "";
                string incidentDate = "";
                string bsWarehouseReceiveDate = "";
                string hmsMemoDate = "";
                string invoiceDate = "";
                string vendorPaymentDate = "";
                string edpsDate = "";
                string incinerationReportDate = "";
                if (record.SJDate != "01-Jan-0001")
                    dateOfReceipt = record.SJDate;
                if (record.DateOfDelivery != "01-Jan-0001")
                    deliveryDate = record.DateOfDelivery;
                if (record.DateOfIncident != "01-Jan-0001")
                    incidentDate = record.DateOfIncident;
                if (record.BSWarehouseReceiveDate != "01-Jan-0001")
                    bsWarehouseReceiveDate = record.BSWarehouseReceiveDate;
                if (record.HMSMemoDate != "01-Jan-0001")
                    hmsMemoDate = record.HMSMemoDate;
                if (record.InvoiceDate != "01-Jan-0001")
                    invoiceDate = record.InvoiceDate;
                if (record.VendorPaymentDate != "01-Jan-0001")
                    vendorPaymentDate = record.VendorPaymentDate;
                if (record.EDPSDate != "01-Jan-0001")
                    edpsDate = record.EDPSDate;
                if (record.IncinerationReportDate != "01-Jan-0001")
                    incinerationReportDate = record.IncinerationReportDate;
                //dict["Date of Receipt"] = dateOfReceipt;
                dict["Origin"] = record.OriginWarehouse;
                dict["Destination"] = record.DestinationWarehouse;
                dict["Order Number/STO"] = record.SJNumber;
                dict["DN Number"] = record.DeliveryNumberNote;
                dict["Police Reg. Number"] = record.PoliceRegNumber;
                dict["Vendor Name"] = record.TransportationVendorName;
                dict["Transportation Number"] = record.GPNumber;
                //dict["SJNumber"] = record.SJNumber;
                dict["Status"] = record.RealStatus;
                dict["Claim Type"] = record.ClaimType;
                dict["Delivery Date"] = deliveryDate;
                dict["Incident Date"] = incidentDate;
                dict["Driver 1"] = record.Driver;
                dict["Driver 2"] = record.Driver2;
                dict["CoDriver"] = record.CoDriver;
                dict["Receiver"] = record.Receiver;
                dict["Witness"] = record.Witness;
                dict["Claim Expense"] = record.ClaimExpense;
                //dict["DN TO Reserve Logistics"] = record.DNtoReverseLogistics;
                dict["Supervisor"] = record.Supervisor;
                dict["Transportation Mode"] = record.TransportationMode;
                dict["Attachment"] = record.Attachment;
                dict["Goods Category"] = record.GoodsCategory;
                dict["Initial Claimed Amount (IDR)"] = record.InitClaimedAmount;
                dict["Claim Category"] = record.ClaimCategory;
                dict["Total Claim In Pack"] = record.TotalClaimInPack;
                dict["Total Claim In Stick"] = record.TotalClaimInStick;
                dict["BS Warehouse Receive Date"] = bsWarehouseReceiveDate;
                dict["HMS Memo Number"] = record.HMSMemoNumber;
                dict["HMS Memo Date"] = hmsMemoDate;
                dict["HMS Memo File"] = record.HMSMemoFile;
                dict["Invoice Number"] = record.InvoiceNumber;
                dict["Invoice Date"] = invoiceDate;
                dict["Invoice Amount"] = record.InvoiceAmount;
                dict["Vendor Payment Date"] = vendorPaymentDate;
                dict["Gross Loss Amount"] = record.GrossLossAmount;
                dict["Net Claim (IDR)"] = record.NetClaim;
                dict["Insurance Payment Proof"] = record.InsurancePaymentProof;
                dict["eDPS Number"] = record.EDPSNumber;
                dict["eDPS Date"] = edpsDate;
                dict["Incineration Report Date"] = incinerationReportDate;
                dict["Incineration Report File"] = record.IncinerationReportFile;
                dict["Remarks"] = record.Remarks;

                dynamic dr = boundTable.NewRow();

                if (brandRepeat)
                {
                    List<TransportLostClaimFACode> tempFACodeList = _bll.GetTransporftLostClaimFACodeBySTO(record.GPNumber, record.SJNumber);
                    if (tempFACodeList.Count > 0)
                    {
                        foreach (TransportLostClaimFACode tempFACode in tempFACodeList)
                        {
                            dr = boundTable.NewRow();
                            dict["Material Code"] = tempFACode.FACode;
                            dict["Material Description"] = tempFACode.SpeakingCode;
                            dict["Category"] = tempFACode.LostOrDamage;
                            dict["Pack"] = tempFACode.Pack.ToString();
                            dict["Remark"] = tempFACode.Description;
                            foreach (string Field in Fields)
                            {
                                dr[Field] = dict[Field];
                            }
                            boundTable.Rows.Add(dr);
                        }
                    }
                    else {
                        foreach (string Field in Fields)
                        {
                            dr[Field] = dict[Field];
                        }
                        boundTable.Rows.Add(dr);
                    }
                }
                else {
                    foreach (string Field in Fields)
                    {
                        dr[Field] = dict[Field];
                    }
                    boundTable.Rows.Add(dr);
                }
            }

            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "TransportLostClaimDamage" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            return Json(path, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetAddNewDetail(string id)
        {
            try
            {
                var dto = _bll.GetAddNewDetail(id.Replace("~", "/"));

                var viewModel = Mapper.Map<TransportLostClaimDamageViewModel>(dto);

                if (String.IsNullOrEmpty(viewModel.GPNumber))
                {
                    viewModel.SJDate = null;
                    viewModel.DateOfDelivery = null;
                }
                if (dto.GPNumber != null)
                {
                    viewModel.Success = true;
                    return Json(viewModel, JsonRequestBehavior.AllowGet);
                }

                viewModel.Success = false;
                return Json(new
                {
                    Success = false,
                    message = "Unavailable Order Number reference"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    Success = false,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSpeakingCode(string id)
        {
            try
            {
                var speakingCode = _bll.GetSpeakingCode(id.Replace("~", "."));

                return Json(speakingCode, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    Success = false,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult DownloadAttachment(string id)
        {
            var gpNumber = id.Replace("~", "/").Split('$')[0];
            var sjNumber = id.Replace("~", "/").Split('$')[1];

            var data = _bll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);

            string fullPathFile = string.Empty;

            if (data != null)
            {
                fullPathFile = "~/Assets/Uploads/TransportLostClaimDamage/" + data.Attachment;
            }

            var FileVirtualPath = fullPathFile;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public FileResult DownloadMemoFile(string id)
        {
            var gpNumber = id.Replace("~", "/").Split('$')[0];
            var sjNumber = id.Replace("~", "/").Split('$')[1];

            var data = _bll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);

            string fullPathFile = string.Empty;

            if (data != null)
            {
                fullPathFile = "~/Assets/Uploads/TransportLostClaimDamage/" + data.HMSMemoFile;
            }

            var FileVirtualPath = fullPathFile;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public FileResult DownloadPaymentProof(string id)
        {
            var gpNumber = id.Replace("~", "/").Split('$')[0];
            var sjNumber = id.Replace("~", "/").Split('$')[1];

            var data = _bll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);

            string fullPathFile = string.Empty;

            if (data != null)
            {
                fullPathFile = "~/Assets/Uploads/TransportLostClaimDamage/" + data.InsurancePaymentProof;
            }

            var FileVirtualPath = fullPathFile;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public FileResult DownloadIncinerationFile(string id)
        {
            var gpNumber = id.Replace("~", "/").Split('$')[0];
            var sjNumber = id.Replace("~", "/").Split('$')[1];

            var data = _bll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);

            string fullPathFile = string.Empty;

            if (data != null)
            {
                fullPathFile = "~/Assets/Uploads/TransportLostClaimDamage/" + data.IncinerationReportFile;
            }

            var FileVirtualPath = fullPathFile;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        [HttpPost]
        public JsonResult CheckIncinerationFile(string id)
        {
            var gpNumber = id.Replace("~", "/").Split('$')[0];
            var sjNumber = id.Replace("~", "/").Split('$')[1];

            var data = _bll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);

            string fullPathFile = string.Empty;
            var isExist = "";
            
            if (data != null)
            {
                //var filewithoutext = Path.GetFileNameWithoutExtension(data.IncinerationReportFile);
                //fullPathFile = ConfigurationManager.AppSettings["LocationLostClaim"] + data.IncinerationReportFile;
                fullPathFile = "~/Assets/Uploads/TransportLostClaimDamage/" + data.IncinerationReportFile;

                if (System.IO.File.Exists(Server.MapPath(fullPathFile)))
                {
                    isExist = "Y";
                }
                else
                {
                    isExist = "N";
                }
            }


            var res = new { isExist = isExist, file = data.IncinerationReportFile };
            return Json(res);
        }

        public JsonResult RemoveAttachment(string id)
        {
            try
            {
                var gpNumber = id.Replace("~", "/").Split('$')[0];
                var sjNumber = id.Replace("~", "/").Split('$')[1];

                _bll.RemoveAttachment(gpNumber, sjNumber);

                return Json(new { success = true, message = "Remove Attachment Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    Success = false,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveMemoFile(string id)
        {
            try
            {
                var gpNumber = id.Replace("~", "/").Split('$')[0];
                var sjNumber = id.Replace("~", "/").Split('$')[1];

                _bll.RemoveMemoFile(gpNumber, sjNumber);

                return Json(new { success = true, message = "Remove Memo File Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    Success = false,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemovePaymentProof(string id)
        {
            try
            {
                var gpNumber = id.Replace("~", "/").Split('$')[0];
                var sjNumber = id.Replace("~", "/").Split('$')[1];

                _bll.RemovePaymentProof(gpNumber, sjNumber);

                return Json(new { success = true, message = "Remove Memo File Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    Success = false,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveIncinerationFile(string id)
        {
            try
            {
                var gpNumber = id.Replace("~", "/").Split('$')[0];
                var sjNumber = id.Replace("~", "/").Split('$')[1];

                _bll.RemoveIncinerationReportFile(gpNumber, sjNumber);

                return Json(new { success = true, message = "Remove Memo File Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msgError = ExceptionExtensions.GetFullMessage(ex);
                return Json(new
                {
                    Success = false,
                    message = msgError
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TestEmptyAction()
        {
            return Json(new
            {
                success = true,
                message = "Successfully"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadTransport()
        {
            var context = new TOMContextDB();
            var userId = GetUserId();
            var role = context.MasterUserRoleMappings
                .Where(n => n.IDUser == userId)
                .Select(n => n.IDRole).ToList();

            string hasil = "";
            string hasilConflict = "Data error in rows - ";
            DataSet result = null;
            Excel.IExcelDataReader reader = null;
            Console.WriteLine(this.Request.Files.Count);
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
                        string message = "This file format is not supported";
                    }

                    var count = 0;
                    var flag = 0;
                    reader.IsFirstRowAsColumnNames = true;
                    result = reader.AsDataSet();
                    var DataTable = result.Tables[0];
                    reader.Close();

                    //remove all the empty row from dataTable so that dataTable.rows.count can get the exact value
                    //DataTable = DataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
                    //https://stackoverflow.com/questions/7023140/how-to-remove-empty-rows-from-datatable

                    var sheetName = reader.Name;

                    var temp = result.Tables.Count;
                    if (result.Tables.Count > 0)
                    {
                        var inputs = new TransportLostClaimDamageDTO();

                        if (sheetName == "Process")
                        {
                            var b = 0;
                            //DATA CHECKING PROCESS
                            for (var i = 0; i < DataTable.Rows.Count; i++)
                            {
                                b = DataTable.Rows.Count;
                                inputs.SJNumber = DataTable.Rows[i][0].ToString();
                                if (!string.IsNullOrEmpty(DataTable.Rows[i][1].ToString()))
                                {
                                    try { inputs.BSWarehouseReceiveDate = Convert.ToDateTime(DataTable.Rows[i][1] + " 00:00"); }
                                    catch { inputs.BSWarehouseReceiveDate = Convert.ToDateTime(DataTable.Rows[i][1]); }
                                }
                                else { inputs.BSWarehouseReceiveDate = null; }
                                inputs.HMSMemoNumber = DataTable.Rows[i][2].ToString();
                                if (!string.IsNullOrEmpty(DataTable.Rows[i][1].ToString()))
                                {
                                    try { inputs.HMSMemoDate = Convert.ToDateTime(DataTable.Rows[i][3] + " 00:00"); }
                                    catch { inputs.HMSMemoDate = Convert.ToDateTime(DataTable.Rows[i][3]); }
                                }
                                else { inputs.HMSMemoDate = null; }

                                if (inputs.SJNumber == "" && inputs.HMSMemoDate.ToString() == "" && inputs.HMSMemoNumber == "" && inputs.BSWarehouseReceiveDate.ToString() == "")
                                {
                                    b = i;
                                    break;
                                }

                                if ((inputs.SJNumber == "") && (inputs.HMSMemoDate != null || inputs.HMSMemoNumber != "" || inputs.BSWarehouseReceiveDate != null))
                                {
                                    hasilConflict += (i + 1).ToString() + ", ";
                                    flag = 1;
                                }
                                else
                                {
                                    var checkDataUpload = _bll.checkDataUpload(inputs, sheetName);

                                    if (checkDataUpload == "conflict")
                                    {
                                        flag = 1;
                                        hasilConflict += (i + 1).ToString() + ", ";
                                    }
                                }

                            }

                            //UPDATE PROCESS
                            if (flag == 0 && b != 0)
                            {
                                for (var j = 0; j < b; j++)
                                {
                                    inputs.SJNumber = DataTable.Rows[j][0].ToString();
                                    try { inputs.BSWarehouseReceiveDate = Convert.ToDateTime(DataTable.Rows[j][1] + " 00:00"); }
                                    catch { inputs.BSWarehouseReceiveDate = Convert.ToDateTime(DataTable.Rows[j][1]); }
                                    inputs.HMSMemoNumber = DataTable.Rows[j][2].ToString();
                                    try { inputs.HMSMemoDate = Convert.ToDateTime(DataTable.Rows[j][3] + " 00:00"); }
                                    catch { inputs.HMSMemoDate = Convert.ToDateTime(DataTable.Rows[j][3]); }

                                    var UpdateDataTransport = _bll.UpdateDataTransport(inputs, sheetName);

                                    if (UpdateDataTransport != null)
                                    {
                                        count++;
                                    }
                                }
                            }

                            if (b == 0)
                            {
                                hasil = "Empty";
                            }

                            if (flag == 1)
                            {
                                hasil = hasilConflict;
                            }
                            else if (flag == 0 && count > 0)
                            {
                                hasil = count.ToString();
                            }
                        }
                        else if (sheetName == "Invoicing")
                        {
                            var b = 0;
                            //DATA CHECKING INVOICING
                            for (var i = 0; i < DataTable.Rows.Count; i++)
                            {
                                b = DataTable.Rows.Count;
                                inputs.SJNumber = DataTable.Rows[i][0].ToString();
                                inputs.InvoiceNumber = DataTable.Rows[i][1].ToString();
                                if (!string.IsNullOrEmpty(DataTable.Rows[i][2].ToString()))
                                {
                                    try { inputs.InvoiceDate = Convert.ToDateTime(DataTable.Rows[i][2] + " 00:00"); }
                                    catch { inputs.InvoiceDate = Convert.ToDateTime(DataTable.Rows[i][2]); }
                                }
                                else { inputs.InvoiceDate = null; }
                                if (!(string.IsNullOrEmpty(DataTable.Rows[i][3].ToString())))
                                {
                                    inputs.InvoiceAmount = Convert.ToDecimal(DataTable.Rows[i][3]);
                                }
                                else { inputs.InvoiceAmount = 0; }
                                if (!string.IsNullOrEmpty(DataTable.Rows[i][4].ToString()))
                                {
                                    try { inputs.VendorPaymentDate = Convert.ToDateTime(DataTable.Rows[i][4] + " 00:00"); }
                                    catch { inputs.VendorPaymentDate = Convert.ToDateTime(DataTable.Rows[i][4]); }
                                }
                                else { inputs.VendorPaymentDate = null; }

                                if (inputs.SJNumber == "" && inputs.InvoiceNumber == "" && inputs.InvoiceDate == null && inputs.InvoiceAmount == 0 && inputs.VendorPaymentDate == null)
                                {
                                    b = i;
                                    break;
                                }

                                if (inputs.SJNumber == "" && (inputs.InvoiceNumber != "" && inputs.InvoiceDate != null && inputs.InvoiceAmount.ToString() != "" && inputs.VendorPaymentDate != null))
                                {
                                    hasilConflict += (i + 1).ToString() + ", ";
                                    flag = 1;
                                }
                                else
                                {
                                    var checkDataUpload = _bll.checkDataUpload(inputs, sheetName);

                                    if (checkDataUpload == "conflict")
                                    {
                                        flag = 1;
                                        hasilConflict += (i + 1).ToString() + ", ";
                                    }
                                }
                            }

                            //UPDATE INVOICING
                            if (flag == 0 && b != 0)
                            {
                                for (var j = 0; j < b; j++)
                                {
                                    inputs.SJNumber = DataTable.Rows[j][0].ToString();
                                    inputs.InvoiceNumber = DataTable.Rows[j][1].ToString();
                                    try { inputs.InvoiceDate = Convert.ToDateTime(DataTable.Rows[j][2] + " 00:00"); }
                                    catch { inputs.InvoiceDate = Convert.ToDateTime(DataTable.Rows[j][2]); }
                                    if (!(string.IsNullOrEmpty(DataTable.Rows[j][3].ToString())))
                                    {
                                        inputs.InvoiceAmount = Convert.ToDecimal(DataTable.Rows[j][3]);
                                    }
                                    else { inputs.InvoiceAmount = 0; }
                                    try { inputs.VendorPaymentDate = Convert.ToDateTime(DataTable.Rows[j][4] + " 00:00"); }
                                    catch { inputs.VendorPaymentDate = Convert.ToDateTime(DataTable.Rows[j][4]); }

                                    var UpdateDataTransport = _bll.UpdateDataTransport(inputs, sheetName);

                                    if (UpdateDataTransport != null)
                                    {
                                        count++;
                                    }
                                }
                            }

                            if (b == 0)
                            {
                                hasil = "Empty";
                            }

                            if (flag == 1)
                            {
                                hasil = hasilConflict;
                            }
                            else if (count > 0)
                            {
                                hasil = count.ToString();
                            }
                        }
                        else if (sheetName == "Dispose")
                        {
                            var b = 0;
                            //DATA CHECKING DISPOSE
                            for (var i = 0; i < DataTable.Rows.Count; i++)
                            {
                                b = DataTable.Rows.Count;
                                inputs.SJNumber = DataTable.Rows[i][0].ToString();
                                inputs.EDPSNumber = DataTable.Rows[i][1].ToString();
                                if (!string.IsNullOrEmpty(DataTable.Rows[i][2].ToString()))
                                {
                                    try { inputs.EDPSDate = Convert.ToDateTime(DataTable.Rows[i][2] + " 00:00"); }
                                    catch { inputs.EDPSDate = Convert.ToDateTime(DataTable.Rows[i][2]); }
                                }
                                else { inputs.EDPSDate = null; }
                                if (!string.IsNullOrEmpty(DataTable.Rows[i][3].ToString()))
                                {
                                    try { inputs.IncinerationReportDate = Convert.ToDateTime(DataTable.Rows[i][3] + " 00:00"); }
                                    catch { inputs.IncinerationReportDate = Convert.ToDateTime(DataTable.Rows[i][3]); }
                                }
                                else { inputs.IncinerationReportDate = null; }

                                if (inputs.SJNumber == "" && inputs.EDPSNumber == "" && inputs.EDPSDate == null && inputs.IncinerationReportDate == null)
                                {
                                    b = i;
                                    break;
                                }

                                if (inputs.SJNumber == "" && (inputs.EDPSNumber != "" && inputs.EDPSDate != null && inputs.IncinerationReportDate != null))
                                {
                                    hasilConflict += (i + 1).ToString() + ", ";
                                    flag = 1;
                                }
                                else
                                {
                                    var checkDataUpload = _bll.checkDataUpload(inputs, sheetName);

                                    if (checkDataUpload == "conflict")
                                    {
                                        flag = 1;
                                        hasilConflict += (i + 1).ToString() + ", ";
                                    }
                                }
                            }

                            //UPDATE DISPOSE
                            if (flag == 0 && b != 0)
                            {
                                for (var j = 0; j < b; j++)
                                {
                                    inputs.SJNumber = DataTable.Rows[j][0].ToString();
                                    inputs.EDPSNumber = DataTable.Rows[j][1].ToString();
                                    try { inputs.EDPSDate = Convert.ToDateTime(DataTable.Rows[j][2] + " 00:00"); }
                                    catch { inputs.EDPSDate = Convert.ToDateTime(DataTable.Rows[j][2]); }
                                    try { inputs.IncinerationReportDate = Convert.ToDateTime(DataTable.Rows[j][3] + " 00:00"); }
                                    catch { inputs.IncinerationReportDate = Convert.ToDateTime(DataTable.Rows[j][3]); }

                                    var UpdateDataTransport = _bll.UpdateDataTransport(inputs, sheetName);

                                    if (UpdateDataTransport != null)
                                    {
                                        count++;
                                    }
                                }
                            }

                            if (b == 0)
                            {
                                hasil = "Empty";
                            }

                            if (flag == 1)
                            {
                                hasil = hasilConflict;
                            }
                            else if (count > 0)
                            {
                                hasil = count.ToString();
                            }
                        }
                        else
                        {
                            hasil = "No Sheet";
                        }

                        var HasilUpdateMemo = UpdateCloneFile();
                    }
                    else
                    {
                        hasil = "Empty";
                    }
                }
                else
                {
                    hasil = "Empty";
                }
            }

            return Json(hasil, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateCloneFile()
        {
            var result = "";

            try
            {
                _bll.UpdateCloneFile(Server.MapPath("~/Assets/Uploads/TransportLostClaimDamage"));
                result = "Berhasil";

            }
            catch (Exception ex)
            {
                result = "Error tjuy";
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

    }
}