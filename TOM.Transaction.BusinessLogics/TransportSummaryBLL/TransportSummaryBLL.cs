using AutoMapper;
using DFIS.Contracts;
using DFIS.Universal.Domain.DTOs;
using DFIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using DFIS.Universal.Domain.Outputs;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Repositories.TransportExecutionRepo;
using TOM.Master.Repositories;
using TOM.Transport.BusinessLogics.TransportExecutionBLL;
using TOM.Transport.Repositories;
using TOM.Transport.Repositories.TransportRouteRepo;
using System.Configuration;

namespace TOM.Transport.BusinessLogics.TransportSummaryBLL
{
    public class TransportSummaryBLL : ITransportSummaryBLL
    {
        private readonly IGenericRepository<MasterList> _mstListRepo;
        private readonly IGenericRepository<MasterLocation> _mstLocationRepo;
        private readonly IGenericRepository<MasterVendor> _mstVendorRepo;
        private readonly IGenericRepository<TransportExecution> _transExecutionRepo;
        private readonly IGenericRepository<TransportOrder> _transOrderRepo;
        private readonly IGenericRepository<TransportSummaryReclassView> _reclassView;
        private readonly ITransportOrderRepo _transportOrderRepo;
        private readonly ITransportExecutionRepo _transportExecutionRepo;
        private readonly ITransportRouteRepo _transportRouteRepo;
        private readonly IMasterVendorTOMRepo _masterVendorRepo;
        private readonly IMasterConfigurationRepo _masterConfigurationRepo;
        private readonly ITransportExecutionBLL _transportExecutionBll;
        private readonly IMasterLocationRepo _masterLocationRepo;

        private readonly DateTime vendorDate = new DateTime(2020, 1, 31);

        public TransportSummaryBLL(IGenericRepository<MasterList> mstListRepo, IGenericRepository<MasterLocation> mstLocationRepo, IGenericRepository<MasterVendor> mstVendorRepo, IGenericRepository<TransportExecution> transExecutionRepo, IGenericRepository<TransportOrder> transOrderRepo,ITransportOrderRepo transportOrderRepo, 
            ITransportRouteRepo transportRouteRepo, ITransportExecutionRepo transportExecutionRepo, IGenericRepository<TransportSummaryReclassView> reclassView,
            IMasterVendorTOMRepo masterVendorRepo, IMasterConfigurationRepo masterConfigurationRepo, ITransportExecutionBLL transportExecutionBll, IMasterLocationRepo masterLocationRepo)
        {
            _mstListRepo = mstListRepo;
            _mstLocationRepo = mstLocationRepo;
            _mstVendorRepo = mstVendorRepo;
            _transExecutionRepo = transExecutionRepo;
            _transOrderRepo = transOrderRepo;
            _transportOrderRepo = transportOrderRepo;
            _transportExecutionRepo = transportExecutionRepo;
            _transportRouteRepo = transportRouteRepo;
            _reclassView = reclassView;
            _masterVendorRepo = masterVendorRepo;
            _masterConfigurationRepo = masterConfigurationRepo;
            _transportExecutionBll = transportExecutionBll;
            _masterLocationRepo = masterLocationRepo;
        }

        public List<TransportRouteDTO> GetTransportRoute(string stono)
        {
            // var queryFilter = PredicateHelper.True<TransportExecution>();
            // queryFilter = queryFilter.And(x => x.TransportNo == stono);

            var trexec = _transExecutionRepo.Get(x => x.TransportNo == stono).First();

            var dbResult = Mapper.Map<List<TransportRoute>, List<TransportRouteDTO>>(
                _transportRouteRepo.GetTransportRouteByIDTransactionExecution(trexec.IDTransportExecution));
            
            return dbResult;
        }

        public List<TransportExecutionDTO> GetTransportList()
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(q => q.IsActive == true);
            var dbResult = _transExecutionRepo.Get(queryFilter).OrderBy(o => o.TransportNo).Select(f => new TransportExecutionDTO { TransportNo = f.TransportNo }).ToList();
            return dbResult;
        }

        public List<TransportOrderDTO> GetSTONo()
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(q => q.IsActive == true);
            var dbResult = _transOrderRepo.Get(queryFilter).OrderBy(o => o.STONo).Select(f => new TransportOrderDTO { STONo = f.STONo }).ToList();
            return dbResult;
        }

        public List<MasterListDTO> GetMstList()
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(q => q.IsActive == true);
            var dbResult = _mstListRepo.Get(queryFilter).OrderBy(o => o.FieldValue).Select(f => new MasterListDTO { FieldValue = f.FieldValue, FieldName = f.FieldName }).ToList();
            return dbResult;
        }

        public List<MasterVendorDTO> GetVendorName()
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(q => q.IsActive == true && q.ParentVendor == null);
            var dbResult = _mstVendorRepo.Get(queryFilter).OrderBy(o => o.VendorName).Select(f => new MasterVendorDTO { IDVendor = f.IDVendor.ToString(), VendorName = f.VendorName }).ToList();
            return dbResult;
        }

        public List<MasterLocationDTO> GetLocationName()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(q => q.IsActive == true && q.IsAssigned == true);
            queryFilter = queryFilter.And(q => q.Type == "Warehouse" || q.Type == "Factory" || q.Type == "Agent" || q.Type == "Other");
            var dbResult = _mstLocationRepo.Get(queryFilter).OrderBy(o => o.LocationName).Select(f => new MasterLocationDTO { IDLocation = f.IDLocation, LocationName = f.LocationName }).ToList();
            return dbResult;
        }

        public List<MasterListDTO> GetListTabName()
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(q => q.FieldName == "TransportationSummary" && q.IsActive);
            var dbResult = _mstListRepo.Get(queryFilter).OrderBy(o => o.IDList).Select(f => new MasterListDTO { FieldValue = f.FieldValue }).ToList();
            return dbResult;
        }

        public List<MasterVendorTOMDTO> GetVendorSuggestionList(List<UserRole> userRole)
        {
            if (userRole[0].RoleName == "SUPER ADMIN")
            {
                return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorActiveNoChild());
            }
            else
            {
                List<MasterConfigurationDTO> tempConfig = Mapper.Map<List<MasterConfigurationDTO>>(_masterConfigurationRepo.GetListMasterConfigurationByPageNameDescription(EnumHelper.GetDescription(Enums.MasterConfigurationValue.RoleVendorMapping), userRole[0].RoleName));
                return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorByListVendorName(tempConfig.Select(x => x.Value).ToList()));
            }
        }

        public List<TransportOrderDTO> GetSTONoFilter(string stono)
        {
            return Mapper.Map<List<TransportOrder>, List<TransportOrderDTO>>(_transportOrderRepo.GetSTONoFilter(stono));
        }


        public List<TransportExecutionDTO> GetTransportationNumberFilter(string transno)
        {
            return Mapper.Map<IEnumerable<TransportExecution>, List<TransportExecutionDTO>>(_transportExecutionRepo.GetTransportExecutionFilter(transno));
        }

        private List<UserRole> GetListUserRole()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listrole = currentSession.Role.ToList();
            return listrole;
        }

        public List<TransportExecutionDTOMin> GetListView(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole)
        {
            DateTime dateNow = new DateTime();
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            List<String>  filterVendor = new List<String> ();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                filterVendor = GetVendorSuggestionList(userRole).Select(x => x.VendorName).ToList();
            }
            
            
            string TransportStatusClose = EnumHelper.GetDescription(Enums.TransportationStatus.Close);
            var firstDayOfMonth = new DateTime(dateNow.Year, dateNow.Month, 1);
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN
                        from TO in LJOIN.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor
                        //join MCF in context.MasterConfigurations on MV.VendorName equals MCF.Value into LJOIN1
                        //from MCF in LJOIN1.DefaultIfEmpty()
                        //join MC in context.MasterCosts on TE.IDCost equals MC.IDCost into RJOIN_3
                        //from MC in RJOIN_3.DefaultIfEmpty()
                        //join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation into RJOIN_4
                        //from MLSE in RJOIN_4.DefaultIfEmpty()
                        //join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation into RJOIN_5
                        //from MLRE in RJOIN_5.DefaultIfEmpty()
                        //join MLSL in context.MasterLocations on TE.StartLocation equals MLSL.IDLocation into RJOIN_6
                        //from MLSL in RJOIN_6.DefaultIfEmpty()
                        //where ((TE.TransportStatus == TransportStatusClose && TE.TransportDate < firstDayOfMonth) || (TE.TransportDate < firstDayOfMonth)) && TE.ServiceGRNo == null && TE.IsActive == true && TO.IsActive == true
                        where TE.IsActive && !TE.TransportNo.Contains("E-") && (TO == null || (TO != null && TO.IsActive == true))
                        select new TransportExecutionDTOMin
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            //IDTransportOrder = TO.IDTransportOrder,
                            TransportDate = TE.TransportDate,
                            TransportNo = TE.TransportNo,
                            STONo = TO.STONo,
                            //TransportCategory = TE.TransportCategory,
                            TransportMode = TE.TransportMode,
                            IDVendor = TE.IDVendor,
                            VendorName = MV.VendorName,
                            ActualVehicleType = TE.ActualVehicleType,
                            OrderCategory = TO.OrderCategory,
                            ZoneBased = TO.ZoneBased,
                            IDStartLocation = TE.StartLocation,
                            //StartLocation = MLSL.LocationName,
                            IDSender = TO.ActualSenderIDLocation,
                            //Sender = MLSE.LocationName,
                            IDReceiver = TO.ActualReceiverIDLocation,
                            //Receiver = MLRE.LocationName,
                            SIWeek = TE.SIWeek,
                            TransportStatus = TE.TransportStatus,
                            IsActive = TE.IsActive,
                            KMBased = TE.TotalKMBased > 0 ? TE.TotalKMBased : 0,
                            BasedPrice = TE.BasedCost > 0 ? TE.BasedCost : 0,
                            //CargoType = "",
                            //MapFrom = MCF.Description
                        });

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] ftc = fltrtc.Split(',').ToArray();
            string[] ftm = fltrtm.Split(',').ToArray();
            string[] fvn = fltrvn.Split(',').ToArray();
            string[] fvt = fltrvt.Split(',').ToArray();
            string[] foc = fltroc.Split(',').ToArray();
            string[] fzo = fltrzo.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();

            int? fth = null;
            if(!string.IsNullOrEmpty(fltrth))
                fth = int.Parse(fltrth);
            
            if (fltrdf.Length > 1)
            {
                var splitdf = fltrdf.Split('/');
                DateTime fdf = new DateTime(Int32.Parse(splitdf[2]), Int32.Parse(splitdf[1]), Int32.Parse(splitdf[0]));
                dbResult = dbResult.Where(q => q.TransportDate >= fdf);
            }
            if (fltrdt.Length > 1)
            {
                var splitdt = fltrdt.Split('/');
                DateTime fdt = new DateTime(Int32.Parse(splitdt[2]), Int32.Parse(splitdt[1]), Int32.Parse(splitdt[0]));
                dbResult = dbResult.Where(q => q.TransportDate <= fdt);
            }
            if (fltrtn.Length > 0) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltron.Length > 0) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltrtc.Length > 0) { dbResult = dbResult.Where(q => ftc.Contains(q.TransportCategory)); }
            if (fltrtm.Length > 0) { dbResult = dbResult.Where(q => ftm.Contains(q.TransportMode)); }
            if (fltrvn.Length > 0) { dbResult = dbResult.Where(q => fvn.Contains(q.IDVendor.ToString())); }
            if (fltrvt.Length > 0) { dbResult = dbResult.Where(q => fvt.Contains(q.ActualVehicleType)); }
            if (fltroc.Length > 0) { dbResult = dbResult.Where(q => foc.Contains(q.OrderCategory)); }
            if (fltrzo.Length > 0) { dbResult = dbResult.Where(q => fzo.Contains(q.ZoneBased)); }
            if (fltrsl.Length > 0) { dbResult = dbResult.Where(q => fsl.Contains(q.IDStartLocation)); }
            if (fltrse.Length > 0) { dbResult = dbResult.Where(q => fse.Contains(q.IDSender)); }
            if (fltrre.Length > 0) { dbResult = dbResult.Where(q => fre.Contains(q.IDReceiver)); }

            if (fth != null)
                dbResult = dbResult.Where(q => q.TransportDate.Year == fth);
            if (fltrro.Length > 1 && fltrro != "ALL")
            {
                // dbResult = dbResult.Where(q => q.MapFrom == "SUMMARY " + fltrro);
                dbResult = from db in dbResult
                           join MCF in context.MasterConfigurations on db.VendorName equals MCF.Value
                           where MCF.Description == fltrro && MCF.PageName == "RoleVendorMapping"
                           select db;
            }
            
            var result = dbResult.ToList();

            List<int> distinct = (from dis in result
                            select  dis.IDTransportExecution).Distinct().ToList();

            var temp = result.Where(x => distinct.Contains(x.IDTransportExecution)).GroupBy(x => x.IDTransportExecution).Select(x => x.FirstOrDefault()).ToList();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                temp = temp.Where(x => filterVendor.Contains(x.VendorName)).OrderBy(o => o.IDTransportExecution).ToList();
            }
            else {
                temp = temp.Where(x => UserRole.ToLower().Contains(x.TransportMode.ToLower()) && x.TransportDate > vendorDate).OrderBy(o => o.IDTransportExecution).ToList();
            }
            return temp;
        }

        public List<TransportExecutionDTO> GetExportXlsDetailClass(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole)
        {
            var context = new TOMContextDB();
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            List<String> filterVendor = new List<String>();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                filterVendor = GetVendorSuggestionList(userRole).Select(x => x.VendorName).ToList();
            }

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN1
                        from TO in LJOIN1.DefaultIfEmpty()
                        join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder into LJOIN2
                        from TOD in LJOIN2.DefaultIfEmpty()
                        join MF in context.MasterFABrands on TOD.Code equals MF.FACode into RJOIN_1
                        from MF in RJOIN_1.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_2
                        from MV in RJOIN_2.DefaultIfEmpty()
                        //join MC in context.MasterCosts on TE.IDCost equals MC.IDCost into RJOIN_3
                        //from MC in RJOIN_3.DefaultIfEmpty()
                        join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation into RJOIN_4
                        from MLSE in RJOIN_4.DefaultIfEmpty()
                        join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation into RJOIN_5
                        from MLRE in RJOIN_5.DefaultIfEmpty()
                        join MLSL in context.MasterLocations on TE.StartLocation equals MLSL.IDLocation into RJOIN_6
                        from MLSL in RJOIN_6.DefaultIfEmpty()
                        join MLFL in context.MasterLocations on TE.FinishLocation equals MLFL.IDLocation into RJOIN_7
                        from MLFL in RJOIN_7.DefaultIfEmpty()
                        join MCCC in context.MasterCostCenters on TE.IDCostCenter equals MCCC.IDCostCenter into LJOIN_3
                        from MCCC in LJOIN_3.DefaultIfEmpty()
                        where TE.IsActive && !TE.TransportNo.Contains("E-") && (TO == null || (TO != null && TO.IsActive == true)) && (TOD == null || (TOD != null && TOD.IsActive == true))
                        select new TransportExecutionDTO
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            IDTransportOrder = TO.IDTransportOrder,
                            IDTransportOrderDetail = TOD.IDTransportOrderDetail,

                            Trip = "",

                            TransportNo = TE.TransportNo,
                            TransportDate = TE.TransportDate,
                            TransportCategory = TE.TransportCategory,
                            SIWeek = TE.SIWeek ?? 0,
                            Month = TE.Month ?? 0,
                            TransportMode = TE.TransportMode,
                            TransportStatus = TE.TransportStatus,
                            ActualVehicleType = TE.ActualVehicleType,
                            PoliceRegNo = TE.PoliceRegNo,

                            ServicePONo = TE.ServicePONo,
                            SerivceGRNo = TE.ServiceGRNo,
                            ActualCostCenter = TE.ActualCostCenter ?? "",

                            STONo = TO.STONo,
                            ZoneBased = TO.ZoneBased,
                            IDStartLocation = TE.StartLocation,
                            IDSender = TO.ActualSenderIDLocation,
                            IDReceiver = TO.ActualReceiverIDLocation,
                            OrderType = TO.OrderType,
                            GRDate = TO.GRDate,
                            GIDate = TO.GIDate,

                            MaterialType = TOD.MaterialType,
                            Description = TOD.Description,
                            UoM = TOD.UoM,

                            Sender = MLSE.LocationName,
                            Receiver = MLRE.LocationName,
                            StartLocation = MLSL.LocationName,
                            FinishLocation = MLFL.LocationName,

                            StickPerBox = MF.StickPerBox,
                            PackPerBox = MF.PackPerBox,
                            StickPerPack = MF.StickPerPack,
                            IDVendor = TE.IDVendor,
                            VendorName = MV.VendorName,

                            Route = "",

                            KMBased = TE.TotalKMBased ?? 0,
                            KMRail = TE.TotalKMRail ?? 0,
                            KMSea = TE.TotalKMSea ?? 0,
                            TotalKM = TE.TotalKM ?? 0,
                            ASDPCost = TE.ASDPCost ?? 0,
                            SPSICost = TE.SPSICost ?? 0,
                            AdditionalCost = TE.AdditionalCost ?? 0,
                            CFPLiter = TE.CFPLiter ?? 0,
                            CFPTKM = TE.CFPTKM ?? 0,
                            KGCO2 = TE.KGCO2 ?? 0,
                            AVGLoadFactor = TE.AVGLoadFactor ?? 0,
                            KMOrder = TO.KM ?? 0,
                            Qty = TOD.Qty ?? 0,
                            BasedPrice = TE.BasedCost ?? 0,
                            DiscountPrice = TE.DiscountCost ?? 0,
                            TotalCost = TE.TotalCost ?? 0,

                            MappingCC = MCCC.Description ?? "",
                            Account = MCCC.Account ?? ""
                        }
                        );
            var a = dbResult;

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] ftc = fltrtc.Split(',').ToArray();
            string[] ftm = fltrtm.Split(',').ToArray();
            string[] fvn = fltrvn.Split(',').ToArray();
            string[] fvt = fltrvt.Split(',').ToArray();
            string[] foc = fltroc.Split(',').ToArray();
            string[] fzo = fltrzo.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();

            int? fth = null;
            if(!string.IsNullOrEmpty(fltrth))
                fth = int.Parse(fltrth);

            if (fltrdf.Length > 1)
            {
                var splitdf = fltrdf.Split('/');
                DateTime fdf = new DateTime(Int32.Parse(splitdf[2]), Int32.Parse(splitdf[1]), Int32.Parse(splitdf[0]));
                dbResult = dbResult.Where(q => q.TransportDate >= fdf);
            }
            if (fltrdt.Length > 1)
            {
                var splitdt = fltrdt.Split('/');
                DateTime fdt = new DateTime(Int32.Parse(splitdt[2]), Int32.Parse(splitdt[1]), Int32.Parse(splitdt[0]));
                dbResult = dbResult.Where(q => q.TransportDate <= fdt);
            }
            if (fltrtn.Length > 0) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltron.Length > 0) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltrtc.Length > 0) { dbResult = dbResult.Where(q => ftc.Contains(q.TransportCategory)); }
            if (fltrtm.Length > 0) { dbResult = dbResult.Where(q => ftm.Contains(q.TransportMode)); }
            if (fltrvn.Length > 0) { dbResult = dbResult.Where(q => fvn.Contains(q.IDVendor.ToString())); }
            if (fltrvt.Length > 0) { dbResult = dbResult.Where(q => fvt.Contains(q.ActualVehicleType)); }
            if (fltroc.Length > 0) { dbResult = dbResult.Where(q => foc.Contains(q.OrderCategory)); }
            if (fltrzo.Length > 0) { dbResult = dbResult.Where(q => fzo.Contains(q.ZoneBased)); }
            if (fltrsl.Length > 0) { dbResult = dbResult.Where(q => fsl.Contains(q.IDStartLocation)); }
            if (fltrse.Length > 0) { dbResult = dbResult.Where(q => fse.Contains(q.IDSender)); }
            if (fltrre.Length > 0) { dbResult = dbResult.Where(q => fre.Contains(q.IDReceiver)); }

            if(fth != null)
                dbResult = dbResult.Where(q => q.TransportDate.Year == fth);
            if (fltrro.Length > 1 && fltrro != "ALL")
            {
                // dbResult = dbResult.Where(q => q.MapFrom == "SUMMARY " + fltrro);
                dbResult = from db in dbResult
                           join MCF in context.MasterConfigurations on db.VendorName equals MCF.Value
                           where MCF.Description == fltrro && MCF.PageName == "RoleVendorMapping"
                           select db;
            }
            //int n = fltron.Split(',').Count();

            //if (!String.IsNullOrEmpty(fltron))
            //{
            //    if (n > 1)
            //    {
            //        string[] te = fltron.Split(',').ToArray();

            //        dbResult = dbResult.Where(q => te.Contains(q.IDTransportExecution.ToString()));
            //    }
            //    else
            //    {
            //        dbResult = dbResult.Where(q => q.IDTransportExecution.ToString() == fltron);
            //    }
            //}
            //else
            //{
            //    dbResult = dbResult.Where(q => q.IDTransportExecution == 0);
            //}
            var result = dbResult.ToList();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                result = result.Where(x => filterVendor.Contains(x.VendorName))
                .OrderBy(o => o.TransportDate)
                .ThenBy(o => o.TransportNo)
                .ThenBy(o => o.IDTransportOrder)
                .ThenBy(o => o.IDTransportOrderDetail)
                .ToList();
            }
            else {
                result = result.Where(x => UserRole.ToLower().Contains(x.TransportMode.ToLower()) && x.TransportDate > vendorDate)
                .OrderBy(o => o.TransportDate)
                .ThenBy(o => o.TransportNo)
                .ThenBy(o => o.IDTransportOrder)
                .ThenBy(o => o.IDTransportOrderDetail)
                .ToList();
            }

            //foreach(int idexe in result.GroupBy(x => x.IDTransportExecution).Select(grp => grp.First()).Select(x => x.IDTransportExecution))
            //{
            //    string tempStringRoute = "";
            //    List<TransportRoute> tempListRoute = _transportRouteRepo.GetTransportRouteByIDTransactionExecution(idexe);
            //    foreach (TransportRoute tempRoute in tempListRoute)
            //    {
            //        MasterLocation locName = _masterLocationRepo.GetMasterLocationByID(tempRoute.IDLocation);
            //        if (!tempStringRoute.Contains(locName.LocationName))
            //            tempStringRoute = tempStringRoute + ", " + locName.LocationName;

            //    }
            //    result.Select(x => { x.Route = tempStringRoute; return x; }).ToList();
            //}

            return result;
        }

        public List<TransportExecutionDTO> GetExportXlsCompactValidation(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole)
        {
            var context = new TOMContextDB();
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            List<String> filterVendor = new List<String>();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                filterVendor = GetVendorSuggestionList(userRole).Select(x => x.VendorName).ToList();
            }            

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN1
                        from TO in LJOIN1.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_2
                        from MV in RJOIN_2.DefaultIfEmpty()
                        //join MC in context.MasterCosts on TE.IDCost equals MC.IDCost into RJOIN_3
                        //from MC in RJOIN_3.DefaultIfEmpty()
                        join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation into RJOIN_4
                        from MLSE in RJOIN_4.DefaultIfEmpty()
                        join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation into RJOIN_5
                        from MLRE in RJOIN_5.DefaultIfEmpty()
                        join MLSL in context.MasterLocations on TE.StartLocation equals MLSL.IDLocation into RJOIN_6
                        from MLSL in RJOIN_6.DefaultIfEmpty()
                        join MLFL in context.MasterLocations on TE.FinishLocation equals MLFL.IDLocation into RJOIN_7
                        from MLFL in RJOIN_7.DefaultIfEmpty()
                        join TR in context.TransportRoutes on TE.IDTransportExecution equals TR.IDTransportExecution into LJOIN_2
                        from TR in LJOIN_2.DefaultIfEmpty()
                        join MLTR in context.MasterLocations on TR.IDLocation equals MLTR.IDLocation into RJOIN_8
                        from MLTR in RJOIN_8.DefaultIfEmpty()
                        where TE.IsActive && !TE.TransportNo.Contains("E-") && (TO == null || (TO != null && TO.IsActive == true))
                        select new TransportExecutionDTO
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            IDTransportOrder = TO.IDTransportOrder,
                            IDRoute = TR.IDTransportRoute,
                            IDVendor = TE.IDVendor,

                            Trip = "",

                            TransportNo = TE.TransportNo,
                            TransportDate = TE.TransportDate,
                            TransportCategory = TE.TransportCategory,
                            SIWeek = TE.SIWeek ?? 0,
                            Month = TE.Month ?? 0,
                            TransportMode = TE.TransportMode,
                            TransportStatus = TE.TransportStatus,
                            ActualVehicleType = TE.ActualVehicleType,
                            PoliceRegNo = TE.PoliceRegNo,
                            IDStartLocation = TE.StartLocation,
                            StartLocation = MLSL.LocationName,
                            FinishLocation = MLFL.LocationName,
                            ServicePONo = TE.ServicePONo ?? "",
                            SerivceGRNo = TE.ServiceGRNo ?? "",
                            ActualCostCenter = TE.ActualCostCenter ?? "",

                            STONo = TO.STONo,
                            ZoneBased = TO.ZoneBased,
                            IDSender = TO.ActualSenderIDLocation,
                            IDReceiver = TO.ActualReceiverIDLocation,
                            OrderType = TO.OrderType,
                            GRDate = TO.GRDate,
                            GIDate = TO.GIDate,
                            Remarks = "",

                            Sender = MLSE.LocationName,
                            Receiver = MLRE.LocationName,
                            VendorName = MV.VendorName,

                            Route = MLTR.LocationName,

                            KMBased = TE.TotalKMBased ?? 0,
                            KMRail = TE.TotalKMRail ?? 0,
                            KMSea = TE.TotalKMSea ?? 0,
                            TotalKM = TE.TotalKM ?? 0,
                            ASDPCost = TE.ASDPCost ?? 0,
                            SPSICost = TE.SPSICost ?? 0,
                            AdditionalCost = TE.AdditionalCost ?? 0,
                            CFPLiter = TE.CFPLiter ?? 0,
                            CFPTKM = TE.CFPTKM ?? 0,
                            KGCO2 = TE.KGCO2 ?? 0,
                            AVGLoadFactor = TE.AVGLoadFactor ?? 0,
                            KMOrder = TO.KM ?? 0,
                            BasedPrice = TE.BasedCost ?? 0,
                            DiscountPrice = TE.DiscountCost ?? 0,
                            TotalCost = TE.TotalCost ?? 0,
                        });

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] ftc = fltrtc.Split(',').ToArray();
            string[] ftm = fltrtm.Split(',').ToArray();
            string[] fvn = fltrvn.Split(',').ToArray();
            string[] fvt = fltrvt.Split(',').ToArray();
            string[] foc = fltroc.Split(',').ToArray();
            string[] fzo = fltrzo.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();

            int? fth = null;
            if (!string.IsNullOrEmpty(fltrth))
                fth = int.Parse(fltrth);

            if (fltrdf.Length > 1)
            {
                var splitdf = fltrdf.Split('/');
                DateTime fdf = new DateTime(Int32.Parse(splitdf[2]), Int32.Parse(splitdf[1]), Int32.Parse(splitdf[0]));
                dbResult = dbResult.Where(q => q.TransportDate >= fdf);
            }
            if (fltrdt.Length > 1)
            {
                var splitdt = fltrdt.Split('/');
                DateTime fdt = new DateTime(Int32.Parse(splitdt[2]), Int32.Parse(splitdt[1]), Int32.Parse(splitdt[0]));
                dbResult = dbResult.Where(q => q.TransportDate <= fdt);
            }
            if (fltrtn.Length > 0) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltron.Length > 0) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltrtc.Length > 0) { dbResult = dbResult.Where(q => ftc.Contains(q.TransportCategory)); }
            if (fltrtm.Length > 0) { dbResult = dbResult.Where(q => ftm.Contains(q.TransportMode)); }
            if (fltrvn.Length > 0) { dbResult = dbResult.Where(q => fvn.Contains(q.IDVendor.ToString())); }
            if (fltrvt.Length > 0) { dbResult = dbResult.Where(q => fvt.Contains(q.ActualVehicleType)); }
            if (fltroc.Length > 0) { dbResult = dbResult.Where(q => foc.Contains(q.OrderCategory)); }
            if (fltrzo.Length > 0) { dbResult = dbResult.Where(q => fzo.Contains(q.ZoneBased)); }
            if (fltrsl.Length > 0) { dbResult = dbResult.Where(q => fsl.Contains(q.IDStartLocation)); }
            if (fltrse.Length > 0) { dbResult = dbResult.Where(q => fse.Contains(q.IDSender)); }
            if (fltrre.Length > 0) { dbResult = dbResult.Where(q => fre.Contains(q.IDReceiver)); }

            if(fth != null)
                dbResult = dbResult.Where(q => q.TransportDate.Year == fth);

            if (fltrro.Length > 1 && fltrro != "ALL")
            {
                // dbResult = dbResult.Where(q => q.MapFrom == "SUMMARY " + fltrro);
                dbResult = from db in dbResult
                           join MCF in context.MasterConfigurations on db.VendorName equals MCF.Value
                           where MCF.Description == fltrro && MCF.PageName == "RoleVendorMapping"
                           select db;
            }
            //int n = fltron.Split(',').Count();

            //if (!String.IsNullOrEmpty(fltron))
            //{
            //    if (n > 1)
            //    {
            //        string[] te = fltron.Split(',').ToArray();

            //        dbResult = dbResult.Where(q => te.Contains(q.IDTransportExecution.ToString()));
            //    }
            //    else
            //    {
            //        dbResult = dbResult.Where(q => q.IDTransportExecution.ToString() == fltron);
            //    }
            //}
            //else
            //{
            //    dbResult = dbResult.Where(q => q.IDTransportExecution == 0);
            //}
            var result = dbResult.ToList();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                result = result.Where(x => filterVendor.Contains(x.VendorName))
                .OrderBy(o => o.IDTransportExecution)
                .OrderBy(o => o.IDRoute)
                .OrderBy(o => o.IDTransportOrder)
                .ToList();
            }
            else
            {
                result = result.Where(x => UserRole.ToLower().Contains(x.TransportMode.ToLower()) && x.TransportDate > vendorDate)
                .OrderBy(o => o.IDTransportExecution)
                .OrderBy(o => o.IDRoute)
                .OrderBy(o => o.IDTransportOrder)
                .ToList();
            }

            return result;
        }

        public List<TransportExecutionDTO> GetCustomXls(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole)
        {
            var context = new TOMContextDB();
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            List<String> filterVendor = new List<String>();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                filterVendor = GetVendorSuggestionList(userRole).Select(x => x.VendorName).ToList();
            }

            var dbResult = (
                        from TE in context.TransportExecutions
                        //join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_2
                        from MV in RJOIN_2.DefaultIfEmpty()
                        join MC in context.MasterCosts on TE.IDCost equals MC.IDCost into RJOIN_3
                        from MC in RJOIN_3.DefaultIfEmpty()
                        //join MLSE in context.MasterLocations on TO.SenderIDLocation equals MLSE.IDLocation into RJOIN_4
                        //from MLSE in RJOIN_4.DefaultIfEmpty()
                        //join MLRE in context.MasterLocations on TO.ReceiverIDLocation equals MLRE.IDLocation into RJOIN_5
                        //from MLRE in RJOIN_5.DefaultIfEmpty()
                        join MLSL in context.MasterLocations on TE.StartLocation equals MLSL.IDLocation into RJOIN_6
                        from MLSL in RJOIN_6.DefaultIfEmpty()
                        where TE.IsActive && !TE.TransportNo.Contains("E-")
                        select new TransportExecutionDTO
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            //IDTransportOrder = TO.IDTransportOrder,
                            TransportDate = TE.TransportDate,
                            TransportNo = TE.TransportNo,
                            //STONo = TO.STONo,
                            TransportCategory = TE.TransportCategory,
                            TransportMode = TE.TransportMode,
                            IDVendor = TE.IDVendor,
                            VendorName = MV.VendorName,
                            ActualVehicleType = TE.ActualVehicleType,
                            //OrderCategory = TO.OrderCategory,
                            //ZoneBased = TO.ZoneBased,
                            IDStartLocation = TE.StartLocation,
                            StartLocation = MLSL.LocationName,
                            //IDSender = TO.SenderIDLocation,
                            //Sender = MLSE.LocationName,
                            //IDReceiver = TO.ReceiverIDLocation,
                            //Receiver = MLRE.LocationName,
                            TransportStatus = TE.TransportStatus,
                            IsActive = TE.IsActive,
                            KMBased = TE.TotalKMBased ?? 0,
                            BasedPrice = TE.BasedCost ?? 0,
                            CargoType = ""
                        }
                        );

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] ftc = fltrtc.Split(',').ToArray();
            string[] ftm = fltrtm.Split(',').ToArray();
            string[] fvn = fltrvn.Split(',').ToArray();
            string[] fvt = fltrvt.Split(',').ToArray();
            string[] foc = fltroc.Split(',').ToArray();
            string[] fzo = fltrzo.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();

            int? fth = null;
            if (!string.IsNullOrEmpty(fltrth))
                fth = int.Parse(fltrth);

            if (fltrdf.Length > 1)
            {
                var splitdf = fltrdf.Split('/');
                DateTime fdf = new DateTime(Int32.Parse(splitdf[2]), Int32.Parse(splitdf[1]), Int32.Parse(splitdf[0]));
                dbResult = dbResult.Where(q => q.TransportDate >= fdf);
            }
            if (fltrdt.Length > 1)
            {
                var splitdt = fltrdt.Split('/');
                DateTime fdt = new DateTime(Int32.Parse(splitdt[2]), Int32.Parse(splitdt[1]), Int32.Parse(splitdt[0]));
                dbResult = dbResult.Where(q => q.TransportDate <= fdt);
            }
            if (fltrtn.Length > 1) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltron.Length > 1) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltrtc.Length > 1) { dbResult = dbResult.Where(q => ftc.Contains(q.TransportCategory)); }
            if (fltrtm.Length > 1) { dbResult = dbResult.Where(q => ftm.Contains(q.TransportMode)); }
            if (fltrvn.Length > 1) { dbResult = dbResult.Where(q => fvn.Contains(q.IDVendor.ToString())); }
            if (fltrvt.Length > 1) { dbResult = dbResult.Where(q => fvt.Contains(q.ActualVehicleType)); }
            if (fltroc.Length > 1) { dbResult = dbResult.Where(q => foc.Contains(q.OrderCategory)); }
            if (fltrzo.Length > 1) { dbResult = dbResult.Where(q => fzo.Contains(q.ZoneBased)); }
            if (fltrsl.Length > 1) { dbResult = dbResult.Where(q => fsl.Contains(q.IDStartLocation)); }
            if (fltrse.Length > 1) { dbResult = dbResult.Where(q => fse.Contains(q.IDSender)); }
            if (fltrre.Length > 1) { dbResult = dbResult.Where(q => fre.Contains(q.IDReceiver)); }

            if(fth != null)
                dbResult = dbResult.Where(q => q.TransportDate.Year == fth);

            if (fltrro.Length > 1 && fltrro != "ALL") { dbResult = dbResult.Where(q => q.MapFrom == "SUMMARY " + fltrro); }
            //int n = fltron.Split(',').Count();
            //Int64 te, to; string nte = "", nto = "";

            //if (!String.IsNullOrEmpty(fltron))
            //{
            //    if (n > 1)
            //    {
            //        string[] v = fltron.Split(',').ToArray();

            //        for (int i = 0; i < n; i++)
            //        {
            //            nte = nte + v[i].Split('R')[0] + ',';
            //            //nto = nto + v[i].Split('R')[1] + ',';
            //        }

            //        string[] vte = nte.Split(',').ToArray();
            //        string[] vto = nto.Split(',').ToArray();

            //        dbResult = dbResult.Where(q => vte.Contains(q.IDTransportExecution.ToString()));
            //        //dbResult = dbResult.Where(q => vto.Contains(q.IDTransportOrder.ToString()));
            //    }
            //    else
            //    {
            //        te = Int64.Parse(fltron.Split('R')[0]);
            //        //to = Int64.Parse(fltron.Split('R')[1]);

            //        dbResult = dbResult.Where(q => q.IDTransportExecution == te);
            //        //dbResult = dbResult.Where(q => q.IDTransportOrder == to);
            //    }
            //}
            //else
            //{
            //    dbResult = dbResult.Where(q => q.IDTransportExecution == 0);
            //    //dbResult = dbResult.Where(q => q.IDTransportOrder == 0);
            //}
            var result = dbResult.ToList();
            if (!UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                result = result.Where(x => filterVendor.Contains(x.VendorName))
                .OrderBy(o => o.IDTransportExecution).ToList();
            }
            else
            {
                result = result.Where(x => UserRole.ToLower().Contains(x.TransportMode.ToLower()) && x.TransportDate > vendorDate)
                .OrderBy(o => o.IDTransportExecution).ToList();
            }

            return result;
        }

        public List<TransportExecutionDTO> GetImportValidateXls(string fltrtn, string fltron)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN1
                        from TO in LJOIN1.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_2
                        from MV in RJOIN_2.DefaultIfEmpty()
                        join MC in context.MasterCosts on TE.IDCost equals MC.IDCost into RJOIN_3
                        from MC in RJOIN_3.DefaultIfEmpty()
                        join MLSE in context.MasterLocations on TO.SenderIDLocation equals MLSE.IDLocation into RJOIN_4
                        from MLSE in RJOIN_4.DefaultIfEmpty()
                        join MLRE in context.MasterLocations on TO.ReceiverIDLocation equals MLRE.IDLocation into RJOIN_5
                        from MLRE in RJOIN_5.DefaultIfEmpty()
                        join MLSL in context.MasterLocations on TE.StartLocation equals MLSL.IDLocation into RJOIN_6
                        from MLSL in RJOIN_6.DefaultIfEmpty()
                        join MLFL in context.MasterLocations on TE.FinishLocation equals MLFL.IDLocation into RJOIN_7
                        from MLFL in RJOIN_7.DefaultIfEmpty()
                        join TR in context.TransportRoutes on TE.IDTransportExecution equals TR.IDTransportExecution into LJOIN_2
                        from TR in LJOIN_2.DefaultIfEmpty()
                        join MLTR in context.MasterLocations on TR.IDLocation equals MLTR.IDLocation into RJOIN_8
                        from MLTR in RJOIN_8.DefaultIfEmpty()
                        where TE.TransportStatus == "Close" && TE.TransportNo == fltrtn && TO.STONo == fltron
                        select new TransportExecutionDTO
                        {
                            IDTransportExecution = TE.IDTransportExecution,
                            IDTransportOrder = TO.IDTransportOrder,
                            IDRoute = TR.IDTransportRoute,
                            TransportNo = TE.TransportNo,
                            STONo = TO.STONo,
                            TransportDate = TE.TransportDate,
                            SIWeek = TE.SIWeek ?? 0,
                            Month = TE.Month ?? 0,
                            Sender = MLSE.LocationName,
                            Receiver = MLRE.LocationName,
                            Route = MLTR.LocationName,
                            VendorName = MV.VendorName,
                            VehicleType = TE.ActualVehicleType,
                            PoliceRegNo = TE.PoliceRegNo,
                            KMBased = TE.TotalKMBased ?? 0,
                            BasedPrice = MC.BasedPrice,
                            DiscountPrice = MC.DiscountPrice ?? 0,
                            ASDPCost = TE.ASDPCost ?? 0,
                            SPSICost = TE.SPSICost ?? 0,
                            AdditionalCost = TE.AdditionalCost ?? 0,
                            ServicePONo = TE.ServicePONo,
                            Remarks = TO.Remarks ?? ""
                        }
                        );

            return
                dbResult
                .OrderBy(o => o.IDRoute)
                .ToList();
        }

        public List<FnTransportationSummaryDTO> GetSummaryVendor(string filtertab, string filterrole, string filtersort, string filteryear)
        {
            using (TOMContextDB context = new TOMContextDB())
            {
                var dbresult = context.FnTransportationSummary(filtertab, filterrole, filtersort,filteryear);
                return Mapper.Map<List<FnTransportationSummaryDTO>>(dbresult);
            }
        }
        public List<KPILoadFactorAvgDTO> GetRecordsAvg(int year)
        {
            using (TOMContextDB context = new TOMContextDB())
            {
                var dbResult = context.KPILoadFactorAvg(year);
                var orderedResult = dbResult.OrderBy(x => x.RowNumber).ToList();
                return Mapper.Map<List<KPILoadFactorAvgDTO>>(orderedResult);
            }
        }

        public void CalculateCost(List<string> idTransportExecution, string userId)
        {
            var ctx = new TOMContextDB();
            var IDExecs = idTransportExecution.Select(int.Parse).ToList();
            //recalculate total box
            var ListExec = (from te in ctx.TransportExecutions
                    join to in ctx.TransportOrders on te.IDTransportExecution equals to.IDTransportExecution
                    join tod in ctx.TransportOrderDetails on to.IDTransportOrder equals tod.IDTransportOrder
                    join mfb in ctx.MasterFABrands on tod.Code equals mfb.FACode
                    where IDExecs.Contains(te.IDTransportExecution)
                          && to.OrderType == "Finished Good" && tod.Qty > 0
                          && to.IsActive && tod.IsActive
                    select new
                    {
                        TotalBox = tod.UoM.ToLower() != "box" ? tod.Qty / mfb.PackPerBox : tod.Qty,
                        IDExecution = te.IDTransportExecution,
                        Qty = tod.Qty,
                        PackPerBox = mfb.PackPerBox,
                        UoM = tod.UoM.ToLower()
                    })
                .GroupBy(f => f.IDExecution)
                .Select(f => new {
                    IDTransportExecution = f.Key,
                    TotalBox = f.Sum(g => g.TotalBox)
                })
                .ToDictionary(f => f.IDTransportExecution, f => f.TotalBox);
            /*
            decimal? TotalBox = 0;

            foreach (var Brand in ListBrand)
            {
                TotalBox += Brand.UoM != "box" ? Brand.Qty / Brand.PackPerBox : Brand.Qty;
            }
            */

            //update Total Box for each selected ID Execution
            if (ListExec.Count > 0)
            {
                ctx.TransportExecutions
                    .Where(f => IDExecs.Contains(f.IDTransportExecution))
                    .ToList()
                    .ForEach(r =>
                    {
                        r.TotalBox = ListExec.ContainsKey(r.IDTransportExecution) ? (int)ListExec[r.IDTransportExecution] : (int?)null;
                    });
                ctx.SaveChanges();
            }
            //dbRes.TotalBox = (int)TotalBox;

            #region Hitung Total KM
            foreach (string id in idTransportExecution)
            {
                TransportExecution tempExe = _transportExecutionRepo.GetTransportExecutionActiveByID(Int32.Parse(id));
                //decimal? totalKM = 0;
                //List<String> hasilTotalKM = _transportExecutionBll.CalculateTransportExecutionDistance("KM Based", tempExe.TransportDate, tempExe.TransportMode, Mapper.Map<List<TransportRoute>,List<TransportRouteDTO>>(tempExe.TransportRoutes.ToList()), "");
                //foreach (string hasil in hasilTotalKM)
                //{
                //    string[] split = hasil.Split('-');
                //    if (split[1] == "Land")
                //    {
                //        tempExe.TotalKMBased = Convert.ToDecimal(split[2]);
                //        totalKM = totalKM + tempExe.TotalKMBased;
                //    }
                //    else if (split[1] == "Sea")
                //    {
                //        tempExe.TotalKMSea = Convert.ToDecimal(split[2]);
                //        totalKM = totalKM + tempExe.TotalKMSea;
                //    }
                //    else if (split[1] == "Train")
                //    {
                //        tempExe.TotalKMRail = Convert.ToDecimal(split[2]);
                //        totalKM = totalKM + tempExe.TotalKMRail;
                //    }
                //}
                var totalKM = _transportExecutionBll.CalculateDistance("KM Based", tempExe.TransportDate, tempExe.TransportMode, Mapper.Map<List<TransportRouteDTO>>(tempExe.TransportRoutes));
                tempExe.TotalKM = totalKM;
                _transportExecutionRepo.SaveData(tempExe);
                _transportExecutionRepo.CalculateLoadFactorCFP(Int32.Parse(id));
            }
            #endregion
            _transportExecutionRepo.CalculateCost(String.Join(",", idTransportExecution), userId);
        }

        public IEnumerable<TransportSummaryReclassViewDTO> GetReclass(int year)
        {
            var queryFilter = PredicateHelper.True<TransportSummaryReclassView>();
            if (year != 0)
            {
                queryFilter = queryFilter.And(k => k.Year.Value == year);
            }
            var dbResult = _reclassView.Get(queryFilter).ToList();
            return Mapper.Map<List<TransportSummaryReclassViewDTO>>(dbResult);
        }

        public IEnumerable<FnTransportationSummaryCRateDTO> GetCrashRate(string year)
        {
            var dbResult = _transportExecutionRepo.GetCrashRate(year);
            return Mapper.Map<List<FnTransportationSummaryCRateDTO>>(dbResult);
        }

        public FnTransportationSummaryDTO GenerateChart(string filtertab, string filterrole, string filtersort, string filteryear)
        {
            var dtReturn = new FnTransportationSummaryDTO();
            var result = new List<Series>();
            try
            {
                using (TOMContextDB context = new TOMContextDB())
                {
                    var dbresult =
                        context.FnTransportationSummary(filtertab, filterrole, filtersort, filteryear).ToList();
                    var resultName = dbresult.GroupBy(x => x.p2).Select(x => new {x.Key}).ToList();

                    for (var i = 0; i < resultName.Count; i++)
                    {
                        var seriesData = new Series();
                        seriesData.name = resultName[i].Key;
                        seriesData.data =
                            dbresult.Where(x => x.p2 == resultName[i].Key).Select(x => Convert.ToInt32(x.val)).ToList();

                        result.Add(seriesData);
                    }

                }
                dtReturn.SeriesList = result;
                return dtReturn;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public FnTransportationSummaryDTO GenerateChartDecimal(string filtertab, string filterrole, string filtersort, string filteryear)
        {
            var dtReturn = new FnTransportationSummaryDTO();
            var result = new List<SeriesDecimalsDTO>();
            try
            {
                using (TOMContextDB context = new TOMContextDB())
                {
                    var dbresult =
                        context.FnTransportationSummary(filtertab, filterrole, filtersort, filteryear).ToList();
                    var resultName = dbresult.GroupBy(x => x.p2).Select(x => new { x.Key }).ToList();

                    for (var i = 0; i < resultName.Count; i++)
                    {
                        var seriesData = new SeriesDecimalsDTO();
                        seriesData.name = resultName[i].Key;
                        var val = dbresult.Where(x => x.p2 == resultName[i].Key).Select(x => Convert.ToInt64(Math.Round(Convert.ToDouble(x.val.Replace(",","."))))).ToList();
                        seriesData.data = val;
                            //dbresult.Where(x => x.p2 == resultName[i].Key).Select(x => Convert.ToDecimal(x.val)).ToList();

                        result.Add(seriesData);
                    }

                }
                dtReturn.SeriesListDecimal = result;
                return dtReturn;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
    }
}
