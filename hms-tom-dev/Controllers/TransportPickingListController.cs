using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using TOM.Master.Repositories;
using System.Data;
using System.Web;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.Outputs;
using hms_tom_dev.Models.Transport;
using hms_tom_dev.Util;
using TOM.Transport.BusinessLogics;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using DFIS.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using hms_tom_dev.Helper;
using TOM.Master.BusinessLogics;
using System.Drawing;
using DFIS.Universal.Domain.DTOs;
using TOM.EntitiesDAL.EDMX;

namespace hms_tom_dev.Controllers
{
    public class TransportPickingListController : BaseController
    {
        private ITransportPickingListBLL _transportPickingListBll;
        private IMasterLocationBLL _locBll;
        private ITransportOrderBLL _trOrderBll;
        private static List<TransportOrderViewModel> TransportOrderList = new List<TransportOrderViewModel>();
        private static List<MasterLocationDTO> listLocation = new List<MasterLocationDTO>();
        private IMasterGenWeekRepo _masterGenWeekRepo;
        private IMasterUomRepo _masterUoMRepo;

        public TransportPickingListController(IMasterLocationBLL locBll, ITransportPickingListBLL transportPickingListBll, IMasterUomRepo masterUoMRepo, ITransportOrderBLL trOrderBl, IMasterGenWeekRepo masterGenWeekRepo)
        {
            _trOrderBll = trOrderBl;
            _transportPickingListBll = transportPickingListBll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportPickingList));
            _masterUoMRepo = masterUoMRepo;
            _masterGenWeekRepo = masterGenWeekRepo;
            _locBll = locBll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportPickingList));
        }

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            var viewModel = new TransportOrderViewModel();
            List<MasterListDTO> tempMasterList = _transportPickingListBll.getMasterList();
            viewModel.weekNow = _transportPickingListBll.GetWeekNow().Week;
            viewModel.yearNow = DateTime.Now.Year;
            /*List<SelectListItem> tempZoneList = _transportPickingListBll.GetZoneList().Select(a => new SelectListItem { Text = a.LocationName, Value = a.IDLocation }).ToList();
            viewModel.zoneList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.zoneList.AddRange(tempZoneList);*/
            List<SelectListItem> tempZoneList = tempMasterList.Where(x => x.FieldName == "Zone").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.zoneList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.zoneList.AddRange(tempZoneList);
            List<SelectListItem> tempSenderReceiverList = _transportPickingListBll.GetSenderReceiverList().Select(a => new SelectListItem { Text = a.LocationName, Value = a.IDLocation }).ToList();
            viewModel.senderList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.senderList.AddRange(tempSenderReceiverList);
            viewModel.receiveList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.receiveList.AddRange(tempSenderReceiverList);
            TransportOrderInput criteria = new TransportOrderInput();
            criteria.weekFilter = (int)viewModel.weekNow;
            criteria.yearFilter = viewModel.yearNow;
            //var listUploadedBy = _transportPickingListBll.GetAllUploadedBy(criteria);
            //List<SelectListItem> tempUploadedByList = _transportPickingListBll.GetAllUploadedBy(criteria).Select(x => new SelectListItem { Text = x, Value = x }).ToList();
            //viewModel.UploadedBy = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            //viewModel.UploadedBy.AddRange(tempUploadedByList);
            return View(viewModel);
        }

        public ViewResult Edit(string stono = null)
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            var temp = _transportPickingListBll.SelectTransportOrderByStoNo(stono);
            TransportOrderViewModel viewModel = Mapper.Map<TransportOrderDTO, TransportOrderViewModel>(temp);
            viewModel.VehicleTypeList = temp.MasterList.Select(
                a => new SelectListItem
                {
                    Text = a.FieldValue,
                    Value = a.FieldValue
                });
            viewModel.UoMList = _masterUoMRepo.GetMasterUomByMaterial("Cigarette").Select(
                a => new SelectListItem
                {
                    Text = a.UoM,
                    Value = a.UoM
                });
            var t = _transportPickingListBll.getMasterList("Supplier");
            var tx = t.Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            tx.Insert(0, new SelectListItem { Text = "", Value = "" });
            viewModel.SupplierList = tx;

            //foreach(var item in viewModel.TransportOrderDetails)
            //{
            //    item.UoMList = _masterUoMRepo.GetMasterUomByMaterial(item.MaterialType).Select(
            //    a => new SelectListItem
            //    {
            //        Text = a.UoM,
            //        Value = a.UoM
            //    });
            //}
            return View("Edit", viewModel);
        }

        public ActionResult GetStoNoFilter(string term)
        {
            var res = _transportPickingListBll.GetSTONoFilter(term).Select(o => o.STONo).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
            //return 'aa';
        }

        [HttpPost]
        public ActionResult GetUploadedBy(TransportOrderInput criteria)
        {
            Dictionary<string, string> user = new Dictionary<string, string>();
            user.Add("", "");
            var listUploadedBy = _transportPickingListBll.GetAllUploadedBy(criteria);
            foreach (var ub in listUploadedBy)
            {
                user.Add(ub.ToString(), _transportPickingListBll.getUserFullName(ub).ToString());
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTransportPickingList(TransportOrderInput criteria)
        {
            /*
            TransportOrderList = Mapper.Map<List<TransportOrderDTO>, List<TransportOrderViewModel>>(_transportPickingListBll.GetALLPickingList(criteria));
            listLocation = _transportPickingListBll.getAllMasterLocation().ToList();
            foreach (var item in TransportOrderList)
            {
                foreach (var detailItem in item.TransportOrderDetails)
                {
                    if (detailItem.TransportOrderDetailChangeLogs.Any(x => x.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.UoM)))
                    {
                        detailItem.UoMColor = "Yellow";
                    }
                    else
                    {
                        detailItem.UoMColor = null;
                    }

                    if (detailItem.TransportOrderDetailChangeLogs.Any(x => x.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Quantity)))
                    {
                        detailItem.QtyColor = "Yellow";
                    }
                    else
                    {
                        detailItem.QtyColor = null;
                    }
                }
            }
            */
            return Json("Sukses");
        }

        public ActionResult GetTransportPickingListFilterDay(string hari, TransportOrderInput criteria)
        {
            var ctx = new TOM.EntitiesDAL.EDMX.TOMContextDB();
            criteria.DayName = hari;
            List<TransportOrderViewModel> viewModelList = Mapper.Map<List<TransportOrderDTO>, List<TransportOrderViewModel>>(_transportPickingListBll.GetALLPickingList(criteria)).OrderBy(x => x.SenderIDLocation).ThenBy(x => x.SeqNo).ToList();
            //List<TransportOrderViewModel> viewModelList = TransportOrderList.Where(x => x.IsActive && x.weekDayNameShipmentDateEnglish == hari && x.IDTransportPickingListLog != null).OrderBy(x => x.SenderIDLocation).ThenBy(x => x.SeqNo).ToList();
            var locationDC = ctx.MasterLocations
                             .Where(x => x.LocationName.Contains("DC"))
                             .ToList();
            //listLocation = _transportPickingListBll.getAllMasterLocation().ToList();

            List<PickingListTotalUnit> totalUnit = new List<PickingListTotalUnit>();
            foreach (var loc in locationDC)
            {
                var pickingList = new PickingListTotalUnit();
                if (viewModelList.Where(x => x.SenderIDLocation == loc.IDLocation).Count() > 0)
                {
                    pickingList.SenderIDLocation = loc.LocationName;
                    pickingList.Total = viewModelList.Where(x => x.SenderIDLocation == loc.IDLocation && x.DefaultSeqNo != null).Select(x => x.DefaultSeqNo).Distinct().Count();
                    totalUnit.Add(pickingList);
                }
            }
            //var viewModelListOrdered = viewModelList.Where(x => x.MasterLocation1.LocationName.Contains("DC")).GroupBy(x => x.SenderIDLocation);
            //foreach(var pickingListItem in viewModelListOrdered)
            //{
            //    var pickingList = new PickingListTotalUnit();
            //    pickingList.SenderIDLocation = pickingListItem.First().SenderIDLocation;
            //    var prevItem = "";
            //    foreach(var item in pickingListItem.OrderBy(x => x.SeqNo))
            //    {
            //        if (string.IsNullOrEmpty(prevItem))
            //        {
            //            pickingList.Total += 1;
            //            prevItem = item.SeqNo;
            //        }
            //        else if (item.SeqNo != prevItem)
            //        {
            //            pickingList.Total += 1;
            //            prevItem = item.SeqNo;
            //        }
            //    }
            //    totalUnit.Add(pickingList);
            //}

            viewModelList = viewModelList
                .OrderBy(x => x.SeqNo != null ? Int64.Parse(x.SeqNo) : 999999999999999)
                .ThenBy(x => x.DefaultSeqNo != null ? Int64.Parse(x.DefaultSeqNo) : 999999999999999)
                .ThenBy(x => x.ReceiverLocationName)
                .ThenBy(x => x.SenderLocationName)
                .ToList();

            //var temp = new { viewModel = viewModelList, totUnit = totalUnit };
            
            var tmpViewModel = from v in viewModelList
                      join l1 in ctx.MasterLocations on v.SenderIDLocation equals l1.IDLocation into sender
                      from l1 in sender.DefaultIfEmpty()                      
                      join l2 in ctx.MasterLocations on v.ActualSenderIDLocation equals l2.IDLocation into actualsender
                      from l2 in actualsender.DefaultIfEmpty()                      
                      join l3 in ctx.MasterLocations on v.ReceiverIDLocation equals l3.IDLocation into receiver
                      from l3 in receiver.DefaultIfEmpty()                      
                      join l4 in ctx.MasterLocations on v.ActualReceiverIDLocation equals l4.IDLocation into actualreceiver
                      from l4 in actualreceiver.DefaultIfEmpty()                      
                      select new {
                          STONo = v.STONo,
                          ShipmentDate = v.ShipmentDate,
                          SenderIDLocation = v.SenderIDLocation,
                          ReceiverIDLocation = v.ReceiverIDLocation,
                          ActualSenderIDLocation = v.ActualSenderIDLocation,
                          ActualReceiverIDLocation = v.ActualReceiverIDLocation,
                          SenderName = l1 != null ? l1.LocationName : "",
                          ReceiverName = l3 != null ? l3.LocationName : "",
                          ActualSenderName = l2 != null ? l2.LocationName : "",
                          ActualReceiverName = l4 != null ? l4.LocationName : "",
                          DefaultSeqNo = v.DefaultSeqNo,
                          SeqNo = v.SeqNo,
                          ZoneBased = v.ZoneBased,
                          IsActive = v.IsActive,
                      };
            
            var temp = new { viewModel = tmpViewModel, totUnit = totalUnit };

            /*
            foreach (var t in temp.viewModel)
            {
                t.SenderName = listLocation.Where(x => x.IDLocation == t.SenderIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == t.SenderIDLocation).Select(x => x.LocationName).Single().ToString() : "";
                t.ReceiverName = listLocation.Where(x => x.IDLocation == t.ReceiverIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == t.ReceiverIDLocation).Select(x => x.LocationName).Single().ToString() : "";
                t.ActualSenderName = listLocation.Where(x => x.IDLocation == t.ActualSenderIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == t.ActualSenderIDLocation).Select(x => x.LocationName).Single().ToString() : "";
                t.ActualReceiverName = listLocation.Where(x => x.IDLocation == t.ActualReceiverIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == t.ActualReceiverIDLocation).Select(x => x.LocationName).Single().ToString() : "";
            }
            */           
            var jsonRes = Json(temp, JsonRequestBehavior.AllowGet);
            jsonRes.MaxJsonLength = int.MaxValue;
            return jsonRes;
        }

        #region new code Nicco

        public ActionResult LoadPage(TransportOrderInput criteria)
        {
            var dateDist = Index2(criteria);
            Session["DateDist"] = dateDist; // Store in session
            return View();
        }

        private List<TransportPickingListViewDTO> Index2(TransportOrderInput criteria)
        {
            var viewModelList = new List<TransportPickingListViewDTO>();
            using (var ctx = new TOM.EntitiesDAL.EDMX.TOMContextDB())
            {
                var dateDist = _transportPickingListBll.GetRawTPL(criteria)
                    .Select(x => new TransportPickingListViewDTO
                    {
                        STONo = x.STONo,
                        Supplier = x.Supplier,
                        Item = x.Item,
                        Brand = x.Brand,
                        Qty = (long?)x.Qty,
                        UoM = x.UoM,
                        Sender = x.Sender,
                        Receiver = x.Receiver,
                        ShipmentDate = x.ShipmentDate,
                        SeqNo = x.SeqNo,
                        Sequence = x.Sequence,
                        VehicleType = x.VehicleType,
                        Remarks = x.Remarks,
                        CreatedBy = x.CreatedBy,
                        UpdatedBy = x.UpdatedBy,
                        CreatedByID = x.CreatedByID,
                        TransportNo = x.TransportNo,
                        IsActive = x.IsActive,
                        ReceiverIDLocation = x.ReceiverIDLocation,
                        SenderIDLocation = x.SenderIDLocation,
                        IDTransportOrder = x.IDTransportOrder,
                        IDTransportOrderDetail = x.IDTransportOrderDetail
                    })
                    .OrderBy(x => x.ShipmentDate)
                    .ThenBy(x => x.SeqNo != null ? Int64.Parse(x.SeqNo) : 999999999999999)
                    .ThenBy(x => x.Sequence != null ? Int64.Parse(x.Sequence) : 999999999999999)
                    .ThenBy(x => x.Receiver)
                    .ThenBy(x => x.Sender)
                    .ThenBy(x => x.STONo)
                    .ToList();

                return dateDist;
            }




        }

        #endregion

        [HttpPost]
        public ActionResult InsertList(TransportOrderViewModel insertData)
        {
            string result;
            try
            {
                var details = insertData.TransportOrderDetails;
                insertData.TransportOrderDetails = null;
                _transportPickingListBll.SaveData(Mapper.Map<TransportOrderViewModel, TransportOrderDTO>(insertData), GetUserId());
                foreach (var item in details)
                {
                    _transportPickingListBll.SaveDetailData(Mapper.Map<TransportOrderDetailViewModel, TransportOrderDetailDTO>(item), GetUserId());
                }
                result = "";
            }
            catch (ExceptionBase ex)
            {
                result = ex.ToString();
            }
            return Json(result);
        }
        public ActionResult UploadPickingList()
        {
            //TOMContextDB context = new TOMContextDB();
            List<string> listError = new List<string>();
            /*var dbContextTransaction = context.Database.BeginTransaction();
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase fileContent = Request.Files.Get(0);
                    listError = _transportPickingListBll.UploadPickingList2(fileContent, GetUserId());
                }
                dbContextTransaction.Commit();
            }
            catch (Exception e)
            {
                listError.Add(e.Message);
                dbContextTransaction.Rollback();
            }
            finally
            {
                dbContextTransaction.Dispose();
            }*/
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase fileContent = Request.Files.Get(0);
                    listError = _transportPickingListBll.UploadPickingList2(fileContent, GetUserId());
                }
            }
            catch (Exception e)
            {
                return Json(e);
            }
            //Debug.WriteLine("listError:" + listError);
            return Json(listError);
        }

        public JsonResult GenerateSAPFile(TransportPickingListLogInput criteria)
        {
            var dataTO = _transportPickingListBll.GetTransportOrderByShipmentDate(criteria.DateFrom, criteria.DateTo, criteria.uploadedBy)
                .OrderBy(a => a.IDTransportOrder)
                .ThenBy(a => a.ReceiverIDLocation)
                .ThenBy(a => a.Supplier)
                .ToList();
            DataTable boundTable = new DataTable();
            boundTable = _transportPickingListBll.ListToDataTable(dataTO, criteria.DocDate, GetUserId());
            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "SAPFile" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            string[] columns = { "Order Type", "Vendor", "Doc. Date", "Purchasing Org.", "Purch. Group", "Company Code", "Header Text", "Funct", "Number", "A", "I", "Material", "PO Quantity", "OuN", "C", "Deliv. date", "Plnt", "Stor.loc", "Batch", "Shipping Pt.", "TrackingNo", "Requisitioner", "Returns item ", "Free", "Texts", "Purch.req.", "Requisn. item", "Outline agreement", "Agreement item", "RFQ", "Item", "Purchasing Doc.", "Item", "Higher-level item", "Subitem Category" };
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path,
                columns
                , null);
            return Json(path, JsonRequestBehavior.AllowGet);
        }

        /* ----- LOG ----- */
        //public ActionResult GetAllTransportOrder()
        //{
        //    var transOrders = _transportOrderBLL.GetTransportOrder();
        //    return Json(transOrders, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetBatchDate(DateTime tfrom, DateTime tto)
        {
            var batchDates = _transportPickingListBll.GetAllBatchDate(tfrom, tto);
            return Json(batchDates, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBatchData(DateTime tbtc, string usr, DateTime tfrom, DateTime tto)
        {
            var batchData = _transportPickingListBll.GetBatchData(tbtc, usr, tfrom, tto)
                .OrderBy(x => x.SeqNo != null ? Int64.Parse(x.SeqNo) : 99999999999999)
                .ThenBy(x => x.DefaultSeqNo != null ? Int64.Parse(x.DefaultSeqNo) : 99999999999999)
                .ThenBy(x => x.ReceiverLocationName)
                .ThenBy(x => x.SenderLocationName);

            return Json(batchData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBatchData(DateTime tbtc, string usr, DateTime tfrom, DateTime tto)
        {
            //bool result = false;
            var result = _transportPickingListBll.DeleteBatchData(tbtc, usr, tfrom, tto, GetUserId());
            return Json(result);
        }
        /* ----- LOG END ----- */

        /* ----- EXPORT ----- */
        public JsonResult ExportPickingListForm(TransportOrderInput criteria)
        {
            var data = ExportPickingList("LF", criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #region EXPORT
        public string ExportPickingList(string tipe, TransportOrderInput criteria)
        {
            ExcelPackage xls_file = new ExcelPackage();
            ExcelWorksheet xls_sheet;
            ExcelRange xls_range;
            var ctx = new TOM.EntitiesDAL.EDMX.TOMContextDB();

            string[] colTitles = { "", "STO Number", "Supplier", "Item", "Brand", "Qty", "UoM", "Promise Date", "", "Sender", "", "Receiver", "Seq", "Vehicle Type", "Remarks", "Created By", "Updated By", "TN" };
            string path = "";

            if (tipe == "RD")
            {
                #region RAW DATA
                xls_sheet = xls_file.Workbook.Worksheets.Add("Export");

                xls_sheet.Column(1).Width = 11.3;
                xls_sheet.Column(2).Width = 29;
                xls_sheet.Column(3).Width = 14.3;
                xls_sheet.Column(4).Width = 4.7;
                xls_sheet.Column(5).Width = 4.8;
                xls_sheet.Column(6).Width = 12.6;
                xls_sheet.Column(7).Width = 5.3;
                xls_sheet.Column(8).Width = 26;
                xls_sheet.Column(9).Width = 5.3;
                xls_sheet.Column(10).Width = 26;
                xls_sheet.Column(11).Width = 3.3;
                xls_sheet.Column(12).Width = 21.7;
                xls_sheet.Column(13).Width = 27.8;
                xls_sheet.Column(14).Width = 13.3;

                using (xls_range = xls_sheet.Cells[1, 1, 1, 14])
                {
                    xls_range.Value = "Pengiriman";
                    xls_range.Merge = true;
                    xls_range.Style.Font.Size = 10;
                    xls_range.Style.Font.Bold = true;
                    xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                for (int c = 1; c <= 14; c++)
                {
                    using (xls_range = xls_sheet.Cells[2, c])
                    {
                        xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White);
                        if (c == 14) { xls_range.Style.Border.Right.Style = ExcelBorderStyle.None; }
                    }
                    using (xls_range = xls_sheet.Cells[3, c])
                    {
                        xls_range.Value = colTitles[c];
                        xls_range.Style.Font.Size = 10;
                        xls_range.Style.Font.Bold = true;
                        xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                        xls_range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                    }
                }

                int r = 4;

                //var dbResult = _transportPickingListBll.GetRawDataExport();
                var dbResult = _transportPickingListBll.GetListFormExport();

                foreach (var data in dbResult)
                {
                    string[] dtaRows = { "", data.STONo, data.Supplier, data.Code, data.Qty.ToString(), data.UoM, data.ShipmentDate.ToString("dd/MMM/yyyy"), data.ActualSenderIDLocation, data.LocNameSender, data.ActualReceiverIDLocation, data.LocNameReceiver, data.SeqNo, data.VehicleType, data.Remarks, data.Description };

                    for (int c = 1; c <= 14; c++)
                    {
                        using (xls_range = xls_sheet.Cells[r, c])
                        {
                            xls_range.Value = dtaRows[c];
                            xls_range.Style.Font.Size = 10;
                            xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            xls_range.Style.Border.Bottom.Color.SetColor(Color.White);
                            xls_range.Style.Border.Top.Color.SetColor(Color.White);

                            if (c == 1) { xls_range.Style.Border.Left.Style = ExcelBorderStyle.Medium; }
                            if (c == 14) { xls_range.Style.Border.Right.Style = ExcelBorderStyle.Medium; }
                        }

                        if ((c == 1 || c == 11) && (xls_sheet.Cells[r, c].Value != null && xls_sheet.Cells[r, c].Value.ToString() != ""))
                        {
                            xls_sheet.Cells[r, c].Value = Convert.ToInt64(xls_sheet.Cells[r, c].Value);
                            xls_sheet.Cells[r, c].Style.Numberformat.Format = "#";
                        }
                        if (c == 4)
                        {
                            xls_sheet.Cells[r, c].Value = Convert.ToDecimal(xls_sheet.Cells[r, c].Value);
                            xls_sheet.Cells[r, c].Style.Numberformat.Format = "#";
                        }
                    }

                    r++;
                }

                path = "PickingListRawData" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                xls_file.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
                #endregion
            }
            else if (tipe == "LF")
            {
                string[] dayWeeks = { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };

                Dictionary<string, string> userCheck = new Dictionary<string, string>();

                #region old code
                ////var getAllData = TransportOrderList.ToList();
                //var getAllData = _transportPickingListBll.GetALLPickingList(criteria).OrderBy(x => x.SenderIDLocation).ThenBy(x => x.SeqNo).ToList();
                //var sheetGroup = getAllData.GroupBy(g => g.ShipmentDate).OrderBy(g => g.Key).ToList();
                ////var getAllTE = _transportPickingListBll.getAllTransportExecution();
                ////var getAllUser = _transportPickingListBll.getAllMasterUser().ToList();

                //foreach (var tab in sheetGroup)
                //{
                //    //xls_sheet = xls_file.Workbook.Worksheets.Add(tab.Key.Date.ToString("dd-MMM-yyyy"));
                //    xls_sheet = xls_file.Workbook.Worksheets.Add(dayWeeks[(int)tab.Key.DayOfWeek]); //SHEET NAME

                //    #region COLUMN WIDTH & FORMAT CELLS
                //    xls_sheet.Column(1).Width = 11.3;
                //    xls_sheet.Column(2).Width = 29;
                //    xls_sheet.Column(3).Width = 14.3;
                //    xls_sheet.Column(4).Width = 14;
                //    xls_sheet.Column(5).Width = 5;
                //    xls_sheet.Column(6).Width = 4.8;
                //    xls_sheet.Column(7).Width = 12.6;
                //    xls_sheet.Column(8).Width = 5.3;
                //    xls_sheet.Column(9).Width = 26;
                //    xls_sheet.Column(10).Width = 5.3;
                //    xls_sheet.Column(11).Width = 26;
                //    xls_sheet.Column(12).Width = 5;
                //    xls_sheet.Column(13).Width = 21.7;
                //    xls_sheet.Column(14).Width = 27.8;
                //    xls_sheet.Column(15).Width = 13.3;
                //    xls_sheet.Column(16).Width = 14;
                //    xls_sheet.Column(17).Width = 14;

                //    xls_sheet.Column(1).Style.Numberformat.Format = "0";
                //    xls_sheet.Column(12).Style.Numberformat.Format = "0";
                //    #endregion

                //    #region COLUMN HEADER
                //    using (xls_range = xls_sheet.Cells[1, 1, 1, 17])
                //    {
                //        xls_range.Value = "Pengiriman ( " + dayWeeks[(int)tab.Key.DayOfWeek] + " ) Tgl. " + tab.Key.ToString("dd MMMM yyyy");
                //        xls_range.Merge = true;
                //        xls_range.Style.Font.Size = 10;
                //        xls_range.Style.Font.Bold = true;
                //        xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //    }

                //    for (int c = 1; c <= 17; c++)
                //    {
                //        #region Merge & Center Header
                //        using (xls_range = xls_sheet.Cells[2, c])
                //        {
                //            xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White);
                //            //if (c == 14) { xls_range.Style.Border.Right.Style = ExcelBorderStyle.None; }
                //        }
                //        #endregion

                //        using (xls_range = xls_sheet.Cells[3, c])
                //        {
                //            xls_range.Value = colTitles[c];
                //            xls_range.Style.Font.Size = 10;
                //            xls_range.Style.Font.Bold = true;
                //            xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //            xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //            xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //            xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                //            xls_range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                //        }
                //    }
                //    #endregion

                //    string TO_STONo = "", TOD_Code = "";
                //    int r = 4; //START ROW

                //    var dbResult = getAllData.Where(w => w.ShipmentDate.Day == tab.Key.Day && w.ShipmentDate.Month == tab.Key.Month && w.ShipmentDate.Year == tab.Key.Year);
                //    //dbResult = dbResult.OrderBy(c => c.SeqNo != null ? c.SeqNo.Length : c.DefaultSeqNo != null ? c.DefaultSeqNo.Length : "99999").ThenBy(c => c.SeqNo != null ? c.SeqNo : c.DefaultSeqNo != null ? c.DefaultSeqNo : "99999").ThenBy(x => x.ReceiverLocationName).ThenBy(x => x.SenderLocationName).ToList();                    
                //    dbResult = dbResult
                //        .OrderBy(x => x.SeqNo != null ? Int64.Parse(x.SeqNo) : 999999999999999)
                //        //.ThenBy(x => x.DefaultSeqNo != null ? x.DefaultSeqNo.Length : 99999999999999)
                //        .ThenBy(x => x.DefaultSeqNo != null ? Int64.Parse(x.DefaultSeqNo) : 999999999999999)
                //        .ThenBy(x => x.ReceiverLocationName)
                //        .ThenBy(x => x.SenderLocationName)
                //        .ToList();

                //    //update TN, CreatedBy and UpdatedBy
                //    (from d in dbResult
                //     join te in ctx.TransportExecutions on d.IDTransportExecution equals te.IDTransportExecution into dte
                //     from te in dte.DefaultIfEmpty()
                //     join u1 in ctx.MasterUsers on d.CreatedBy.ToLower() equals u1.IDUser.ToLower() into du
                //     from u1 in du.DefaultIfEmpty()
                //     join u2 in ctx.MasterUsers on d.UpdatedBy.ToLower() equals u2.IDUser.ToLower() into du2
                //     from u2 in du2.DefaultIfEmpty()
                //     select new { d, te, u1, u2 }).ToList()
                //    .ForEach(row =>
                //    {
                //        row.d.TransportNo = row.te != null ? row.te.TransportNo : "";
                //        row.d.CreatedByFullName = row.u1 != null ? row.u1.FullName : "";
                //        row.d.UpdatedByFullName = row.u2 != null ? row.u2.FullName : "";
                //    });

                //    foreach (var data in dbResult)
                //    {
                //        var TNumber = data.TransportNo;
                //        var createdByFullName = data.CreatedByFullName;
                //        var updatedByFullName = data.UpdatedByFullName;
                //        /*
                //        var TNumber = "";
                //        if (data.IDTransportExecution != null)
                //        {
                //            //TNumber = getAllTE.Where(x => x.IDTransportExecution == data.IDTransportExecution).Select(x => x.TransportNo).Single();
                //            var ExecData = getAllTE.Where(x => x.IDTransportExecution == data.IDTransportExecution).FirstOrDefault();

                //            if (ExecData != null)
                //            {
                //                TNumber = ExecData.TransportNo;
                //            }
                //        }
                //        var createdByFullName = _transportPickingListBll.getUserFullName(data.CreatedBy);
                //        var updatedByFullName = _transportPickingListBll.getUserFullName(data.UpdatedBy);
                //        */
                //        foreach (var detail in data.TransportOrderDetails)
                //        {
                //            #region DATA ROWS
                //            string[] dtaRows = {
                //                "",
                //                data.STONo,
                //                detail.Supplier,
                //                detail.Code,
                //                detail.Description,
                //                detail.Qty.ToString(),
                //                detail.UoM,
                //                data.ShipmentDate.ToString("dd/MMM/yyyy"),
                //                data.SenderIDLocation,
                //                listLocation.Where(x => x.IDLocation == data.SenderIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == data.SenderIDLocation).Select(x => x.LocationName).Single().ToString() : "",
                //                data.ReceiverIDLocation,
                //                listLocation.Where(x => x.IDLocation == data.ReceiverIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == data.ReceiverIDLocation).Select(x => x.LocationName).Single().ToString() : "",
                //                data.SeqNo == null ? data.DefaultSeqNo : data.SeqNo,
                //                data.VehicleType,
                //                data.Remarks,
                //                createdByFullName,
                //                updatedByFullName,
                //                data.IDTransportExecution.ToString()
                //            };
                //            #endregion

                //            var Sender = _locBll.GetNameById(data.ActualSenderIDLocation);
                //            var Receiver = _locBll.GetNameById(data.ActualReceiverIDLocation);

                //            for (int c = 1; c <= 17; c++)
                //            {
                //                #region WRITE VALUE
                //                using (xls_range = xls_sheet.Cells[r, c])
                //                {
                //                    if (c == 1 && TO_STONo == dtaRows[c]) { xls_range.Value = ""; }
                //                    else if (c == 9) { xls_range.Value = Sender; }
                //                    else if (c == 11) { xls_range.Value = Receiver; }
                //                    else if (c == 17) xls_sheet.Cells[r, c].Value = TNumber;
                //                    else
                //                    {
                //                        if (c == 1)
                //                        {
                //                            if (string.IsNullOrEmpty(dtaRows[c]))
                //                            {
                //                                xls_range.Value = "";
                //                            }
                //                            else if (dtaRows[c].Contains("PO") || dtaRows[c].Contains("ON"))
                //                            {
                //                                xls_range.Value = dtaRows[c];
                //                            }
                //                            else
                //                            {
                //                                xls_range.Value = Int64.Parse(dtaRows[c]);
                //                            }
                //                        }
                //                        else xls_range.Value = dtaRows[c];
                //                    }

                //                    xls_range.Style.Font.Size = 10;
                //                    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //                    xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                //                    xls_range.Style.Border.Bottom.Color.SetColor(Color.White);
                //                    xls_range.Style.Border.Top.Color.SetColor(Color.White);

                //                    if (c == 1) xls_range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                //                    if (c == 14) xls_range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                //                }
                //                if ((c == 12) && (xls_sheet.Cells[r, c].Value != null && xls_sheet.Cells[r, c].Value.ToString() != ""))
                //                {
                //                    xls_sheet.Cells[r, c].Value = Convert.ToInt64(xls_sheet.Cells[r, c].Value);
                //                }
                //                if (c == 5)
                //                {
                //                    xls_sheet.Cells[r, c].Value = Convert.ToDecimal(String.IsNullOrEmpty(xls_sheet.Cells[r, c].Value.ToString()) ? 0 : xls_sheet.Cells[r, c].Value);
                //                }
                //                #endregion

                //                #region CHANGE LOG
                //                if (data.TransportOrderChangeLogs.Count > 0)
                //                {
                //                    if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.ShipmentDate)))
                //                    {
                //                        xls_sheet.Cells[r, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.SenderLocation)))
                //                    {
                //                        xls_sheet.Cells[r, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 8].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                        xls_sheet.Cells[r, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.ReceiverLocation)))
                //                    {
                //                        xls_sheet.Cells[r, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 10].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                        xls_sheet.Cells[r, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 11].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence)))
                //                    {
                //                        xls_sheet.Cells[r, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 12].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.VehicleType)))
                //                    {
                //                        xls_sheet.Cells[r, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 13].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Remarks)))
                //                    {
                //                        xls_sheet.Cells[r, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 14].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    //if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains("Created By"))
                //                    //{
                //                    //    xls_sheet.Cells[r, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                    //    xls_sheet.Cells[r, 15].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    //}
                //                    //if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains("Updated By"))
                //                    //{
                //                    //    xls_sheet.Cells[r, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                    //    xls_sheet.Cells[r, 16].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    //}
                //                    //if (data.TransportOrderChangeLogs.Select(x => x.ModifiedField).Contains("TN"))
                //                    //{
                //                    //    xls_sheet.Cells[r, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                    //    xls_sheet.Cells[r, 17].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    //}
                //                }
                //                if (detail.TransportOrderDetailChangeLogs.Count > 0)
                //                {
                //                    if (detail.TransportOrderDetailChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Supplier)))
                //                    {
                //                        xls_sheet.Cells[r, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (detail.TransportOrderDetailChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Code)))
                //                    {
                //                        xls_sheet.Cells[r, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (detail.TransportOrderDetailChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Description)))
                //                    {
                //                        xls_sheet.Cells[r, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (detail.TransportOrderDetailChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Quantity)))
                //                    {
                //                        xls_sheet.Cells[r, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                    if (detail.TransportOrderDetailChangeLogs.Select(x => x.ModifiedField).Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.UoM)))
                //                    {
                //                        xls_sheet.Cells[r, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                        xls_sheet.Cells[r, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //                    }
                //                }
                //                #endregion
                //            }

                //            // kasi kuning kalo kedelete
                //            if (!detail.IsActive)
                //            {
                //                xls_sheet.Cells[r, 1, r, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //                xls_sheet.Cells[r, 1, r, 17].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                //            }

                //            TO_STONo = dtaRows[1]; TOD_Code = dtaRows[3];
                //            r++;
                //        }
                //    }
                //}
                #endregion

                #region new code Nicco
                var getAllData2 = _transportPickingListBll.GetRawTPL(criteria).OrderBy(x => x.SenderIDLocation).ThenBy(x => x.SeqNo).ToList();
                var idTRansportOrder = getAllData2.Select(x => x.IDTransportOrder).ToList();
                var getTOL = (from x in ctx.TransportOrderChangeLogs
                              where idTRansportOrder.Contains(x.IDTransportOrder)
                              select new TransportOrderChangeLogDTO
                              {
                                  IDTransportOrder = x.IDTransportOrder,
                                  ModifiedField = x.ModifiedField
                              }).ToList();

                var idTRansportOrderDetail = getAllData2.Select(x => x.IDTransportOrderDetail).ToList();
                var getTODL = (from x in ctx.TransportOrderDetailChangeLogs
                               where idTRansportOrderDetail.Contains(x.IDTransportOrderDetail)
                               select new TransportOrderDetailChangeLogDTO
                               {
                                   IDTransportOrderDetail = x.IDTransportOrderDetail,
                                   ModifiedField = x.ModifiedField
                               }).ToList();

                var sheetGroup2 = getAllData2.GroupBy(g => g.ShipmentDate).OrderBy(g => g.Key).ToList();

                foreach (var tab in sheetGroup2)
                {
                    xls_sheet = xls_file.Workbook.Worksheets.Add(dayWeeks[(int)tab.Key.DayOfWeek]); //SHEET NAME

                    #region COLUMN WIDTH & FORMAT CELLS
                    xls_sheet.Column(1).Width = 11.3;
                    xls_sheet.Column(2).Width = 29;
                    xls_sheet.Column(3).Width = 14.3;
                    xls_sheet.Column(4).Width = 14;
                    xls_sheet.Column(5).Width = 5;
                    xls_sheet.Column(6).Width = 4.8;
                    xls_sheet.Column(7).Width = 12.6;
                    xls_sheet.Column(8).Width = 5.3;
                    xls_sheet.Column(9).Width = 26;
                    xls_sheet.Column(10).Width = 5.3;
                    xls_sheet.Column(11).Width = 26;
                    xls_sheet.Column(12).Width = 5;
                    xls_sheet.Column(13).Width = 21.7;
                    xls_sheet.Column(14).Width = 27.8;
                    xls_sheet.Column(15).Width = 13.3;
                    xls_sheet.Column(16).Width = 14;
                    xls_sheet.Column(17).Width = 14;

                    xls_sheet.Column(1).Style.Numberformat.Format = "0";
                    xls_sheet.Column(12).Style.Numberformat.Format = "0";
                    #endregion

                    #region COLUMN HEADER
                    using (xls_range = xls_sheet.Cells[1, 1, 1, 17])
                    {
                        xls_range.Value = "Pengiriman ( " + dayWeeks[(int)tab.Key.DayOfWeek] + " ) Tgl. " + tab.Key.ToString("dd MMMM yyyy");
                        xls_range.Merge = true;
                        xls_range.Style.Font.Size = 10;
                        xls_range.Style.Font.Bold = true;
                        xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    for (int c = 1; c <= 17; c++)
                    {
                        #region Merge & Center Header
                        using (xls_range = xls_sheet.Cells[2, c])
                        {
                            xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White);
                            //if (c == 14) { xls_range.Style.Border.Right.Style = ExcelBorderStyle.None; }
                        }
                        #endregion

                        using (xls_range = xls_sheet.Cells[3, c])
                        {
                            xls_range.Value = colTitles[c];
                            xls_range.Style.Font.Size = 10;
                            xls_range.Style.Font.Bold = true;
                            xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                            xls_range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        }
                    }
                    #endregion

                    string TO_STONo = "", TOD_Code = "";
                    int r = 4; //START ROW

                    var dbResult2 = getAllData2.Where(w => w.ShipmentDate.Day == tab.Key.Day && w.ShipmentDate.Month == tab.Key.Month && w.ShipmentDate.Year == tab.Key.Year);
                    dbResult2 = dbResult2
                        .OrderBy(x => x.SeqNo != null ? Int64.Parse(x.SeqNo) : 999999999999999)
                        .ThenBy(x => x.Sequence != null ? Int64.Parse(x.Sequence) : 999999999999999)
                        .ThenBy(x => x.Receiver)
                        .ThenBy(x => x.Sender)
                        .ToList();


                    foreach (var data in dbResult2)
                    {
                        var TNumber = data.TransportNo;
                        #region DATA ROWS
                        string[] dtaRows = {
                                "",
                                data.STONo,
                                data.Supplier,
                                data.Item,
                                data.Brand,
                                data.Qty.ToString(),
                                data.UoM,
                                data.ShipmentDate.ToString("dd/MMM/yyyy"),
                                data.SenderIDLocation,
                                listLocation.Where(x => x.IDLocation == data.SenderIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == data.SenderIDLocation).Select(x => x.LocationName).Single().ToString() : "",
                                data.ReceiverIDLocation,
                                listLocation.Where(x => x.IDLocation == data.ReceiverIDLocation).Select(x => x.LocationName).ToList().Count > 0 ? listLocation.Where(x => x.IDLocation == data.ReceiverIDLocation).Select(x => x.LocationName).Single().ToString() : "",
                                data.SeqNo == null ? data.Sequence : data.SeqNo,
                                data.VehicleType,
                                data.Remarks,
                                data.CreatedBy,
                                data.UpdatedBy,
                                //data.IDTransportExecution.ToString()
                            };
                        #endregion

                        var Sender = data.Sender;
                        var Receiver = data.Receiver;

                        for (int c = 1; c <= 17; c++)
                        {
                            #region WRITE VALUE
                            using (xls_range = xls_sheet.Cells[r, c])
                            {
                                if (c == 1 && TO_STONo == dtaRows[c]) { xls_range.Value = ""; }
                                else if (c == 9) { xls_range.Value = Sender; }
                                else if (c == 11) { xls_range.Value = Receiver; }
                                else if (c == 17) xls_sheet.Cells[r, c].Value = TNumber;
                                else
                                {
                                    if (c == 1)
                                    {
                                        if (string.IsNullOrEmpty(dtaRows[c]))
                                        {
                                            xls_range.Value = "";
                                        }
                                        else if (dtaRows[c].Contains("PO") || dtaRows[c].Contains("ON"))
                                        {
                                            xls_range.Value = dtaRows[c];
                                        }
                                        else
                                        {
                                            xls_range.Value = Int64.Parse(dtaRows[c]);
                                        }
                                    }
                                    else xls_range.Value = dtaRows[c];
                                }

                                xls_range.Style.Font.Size = 10;
                                xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                xls_range.Style.Border.Bottom.Color.SetColor(Color.White);
                                xls_range.Style.Border.Top.Color.SetColor(Color.White);

                                if (c == 1) xls_range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                                if (c == 14) xls_range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }
                            if ((c == 12) && (xls_sheet.Cells[r, c].Value != null && xls_sheet.Cells[r, c].Value.ToString() != ""))
                            {
                                xls_sheet.Cells[r, c].Value = Convert.ToInt64(xls_sheet.Cells[r, c].Value);
                            }
                            if (c == 5)
                            {
                                xls_sheet.Cells[r, c].Value = Convert.ToDecimal(String.IsNullOrEmpty(xls_sheet.Cells[r, c].Value.ToString()) ? 0 : xls_sheet.Cells[r, c].Value);
                            }
                            #endregion

                            #region ALL LOG
                            if (getTOL.Count > 0)
                            {
                                foreach (var log in getTOL)
                                {
                                    if (log.IDTransportOrder == data.IDTransportOrder)
                                    {
                                        #region CHANGE LOG
                                        if (log.ModifiedField.ToLower().Contains("shipmentdate"))
                                        {
                                            xls_sheet.Cells[r, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                                        }
                                        if (log.ModifiedField.ToLower().Contains("senderlocation"))
                                        {
                                            xls_sheet.Cells[r, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 8].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                            xls_sheet.Cells[r, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }
                                        if (log.ModifiedField.ToLower().Contains("receiveridlocation"))
                                        {
                                            xls_sheet.Cells[r, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 10].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                            xls_sheet.Cells[r, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 11].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }
                                        if (log.ModifiedField.ToLower().Contains("sequence"))
                                        {
                                            xls_sheet.Cells[r, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 12].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                                        }
                                        if (log.ModifiedField.ToLower().Contains("vehicle type"))
                                        {
                                            xls_sheet.Cells[r, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 13].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }
                                        if (log.ModifiedField.ToLower().Contains("remarks"))
                                        {
                                            xls_sheet.Cells[r, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 14].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }
                                        #endregion
                                    }
                                }
                            }

                            if (getTODL.Count > 0)
                            {
                                foreach (var log in getTODL)
                                {
                                    var logDetail = log.IDTransportOrderDetail == log.IDTransportOrderDetail;
                                    if (log.IDTransportOrderDetail == data.IDTransportOrderDetail)
                                    {
                                        #region CHANGE LOG
                                        if (log.ModifiedField.ToLower().Contains("supplier"))
                                        {
                                            xls_sheet.Cells[r, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                                        }
                                        if (log.ModifiedField.ToLower().Contains("code"))
                                        {
                                            xls_sheet.Cells[r, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }
                                        if (log.ModifiedField.ToLower().Contains("description"))
                                        {
                                            xls_sheet.Cells[r, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }
                                        if (log.ModifiedField.ToLower().Contains("qty"))
                                        {
                                            xls_sheet.Cells[r, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }
                                        if (log.ModifiedField.ToLower().Contains("uom"))
                                        {
                                            xls_sheet.Cells[r, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            xls_sheet.Cells[r, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                                        }

                                        #endregion
                                    }
                                }
                            }
                            #endregion
                        }

                        // kasi kuning kalo kedelete
                        if (!data.IsActive)
                        {
                            xls_sheet.Cells[r, 1, r, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            xls_sheet.Cells[r, 1, r, 17].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                        }

                        TO_STONo = dtaRows[1]; TOD_Code = dtaRows[3];
                        r++;
                    }
                }
                #endregion
                //var week = _masterGenWeekRepo.GetGenWeekNowByDate(DateTime.Today).Week;
                var week = criteria.weekFilter;
                path = "PickingListForm_Week" + week + "_" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
                xls_file.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
            }

            return path;
        }
        #endregion EXPORT
        public JsonResult ExportRawData(TransportOrderInput criteria)
        {
            #region old code
            //var ctx = new TOM.EntitiesDAL.EDMX.TOMContextDB();
            //using (var pkg = new ExcelExportHelper())
            //{
            //    TransportPickingListExportDTO a;
            //    pkg.Map("STO Number", "STONo");
            //    pkg.Map("Supplier", "Supplier");
            //    pkg.Map("Item", "Item");
            //    pkg.Map("Brand", "Brand");
            //    pkg.Map("Qty", "Qty");
            //    pkg.Map("UoM", "UoM");
            //    pkg.Map("Promise Date", "ShipmentDate");
            //    pkg.Map("ID From", "SenderIDLocation");
            //    pkg.Map("From", "ActualSenderName");
            //    pkg.Map("ID To", "ReceiverIDLocation");
            //    pkg.Map("To", "ActualReceiverName");
            //    pkg.Map("Seq", "SeqNo");
            //    pkg.Map("Vehicle Type", "VehicleType");
            //    pkg.Map("Remarks", "Remarks");
            //    pkg.Map("Created By", "CreatedByFullName");
            //    pkg.Map("Updated By", "UpdatedByFullName");
            //    pkg.Map("TN", "TN");

            //    //var to = _trOrderBll.Get(new TransportOrderInput() { IsActive = true }, "ShipmentDate");
            //    //var dateDist = to.Select(t => t.ShipmentDate).Distinct().ToList();

            //    var dateDist = _transportPickingListBll.GetRawTOD(criteria)
            //        .OrderBy(x => x.ShipmentDate)
            //        .ThenBy(x => x.SeqNo != null ? Int64.Parse(x.SeqNo) : 999999999999999)
            //        //.ThenBy(x => x.DefaultSeqNo != null ? x.DefaultSeqNo.Length : 99999999999999)
            //        .ThenBy(x => x.DefaultSeqNo != null ? Int64.Parse(x.DefaultSeqNo) : 999999999999999)
            //        .ThenBy(x => x.ReceiverLocationName)
            //        .ThenBy(x => x.SenderLocationName);

            //    //var getAllTE = _transportPickingListBll.getAllTransportExecution();

            //    //update TN, CreatedBy and UpdatedBy
            //    (from d in dateDist
            //     join te in ctx.TransportExecutions on d.IDTransportExecution equals te.IDTransportExecution into dte
            //     from te in dte.DefaultIfEmpty()
            //     join u1 in ctx.MasterUsers on d.CreatedBy.ToLower() equals u1.IDUser.ToLower() into du
            //     from u1 in du.DefaultIfEmpty()
            //     join u2 in ctx.MasterUsers on d.UpdatedBy.ToLower() equals u2.IDUser.ToLower() into du2
            //     from u2 in du2.DefaultIfEmpty()
            //     select new { d, te, u1, u2 }).ToList()
            //    .ForEach(row =>
            //    {
            //        row.d.TN = row.te != null ? row.te.TransportNo : "";
            //        row.d.CreatedByFullName = row.u1 != null ? row.u1.FullName : "";
            //        row.d.UpdatedByFullName = row.u2 != null ? row.u2.FullName : "";
            //    });
            //    /*
            //    foreach (var datas in dateDist)
            //    {
            //        if (datas.IDTransportExecution != null)
            //        {
            //            var ExecData = getAllTE.Where(x => x.IDTransportExecution == datas.IDTransportExecution).FirstOrDefault();

            //            if (ExecData != null)
            //            {
            //                datas.TN = ExecData.TransportNo;
            //            }
            //        }
            //        datas.CreatedByFullName = _transportPickingListBll.getUserFullName(datas.CreatedBy).ToString();
            //        datas.UpdatedByFullName = _transportPickingListBll.getUserFullName(datas.UpdatedBy).ToString();
            //    }
            //    */
            //    //dateDist.Sort();
            //    //foreach (var date in dateDist)
            //    //{
            //    //var ws = pkg.CreateWorksheet(date.ToString("yyyy-MMM-dd"));
            //    var ws = pkg.CreateWorksheet("Export");

            //    // write header
            //    pkg.WriteHeader(ws);

            //    // write values
            //    pkg.WriteRows(ws, dateDist, (dto, cell, pi) =>
            //    {
            //        var val = pi.GetValue(dto);
            //        if (pi.Name == "ShipmentDate") cell.Style.Numberformat.Format = "dd MMM yyyy";
            //        return val;
            //    });
            //    pkg.AutoSizeColumns(ws);

            //    ws.Column(1).Style.Numberformat.Format = "@";
            //    ws.Column(12).Style.Numberformat.Format = "@";

            //    var path = "RawData_" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            //    pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

            //    return Json(path, JsonRequestBehavior.AllowGet);
            //}
            #endregion

            #region new code Nicco

            var dateDist = Index2(criteria) as List<TransportPickingListViewDTO>;

            if (dateDist == null)
            {
                return Json("Error: Data not found", JsonRequestBehavior.AllowGet);
            }

            using (var pkg = new ExcelExportHelper())
            {
                TransportPickingListExportDTO a;
                pkg.Map("STO Number", "STONo");
                pkg.Map("Supplier", "Supplier");
                pkg.Map("Item", "Item");
                pkg.Map("Brand", "Brand");
                pkg.Map("Qty", "Qty");
                pkg.Map("UoM", "UoM");
                pkg.Map("Promise Date", "ShipmentDate");
                pkg.Map("ID From", "SenderIDLocation");
                pkg.Map("From", "Sender");
                pkg.Map("ID To", "ReceiverIDLocation");
                pkg.Map("To", "Receiver");
                pkg.Map("Seq", "SeqNo");
                pkg.Map("Vehicle Type", "VehicleType");
                pkg.Map("Remarks", "Remarks");
                pkg.Map("Created By", "CreatedBy");
                pkg.Map("Updated By", "UpdatedBy");
                pkg.Map("TN", "TransportNo");

                var ws = pkg.CreateWorksheet("Export");

                // write header
                pkg.WriteHeader(ws);

                // write values
                pkg.WriteRows(ws, dateDist, (dto, cell, pi) =>
                {
                    var val = pi.GetValue(dto);
                    if (pi.Name == "ShipmentDate") cell.Style.Numberformat.Format = "dd MMM yyyy";
                    return val;
                });
                pkg.AutoSizeColumns(ws);

                ws.Column(1).Style.Numberformat.Format = "@";
                ws.Column(12).Style.Numberformat.Format = "@";

                var path = "RawData_" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

                return Json(path, JsonRequestBehavior.AllowGet);
            }

            #endregion
            return null;
        }

        //public JsonResult CustomExportXls(string colCaption, TransportOrderInput criteria)
        public JsonResult CustomExportXls(string colCaption, string colIndex, string colWidth, TransportOrderInput criteria)
        {
            #region OLD CODE
            //ExcelPackage xls_file = new ExcelPackage();
            //ExcelWorksheet xls_sheet;
            //ExcelRange xls_range;

            //xls_sheet = xls_file.Workbook.Worksheets.Add("Export");

            //int cols = colCaption.Split(',').Count();

            //#region TOP HEADER
            //using (xls_range = xls_sheet.Cells[1, 1, 1, cols])
            //{
            //    xls_range.Value = "Picking List Custom";
            //    xls_range.Merge = true;
            //    xls_range.Style.Font.Size = 10;
            //    xls_range.Style.Font.Bold = true;
            //    xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //}
            //#endregion

            //int nn = 0;
            //for (int n = 0; n < cols; n++)
            //{
            //    using (xls_range = xls_sheet.Cells[2, n + 1])
            //    {
            //        xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White);
            //    }
            //    using (xls_range = xls_sheet.Cells[3, n + 1])
            //    {
            //        xls_range.Value = colCaption.Split(',')[n];

            //        xls_range.Style.Font.Size = 10;
            //        xls_range.Style.Font.Bold = true;
            //        xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //        xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
            //        xls_range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
            //    }

            //    xls_sheet.Column(n + 1).Width = int.Parse(colWidth.Split(',')[n]);
            //    nn = n;
            //}

            //xls_sheet.Cells[2, nn + 1].Style.Border.Right.Style = ExcelBorderStyle.None;

            //string TN = "", TOD = "";
            //int r = 4;

            //var dbResult = _transportPickingListBll.GetExportXls(criteria);

            //foreach (var data in dbResult)
            //{
            //    string[] dtaRows = {
            //                            "", data.STONo, data.ShipmentDate.ToString("dd/MMM/yyyy"), data.SenderLocationName, data.ReceiverLocationName,
            //                            data.ZoneBased, data.DefaultSeqNo == null? "": data.DefaultSeqNo.ToString()
            //                        };

            //    for (int n = 0; n < cols; n++)
            //    {
            //        if (dtaRows[1] != TN)
            //        {
            //            using (xls_range = xls_sheet.Cells[r, n + 1])
            //            {
            //                xls_range.Value = dtaRows[int.Parse(colIndex.Split(',')[n])];

            //                xls_range.Style.Font.Size = 10;
            //                xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //                xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            //                xls_range.Style.Border.Bottom.Color.SetColor(Color.White);
            //                xls_range.Style.Border.Top.Color.SetColor(Color.White);
            //            }

            //            if (int.Parse(colIndex.Split(',')[n]) == 15)
            //            {
            //                xls_sheet.Cells[r, n + 1].Value = Convert.ToDecimal(xls_sheet.Cells[r, n + 1].Value);
            //                xls_sheet.Cells[r, n + 1].Style.Numberformat.Format = "#";
            //            }
            //            if (int.Parse(colIndex.Split(',')[n]) == 16)
            //            {
            //                xls_sheet.Cells[r, n + 1].Value = Convert.ToInt32(xls_sheet.Cells[r, n + 1].Value);
            //                xls_sheet.Cells[r, n + 1].Style.Numberformat.Format = "#";
            //            }

            //            if (n == 0)
            //            {
            //                xls_sheet.Cells[r, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            //            }
            //            else if ((n + 1) == cols)
            //            {
            //                xls_sheet.Cells[r, n + 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            //            }
            //        }
            //        else
            //        {
            //            if (int.Parse(colIndex.Split(',')[n]) == 17 && TOD == dtaRows[1])
            //            {
            //                r--;
            //                xls_sheet.Cells[r, n + 1].Value = xls_sheet.Cells[r, n + 1].Value + ", " + dtaRows[int.Parse(colIndex.Split(',')[n])];
            //                r++;
            //            }
            //        }
            //    }

            //    if (dtaRows[1] != TN) { TOD = dtaRows[1]; r++; }
            //    TN = dtaRows[1];
            //}

            //String path = "TransportPickingListCustom" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            //xls_file.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

            //return Json(path, JsonRequestBehavior.AllowGet);
            #endregion

            #region OLD CODE
            //var ctx = new TOM.EntitiesDAL.EDMX.TOMContextDB();

            //DataTable dt_Raw = new DataTable();

            //using (var pkg = new ExcelExportHelper())
            //{
            //    foreach (var headerName in colCaption.Split(','))
            //    {
            //        if (headerName.Contains("STO Number")) pkg.Map("STO Number", "STONo");
            //        else if (headerName.Contains("Supplier")) pkg.Map("Supplier", "Supplier");
            //        else if (headerName.Contains("Item")) pkg.Map("Item", "Item");  
            //        else if (headerName.Contains("Brand")) pkg.Map("Brand", "Brand");
            //        else if (headerName.Contains("Qty")) pkg.Map("Qty", "Qty");
            //        else if (headerName.Contains("UoM")) pkg.Map("UoM", "UoM");
            //        else if (headerName.Contains("Shipment Date")) pkg.Map("Promise Date", "ShipmentDate");
            //        else if (headerName.Contains("Sender")) pkg.Map("Sender", "ActualSenderName");
            //        else if (headerName.Contains("Receiver")) pkg.Map("Receiver", "ActualReceiverName");
            //        else if (headerName.Contains("Zone")) ;
            //        else if (headerName.Contains("Sequence")) pkg.Map("Seq", "DefaultSeqNo");
            //        else if (headerName.Contains("Vehicle Type")) pkg.Map("Vehicle Type", "VehicleType");
            //        else if (headerName.Contains("Remarks")) pkg.Map("Remarks", "Remarks");
            //        else if (headerName.Contains("Created By")) pkg.Map("Created By", "CreatedByFullName");
            //        else if (headerName.Contains("Updated By")) pkg.Map("Updated By", "UpdatedByFullName");
            //    }


            //    /*var dateDist = ctx.TransportOrders.SqlQuery("" +
            //        "select " +
            //        "	TransportOrder.IDTransportExecution" +
            //        "	, TransportOrder.STONo" +
            //        "	, TransportOrderDetail.Supplier" +
            //        "	, TransportOrderDetail.Code Item" +
            //        "	, TransportOrderDetail.Description Brand" +
            //        "	, TransportOrderDetail.Qty" +
            //        "	, TransportOrderDetail.UoM" +
            //        "	, TransportOrder.ShipmentDate" +
            //        "	, TransportOrder.SenderIDLocation" +
            //        "	, MasterLocation1.LocationName SenderLocationName" +
            //        "	, TransportOrder.ActualSenderIDLocation" +
            //        "	--, MasterLocation3.LocationName ActualSenderName" +
            //        "	, '' ActualSenderName" +
            //        "	, TransportOrder.ReceiverIDLocation" +
            //        "	, MasterLocation2.LocationName ReceiverLocationName" +
            //        "	, TransportOrder.ActualReceiverIDLocation" +
            //        "	--, MasterLocation4.LocationName ActualReceiverName" +
            //        "	, '' ActualReceiverName" +
            //        "	, TransportOrder.DefaultSeqNo" +
            //        "	, TransportOrder.SeqNo" +
            //        "	, TransportOrder.VehicleType" +
            //        "	, TransportOrder.Remarks" +
            //        "	, TransportOrder.CreatedBy" +
            //        "	, TransportOrder.UpdatedBy " +
            //        "	, TransportExecution.TransportNo TN" +
            //        "	, MasterUser1.FullName CreatedByFullName" +
            //        "	, MasterUser2.FullName UpdatedByFullName" +
            //        "from TransportOrder" +
            //        "join MasterGenWeek" +
            //        "	on TransportOrder.ShipmentDate between MasterGenWeek.StartDate and MasterGenWeek.EndDate" +
            //        "	and MasterGenWeek.Week = '30'" +
            //        "	and MasterGenWeek.Year = '2024'" +
            //        "join TransportOrderDetail " +
            //        "	ON TransportOrder.IDTransportOrder = TransportOrderDetail.IDTransportOrder" +
            //        "join MasterLocation MasterLocation1" +
            //        "	ON MasterLocation1.IDLocation = TransportOrder.SenderIDLocation" +
            //        "join MasterLocation MasterLocation2" +
            //        "	ON MasterLocation2.IDLocation = TransportOrder.ReceiverIDLocation" +
            //        "--join MasterLocation MasterLocation3" +
            //        "--	ON MasterLocation1.IDLocation = TransportOrder.ActualSenderIDLocation" +
            //        "--join MasterLocation MasterLocation4" +
            //        "--	ON MasterLocation2.IDLocation = TransportOrder.ActualReceiverIDLocation" +
            //        "join TransportExecution" +
            //        "	on TransportExecution.IDTransportExecution = transportorder.IDTransportExecution" +
            //        "join MasterUser MasterUser1" +
            //        "	on MasterUser1.iduser = TransportOrder.CreatedBy" +
            //        "join MasterUser MasterUser2" +
            //        "	on MasterUser2.iduser = TransportOrder.UpdatedBy" +
            //        "WHERE TransportOrder.IsActive = 1" +
            //        "and TransportOrder.IDTransportPickingListLog is not null" +
            //        "order by" +
            //        "	TransportOrder.ShipmentDate" +
            //        "	, TransportOrder.SeqNo" +
            //        "	, MasterLocation2.LocationName" +
            //        "	, MasterLocation1.LocationName" +
            //        "", '1').ToList();*/

            //    var dateDist = _transportPickingListBll.GetRawTOD(criteria)
            //    .OrderBy(x => x.ShipmentDate)
            //    .ThenBy(x => x.SeqNo != null ? Int64.Parse(x.SeqNo) : 999999999999999)
            //    //.ThenBy(x => x.DefaultSeqNo != null ? x.DefaultSeqNo.Length : 99999999999999)
            //    .ThenBy(x => x.DefaultSeqNo != null ? Int64.Parse(x.DefaultSeqNo) : 999999999999999)
            //    .ThenBy(x => x.ReceiverLocationName)
            //    .ThenBy(x => x.SenderLocationName);

            //    //update TN, CreatedBy and UpdatedBy
            //    (from d in dateDist
            //     join te in ctx.TransportExecutions on d.IDTransportExecution equals te.IDTransportExecution into dte
            //     from te in dte.DefaultIfEmpty()
            //     join u1 in ctx.MasterUsers on d.CreatedBy.ToLower() equals u1.IDUser.ToLower() into du
            //     from u1 in du.DefaultIfEmpty()
            //     join u2 in ctx.MasterUsers on d.UpdatedBy.ToLower() equals u2.IDUser.ToLower() into du2
            //     from u2 in du2.DefaultIfEmpty()
            //     select new { d, te, u1, u2 }).ToList()
            //    .ForEach(r =>
            //    {
            //        r.d.TN = r.te != null ? r.te.TransportNo : "";
            //        r.d.CreatedByFullName = r.u1 != null ? r.u1.FullName : "";
            //        r.d.UpdatedByFullName = r.u2 != null ? r.u2.FullName : "";
            //    });
            //              //select  d.IDTransportExecution = te.IDTransportExecution;

            //    /*
            //    var getAllTE = _transportPickingListBll.getAllTransportExecution();
            //    foreach (var datas in dateDist)
            //    {
            //        if (datas.IDTransportExecution != null)
            //        {
            //           var ExecData = getAllTE.Where(x => x.IDTransportExecution == datas.IDTransportExecution).FirstOrDefault();

            //            if (ExecData != null)
            //            {
            //                datas.TN = ExecData.TransportNo;
            //            }
            //        }
            //        datas.CreatedByFullName = _transportPickingListBll.getUserFullName(datas.CreatedBy).ToString();
            //        datas.UpdatedByFullName = _transportPickingListBll.getUserFullName(datas.UpdatedBy).ToString();
            //    }
            //    */
            //    var ws = pkg.CreateWorksheet("Export");

            //    // write header
            //    pkg.WriteHeader(ws);
            //    // write values
            //    pkg.WriteRows(ws, dateDist, (dto, cell, pi) =>
            //    {
            //        var val = pi.GetValue(dto);
            //        if (pi.Name == "ShipmentDate") cell.Style.Numberformat.Format = "dd MMM yyyy";
            //        return val;
            //    });
            //    pkg.AutoSizeColumns(ws);

            //    String path = "TransportPickingListCustom" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            //    pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

            //    return Json(path, JsonRequestBehavior.AllowGet);
            //}
            #endregion

            #region new code Nicco

            LoadPage(criteria);
            var dateDist = Session["DateDist"] as List<TransportPickingListViewDTO>;

            if (dateDist == null)
            {
                return Json("Error: Data not found", JsonRequestBehavior.AllowGet);
            }

            using (var pkg = new ExcelExportHelper())
            {
                foreach (var headerName in colCaption.Split(','))
                {
                    if (headerName.Contains("STO Number")) pkg.Map("STO Number", "STONo");
                    else if (headerName.Contains("Supplier")) pkg.Map("Supplier", "Supplier");
                    else if (headerName.Contains("Item")) pkg.Map("Item", "Item");
                    else if (headerName.Contains("Brand")) pkg.Map("Brand", "Brand");
                    else if (headerName.Contains("Qty")) pkg.Map("Qty", "Qty");
                    else if (headerName.Contains("UoM")) pkg.Map("UoM", "UoM");
                    else if (headerName.Contains("Shipment Date")) pkg.Map("Promise Date", "ShipmentDate");
                    else if (headerName.Contains("Sender")) pkg.Map("Sender", "Sender");
                    else if (headerName.Contains("Receiver")) pkg.Map("Receiver", "Receiver");
                    else if (headerName.Contains("Zone")) ;
                    else if (headerName.Contains("Sequence")) pkg.Map("Seq", "Sequence");
                    else if (headerName.Contains("Vehicle Type")) pkg.Map("Vehicle Type", "VehicleType");
                    else if (headerName.Contains("Remarks")) pkg.Map("Remarks", "Remarks");
                    else if (headerName.Contains("Created By")) pkg.Map("Created By", "CreatedBy");
                    else if (headerName.Contains("Updated By")) pkg.Map("Updated By", "UpdatedBy");
                }

                var ws = pkg.CreateWorksheet("Export");

                pkg.WriteHeader(ws);
                pkg.WriteRows(ws, dateDist, (dto, cell, pi) =>
                {
                    var val = pi.GetValue(dto);
                    if (pi.Name == "ShipmentDate") cell.Style.Numberformat.Format = "dd MMM yyyy";
                    return val;
                });
                pkg.AutoSizeColumns(ws);

                String path = "TransportPickingListCustom" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

                return Json(path, JsonRequestBehavior.AllowGet);
            }

            #endregion

            //return null;
        } 
        //public JsonResult CustomExportExcel(TransportOrderInput criteria)
        //{
        //    var data = CustomExportXls("ShipmentDate", criteria);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        /* ----- EXPORT END ----- */

        /* ----- START OF SEND EMAIL ----- */
        public ActionResult SendEmail()
        {
            return Json(_transportPickingListBll.SendEmail(GetUserId()), JsonRequestBehavior.AllowGet);
        }
        /* ----- END OF SEND EMAIL ----- */

        public ActionResult updateIsActive()
        {
            return Json(_transportPickingListBll.UpdateIsActive(), JsonRequestBehavior.AllowGet);
        }
    }
}