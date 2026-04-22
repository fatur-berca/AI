using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using OfficeOpenXml;
using TOM.Transport.Repositories;
using TOM.Master.Repositories;
using DFIS.Universal.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using DFIS.Contracts;
using System.Diagnostics;
using System.Collections;

namespace TOM.Transport.BusinessLogics
{
    public class TransportPickingListBLL : ITransportPickingListBLL
    {
        private readonly ITransportPickingListLogRepo _transportPickingListLogRepo;
        private readonly ITransportOrderRepo _transportOrderRepo;
        private readonly ITransportOrderDetailRepo _transportOrderDetailRepo;
        private readonly ITransportOrderChangeLogRepo _transportOrderChangeLogRepo;
        private readonly ITransportOrderDetailChangeLogRepo _transportOrderDetailChangeLogRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IMasterGenWeekRepo _masterGenWeekRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;
        private readonly IMasterMappingRepo _masterMappingRepo;
        private readonly IGenericRepository<TransportPickingListLog> _generalRepo;
        private readonly IGenericRepository<TransportOrder> _transportOrderGenericRepo;
        private readonly IGenericRepository<TransportExecution> _transportExecutionRepo;
        private readonly IGenericRepository<MasterLocation> _masterLocationGenericRepo;
        private readonly IGenericRepository<MasterUser> _masterUserRepo;
        private readonly IGenericRepository<TransportOrderChangeLog> _transportOrderCLRepo;
        private readonly IGenericRepository<TransportOrderDetailChangeLog> _transportOrderDetailCLRepo;
        private readonly IMasterFABrandRepo  _masterFABrand;
        private string _hexYellow = "#FFFF00";
        private readonly string[] DayNames = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        public TransportPickingListBLL(IGenericRepository<TransportPickingListLog> generalRepo, IGenericRepository<TransportOrderDetailChangeLog> transportOrderDetailCLRepo, IGenericRepository<TransportOrderChangeLog> transportOrderCLRepo, IGenericRepository<MasterUser> masterUserRepo, IGenericRepository<MasterLocation> masterLocationGenericRepo, IGenericRepository<TransportOrder> transportOrderGenericRepo, ITransportOrderRepo transportOrderRepo, ITransportOrderChangeLogRepo transportOrderChangeLogRepo, IMasterListRepo masterListRepo, IMasterGenWeekRepo masterGenWeekRepo, IMasterLocationRepo masterLocationRepo, ITransportOrderDetailRepo transportOrderDetailRepo, ITransportPickingListLogRepo transportPickingListLogRepo, IMasterMappingRepo masterMappingRepo, ITransportOrderDetailChangeLogRepo transportOrderDetailChangeLogRepo, IGenericRepository<TransportExecution> transportExecutionRepo, IMasterFABrandRepo masterFABrand)
        {
            _transportOrderRepo = transportOrderRepo;
            _transportOrderChangeLogRepo = transportOrderChangeLogRepo;
            _masterListRepo = masterListRepo;
            _masterGenWeekRepo = masterGenWeekRepo;
            _masterLocationRepo = masterLocationRepo;
            _transportOrderDetailRepo = transportOrderDetailRepo;
            _transportPickingListLogRepo = transportPickingListLogRepo;
            _masterMappingRepo = masterMappingRepo;
            _transportOrderDetailChangeLogRepo = transportOrderDetailChangeLogRepo;
            _generalRepo = generalRepo;
            _transportExecutionRepo = transportExecutionRepo;
            _transportOrderGenericRepo = transportOrderGenericRepo;
            _masterLocationGenericRepo = masterLocationGenericRepo;
            _masterUserRepo = masterUserRepo;
            _transportOrderCLRepo = transportOrderCLRepo;
            _transportOrderDetailCLRepo = transportOrderDetailCLRepo;
            _masterFABrand = masterFABrand;
        }

        public List<TransportOrderDTO> GetSTONoFilter(string stono)
        {
            //Configuration.ProxyCreationEnabled = false; //menambah ini
            return Mapper.Map<List<TransportOrder>, List<TransportOrderDTO>>(_transportOrderRepo.GetSTONoFilter(stono)); 
        }

        public List<TransportOrderDTO> GetALLPickingList(TransportOrderInput criteria)
        {           
            MasterGenWeek dateFilter = new MasterGenWeek();
            if (criteria.weekFilter != 0 && criteria.yearFilter != 0)
            {
                dateFilter = _masterGenWeekRepo.GetFromToDateByWeekYear(criteria.weekFilter, criteria.yearFilter);
                criteria.dateFromFilter = dateFilter.StartDate;
                criteria.dateToFilter = dateFilter.EndDate;
            }
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive && x.IDTransportPickingListLog != null);

            if (!string.IsNullOrEmpty(criteria.DayName))
            {
                int Day = Array.IndexOf(DayNames, criteria.DayName);
                criteria.dateFromFilter = criteria.dateFromFilter.Value.AddDays(Day);
                criteria.dateToFilter = null;
                queryFilter = queryFilter.And(x => x.ShipmentDate == criteria.dateFromFilter);
            }
            if (criteria.dateFromFilter != null && criteria.dateToFilter != null)
            {
                queryFilter = queryFilter.And(x => x.ShipmentDate >= criteria.dateFromFilter && x.ShipmentDate <= criteria.dateToFilter);
            }
            if (criteria.zoneFilter != null)
            {
                queryFilter = queryFilter.And(x => x.ZoneBased == criteria.zoneFilter);
            }
            if (criteria.stoNoListFilter != null && !String.IsNullOrEmpty(criteria.stoNoListFilter[0]))
            {
                var stoNoList = criteria.stoNoListFilter[0].Split(',');
                queryFilter = queryFilter.And(x => stoNoList.Contains(x.STONo));
            }
            if (criteria.senderIdLocListFilter != null && !String.IsNullOrEmpty(criteria.senderIdLocListFilter[0]))
            {
                var senderIdLocList = criteria.senderIdLocListFilter[0].Split(',');
                queryFilter = queryFilter.And(x => criteria.senderIdLocListFilter.Contains(x.ActualSenderIDLocation));
            }
            if (criteria.receiverIdLocListFilter != null && !String.IsNullOrEmpty(criteria.receiverIdLocListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.receiverIdLocListFilter.Contains(x.ActualReceiverIDLocation));
            }
            if (criteria.uploadedByFilter != null && criteria.uploadedByFilter != "")
            {
                queryFilter = queryFilter.And(x => criteria.uploadedByFilter.Contains(x.CreatedBy));
            }

            queryFilter = queryFilter.And(x => x.IsActive && x.IDTransportPickingListLog != null);

            return Mapper.Map<List<TransportOrderDTO>>(_transportOrderGenericRepo.Get(queryFilter));
            //return _transportOrderRepo.GetAllTransportOrder(criteria);
        }

        public List<MasterLocationDTO> getAllMasterLocation() {
            return Mapper.Map<List<MasterLocationDTO>>(_masterLocationGenericRepo.GetAll().Where(x => x.IsActive));
        }

        public string getUserFullName(string IDUser)
        {
            return _masterUserRepo.GetByID(IDUser) == null ? "" : _masterUserRepo.GetByID(IDUser).FullName.ToString();
        }

        public List<string> GetAllUploadedBy(TransportOrderInput criteria)
        {
            MasterGenWeek dateFilter = new MasterGenWeek();
            if (criteria.weekFilter != 0 && criteria.yearFilter != 0)
            {
                dateFilter = _masterGenWeekRepo.GetFromToDateByWeekYear(criteria.weekFilter, criteria.yearFilter);
                criteria.dateFromFilter = dateFilter.StartDate;
                criteria.dateToFilter = dateFilter.EndDate;
            }
            return _transportOrderRepo.GetAllUploadedBy(criteria);
        }

        public List<MasterListDTO> GetVehicleTypeList()
        {
            return Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("VehicleType"));
        }

        public MasterGenWeek GetWeekNow()
        {
            DateTime dateNow = DateTime.Today;
            return _masterGenWeekRepo.GetGenWeekNowByDate(dateNow);
        }

        public List<MasterLocationDTO> GetZoneList()
        {
            return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Zone").ToList());
        }

        public List<MasterLocationDTO> GetSenderReceiverList()
        {
            List<MasterLocationDTO> temp = new List<MasterLocationDTO>();
            temp.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocation().ToList()));
            /*temp.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Warehouse").ToList()));
            temp.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Factory").ToList()));
            temp.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Agent").ToList()));
            temp.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Other").ToList()));*/
            return temp;
        }

        public List<TransportExecutionDTO> getAllTransportExecution()
        {
            return Mapper.Map<List<TransportExecutionDTO>>(_transportExecutionRepo.GetAll().Where(x => x.IsActive)); 
        }

        public TransportOrderDTO SelectTransportOrderByStoNo(string stono)
        {
            TransportOrderDTO temp = new TransportOrderDTO();
            if (stono != null)
            {
                temp = _transportOrderRepo.GetTransportOrderBySTONo(stono);
                TransportOrderInput filter = new TransportOrderInput();
                filter.IDTransportOrder = temp.IDTransportOrder;
                temp.TransportOrderDetails = _transportOrderDetailRepo.GetAllTransportOrderDetail(filter);
                temp.TransportOrderChangeLogs = Mapper.Map<IEnumerable<TransportOrderChangeLog>,List<TransportOrderChangeLogDTO>>(_transportOrderCLRepo.Get(x => x.IDTransportOrder == temp.IDTransportOrder));
                //if (temp.TransportOrderChangeLogs.Any(x => x.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Supplier)))
                //    temp.SupplierColor = _hexYellow;
                if (temp.TransportOrderChangeLogs != null)
                {
                    if (temp.TransportOrderChangeLogs.Any(x => x.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.ShipmentDate)))
                        temp.ShipmentDateColor = _hexYellow;
                    if (temp.TransportOrderChangeLogs.Any(x => x.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.SenderLocation)))
                        temp.SenderLocationColor = _hexYellow;
                    if (temp.TransportOrderChangeLogs.Any(x => x.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.ReceiverLocation)))
                        temp.ReceiverLocationColor = _hexYellow;
                }
                if (temp.TransportOrderDetails != null)
                {
                    foreach (var idDetail in temp.TransportOrderDetails)
                    {
                        idDetail.TransportOrderDetailChangeLogs = Mapper.Map<IEnumerable<TransportOrderDetailChangeLog>, List<TransportOrderDetailChangeLogDTO>>(_transportOrderDetailCLRepo.Get(x => x.IDTransportOrderDetail == idDetail.IDTransportOrderDetail));
                    }
                    temp.TransportOrderDetails.ToList().ForEach(x =>
                    {
                        if (x.TransportOrderDetailChangeLogs != null)
                        {
                            if (x.TransportOrderDetailChangeLogs.Any(y => y.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.MaterialType)))
                                x.MaterialTypeColor = _hexYellow;
                            if (x.TransportOrderDetailChangeLogs.Any(y => y.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Supplier)))
                                x.SupplierColor = _hexYellow;
                            if (x.TransportOrderDetailChangeLogs.Any(y => y.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Code)))
                                x.CodeColor = _hexYellow;
                            if (x.TransportOrderDetailChangeLogs.Any(y => y.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Description)))
                                x.DescriptionColor = _hexYellow;
                            if (x.TransportOrderDetailChangeLogs.Any(y => y.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Quantity)))
                                x.QtyColor = _hexYellow;
                            if (x.TransportOrderDetailChangeLogs.Any(y => y.ModifiedField == EnumHelper.GetDescription(Enums.TransportOrderChangeLog.UoM)))
                                x.UoMColor = _hexYellow;
                        }
                    });
                }
            }
            temp.MasterList = GetVehicleTypeList();
            return temp;
        }

        public int SaveData(TransportOrderDTO input, string userid)
        {
            TransportOrder temp = _transportOrderRepo.GetTransportOrderBySTONo2(input.STONo);
            List<TransportOrderChangeLog> tempListLog = new List<TransportOrderChangeLog>();
            TransportOrderChangeLog latestVersion = _transportOrderChangeLogRepo.GetLatestVersionByTransportOrderID(temp.IDTransportOrder);
            TransportOrderChangeLog tempLog;
            string changeLogFields = temp.ChangeLogFields;
            if (input.VehicleType != temp.VehicleType)
            {
                tempLog = new TransportOrderChangeLog
                {
                    IDTransportOrder = temp.IDTransportOrder,
                    Version = (latestVersion == null ? 0 : latestVersion.Version) + 1,
                    OldValue = temp.VehicleType,
                    ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.VehicleType),
                    CreatedDate = DateTime.Now,
                    CreatedBy = userid,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = userid,
                    TransportOrder = temp
                };
                tempListLog.Add(tempLog);
                temp.VehicleType = input.VehicleType;
                if (changeLogFields == null || !changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.VehicleType)))
                    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.VehicleType);
            }
            if (input.SeqNo != temp.SeqNo)
            {
                tempLog = new TransportOrderChangeLog
                {
                    IDTransportOrder = temp.IDTransportOrder,
                    Version = (latestVersion != null ? latestVersion.Version ?? 0 : 0) + 1,
                    OldValue = temp.SeqNo,
                    ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence),
                    CreatedDate = DateTime.Now,
                    CreatedBy = userid,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = userid,
                    TransportOrder = temp
                };
                tempListLog.Add(tempLog);
                temp.SeqNo = input.SeqNo;                
                if (changeLogFields == null || !changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence)))
                    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence);
            }
            input.Remarks = input.Remarks ?? string.Empty;
            temp.Remarks = temp.Remarks ?? string.Empty;
            if (input.Remarks != temp.Remarks)
            {
                tempLog = new TransportOrderChangeLog
                {
                    IDTransportOrder = temp.IDTransportOrder,
                    Version = (latestVersion != null ? latestVersion.Version ?? 0 : 0) + 1,
                    OldValue = temp.Remarks,
                    ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Remarks),
                    CreatedDate = DateTime.Now,
                    CreatedBy = userid,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = userid,
                    TransportOrder = temp
                };
                tempListLog.Add(tempLog);
                temp.Remarks = input.Remarks;
                if (changeLogFields == null || !changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Remarks)))
                    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Remarks);
            }
            if (temp.FlagEmail == 2)
                temp.FlagEmail = 1;
            temp.ChangeLogFields = changeLogFields;
            temp.TransportOrderChangeLogs = tempListLog;
            temp.UpdatedBy = userid;
            _transportOrderRepo.SaveData(temp);
            return 0;
        }

        public int SaveDetailData(TransportOrderDetailDTO input, string userid)
        {
            TransportOrderDetail temp = _transportOrderDetailRepo.GetDataByID(input.IDTransportOrderDetail);
            if (temp != null)
            // Update data
            {
                List<TransportOrderDetailChangeLog> tempListLog = new List<TransportOrderDetailChangeLog>();
                TransportOrderDetailChangeLog latestVersion = _transportOrderDetailChangeLogRepo.GetLatestVersionByTransportOrderDetailID(temp.IDTransportOrderDetail);
                TransportOrderDetailChangeLog tempLog;
                //string changeLogFields = temp.ChangeLogFields;
                if (input.Qty != temp.Qty)
                {
                    tempLog = new TransportOrderDetailChangeLog
                    {
                        IDTransportOrderDetail = temp.IDTransportOrderDetail,
                        Version = (latestVersion == null ? 0 : latestVersion.Version) + 1,
                        OldValue = temp.Qty.ToString(),
                        ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Quantity),
                        CreatedDate = DateTime.Now,
                        CreatedBy = userid,
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = userid,
                    };
                    tempListLog.Add(tempLog);
                    temp.Qty = input.Qty;
                    //if (changeLogFields == null || !changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.VehicleType)))
                    //    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.VehicleType);
                }
                if (input.UoM != temp.UoM)
                {
                    tempLog = new TransportOrderDetailChangeLog
                    {
                        IDTransportOrderDetail = temp.IDTransportOrderDetail,
                        Version = (latestVersion == null ? 0 : latestVersion.Version) + 1,
                        OldValue = temp.UoM,
                        ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.UoM),
                        CreatedDate = DateTime.Now,
                        CreatedBy = userid,
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = userid,
                    };
                    tempListLog.Add(tempLog);
                    temp.UoM = input.UoM;
                    //if (!changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence)))
                    //    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence);
                }
                if (input.Code != temp.Code)
                {
                    tempLog = new TransportOrderDetailChangeLog
                    {
                        IDTransportOrderDetail = temp.IDTransportOrderDetail,
                        Version = (latestVersion == null ? 0 : latestVersion.Version) + 1,
                        OldValue = temp.Code,
                        ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Code),
                        CreatedDate = DateTime.Now,
                        CreatedBy = userid,
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = userid,
                    };
                    tempListLog.Add(tempLog);
                    temp.Code = input.Code;
                    //if (!changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence)))
                    //    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence);
                }
                if (input.Description != temp.Description)
                {
                    tempLog = new TransportOrderDetailChangeLog
                    {
                        IDTransportOrderDetail = temp.IDTransportOrderDetail,
                        Version = (latestVersion == null ? 0 : latestVersion.Version) + 1,
                        OldValue = temp.Description,
                        ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Description),
                        CreatedDate = DateTime.Now,
                        CreatedBy = userid,
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = userid,
                    };
                    tempListLog.Add(tempLog);
                    temp.Description = input.Description;
                    //if (!changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence)))
                    //    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence);
                }
                if (input.Supplier != temp.Supplier)
                {
                    tempLog = new TransportOrderDetailChangeLog
                    {
                        IDTransportOrderDetail = temp.IDTransportOrderDetail,
                        Version = (latestVersion == null ? 0 : latestVersion.Version) + 1,
                        OldValue = temp.Supplier,
                        ModifiedField = EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Supplier),
                        CreatedDate = DateTime.Now,
                        CreatedBy = userid,
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = userid,
                    };
                    tempListLog.Add(tempLog);
                    temp.Supplier = input.Supplier;
                    //if (!changeLogFields.Contains(EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence)))
                    //    changeLogFields = changeLogFields + ", " + EnumHelper.GetDescription(Enums.TransportOrderChangeLog.Sequence);
                }
                //temp.ChangeLogFields = changeLogFields;
                temp.TransportOrderDetailChangeLogs = tempListLog;
                temp.UpdatedBy = userid;
                temp.MaterialType = "Cigarette";
                temp.IsActive = input.IsActive;
                if (!input.IsActive) temp.Qty = 0;
                _transportOrderDetailRepo.SaveData(temp, false);
            }
            else
            // Add data
            {
                temp = Mapper.Map<TransportOrderDetailDTO, TransportOrderDetail>(input);
                if (temp.Description != null && temp.Code != null && input.IsActive)
                {
                    temp.UpdatedBy = userid;
                    temp.CreatedBy = userid;
                    temp.UpdatedDate = DateTime.Now;
                    temp.CreatedDate = DateTime.Now;
                    temp.IsActive = true;
                    temp.MaterialType = "Cigarette";
                    _transportOrderDetailRepo.SaveData(temp, true);
                }
            }
            return 0;
        }
        public List<string> UploadPickingList2(HttpPostedFileBase input, string userid)
        {
            TOMContextDB context = new TOMContextDB();

            var stream = input.InputStream;
            var filename = input.FileName;
            List<string> listError = new List<string>();
            List<TransportPickingListLog> listPickingList = new List<TransportPickingListLog>();
            List<TransportOrder> listTO = new List<TransportOrder>();
            List<TransportOrderDetail> listTODetail = new List<TransportOrderDetail>();
            List<string> listSupplierPmi = _masterListRepo.GetMasterListFieldNameValue("ExcludeSupplier");
            var currentdatetime = DateTime.Now;
            var dbContextTransaction = context.Database.BeginTransaction();
            //var latestSTOPO = "";
            var month = 0;
            var year = 0;
            int lastSeq = 0;
            int setSeq = 0;
            Hashtable HashSeq = new Hashtable();
            try
            {
                using (ExcelPackage xlPackage = new ExcelPackage(stream))
                {                   
                    foreach (var sheet in xlPackage.Workbook.Worksheets)
                    {
                        if (string.IsNullOrEmpty(sheet.Cells[4, 2].Text)) continue;

                        var totalRows = sheet.Dimension.End.Row;
                        int iRowCount = sheet.Dimension.End.Row - sheet.Dimension.Start.Row;

                        //bool isError = false;
                        for (int rowNum = 4; rowNum <= totalRows; rowNum++)
                        {
                            var sheetName = sheet.Name;
                            var cellsItem = sheet.Cells[rowNum, 3].Text.Trim();
                            var cellsBranch = sheet.Cells[rowNum, 14].Text.Trim();

                            //if ( !string.IsNullOrEmpty(sheet.Cells[rowNum, 2].Text) && !string.IsNullOrEmpty(sheet.Cells[rowNum, 3].Text))
                            //{
                            //    listError.AddRange(CheckValidate(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, sheet.Name));
                            //    //isError = true;
                            //}
                            var rowStatus = true;
                            if (sheet.Cells[rowNum, 2].Text.Trim() == "" && sheet.Cells[rowNum, 1].Text.Trim() == "")
                            {
                                rowStatus = false;
                            }
                            if (rowStatus == true)
                            {
                                listError.AddRange(CheckValidate(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, sheet.Name));
                            }
                        }
                        /*if (isError)
                        {
                            continue;
                        }*/
                        //if (!string.IsNullOrEmpty(sheet.Cells[4, 6].Text)){
                            setSeq = _transportOrderRepo.getLastSequenceByShipmentDate(DateTime.Parse(sheet.Cells[4, 6].Text));
                            var latestSTOPO = _transportOrderRepo.getLastSTOPreOrder(DateTime.Parse(sheet.Cells[4, 6].Text));
                        //}
                        //month = latestSTOPO.Substring
                        //latestSTOPO = _transportOrderRepo.getLastSTOPreOrder(DateTime.Parse(sheet.Cells[4, 6].Text));
                        HashSeq.Add(sheet.Name, setSeq);
                        var YearMonth = DateTime.Parse(sheet.Cells[4, 6].Text).ToString("yyMM");
                        if (!HashSeq.ContainsKey(YearMonth))
                        {
                            HashSeq.Add(YearMonth, latestSTOPO);
                        }
                        // check validate
                        
                    }
                    if (listError.Count == 0)
                    {
                        foreach (var sheet in xlPackage.Workbook.Worksheets)
                        {
                            if (string.IsNullOrEmpty(sheet.Cells[4, 2].Text)) continue;
                            //Debug.WriteLine("Sheet: " + sheet.Index);
                            lastSeq = Convert.ToInt32(HashSeq[sheet.Name]);
                            //var lastStoPO = Convert.ToString(HashSeq[sheet.Index]);
                            //int lastSeq = _transportOrderRepo.getLastSequenceByShipmentDate(DateTime.Parse(sheet.Cells[4, 6].Text));
                            var defaultSeq = "";                        
                            // insert picking list log
                            TransportPickingListLog tempPL = new TransportPickingListLog();
                            tempPL.ShipmentDate = DateTime.Parse(sheet.Cells[4, 6].Text);
                            tempPL.IsActive = true;
                            tempPL.CreatedBy = userid;
                            tempPL.UpdatedBy = userid;
                            tempPL.CreatedDate = currentdatetime;
                            tempPL.UpdatedDate = currentdatetime;
                            context.TransportPickingListLogs.Add(tempPL);
                            context.SaveChanges();
                            var idPL = tempPL.IDTransportPickingListLog;
                            //Debug.WriteLine("idPL:" + idPL);                            
                            //var idPL = _transportPickingListLogRepo.SaveData(tempPL, true);

                            var totalRows = sheet.Dimension.End.Row;
                            var totalCols = sheet.Dimension.End.Column;
                            var seq = "";
                            var currentSeq = "";
                            var sender = "";
                            var currentsender = "";
                            var receiver = "";
                            var currentreceiver = "";
                            var supplier = "";
                            var currentsup = "";
                            var idTO = 0;
                            for (int rowNum = 4; rowNum <= totalRows; rowNum++)
                            {
                                //if (!string.IsNullOrEmpty(sheet.Cells[rowNum, 2].Text) && !string.IsNullOrEmpty(sheet.Cells[rowNum, 3].Text))
                                var rowStatus = true;
                                if (sheet.Cells[rowNum, 2].Text.Trim() == "" && sheet.Cells[rowNum, 1].Text.Trim() == "")
                                {
                                    rowStatus = false;
                                }
                                if (rowStatus == true)
                                {
                                    //Debug.WriteLine("rowNum: " + rowNum);
                                    currentsup = sheet.Cells[rowNum, 2].Text;
                                    currentsender = sheet.Cells[rowNum, 9].Text + sheet.Cells[rowNum, 10].Text;
                                    currentreceiver = sheet.Cells[rowNum, 7].Text + sheet.Cells[rowNum, 8].Text;
                                    if (string.IsNullOrEmpty(sheet.Cells[rowNum, 11].Text))
                                    {
                                        currentSeq = null;
                                    }
                                    else
                                    {
                                        currentSeq = sheet.Cells[rowNum, 11].Text;
                                    }
                                    if (currentSeq == null)
                                    {
                                        if (currentsender == sender && currentreceiver == receiver)
                                        {
                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);
                                        }
                                        else
                                        {
                                            // insert TO      
                                            var latestSTOPO = ""+HashSeq[DateTime.Parse(sheet.Cells[4, 6].Text).ToString("yyMM")];
                                            latestSTOPO = _transportPickingListLogRepo.setCurrSTOPreOrder(latestSTOPO);
                                            HashSeq[DateTime.Parse(sheet.Cells[4, 6].Text).ToString("yyMM")] = latestSTOPO;

                                            var promiseDate = DateTime.Parse(sheet.Cells[4, 6].Text).ToString("yy-MM-dd");
                                            TransportOrder existPO = context.TransportOrders.Where(x => x.SenderIDLocation == currentsender && x.ReceiverIDLocation == currentreceiver && x.IDTransportPickingListLog != null && x.DefaultSeqNo == null && x.ShipmentDate.ToString().Contains(promiseDate)).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                                            if (existPO != null)
                                                existPO.IsActive = false;
                                            
                                            TransportOrder tempTO = new TransportOrder();                                            
                                            tempTO = setEntityTO(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idPL, 0, userid, filename, latestSTOPO);
                                            //context.Configuration.AutoDetectChangesEnabled = false;
                                            //context.Configuration.ValidateOnSaveEnabled = false;
                                            //context.Set<TransportOrder>().Add(tempTO);
                                            context.TransportOrders.Add(tempTO);
                                            context.SaveChanges();
                                            idTO = tempTO.IDTransportOrder;
                                            //idTO = _transportOrderRepo.SaveDataTOFromUpload(tempTO);
                                            //Debug.WriteLine("idTO: " + idTO);

                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);                                            

                                            seq = currentSeq;
                                            supplier = currentsup;
                                            sender = currentsender;
                                            receiver = currentreceiver;
                                        }
                                    }
                                    else
                                    {
                                        if (currentSeq == seq && (currentsup == supplier || (!listSupplierPmi.Contains(currentsup) && !listSupplierPmi.Contains(supplier))) && currentsender == sender && currentreceiver == receiver)
                                        {
                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);
                                        }
                                    else
                                    {
                                        if (seq != currentSeq)
                                        {
                                                lastSeq = lastSeq + 1;
                                        }
                                        // insert TO
                                        TransportOrder tempTO = new TransportOrder();
                                        tempTO = setEntityTO(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idPL, lastSeq, userid, filename);
                                            //context.Configuration.AutoDetectChangesEnabled = false;
                                            //context.Configuration.ValidateOnSaveEnabled = false;
                                            //context.Set<TransportOrder>().Add(tempTO);
                                            context.TransportOrders.Add(tempTO);
                                            context.SaveChanges();
                                            idTO = tempTO.IDTransportOrder;
                                            //idTO = _transportOrderRepo.SaveDataTOFromUpload(tempTO);
                                            //Debug.WriteLine("idTO: " + idTO);

                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);

                                            seq = currentSeq;
                                        supplier = currentsup;
                                        sender = currentsender;
                                        receiver = currentreceiver;
                                    }
                                }

                            }

                        }
                    }
                }
                    //_transportOrderDetailRepo.SaveDataTODetail(listTODetail);     
                    for (int i = 0; i < listTODetail.Count; i++)
                    {
                        context.TransportOrderDetails.Add(listTODetail[i]);                        
                    }
                    context.SaveChanges();
                    dbContextTransaction.Commit();
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                listError.Add(e.Message);
                dbContextTransaction.Rollback();
            }
            finally
            {
                dbContextTransaction.Dispose();
            }
            return listError;
        }
        public List<string> UploadPickingList(HttpPostedFileBase input, string userid)
        {
            TOMContextDB context = new TOMContextDB();
            var stream = input.InputStream;
            var filename = input.FileName;
            List<string> listError = new List<string>();
            List<TransportPickingListLog> listPickingList = new List<TransportPickingListLog>();
            List<TransportOrder> listTO = new List<TransportOrder>();
            List<TransportOrderDetail> listTODetail = new List<TransportOrderDetail>();
            List<string> listSupplierPmi = _masterListRepo.GetMasterListFieldNameValue("ExcludeSupplier");
            var currentdatetime = DateTime.Now;
            var dbContextTransaction = context.Database.BeginTransaction();
            try
            {
                using (ExcelPackage xlPackage = new ExcelPackage(stream))
                {
                    foreach (var sheet in xlPackage.Workbook.Worksheets)
                    {
                        // check validate
                        var totalRows = sheet.Dimension.End.Row;
                        for (int rowNum = 4; rowNum <= totalRows; rowNum++)
                        {
                            //if (!string.IsNullOrEmpty(sheet.Cells[rowNum, 2].Text) && !string.IsNullOrEmpty(sheet.Cells[rowNum, 3].Text))
                            //{
                            //    listError.AddRange(CheckValidate(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, sheet.Name));
                            //}
                            var rowStatus = true;
                            if (sheet.Cells[rowNum, 2].Text.Trim() == "" && sheet.Cells[rowNum, 1].Text.Trim() == "")
                            {
                                rowStatus = false;
                            }
                            if (rowStatus == true)
                            {
                                listError.AddRange(CheckValidate(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, sheet.Name));
                            }
                        }

                    }
                    if (listError.Count == 0)
                    {
                        foreach (var sheet in xlPackage.Workbook.Worksheets)
                        {
                            //Debug.WriteLine("Sheet: " + sheet.Name);
                            int lastSeq = _transportOrderRepo.getLastSequenceByShipmentDate(DateTime.Parse(sheet.Cells[4, 6].Text));
                            //Debug.WriteLine("last sequence:" + lastSeq);
                            var defaultSeq = "";
                            // insert picking list log
                            TransportPickingListLog tempPL = new TransportPickingListLog();
                            tempPL.ShipmentDate = DateTime.Parse(sheet.Cells[4, 6].Text);
                            tempPL.CreatedBy = userid;
                            tempPL.UpdatedBy = userid;
                            tempPL.CreatedDate = currentdatetime;
                            tempPL.UpdatedDate = currentdatetime;
                            var idPL = _transportPickingListLogRepo.SaveData(tempPL, true);
                            //Debug.WriteLine("idPL: " + idPL);
                            var totalRows = sheet.Dimension.End.Row;
                            var totalCols = sheet.Dimension.End.Column;
                            var seq = "";
                            var currentSeq = "";
                            var sender = "";
                            var currentsender = "";
                            var receiver = "";
                            var currentreceiver = "";
                            var supplier = "";
                            var currentsup = "";
                            var idTO = 0;
                            for (int rowNum = 4; rowNum <= totalRows; rowNum++)
                            {
                                //if (!string.IsNullOrEmpty(sheet.Cells[rowNum, 2].Text) && !string.IsNullOrEmpty(sheet.Cells[rowNum, 3].Text))
                                var rowStatus = true;
                                if (sheet.Cells[rowNum, 2].Text.Trim() == "" && sheet.Cells[rowNum, 1].Text.Trim() == "")
                                {
                                    rowStatus = false;
                                }
                                if (rowStatus == true)
                                {
                                    //Debug.WriteLine("row:" + rowNum);
                                    currentsup = sheet.Cells[rowNum, 2].Text;
                                    currentsender = sheet.Cells[rowNum, 9].Text;
                                    currentreceiver = sheet.Cells[rowNum, 7].Text;
                                    if (string.IsNullOrEmpty(sheet.Cells[rowNum, 11].Text))
                                    {
                                        currentSeq = null;
                                    }
                                    else
                                    {
                                        currentSeq = sheet.Cells[rowNum, 11].Text;
                                    }
                                    if (currentSeq == null)
                                    {
                                        if (currentsender == sender && currentreceiver == receiver)
                                        {
                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);
                                        }
                                        else
                                        {
                                            // insert TO                                
                                            TransportOrder tempTO = new TransportOrder();
                                            tempTO = setEntityTO(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idPL, lastSeq, userid, filename);
                                            idTO = _transportOrderRepo.SaveDataTOFromUpload(tempTO);
                                            //Debug.WriteLine("idTO: " + idTO);

                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);

                                            seq = currentSeq;
                                            supplier = currentsup;
                                            sender = currentsender;
                                            receiver = currentreceiver;
                                        }
                                    }
                                    else
                                    {
                                        if (currentSeq == seq && (currentsup == supplier || !listSupplierPmi.Contains(currentsup)) && currentsender == sender && currentreceiver == receiver)
                                        {
                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);
                                        }
                                        else
                                        {
                                            if (seq != currentSeq && rowNum >= 5)
                                            {
                                                defaultSeq = lastSeq++.ToString();
                                            }
                                            // insert TO
                                            TransportOrder tempTO = new TransportOrder();
                                            tempTO = setEntityTO(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idPL, lastSeq, userid, filename);
                                            idTO = _transportOrderRepo.SaveDataTOFromUpload(tempTO);
                                            //Debug.WriteLine("idTO: " + idTO);

                                            // insert order detail
                                            TransportOrderDetail tempTODetail = new TransportOrderDetail();
                                            tempTODetail = setEntityTODetail(sheet.Cells[rowNum, 1, totalRows, 13], rowNum, idTO, userid);
                                            listTODetail.Add(tempTODetail);

                                            seq = currentSeq;
                                            supplier = currentsup;
                                            sender = currentsender;
                                            receiver = currentreceiver;
                                        }
                                    }

                                }

                            }
                        }
                    }
                    _transportOrderDetailRepo.SaveDataTODetail(listTODetail);
                    dbContextTransaction.Commit();
                }
            }
            catch (Exception e)
            {
                listError.Add(e.Message);
                dbContextTransaction.Rollback();
            }
            finally
            {
                dbContextTransaction.Dispose();
            }
            return listError;
        }

        public List<string> CheckValidate(ExcelRange sheet, int rowNum, string sheetx)
        {
            List<string> listError = new List<string>();
            // cek validasi
            // get sender
            var currentsender = "";
            var currentreceiver = "";
            var actualsender = "";
            var actualreceiver = "";
            var uom = "";
            var vehicleType = "";
            var Item = sheet[rowNum, 3].Text.Trim();
            var Brand = sheet[rowNum, 14].Text.Trim();
            DateTime parseDate;
            int parseInt;
            if (string.IsNullOrEmpty(sheet[rowNum, 2].Text) || string.IsNullOrEmpty(sheet[rowNum, 3].Text) || string.IsNullOrEmpty(sheet[rowNum, 4].Text) || string.IsNullOrEmpty(sheet[rowNum, 5].Text) || string.IsNullOrEmpty(sheet[rowNum, 6].Text) || string.IsNullOrEmpty(sheet[rowNum, 7].Text) || string.IsNullOrEmpty(sheet[rowNum, 8].Text) || string.IsNullOrEmpty(sheet[rowNum, 9].Text) || string.IsNullOrEmpty(sheet[rowNum, 10].Text) || string.IsNullOrEmpty(sheet[rowNum, 14].Text))
            {
                listError.Add("Sheet - " + sheetx + " in Row - " + rowNum);
            }
            // check shipment date
            if (!String.IsNullOrEmpty(sheet[rowNum,6].Text))
            {
                bool parse = DateTime.TryParse(sheet[rowNum, 6].Text, out parseDate);
                if (!parse)                    
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Check Column Promise Date"); //Promise Date
            }
            // check sequence
            if (!String.IsNullOrEmpty(sheet[rowNum, 11].Text))
            {
                bool parse = int.TryParse(sheet[rowNum, 11].Text, out parseInt);
                if (!parse)
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Check Column Sequence"); //Seq
            }
            // check qty
            if (!String.IsNullOrEmpty(sheet[rowNum, 4].Text))
            {
                bool parse = int.TryParse(sheet[rowNum, 4].Text, out parseInt);
                if (!parse)
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Check Column Qty"); //Qty
                }
                else if (Convert.ToInt32(sheet[rowNum, 4].Text) < 1)
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Check Column Qty is 0"); //Qty
                }
                    
            }
            // get sender id
            if (!string.IsNullOrEmpty(sheet[rowNum, 9].Text))
            {
                currentsender = sheet[rowNum, 9].Text;
                var loc = _masterLocationRepo.GetMasterLocationByID(currentsender);
                if (loc == null)
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Chack Column From id"); //From ID
                }
            }
            // get actual sender id
            if (!string.IsNullOrEmpty(sheet[rowNum, 10].Text))
            {
                actualsender = sheet[rowNum, 10].Text;
                /*if (actualsender.Contains("Pre Order"))
                {
                    actualsender = actualsender.Substring(0, actualsender.Length - 9);
                } */
                var loc = _masterLocationRepo.GetMasterLocationActiveByName(actualsender);
                if (loc == null)
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Chack Column From"); //From Name
                }
            }
            // get receiver id
            if (!string.IsNullOrEmpty(sheet[rowNum, 7].Text))
            {
                currentreceiver = sheet[rowNum, 7].Text;
                var loc = _masterLocationRepo.GetMasterLocationByID(currentreceiver);
                if (loc == null)
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Chack Column Ship Id"); //column Ship to
                }
            }
            // get actual receiver id
            if (!string.IsNullOrEmpty(sheet[rowNum, 8].Text))
            {
                actualreceiver = sheet[rowNum, 8].Text;
                if (actualreceiver.Contains("Pre Order"))
                {
                    actualreceiver = actualreceiver.Substring(0, actualreceiver.Length - 9);
                    //Debug.WriteLine(actualreceiver);
                }
                var loc = _masterLocationRepo.GetMasterLocationActiveByName(actualreceiver);
                if (loc == null)
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Chack Column Ship To"); //Ship Name
                }
            }
            // get uom
            if (!string.IsNullOrEmpty(sheet[rowNum, 5].Text))
            {
                uom = sheet[rowNum, 5].Text;
                var uomvalue = _masterListRepo.GetMasterListCustom("UoM", uom);
                if (uomvalue == null)
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Chack Column UOM");
                }
            }
            // get vehicle type
            if (!string.IsNullOrEmpty(sheet[rowNum, 12].Text))
            {
                vehicleType = sheet[rowNum, 12].Text;
                var vehicle = _masterListRepo.GetMasterListCustom("VehicleType", vehicleType);
                if (vehicle == null || !string.Equals(vehicleType, vehicle.FieldValue, StringComparison.Ordinal))
                {
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " Chack Column VehicleType");
                }
            }
            if (!string.IsNullOrEmpty(sheet[rowNum, 3].Text) && !string.IsNullOrEmpty(sheet[rowNum, 14].Text))
            {
                var dataBaran = _masterFABrand.CheckBrand(Item, Brand);
                if (dataBaran == null)
                {
                    var dataFACode = _masterFABrand.CheckBrandFACode(Item);
                    var dataLONGCode = _masterFABrand.CheckBrandLONGCode(Brand);
                    var txtItem = "";
                    var txtBrand = "";
                    var Operator = "";
                    var TotNotFound = 0;
                    if (dataFACode == null)
                    {
                        txtItem = " Item [<b>" + Item  + "</b>]";
                        TotNotFound = TotNotFound + 1;
                    }

                    if (dataLONGCode == null)
                    {
                        txtBrand = " Brand [<b>" + Brand + "</b>]";
                        TotNotFound = TotNotFound + 1;
                    }
                    if (TotNotFound > 1)
                    {
                        Operator = " And";
                    }
                    //Sheet Minggu Row 6 tidak dapat di proses". Silahkan hubungin admin
                    listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " | " + txtItem + Operator + txtBrand + " Not Found");
                }
            }

            if (Item == "" || Brand == "")
            {
                var txtItem = "";
                var txtBrand = "";
                var Operator = "";
                var TotNotFound = 0;
                if (Item == "")
                {
                    txtItem = " Item ";
                    TotNotFound = TotNotFound + 1;
                }

                if (Item == null)
                {
                    txtBrand = " Brand";
                    TotNotFound = TotNotFound + 1;
                }
                if (TotNotFound > 1)
                {
                    Operator = " And";
                }
                listError.Add("Sheet - " + sheetx + " in Row - " + rowNum + " | " + txtItem + Operator + txtBrand + "  Is Empty");
            }
            return listError;
        }

        public string setActualSenderReceiver(string idloc, string locname)
        {
            var actualsender = "";
            var senderx = _masterLocationRepo.GetMasterLocationByID(idloc);
            if (senderx.LocationName == locname)
            {
                actualsender = idloc;
            }
            else
            {
                var actualx = _masterLocationRepo.GetMasterLocationActiveByName(locname);
                if (actualx == null)
                {
                    actualsender = locname;
                }
                else
                {
                    actualsender = actualx.IDLocation;
                }
            }
            return actualsender;
        }

        private TransportOrder setEntityTO(ExcelRange sheet, int rowNum, int idPL, int currentSeq, string userid, string filename, string STONo = "")
        {
            TransportOrder tempTO = new TransportOrder();
            // insert TO
            //int lastSeq = _transportOrderRepo.getLastSequenceByShipmentDate(DateTime.Parse(sheet[rowNum, 6].Text));
            tempTO.IDTransportPickingListLog = idPL;
            tempTO.ShipmentDate = DateTime.Parse(sheet[rowNum, 6].Text);
            tempTO.OrderType = "Finished Good";
            //if (string.IsNullOrEmpty(sheet[rowNum, 11].Text))
            if(currentSeq == 0)
            {
                //var STONo = _transportOrderRepo.getLastSTOPreOrder(DateTime.Parse(sheet[rowNum, 6].Text));
                tempTO.STONo = STONo;
                tempTO.ReceiverIDLocation = sheet[rowNum, 7].Text;
                var actualShipTo = sheet[rowNum, 8].Text;
                //tempTO.ActualReceiverIDLocation = setActualSenderReceiver(sheet[rowNum, 7].Text, actualShipTo.Substring(0, actualShipTo.Length - 9));
                tempTO.ActualReceiverIDLocation = setActualSenderReceiver(sheet[rowNum, 7].Text, sheet[rowNum, 8].Text);
                tempTO.SenderIDLocation = sheet[rowNum, 9].Text;
                tempTO.ActualSenderIDLocation = setActualSenderReceiver(sheet[rowNum, 9].Text, sheet[rowNum, 10].Text);
                tempTO.DefaultSeqNo = null;
                tempTO.VehicleType = "CBU";
                //tempTO.Remarks = sheet[rowNum, 13].Text;            
                //TransportOrder lastTO = _transportOrderRepo.GetTransportOrderBySTONo2();

            }
            else
            {
                if(String.IsNullOrEmpty(sheet[rowNum, 1].Text))
                    tempTO.STONo = null;
                else
                    tempTO.STONo = sheet[rowNum, 1].Text;
                tempTO.ReceiverIDLocation = sheet[rowNum, 7].Text;
                tempTO.ActualReceiverIDLocation = setActualSenderReceiver(sheet[rowNum, 7].Text, sheet[rowNum, 8].Text);
                tempTO.SenderIDLocation = sheet[rowNum, 9].Text;
                tempTO.ActualSenderIDLocation = setActualSenderReceiver(sheet[rowNum, 9].Text, sheet[rowNum, 10].Text);
                tempTO.DefaultSeqNo = currentSeq.ToString();
                tempTO.VehicleType = string.IsNullOrEmpty(sheet[rowNum, 12].Text)? "CBU": sheet[rowNum, 12].Text;
                //tempTO.Remarks = sheet[rowNum, 13].Text;
            }
            tempTO.Remarks = sheet[rowNum, 13].Text;
            if (sheet[rowNum, 12].Text.Contains("Cont") && (!sheet[rowNum, 13].Text.ToLower().Contains("train")))
                tempTO.Remarks = tempTO.Remarks + " Ship";
            if (sheet[rowNum, 10].Text.ToUpper().Contains("TPO") && (!sheet[rowNum, 13].Text.ToLower().Contains("synergy")))
                tempTO.Remarks = tempTO.Remarks + " Regular";

            tempTO.FlagEmail = 0;
            tempTO.OrderStatus = "Submit";
            tempTO.IsActive = true;
            tempTO.CreatedBy = userid;
            tempTO.UpdatedBy = userid;
            tempTO.CreatedDate = DateTime.Now;
            tempTO.UpdatedDate = DateTime.Now;
            if (filename.ToLower().Contains("east"))
                tempTO.ZoneBased = "East";
            else if (filename.ToLower().Contains("west"))
                tempTO.ZoneBased = "West";
            else
                tempTO.ZoneBased = _masterLocationRepo.GetParentLocationEastWest(sheet[rowNum, 9].Text);
            return tempTO;
        }
        private TransportOrderDetail setEntityTODetail(ExcelRange sheet, int rowNum, int idTO, string userid)
        {
            TransportOrderDetail tempTODetail = new TransportOrderDetail();
            tempTODetail.IDTransportOrder = idTO;
            tempTODetail.Code = sheet[rowNum, 3].Text;
            tempTODetail.Qty = Decimal.Parse(sheet[rowNum, 4].Text);
            tempTODetail.UoM = sheet[rowNum, 5].Text;
            tempTODetail.Supplier = sheet[rowNum, 2].Text;
            tempTODetail.Description = sheet[rowNum, 14].Text;
            tempTODetail.MaterialType = "Cigarette";
            tempTODetail.IsActive = true;
            tempTODetail.CreatedBy = userid;
            tempTODetail.UpdatedBy = userid;
            tempTODetail.CreatedDate = DateTime.Now;
            tempTODetail.UpdatedDate = DateTime.Now;
            return tempTODetail;
        }

        #region LOG
        public List<TransportPickingListLogDTO> GetAllBatchDate(DateTime tfrom, DateTime tto)
        {
            return Mapper.Map<List<TransportPickingListLogDTO>>(_transportPickingListLogRepo.GetAllBatchDate(tfrom, tto));
        }

        public List<TransportOrderDTO> GetBatchData(DateTime tbtc, string usr, DateTime tfrom, DateTime tto)
        {
            var context = new TOMContextDB();

            var dbResult = (
                            from PL in context.TransportPickingListLogs
                            join TO in context.TransportOrders
                            on PL.IDTransportPickingListLog equals TO.IDTransportPickingListLog
                            join LOC1 in context.MasterLocations
                            on TO.ActualSenderIDLocation equals LOC1.IDLocation
                            join LOC2 in context.MasterLocations
                            on TO.ReceiverIDLocation equals LOC2.IDLocation
                            where PL.CreatedDate.Day == tbtc.Day && PL.CreatedDate.Month == tbtc.Month && PL.CreatedDate.Year == tbtc.Year && PL.CreatedDate.Hour == tbtc.Hour && PL.CreatedDate.Minute == tbtc.Minute && PL.CreatedBy == usr
                            && TO.ShipmentDate >= tfrom && TO.ShipmentDate <= tto && PL.IsActive == true
                            orderby PL.IDTransportPickingListLog, TO.IDTransportOrder
                            select new TransportOrderDTO
                            {
                                IDTransportOrder = TO.IDTransportOrder,
                                IDTransportPickingListLog = PL.IDTransportPickingListLog,
                                STONo = TO.STONo,
                                SeqNo = TO.SeqNo,
                                DefaultSeqNo = TO.DefaultSeqNo,
                                ShipmentDate = TO.ShipmentDate,
                                SenderIDLocation = LOC1.LocationName,
                                ReceiverIDLocation = LOC2.LocationName,
                                ZoneBased = TO.ZoneBased,
                                IsActive = (bool)PL.IsActive
                            }
                            )
                            .ToList();

            var checkSTO = dbResult.Where(y => y.STONo != null && y.STONo != "" && !y.STONo.Contains("PO") && !y.STONo.Contains("ON")).ToList();
            if (checkSTO.Count > 0)
            {
                foreach (var ston in checkSTO)
                {
                    dbResult.RemoveAll(x => x.IDTransportPickingListLog == ston.IDTransportPickingListLog);
                }
            }

            return dbResult;
        }

        public string DeleteBatchData(DateTime tbtc, string usr, DateTime tfrom, DateTime tto, string IDUser)
        {
            var DataBatch = _generalRepo.Get(x => x.CreatedBy == usr && x.ShipmentDate >= tfrom && x.ShipmentDate <= tto && x.IsActive && x.CreatedDate.Year == tbtc.Year && x.CreatedDate.Month == tbtc.Month && x.CreatedDate.Day == tbtc.Day && x.CreatedDate.Hour == tbtc.Hour && x.CreatedDate.Minute == tbtc.Minute);

            var ctx = new TOMContextDB();

            var message = "";

            if (DataBatch.Count() > 0)
            {
                var PLIDList = DataBatch.Select(f => f.IDTransportPickingListLog).ToList();
                var PickingList = ctx.TransportPickingListLogs
                                  .Where(f => PLIDList.Contains(f.IDTransportPickingListLog));
                var UpdatedDate = DateTime.Now;

                //Inactive PL
                foreach (var PL in PickingList)
                {
                    PL.UpdatedBy = IDUser;
                    PL.UpdatedDate = UpdatedDate;
                    PL.IsActive = false;
                }

                //ctx.SaveChanges();
                var OrderList = ctx.TransportOrders
                                .Where(f => PLIDList.Contains(f.IDTransportPickingListLog ?? 0));

                var CheckOrderList = OrderList
                                .Where(f => 
                                       f.STONo != null &&
                                       f.STONo != "" &&
                                       !f.STONo.Contains("PO") &&
                                       !f.STONo.Contains("ON"))
                                 .ToList();

                if (CheckOrderList.Count() == 0)
                {
                    var ExecIDList = OrderList
                                    .Where(f => f.IDTransportExecution != null)
                                    .Select(f => f.IDTransportExecution);

                    //Inactive Order
                    foreach (var Order in OrderList)
                    {
                        Order.UpdatedBy = IDUser;
                        Order.UpdatedDate = UpdatedDate;
                        Order.IsActive = false;
                        Order.TransportOrderDetails
                            .ToList()
                            .ForEach(f =>
                            {
                                f.UpdatedBy = IDUser;
                                f.UpdatedDate = UpdatedDate;
                                f.IsActive = false;
                            });
                    }

                    //ctx.SaveChanges();

                    var ExecList = ctx.TransportExecutions
                                   .Where(f => 
                                    ExecIDList.Contains(f.IDTransportExecution) &&
                                    f.TransportNo.Contains("E")
                                    );

                    //Inactive Exec
                    foreach (var Exec in ExecList)
                    {
                        Exec.UpdatedBy = IDUser;
                        Exec.UpdatedDate = UpdatedDate;
                        Exec.IsActive = false;
                    }

                    ctx.SaveChanges();
                }
                else
                {
                   var ShipmentDates = CheckOrderList.Select(f => f.ShipmentDate.ToString("dd MMM yyy")).Distinct().ToArray();
                    message = "Shipment Date : " + String.Join(", ", ShipmentDates) + " have order number already synced";
                }

                return message;
            }
            else
            {
                return "Failed to delete data";
            }

            /*
            if (DataBatch.Count() > 0)
            {
                var listIDTE = new List<int>();
                var listTE = _transportExecutionRepo.GetAll().Where(x => x.IsActive);
                foreach (var PL in DataBatch)
                {
                    var TOresult = _transportOrderRepo.GetAll().Where(x => x.IDTransportPickingListLog == PL.IDTransportPickingListLog);
                    var tempTO = TOresult.Where(y => y.STONo != null && y.STONo != "" && !y.STONo.Contains("PO") && !y.STONo.Contains("ON")).ToList();
                    if (tempTO.Count == 0)
                    {
                        var PLL = new TransportPickingListLog();
                        PLL = PL;
                        PLL.IsActive = false;
                        PLL.UpdatedBy = IDUser;
                        PLL.UpdatedDate = System.DateTime.Now;
                        _generalRepo.Update(PLL);
                        _generalRepo.Save();

                        foreach (var TO in TOresult)
                        {
                            TO.IsActive = false;
                            TO.UpdatedBy = IDUser;
                            TO.UpdatedDate = System.DateTime.Now;
                            _transportOrderRepo.Update(TO);
                            _transportOrderRepo.Save();

                            //if (!string.IsNullOrEmpty(TO.STONo) && !string.IsNullOrEmpty(TO.IDTransportExecution.ToString()))
                            if (!string.IsNullOrEmpty(TO.IDTransportExecution.ToString()))
                            {
                                //if (TO.STONo.Contains("ON"))
                                //{
                                    var transportExe = new TransportExecution();
                                    //listIDTE.Add((int) TO.IDTransportExecution);

                                    transportExe = listTE.Where(x => x.IDTransportExecution == TO.IDTransportExecution && x.IsActive).FirstOrDefault();
                                    if (transportExe != null && transportExe.TransportNo.Contains("E"))
                                    {
                                        transportExe.IsActive = false;
                                        transportExe.UpdatedBy = IDUser;
                                        transportExe.UpdatedDate = System.DateTime.Now;
                                        _transportExecutionRepo.Update(transportExe);
                                        _transportExecutionRepo.Save();
                                    }
                                //}
                            }

                            //_transportOrderDetailRepo.setInActiveByTO(TO.IDTransportOrder);
                        }
                    }
                }               
                return true;
            }
            else
            {
                return false;
            }
            */

            
        }
        #endregion LOG

        #region EXPORT
        public List<TransportOrderDTO> GetRawDataExport()
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TO in context.TransportOrders
                        join TO_D in context.TransportOrderDetails on TO.IDTransportOrder equals TO_D.IDTransportOrder
                        join LOC1 in context.MasterLocations on TO.ActualSenderIDLocation equals LOC1.IDLocation
                        join LOC2 in context.MasterLocations on TO.ActualReceiverIDLocation equals LOC2.IDLocation
                        join TE in context.TransportExecutions on TO.IDTransportExecution equals TE.IDTransportExecution
                        where TO.IsActive == true
                        orderby TO.ShipmentDate, TO.IDTransportOrder, TO_D.IDTransportOrderDetail
                        select new TransportOrderDTO
                        {
                            STONo = TO.STONo,
#warning [Migration] Supplier is missing
                            Supplier = TO_D.Supplier,
                            Code = TO_D.Code,
                            Qty = TO_D.Qty,
                            UoM = TO_D.UoM,
                            ShipmentDate = TO.ShipmentDate,
                            ActualSenderIDLocation = TO.ActualSenderIDLocation,
                            LocNameSender = LOC1.LocationName,
                            ActualReceiverIDLocation = TO.ActualReceiverIDLocation,
                            LocNameReceiver = LOC2.LocationName,
                            SeqNo = TO.SeqNo,
                            VehicleType = TO.VehicleType,
                            Remarks = TO.Remarks,
                            Description = TO_D.Description
                        }
                        )
                        .ToList();

            return dbResult;
        }
        public List<TransportOrderDTO> GetListFormExport()
        {
            var context = new TOMContextDB();
            var dbResult = (
                        from TO in context.TransportOrders
                        join TO_D in context.TransportOrderDetails on TO.IDTransportOrder equals TO_D.IDTransportOrder
                        join LOC1 in context.MasterLocations on TO.ActualSenderIDLocation equals LOC1.IDLocation
                        join LOC2 in context.MasterLocations on TO.ActualReceiverIDLocation equals LOC2.IDLocation
                        join TE in context.TransportExecutions on TO.IDTransportExecution equals TE.IDTransportExecution
                        join TOC in context.TransportOrderChangeLogs on TO.IDTransportOrder equals TOC.IDTransportOrder into tbl_LEFT1
                        from TOC in tbl_LEFT1.DefaultIfEmpty()
                        join TOC_D in context.TransportOrderDetailChangeLogs on TO_D.IDTransportOrderDetail equals TOC_D.IDTransportOrderDetail into tbl_LEFT2
                        from TOC_D in tbl_LEFT2.DefaultIfEmpty()
                        where TO.IsActive == true && TO.IDTransportPickingListLog != null
                        orderby TO.IDTransportOrder, TO_D.IDTransportOrderDetail
                        select new TransportOrderDTO    
                        {
                            STONo = TO.STONo,
                            //warning Please fill below field
                            Supplier = null,
                            Code = TO_D.Code,
                            Qty = TO_D.Qty,
                            UoM = TO_D.UoM,
                            ShipmentDate = TO.ShipmentDate,
                            ActualSenderIDLocation = TO.ActualSenderIDLocation,
                            LocNameSender = LOC1.LocationName,
                            ActualReceiverIDLocation = TO.ActualReceiverIDLocation,
                            LocNameReceiver = LOC2.LocationName,
                            SeqNo = TO.SeqNo,
                            VehicleType = TO.VehicleType,
                            Remarks = TO.Remarks,
                            Description = TO_D.Description,
                            ModifChangeLog = TOC != null ? TOC.ModifiedField : "",
                            ModifChangeLogD = TOC_D != null ? TOC_D.ModifiedField : ""
                        }
                )
                .ToList();
            return dbResult;
        }
        #endregion EXPORT

        public List<TransportOrderDTO> GetTransportOrderByShipmentDate(DateTime fromDate, DateTime toDate, string uploadedBy)
        {
            var context = new TOMContextDB();
            var dbResult = (
                            from PL in context.TransportPickingListLogs
                            join TO in context.TransportOrders
                            on PL.IDTransportPickingListLog equals TO.IDTransportPickingListLog
                            join TOD in context.TransportOrderDetails
                            on TO.IDTransportOrder equals TOD.IDTransportOrder
                            join LOC1 in context.MasterLocations
                            on TO.SenderIDLocation equals LOC1.IDLocation
                            join LOC2 in context.MasterLocations
                            on TO.ReceiverIDLocation equals LOC2.IDLocation
                            where PL.ShipmentDate >= fromDate && PL.ShipmentDate <= toDate && PL.IsActive == true && TO.Remarks != "Pre Order" && !(TO.Remarks.ToLower().Contains("moving"))
                            orderby PL.ShipmentDate
                            select new TransportOrderDTO
                            {
                                IDTransportOrder = TO.IDTransportOrder,
                                SenderIDLocation = TO.SenderIDLocation,
                                DocDate = PL.CreatedDate,
                                Material = TOD.Code,
                                Qty = TOD.Qty,
                                ShipmentDate = PL.ShipmentDate,
                                ReceiverIDLocation = TO.ReceiverIDLocation,
                                Supplier = TOD.Supplier,
                                UoM = TOD.UoM,
                                CreatedBy = TO.CreatedBy,
                                STONo = TO.STONo
                            }
                            )
                            .ToList();
            dbResult = dbResult.Where(x => x.STONo == null || x.STONo == "").ToList();
            if (!string.IsNullOrEmpty(uploadedBy))
            {
                dbResult = dbResult.Where(x => x.CreatedBy.Trim().ToUpper() == uploadedBy.Trim().ToUpper()).ToList();
            }
            return dbResult;
        }

        public DataTable ListToDataTable(List<TransportOrderDTO> dataTO, DateTime DocDate, string userid)
        {
            List<string> listSupplierPmi = _masterListRepo.GetMasterListFieldNameValue("ExcludeSupplier");
            DataTable boundTable = new DataTable();
            string[] columns = { "Order Type", "Vendor", "Doc.Date", "Purchasing Org.", "Purch Group", "Company Code", "Header Text", "Funct", "Number", "A", "I", "Material", "PO Quantity", "OuN", "C", "Deliv.Date", "Plnt", "Stor.loc", "Batch", "Shipping Pt.", "Tracking No", "Requisitioner", "Returns item", "Free", "Texts", "Purch.req.", "Requisn", "Outline agreement", "Agreement item", "RFQ", "Item", "Purchasing Doc.", "Item2", "Higher-level", "Subitem Category" };
            string[] intFields = { "PO Quantity" };
            string userID = userid.Substring(4).ToUpper();
            for (int i = 0; i < columns.Length; i++)
            {
                if (intFields.Contains(columns[i]))
                {
                    boundTable.Columns.Add(columns[i], typeof(int));
                }
                else
                {
                    boundTable.Columns.Add(columns[i], typeof(string));
                }
            }
            var prevTO = 0;
            var j = 1;
            var sequence = 0;
            foreach (TransportOrderDTO record in dataTO)
            {
                var dict = new Dictionary<string, string>();
                var trackingno = "";
                dict["Order Type"] = (prevTO == record.IDTransportOrder) ? "" : "UB";
                dict["Vendor"] = (prevTO == record.IDTransportOrder) ? "" : record.SenderIDLocation;
                dict["Doc.Date"] = (prevTO == record.IDTransportOrder) ? "" : DocDate.ToString("dd.MM.yyyy");
                dict["Purchasing Org."] = (prevTO == record.IDTransportOrder) ? "" : "ID01";
                dict["Purch Group"] = (prevTO == record.IDTransportOrder) ? "" : "633";
                dict["Company Code"] = (prevTO == record.IDTransportOrder) ? "" : "3066";
                dict["Header Text"] = (prevTO == record.IDTransportOrder) ? "" : j.ToString();
                /*if (String.IsNullOrEmpty(dict["Header Text"]))
                {
                    trackingno = record.ShipmentDate.ToString("dd.MM.yyyy") + "-" + (j - 1).ToString();
                }
                else
                {
                    trackingno = record.ShipmentDate.ToString("dd.MM.yyyy") + "-" + j.ToString();
                }*/
                dict["Funct"] = "";
                dict["Number"] = "";
                dict["A"] = "";
                dict["I"] = "";
                dict["Material"] = record.Material;
                dict["PO Quantity"] = Convert.ToInt32(record.Qty).ToString();
                MasterMapping oun = _masterMappingRepo.CheckAvailabilityMasterMappingByMapFrom(record.UoM);
                dict["OuN"] = (oun == null) ? "" : oun.MapTo;
                dict["C"] = "";
                dict["Deliv.Date"] = record.ShipmentDate.ToString("dd.MM.yyyy");
                dict["Plnt"] = record.ReceiverIDLocation;
                dict["Stor.loc"] = "1000";
                dict["Batch"] = "";
                dict["Shipping Pt."] = "";
                dict["Tracking No"] = record.ShipmentDate.ToString("dd.MM.yyyy");
                /*if (listSupplierPmi.Contains(record.Supplier))
                {
                    dict["Tracking No"] = trackingno + "a";                                        
                }else
                {            
                    dict["Tracking No"] = trackingno;
                }*/
                dict["Requisitioner"] = userID;
                dict["Returns item"] = "";
                dict["Free"] = "";
                dict["Texts"] = "";
                dict["Purch.req."] = "";
                dict["Requisn"] = "";
                dict["Outline agreement"] = "";
                dict["Agreement item"] = "";
                dict["RFQ"] = "";
                dict["Item"] = "";
                dict["Purchasing Doc."] = "";
                dict["Item2"] = "";
                dict["Higher-level"] = "";
                dict["Subitem Category"] = "";
                dynamic dr = boundTable.NewRow();
                foreach (string Field in columns)
                {
                    if (intFields.Contains(Field))
                    {
                        dr[Field] = Convert.ToInt32(dict[Field]);
                    }
                    else
                    {
                        dr[Field] = dict[Field];
                    }
                }
                if (prevTO != record.IDTransportOrder)
                {
                    j++;
                }

                prevTO = record.IDTransportOrder;
                boundTable.Rows.Add(dr);
            }
            return boundTable;
        }

        public byte[] ExportToExcel(DataTable dataTable)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", ""));
                int startRowFrom = 1;
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    /*ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                    if (maxLength < 150)
                    {*/
                    workSheet.Column(columnIndex).AutoFit();
                    //}
                    columnIndex++;
                }

                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                }
                result = package.GetAsByteArray();
            }
            return result;
        }

        public List<TransportOrderDTO> GetExportXls(TransportOrderInput criteria)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TO in context.TransportOrders
                        join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder into LJOIN_1
                        from TOD in LJOIN_1.DefaultIfEmpty()
                        join MV in context.MasterVendors on TOD.Supplier equals MV.VendorName into RJOIN_1
                        from MV in RJOIN_1.DefaultIfEmpty()
                        join MLTOS in context.MasterLocations on TO.SenderIDLocation equals MLTOS.IDLocation into RJOIN_2
                        from MLTOS in RJOIN_2.DefaultIfEmpty()
                        join MLTOR in context.MasterLocations on TO.ReceiverIDLocation equals MLTOR.IDLocation into RJOIN_3
                        from MLTOR in RJOIN_3.DefaultIfEmpty()
                        where TO.IsActive == true && TO.IsActive == true && TO.IDTransportPickingListLog != null
                        orderby TO.IDTransportOrder, TOD.IDTransportOrderDetail
                        select new TransportOrderDTO
                        {
                            STONo = TO.STONo,
                            ZoneBased = TO.ZoneBased,
                            OrderType = TO.OrderType,
                            SenderIDLocation = MLTOS.IDLocation,
                            SenderLocationName = MLTOS.LocationName,
                            ReceiverIDLocation = MLTOR.IDLocation,
                            ReceiverLocationName = MLTOR.LocationName,
                            ShipmentDate = TO.ShipmentDate,
                            VehicleType = TO.VehicleType,
                            Qty = TOD.Qty,
                            UoM = TOD.UoM,
                            DefaultSeqNo = TO.DefaultSeqNo,
                            CreatedBy = TO.CreatedBy,
                            Supplier = MV.VendorName,
                            UpdatedBy = TO.UpdatedBy,
                            UpdatedDate = TO.UpdatedDate
                        }
                        );

            if (criteria.weekFilter != 0 && criteria.yearFilter != 0)
            {
                var dateFilter = _masterGenWeekRepo.GetFromToDateByWeekYear(criteria.weekFilter, criteria.yearFilter);
                criteria.dateFromFilter = dateFilter.StartDate;
                criteria.dateToFilter = dateFilter.EndDate;
            }

            if (criteria.dateFromFilter != null) { dbResult = dbResult.Where(q => q.ShipmentDate >= criteria.dateFromFilter); }
            if (criteria.dateToFilter != null) { dbResult = dbResult.Where(q => q.ShipmentDate <= criteria.dateToFilter); }
            if (!string.IsNullOrEmpty(criteria.zoneFilter)) { dbResult = dbResult.Where(q => q.ZoneBased == criteria.zoneFilter); }
            if (criteria.stoNoListFilter.Count() > 1) { dbResult = dbResult.Where(q => criteria.stoNoListFilter.Contains(q.STONo)); }
            if (criteria.senderIdLocListFilter.Count() > 1) { dbResult = dbResult.Where(q => criteria.senderIdLocListFilter.Contains(q.SenderIDLocation)); }
            if (criteria.receiverIdLocListFilter.Count() > 1) { dbResult = dbResult.Where(q => criteria.receiverIdLocListFilter.Contains(q.ReceiverIDLocation)); }
            if (!string.IsNullOrEmpty(criteria.uploadedByFilter)) { dbResult = dbResult.Where(q => criteria.uploadedByFilter.Contains(q.CreatedBy)); }     
            return dbResult.ToList();
        }

        public string SendEmail(string userid)
        {
            return _transportPickingListLogRepo.SendEmail(userid);
        }

        public List<TransportPickingListExportDTO> GetRawTOD(TransportOrderInput input)
        {
            MasterGenWeek dateFilter = new MasterGenWeek();
            if (input.weekFilter != 0 && input.yearFilter != 0)
            {
                dateFilter = _masterGenWeekRepo.GetFromToDateByWeekYear(input.weekFilter, input.yearFilter);
                input.dateFromFilter = dateFilter.StartDate;
                input.dateToFilter = dateFilter.EndDate;
            }
            List<TransportPickingListExportDTO> tempTOD = _transportOrderRepo.GetRawDataTODPL(input);
            return tempTOD;
        }

        #region new code NIcco

        public List<TransportPickingListViewDTO> GetRawTPL(TransportOrderInput input)
        {
            MasterGenWeek dateFilter = new MasterGenWeek();
            if (input.weekFilter != 0 && input.yearFilter != 0)
            {
                dateFilter = _masterGenWeekRepo.GetFromToDateByWeekYear(input.weekFilter, input.yearFilter);
                input.dateFromFilter = dateFilter.StartDate;
                input.dateToFilter = dateFilter.EndDate;
            }

            //dateFrom
            var dateFrom = input.dateFromFilter.ToString();
            DateTime dateFromFilter = DateTime.Parse(dateFrom);

            //dateTo
            var dateTo = input.dateToFilter.ToString();
            DateTime dateToFilter = DateTime.Parse(dateTo);

            using (var context = new TOMContextDB())
            {
                var tplv = (from x in context.TransportPickingListViews
                            where x.ShipmentDate >= input.dateFromFilter && x.ShipmentDate <= input.dateToFilter
                            select new TransportPickingListViewDTO
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
                                IDTransportOrderDetail = x.IDTransportOrderDetail,
                                ZoneBased = x.ZoneBased
                            }).ToList();


                if (input.stoNoListFilter != null && !String.IsNullOrEmpty(input.stoNoListFilter[0]))
                {
                    var stoNoList = input.stoNoListFilter[0].Split(',');
                    tplv = tplv.Where(x => stoNoList.Contains(x.STONo)).ToList();
                }
                if (input.zoneFilter != null)
                {
                    tplv = tplv.Where(x => input.zoneFilter.Contains(x.ZoneBased)).ToList();
                }
                if (input.dateFromFilter != null && input.dateToFilter != null)
                {
                    tplv = tplv.Where(x => x.ShipmentDate >= input.dateFromFilter && x.ShipmentDate <= input.dateToFilter).ToList();
                }
                if (input.senderIdLocListFilter != null && !String.IsNullOrEmpty(input.senderIdLocListFilter[0]))
                {
                    tplv = tplv.Where(x => input.senderIdLocListFilter.Contains(x.SenderIDLocation)).ToList();
                }
                if (input.receiverIdLocListFilter != null && !String.IsNullOrEmpty(input.receiverIdLocListFilter[0]))
                {
                    tplv = tplv.Where(x => input.receiverIdLocListFilter.Contains(x.ReceiverIDLocation)).ToList();
                }
                if (input.vehicleTypeFilter != null)
                {
                    tplv = tplv.Where(x => input.vehicleTypeFilter.Contains(x.VehicleType)).ToList();
                }
                if (!String.IsNullOrEmpty(input.uploadedByFilter))
                {
                    tplv = tplv.Where(x => input.uploadedByFilter.Contains(x.CreatedByID)).ToList();
                }
          
                return tplv;
            }
        }

        #endregion

        public List<MasterListDTO> getMasterList()
        {
            List<string> temp = new List<string>(new[] { "Zone" });
            return Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByListFieldName(temp));
        }
        public List<MasterListDTO> getMasterList(string id)
        {
            List<string> temp = new List<string>(new[] { id });
            return Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByListFieldName(temp));
        }

        public int UpdateIsActive()
        {
            List<int> idList = new List<int>();
            for (var i = 27528; i<= 27630; i++)
            {
                idList.Add(i);
            }
            using (var db=new TOMContextDB())
            {
                var to1= db.TransportOrders.Where(f=>idList.Contains(f.IDTransportOrder)).ToList();
                to1.ForEach(a =>
                {
                    a.STONo = null; 
                    a.IsActive = false;
                });
                var tod = db.TransportOrderDetails.Where(x => idList.Contains(x.IDTransportOrder)).ToList();
                tod.ForEach(y => y.IsActive = false);
                db.SaveChanges();
            }
            return 0;
        }
    }
}
