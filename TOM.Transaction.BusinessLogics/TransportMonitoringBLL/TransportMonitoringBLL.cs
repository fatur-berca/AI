using AutoMapper;
using DFIS.Contracts;
using DFIS.Universal.Domain.DTOs;
using DFIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Repositories;
using TOM.Transport.Repositories.TransportVesselMonitoringRepo;

namespace TOM.Transport.BusinessLogics.TransportMonitoringBLL
{
    public class TransportMonitoringBLL : ITransportMonitoringBLL
    {
        private readonly IGenericRepository<TransportVesselMonitoring> _generalRepo;
        private readonly IGenericRepository<MasterList> _mstListOrderSts;
        private readonly IGenericRepository<MasterLocation> _mstLocationRepo;
        private readonly IGenericRepository<MasterVendor> _mstVendorRepo;
        private readonly IGenericRepository<TransportExecution> _transExecutionRepo;
        private readonly IGenericRepository<TransportOrder> _transOrderRepo;
        private readonly IGenericRepository<TransportPositionDetail> _transPositionRepo;
        private readonly IGenericRepository<TransportMonitoringTruckTrainList> _transportMonitoringTruckTrainList;
        private readonly IGenericRepository<TransportDriverManagement> _transDriverManagementRepo;
        private readonly IGenericRepository<MasterConfiguration> _masterConfigurationRepo;
        private readonly ITransportVesselMonitoringRepo _transportVesselMonitoringRepo;
        private readonly ITransportOrderRepo _transportOrderRepo;
        private readonly IMasterListRepo _masterListRepo;

        public TransportMonitoringBLL(IGenericRepository<TransportVesselMonitoring> generalRepo, IGenericRepository<MasterConfiguration> masterConfigurationRepo, ITransportOrderRepo transportOrderRepo, ITransportVesselMonitoringRepo transportVesselMonitoringRepo, IGenericRepository<TransportDriverManagement> transDriverManagementRepo, IGenericRepository<MasterList> mstListOrderSts, IGenericRepository<MasterLocation> mstLocationRepo, IGenericRepository<MasterVendor> mstVendorRepo, IGenericRepository<TransportExecution> transExecutionRepo, IGenericRepository<TransportOrder> transOrderRepo, IGenericRepository<TransportPositionDetail> transPositionRepo, IGenericRepository<TransportMonitoringTruckTrainList> transportMonitoringTruckTrainList, IMasterListRepo masterListRepo)
        {
            _generalRepo = generalRepo;
            _mstListOrderSts = mstListOrderSts;
            _mstLocationRepo = mstLocationRepo;
            _mstVendorRepo = mstVendorRepo;
            _transExecutionRepo = transExecutionRepo;
            _transOrderRepo = transOrderRepo;
            _transPositionRepo = transPositionRepo;
            _transportMonitoringTruckTrainList = transportMonitoringTruckTrainList;
            _transDriverManagementRepo = transDriverManagementRepo;
            _transportVesselMonitoringRepo = transportVesselMonitoringRepo;
            _transportOrderRepo = transportOrderRepo;
            _masterConfigurationRepo = masterConfigurationRepo;
            _masterListRepo = masterListRepo;
        }

        public List<TransportMonitoringDTO> GetDatas(TransportMonitoringInput input)
        {
            var queryFilter = PredicateHelper.True<TransportVesselMonitoring>();

            if (!String.IsNullOrEmpty(input.TransportationNo))
            {
                string[] inArray = input.TransportationNo.Split(',');
                queryFilter = queryFilter.And(k => inArray.Contains(k.TransportExecution.TransportNo));
            }

            if (!String.IsNullOrEmpty(input.StartLocation))
            {
                queryFilter = queryFilter.And(k => k.TransportExecution.StartLocation == input.StartLocation);
            }

            if (!String.IsNullOrEmpty(input.TransportationStatus))
            {
                queryFilter = queryFilter.And(k => k.TransportExecution.TransportStatus == input.TransportationStatus);
            }

            if (input.DateOfStuffingFrom != null)
            {
                DateTime DateFrom = Convert.ToDateTime(input.DateOfStuffingFrom + " 00:00");
                queryFilter = queryFilter.And(m => m.TransportExecution.TransportDate >= DateFrom);
            }
            if (input.DateOfStuffingTo != null)
            {
                DateTime DateTo = Convert.ToDateTime(input.DateOfStuffingTo + " 00:00");
                queryFilter = queryFilter.And(m => m.TransportExecution.TransportDate <= DateTo);
            }

            if (input.IsExportOrSearch)
            {
                if (input.IncludeInActive == "0")
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }
            else
            {
                queryFilter = queryFilter.And(k => k.IsActive);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportVesselMonitoring>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<TransportMonitoringDTO>>(dbResult);
        }

        public IEnumerable<MasterListDTO> getMasterListForFilter(List<string> fieldName = null)
        {
            if (fieldName == null)
            {
                fieldName = new List<string>(new[] { "OrderCategory", "SIStatus", "OrderType", "MaterialType", "ExecutionType", "VehicleType", "TransportationMode", "TransportationCategory", "SIType", "ViaRoute", "VesselName", "TransportationStatus" });
            }
            var getData = _masterListRepo.GetMasterListByListFieldName(fieldName);
            var result = Mapper.Map<IEnumerable<MasterListDTO>>(getData);

            return result;
        }

        #region SUB TRUCK TRAIN
        public List<TransportExecutionDTO> GetTransportList()
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(q => q.IsActive == true);
            var dbResult = _transExecutionRepo.Get(queryFilter).Select(f => new TransportExecutionDTO { TransportNo = f.TransportNo, TransportStatus = f.TransportStatus, TransportMode = f.TransportMode }).ToList();
            return dbResult;
        }

        public List<MasterVendorDTO> GetVendorName()
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(q => q.IsActive == true);
            queryFilter = queryFilter.And(q => q.ParentVendor == null);
            var dbResult = _mstVendorRepo.Get(queryFilter).Select(f => new MasterVendorDTO { IDVendor = f.IDVendor.ToString(), VendorName = f.VendorName }).ToList();
            return dbResult;
        }

        public List<MasterLocationDTO> GetLocationName()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(q => q.IsActive);
            queryFilter = queryFilter.And(q => q.IsAssigned.Value);
            var dbResult = _mstLocationRepo.Get(queryFilter).Select(f => new MasterLocationDTO { IDLocation = f.IDLocation, LocationName = f.LocationName }).ToList();
            return dbResult;
        }

        public List<TransportOrderDTO> GetSTONo()
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(q => q.IsActive == true);
            var dbResult = _transOrderRepo.Get(queryFilter).Select(f => new TransportOrderDTO { STONo = f.STONo }).ToList();
            return dbResult;
        }

        public List<MasterListDTO> GetListOrderSts()
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(q => q.FieldName == "TransportationStatus" && q.IsActive == true);
            var dbResult = _mstListOrderSts.Get(queryFilter).OrderBy(o => o.FieldValue).Select(f => new MasterListDTO { FieldValue = f.FieldValue }).ToList();
            return dbResult;
        }

        public void SetCurrentLocation(TransportExecutionDTO dto)
        {
            var context = new TOMContextDB();
            List<TransportPositionDetail> d = context.TransportPositionDetails.Where(c => c.IDTransportExecution == dto.IDTransportExecution).ToList();
            if (d.Count > 0)
            {
                dto.TransportPositionDetails = Mapper.Map<List<TransportPositionDetail>, List<TransportPositionDetailDTO>>(d);
                dto.CurrentLocation = d[d.Count-1].PositionName;
                dto.CurrentLocationUpdateTime = d[d.Count-1].PositionDate ?? DateTime.MinValue;
            }
        }

        public class TextInput
        {
            protected List<char> karakter = new List<char>();
            public virtual void Add(char c)
            {
                karakter.Add(c);
            }

            public string Getvalue()
            {
                return new string(karakter.ToArray());
            }
        }

        public class NumericInput : TextInput
        {
            public override void Add(char c)
            {
                int n;
                bool isNumeric = int.TryParse(c.ToString(), out n);
                if(isNumeric)
                    karakter.Add(c);
            }
        }

        public List<TransportExecutionDTO> GetListTruckTrain(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        from TO in LJOIN_1.DefaultIfEmpty()
                        join TV in context.TransportVesselMonitorings on TE.IDTransportExecution equals TV.IDTransportExecution into LJOIN_2
                        from TV in LJOIN_2.DefaultIfEmpty()
                        join ML in context.MasterLocations on TE.StartLocation equals ML.IDLocation into RJOIN_1
                        from ML in RJOIN_1.DefaultIfEmpty()
                        join ML2 in context.MasterLocations on TE.FinishLocation equals ML2.IDLocation into LJOIN_3
                        from ML2 in LJOIN_3.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_2
                        from MV in RJOIN_2.DefaultIfEmpty()
                        where TE.TransportMode == mode
                        select new
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            IDTransportOrder = (int?)TO.IDTransportOrder,
                            TransportNo = TE.TransportNo,
                            TransportStatus = TE.TransportStatus,
                            ActualVehicleType = TE.ActualVehicleType,
                            VehicleType = TO.VehicleType,
                            PoliceRegistrationNumber = TE.PoliceRegNo,
                            Driver1 = TE.IDDriver1,
                            Driver2 = TE.IDDriver2,
                            CoDriver = TE.IDCoDriver,
                            STONo = TO.STONo,
                            IDVendor = (int?)TE.IDVendor,
                            VendorName = MV.VendorName,
                            IDSender = TO.SenderIDLocation,
                            IDReceiver = TO.ReceiverIDLocation,
                            IDStartLocation = TE.StartLocation,
                            IDFinishLocation = TE.FinishLocation != null ? TE.FinishLocation : "",
                            StartLocation = ML.LocationName,
                            FinishLocation = TE.FinishLocation != null ? ML2.LocationName : "",
                            TransportDate = TE.TransportDate,
                            //IDTransportVesselMonitoring = TV.IDTransportVesselMonitoring,
                            ETD1 = TV.ETD1,
                            ETD2 = TV.ETD2,
                            ATD = TV.ATD,
                            EstReceived = TV.EstReceived,
                            Remarks = TV.Remarks,
                            IsActve = TE.IsActive,
                            GRBy = TO.GRBy,
                            GIBy = TO.GIBy,
                            TransportationMode = MV.TransportationMode
                        }
                        );

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fts = fltrts.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] fvn = fltrvn.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] ffl = fltrfl.Split(',').ToArray();

            if (fltrtn.Length > 1) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltrts.Length > 1) { dbResult = dbResult.Where(q => fts.Contains(q.TransportStatus)); }
            if (fltron.Length > 1) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltrvn.Length > 1) { dbResult = dbResult.Where(q => fvn.Contains(q.IDVendor.ToString())); }
            if (fltrse.Length > 1) { dbResult = dbResult.Where(q => fse.Contains(q.IDSender)); }
            if (fltrre.Length > 1) { dbResult = dbResult.Where(q => fre.Contains(q.IDReceiver)); }
            if (fltrsl.Length > 1) { dbResult = dbResult.Where(q => fsl.Contains(q.IDStartLocation)); }
            if (fltrfl.Length > 1) { dbResult = dbResult.Where(q => ffl.Contains(q.IDFinishLocation)); }
            if (fltrdf.Length > 1)
            {
                try
                {
                    DateTime fdf = Convert.ToDateTime(fltrdf);
                    dbResult = dbResult.Where(q => q.TransportDate >= fdf);
                }
                catch { }
            }
            if (fltrdt.Length > 1)
            {
                try
                {
                    DateTime fdt = Convert.ToDateTime(fltrdt);
                    dbResult = dbResult.Where(q => q.TransportDate <= fdt);
                }
                catch { }
            }

            var tempRes = dbResult.ToList();

            var res = tempRes
                .GroupBy(g => g.TransportNo)
                .Select(f => new TransportExecutionDTO
                {
                    IDTransportExecution = f.FirstOrDefault().IDTransportExecution,
                    IDTransportOrder = f.FirstOrDefault().IDTransportOrder,
                    TransportNo = f.FirstOrDefault().TransportNo,
                    TransportDate = f.FirstOrDefault().TransportDate,
                    ActualVehicleType = f.FirstOrDefault().ActualVehicleType,
                    IDStartLocation = f.FirstOrDefault().IDStartLocation,
                    StartLocation = f.FirstOrDefault().StartLocation,
                    FinishLocation = f.FirstOrDefault().FinishLocation,
                    IDFinishLocation = f.FirstOrDefault().IDFinishLocation,
                    VendorName = f.FirstOrDefault().VendorName,
                    ETD1 = f.FirstOrDefault().ETD1,
                    ETD2 = f.FirstOrDefault().ETD2,
                    ATD = f.FirstOrDefault().ATD,
                    EstReceived = f.FirstOrDefault().EstReceived,
                    Remarks = f.FirstOrDefault().Remarks,
                    TransportStatus = f.FirstOrDefault().TransportStatus,
                    IsActive = f.FirstOrDefault().IsActve,
                    VehicleType = f.FirstOrDefault().VehicleType,
                    PoliceRegistrationNumber = f.FirstOrDefault().PoliceRegistrationNumber,
                    Driver1 = f.FirstOrDefault().Driver1,
                    Driver2 = f.FirstOrDefault().Driver2,
                    CoDriver = f.FirstOrDefault().CoDriver,
                    GRBy = f.FirstOrDefault().GRBy,
                    GIBy = f.FirstOrDefault().GIBy,
                    TransportMode = f.FirstOrDefault().TransportationMode
                })
                .OrderByDescending(o => o.TransportDate)
                .OrderBy(o => o.IDTransportOrder)
                .OrderBy(o => o.IDStartLocation)
                .OrderBy(o => o.TransportNo)
                .ToList();

            foreach (var r in res)
            {
                SetCurrentLocation(r);
            }

            return res;
        }

        public dynamic GetMonitoringDataTableTruckTrain(TransportMonitoringFilterInput input, bool isSuperAdmin, DataTableModel model = null)
        {
            #region FILTERS
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(_ => _.IsActive || isSuperAdmin);
            //queryFilter = queryFilter.And(_ => _.TransportStatus == "Complete");
            //queryFilter = queryFilter.And(_ => !_.TransportNo.StartsWith("E-TN"));
            //input.transportStatus.Add("Complete");
            if (input.transportNo != null && input.transportNo.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                var transportNo = new List<string>();
                foreach (var s in input.transportNo)
                {
                    if (!string.IsNullOrEmpty(s))
                        transportNo.Add(s);
                }
                if (transportNo.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => transportNo.Contains(c.TransportNo));
                }
            }

            if (input.dateFrom.HasValue)
            {
                queryFilter = queryFilter.And(_ => _.TransportDate >= input.dateFrom.Value);
            }
            if (input.dateTo.HasValue)
            {
                queryFilter = queryFilter.And(c => c.TransportDate <= input.dateTo.Value);
            }
            if (!String.IsNullOrEmpty(input.mode))
            {
                queryFilter = queryFilter.And(_ => _.TransportMode.ToLower() == input.mode.ToLower());
            }
            if (input.startLocation != null && input.startLocation.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                var startLocation = new List<string>();
                foreach (var s in input.startLocation)
                {
                    if (!string.IsNullOrEmpty(s))
                        startLocation.Add(s);
                }
                if (startLocation.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => startLocation.Contains(c.StartLocation));
                }
            }
            if (input.finishLocation != null && input.finishLocation.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                var finishLocation = new List<string>();
                foreach (var s in input.finishLocation)
                {
                    if (!string.IsNullOrEmpty(s))
                        finishLocation.Add(s);
                }
                if (finishLocation.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => finishLocation.Contains(c.FinishLocation));
                }
            }
            if (input.transportStatus != null && input.transportStatus.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                var transportStatus = new List<string>();
                foreach (var s in input.transportStatus)
                {
                    if (!string.IsNullOrEmpty(s))
                        transportStatus.Add(s);
                }
                if (transportStatus.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => transportStatus.Contains(c.TransportStatus));
                }
            }
            if (input.orderNo != null && input.orderNo.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                var orderNo = new List<string>();
                foreach (var s in input.orderNo)
                {
                    if (!string.IsNullOrEmpty(s))
                        orderNo.Add(s);
                }
                if (orderNo.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => orderNo.Contains(c2.STONo)));
                }
            }
            if (input.sender != null && input.sender.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                var sender = new List<string>();
                foreach (var s in input.sender)
                {
                    if (!string.IsNullOrEmpty(s))
                        sender.Add(s);
                }
                if (sender.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => sender.Contains(c2.ActualSenderIDLocation)));
                }
                queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => sender.Contains(c2.ActualSenderIDLocation)));
            }
            if (input.receiver != null && input.receiver.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                var receiver = new List<string>();
                foreach (var s in input.receiver)
                {
                    if (!string.IsNullOrEmpty(s))
                        receiver.Add(s);
                }
                if (receiver.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => receiver.Contains(c2.ActualReceiverIDLocation)));
                }
                    
            }
            if (input.vendor != null && input.vendor.Any(i => !String.IsNullOrWhiteSpace(i)))
            {
                var vendor = new List<string>();
                foreach (var s in input.vendor)
                {
                    if (!string.IsNullOrEmpty(s))
                        vendor.Add(s);
                }
                if (vendor.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => vendor.Contains(_.IDVendor.ToString()));
                }
            }
            #endregion

            #region filter from DataTable
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.search.value))
                {
                    var _us = model.search.value;
                    queryFilter = queryFilter.
                        And(_ => 
                        _.TransportNo.Contains(_us) || _.TransportStatus.Contains(_us) || 
                        _.CreatedBy.Contains(_us) || _.MasterVendor.VendorName.Contains(_us) || 
                        (input.mode == "Train" ? _.TransportVesselMonitorings.FirstOrDefault().ContainerNo.Contains(_us) : true) ||
                        (input.mode == "Train" ? _.Via.Contains(_us) : true) ||
                        _.SIWeek.ToString().Contains(_us) || _.SIType.Contains(_us) ||
                        _.SIStatus.Contains(_us) || _.StartLocation.Contains(_us) || 
                        _.TransportDate.ToString().Contains(_us) || _.UpdatedBy.Contains(_us));
                }

                foreach (var fil in model.columns.Where(_ => !String.IsNullOrEmpty(_.search.value)))
                {
                    var _col = fil.data;
                    var _val = fil.search.value;
                    switch (_col)
                    {
                        case "TransportNo":
                            queryFilter = queryFilter.And(_ => _.TransportNo.Contains(_val));
                            break;
                        case "TransportStatus":
                            queryFilter = queryFilter.And(_ => _.TransportStatus.Contains(_val));
                            break;
                        case "VendorName":
                            queryFilter = queryFilter.And(_ => _.MasterVendor.VendorName.Contains(_val));
                            break;
                        case "ActualVehicleType":
                            queryFilter = queryFilter.And(_ => _.ActualVehicleType.Contains(_val));
                            break;
                        case "StartLocation":
                            var _getStr = _mstLocationRepo.Get(_ => _.LocationName.Contains(_val)).Select(_ => _.IDLocation);
                            queryFilter = queryFilter.And(_ => _getStr.Contains(_.StartLocation));
                            break;
                        case "FinishLocation":
                            var _getFns = _mstLocationRepo.Get(_ => _.LocationName.Contains(_val)).Select(_ => _.IDLocation);
                            queryFilter = queryFilter.And(_ => _getFns.Contains(_.FinishLocation));
                            break;
                        case "ContainerNo":
                            queryFilter = queryFilter.And(_ => _.TransportVesselMonitorings.FirstOrDefault().ContainerNo.Contains(_val));
                            break;
                        case "Via":
                            queryFilter = queryFilter.And(_ => _.Via.Contains(_val));
                            break;
                        case "PoliceRegistrationNumber":
                            queryFilter = queryFilter.And(_ => _.PoliceRegNo.Contains(_val));
                            break;
                        case "TransportDate":
                            queryFilter = queryFilter.And(_ => _.TransportDate.ToString().Contains(_val));
                            break;
                        case "Driver1":
                            var _getDrv1 = _transDriverManagementRepo.Get(_ => _.Name.Contains(_val)).Select(_ => _.ID).ToList();
                            queryFilter = queryFilter.And(_ => _getDrv1.Contains(_.IDDriver1));
                            break;
                        case "Driver2":
                            var _getDrv2 = _transDriverManagementRepo.Get(_ => _.Name.Contains(_val)).Select(_ => _.ID).ToList();
                            queryFilter = queryFilter.And(_ => _getDrv2.Contains(_.IDDriver2));
                            break;
                        case "CoDriver":
                            var _getcoDrv = _transDriverManagementRepo.Get(_ => _.Name.Contains(_val)).Select(_ => _.ID).ToList();
                            queryFilter = queryFilter.And(_ => _getcoDrv.Contains(_.IDCoDriver));
                            break;
                        case "CurrentLocation":
                            queryFilter = queryFilter.And(_ => _.TransportPositionDetails.LastOrDefault().PositionName.Contains(_val));
                            break;
                        case "CurrentLocationUpdateTime":
                            queryFilter = queryFilter.And(_ => _.TransportPositionDetails.LastOrDefault().PositionDate.ToString().Contains(_val));
                            break;
                    }
                }
            }
            #endregion

            var getTM = Enumerable.Empty<TransportExecution>();
            if (model == null)
            {
                getTM = _transExecutionRepo.Get(queryFilter);
            }
            else
            {
                getTM = _transExecutionRepo.GetAllPagination(queryFilter, model.start, model.length, "IDTransportExecution");
            }

            var _listLoc = new List<string>();
            var _listDrivers = new List<string>();
            var getLoc = Enumerable.Empty<MasterLocation>();
            var getDrivers = Enumerable.Empty<TransportDriverManagement>();
            if (getTM != null)
            {
                var _locs = getTM.Select(_ => new { _.StartLocation, _.FinishLocation });
                _listLoc.AddRange(_locs.Select(_ => _.StartLocation).ToList());
                _listLoc.AddRange(_locs.Select(_ => _.FinishLocation).ToList());
                _listLoc = _listLoc.Distinct().ToList();

                var _drvs = getTM.Select(_ => new { _.IDDriver1, _.IDDriver2, _.IDCoDriver });
                _listDrivers.AddRange(_drvs.Select(_ => _.IDDriver1).ToList());
                _listDrivers.AddRange(_drvs.Select(_ => _.IDDriver2).ToList());
                _listDrivers.AddRange(_drvs.Select(_ => _.IDCoDriver).ToList());
                _listDrivers = _listDrivers.Distinct().ToList();

                getLoc = _mstLocationRepo.Get(_ => _listLoc.Contains(_.IDLocation));
                getDrivers = _transDriverManagementRepo.Get(_ => _listDrivers.Contains(_.ID));
            }

            var data = new List<TransportExecutionDTO>();

            foreach (var tm in getTM)
            {
                var _startLoc = getLoc.Where(_ => _.IDLocation == tm.StartLocation).FirstOrDefault();
                var _finishLoc = getLoc.Where(_ => _.IDLocation == tm.FinishLocation).FirstOrDefault();
                var _driver1 = getDrivers.Where(_ => _.ID == tm.IDDriver1).FirstOrDefault();
                var _driver2 = getDrivers.Where(_ => _.ID == tm.IDDriver2).FirstOrDefault();
                var _codriver = getDrivers.Where(_ => _.ID == tm.IDCoDriver).FirstOrDefault();
                var _currentPos = tm.TransportPositionDetails.LastOrDefault();

                data.Add(new TransportExecutionDTO
                {
                    IDTransportExecution = tm.IDTransportExecution,
                    TransportNo = tm.TransportNo,
                    TransportDate = tm.TransportDate,
                    TransportStatus = tm.TransportStatus,
                    ActualVehicleType = tm.ActualVehicleType,
                    VendorName = tm.MasterVendor != null ? tm.MasterVendor.VendorName : "",
                    StartLocation = _startLoc == null ? "" : _startLoc.LocationName,
                    FinishLocation = _finishLoc == null ? "" : _finishLoc.LocationName,
                    PoliceRegistrationNumber = tm.PoliceRegNo,
                    Driver1 = _driver1 == null ? "" : _driver1.Name,
                    Driver2 = _driver2 == null ? "" : _driver2.Name,
                    CoDriver = _codriver == null ? "" : _codriver.Name,
                    CurrentLocation = _currentPos == null ? "" : _currentPos.PositionName,
                    CurrentLocationUpdateTime = _currentPos == null ? DateTime.MinValue : ((DateTime)_currentPos.PositionDate),
                    IsActive = tm.IsActive,
                    ContainerNo = tm.TransportVesselMonitorings.Count > 0 ? tm.TransportVesselMonitorings.FirstOrDefault().ContainerNo : "",
                    Via = tm.Via,
                });
            }

            var count = _transExecutionRepo.Count(queryFilter);
            dynamic res = new System.Dynamic.ExpandoObject();
            res.data = data;
            res.total = count;
            
            return res;
        }

        public dynamic GetMonitoringDataTableShip(TransportMonitoringFilterInput input, bool isSuperAdmin, DataTableModel model = null)
        {
            #region FILTERS
            var queryFilter = PredicateHelper.True<TransportVesselMonitoring>();
            queryFilter = queryFilter.And(_ => _.IsActive || isSuperAdmin);
            //queryFilter = queryFilter.And(_ => !_.TransportExecution.TransportNo.StartsWith("E-TN"));

            if (input.transportNo != null && input.transportNo.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                //queryFilter = queryFilter.And(_ => input.transportNo.Contains(_.TransportExecution.TransportNo));
                var transportNo = new List<string>();
                foreach (var s in input.transportNo)
                {
                    if (!string.IsNullOrEmpty(s))
                        transportNo.Add(s);
                }
                if (transportNo.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => transportNo.Contains(_.TransportExecution.TransportNo));
                }
            }
            if (input.dateFrom.HasValue)
            {
                queryFilter = queryFilter.And(_ => _.TransportExecution.TransportDate >= input.dateFrom.Value);
            }
            if (input.dateTo.HasValue)
            {
                queryFilter = queryFilter.And(c => c.TransportExecution.TransportDate <= input.dateTo.Value);
            }
            if (!String.IsNullOrEmpty(input.mode))
            {
                queryFilter = queryFilter.And(_ => _.TransportExecution.TransportMode.ToLower() == input.mode.ToLower());
            }
            if (input.startLocation != null && input.startLocation.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                //queryFilter = queryFilter.And(c => input.startLocation.Contains(c.TransportExecution.StartLocation));
                var startLocation = new List<string>();
                foreach (var s in input.startLocation)
                {
                    if (!string.IsNullOrEmpty(s))
                        startLocation.Add(s);
                }
                if (startLocation.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => startLocation.Contains(c.TransportExecution.StartLocation));
                }
            }
            if (input.transportStatus != null && input.transportStatus.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                //queryFilter = queryFilter.And(c => input.transportStatus.Contains(c.TransportExecution.TransportStatus));
                var transportStatus = new List<string>();
                foreach (var s in input.transportStatus)
                {
                    if (!string.IsNullOrEmpty(s))
                        transportStatus.Add(s);
                }
                if (transportStatus.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => transportStatus.Contains(c.TransportExecution.TransportStatus));
                }
            }

            if (input.orderNo != null && input.orderNo.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                //queryFilter = queryFilter.And(c => c.TransportExecution.TransportOrders.Any(c2 => input.orderNo.Contains(c2.STONo)));
                var orderNo = new List<string>();
                foreach (var s in input.orderNo)
                {
                    if (!string.IsNullOrEmpty(s))
                        orderNo.Add(s);
                }
                if (orderNo.Count() > 0)
                {
                    queryFilter.And(c => c.TransportExecution.TransportOrders.Any(c2 => orderNo.Contains(c2.STONo)));
                }
            }
            if (input.sender != null && input.sender.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                //queryFilter = queryFilter.And(c => c.TransportExecution.TransportOrders.Any(c2 => input.sender.Contains(c2.ActualSenderIDLocation)));
                var sender = new List<string>();
                foreach (var s in input.sender)
                {
                    if (!string.IsNullOrEmpty(s))
                        sender.Add(s);
                }
                if (sender.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => c.TransportExecution.TransportOrders.Any(c2 => sender.Contains(c2.ActualSenderIDLocation)));
                }
            }
            if (input.receiver != null && input.receiver.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                //queryFilter = queryFilter.And(c => c.TransportExecution.TransportOrders.Any(c2 => input.receiver.Contains(c2.ActualReceiverIDLocation)));
                var receiver = new List<string>();
                foreach (var s in input.receiver)
                {
                    if (!string.IsNullOrEmpty(s))
                        receiver.Add(s);
                }
                if (receiver.Count() > 0)
                {
                    queryFilter = queryFilter.And(c => c.TransportExecution.TransportOrders.Any(c2 => receiver.Contains(c2.ActualReceiverIDLocation)));
                }
            }
            if (input.vendor != null && input.vendor.Any(i => !String.IsNullOrWhiteSpace(i)))
            {
                var vendor = new List<string>();
                foreach (var s in input.vendor)
                {
                    if (!string.IsNullOrEmpty(s))
                        vendor.Add(s);
                }
                if (vendor.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => vendor.Contains(_.TransportExecution.IDVendor.ToString()));
                }
                //queryFilter = queryFilter.And(_ => input.vendor.Contains(_.TransportExecution.IDVendor.ToString()));
            }
            if ((bool)input.delayShipment)
            {
                TimeSpan span = new TimeSpan(2, 0, 0, 0);
                TimeSpan span1Day = new TimeSpan(1, 0, 0, 0);
                DateTime currentDate = DateTime.Now;
                queryFilter = queryFilter.And(y => (y.ETD2 != null && y.ETD2 != null ? (y.ETD2.Value - y.ETD1 >= span && y.ATD == null) : false) ||
                    (y.ETD1 != null ? (currentDate - y.ETD1 - span1Day >= span && y.ATD == null && y.ETD2 == null) : false) ||
                    (y.ETD2 != null ? (currentDate - y.ETD2 - span1Day >= span && y.ATD == null) : false));
            }
            #endregion

            #region filter from DataTable
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.search.value))
                {
                    var _us = model.search.value;
                    queryFilter = queryFilter.
                        And(_ =>
                        _.TransportExecution.TransportNo.Contains(_us) || _.TransportExecution.TransportStatus.Contains(_us) ||
                        _.TransportExecution.MasterVendor.VendorName.Contains(_us) || _.TransportExecution.TransportVesselMonitorings.FirstOrDefault().ContainerNo.Contains(_us) ||
                        _.TransportExecution.Via.Contains(_us) || _.TransportExecution.FinishLocation.ToString().Contains(_us) ||
                        _.VesselName.Contains(_us) || _.ETD1.ToString().Contains(_us) ||
                        _.TransportExecution.StartLocation.Contains(_us) || _.ETD2.ToString().Contains(_us) ||
                        _.ATD.ToString().Contains(_us) || _.EstReceived.ToString().Contains(_us) ||
                        _.TransportExecution.TransportDate.ToString().Contains(_us) || _.Remarks.Contains(_us));
                }

                foreach (var fil in model.columns.Where(_ => !String.IsNullOrEmpty(_.search.value)))
                {
                    var _col = fil.data;
                    var _val = fil.search.value;
                    switch (_col)
                    {
                        case "TransportNo":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.TransportNo.Contains(_val));
                            break;
                        case "TransportStatus":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.TransportStatus.Contains(_val));
                            break;
                        case "VendorName":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.MasterVendor.VendorName.Contains(_val));
                            break;
                        case "ActualVehicleType":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.ActualVehicleType.Contains(_val));
                            break;
                        case "StartLocationName":
                            var _getStr = _mstLocationRepo.Get(_ => _.LocationName.Contains(_val)).Select(_ => _.IDLocation);
                            queryFilter = queryFilter.And(_ => _getStr.Contains(_.TransportExecution.StartLocation));
                            break;
                        case "FinishLocationName":
                            var _getFns = _mstLocationRepo.Get(_ => _.LocationName.Contains(_val)).Select(_ => _.IDLocation);
                            queryFilter = queryFilter.And(_ => _getFns.Contains(_.TransportExecution.FinishLocation));
                            break;
                        case "ContainerNo":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.TransportVesselMonitorings.FirstOrDefault().ContainerNo.Contains(_val));
                            break;
                        case "Via":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.Via.Contains(_val));
                            break;
                        case "PoliceRegistrationNumber":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.PoliceRegNo.Contains(_val));
                            break;
                        case "TransportDate":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.TransportDate.ToString().Contains(_val));
                            break;
                        case "Driver1":
                            var _getDrv1 = _transDriverManagementRepo.Get(_ => _.Name.Contains(_val)).Select(_ => _.ID).ToList();
                            queryFilter = queryFilter.And(_ => _getDrv1.Contains(_.TransportExecution.IDDriver1));
                            break;
                        case "Driver2":
                            var _getDrv2 = _transDriverManagementRepo.Get(_ => _.Name.Contains(_val)).Select(_ => _.ID).ToList();
                            queryFilter = queryFilter.And(_ => _getDrv2.Contains(_.TransportExecution.IDDriver2));
                            break;
                        case "CoDriver":
                            var _getcoDrv = _transDriverManagementRepo.Get(_ => _.Name.Contains(_val)).Select(_ => _.ID).ToList();
                            queryFilter = queryFilter.And(_ => _getcoDrv.Contains(_.TransportExecution.IDCoDriver));
                            break;
                        case "CurrentLocation":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.TransportPositionDetails.LastOrDefault().PositionName.Contains(_val));
                            break;
                        case "CurrentLocationUpdateTime":
                            queryFilter = queryFilter.And(_ => _.TransportExecution.TransportPositionDetails.LastOrDefault().PositionDate.ToString().Contains(_val));
                            break;
                    }
                }
            }
            #endregion

            var getTM = Enumerable.Empty<TransportVesselMonitoring>();
            if (model == null)
            {
                getTM = _transportVesselMonitoringRepo.Get(queryFilter);
            }
            else
            {
                getTM = _transportVesselMonitoringRepo.GetAllPagination(queryFilter, model.start, model.length, "IDTransportVesselMonitoring");
            }

            var _listLoc = new List<string>();
            var _listDrivers = new List<string>();
            var getLoc = Enumerable.Empty<MasterLocation>();
            int config = 0;
            if (getTM != null)
            {
                var _locs = getTM.Select(_ => new { _.TransportExecution.StartLocation, _.TransportExecution.FinishLocation });
                _listLoc.AddRange(_locs.Select(_ => _.StartLocation).ToList());
                _listLoc.AddRange(_locs.Select(_ => _.FinishLocation).ToList());
                _listLoc = _listLoc.Distinct().ToList();

                getLoc = _mstLocationRepo.Get(_ => _listLoc.Contains(_.IDLocation));
                var getConfig = _masterConfigurationRepo.Get(x => x.PageName.Contains("VesselMonitoringDetail") && x.Description.Contains("est. received") && x.IsActive);
                config = getConfig.Count() > 0 ? Convert.ToInt16(getConfig.First().Value) : 0;
            }

            var data = Mapper.Map<List<TransportVesselMonitoringDTO>>(getTM);

            foreach (var tm in data)
            {
                var _startLoc = getLoc.Where(_ => _.IDLocation == tm.TransportExecution.StartLocation).FirstOrDefault();
                var _finishLoc = getLoc.Where(_ => _.IDLocation == tm.TransportExecution.FinishLocation).FirstOrDefault();

                tm.StartLocationName = _startLoc == null ? "" : _startLoc.LocationName;
                tm.FinishLocationName = _finishLoc == null ? "" : _finishLoc.LocationName;
                tm.ETA2 = tm.ETA2.HasValue && tm.ETA1.HasValue && tm.ETD1.HasValue && tm.ETD2.HasValue ? tm.ETA2 : (tm.ETA1 + (tm.ETD2 - tm.ETD1));
                tm.EstReceived = tm.EstReceived.HasValue ? tm.EstReceived : (tm.ETA2.HasValue ? tm.ETA2.Value.AddDays(config) : (tm.ETA1.HasValue ? tm.ETA1.Value.AddDays(config) : (DateTime?)null));
                var _tOrder = getTM.Where(_ => _.IDTransportVesselMonitoring.Equals(tm.IDTransportVesselMonitoring)).FirstOrDefault().TransportExecution.TransportOrders.ToList();
                //var _listOrder = Mapper.Map<List<TransportOrderList>>(_tOrder);
                tm.TransportExecution.TransportOrderList = Mapper.Map<List<TransportOrderList>>(_tOrder);

                if (input.isForExport)
                {
                    getLoc = _mstLocationRepo.GetAll().Where(x => x.IsActive).ToList();

                    foreach (var rec in tm.TransportExecution.TransportOrderList)
                    {
                        var _senderName = getLoc.Where(_ => _.IDLocation == rec.ActualSenderIDLocation).FirstOrDefault();
                        var _receiverName = getLoc.Where(_ => _.IDLocation == rec.ActualReceiverIDLocation).FirstOrDefault();

                        rec.ActualSenderLocationName = _senderName == null ? "" : _senderName.LocationName;
                        rec.ActualReceiverLocationName = _receiverName == null ? "" : _receiverName.LocationName;
                    }
                }
            }

            var count = _transportVesselMonitoringRepo.Count(queryFilter);
            dynamic res = new System.Dynamic.ExpandoObject();
            res.data = data;
            res.total = count;

            return res;
        }

        public List<TransportExecutionDTO> GetListTruckTrainServerSide(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl, bool isSuperAdmin, object req, int skip, int take)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        from TO in LJOIN_1.DefaultIfEmpty()
                        join TV in context.TransportVesselMonitorings on TE.IDTransportExecution equals TV.IDTransportExecution into LJOIN_2
                        from TV in LJOIN_2.DefaultIfEmpty()
                        join ML in context.MasterLocations on TE.StartLocation equals ML.IDLocation into RJOIN_1
                        from ML in RJOIN_1.DefaultIfEmpty()
                        join ML2 in context.MasterLocations on TE.FinishLocation equals ML2.IDLocation into LJOIN_3
                        from ML2 in LJOIN_3.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_2
                        from MV in RJOIN_2.DefaultIfEmpty()
                        where TE.TransportMode == mode
                        select new TransportExecutionDTO()
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            IDTransportOrder = (int?)TO.IDTransportOrder,
                            TransportNo = TE.TransportNo,
                            TransportStatus = TE.TransportStatus,
                            ActualVehicleType = TE.ActualVehicleType,
                            VehicleType = TO.VehicleType,
                            PoliceRegistrationNumber = TE.PoliceRegNo,
                            Driver1 = TE.IDDriver1,
                            Driver2 = TE.IDDriver2,
                            CoDriver = TE.IDCoDriver,
                            STONo = TO.STONo,
                            IDVendor = (int?)TE.IDVendor,
                            VendorName = MV.VendorName,
                            IDSender = TO.SenderIDLocation,
                            IDReceiver = TO.ReceiverIDLocation,
                            IDStartLocation = TE.StartLocation,
                            IDFinishLocation = TE.FinishLocation != null ? TE.FinishLocation : "",
                            StartLocation = ML.LocationName,
                            FinishLocation = TE.FinishLocation != null ? ML2.LocationName : "",
                            TransportDate = TE.TransportDate,
                            ETD1 = TV.ETD1,
                            ETD2 = TV.ETD2,
                            ATD = TV.ATD,
                            EstReceived = TV.EstReceived,
                            Remarks = TV.Remarks,
                            IsActive = TE.IsActive,
                            GRBy = TO.GRBy,
                            GIBy = TO.GIBy,
                            //TransportationMode = MV.TransportationMode
                        }
                        );

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fts = fltrts.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] fvn = fltrvn.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] ffl = fltrfl.Split(',').ToArray();
            if (fltrtn.Length > 1) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltrts.Length > 1) { dbResult = dbResult.Where(q => fts.Contains(q.TransportStatus)); }
            if (fltron.Length > 1) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltrvn.Length > 1) { dbResult = dbResult.Where(q => fvn.Contains(q.IDVendor.ToString())); }
            if (fltrse.Length > 1) { dbResult = dbResult.Where(q => fse.Contains(q.IDSender)); }
            if (fltrre.Length > 1) { dbResult = dbResult.Where(q => fre.Contains(q.IDReceiver)); }
            if (fltrsl.Length > 1) { dbResult = dbResult.Where(q => fsl.Contains(q.IDStartLocation)); }
            if (fltrfl.Length > 1) { dbResult = dbResult.Where(q => ffl.Contains(q.IDFinishLocation)); }
            if (fltrdf.Length > 1)
            {
                try
                {
                    DateTime fdf = Convert.ToDateTime(fltrdf);
                    dbResult = dbResult.Where(q => q.TransportDate >= fdf);
                }
                catch { }
            }
            if (fltrdt.Length > 1)
            {
                try
                {
                    DateTime fdt = Convert.ToDateTime(fltrdt);
                    dbResult = dbResult.Where(q => q.TransportDate <= fdt);
                }
                catch { }
            }

            dbResult = dbResult.OrderBy(o => o.IDTransportOrder).OrderBy(o => o.IDStartLocation).OrderBy(o => o.TransportNo);
            //var tempRes = dbResult
            //    .OrderBy(o => o.IDTransportOrder)
            //    .OrderBy(o => o.IDStartLocation)
            //    .OrderBy(o => o.TransportNo)
            //    .Skip(skip)
            //    .Take(take)
            //    .ToList();

            var reqFilter = (object[])req.GetType().GetProperty("colums").GetValue(req);
            var search = (string)req.GetType().GetProperty("search").GetValue(req);
            var sortBy = (string)req.GetType().GetProperty("sortBy").GetValue(req);
            var sort = (string)req.GetType().GetProperty("sort").GetValue(req);

            if (sort == "asc")
            {
                dbResult = sortBy == "TransportNo" ? dbResult.OrderBy(o => o.TransportNo) :
                        sortBy == "TransportDate" ? dbResult.OrderBy(o => o.TransportDate) :
                        sortBy == "ActualVehicleType" ? dbResult.OrderBy(o => o.ActualVehicleType) :
                        sortBy == "VendorName" ? dbResult.OrderBy(o => o.VendorName) :
                        sortBy == "TransportStatus" ? dbResult.OrderBy(o => o.TransportStatus) :
                        sortBy == "StartLocation" ? dbResult.OrderBy(o => o.StartLocation) :
                        sortBy == "FinishLocation" ? dbResult.OrderBy(o => o.FinishLocation) :
                        sortBy == "PoliceRegistrationNumber" ? dbResult.OrderBy(o => o.PoliceRegistrationNumber) :
                        sortBy == "Driver1" ? dbResult.OrderBy(o => o.Driver1) :
                        sortBy == "Driver2" ? dbResult.OrderBy(o => o.Driver2) : dbResult.OrderBy(o => o.CoDriver);
            }
            else
            {
                dbResult = sortBy == "TransportNo" ? dbResult.OrderByDescending(o => o.TransportNo) :
                        sortBy == "TransportDate" ? dbResult.OrderByDescending(o => o.TransportDate) :
                        sortBy == "ActualVehicleType" ? dbResult.OrderByDescending(o => o.ActualVehicleType) :
                        sortBy == "VendorName" ? dbResult.OrderByDescending(o => o.VendorName) :
                        sortBy == "TransportStatus" ? dbResult.OrderByDescending(o => o.TransportStatus) :
                        sortBy == "StartLocation" ? dbResult.OrderByDescending(o => o.StartLocation) :
                        sortBy == "FinishLocation" ? dbResult.OrderByDescending(o => o.FinishLocation) :
                        sortBy == "PoliceRegistrationNumber" ? dbResult.OrderByDescending(o => o.PoliceRegistrationNumber) :
                        sortBy == "Driver1" ? dbResult.OrderByDescending(o => o.Driver1) :
                        sortBy == "Driver2" ? dbResult.OrderByDescending(o => o.Driver2) : dbResult.OrderBy(o => o.CoDriver);
            }

            if (search != "")
            {
                dbResult = dbResult.Where(q => (q.IsActive || isSuperAdmin) && (q.TransportNo != null && !q.TransportNo.StartsWith("E-TN")) && (search.Contains(q.TransportNo) ||
                                                search.Contains(q.TransportDate.ToString()) ||
                                                search.Contains(q.ActualVehicleType) ||
                                                search.Contains(q.VendorName) ||
                                                search.Contains(q.TransportStatus) ||
                                                search.Contains(q.StartLocation) ||
                                                search.Contains(q.FinishLocation) ||
                                                search.Contains(q.PoliceRegistrationNumber) ||
                                                search.Contains(q.Driver1) ||
                                                search.Contains(q.Driver2) ||
                                                search.Contains(q.CoDriver))
                                                );
            }
            else
            {
                dbResult = dbResult.Where(c => (c.IsActive || isSuperAdmin) && (c.TransportNo != null && !c.TransportNo.StartsWith("E-TN")));
            }

            foreach (var r in reqFilter)
            {
                string searchValue = (string)r.GetType().GetProperty("searchValue").GetValue(r);
                string fields = (string)r.GetType().GetProperty("data").GetValue(r);
                if (searchValue != "")
                {
                    if (fields == "TransportNo")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.TransportNo));
                    }
                    else if (fields == "TransportDate")
                    {
                        try
                        {
                            DateTime dateField = Convert.ToDateTime(searchValue);
                            dbResult = dbResult.Where(q => q.TransportDate == dateField);
                        }
                        catch { }
                    }
                    else if (fields == "ActualVehicleType")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.ActualVehicleType));
                    }
                    else if (fields == "VendorName")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.VendorName));
                    }
                    else if (fields == "TransportStatus")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.TransportStatus));
                    }
                    else if (fields == "StartLocation")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.StartLocation));
                    }
                    else if (fields == "FinishLocation")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.FinishLocation));
                    }
                    else if (fields == "PoliceRegistrationNumber")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.PoliceRegistrationNumber));
                    }
                    else if (fields == "Driver1")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.Driver1));
                    }
                    else if (fields == "Driver2")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.Driver2));
                    }
                    else if (fields == "CoDriver")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.CoDriver));
                    }
                }
            }

            var tempRes = dbResult.Skip(skip).Take(take).ToList();
            foreach (var r in tempRes)
            {
                SetCurrentLocation(r);
            }

            return tempRes;
        }

        public int GetCountTruckTrainServerSide(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl, bool isSuperAdmin, object req, string stOrder)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        from TO in LJOIN_1.DefaultIfEmpty()
                        join TV in context.TransportVesselMonitorings on TE.IDTransportExecution equals TV.IDTransportExecution into LJOIN_2
                        from TV in LJOIN_2.DefaultIfEmpty()
                        join ML in context.MasterLocations on TE.StartLocation equals ML.IDLocation into RJOIN_1
                        from ML in RJOIN_1.DefaultIfEmpty()
                        join ML2 in context.MasterLocations on TE.FinishLocation equals ML2.IDLocation into LJOIN_3
                        from ML2 in LJOIN_3.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_2
                        from MV in RJOIN_2.DefaultIfEmpty()
                        where TE.TransportMode == mode
                        select new
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            IDTransportOrder = (int?)TO.IDTransportOrder,
                            TransportNo = TE.TransportNo,
                            TransportStatus = TE.TransportStatus,
                            ActualVehicleType = TE.ActualVehicleType,
                            VehicleType = TO.VehicleType,
                            PoliceRegistrationNumber = TE.PoliceRegNo,
                            Driver1 = TE.IDDriver1,
                            Driver2 = TE.IDDriver2,
                            CoDriver = TE.IDCoDriver,
                            STONo = TO.STONo,
                            IDVendor = (int?)TE.IDVendor,
                            VendorName = MV.VendorName,
                            IDSender = TO.SenderIDLocation,
                            IDReceiver = TO.ReceiverIDLocation,
                            IDStartLocation = TE.StartLocation,
                            IDFinishLocation = TE.FinishLocation != null ? TE.FinishLocation : "",
                            StartLocation = ML.LocationName,
                            FinishLocation = TE.FinishLocation != null ? ML2.LocationName : "",
                            TransportDate = TE.TransportDate,
                            ETD1 = TV.ETD1,
                            ETD2 = TV.ETD2,
                            ATD = TV.ATD,
                            EstReceived = TV.EstReceived,
                            Remarks = TV.Remarks,
                            IsActive = TE.IsActive,
                            GRBy = TO.GRBy,
                            GIBy = TO.GIBy,
                            OrderStatus = TO.OrderStatus
                            //TransportationMode = MV.TransportationMode
                        }
                        );

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fts = fltrts.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] fvn = fltrvn.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] ffl = fltrfl.Split(',').ToArray();

            if (fltrtn.Length > 1) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltrts.Length > 1) { dbResult = dbResult.Where(q => fts.Contains(q.TransportStatus)); }
            if (fltron.Length > 1) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltrvn.Length > 1) { dbResult = dbResult.Where(q => fvn.Contains(q.IDVendor.ToString())); }
            if (fltrse.Length > 1) { dbResult = dbResult.Where(q => fse.Contains(q.IDSender)); }
            if (fltrre.Length > 1) { dbResult = dbResult.Where(q => fre.Contains(q.IDReceiver)); }
            if (fltrsl.Length > 1) { dbResult = dbResult.Where(q => fsl.Contains(q.IDStartLocation)); }
            if (fltrfl.Length > 1) { dbResult = dbResult.Where(q => ffl.Contains(q.IDFinishLocation)); }
            if (fltrdf.Length > 1)
            {
                try
                {
                    DateTime fdf = Convert.ToDateTime(fltrdf);
                    dbResult = dbResult.Where(q => q.TransportDate >= fdf);
                }
                catch { }
            }
            if (fltrdt.Length > 1)
            {
                try
                {
                    DateTime fdt = Convert.ToDateTime(fltrdt);
                    dbResult = dbResult.Where(q => q.TransportDate <= fdt);
                }
                catch { }
            }

            if (stOrder != "")
            {
                dbResult = dbResult.Where(q => q.TransportStatus == stOrder);
            }

            var tempCount = 0;
            if (req == null)
            {
                tempCount = dbResult.Count();
                return tempCount;
            }

            var reqFilter = (object[])req.GetType().GetProperty("colums").GetValue(req);
            var search = (string)req.GetType().GetProperty("search").GetValue(req);
            var sortBy = (string)req.GetType().GetProperty("sortBy").GetValue(req);
            var sort = (string)req.GetType().GetProperty("sort").GetValue(req);
            if (search != "")
            {
                dbResult = dbResult.Where(q => (q.IsActive || isSuperAdmin) && (q.TransportNo != null && !q.TransportNo.StartsWith("E-TN")) && (search.Contains(q.TransportNo) ||
                                                search.Contains(q.TransportDate.ToString()) ||
                                                search.Contains(q.ActualVehicleType) ||
                                                search.Contains(q.VendorName) ||
                                                search.Contains(q.TransportStatus) ||
                                                search.Contains(q.StartLocation) ||
                                                search.Contains(q.FinishLocation) ||
                                                search.Contains(q.PoliceRegistrationNumber) ||
                                                search.Contains(q.Driver1) ||
                                                search.Contains(q.Driver2) ||
                                                search.Contains(q.CoDriver))
                                                );
            }
            else
            {
                dbResult = dbResult.Where(c => (c.IsActive || isSuperAdmin) && (c.TransportNo != null && !c.TransportNo.StartsWith("E-TN")));
            }
            foreach (var r in reqFilter)
            {
                string searchValue = (string)r.GetType().GetProperty("searchValue").GetValue(r);
                string fields = (string)r.GetType().GetProperty("data").GetValue(r);
                if (searchValue != "")
                {
                    if (fields == "TransportNo")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.TransportNo));
                    }
                    else if (fields == "TransportDate")
                    {
                        try
                        {
                            DateTime dateField = Convert.ToDateTime(searchValue);
                            dbResult = dbResult.Where(q => q.TransportDate == dateField);
                        }
                        catch { }
                    }
                    else if (fields == "ActualVehicleType")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.ActualVehicleType));
                    }
                    else if (fields == "VendorName")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.VendorName));
                    }
                    else if (fields == "TransportStatus")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.TransportStatus));
                    }
                    else if (fields == "StartLocation")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.StartLocation));
                    }
                    else if (fields == "FinishLocation")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.FinishLocation));
                    }
                    else if (fields == "PoliceRegistrationNumber")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.PoliceRegistrationNumber));
                    }
                    else if (fields == "Driver1")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.Driver1));
                    }
                    else if (fields == "Driver2")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.Driver2));
                    }
                    else if (fields == "CoDriver")
                    {
                        dbResult = dbResult.Where(q => searchValue.Contains(q.CoDriver));
                    }
                }
            }
            // Total 
            tempCount = dbResult.Count();

            return tempCount;
        }

        public void updateShip(TransportMonitoringViewInput input, string userid)
        {
            _transportVesselMonitoringRepo.UpdateVessel(input, userid);
            _transportOrderRepo.updateOrderFromShip(input, userid);
            //var result = false;
            //try {
                
            //    return true;
            //}
            //catch (Exception ex) {
                
            //}
            //return result;
        }

        public List<TransportVesselMonitoringDTO> getDataShip(TransportMonitoringViewInput input)
        {
            var queryFilter = PredicateHelper.True<TransportVesselMonitoring>();
            var toItem = new TransportOrder();
            toItem.STONo = input.STONo;
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportExecution.TransportMode.ToUpper() == "SHIP");
            queryFilter = queryFilter.And(x => x.TransportExecution.IsActive);
            queryFilter = queryFilter.And(x => !x.TransportExecution.TransportNo.Contains("E-TN"));
            if (input.IDTransportVesselMonitoring != 0) queryFilter = queryFilter.And(x => x.IDTransportVesselMonitoring == input.IDTransportVesselMonitoring);
            if (input.IDTransportExecution != 0) queryFilter = queryFilter.And(x => x.TransportExecution.IDTransportExecution == input.IDTransportExecution);
            if (input.TransportNo != null) queryFilter = queryFilter.And(x => input.TransportNo.Contains(x.TransportExecution.TransportNo));
            if (input.StartLocation != null) queryFilter = queryFilter.And(x => input.StartLocation.Contains(x.TransportExecution.StartLocation));
            if (input.TransportStatus != null) queryFilter = queryFilter.And(x => input.TransportStatus.Contains(x.TransportExecution.TransportStatus));
            if (input.DateOfStuffingFrom != null)
            {
                DateTime temp = Convert.ToDateTime(input.DateOfStuffingFrom);
                queryFilter = queryFilter.And(x => x.TransportExecution.TransportDate >= temp);
            }
            if (input.DateOfStuffingTo != null)
            {
                DateTime temp = Convert.ToDateTime(input.DateOfStuffingTo);
                queryFilter = queryFilter.And(x => x.TransportExecution.TransportDate <= temp);
            }

            if (input.VendorID != null)
            {
                var listVendor = input.VendorID.Split(',').ToList();
                queryFilter = queryFilter.And(x => input.VendorID.Contains(x.TransportExecution.IDVendor.ToString()));
            }

            var dbResult = _generalRepo.Get(queryFilter);
            var result = Mapper.Map<List<TransportVesselMonitoringDTO>>(dbResult);
            
            var listLocation = _mstLocationRepo.GetAll().Where(x => x.IsActive).ToList();
            var getConfig = _masterConfigurationRepo.GetAll().Where(x => x.PageName.Contains("VesselMonitoringDetail") && x.Description.Contains("est. received") && x.IsActive);
            var config = getConfig.Count() > 0 ? Convert.ToInt16(getConfig.First().Value) : 0;
            foreach (var record in result)
            {
                record.ETA2 = record.ETA2.HasValue && record.ETA1.HasValue && record.ETD1.HasValue && record.ETD2.HasValue ? record.ETA2 : (record.ETA1 + (record.ETD2 - record.ETD1));
                record.EstReceived = record.EstReceived.HasValue ? record.EstReceived : (record.ETA2.HasValue ? record.ETA2.Value.AddDays(config) : (record.ETA1.HasValue ? record.ETA1.Value.AddDays(config) : (DateTime?)null));
                if (record.TransportExecution.StartLocation != null) 
                    record.StartLocationName = listLocation.Where(x => x.IDLocation == record.TransportExecution.StartLocation) == null ? "" : listLocation.Where(x => x.IDLocation == record.TransportExecution.StartLocation).Select(x => x.LocationName).Single().ToString();
                if (record.TransportExecution.FinishLocation != null)
                    record.FinishLocationName = listLocation.Where(x => x.IDLocation == record.TransportExecution.FinishLocation) == null ? "" : listLocation.Where(x => x.IDLocation == record.TransportExecution.FinishLocation).Select(x => x.LocationName).Single().ToString();

                var dataTO = Mapper.Map<List<TransportOrderDTO>>(dbResult.Where(x => x.IDTransportExecution == record.IDTransportExecution).Select(x => x.TransportExecution).Single().TransportOrders.ToList());
                record.TransportExecution.TransportOrderList = Mapper.Map<List<TransportOrderDTO>, List<TransportOrderList>>(dataTO);

                foreach (var rec in record.TransportExecution.TransportOrderList)
                {
                    rec.ActualSenderLocationName = listLocation.Where(x => x.IDLocation == rec.ActualSenderIDLocation) == null ? "" : listLocation.Where(x => x.IDLocation == rec.ActualSenderIDLocation).Select(x => x.LocationName).Single().ToString();
                    rec.ActualReceiverLocationName = listLocation.Where(x => x.IDLocation == rec.ActualReceiverIDLocation) == null ? "" : listLocation.Where(x => x.IDLocation == rec.ActualReceiverIDLocation).Select(x => x.LocationName).Single().ToString();
                }
            }

            if (input.STONo != null) {
                var listSTO = input.STONo.Split(',').ToList();
                result = result.Where(x => x.TransportExecution.TransportOrderList.Any(y => input.STONo.Contains(y.STONo))).ToList();
            }
            if (input.SenderIDLocation != null)
            {
                result = result.Where(x => x.TransportExecution.TransportOrderList.Any(y => input.SenderIDLocation.Contains(y.ActualSenderIDLocation))).ToList();
            }
            if (input.ReceiverIDLocation != null)
            {
                result = result.Where(x => x.TransportExecution.TransportOrderList.Any(y => input.ReceiverIDLocation.Contains(y.ActualReceiverIDLocation))).ToList();
            }
            if (input.DelayShipment)
            {
                TimeSpan span = new TimeSpan(2, 0, 0, 0);
                TimeSpan span1Day = new TimeSpan(1, 0, 0, 0);
                DateTime currentDate = DateTime.Now;
                result = result
                    .Where(y => (y.ETD2 != null && y.ETD2 != null ? (y.ETD2.Value - y.ETD1 >= span && y.ATD == null) : false) ||
                    (y.ETD1 != null ? (currentDate - y.ETD1 - span1Day >= span && y.ATD == null && y.ETD2 == null) : false) ||
                    (y.ETD2 != null ? (currentDate - y.ETD2 - span1Day >= span && y.ATD == null) : false))
                    .ToList();
            }

            return result;
        }

        public List<TransportOrderDTO> GetRowListView(string idte)
        {
            Int64 idten = Int64.Parse(idte);

            var context = new TOMContextDB();

            var dbResult = (
                        from TO in context.TransportOrders
                        join MLTOS in context.MasterLocations on TO.SenderIDLocation equals MLTOS.IDLocation into RJOIN_1
                        from MLTOS in RJOIN_1.DefaultIfEmpty()
                        join MLTOR in context.MasterLocations on TO.ReceiverIDLocation equals MLTOR.IDLocation into RJOIN_2
                        from MLTOR in RJOIN_2.DefaultIfEmpty()
                        where TO.IDTransportExecution == idten
                        select new TransportOrderDTO
                        {
                            IDTransportOrder = TO.IDTransportOrder,
                            STONo = TO.STONo,
                            LocNameSender = MLTOS.LocationName,
                            LocNameReceiver = MLTOR.LocationName,
                            SenderLocationName = MLTOS.LocationName,
                            ReceiverLocationName = MLTOR.LocationName,
                            SenderIDLocation = MLTOS.IDLocation,
                            ReceiverIDLocation = MLTOR.IDLocation,
                            LeadTime = TO.LeadTime,
                            EstArrivalDate = TO.EstArrivalDate,
                            IDRequest = TO.IDRequest,
                            GIDate = TO.GIDate,
                            GRDate = TO.GRDate,
                            OrderStatus = TO.OrderStatus,
                            OrderType = TO.OrderType,
                            IsMaterialReceived = TO.IsMaterialReceived
                        }
                        );

            return dbResult.OrderBy(o => o.IDTransportOrder).ToList();
        }

        public TransportExecutionDTO GetDetail(string id)
        {
            Int64 idd, idn = 0;
            if (!String.IsNullOrEmpty(id) && Int64.TryParse(id, out idd) == true) { idn = Int64.Parse(id); }

            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN
                        from TO in LJOIN.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_1
                        from MV in RJOIN_1.DefaultIfEmpty()
                        join MLTES in context.MasterLocations on TE.StartLocation equals MLTES.IDLocation into RJOIN_4
                        from MLTES in RJOIN_4.DefaultIfEmpty()
                        join ML2TES in context.MasterLocations on TE.FinishLocation equals ML2TES.IDLocation into LJOIN_2
                        from ML2TES in LJOIN_2.DefaultIfEmpty()
                        join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_6
                        from TDTE1 in RJOIN_6.DefaultIfEmpty()
                        join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_7
                        from TDTE2 in RJOIN_7.DefaultIfEmpty()
                        join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_8
                        from TDTE3 in RJOIN_8.DefaultIfEmpty()
                        where TE.IDTransportExecution == idn
                        select new
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            ActualVehicleType = TE.ActualVehicleType,                           
                            TransportStatus = TE.TransportStatus,
                            TransportNo = TE.TransportNo,
                            TransportDate = TE.TransportDate,
                            VendorName = MV.VendorName,
                            VehicleType = TO.VehicleType,
                            StartLocation = MLTES.LocationName,
                            FinishLocation = ML2TES.LocationName,
                            PoliceRegNo = TE.PoliceRegNo,
                            IDDriver1 = TDTE1.Name,
                            IDDriver2 = TDTE2.Name,
                            IDCoDriver = TDTE3.Name
                        }
                        );

            return dbResult
                .GroupBy(g => g.TransportNo)
                .Select(f => new TransportExecutionDTO
                {
                    IDTransportExecution = f.FirstOrDefault().IDTransportExecution,
                    ActualVehicleType = f.FirstOrDefault().ActualVehicleType,
                    TransportStatus = f.FirstOrDefault().TransportStatus,
                    TransportNo = f.FirstOrDefault().TransportNo,
                    TransportDate = f.FirstOrDefault().TransportDate,
                    VendorName = f.FirstOrDefault().VendorName,
                    VehicleType = f.FirstOrDefault().VehicleType,
                    StartLocation = f.FirstOrDefault().StartLocation,
                    FinishLocation = f.FirstOrDefault().FinishLocation,
                    PoliceRegNo = f.FirstOrDefault().PoliceRegNo,
                    IDDriver1 = f.FirstOrDefault().IDDriver1,
                    IDDriver2 = f.FirstOrDefault().IDDriver2,
                    IDCoDriver = f.FirstOrDefault().IDCoDriver
                })
                .SingleOrDefault();
        }

        public List<TransportExecutionDTO> GetExportXlsShip(string fltron)
        {
            DateTime dtnull = Convert.ToDateTime("0001/01/01");
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN
                        from TO in LJOIN.DefaultIfEmpty()
                        join TV in context.TransportVesselMonitorings on TE.IDTransportExecution equals TV.IDTransportExecution into LJOIN_2
                        from TV in LJOIN_2.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_1
                        from MV in RJOIN_1.DefaultIfEmpty()
                        join MLTOS in context.MasterLocations on TO.SenderIDLocation equals MLTOS.IDLocation into RJOIN_2
                        from MLTOS in RJOIN_2.DefaultIfEmpty()
                        join MLTOR in context.MasterLocations on TO.ReceiverIDLocation equals MLTOR.IDLocation into RJOIN_3
                        from MLTOR in RJOIN_3.DefaultIfEmpty()
                        join MLTES in context.MasterLocations on TE.StartLocation equals MLTES.IDLocation into RJOIN_4
                        from MLTES in RJOIN_4.DefaultIfEmpty()
                        join MLTEF in context.MasterLocations on TE.StartLocation equals MLTEF.IDLocation into RJOIN_5
                        from MLTEF in RJOIN_5.DefaultIfEmpty()
                        join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_6
                        from TDTE1 in RJOIN_6.DefaultIfEmpty()
                        join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_7
                        from TDTE2 in RJOIN_7.DefaultIfEmpty()
                        join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_8
                        from TDTE3 in RJOIN_8.DefaultIfEmpty()
                        where TE.TransportMode == "ship" && TO.IsActive == true
                        orderby TE.IDTransportExecution, TO.IDTransportOrder
                        select new TransportExecutionDTO
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            IDTransportOrder = TO.IDTransportOrder,
                            IDStartLocation = TE.StartLocation,
                            TransportNo = TE.TransportNo,
                            TransportDate = TE.TransportDate,
                            StartLocation = MLTES.LocationName,
                            VendorName = MV.VendorName,
                            ETD1 = TV.ETD1 != null ? TV.ETD1 : dtnull,
                            ETD2 = TV.ETD2 != null ? TV.ETD2 : dtnull,
                            ATD = TV.ATD != null ? TV.ATD : dtnull,
                            EstReceived = TV.EstReceived != null ? TV.EstReceived : dtnull,
                            Remarks = TE.Remarks,
                            TransportStatus = TE.TransportStatus                            
                        }
                        );

            string[] fon = fltron.Split(',').ToArray();
            dbResult = dbResult.Where(q => fon.Contains(q.IDTransportOrder.ToString()));

            return dbResult
                .OrderByDescending(o => o.TransportDate)
                .OrderBy(o => o.IDTransportExecution)
                .OrderBy(o => o.IDTransportOrder)
                .OrderBy(o => o.IDStartLocation)
                .OrderBy(o => o.TransportNo)
                .ToList();
        }

        public List<TransportExecutionDTO> GetExportXls(string tab, string fltron)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN
                        from TO in LJOIN.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_1
                        from MV in RJOIN_1.DefaultIfEmpty()
                        join MLTOS in context.MasterLocations on TO.SenderIDLocation equals MLTOS.IDLocation into RJOIN_2
                        from MLTOS in RJOIN_2.DefaultIfEmpty()
                        join MLTOR in context.MasterLocations on TO.ReceiverIDLocation equals MLTOR.IDLocation into RJOIN_3
                        from MLTOR in RJOIN_3.DefaultIfEmpty()
                        join MLTES in context.MasterLocations on TE.StartLocation equals MLTES.IDLocation into RJOIN_4
                        from MLTES in RJOIN_4.DefaultIfEmpty()
                        join MLTEF in context.MasterLocations on TE.StartLocation equals MLTEF.IDLocation into RJOIN_5
                        from MLTEF in RJOIN_5.DefaultIfEmpty()
                        join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_6
                        from TDTE1 in RJOIN_6.DefaultIfEmpty()
                        join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_7
                        from TDTE2 in RJOIN_7.DefaultIfEmpty()
                        join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_8
                        from TDTE3 in RJOIN_8.DefaultIfEmpty()
                        where TE.TransportMode == tab && TO.IsActive == true
                        orderby TE.IDTransportExecution, TO.IDTransportOrder
                        select new TransportExecutionDTO
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            TransportNo = TE.TransportNo,
                            TransportDate = TE.TransportDate,
                            TransportStatus = TE.TransportStatus,
                            STONo = TO.STONo,
                            IDVendor = TE.IDVendor,
                            VendorName = MV.VendorName,
                            IDSender = TO.SenderIDLocation,
                            Sender = MLTOS.LocationName,
                            IDReceiver = TO.ReceiverIDLocation,
                            Receiver = MLTOR.LocationName,
                            IDStartLocation = TE.StartLocation,
                            StartLocation = MLTES.LocationName,
                            IDFinishLocation = TE.FinishLocation,
                            FinishLocation = MLTEF.LocationName,
                            VehicleType = TO.VehicleType,
                            PoliceRegNo = TE.PoliceRegNo,
                            IDDriver1 = TDTE1.Name,
                            IDDriver2 = TDTE2.Name,
                            IDCoDriver = TDTE3.Name,
                            IsActive = TE.IsActive,
                            IDTransportOrder = TO.IDTransportOrder,
                            IsMaterialReceived = TO.IsMaterialReceived == true ? "Yes" : "No",
                            UpdatedBy = TE.UpdatedBy,
                            UpdatedDate = TE.UpdatedDate
                        }
                        );

            if (!string.IsNullOrWhiteSpace(fltron))
            {
                string[] fon = fltron.Split(',').ToArray();
                dbResult = dbResult.Where(q => fon.Contains(q.IDTransportOrder.ToString()));
            }

            return dbResult
                .OrderByDescending(o => o.TransportDate)
                .OrderBy(o => o.IDTransportExecution)
                .OrderBy(o => o.IDTransportOrder)
                .OrderBy(o => o.IDStartLocation)
                .OrderBy(o => o.TransportNo)
                .ToList();
        }

        public bool GetSheetUpdateXls(string tn, DateTime? et, DateTime? td, DateTime? ta, DateTime? tb)
        {
            var context = new TOMContextDB();

            var dbResult = (
                            from TE in context.TransportExecutions
                            join TV in context.TransportVesselMonitorings on TE.IDTransportExecution equals TV.IDTransportExecution into LJOIN
                            from TV in LJOIN.DefaultIfEmpty()
                            where TE.TransportNo == tn
                            select new { TE, TV }
                            )
                            .ToList();

            var getConfig = context.MasterConfigurations.FirstOrDefault(x => x.PageName.Contains("VesselMonitoringDetail") && x.Description.Contains("est. received") && x.IsActive);
            var config = getConfig != null ? Convert.ToInt16(getConfig.Value) : 0;

            if (dbResult.Count > 0)
            {
                foreach (var col in dbResult)
                {
                    col.TV.ETD2 = et;
                    col.TV.ATD = td;
                    col.TV.ATA = ta;
                    col.TV.ActualTimeBerthing = tb;
                    col.TV.ETA2 = col.TV.ETA1 != null && col.TV.ETD1 != null ? col.TV.ETA1 + (et - col.TV.ETD1) : null;
                    col.TV.EstReceived = col.TV.ETA2.HasValue ? col.TV.ETA2.Value.AddDays(config) : (col.TV.ETA1.HasValue ? col.TV.ETA1.Value.AddDays(config) : (DateTime?)null);
                    context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetSheetPOWeekXls(string nod, int? wk)
        {
            var context = new TOMContextDB();

            var dbResult = (
                            from TO in context.TransportOrders
                            where TO.STONo == nod
                            select new { TO }
                            )
                            .ToList();

            if (dbResult.Count > 0)
            {
                foreach (var col in dbResult)
                {
                    col.TO.POWeek = wk;
                    context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetImportXlsGI(string sto, string ods, DateTime gin)
        {
            var context = new TOMContextDB();

            var dbResult = (
                            from TO in context.TransportOrders
                            where TO.STONo == sto
                            select new { TO }
                            )
                            .ToList();

            if (dbResult.Count > 0)
            {
                foreach (var col in dbResult)
                {
                    col.TO.OrderStatus = ods;
                    col.TO.GIDate = gin;
                    context.SaveChanges();
                }
            }

            return true;
        }
        public bool ImportXlsGI(IEnumerable<TransportMonitoringUploadInput> imported, string username)
        {
            var context = new TOMContextDB();
            
            List<string> _statuses = new List<string>() {
                    "In Process",
                    "On Delivery",
                    "Arrive At Destination and Waiting For Confirmation",
                    "Complete"
            };

            foreach (var data in imported)
            {
                if (string.IsNullOrWhiteSpace(data.OrderNumber)) continue;
                var eTO = context.TransportOrders.Where(to => to.STONo == data.OrderNumber).FirstOrDefault();
                if (eTO != null)
                {
                    eTO.UpdatedBy = username;
                    eTO.UpdatedDate = DateTime.Now;
                    if(!string.IsNullOrWhiteSpace(data.OrderStatus))
                        eTO.OrderStatus = data.OrderStatus;
                    if (eTO.OrderStatus == "In Process" && string.IsNullOrWhiteSpace(data.OrderStatus) && data.GIDate.HasValue)
                        eTO.OrderStatus = "On Delivery";
                    if (data.GIDate.HasValue)
                    {
                        eTO.GIDate = data.GIDate.Value;
                        eTO.GIBy = username;
                        eTO.EstArrivalDate = data.GIDate.Value + new TimeSpan(eTO.LeadTime.HasValue ? eTO.LeadTime.Value : 0, 0, 0, 0);
                    }
                    if (data.GRDate.HasValue)
                    {
                        eTO.GRDate = data.GRDate.Value;
                        eTO.GRBy = username;
                        eTO.EstArrivalDate = data.GRDate.Value + new TimeSpan(eTO.LeadTime.HasValue ? eTO.LeadTime.Value : 0, 0, 0, 0);
                    }
                }

                try
                {
                    var gTO = context.TransportOrders.Where(gto => gto.IDTransportExecution == eTO.IDTransportExecution).ToList();

                    var idx = 99;
                    foreach (var itemTO in gTO)
                    {
                        idx = Math.Min(_statuses.IndexOf(itemTO.OrderStatus), idx);
                    }

                    if(idx > 0 && idx <= 99)
                    {
                        var org = context.TransportExecutions.Where(te => te.IDTransportExecution == eTO.IDTransportExecution).FirstOrDefault();
                        if (org != null)
                        {
                            org.TransportStatus = _statuses[idx];
                        }
                    }
                }
                catch(Exception e)
                {
                    return false;
                }

                context.SaveChanges();
            }

            return true;
        }

        public bool GetImportXlsUP(string tnn, TransportPositionDetailDTO input)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(q => q.TransportNo == tnn);
            var getID = _transExecutionRepo.Get(queryFilter)
                .Select(f => f.IDTransportExecution).ToList();

            if (getID.Count > 0)
            {
                input.IDTransportExecution = getID.ElementAt(0);
                var transPosition = Mapper.Map<TransportPositionDetail>(input);
                _transPositionRepo.Insert(transPosition);
                _transPositionRepo.Save();
            }

            return true;
        }
        public bool ImportXlsUP(IEnumerable<TransportMonitoringUploadInput> imported, string username)
        {
            var context = new TOMContextDB();
            
            foreach (var data in imported)
            {
                if (string.IsNullOrWhiteSpace(data.TransportationNumber)) continue;
                var eTE = context.TransportExecutions.Where(to => to.TransportNo == data.TransportationNumber).FirstOrDefault();
                if (eTE != null)
                {
                    TransportPositionDetail ent = new TransportPositionDetail();

                    ent.IDTransportExecution = eTE.IDTransportExecution;
                    ent.PositionDate = data.DateTime.Value;
                    ent.PositionName = data.UpdatePosition;
                    ent.CreatedBy = username;
                    ent.CreatedDate = DateTime.Now;
                    ent.UpdatedBy = username;
                    ent.UpdatedDate = DateTime.Now;

                    context.TransportPositionDetails.Add(ent);
                }
            }

            context.SaveChanges();

            return true;
        }

        public bool SendMail(string subject, string body, string[] recipients, string format = "HTML")
        {
            return _transportOrderRepo.SendMail(subject, body, recipients, format);
        }

        public bool UpdateDataTransOrd(IEnumerable<TransportOrderMonitoringInput> input, int TEID, string userID)
        {
            var context = new TOMContextDB();
            int idx = 99;
            var statuses = new List<string> { "In Process", "On Delivery", "Arrive At Destination and Waiting For Confirmation", "Complete" };

            var email = new Dictionary<string, List<string>>();
            email.Add("Arrive", new List<string>());
            email.Add("Delivery", new List<string>());

            foreach (var i in input)
            {
                var org = context.TransportOrders.Where(to => to.IDTransportOrder == i.ID).FirstOrDefault();
                if (org != null && org.OrderStatus != i.OrderStatus)
                {
                    var ON = org.STONo;

                    // status changed
                    if (i.OrderStatus.Contains("Arrive"))
                    {
                        // send arrive email
                        email["Arrive"].Add(ON);
                    }
                    else if (i.OrderStatus.Contains("Delivery"))
                    {
                        // send deliver email
                        email["Delivery"].Add(ON + " is in delivery" + (org.GIDate.HasValue ? " with GI " + org.GIDate.Value.ToString("dd MMMM yyyy HH:mm:ss") : "" ));
                    }
                }
                if (!UpdateDataTransOrd(i, TEID, userID))
                    return false;
                idx = Math.Min( statuses.IndexOf(i.OrderStatus), idx);
            }

            if (idx >= 0 && idx < 99)
            {
                try
                {
                    var org = context.TransportExecutions.Where(te => te.IDTransportExecution == TEID).FirstOrDefault();
                    string TN = null;
                    if (org != null)
                    {
                        org.TransportStatus = statuses[idx];
                        TN = org.TransportNo;
                    }
                    context.SaveChanges();

                    try
                    {
                        // send email here bro
                        bool needToSendMail = false;
                        string body = "Hi,<br /><br />We would like to inform you that,<br />Transportation Number " + 
                            TN + " with:<br />";
                        foreach(var arr in email["Arrive"])
                        {
                            body += "&bull; STO / Order Number " + arr + " has been arrived at destination<br/>";
                            needToSendMail = true;
                        }
                        foreach (var arr in email["Delivery"])
                        {
                            body += "&bull; STO / Order Number " + arr + "<br/>";
                            needToSendMail = true;
                        }
                        body += "<br /><br />Thank you";

                        List<string> _recipients = new List<string>();

                        // find all people in destination location
                        try
                        {
                            var TO = context.TransportOrders.Where(to=> to.IDTransportExecution == org.IDTransportExecution).FirstOrDefault();
                            if (TO != null)
                            {
                                _recipients = context.MasterUserLocationMappings.Where(ul => ul.IDLocation == TO.ReceiverIDLocation && ul.MasterUser.IsActive).Select(s => s.MasterUser.Email).ToList();
                            }
                        }
                        catch { }

                        if (needToSendMail)
                        {
                            SendMail("Transport Monitoring Notification", body, _recipients.ToArray());
                        }
                    }
                    catch(Exception ex)
                    {
                    }
                }
                catch(Exception ex)
                {
                    
                }
            }

            return true;
        }

        public bool UpdateDataTransOrd(TransportOrderMonitoringInput input, int TEID, string userID)
        {
            try
            {
                var context = new TOMContextDB();
                var org = context.TransportOrders.Where(to => to.IDTransportOrder == input.ID).FirstOrDefault();
                if (org != null)
                {
                    org.OrderStatus = input.OrderStatus;
                    if (input.GI.HasValue && org.GIDate != input.GI)
                        org.GIBy = userID;
                    org.GIDate = input.GI;
                    if (input.GR.HasValue && org.GRDate != input.GR)
                        org.GRBy = userID;
                    org.GRDate = input.GR;
                    if (input.GI !=null)
                    {
                        org.EstArrivalDate = input.GI + new TimeSpan(org.LeadTime ?? 0, 0, 0, 0);
                    }
                    org.IsMaterialReceived = input.CargoReceived == "true";
                }

                context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool UpdateDataTransOrd(Int64 id, string val)
        {
            var context = new TOMContextDB();

            var dbResult = (
                            from TO in context.TransportOrders
                            where TO.IDTransportOrder == id
                            select new { TO }
                            )
                            .ToList();

            if (dbResult.Count > 0)
            {
                foreach (var col in dbResult)
                {
                    if (val.Split(',')[1].Length > 1)
                    {
                        col.TO.GIDate = Convert.ToDateTime(val.Split(',')[1]);
                    }
                    col.TO.OrderStatus = val.Split(',')[2];
                    col.TO.IsMaterialReceived = Convert.ToBoolean(val.Split(',')[3]);
                    context.SaveChanges();
                }
            }

            return true;
        }

        public bool InsertDataTransOrdPU(TransportPositionDetailDTO input)
        {
            var transPosition = Mapper.Map<TransportPositionDetail>(input);
            _transPositionRepo.Insert(transPosition);
            _transPositionRepo.Save();

            return true;
        }

        public List<TransportPositionDetailDTO> GetPosition(int TEID)
        {
            var val = _transPositionRepo.Get(c => c.IDTransportExecution == TEID).OrderBy(c => c.PositionDate).Reverse().ToList();
            foreach (var v in val) v.TransportExecution = null;
            return Mapper.Map<List<TransportPositionDetailDTO>>(val);
        }
        public bool DeletePosition(int TPID)
        {
            var org = _transPositionRepo.Get(c => c.IDTransportPositionDetail == TPID).FirstOrDefault();
            try
            {
                if (org != null)
                {
                    _transPositionRepo.Delete(org);
                    _transPositionRepo.Save();
                    return true;
                }
            }
            catch { }
            return false;
        }
        #endregion SUB TRUCK TRAIN
    }
}
