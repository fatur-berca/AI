using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Universal.Domain.DTOs;
using DFIS.Utils;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Repositories;
using TOM.Transport.Repositories.TransportExecutionRepo;
using DFIS.Contracts;
using System.Web;
using OfficeOpenXml;
using TOM.Transport.Repositories.TransportRouteRepo;
using TOM.Transport.Repositories.TransportVesselMonitoringRepo;
using TOM.Transport.Repositories.TransportVendorChangeLogRepo;
using DFIS.Universal.Domain.Outputs;
using System.Collections;
using DFIS.Universal;
using TOM.Transport.Repositories.TransportExecutionTempRepo;
using System.Configuration;
using TOM.Master.BusinessLogics.Mappers;

namespace TOM.Transport.BusinessLogics.TransportExecutionBLL
{
    public class TransportExecutionBLL : ITransportExecutionBLL
    {
        private readonly ITransportOrderRepo _transportOrderRepo;
        private readonly ITransportOrderDetailRepo _transportOrderDetailRepo;
        private readonly ITransportExecutionRepo _transportExecutionRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;
        private readonly IMasterVendorTOMRepo _masterVendorRepo;//
        private readonly IGenericRepository<TransportMonitoringView> _transportMonitoringView;
        private readonly IMasterMappingRepo _masterMappingRepo;
        private readonly IMasterGenWeekRepo _masterGenWeekRepo;
        private readonly IMasterVendorSuggestionRepo _masterVendorSuggestionRepo;
        private readonly ITransportDriverManagementRepo _transportDriverManagementRepo;
        private readonly IMasterServicePORepo _masterServicePORepo;
        private readonly ITransportVehicleDataRepo _transportVehicleDataRepo;
        private readonly ITransportVesselMonitoringRepo _transportVesselRepo;
        private readonly IGenericRepository<TransportExecution> _generalRepo;
        private readonly IMasterCostCenterAccountRepo _masterCostCenterRepo;
        private readonly IMasterDistanceRepo _masterDistanceRepo;
        private readonly ITransportRouteRepo _transportRouteRepo;
        private readonly ITransportVesselMonitoringRepo _transportVesselMonitoringRepo;
        private readonly ITransportVendorChangeLogRepo _transportVendorChangeLogRepo;
        private readonly IMasterFABrandRepo _masterFaBrandRepo;
        private readonly IMasterLeadTimeTomRepo _masterLeadTimeRepo;
        private readonly IMasterConfigurationRepo _masterConfigurationRepo;
        private readonly ITransportExecutionTempRepo _transportExecutionTempRepo;
        private readonly IGenericRepository<MasterUser> _masterUserRepo;
        private readonly IGenericRepository<MasterUserLocationMapping> _masterUserLocationMappingRepo;
        private readonly IGenericRepository<MasterDistance> _distanceRepo;
        private readonly IGenericRepository<TransportOrder> _orderRepo;

        public TransportExecutionBLL(IGenericRepository<TransportExecution> repo, IGenericRepository<MasterDistance> distanceRepo, IGenericRepository<TransportOrder> orderRepo, ITransportVesselMonitoringRepo transportVesselRepo, ITransportOrderRepo transportOrderRepo, ITransportOrderDetailRepo transportOrderDetailRepo, ITransportExecutionRepo transportExecutionRepo, IMasterListRepo masterListRepo, IMasterLocationRepo masterLocationRepo, IMasterVendorTOMRepo masterVendorRepo, IGenericRepository<TransportMonitoringView> transportMonitoringView, IMasterMappingRepo masterMappingRepo, IMasterGenWeekRepo masterGenWeekRepo, IMasterVendorSuggestionRepo masterVendorSuggestionRepo, ITransportDriverManagementRepo transportDriverManagementRepo, IMasterServicePORepo masterServicePORepo, ITransportVehicleDataRepo transportVehicleDataRepo, IMasterCostCenterAccountRepo masterCostCenterRepo, IMasterDistanceRepo masterDistanceRepo, ITransportRouteRepo transportRouteRepo, ITransportVesselMonitoringRepo transportVesselMonitoringRepo, ITransportVendorChangeLogRepo transportVendorChangeLogRepo, IMasterFABrandRepo masterFaBrandRepo, IMasterLeadTimeTomRepo masterLeadTimeRepo, IMasterConfigurationRepo masterConfigurationRepo, ITransportExecutionTempRepo transportExecutionTempRepo, IGenericRepository<MasterUser> masterUserRepo, IGenericRepository<MasterUserLocationMapping> masterUserLocationMappingRepo)
        {
            _transportOrderRepo = transportOrderRepo;
            _transportOrderDetailRepo = transportOrderDetailRepo;
            _transportExecutionRepo = transportExecutionRepo;
            _masterListRepo = masterListRepo;
            _masterLocationRepo = masterLocationRepo;
            _masterVendorRepo = masterVendorRepo;
            _masterMappingRepo = masterMappingRepo;
            _masterGenWeekRepo = masterGenWeekRepo;
            _masterVendorSuggestionRepo = masterVendorSuggestionRepo;
            _transportDriverManagementRepo = transportDriverManagementRepo;
            _masterServicePORepo = masterServicePORepo;
            _transportVehicleDataRepo = transportVehicleDataRepo;
            _transportMonitoringView = transportMonitoringView;
            _masterCostCenterRepo = masterCostCenterRepo;
            _masterDistanceRepo = masterDistanceRepo;
            _transportRouteRepo = transportRouteRepo;
            _transportVesselMonitoringRepo = transportVesselMonitoringRepo;
            _transportVendorChangeLogRepo = transportVendorChangeLogRepo;
            _masterFaBrandRepo = masterFaBrandRepo;
            _masterLeadTimeRepo = masterLeadTimeRepo;
            _masterConfigurationRepo = masterConfigurationRepo;
            _transportExecutionTempRepo = transportExecutionTempRepo;
            _transportVesselRepo = transportVesselRepo;
            _masterUserRepo = masterUserRepo;
            _masterUserLocationMappingRepo = masterUserLocationMappingRepo;
            _generalRepo = repo;
            _distanceRepo = distanceRepo;
            _orderRepo = orderRepo;
        }

        // author : Hakim
        // date : 2019-10-01 11:25
        // ------------------------- BEGIN -------------------------------

        public TransportVesselMonitoringDTO GetTransportVessel(int IDTransportExecution)
        {
            var val = _transportVesselRepo.Get(v => v.IDTransportExecution == IDTransportExecution).FirstOrDefault();
            if (val == null)
                return null;
            return Mapper.Map<TransportVesselMonitoringDTO>(val);
        }

        public bool SendMail(string subject, string body, string[] recipients, string format = "HTML")
        {
            return _transportOrderRepo.SendMail(subject, body, recipients, format);
        }

        public List<TransportExecutionDTO> GetTransportNo(TransportationExecutionInput input)
        {
            var dbResult = new List<TransportExecutionDTO>();

            TOMContextDB context = new TOMContextDB();
            if (!String.IsNullOrEmpty(input.TransportNo))
            {
                dbResult = (from a in context.TransportExecutions
                            where
                            (a.TransportNo.Contains(input.TransportNo) &&
                            a.TransportMode.Contains(input.TransportMode))
                            select new TransportExecutionDTO()
                            {
                                IDTransportExecution = a.IDTransportExecution,
                                TransportNo = a.TransportNo,
                                TransportDate = a.TransportDate
                            }).ToList();
            }

            return dbResult;
        }

        public TransportExecutionDTO GetTransportNo(string TN)
        {
            TransportExecutionDTO dbResult = null;

            TOMContextDB context = new TOMContextDB();
            if (!String.IsNullOrEmpty(TN))
            {
                var res = (from a in context.TransportExecutions
                           where
                           (a.TransportNo == TN)
                           select a
                            ).FirstOrDefault();
                if (res != null)
                    dbResult = Mapper.Map<TransportExecutionDTO>(res);
            }

            return dbResult;
        }

        public List<TransportOrderDTO> GetOrderNo(TransportOrderInput input)
        {
            var dbResult = new List<TransportOrderDTO>();

            TOMContextDB context = new TOMContextDB();

            if (!String.IsNullOrEmpty(input.STONo))
            {
                dbResult = (from a in context.TransportOrders
                            where a.STONo.Contains(input.STONo)
                            select new TransportOrderDTO()
                            {
                                IDTransportOrder = a.IDTransportOrder,
                                STONo = a.STONo,
                            }).Take(15).ToList();
            }

            return dbResult;
        }

        public List<TransportMonitoringViewDTO> GetTransportExecutionDTOs(TransportMonitoringViewInput input)
        {
            var queryFilter = PredicateHelper.True<TransportMonitoringView>();

            if (!String.IsNullOrEmpty(input.TransportNo))
            {
                string[] inArray = input.TransportNo.Split(',');
                queryFilter = queryFilter.And(k => inArray.Contains(k.TransportNo));
            }

            if (!String.IsNullOrEmpty(input.STONo))
            {
                string[] inArray = input.STONo.Split(',');
                queryFilter = queryFilter.And(k => inArray.Contains(k.STONo));
            }

            if (!String.IsNullOrEmpty(input.StartLocation))
            {
                queryFilter = queryFilter.And(k => k.StartLocation == input.StartLocation);
            }

            if (!String.IsNullOrEmpty(input.TransportStatus))
            {
                queryFilter = queryFilter.And(k => k.TransportStatus == input.TransportStatus);
            }

            if (input.DateOfStuffingFrom != null)
            {
                DateTime DateFrom = Convert.ToDateTime(input.DateOfStuffingFrom + " 00:00");
                queryFilter = queryFilter.And(m => m.TransportDate >= DateFrom);
            }
            if (input.DateOfStuffingTo != null)
            {
                DateTime DateTo = Convert.ToDateTime(input.DateOfStuffingTo + " 00:00");
                queryFilter = queryFilter.And(m => m.TransportDate <= DateTo);
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
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportMonitoringView>();
            var dbResult = _transportMonitoringView.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<TransportMonitoringViewDTO>>(dbResult);
        }

        public int UpdateVessel(TransportMonitoringViewInput input, string userid)
        {
            return _transportVesselMonitoringRepo.UpdateVessel(input, userid);
        }

        //public int UpdateOrder(TransportMonitoringViewInput input, string userid)
        //{
        //    return _transportVesselMonitoringRepo.UpdateOrder(input, userid);
        //}
        // ------------------------------ END -------------------------------------------

        private List<UserRole> GetListUserRole()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listrole = currentSession.Role.ToList();
            return listrole;
        }
        public TransportExecutionEditDTO GetTransportExecutionByID(int id)
        {
            var getTE = _generalRepo.GetOne(_ => _.IsActive && _.IDTransportExecution == id);
            if (getTE == null)
            {
                throw new Exception("Data not exist!");
            }
            var temp = Mapper.Map<TransportExecutionEditDTO>(getTE);
            temp.TransportOrders.RemoveAll(x => x.IsActive == false);
            temp.TransportOrders.ForEach(x => x.TransportOrderDetails.RemoveAll(y => y.IsActive == false));

            List<MasterLocationDTO> masterLocationList = Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationForDisplay());
            List<TransportDriverManagementDTO> distinctDriverList = GetALLTransportDriverManagementList().GroupBy(x => x.ID).Select(x => x.First()).ToList();
            List<TransportVehicleDataDTO> distinctVehicleList = GetALLTransportVehicleDataList().GroupBy(x => x.IDPoliceRegNumber).Select(x => x.First()).ToList();
            
            TransportVendorChangeLogDTO log = temp.TransportVendorChangeLogs.Where(x => x.IsActive == true).OrderByDescending(x => x.IDTransportExecution).FirstOrDefault();
            temp.IsUnfullfillOrRecall = log == null ? false : log.SIStatus == EnumHelper.GetDescription(Enums.SIStatus.Unfullfill) || log.SIStatus == EnumHelper.GetDescription(Enums.SIStatus.Recall);
            
            List<string> finishGoodList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.OrderType.FinishedGood));
            List<string> rawMaterialList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.OrderType.RawMaterial));
            List<string> bsList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.OrderType.BadStock));
            List<string> otherList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.OrderType.Other));
            List<TransportOrderTransportExecutionEditDTO> tempTOList = temp.TransportOrders;
            foreach (TransportOrderTransportExecutionEditDTO tempTEEdit in tempTOList.Where(x => x.OrderType == EnumHelper.GetDescription(Enums.OrderType.FinishedGood) || finishGoodList.Contains(x.OrderType)))
            {
                decimal tempTotalBox = 0;
                //cara lainnya di bagi 2 list
                //list pertama di grup uom box dan di sum qtynya di tambah
                //list kedua, selain box, di convert dulu baru di sum qtynya
                foreach (TransportOrderDetailDTO tempdet in tempTEEdit.TransportOrderDetails)
                {
                    if (tempdet.Qty != null)
                    {
                        if (tempdet.UoM != "Box")
                        {
                            MasterFABrand tempFaBrand = _masterFaBrandRepo.GetMasterFABrandByFaCode(tempdet.Code);
                            if (tempFaBrand != null)
                            {
                                if (tempdet.UoM == "Pack")
                                {
                                    tempTotalBox = tempTotalBox + (tempdet.Qty.Value / tempFaBrand.PackPerBox);
                                }
                                else if (tempdet.UoM == "Stick")
                                {
                                    tempTotalBox = tempTotalBox + (tempdet.Qty.Value / tempFaBrand.StickPerBox);
                                }
                            }
                        }
                        else
                            tempTotalBox = tempTotalBox + tempdet.Qty.Value;
                    }
                }
                temp.TransportOrders.Where(x => x.IDTransportOrder == tempTEEdit.IDTransportOrder).Select(x =>
                {
                    x.TotalBox = tempTotalBox;
                    return x;
                }).ToList();
            }
            temp.TransportOrders.Select(x =>
            {
                if (x.TransportOrderDetails.Count > 0)
                {
                    MasterCostCenterAccountDTO costCenter = Mapper.Map<MasterCostCenter, MasterCostCenterAccountDTO>(_masterCostCenterRepo.GetMasterCostCenterBySenderReceiveMaterial(x.ShipmentDate, x.ActualSenderIDLocation, x.ActualReceiverIDLocation, x.TransportOrderDetails[0].MaterialType));
                    if (costCenter != null && x.MasterCostCenter == null)
                    {
                        x.MasterCostCenter = costCenter;
                        x.IDCostCenter = costCenter.IDCostCenter;
                        x.CostCenter = x.MasterCostCenter.CostCenter;
                    }
                }
                string tempfg = finishGoodList.FirstOrDefault(y => y.Equals(x.OrderType));
                string temprmt = rawMaterialList.FirstOrDefault(y => y.Equals(x.OrderType));
                string tempbs = bsList.FirstOrDefault(y => y.Equals(x.OrderType));
                string tempother = otherList.FirstOrDefault(y => y.Equals(x.OrderType));
                if (tempfg != null || x.OrderType == EnumHelper.GetDescription(Enums.OrderType.FinishedGood))
                {
                    x.FGRMT = EnumHelper.GetDescription(Enums.OrderType.FinishedGood);
                }
                else if (temprmt != null || x.OrderType == EnumHelper.GetDescription(Enums.OrderType.RawMaterial))
                {
                    x.FGRMT = EnumHelper.GetDescription(Enums.OrderType.RawMaterial);
                }
                else if (tempbs != null || x.OrderType == EnumHelper.GetDescription(Enums.OrderType.BadStock))
                {
                    x.FGRMT = EnumHelper.GetDescription(Enums.OrderType.BadStock);
                }
                else if (tempother != null || x.OrderType == EnumHelper.GetDescription(Enums.OrderType.Other))
                {
                    x.FGRMT = EnumHelper.GetDescription(Enums.OrderType.Other);
                }
                else
                {
                    x.FGRMT = x.OrderType;
                }
                return x;
            }).ToList();
            //temp.TransportOrders = new List<TransportOrderTransportExecutionEditDTO>();
            //foreach (string fg in finishGoodList)
            //{
            //    //bagian cek cost center sudah ada/belum, kalau belum di isi
            //    var orderType = EnumHelper.GetDescription(Enums.MasterialType.FinishedGood);
            //    temp.TransportOrders.AddRange(tempTOList.Where(x => x.OrderType == fg).Select(x =>
            //    {
            //        MasterCostCenterAccountDTO costCenter = Mapper.Map<MasterCostCenter, MasterCostCenterAccountDTO>(_masterCostCenterRepo.GetMasterCostCenterBySenderReceiveMaterial(x.ShipmentDate, x.SenderIDLocation, x.ReceiverIDLocation, ""));
            //        if (costCenter != null && x.MasterCostCenter == null)
            //        {
            //            x.MasterCostCenter = costCenter;
            //            x.IDCostCenter = costCenter.IDCostCenter;
            //            x.CostCenter = x.MasterCostCenter.CostCenter;
            //        }
            //        x.FGRMT = orderType;
            //        return x;
            //    }).ToList());

            //    foreach (TransportOrderTransportExecutionEditDTO toTotalBoxList in tempTOList.Where(x => x.OrderType == fg))
            //    {
            //        decimal tempTotalBox = 0;
            //        //cara lainnya di bagi 2 list
            //        //list pertama di grup uom box dan di sum qtynya di tambah
            //        //list kedua, selain box, di convert dulu baru di sum qtynya
            //        foreach (TransportOrderDetailDTO tempdet in toTotalBoxList.TransportOrderDetails)
            //        {
            //            if (tempdet.Qty != null) { 
            //                if (tempdet.UoM != "Box")
            //                {
            //                    MasterFABrand tempFaBrand = _masterFaBrandRepo.GetMasterFABrandByFaCode(tempdet.Code);
            //                    if (tempFaBrand != null)
            //                    {
            //                        if (tempdet.UoM == "Pack")
            //                        {
            //                            tempTotalBox = tempTotalBox + (tempdet.Qty.Value / tempFaBrand.PackPerBox);
            //                        }
            //                        else if (tempdet.UoM == "Stick")
            //                        {
            //                            tempTotalBox = tempTotalBox + (tempdet.Qty.Value / tempFaBrand.StickPerBox);
            //                        }
            //                    }
            //                }
            //                else
            //                    tempTotalBox = tempTotalBox + tempdet.Qty.Value;
            //            }
            //        }
            //        temp.TransportOrders.Where(x => x.IDTransportOrder == toTotalBoxList.IDTransportOrder).Select(x =>
            //        {
            //            x.TotalBox = tempTotalBox;
            //            return x;
            //        }).ToList();
            //    }
            //}
            ////bagian data dari sap yang order type tulisannya bukan raw material
            //foreach (string rmt in rawMaterialList)
            //{
            //    var orderType = EnumHelper.GetDescription(Enums.MasterialType.RawMaterial);
            //    temp.TransportOrders.AddRange(tempTOList.Where(x => x.OrderType == rmt).Select(x =>
            //    {
            //        MasterCostCenterAccountDTO costCenter = Mapper.Map<MasterCostCenter, MasterCostCenterAccountDTO>(_masterCostCenterRepo.GetMasterCostCenterBySenderReceiveMaterial(x.ShipmentDate, x.SenderIDLocation, x.ReceiverIDLocation, ""));
            //        if (costCenter != null && x.MasterCostCenter == null)
            //        {
            //            x.MasterCostCenter = costCenter;
            //            x.IDCostCenter = costCenter.IDCostCenter;
            //            x.CostCenter = x.MasterCostCenter.CostCenter;
            //        }
            //        x.FGRMT = orderType;
            //        return x;
            //    }).ToList()); 
            //}
            ////bagian data dari to yang order type tulisannya raw material
            //temp.TransportOrders.AddRange(tempTOList.Where(x => x.OrderType == EnumHelper.GetDescription(Enums.MasterialType.RawMaterial)).Select(x =>
            //{
            //    MasterCostCenterAccountDTO costCenter = Mapper.Map<MasterCostCenter, MasterCostCenterAccountDTO>(_masterCostCenterRepo.GetMasterCostCenterBySenderReceiveMaterial(x.ShipmentDate, x.SenderIDLocation, x.ReceiverIDLocation, ""));
            //    if (costCenter != null && x.MasterCostCenter == null)
            //    {
            //        x.MasterCostCenter = costCenter;
            //        x.IDCostCenter = costCenter.IDCostCenter;
            //        x.CostCenter = x.MasterCostCenter.CostCenter;
            //    }
            //    x.FGRMT = EnumHelper.GetDescription(Enums.MasterialType.RawMaterial);
            //    return x;
            //}).ToList()); 
            //temp.TotalKMBased = temp.TransportOrders.Sum(_ => _.KM);
            temp.TransportOrders.ForEach(x =>
            {
                x.MasterLocation2 = masterLocationList.FirstOrDefault(y => y.IDLocation == x.ActualSenderIDLocation);
                x.MasterLocation3 = masterLocationList.FirstOrDefault(z => z.IDLocation == x.ActualReceiverIDLocation);
            });
            temp.TransportRoutes.ForEach(x =>
            {
                x.LocationName = masterLocationList.FirstOrDefault(y => y.IDLocation == x.IDLocation).LocationName;
            });
            temp.TotalKMBased = CalculateDistance("KM Based", temp.TransportDate, temp.TransportMode, temp.TransportRoutes);
            temp.TransportDriverManagement = distinctDriverList.Where(x => x.ID == temp.IDDriver1).FirstOrDefault();
            temp.TransportDriverManagement1 = distinctDriverList.Where(x => x.ID == temp.IDDriver2).FirstOrDefault();
            temp.TransportDriverManagement2 = distinctDriverList.Where(x => x.ID == temp.IDCoDriver).FirstOrDefault();
            temp.TransportVehicleData = distinctVehicleList.Where(x => x.IDPoliceRegNumber == temp.PoliceRegNo).FirstOrDefault();
            return temp;

        }

        public TransportOrderTransportExecutionEditDTO GetTransportOrderActiveByStoNO(string stono)
        {
            TransportExecution tempExe = null;
            decimal totalKM = 0;
            TransportOrder inputTO = _transportOrderRepo.GetTransportOrderBySTONo2(stono);
            TransportOrderTransportExecutionEditDTO temp = Mapper.Map<TransportOrderDTO, TransportOrderTransportExecutionEditDTO>(_transportOrderRepo.GetTransportOrderActiveBySTONo(stono));

            if (temp != null)
            {
                temp.TransportOrderDetails.RemoveAll(x => x.IsActive == false);
                var mstDistance = _masterDistanceRepo.GetMasterDistanceByField(temp.SenderIDLocation, temp.ReceiverIDLocation, temp.ShipmentDate);
                if (mstDistance != null) totalKM = Convert.ToDecimal(mstDistance.Total);
                inputTO.KM = totalKM;
                _transportOrderRepo.SaveData(inputTO);
                List<string> rawMaterialList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.MasterialType.RawMaterial));
                List<string> finishGoodList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.MasterialType.FinishedGood));
                MasterCostCenterAccountDTO costCenter = Mapper.Map<MasterCostCenter, MasterCostCenterAccountDTO>(_masterCostCenterRepo.GetMasterCostCenterBySenderReceiveMaterial(temp.ShipmentDate, temp.SenderIDLocation, temp.ReceiverIDLocation, temp.TransportOrderDetails[0].MaterialType));
                if (temp.IDTransportExecution != null)
                {
                    tempExe = _transportExecutionRepo.GetTransportExecutionActiveByID(temp.IDTransportExecution.Value);
                }
                if (tempExe != null)
                    temp.TransportNo = tempExe.TransportNo;
                if (costCenter != null && temp.MasterCostCenter == null)
                {
                    temp.MasterCostCenter = costCenter;
                    temp.IDCostCenter = costCenter.IDCostCenter;
                    temp.CostCenter = temp.MasterCostCenter.CostCenter;
                }
                if (rawMaterialList.Contains(temp.OrderType) || temp.OrderType == EnumHelper.GetDescription(Enums.MasterialType.RawMaterial))
                    temp.FGRMT = EnumHelper.GetDescription(Enums.MasterialType.RawMaterial);
                if (finishGoodList.Contains(temp.OrderType) || temp.OrderType == EnumHelper.GetDescription(Enums.MasterialType.FinishedGood))
                {
                    temp.FGRMT = EnumHelper.GetDescription(Enums.MasterialType.FinishedGood);
                    decimal tempTotalBox = 0;
                    foreach (TransportOrderDetailDTO tempdet in temp.TransportOrderDetails)
                    {
                        if (tempdet.Qty != null)
                        {
                            if (tempdet.UoM != "Box")
                            {
                                MasterFABrand tempFaBrand = _masterFaBrandRepo.GetMasterFABrandByFaCode(tempdet.Code);
                                if (tempFaBrand != null)
                                {
                                    if (tempdet.UoM == "Pack")
                                    {
                                        tempTotalBox = tempTotalBox + (tempdet.Qty.Value / tempFaBrand.PackPerBox);
                                    }
                                    else if (tempdet.UoM == "Stick")
                                    {
                                        tempTotalBox = tempTotalBox + (tempdet.Qty.Value / tempFaBrand.StickPerBox);
                                    }
                                }
                            }
                            else
                                tempTotalBox = tempTotalBox + tempdet.Qty.Value;
                        }
                    }
                    temp.TotalBox = tempTotalBox;
                }
                temp.KM = totalKM;
            }
            return temp;
        }

        public List<TransportOrderDTO> GetSTONoFilterByRegion(string stono, bool isUserRoleTransport, List<string> parentLocation)
        {
            if (isUserRoleTransport)
                return MappingHelper.Map<TransportOrderDTO>(_transportOrderRepo.GetSTONoFilter(stono)).ToList();
            //return Mapper.Map<List<TransportOrder>, List<TransportOrderDTO>>(_transportOrderRepo.GetSTONoFilter(stono));
            else
                return Mapper.Map<List<TransportOrder>, List<TransportOrderDTO>>(_transportOrderRepo.GetSTONoFilterByListRegion(stono, parentLocation));
        }


        public List<TransportExecutionDTO> GetTransportationNumberFilter(string stono, bool isUserRoleTransport, string UserId)
        {
            if (isUserRoleTransport)
                return Mapper.Map<List<TransportExecution>, List<TransportExecutionDTO>>(_transportExecutionRepo.GetTransportExecutionFilter(stono));
            else
                return Mapper.Map<List<TransportExecution>, List<TransportExecutionDTO>>(_transportExecutionRepo.GetTransportationNumberFilterByUserLocations(stono, UserId));
        }

        public IEnumerable<object> GetTNCreatorShipping(string tncreator)
        {
            var result = _transportExecutionRepo.GetTNCreatorShipping(tncreator);

            return result;
        }

        public List<TransportExecutionDTO> GetTransportationNumberFilterByRegion(string stono, bool isUserRoleTransport, List<string> parentLocation)
        {
            if (isUserRoleTransport)
                return Mapper.Map<List<TransportExecution>, List<TransportExecutionDTO>>(_transportExecutionRepo.GetTransportExecutionFilter(stono));
            else
                return Mapper.Map<List<TransportExecution>, List<TransportExecutionDTO>>(_transportOrderRepo.GetTransportationNumberFilterByListRegion(stono, parentLocation));
        }

        public List<MasterListDTO> getMasterList(List<string> fieldName = null)
        {
            if(fieldName == null)
            {
                fieldName = new List<string>(new[] { "OrderCategory", "SIStatus", "OrderType", "MaterialType", "ExecutionType", "VehicleType", "TransportationMode", "TransportationCategory", "SIType", "ViaRoute", "VesselName", "TransportationStatus" });
            }
            return Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByListFieldName(fieldName));
        }

        public List<MasterLocationDTO> getMasterLocation(bool isUserRoleTransport, List<string> parentLocation)
        {
            /*if (isUserRoleTransport)
                return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Warehouse"));
            else
                return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetMasterLocationListByListParentLocation(parentLocation));*/
            return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocation());
        }

        public List<MasterListDTO> GetZoneList(List<UserRole> userRole)
        {
            if (userRole.Count == 1 && userRole[0].RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR))
            {
                List<MasterConfigurationDTO> tempConfig = Mapper.Map<List<MasterConfigurationDTO>>(_masterConfigurationRepo.GetListMasterConfigurationByPageNameDescription(EnumHelper.GetDescription(Enums.MasterConfigurationValue.RoleZoneMapping), EnumHelper.GetDescription(Enums.RoleUserList.AWR)));
                if (tempConfig.Count > 1)
                    return Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldNameListFieldValue("Zone", tempConfig.Select(x => x.Value).ToList()));
            }
            return Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("Zone"));
        }

        public List<MasterVendorTOMDTO> GetVendorList(List<UserRole> userRole)
        {
            if (userRole.Count == 1 && userRole[0].RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR))
            {
                List<MasterConfigurationDTO> tempConfig = Mapper.Map<List<MasterConfigurationDTO>>(_masterConfigurationRepo.GetListMasterConfigurationByPageNameDescription(EnumHelper.GetDescription(Enums.MasterConfigurationValue.RoleVendorMapping), EnumHelper.GetDescription(Enums.RoleUserList.AWR)));
                if (tempConfig.Count > 1)
                    return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorByListVendorName(tempConfig.Select(x => x.Value).ToList()));
            }
            //return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorActive());
            return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorActiveNoChild());
        }
        public List<MasterVendorTOMDTO> GetVendorSuggestionList(List<UserRole> userRole)
        {
            //if (userRole.Count == 1)
            //{
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

        public List<string> GetCostCenterList()
        {
            return _masterCostCenterRepo.GetDistinctCostCenterListActive();
        }

        public List<TransportVehicleDataDTO> GetVehicleDataList(List<UserRole> userRole)
        {
            TOMContextDB ctx = new TOMContextDB();
            var dbRes = Mapper.Map<List<TransportVehicleData>, List<TransportVehicleDataDTO>>(_transportVehicleDataRepo.GetALLTransportVehicleDataActiveList());

            if (!userRole.Select(f => f.RoleName).Contains("SUPER ADMIN"))
            {
                var retVal = (from veh in dbRes
                              join ven in ctx.MasterVendors on veh.IDVendor equals ven.IDVendor
                              join cfg in ctx.MasterConfigurations on ven.VendorName equals cfg.Value
                              where cfg.PageName.Equals("RoleVendorMapping") && userRole.Select(f => f.RoleName).Contains(cfg.Description)
                              select new TransportVehicleDataDTO { IDPoliceRegNumber = veh.IDPoliceRegNumber, IDVendor = veh.IDVendor }).ToList();
                return retVal;
            }
            return dbRes;
        }

        public List<TransportVehicleDataDTO> GetALLTransportVehicleDataList()
        {
            return Mapper.Map<List<TransportVehicleData>, List<TransportVehicleDataDTO>>(_transportVehicleDataRepo.GetALLTransportVehicleDataList());
        }

        public List<TransportVehicleDataDTO> GetVehicleByCriteriaActive(TransportVehicleDataInput criteria)
        {
            return Mapper.Map<List<TransportVehicleData>, List<TransportVehicleDataDTO>>(_transportVehicleDataRepo.GetDataByCriteriaActive(criteria));
        }

        public List<TransportDriverManagementDTO> GetDriverList(List<UserRole> userRole)
        {
            TOMContextDB ctx = new TOMContextDB();
            var dbRes = Mapper.Map<List<TransportDriverManagement>, List<TransportDriverManagementDTO>>(_transportDriverManagementRepo.GetALLTransportDriverManagementActiveList());

            if (!userRole.Select(f => f.RoleName).Contains("SUPER ADMIN"))
            {
                var retVal = (from drv in dbRes
                              join ven in ctx.MasterVendors on drv.IDVendor equals ven.IDVendor
                              join cfg in ctx.MasterConfigurations on ven.VendorName equals cfg.Value
                              where cfg.PageName.Equals("RoleVendorMapping") && userRole.Select(f => f.RoleName).Contains(cfg.Description)
                              select new TransportDriverManagementDTO { ID = drv.ID, Name = drv.Name, IDVendor = drv.IDVendor, RoleDriver = drv.RoleDriver }).ToList();
                return retVal;
            }
            return dbRes;
        }

        public List<TransportDriverManagementDTO> GetALLTransportDriverManagementList()
        {
            return Mapper.Map<List<TransportDriverManagement>, List<TransportDriverManagementDTO>>(_transportDriverManagementRepo.GetALLTransportDriverManagementList());
        }

        public List<TransportDriverManagementFilterDTO> GetDriverByCriteriaActive(TransportDriverManagementInput criteria)
        {
            var response = Mapper.Map<List<TransportDriverManagement>, List<TransportDriverManagementFilterDTO>>(_transportDriverManagementRepo.GetDataByCriteriaActive(criteria));
            //Filter Remove unecessary value
            //foreach (var item in response) { item.Address = "-"; }

            return response;
        }

        public MasterServicePoTomDTO GetServicePO(int idvendor, DateTime date)
        {
            return Mapper.Map<MasterServicePo, MasterServicePoTomDTO>(_masterServicePORepo.GetMasterServicePOByVendorEffectiveDate(idvendor, date));
        }

        public dynamic GetExecutionDataTable(TransportOrderInput input, string userid, DataTableModel model = null)
        {
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;

            #region FILTERS
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(_ => _.IsActive);
            if (!input.IsRoleTransport && !UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                var UserLocations = _masterUserLocationMappingRepo.Get(f => f.IDUser == userid && f.IsActive).Select(f => f.IDLocation).ToList();
                var Users = _masterUserLocationMappingRepo.Get(f => UserLocations.Contains(f.IDLocation)).Select(f => f.IDUser.ToLower()).Distinct();
                queryFilter = queryFilter.And(_ => Users.All(x => _.CreatedBy.ToLower() == x || _.CreatedBy.ToLower() == userid));
            }
            if (input.dateFromFilter.HasValue)
            {
                queryFilter = queryFilter.And(_ => _.TransportDate >= input.dateFromFilter.Value);
            }
            if (input.dateToFilter.HasValue)
            {
                queryFilter = queryFilter.And(c => c.TransportDate <= input.dateToFilter.Value);
            }
            if (input.transportNumberListFilter != null && input.transportNumberListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => input.transportNumberListFilter.Contains(c.TransportNo));
            }
            if (input.transportModeListFilter != null && input.transportModeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => input.transportModeListFilter.Contains(c.TransportMode));
            }
            if (input.siStatusListFilter != null && input.siStatusListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => input.siStatusListFilter.Contains(c.SIStatus));
            }
            if (input.startLocationListFilter != null && input.startLocationListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => input.startLocationListFilter.Contains(c.StartLocation));
            }
            if (input.transportStatusListFilter != null && input.transportStatusListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => input.transportStatusListFilter.Contains(c.TransportStatus));
            }
            if (input.stoNoListFilter != null && input.stoNoListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => input.stoNoListFilter.Contains(c2.STONo)));
            }
            if (input.senderIdLocListFilter != null && input.senderIdLocListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => input.senderIdLocListFilter.Contains(c2.ActualSenderIDLocation)));
            }
            if (input.receiverIdLocListFilter != null && input.receiverIdLocListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => input.receiverIdLocListFilter.Contains(c2.ActualReceiverIDLocation)));
            }
            if (input.orderTypeListFilter != null && input.orderTypeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(c => c.TransportOrders.Any(c2 => input.orderTypeListFilter.Contains(c2.OrderType)));
            }
            if (input.materialTypeListFilter != null && input.materialTypeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(_ => _.TransportOrders.SelectMany(x => x.TransportOrderDetails).Any(c => input.materialTypeListFilter.Contains(c.MaterialType)));
            }
            if (input.materialTypeListFilter != null && input.materialTypeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                queryFilter = queryFilter.And(_ => _.TransportOrders.SelectMany(x => x.TransportOrderDetails).Any(c => input.materialTypeListFilter.Contains(c.MaterialType)));
            }
            if (input.UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                queryFilter = queryFilter.And(c => input.UserRole.ToLower().Contains(c.TransportMode.ToLower()));
            }
            #endregion

            #region filter from DataTable
            //if (model != null)
            //{
            //    if (!String.IsNullOrEmpty(model.search.value))
            //    {
            //        var _us = model.search.value;
            //        queryFilter = queryFilter.And(_ => _.TransportNo.Contains(_us) || _.TransportStatus.Contains(_us) || _.CreatedBy.Contains(_us) || _.MasterVendor.VendorName.Contains(_us) || _.SIWeek.ToString().Contains(_us) || _.SIType.Contains(_us) || _.SIStatus.Contains(_us) || _.StartLocation.Contains(_us) || _.TransportDate.ToString().Contains(_us) || _.UpdatedBy.Contains(_us));
            //    }

            //    foreach (var fil in model.columns.Where(_ => !String.IsNullOrEmpty(_.search.value)))
            //    {
            //        var _col = fil.data;
            //        var _val = fil.search.value;
            //        switch (_col)
            //        {
            //            case "TransportNo":
            //                queryFilter = queryFilter.And(_ => _.TransportNo.Contains(_val));
            //                break;
            //            case "TransportStatus":
            //                queryFilter = queryFilter.And(_ => _.TransportStatus.Contains(_val));
            //                break;
            //            case "CreatedBy":
            //                //queryFilter = queryFilter.And(_ => _.CreatedBy.Contains(_val));
            //                var _getUser = _masterUserRepo.Get(_ => _.FullName.Contains(_val)).Select(_ => _.IDUser).ToList();
            //                queryFilter = queryFilter.And(c => _getUser.Contains(c.CreatedBy));
            //                break;
            //            case "VendorName":
            //                //queryFilter = queryFilter.And(_ => _.MasterVendor.VendorName.Contains(_val));
            //                var _getVendor = _masterVendorRepo.Get(_ => _.VendorName.Contains(_val)).Select(_ => _.IDVendor.ToString()).ToList();
            //                queryFilter = queryFilter.And(c => _getVendor.Contains(c.IDVendor.ToString()));
            //                break;
            //            case "SIWeek":
            //                queryFilter = queryFilter.And(_ => _.SIWeek.ToString().Contains(_val));
            //                break;
            //            case "SIType":
            //                queryFilter = queryFilter.And(_ => _.SIType.Contains(_val));
            //                break;
            //            case "SIStatus":
            //                queryFilter = queryFilter.And(_ => _.SIStatus.Contains(_val));
            //                break;
            //            case "StartLocation":
            //                //queryFilter = queryFilter.And(_ => _.StartLocation.Contains(_val));
            //                var _getStLocation = _masterLocationRepo.Get(_ => _.LocationName.Contains(_val)).Select(_ => _.IDLocation).ToList();
            //                queryFilter = queryFilter.And(c => _getStLocation.Contains(c.StartLocation));
            //                break;
            //            case "TransportDate":
            //                queryFilter = queryFilter.And(_ => _.TransportDate.ToString().Contains(_val));
            //                break;
            //            case "UpdatedDate":
            //                queryFilter = queryFilter.And(_ => _.UpdatedBy.Contains(_val));
            //                break;
            //        }
            //    }
            //}
            #endregion

            var getTE = Enumerable.Empty<TransportExecution>();
            getTE = _generalRepo.Get(queryFilter);
            //if (model == null)
            //{
            //    getTE = _generalRepo.Get(queryFilter);
            //}
            //else
            //{
            //    getTE = _generalRepo.GetAllPagination(queryFilter, model.start, model.length, "IDTransportExecution");
            //}
            var total = _generalRepo.Count(queryFilter);

            
            var _loc = new List<string>();
            var _usr = new List<string>();
            if (total > 0)
            {
                var listStartLoc = getTE.Select(_ => _.StartLocation);
                var listFinishLoc = getTE.Select(_ => _.FinishLocation);
                _loc.AddRange(listStartLoc);
                _loc.AddRange(listFinishLoc);
                _loc = _loc.Distinct().ToList();

                var listCreateBy = getTE.Select(_ => _.CreatedBy);
                var listUpdateBy = getTE.Select(_ => _.UpdatedBy);

                _usr.AddRange(listCreateBy);
                _usr.AddRange(listUpdateBy);
                _usr = _usr.Distinct().ToList();
            }
            

            var masterLocationList = _masterLocationRepo.Get(_ => _.IsActive && _loc.Contains(_.IDLocation));
            var masterUserList = _masterUserRepo.Get(x => x.IsActive && _usr.Contains(x.IDUser));

            var resultTE = new List<TransportExecutionDataTableDTO>();
            foreach (var d in getTE)
            {
                var _createdBy = masterUserList.Where(_ => _.IDUser.ToLower() == d.CreatedBy.ToLower()).Select(_ => _.FullName).FirstOrDefault();
                var _updatedBy = masterUserList.Where(_ => _.IDUser.ToLower() == d.UpdatedBy.ToLower()).Select(_ => _.FullName).FirstOrDefault();
                var _startLocation = masterLocationList.Where(_ => _.IDLocation == d.StartLocation).Select(_ => _.LocationName).FirstOrDefault();
                var _finishLocation = masterLocationList.Where(_ => _.IDLocation == d.FinishLocation).Select(_ => _.LocationName).FirstOrDefault();

                resultTE.Add(new TransportExecutionDataTableDTO
                {
                    SIStatus = d.SIStatus,
                    SIType = d.SIType,
                    SIWeek = d.SIWeek,
                    StartLocationID = d.StartLocation,
                    StartLocationName = _startLocation,
                    FinishLocationID = d.FinishLocation ?? "",
                    FinishLocationName = d.FinishLocation != null ? _finishLocation : "",
                    CreatedBy = _createdBy,
                    TransportationDate = d.TransportDate,
                    TransportationNumber = d.TransportNo,
                    TransportationStatus = d.TransportStatus,
                    IDTransportExecution = d.IDTransportExecution,
                    UpdatedBy = _updatedBy,
                    UpdatedDate = d.UpdatedDate,
                    VendorID = d.IDVendor,
                    VendorName = d.MasterVendor != null ? d.MasterVendor.VendorName : "",
                    IsActive = d.IsActive,
                    OrderCount = d.TransportOrders.Count,
                    //MaterialType = d.TransportOrders.SelectMany(x => x.TransportOrderDetails.Select(y => y.MaterialType)).Distinct().ToList()
                });
            }

            

            dynamic res = new System.Dynamic.ExpandoObject();
            res.data = resultTE;
            res.total = total;

            return res;
        }

        public List<TransportExecutionDTO> GetExecutionDataTableExportCustomReport(TransportOrderInput input, string userid)
        {
            TOMContextDB ctx = new TOMContextDB();

            var masterLocationList = ctx.MasterLocations.Where(x => x.IsActive).ToList();
            var masterUserList = ctx.MasterUsers.Where(x => x.IsActive).ToList();
            var driverList = ctx.TransportDriverManagements.Where(x => x.IsActive).ToList();

            var preq = ctx.TransportOrderDetails.Where(x => x.IsActive && x.TransportOrder.IsActive && x.TransportOrder.TransportExecution.IsActive);

            if (!input.IsRoleTransport)
            {
                var UserLocations = ctx.MasterUserLocationMappings.Where(f => f.IDUser == userid && f.IsActive).Select(f => f.IDLocation).ToList();
                var Users = ctx.MasterUserLocationMappings.Where(f => UserLocations.Contains(f.IDLocation)).Select(f => f.IDUser.ToLower()).Distinct();
                preq = preq.Where(c => Users.Contains(c.TransportOrder.TransportExecution.CreatedBy.ToLower()) || c.TransportOrder.TransportExecution.CreatedBy == userid);
            }

            if (input.dateFromFilter.HasValue)
                preq = preq.Where(c => c.TransportOrder.TransportExecution.TransportDate >= input.dateFromFilter.Value);
            if (input.dateToFilter.HasValue)
                preq = preq.Where(c => c.TransportOrder.TransportExecution.TransportDate <= input.dateToFilter.Value);
            if (input.transportNumberListFilter != null && input.transportNumberListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.transportNumberListFilter.Contains(c.TransportOrder.TransportExecution.TransportNo));
            if (input.transportModeListFilter != null && input.transportModeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.transportModeListFilter.Contains(c.TransportOrder.TransportExecution.TransportMode));
            if (input.siStatusListFilter != null && input.siStatusListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.siStatusListFilter.Contains(c.TransportOrder.TransportExecution.SIStatus));
            if (input.startLocationListFilter != null && input.startLocationListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.startLocationListFilter.Contains(c.TransportOrder.TransportExecution.StartLocation));
            if (input.transportStatusListFilter != null && input.transportStatusListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.transportStatusListFilter.Contains(c.TransportOrder.TransportExecution.TransportStatus));
            if (input.stoNoListFilter != null && input.stoNoListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.stoNoListFilter.Contains(c.TransportOrder.STONo));
            if (input.senderIdLocListFilter != null && input.senderIdLocListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.senderIdLocListFilter.Contains(c.TransportOrder.ActualSenderIDLocation));
            if (input.receiverIdLocListFilter != null && input.receiverIdLocListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.receiverIdLocListFilter.Contains(c.TransportOrder.ActualReceiverIDLocation));
            if (input.orderTypeListFilter != null && input.orderTypeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.orderTypeListFilter.Contains(c.TransportOrder.OrderType));
            if (input.materialTypeListFilter != null && input.materialTypeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
                preq = preq.Where(c => input.materialTypeListFilter.Contains(c.MaterialType));
            if (input.UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                preq = preq.Where(c => input.UserRole.Contains(c.TransportOrder.TransportExecution.TransportMode));
            }
            var temp = preq.ToList().OrderBy(x => x.TransportOrder.TransportExecution.TransportDate).ThenBy(x => x.TransportOrder.TransportExecution.TransportNo);

            var resultTE = temp.Select(
                d =>
                    new TransportExecutionDTO()
                    {
                        TransportNo = d.TransportOrder.TransportExecution.TransportNo,
                        TransportStatus = d.TransportOrder.TransportExecution.TransportStatus,
                        TransportDate = d.TransportOrder.TransportExecution.TransportDate,
                        StartLocation = masterLocationList.Where(loc => loc.IDLocation == d.TransportOrder.TransportExecution.StartLocation).Select(dt => dt.LocationName).FirstOrDefault(),
                        FinishLocation = d.TransportOrder.TransportExecution.FinishLocation != null ? masterLocationList.Where(loc => loc.IDLocation == d.TransportOrder.TransportExecution.FinishLocation).Select(dt => dt.LocationName).FirstOrDefault() : "",
                        SIStatus = d.TransportOrder.TransportExecution.SIStatus,
                        SIType = d.TransportOrder.TransportExecution.SIType,
                        SIWeek = d.TransportOrder.TransportExecution.SIWeek,
                        CreatedBy = masterUserList.Where(usr => usr.IDUser.ToLower() == d.TransportOrder.TransportExecution.CreatedBy.ToLower()).Select(dt => dt.FullName).FirstOrDefault(),
                        IDTransportExecution = d.TransportOrder.TransportExecution.IDTransportExecution,
                        UpdatedBy = masterUserList.Where(usr => usr.IDUser.ToLower() == d.TransportOrder.TransportExecution.UpdatedBy.ToLower()).Select(dt => dt.FullName).FirstOrDefault(),
                        IsActive = d.TransportOrder.TransportExecution.IsActive,

                        TransportCategory = d.TransportOrder.TransportExecution.TransportCategory,
                        ActualVehicleType = d.TransportOrder.TransportExecution.ActualVehicleType,
                        TransportMode = d.TransportOrder.TransportExecution.TransportMode,
                        VendorName = d.TransportOrder.TransportExecution.MasterVendor == null ? "" : d.TransportOrder.TransportExecution.MasterVendor.VendorName,
                        ServicePONo = (d.TransportOrder.TransportExecution.ServicePONo == null || d.TransportOrder.TransportExecution.ServicePONo == "NULL") ? "" : d.TransportOrder.TransportExecution.ServicePONo,
                        ServiceGRNo = (d.TransportOrder.TransportExecution.ServiceGRNo == null || d.TransportOrder.TransportExecution.ServicePONo == "NULL") ? "" : d.TransportOrder.TransportExecution.ServiceGRNo,
                        ActualCostCenter = d.TransportOrder.TransportExecution.ActualCostCenter == null ? "" : d.TransportOrder.TransportExecution.ActualCostCenter,
                        TotalKM = d.TransportOrder.TransportExecution.TotalKM,
                        TotalBox = d.TransportOrder.TransportExecution.TotalBox,
                        UpdatedDate = d.TransportOrder.TransportExecution.UpdatedDate,
                        Via = d.TransportOrder.TransportExecution.Via,
                        Month = d.TransportOrder.TransportExecution.Month,
                        Year = d.TransportOrder.TransportExecution.Year,
                        AdditionalCost = d.TransportOrder.TransportExecution.AdditionalCost,
                        ActualArrive = d.TransportOrder.TransportExecution.ActualArrive,
                        TargetOfArrival = d.TransportOrder.TransportExecution.TargetOfArrival,
                        PoliceRegistrationNumber = d.TransportOrder.TransportExecution.PoliceRegNo,
                        Driver1 = d.TransportOrder.TransportExecution.IDDriver1 == null ? "" : driverList.Where(x => x.ID == d.TransportOrder.TransportExecution.IDDriver1).Select(x => x.Name).FirstOrDefault(),
                        Driver2 = d.TransportOrder.TransportExecution.IDDriver2 == null ? "" : driverList.Where(x => x.ID == d.TransportOrder.TransportExecution.IDDriver2).Select(x => x.Name).FirstOrDefault(),
                        CoDriver = d.TransportOrder.TransportExecution.IDCoDriver == null ? "" : driverList.Where(x => x.ID == d.TransportOrder.TransportExecution.IDCoDriver).Select(x => x.Name).FirstOrDefault(),
                        DeliveryQty = d.TransportOrder.TransportExecution.DeliveryQty,
                        TotalKMRail = d.TransportOrder.TransportExecution.TotalKMRail,
                        TotalKMSea = d.TransportOrder.TransportExecution.TotalKMSea,
                        TotalKMBased = d.TransportOrder.TransportExecution.TotalKMBased,
                        Remarks = d.TransportOrder.TransportExecution.Remarks,
                        CreatedDate = d.TransportOrder.TransportExecution.CreatedDate,

                        TransportVesselCustomReport = new TransportVesselCustomReport
                        {
                            ContainerNoCustomReport = d.TransportOrder.TransportExecution.TransportVesselMonitorings.Select(x => x.ContainerNo).FirstOrDefault(),
                            ContainerSealCustomReport = d.TransportOrder.TransportExecution.TransportVesselMonitorings.Select(x => x.ContainerSeal).FirstOrDefault(),
                            VesselNameCustomReport = d.TransportOrder.TransportExecution.TransportVesselMonitorings.Select(x => x.VesselName).FirstOrDefault(),
                            ETD1 = d.TransportOrder.TransportExecution.TransportVesselMonitorings.Select(x => x.ETD1).FirstOrDefault(),
                            ETA1 = d.TransportOrder.TransportExecution.TransportVesselMonitorings.Select(x => x.ETA1).FirstOrDefault()
                        },

                        RouteNameCustomReport = string.Join("-", d.TransportOrder.TransportExecution.TransportRoutes
                            .Join(masterLocationList,
                            route => route.IDLocation,
                            location => location.IDLocation,
                            (route, location) => new { location.LocationName })
                            .Select(x => x.LocationName)),

                        IDTransportOrder = d.IDTransportOrder,
                        KMOrder = d.TransportOrder.KM,
                        STONo = d.TransportOrder.STONo,
                        OrderCategory = d.TransportOrder.OrderCategory,
                        VehicleType = d.TransportOrder.VehicleType,
                        Sender = d.TransportOrder.ActualSenderIDLocation != null ? masterLocationList.Where(x => x.IDLocation == d.TransportOrder.ActualSenderIDLocation).Select(x => x.LocationName).FirstOrDefault() : null,
                        Receiver = d.TransportOrder.ActualReceiverIDLocation != null ? masterLocationList.Where(x => x.IDLocation == d.TransportOrder.ActualReceiverIDLocation).Select(x => x.LocationName).FirstOrDefault() : null,
                        MasterCostCenter = Mapper.Map<MasterCostCenter, MasterCostCenterAccountDTO>(d.TransportOrder.MasterCostCenter),

                        MaterialType = d.MaterialType,
                        Description = d.Description,
                        UoM = d.UoM,
                        Qty = d.Qty
                    }
            ).ToList();

            return resultTE;
        }

        public List<TransportExecutionDTO> GetALLTransportationExecution(TransportOrderInput input, string userid)
        {
            List<TransportDriverManagementDTO> distinctDriverList = GetALLTransportDriverManagementList().GroupBy(x => x.ID).Select(x => x.First()).ToList();
            List<TransportVehicleDataDTO> distinctVehicleList = GetALLTransportVehicleDataList().GroupBy(x => x.IDPoliceRegNumber).Select(x => x.First()).ToList();
            TOMContextDB ctx = new TOMContextDB();

            List<TransportOrderDetailDTO> temp = _transportOrderDetailRepo.GetAllTransportOrderDetail(input);
            temp = temp.Where(x => x.TransportOrder.TransportExecution != null).ToList();

            if (input.orderCategoryListFilter != null && !String.IsNullOrEmpty(input.orderCategoryListFilter[0]))
            {
                temp = temp.Where(x => input.orderCategoryListFilter.Contains(x.TransportOrder.OrderCategory)).ToList();
            }
            var qry = ctx.TransportExecutions.Where(f => f.IsActive);

            if (input.dateFromFilter.HasValue)
                qry = qry.Where(f => f.TransportDate >= input.dateFromFilter.Value);
            if (input.dateToFilter.HasValue)
                qry = qry.Where(f => f.TransportDate <= input.dateToFilter.Value);

            var executionTemp = Mapper.Map<List<TransportExecutionEditDTO>>(qry);

            if (input.stoNoListFilter != null && !String.IsNullOrEmpty(input.stoNoListFilter[0]))
            {
                var OrdersTemp = executionTemp
                                .SelectMany(f => f.TransportOrders)
                                .Where(f => input.stoNoListFilter
                                .Contains(f.STONo))
                                .Select(f => f.IDTransportExecution)
                                .ToList();

                executionTemp = executionTemp.Where(f => OrdersTemp.Contains(f.IDTransportExecution)).ToList();
            }
            if (input.materialTypeListFilter != null && !String.IsNullOrEmpty(input.materialTypeListFilter[0]))
            {
                var OrderDetailsTemp = (from o in ctx.TransportOrders
                                        join od in ctx.TransportOrderDetails on o.IDTransportOrder equals od.IDTransportOrder
                                        where input.materialTypeListFilter.Contains(od.MaterialType) && o.IsActive && od.IsActive
                                        select o.IDTransportExecution).ToList();

                executionTemp = executionTemp.Where(f => OrderDetailsTemp.Contains(f.IDTransportExecution)).ToList();
            }
            if (input.senderIdLocListFilter != null && !String.IsNullOrEmpty(input.senderIdLocListFilter[0]))
            {
                var OrdersTemp = executionTemp
                                .SelectMany(f => f.TransportOrders)
                                .Where(f => input.senderIdLocListFilter
                                .Contains(f.ActualSenderIDLocation))
                                .Select(f => f.IDTransportExecution)
                                .ToList();

                executionTemp = executionTemp.Where(f => OrdersTemp.Contains(f.IDTransportExecution)).ToList();
            }
            if (input.receiverIdLocListFilter != null && !String.IsNullOrEmpty(input.receiverIdLocListFilter[0]))
            {
                var OrdersTemp = executionTemp
                                .SelectMany(f => f.TransportOrders)
                                .Where(f => input.receiverIdLocListFilter
                                .Contains(f.ActualReceiverIDLocation))
                                .Select(f => f.IDTransportExecution)
                                .ToList();

                executionTemp = executionTemp.Where(f => OrdersTemp.Contains(f.IDTransportExecution)).ToList();
            }
            if (input.transportNumberListFilter != null && !String.IsNullOrEmpty(input.transportNumberListFilter[0]))
            {
                executionTemp = executionTemp.Where(x => input.transportNumberListFilter.Contains(x.TransportNo)).ToList();
            }
            if (input.transportStatusListFilter != null && !String.IsNullOrEmpty(input.transportStatusListFilter[0]))
            {
                executionTemp = executionTemp.Where(x => input.transportStatusListFilter.Contains(x.TransportStatus)).ToList();
            }
            if (input.siStatusListFilter != null && !String.IsNullOrEmpty(input.siStatusListFilter[0]))
            {
                executionTemp = executionTemp.Where(x => input.siStatusListFilter.Contains(x.SIStatus)).ToList();
            }
            if (input.startLocationListFilter != null && !String.IsNullOrEmpty(input.startLocationListFilter[0]))
            {
                executionTemp = executionTemp.Where(x => input.startLocationListFilter.Contains(x.StartLocation)).ToList();
            }
            /*
            if (input.dateFromFilter != null && input.dateToFilter != null)
            {
                executionTemp = executionTemp.Where(x => x.TransportDate >= input.dateFromFilter && x.TransportDate <= input.dateToFilter).ToList();
            }
            */
            if (input.transportModeListFilter != null && !String.IsNullOrEmpty(input.transportModeListFilter[0]))
            {
                executionTemp = executionTemp.Where(x => input.transportModeListFilter.Contains(x.TransportMode)).ToList();
            }
            if (input.idTransportExecutionList != null)
            {
                executionTemp = executionTemp.Where(x => input.idTransportExecutionList.Contains(x.IDTransportExecution)).ToList();
            }

            var tempResult = (from te in executionTemp
                              join t in temp on te.IDTransportExecution equals t.TransportOrder.IDTransportExecution into rj1
                              from t in rj1.DefaultIfEmpty()
                              join l in ctx.MasterLocations on te.StartLocation equals l.IDLocation
                              join u in ctx.MasterUsers on te.CreatedBy.ToLower() equals u.IDUser.ToLower()
                              join u2 in ctx.MasterUsers on te.UpdatedBy.ToLower() equals u2.IDUser.ToLower()
                              join dr1 in distinctDriverList on te.IDDriver1 equals dr1.ID into lj1
                              from dr1 in lj1.DefaultIfEmpty()
                              join dr2 in distinctDriverList on te.IDDriver2 equals dr2.ID into lj2
                              from dr2 in lj2.DefaultIfEmpty()
                              join codr in distinctDriverList on te.IDCoDriver equals codr.ID into lj3
                              from codr in lj3.DefaultIfEmpty()
                              join vech in distinctVehicleList on te.PoliceRegNo equals vech.IDPoliceRegNumber into lj4
                              from vech in lj4.DefaultIfEmpty()
                              select new TransportExecutionDTO
                              {
                                  IDTransportExecution = te.IDTransportExecution,
                                  TransportNo = te.TransportNo,
                                  TransportDate = te.TransportDate,
                                  TransportCategory = te.TransportCategory,
                                  ActualVehicleType = te.ActualVehicleType,
                                  TransportMode = te.TransportMode,
                                  CreatedBy = u.FullName,
                                  CreatedByIDUser = te.CreatedBy,
                                  VendorName = te.MasterVendor == null ? "" : te.MasterVendor.VendorName,
                                  SIWeek = te.SIWeek,
                                  SIType = te.SIType,
                                  SIStatus = te.SIStatus,
                                  ServicePONo = te.ServicePONo,
                                  ServiceGRNo = te.ServiceGRNo,
                                  ActualCostCenter = te.ActualCostCenter,
                                  TotalKM = te.TotalKM,
                                  TotalBox = te.TotalBox,
                                  TransportStatus = te.TransportStatus,
                                  StartLocation = l.LocationName,
                                  UpdatedDate = te.UpdatedDate,
                                  UpdatedBy = u2.FullName,
                                  IsActive = te.IsActive,
                                  PoliceRegistrationNumber = vech == null ? "" : vech.IDPoliceRegNumber,
                                  PoliceRegNo = vech == null ? "" : vech.IDPoliceRegNumber,
                                  IDDriver1 = dr1 == null ? "" : dr1.ID,
                                  Driver1 = dr1 == null ? "" : dr1.Name,
                                  IDDriver2 = dr2 == null ? "" : dr2.ID,
                                  Driver2 = dr2 == null ? "" : dr2.Name,
                                  IDCoDriver = codr == null ? "" : codr.ID,
                                  CoDriver = codr == null ? "" : codr.Name,
                                  TransportVesselCustomReport = ctx.TransportVesselMonitorings.Where(x => x.IDTransportExecution == te.IDTransportExecution).Select(x => new TransportVesselCustomReport { VesselNameCustomReport = x.VesselName, ContainerNoCustomReport = x.ContainerNo, ContainerSealCustomReport = x.ContainerSeal }).FirstOrDefault(),
                                  RouteNameCustomReport = string.Join(",", te.TransportRoutes.Join(ctx.MasterLocations, tr => tr.IDLocation, ml => ml.IDLocation, (tr, ml) => ml.LocationName).ToList()),
                                  DeliveryQty = te.DeliveryQty,
                                  TotalCost = te.TotalCost,
                                  CFPLiter = te.CFPLiter,
                                  CFPTKM = te.CFPTKM,
                                  TotalKMSea = te.TotalKMSea,
                                  TotalKMRail = te.TotalKMRail,
                                  TotalKMBased = te.TotalKMBased,
                                  AVGLoadFactor = te.AVGLoadFactor,
                                  Remarks = te.Remarks,
                                  BasedPrice = te.BasedCost,
                                  CreatedDate = te.CreatedDate,
                                  Month = te.Month,
                                  Year = te.Year,
                                  TargetOfArrival = te.TargetOfArrival,
                                  AdditionalCost = te.AdditionalCost,
                                  Sender = t == null ? "" : t.TransportOrder.SenderLocationName,
                                  Receiver = t == null ? "" : t.TransportOrder.ReceiverLocationName,
                                  SerivceGRNo = te.ServiceGRNo,
                                  ActualArrive = te.ActualArrive,
                                  FinishLocation = te.FinishLocation,
                                  Via = te.Via
                              }
                         ).ToList();
            //var tempResult = (from t in temp
            //                  join te in ctx.TransportExecutions on t.TransportOrder.IDTransportExecution equals te.IDTransportExecution
            //                  join l in ctx.MasterLocations on te.StartLocation equals l.IDLocation
            //                  join u in ctx.MasterUsers on te.CreatedBy.ToLower() equals u.IDUser.ToLower()
            //                  join u2 in ctx.MasterUsers on te.UpdatedBy.ToLower() equals u2.IDUser.ToLower()
            //                               select new TransportExecutionDTO
            //                               {
            //                                   IDTransportExecution = te.IDTransportExecution,
            //                                   TransportNo = te.TransportNo,
            //                                   TransportDate = te.TransportDate,
            //                                   TransportCategory = te.TransportCategory,
            //                                   ActualVehicleType = te.ActualVehicleType,
            //                                   TransportMode = te.TransportMode,
            //                                   CreatedBy = u.FullName,
            //                                   CreatedByIDUser = te.CreatedBy,
            //                                   VendorName = te.MasterVendor == null ? "" : te.MasterVendor.VendorName,
            //                                   SIWeek = te.SIWeek,
            //                                   SIType = te.SIType,
            //                                   SIStatus = te.SIStatus,
            //                                   ServicePONo = te.ServicePONo,
            //                                   ServiceGRNo = te.ServiceGRNo,
            //                                   ActualCostCenter = te.ActualCostCenter,
            //                                   TotalKM = te.TotalKM,
            //                                   TotalBox = te.TotalBox,
            //                                   TransportStatus = te.TransportStatus,
            //                                   StartLocation = l.LocationName,
            //                                   UpdatedDate = te.UpdatedDate,
            //                                   UpdatedBy = u2.FullName,
            //                                   IsActive = te.IsActive                                               
            //                               }
            //             ).ToList();
            var retVal = from t in tempResult
                         group t by t.IDTransportExecution into g
                         select g.First();
            foreach (var v in retVal)
            {
                if (v.FinishLocation != null)
                {
                    var loc = ctx.MasterLocations.Where(l => l.IDLocation == v.FinishLocation).FirstOrDefault();

                    v.FinishLocation = loc != null ? loc.LocationName : null;
                }
            }
            //retVal = retVal.OrderByDescending(x => x.TransportDate)
            //       .ThenByDescending(x => x.TransportNo);//.ThenByDescending(x => x.STONo;

            List<TransportExecutionDTO> returnData = Mapper.Map<List<TransportExecutionDTO>>(retVal);
            if (!input.IsRoleTransport)
            {
                /*remarked by andre 2019-01-04
                List<string> listDistinctRegion = _masterLocationRepo.GetAllMasterLocationActiveByType("Warehouse").Where(x => input.userLocationList.Contains(x.IDLocation)).Select(x => x.ParentLocation).Distinct().ToList();
                List<string> tempIDUser = _transportExecutionRepo.GetDistinctUserByRegional(listDistinctRegion);
                tempIDUser.Add(userid);
                retVal = retVal.Where(x => tempIDUser.Contains(x.CreatedByIDUser)).ToList();
                */
                var UserLocations = ctx.MasterUserLocationMappings
                                    .Where(f => f.IDUser == userid && f.IsActive)
                                    .Select(f => f.IDLocation).ToList();

                returnData = Mapper.Map<List<TransportExecutionDTO>>(
                             from r in retVal
                             join ul in ctx.MasterUserLocationMappings on r.CreatedByIDUser.ToLower() equals ul.IDUser.ToLower()
                             where UserLocations.Contains(ul.IDLocation)
                             group r by new { r.IDTransportExecution }
                             into g
                             select g.FirstOrDefault()
                             ).OrderByDescending(f => f.TransportNo).ToList();
            }

            return returnData;
            //retVal = retVal.OrderByDescending(x => x.TransportNo);//.ThenByDescending(x => x.STONo;
            //return retVal.ToList();
        }

        public List<TransportExecutionAddNewDTO> GetTransportationExecutionAddNew(TransportOrderInput input)
        {
            return Mapper.Map<List<FN_TRANSPORT_EXECUTION_LIST_ADD_Result>, List<TransportExecutionAddNewDTO>>(_transportExecutionRepo.GetTransportExecutionAddNew(input));
        }

        public List<string> GenerateTransportationNumber(string idStartLocation, string transportMode, string userid, List<TransportExecutionAddNewDTO> toList)
        {
            List<string> listError = new List<string>();
            List<string> finishGoodList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.MasterialType.FinishedGood));
            List<string> rawMaterialList = _masterMappingRepo.GetMapFromByMapTo(EnumHelper.GetDescription(Enums.MasterialType.RawMaterial));
            List<TransportExecutionDTO> tempList = new List<TransportExecutionDTO>();
            Hashtable HashSeq = new Hashtable();
            foreach (int idCheck in toList.GroupBy(x => x.IDCheck).Select(x => x.FirstOrDefault().IDCheck))
            {
                // cek existing TO
                var idTO = toList.Where(x => x.IDCheck == idCheck).Select(x => x.IDTransportOrder).First();
                TransportOrder rowTO = _transportOrderRepo.GetTransportOrderByID(idTO.Value);
                if (rowTO.IDTransportExecution != null)
                {
                    string error = "Exists-" + rowTO.STONo;
                    listError.Add(error);
                }
                DateTime? tnDate = toList.Where(x => x.IDCheck == idCheck).OrderBy(x => x.ShipmentDate).Select(x => x.ShipmentDate).First();
                int checkFGList = toList.Where(x => x.IDCheck == idCheck && finishGoodList.Contains(x.MaterialType)).ToList().Count;
                int checkRMList = toList.Where(x => x.IDCheck == idCheck && rawMaterialList.Contains(x.MaterialType)).ToList().Count;
                int checkVechType = toList.Where(x => x.IDCheck == idCheck).GroupBy(x => x.OrderedVehicleType).Select(x => x.FirstOrDefault()).ToList().Count;
                int checkCostCenter = toList.Where(x => x.IDCheck == idCheck).GroupBy(x => x.CostCenter).Select(x => x.FirstOrDefault()).ToList().Count;

                if (tnDate != null)
                {
                    string month = tnDate.Value.Month.ToString().PadLeft(2, '0');
                    string year = tnDate.Value.Year.ToString().Substring(2);
                    MasterGenWeek week = _masterGenWeekRepo.GetGenWeekNowByDate(tnDate.Value);
                    string tempTn = "E-TN/" + year + week.Week + "/";
                    string transCategory = "Regular";
                    string vechType = "";
                    string costCenter = "";
                    if (toList.FirstOrDefault(x => string.IsNullOrEmpty(x.OrderNumber)) == null)
                        tempTn = "TN/" + year + month + "/";

                    if (toList.First().ShipmentDate.HasValue && toList.First().ShipmentDate.Value.Date >= (DateTime.Today + new TimeSpan(1, 0, 0, 0)))
                    {
                        // next day
                        //if (toList.Where(c => !string.IsNullOrWhiteSpace(c.OrderNumber)).Count() > 0)
                        if (toList.Where(x => x.IDCheck == idCheck && !String.IsNullOrEmpty(x.OrderNumber)).FirstOrDefault() != null)
                        {
                            // jika memiliki ON, keluarnya TN
                            tempTn = "TN/" + year + month + "/";
                        }
                    }
                    else
                    {
                        // selain next day
                        tempTn = "TN/" + year + month + "/";
                    }

                    if (checkFGList > 0 && checkRMList > 0)
                    {
                        transCategory = "Synergy";
                        tempTn = "TN/" + year + month + "/";

                        // sort da toList
                        var wtn = toList.Where(c => !string.IsNullOrWhiteSpace(c.OrderNumber)).ToList();
                        var wotn = toList.Where(c => string.IsNullOrWhiteSpace(c.OrderNumber)).ToList();
                        toList.Clear();
                        toList.AddRange(wtn);
                        toList.AddRange(wotn);

                        /*
                        if (toList.Count == 2)
                        {
                            if (rawMaterialList.Contains(toList[0].MaterialType) && finishGoodList.Contains(toList[1].MaterialType))
                            {
                                if (toList[0].ShipmentDate.HasValue && toList[0].ShipmentDate.Value.Date == DateTime.Today)
                                {
                                    if (toList[1].ShipmentDate.HasValue && toList[1].ShipmentDate.Value > DateTime.Today)
                                    {
                                        tempTn = "TN/" + year + month + "/";
                                    }
                                }
                            }
                            else if (rawMaterialList.Contains(toList[1].MaterialType) && finishGoodList.Contains(toList[0].MaterialType))
                            {
                                if (toList[1].ShipmentDate.HasValue && toList[1].ShipmentDate.Value.Date == DateTime.Today)
                                {
                                    if (toList[0].ShipmentDate.HasValue && toList[0].ShipmentDate.Value > DateTime.Today)
                                    {
                                        tempTn = "TN/" + year + month + "/";
                                    }
                                }
                            }
                        }
                        */
                    }

                    try
                    {
                        if (checkVechType == 1)
                            vechType = toList.Where(x => x.IDCheck == idCheck).Select(x => x.OrderedVehicleType).First();
                        if (checkCostCenter == 1)
                            costCenter = toList.Where(x => x.IDCheck == idCheck).Select(x => x.CostCenter).First();
                    }
                    catch { }

                    TransportExecutionDTO temp = new TransportExecutionDTO
                    {
                        IDCheckSave = idCheck,
                        TransportNo = tempTn,
                        TransportStatus = "In Process",
                        TransportDate = tnDate.Value,
                        TransportCategory = transCategory,
                        ActualVehicleType = vechType,
                        TransportMode = transportMode,
                        StartLocation = idStartLocation,
                        ActualCostCenter = costCenter,
                        SIType = "",
                        SIWeek = (byte?)week.Week,
                        Month = (byte?)tnDate.Value.Month,
                        Year = (int?)week.Year,
                        IsActive = true,
                        CreatedBy = userid,
                        UpdatedBy = userid
                    };
                    tempList.Add(temp);

                    foreach (var tempTO in toList.Where(x => x.IDCheck == idCheck).GroupBy(x => x.IDTransportOrder).Select(x => new { idSenderLoc = x.FirstOrDefault().IDSenderLoc, idReceiveLoc = x.FirstOrDefault().IDReceiverLoc, ShipmentDate = x.FirstOrDefault().ShipmentDate, orderNumber = x.FirstOrDefault().OrderNumber, idTransportOrder = x.FirstOrDefault().IDTransportOrder }))
                    {
                        var mstDistance = _masterDistanceRepo.GetMasterDistanceByField(tempTO.idSenderLoc, tempTO.idReceiveLoc, tempTO.ShipmentDate);
                        if (mstDistance == null)
                        {
                            //string error = "Distance-" + idCheck + "-" + tempTO.orderNumber;
                            //listError.Add(error);
                            HashSeq.Add(tempTO.idTransportOrder, 0);
                        }
                        else
                            HashSeq.Add(tempTO.idTransportOrder, Math.Round(mstDistance.Total.Value));
                    }
                    /*foreach (var checkSuggestVendor in toList.Where(x => x.IDCheck == idCheck).GroupBy(x => x.IDTransportOrder).Select( x => new { idReceiveLoc = x.FirstOrDefault().IDReceiverLoc, orderType = x.FirstOrDefault().OrderType, vehicleType = x.FirstOrDefault().OrderedVehicleType, orderNumber = x.FirstOrDefault().OrderNumber}))
                    {
                        if (_masterVendorSuggestionRepo.GetMasterVendorSuggestionByField(idStartLocation,checkSuggestVendor.idReceiveLoc, checkSuggestVendor.orderType, temp.TransportCategory,transportMode, checkSuggestVendor.vehicleType) == null)
                        {
                            string error = "Vendor-"+idCheck+"-"+checkSuggestVendor.orderNumber;
                            listError.Add(error);
                        }
                    }*/
                }
            }
            if (listError.Count < 1)
                listError = _transportExecutionRepo.GenerateTransportationNumber(tempList, toList, HashSeq);
            return listError;
        }

        public void SetActiveTransportExecution(List<int> idTransporExe, string userid)
        {
            foreach (int id in idTransporExe)
            {
                TransportExecution temp = _transportExecutionRepo.GetTransportExecutionByID(id);
                List<TransportOrder> dataTO = _transportOrderRepo.GetTransportOrderFilterByTE(id);
                foreach (TransportOrder tempTO in dataTO)
                {
                    TransportOrder rowTO = _transportOrderRepo.GetTransportOrderByID(tempTO.IDTransportOrder);
                    if (rowTO != null)
                    {
                        rowTO.IDTransportExecution = null;
                        rowTO.OrderStatus = "Submit";
                        rowTO.UpdatedBy = userid;
                        _transportOrderRepo.SaveData(rowTO);
                    }
                }
                temp.IsActive = false;
                temp.UpdatedBy = userid;
                _transportExecutionRepo.SaveData(temp);
            }
        }

        //public decimal CalculateDistance(string distanceType, DateTime transDate, string transportMode, List<TransportRouteDTO> routes = null)
        //{
        //    if (routes == null)
        //    {
        //        return 0;
        //    }

        //    var _idloc = routes.Where(_ => String.IsNullOrEmpty(_.LocationName)).Select(_ => _.IDLocation).ToList();
        //    var getLocName = _masterLocationRepo.Get(_ => _idloc.Contains(_.IDLocation));

        //    foreach (var rute in routes)
        //    {
        //        if (String.IsNullOrEmpty(rute.LocationName))
        //        {
        //            var _locName = getLocName.FirstOrDefault(_ => _.IDLocation == rute.IDLocation);
        //            rute.LocationName = _locName == null ? "" : _locName.LocationName;
        //        }
        //    }

        //    routes = routes.Where(_ => !_.LocationName.ToUpper().Contains("AOT ")).ToList();
        //    decimal _totalKM = 0;
        //    for (int i = 1; i < routes.Count; i++)
        //    {
        //        var _from = routes[i - 1].IDLocation;
        //        var _to = routes[i].IDLocation;
        //        if (_from == _to || (routes[i].IsNonKMBased.Value && routes[i - 1].IsNonKMBased.Value)) continue;

        //        distanceType = "KM Based";

        //        if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Ship))
        //        {
        //            distanceType = EnumHelper.GetDescription(Enums.TransportationMode.Sea);
        //        }
        //        else if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Train))
        //        {
        //            distanceType = EnumHelper.GetDescription(Enums.TransportationMode.Rail);
        //        }

        //        var getDistance = _distanceRepo.GetOne(_ => _.IDSender == _from && _.IDReceiver == _to && _.DistanceType == distanceType && _.EffectiveStartDate <= transDate && _.EffectiveEndDate >= transDate && _.IsActive);

        //        if (getDistance != null)
        //        {
        //            _totalKM += (decimal)(getDistance.Total == null ? 0 : getDistance.Total);
        //        }
        //    }

        //    return _totalKM;
        //}

        public decimal CalculateDistance(string vendorCategory, DateTime transDate, string transportMode, List<TransportRouteDTO> routes = null)
        {
            List<string> tempList = new List<string>();
            if (routes == null)
            {
                return 0;
            }

            var _idloc = routes.Where(_ => String.IsNullOrEmpty(_.LocationName)).Select(_ => _.IDLocation).ToList();
            var getLocName = _masterLocationRepo.Get(_ => _idloc.Contains(_.IDLocation));

            foreach (var rute in routes)
            {
                if (String.IsNullOrEmpty(rute.LocationName))
                {
                    var _locName = getLocName.FirstOrDefault(_ => _.IDLocation == rute.IDLocation);
                    rute.LocationName = _locName == null ? "" : _locName.LocationName;
                }
            }

            //routes = routes.Where(_ => _.IsMain).ToList();
            routes = routes.Where(_ => !_.LocationName.ToUpper().Contains("AOT ")).ToList();
            decimal land = 0;
            decimal total;

            bool nonKM;

            MasterDistance temp = new MasterDistance();

            for (int i = 0; i < routes.Count - 1; i++)
            {
                nonKM = false;
                var _from = routes[i].IDLocation;
                var _to = routes[i+1].IDLocation;
                if (_from == _to || (routes[i].IsNonKMBased.Value && routes[i + 1].IsNonKMBased.Value) )
                {
                    if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Ship))
                    {
                        vendorCategory = EnumHelper.GetDescription(Enums.TransportationMode.Sea);
                    }
                    else if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Train))
                    {
                        vendorCategory = EnumHelper.GetDescription(Enums.TransportationMode.Rail);
                    }
                    temp = _masterDistanceRepo.GetMasterDistanceActiveByTypeSenderReceiverModeVia(vendorCategory, transDate, routes[i].IDLocation, routes[i + 1].IDLocation, "", "");
                    nonKM = true;
                }
                else
                {
                    temp = _masterDistanceRepo.GetMasterDistanceActiveByTypeSenderReceiverModeVia(vendorCategory, transDate, routes[i].IDLocation, routes[i + 1].IDLocation, "", "");
                }
                if (temp != null)
                {
                    total = 0;
                    if (temp.Total != null)
                        total = temp.Total.Value;
                    if (!nonKM)
                    {
                        land = land + total;
                    }
                }
                else
                {
                    land = land + 0;
                }
            }

            return Math.Round(land);
        }

        public List<String> CalculateExecutionDistance(string vendorCategory, DateTime transDate, string transportMode, List<TransportRouteDTO> routes = null)
        {
            List<string> tempList = new List<string>();
            if (routes == null)
            {
                return tempList;
            }

            var _idloc = routes.Where(_ => String.IsNullOrEmpty(_.LocationName)).Select(_ => _.IDLocation).ToList();
            var getLocName = _masterLocationRepo.Get(_ => _idloc.Contains(_.IDLocation));

            foreach (var rute in routes)
            {
                if (String.IsNullOrEmpty(rute.LocationName))
                {
                    var _locName = getLocName.FirstOrDefault(_ => _.IDLocation == rute.IDLocation);
                    rute.LocationName = _locName == null ? "" : _locName.LocationName;
                }
            }

            //routes = routes.Where(_ => _.IsMain).ToList();
            routes = routes.Where(_ => !_.LocationName.ToUpper().Contains("AOT ")).ToList();
            decimal land = 0;
            decimal sea = 0;
            decimal train = 0;
            decimal total;

            bool nonKM;
            
            MasterDistance temp = new MasterDistance();

            for (int i = 0; i < routes.Count - 1; i++)
            {
                nonKM = false;
                var _from = routes[i].IDLocation;
                var _to = routes[i + 1].IDLocation;
                if (_from == _to || (routes[i].IsNonKMBased.Value && routes[i + 1].IsNonKMBased.Value))
                {
                    if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Ship))
                    {
                        vendorCategory = EnumHelper.GetDescription(Enums.TransportationMode.Sea);
                    }
                    else if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Train))
                    {
                        vendorCategory = EnumHelper.GetDescription(Enums.TransportationMode.Rail);
                    }
                    temp = _masterDistanceRepo.GetMasterDistanceActiveByTypeSenderReceiverModeVia(vendorCategory, transDate, routes[i].IDLocation, routes[i + 1].IDLocation, "", "");
                    nonKM = true;
                }
                else
                {
                    temp = _masterDistanceRepo.GetMasterDistanceActiveByTypeSenderReceiverModeVia(vendorCategory, transDate, routes[i].IDLocation, routes[i + 1].IDLocation, "", "");
                }
                if (temp != null)
                {
                    total = 0;
                    if (temp.Total != null)
                        total = temp.Total.Value;
                    if (nonKM)
                    {
                        if (vendorCategory == EnumHelper.GetDescription(Enums.TransportationMode.Sea))
                        {
                            sea = sea + total;
                        }
                        else if (vendorCategory == EnumHelper.GetDescription(Enums.TransportationMode.Rail))
                        {
                            train = train + total;
                        }
                    }
                    else
                    {
                        land = land + total;
                    }
                }
                else
                {
                    tempList.Add("Error-" + routes[i].LocationName + "-" + routes[i + 1].LocationName);
                }
            }

            tempList.Add("Distance-Land-" + Math.Round(land));
            tempList.Add("Distance-Sea-" + Math.Round(sea));
            tempList.Add("Distance-Train-" + Math.Round(train));
            return tempList;
        }

        public List<String> CalculateTransportExecutionDistance(string vendorCategory, DateTime transDate, string transportMode, List<TransportRouteDTO> transRoute, string via)
        {
            transRoute = transRoute.Where(_ => _.IsMain).ToList();
            decimal land = 0;
            decimal sea = 0;
            decimal train = 0;
            decimal total;
            
            bool nonKM;
            List<string> tempList = new List<string>();
            MasterDistance temp = new MasterDistance();
            
            for (int i = 0; i < transRoute.Count - 1; i++)
            {
                nonKM = false;
                if (transRoute[i].IsNonKMBased.Value && transRoute[i + 1].IsNonKMBased.Value)
                {
                    if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Ship))
                    {
                        vendorCategory = EnumHelper.GetDescription(Enums.TransportationMode.Sea);
                    }
                    else if (transportMode == EnumHelper.GetDescription(Enums.TransportationMode.Train))
                    {
                        vendorCategory = EnumHelper.GetDescription(Enums.TransportationMode.Rail);
                    }
                    temp = _masterDistanceRepo.GetMasterDistanceActiveByTypeSenderReceiverModeVia(vendorCategory, transDate, transRoute[i].IDLocation, transRoute[i + 1].IDLocation, "", "");
                    nonKM = true;
                }
                else
                {
                    temp = _masterDistanceRepo.GetMasterDistanceActiveByTypeSenderReceiverModeVia(vendorCategory, transDate, transRoute[i].IDLocation, transRoute[i + 1].IDLocation, "", "");
                }
                if (temp != null)
                {
                    total = 0;
                    if (temp.Total != null)
                        total = temp.Total.Value;
                    if (nonKM)
                    {
                        if (vendorCategory == EnumHelper.GetDescription(Enums.TransportationMode.Sea))
                        {
                            sea = sea + total;
                        }
                        else if (vendorCategory == EnumHelper.GetDescription(Enums.TransportationMode.Rail))
                        {
                            train = train + total;
                        }
                    }
                    else
                    {
                        land = land + total;
                    }
                }
                else
                {
                    tempList.Add("Error-" + transRoute[i].LocationName + "-" + transRoute[i + 1].LocationName);
                }
            }

            tempList.Add("Distance-Land-" + Math.Round(land));
            tempList.Add("Distance-Sea-" + Math.Round(sea));
            tempList.Add("Distance-Train-" + Math.Round(train));
            return tempList;
        }

        public List<string> CheckSaveData(TransportExecutionDTO data, List<TransportRouteDTO> saveTransRoute, List<TransportOrderDTO> saveTransOrder, TransportVesselMonitoringDTO saveTransVessel, TransportVendorChangeLogDTO saveVendorLog, string userid, DateTime latestDateEdit)
        {
            List<string> listError = new List<string>();
            TransportVehicleDataDTO vehicle = Mapper.Map<TransportVehicleData, TransportVehicleDataDTO>(_transportVehicleDataRepo.GetTransportVehicleData(data.PoliceRegNo));
            TransportDriverManagementDTO driver1 = Mapper.Map<TransportDriverManagement, TransportDriverManagementDTO>(_transportDriverManagementRepo.GetTransportDriverManagement(data.IDDriver1));
            TransportDriverManagementDTO driver2 = Mapper.Map<TransportDriverManagement, TransportDriverManagementDTO>(_transportDriverManagementRepo.GetTransportDriverManagement(data.IDDriver2));
            TransportDriverManagementDTO codriver = Mapper.Map<TransportDriverManagement, TransportDriverManagementDTO>(_transportDriverManagementRepo.GetTransportDriverManagement(data.IDCoDriver));
            if (vehicle != null && vehicle.STNKValidityPeriod < data.TransportDate)
                listError.Add("STNKInvalid-" + data.PoliceRegNo);
            if (driver1 != null && driver1.DrivingLicensePeriod < data.TransportDate)
                listError.Add("SIM1Invalid-" + driver1.Name);
            if (driver2 != null && driver2.DrivingLicensePeriod < data.TransportDate)
                listError.Add("SIM2Invalid-" + driver2.Name);
            if (codriver != null && codriver.DrivingLicensePeriod < data.TransportDate)
                listError.Add("SIM3Invalid-" + codriver.Name);
            if (listError.Count < 1)
            {
                TransportExecutionDTO checkDriver1 = Mapper.Map<TransportExecution, TransportExecutionDTO>(_transportExecutionRepo.GetTransportExecutionByIDDriver(data.IDDriver1, data.IDTransportExecution));
                TransportExecutionDTO checkDriver2 = Mapper.Map<TransportExecution, TransportExecutionDTO>(_transportExecutionRepo.GetTransportExecutionByIDDriver(data.IDDriver2, data.IDTransportExecution));
                TransportExecutionDTO checkCoDriver = Mapper.Map<TransportExecution, TransportExecutionDTO>(_transportExecutionRepo.GetTransportExecutionByIDDriver(data.IDCoDriver, data.IDTransportExecution));
                if (checkDriver1 != null && driver1 != null)
                    listError.Add("Driver1Used-" + driver1.Name + "-" + checkDriver1.TransportNo);
                if (checkDriver2 != null && driver2 != null)
                    listError.Add("Driver2Used-" + driver2.Name + "-" + checkDriver2.TransportNo);
                if (checkCoDriver != null && codriver != null)
                    listError.Add("Driver3Used-" + codriver.Name + "-" + checkCoDriver.TransportNo);
                if (listError.Count < 1)
                {
                    listError.AddRange(SaveData(data, saveTransRoute, saveTransOrder, saveTransVessel, saveVendorLog, userid, latestDateEdit));
                }
            }
            return listError;
        }

        public List<string> SaveData(TransportExecutionDTO data, List<TransportRouteDTO> saveTransRoute, List<TransportOrderDTO> saveTransOrder, TransportVesselMonitoringDTO saveTransVessel, TransportVendorChangeLogDTO saveVendorLog, string userid, DateTime latestDateEdit)
        {
            string tempTransportStatus = "In Process";
            List<string> listError = new List<string>();
            TransportExecution temp = _transportExecutionRepo.GetTransportExecutionActiveByID(data.IDTransportExecution);
            if (temp.UpdatedDate != latestDateEdit)
            {
                listError.Add("Modified-" + temp.UpdatedBy);
            }
            else
            {
                //dipindah atau di taruh di atas untuk menghitung total box
                #region Save Transport Order

                //transport order setidaknya ada satu
                if (saveTransOrder != null)
                {
                    List<TransportOrder> tempUnreferenceToList = temp.TransportOrders.Where(x => saveTransOrder.All(y => y.IDTransportOrder != x.IDTransportOrder)).ToList();
                    foreach (TransportOrder tempUnreferencedTo in tempUnreferenceToList)
                    {
                        TransportOrder unreferencedTo = _transportOrderRepo.GetTransportOrderByID(tempUnreferencedTo.IDTransportOrder);
                        unreferencedTo.IDTransportExecution = null;
                        if (unreferencedTo.OrderStatus == "In Process")
                            unreferencedTo.OrderStatus = "Submit";
                        unreferencedTo.OrderCategory = "";
                        unreferencedTo.KM = null;
                        _transportOrderRepo.Update(unreferencedTo);
                        _transportOrderRepo.Save();
                    }

                    List<TransportOrderDTO> tempReferenceToList = saveTransOrder;

                    if (temp.TransportOrders.Any())
                    {
                        //decimal totalKM = 0;
                        List<TransportOrderDTO> tempUpdateList = saveTransOrder.Where(x => temp.TransportOrders.Any(y => y.IDTransportOrder == x.IDTransportOrder)).ToList();
                        foreach (TransportOrderDTO tempUpdateTo in tempUpdateList)
                        {
                            TransportOrder updateTo = _transportOrderRepo.GetTransportOrderByID(tempUpdateTo.IDTransportOrder);
                            //var mstDistance = _masterDistanceRepo.GetMasterDistanceByField(updateTo.ActualSenderIDLocation, updateTo.ActualReceiverIDLocation, updateTo.ShipmentDate, data.VendorCategory);
                            //if (mstDistance != null) totalKM = Convert.ToDecimal(mstDistance.Total);
                            updateTo.OrderCategory = tempUpdateTo.OrderCategory;
                            //updateTo.KM = totalKM;
                            _transportOrderRepo.Update(updateTo);
                            _transportOrderRepo.Save();
                        }

                        tempReferenceToList = saveTransOrder.Where(x => temp.TransportOrders.All(y => y.IDTransportOrder != x.IDTransportOrder)).ToList();
                    }

                    if (tempReferenceToList != null)
                    {
                        //decimal totalKM = 0;
                        foreach (TransportOrderDTO tempReferencedTo in tempReferenceToList)
                        {
                            TransportOrder referencedTo = _transportOrderRepo.GetTransportOrderByID(tempReferencedTo.IDTransportOrder);
                            //var mstDistance = _masterDistanceRepo.GetMasterDistanceByField(referencedTo.ActualSenderIDLocation, referencedTo.ActualReceiverIDLocation, referencedTo.ShipmentDate, data.VendorCategory);
                            //if (mstDistance != null) totalKM = Convert.ToDecimal(mstDistance.Total);
                            referencedTo.IDTransportExecution = data.IDTransportExecution;
                            if (referencedTo.OrderStatus == "Submit")
                                referencedTo.OrderStatus = "In Process";
                            //referencedTo.KM = totalKM;
                            referencedTo.OrderCategory = tempReferencedTo.OrderCategory;
                            _transportOrderRepo.Update(referencedTo);
                            _transportOrderRepo.Save();
                        }
                    }

                    data.TotalBox = (int?)saveTransOrder.Sum(x => x.TotalBox);

                    if (saveTransOrder.Where(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.Draft)).FirstOrDefault() != null)
                    {
                        tempTransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.Draft);
                    }
                    else if (saveTransOrder.Where(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.Submit)).FirstOrDefault() != null)
                    {
                        tempTransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.Submit);
                    }
                    else if (saveTransOrder.Where(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.InProcess)).FirstOrDefault() != null)
                    {
                        tempTransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.InProcess);
                    }
                    else if (saveTransOrder.Where(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.OnDeliver)).FirstOrDefault() != null)
                    {
                        tempTransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.OnDeliver);
                    }
                    else if (saveTransOrder.Where(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.Arrive)).FirstOrDefault() != null)
                    {
                        tempTransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.Arrive);
                    }
                    else if (saveTransOrder.Where(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.Complete)).FirstOrDefault() != null)
                    {
                        tempTransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.Complete);
                    }
                    else if (saveTransOrder.Where(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.Close)).FirstOrDefault() != null)
                    {
                        tempTransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.Close);
                    }
                }
                else//semua TO di hapus dari TE
                {
                    foreach (TransportOrder tempUnreferencedTo in temp.TransportOrders)
                    {
                        TransportOrder unreferencedTo = _transportOrderRepo.GetTransportOrderByID(tempUnreferencedTo.IDTransportOrder);
                        unreferencedTo.IDTransportExecution = null;
                        if (unreferencedTo.OrderStatus == "In Process")
                            unreferencedTo.OrderStatus = "Submit";
                        unreferencedTo.OrderCategory = "";
                        _transportOrderRepo.Update(unreferencedTo);
                        _transportOrderRepo.Save();
                    }
                }

                #endregion

                #region Hitung Total KM
                #region OLD LOGIC

                decimal? totalKM = 0;
                //List<String> hasilTotalKM = CalculateTransportExecutionDistance("KM Based", data.TransportDate, data.TransportMode, saveTransRoute, data.Via);
                //foreach (string hasil in hasilTotalKM)
                //{
                //    string[] split = hasil.Split('-');
                //    if (split[1] == "Land")
                //    {
                //        data.KMBased = Convert.ToDecimal(split[2]);
                //        totalKM = totalKM + data.KMBased;
                //    }
                //    else if (split[1] == "Sea")
                //    {
                //        data.KMSea = Convert.ToDecimal(split[2]);
                //        totalKM = totalKM + data.KMSea;
                //    }
                //    else if (split[1] == "Train")
                //    {
                //        data.KMRail = Convert.ToDecimal(split[2]);
                //        totalKM = totalKM + data.KMRail;
                //    }
                //}

                List<String> hasilTotalKM = CalculateExecutionDistance("KM Based", data.TransportDate, data.TransportMode, saveTransRoute);
                foreach (string hasil in hasilTotalKM)
                {
                    string[] split = hasil.Split('-');
                    if (split[1] == "Land")
                    {
                        data.KMBased = Convert.ToDecimal(split[2]);
                        totalKM = totalKM + data.KMBased;
                    }
                    else if (split[1] == "Sea")
                    {
                        data.KMSea = Convert.ToDecimal(split[2]);
                        totalKM = totalKM + data.KMSea;
                    }
                    else if (split[1] == "Train")
                    {
                        data.KMRail = Convert.ToDecimal(split[2]);
                        totalKM = totalKM + data.KMRail;
                    }
                }
                #endregion
                //var hasilTotalKM = CalculateDistance("KM Based", data.TransportDate, data.TransportMode, saveTransRoute);
                data.TotalKM = totalKM;
                #endregion

                //List<String> hasilTota = CalculateExecutionDistance("KM Based", data.TransportDate, data.TransportMode, saveTransRoute);
                //foreach (string hasil in hasilTota)
                //{
                //    string[] split = hasil.Split('-');
                //    if (split[1] == "Land")
                //    {
                //        data.KMBased = Convert.ToDecimal(split[2]);
                //    }
                //    else if (split[1] == "Sea")
                //    {
                //        data.KMSea = Convert.ToDecimal(split[2]);
                //    }
                //    else if (split[1] == "Train")
                //    {
                //        data.KMRail = Convert.ToDecimal(split[2]);
                //    }
                //}

                #region Save Transport Execution
                MasterGenWeek week = _masterGenWeekRepo.GetGenWeekNowByDate(data.TransportDate);
                temp.TransportNo = data.TransportNo;
                temp.TransportDate = data.TransportDate;
                temp.TransportCategory = data.TransportCategory;
                temp.ActualVehicleType = data.ActualVehicleType;
                temp.TransportMode = data.TransportMode;
                temp.TransportStatus = tempTransportStatus;
                temp.IDVendor = data.IDVendor;
                temp.SIType = data.SIType;
                temp.SIWeek = Byte.Parse(week.Week.Value.ToString());
                temp.Month = (byte)data.TransportDate.Month;
                temp.Year = (int)data.TransportDate.Year;
                temp.SIStatus = data.SIStatus;
                temp.TargetOfArrival = data.TargetOfArrival;
                temp.ActualArrive = data.ActualArrive;
                temp.StartLocation = data.StartLocation;
                temp.FinishLocation = data.FinishLocation;
                temp.ServicePONo = data.ServicePONo;
                temp.IDCostCenter = data.IDCostCenter;
                temp.ActualCostCenter = data.ActualCostCenter;
                if (data.PoliceRegNo == "-")
                    temp.PoliceRegNo = null;
                else
                    temp.PoliceRegNo = data.PoliceRegNo;
                if (data.IDDriver1 == "-")
                    temp.IDDriver1 = null;
                else
                    temp.IDDriver1 = data.IDDriver1;
                if (data.IDDriver2 == "-")
                    temp.IDDriver2 = null;
                else
                    temp.IDDriver2 = data.IDDriver2;
                if (data.IDCoDriver == "-")
                    temp.IDCoDriver = null;
                else
                    temp.IDCoDriver = data.IDCoDriver;
                temp.TotalKM = Convert.ToInt32(data.TotalKM);
                temp.TotalKMRail = Convert.ToInt32(data.KMRail);
                temp.TotalKMBased = Convert.ToInt32(data.KMBased);
                temp.TotalKMSea = Convert.ToInt32(data.KMSea);
                //temp.TotalKMBased = Convert.ToInt32(data.TotalKM);
                temp.TotalBox = Convert.ToInt32(data.TotalBox);
                temp.AdditionalCost = data.AdditionalCost;
                temp.Via = data.Via;
                temp.ServiceGRNo = data.ServiceGRNo;
                temp.Remarks = data.Remarks;
                temp.UpdatedBy = userid;
                temp.UpdatedDate = DateTime.Now;
                _transportExecutionRepo.Update(temp);
                _transportExecutionRepo.Save();

                _transportExecutionRepo.RefreshTransportStatus(data.IDTransportExecution);
                #endregion

                #region Save Transport Vendor Change Log

                if (saveVendorLog.IDVendor != 0)
                {
                    saveVendorLog.IDTransportExecution = data.IDTransportExecution;
                    saveVendorLog.IsActive = true;
                    saveVendorLog.CreatedBy = userid;
                    saveVendorLog.CreatedDate = DateTime.Now;
                    saveVendorLog.UpdatedBy = userid;
                    saveVendorLog.UpdatedDate = DateTime.Now;
                    _transportVendorChangeLogRepo.Insert(Mapper.Map<TransportVendorChangeLogDTO, TransportVendorChangeLog>(saveVendorLog));
                    _transportVendorChangeLogRepo.Save();
                }

                #endregion

                #region Save Transport Vessel

                if (temp.TransportMode == EnumHelper.GetDescription(Enums.TransportationMode.Ship)
                   || temp.TransportMode == EnumHelper.GetDescription(Enums.TransportationMode.Train))
                {
                    TransportVesselMonitoring tempTransVessel =
                        _transportVesselMonitoringRepo.GetTransportVesselMonitoringByIDTransactionExecution(data
                            .IDTransportExecution);

                    if (temp.TransportMode == EnumHelper.GetDescription(Enums.TransportationMode.Train))
                    {
                        saveTransVessel.VesselName = "";
                    }

                    if (tempTransVessel == null)
                    {
                        if (string.IsNullOrEmpty(saveTransVessel.ContainerNo))
                            saveTransVessel.ContainerNo = "";
                        if (string.IsNullOrEmpty(saveTransVessel.ContainerSeal))
                            saveTransVessel.ContainerSeal = "";
                        saveTransVessel.IsActive = true;
                        saveTransVessel.CreatedBy = userid;
                        saveTransVessel.CreatedDate = DateTime.Now;
                        saveTransVessel.UpdatedBy = userid;
                        saveTransVessel.UpdatedDate = DateTime.Now;
                        _transportVesselMonitoringRepo.Insert(Mapper
                            .Map<TransportVesselMonitoringDTO, TransportVesselMonitoring>(saveTransVessel));
                        _transportVesselMonitoringRepo.Save();
                    }
                    else
                    {
                        tempTransVessel.VesselName = saveTransVessel.VesselName;
                        tempTransVessel.ETD1 = saveTransVessel.ETD1;
                        tempTransVessel.ETA1 = saveTransVessel.ETA1;
                        if (saveTransVessel.ETA1.HasValue)
                        {
                            MasterConfigurationDTO configVessel = (Mapper.Map<List<MasterConfigurationDTO>>(_masterConfigurationRepo.GetListMasterConfigurationByPageNameDescription(EnumHelper.GetDescription(Enums.MasterConfigurationValue.VesselMonitoringDetail), "est. received"))).First();
                            tempTransVessel.EstReceived = saveTransVessel.ETA1.Value.AddDays(Convert.ToInt16(configVessel.Value));
                        }
                        //tempTransVessel.ContainerNo = saveTransVessel.ContainerNo;
                        if (string.IsNullOrEmpty(saveTransVessel.ContainerNo))
                            tempTransVessel.ContainerNo = "";
                        else
                            tempTransVessel.ContainerNo = saveTransVessel.ContainerNo;
                        if (string.IsNullOrEmpty(saveTransVessel.ContainerSeal))
                            tempTransVessel.ContainerSeal = "";
                        else
                            tempTransVessel.ContainerSeal = saveTransVessel.ContainerSeal;
                        tempTransVessel.UpdatedBy = userid;
                        tempTransVessel.UpdatedDate = DateTime.Now;
                        _transportVesselMonitoringRepo.Update(tempTransVessel);
                        _transportVesselMonitoringRepo.Save();
                    }
                }
                else//transport vessel di delete
                {
                    TransportVesselMonitoring tempTransVessel = _transportVesselMonitoringRepo.GetTransportVesselMonitoringByIDTransactionExecution(data.IDTransportExecution);
                    if (tempTransVessel != null)
                    {
                        _transportVesselMonitoringRepo.Delete(tempTransVessel);
                        _transportVesselMonitoringRepo.Save();
                    }
                }

                #endregion

                #region Save Transport Route

                List<TransportRoute> tempRouteList = _transportRouteRepo.GetTransportRouteByIDTransactionExecution(data.IDTransportExecution);
                string tempRouteCreatedBy = "";
                DateTime tempRouteCreatedDate = DateTime.Now;
                foreach (TransportRoute tempRouteDelete in tempRouteList)
                {
                    tempRouteCreatedBy = tempRouteDelete.CreatedBy;
                    tempRouteCreatedDate = tempRouteDelete.CreatedDate;
                    _transportRouteRepo.Delete(tempRouteDelete);
                    _transportRouteRepo.Save();
                }
                if (saveTransRoute != null)
                {
                    foreach (TransportRouteDTO tempRouteSave in saveTransRoute)
                    {
                        //jika data baru, create by nya user id
                        if (string.IsNullOrEmpty(tempRouteCreatedBy))
                        {
                            tempRouteCreatedBy = userid;
                        }
                        TransportRoute SaveRoute = new TransportRoute();
                        SaveRoute.IDTransportExecution = temp.IDTransportExecution;
                        SaveRoute.IDLocation = tempRouteSave.IDLocation;
                        SaveRoute.IsMain = tempRouteSave.IsMain;
                        SaveRoute.IsNonKMBased = tempRouteSave.IsNonKMBased;
                        SaveRoute.CreatedBy = tempRouteCreatedBy;
                        SaveRoute.CreatedDate = tempRouteCreatedDate;
                        SaveRoute.UpdatedBy = userid;
                        SaveRoute.UpdatedDate = DateTime.Now;
                        _transportRouteRepo.Insert(SaveRoute);
                        _transportRouteRepo.Save();
                    }
                }

                #endregion

                #region Calculate Carbon Foot Print
                _transportExecutionRepo.CalculateLoadFactorCFP(data.IDTransportExecution);
                #endregion

                //#region Calculate Cost
                _transportExecutionRepo.CalculateCost(data.IDTransportExecution.ToString(), userid, 2);
                //#endregion
            }
            return listError;
        }

        #region SUB EXPORT
        public List<TransportExecutionDTO> GetExportXls(string fltrtn, string fltron, string fltroc, string fltrsi, string fltrot, string fltrzo, string fltrdf, string fltrdt, string fltrsl, string fltrmt, string fltrse, string fltrre)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        from TO in LJOIN_1.DefaultIfEmpty()
                        join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder into LJOIN_2
                        from TOD in LJOIN_2.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_1
                        from MV in RJOIN_1.DefaultIfEmpty()
                        join MLTE in context.MasterLocations on TE.StartLocation equals MLTE.IDLocation into RJOIN_2
                        from MLTE in RJOIN_2.DefaultIfEmpty()
                        join MLTOS in context.MasterLocations on TO.SenderIDLocation equals MLTOS.IDLocation into RJOIN_3
                        from MLTOS in RJOIN_3.DefaultIfEmpty()
                        join MLTOR in context.MasterLocations on TO.ReceiverIDLocation equals MLTOR.IDLocation into RJOIN_4
                        from MLTOR in RJOIN_4.DefaultIfEmpty()
                        join TR in context.TransportRoutes on TE.IDTransportExecution equals TR.IDTransportExecution into LJOIN_3
                        from TR in LJOIN_3.DefaultIfEmpty()
                        join MLTR in context.MasterLocations on TR.IDLocation equals MLTR.IDLocation into RJOIN_5
                        from MLTR in RJOIN_5.DefaultIfEmpty()
                        join TV in context.TransportVesselMonitorings on TE.IDTransportExecution equals TV.IDTransportExecution into LJOIN_4
                        from TV in LJOIN_4.DefaultIfEmpty()
                        where TE.IsActive == true && TO.IsActive == true && TOD.MaterialType != null && TR.IsAssigned == true
                        orderby TE.IDTransportExecution, TO.IDTransportOrder, TOD.IDTransportOrderDetail, TR.IDTransportRoute
                        select new TransportExecutionDTO
                        {
                            TransportNo = TE.TransportNo,
                            TransportDate = TE.TransportDate,
                            STONo = TO.STONo,
                            ZoneBased = TO.ZoneBased,
                            OrderType = TO.OrderType,
                            Sender = MLTOS.LocationName,
                            Receiver = MLTOR.LocationName,
                            ShipmentDate = TO.ShipmentDate,
                            VehicleType = TO.VehicleType,
                            MaterialType = TOD.MaterialType,
                            Description = TOD.Description,
                            Qty = TOD.Qty,
                            UoM = TOD.UoM,
                            TransportCategory = TE.TransportCategory,
                            CreatedBy = TE.CreatedBy,
                            VendorName = MV.VendorName,
                            SIWeek = TE.SIWeek,
                            SIType = TE.SIType,
                            SIStatus = TE.SIStatus,
                            StartLocation = MLTE.LocationName,
                            UpdatedBy = TE.UpdatedBy,
                            UpdatedDate = TE.UpdatedDate,
                            IDTransportOrderDetail = TOD.IDTransportOrderDetail,
                            IDLocation = TR.IDLocation,
                            LocationName = MLTR.LocationName,
                            TotalKM = TE.TotalKM,
                            TotalBox = TE.TotalBox,
                            TransportMode = TE.TransportMode,
                            //VesselName = TV.VesselName,
                            //ContainerNo = TE.ContainerNo,
                            //ContainerSeal = TE.ContainerSeal,
                            ServicePONo = TE.ServicePONo,
                            SerivceGRNo = TE.ServiceGRNo,
                            ActualCostCenter = TE.ActualCostCenter
                        }
                        );

            string[] ftn = fltrtn.Split(',').ToArray();
            string[] fon = fltron.Split(',').ToArray();
            string[] foc = fltroc.Split(',').ToArray();
            string[] fsi = fltrsi.Split(',').ToArray();
            string[] fot = fltrot.Split(',').ToArray();
            string[] fzo = fltrzo.Split(',').ToArray();
            string[] fsl = fltrsl.Split(',').ToArray();
            string[] fmt = fltrmt.Split(',').ToArray();
            string[] fse = fltrse.Split(',').ToArray();
            string[] fre = fltrre.Split(',').ToArray();

            if (fltrtn.Length > 1) { dbResult = dbResult.Where(q => ftn.Contains(q.TransportNo)); }
            if (fltron.Length > 1) { dbResult = dbResult.Where(q => fon.Contains(q.STONo)); }
            if (fltroc.Length > 1) { dbResult = dbResult.Where(q => foc.Contains(q.TransportCategory)); }
            if (fltrsi.Length > 1) { dbResult = dbResult.Where(q => fsi.Contains(q.SIStatus)); }
            if (fltrot.Length > 1) { dbResult = dbResult.Where(q => fot.Contains(q.OrderType)); }
            if (fltrzo.Length > 1) { dbResult = dbResult.Where(q => fzo.Contains(q.ZoneBased)); }
            if (fltrsl.Length > 1) { dbResult = dbResult.Where(q => fsl.Contains(q.StartLocation)); }
            if (fltrmt.Length > 1) { dbResult = dbResult.Where(q => fmt.Contains(q.MaterialType)); }
            if (fltrse.Length > 1) { dbResult = dbResult.Where(q => fse.Contains(q.Sender)); }
            if (fltrre.Length > 1) { dbResult = dbResult.Where(q => fre.Contains(q.Receiver)); }
            if (fltrdf.Length > 1)
            {
                DateTime fdf = Convert.ToDateTime(fltrdf);
                dbResult = dbResult.Where(q => q.ShipmentDate >= fdf);
            }
            if (fltrdt.Length > 1)
            {
                DateTime fdt = Convert.ToDateTime(fltrdt);
                dbResult = dbResult.Where(q => q.ShipmentDate <= fdt);
            }

            return dbResult.ToList();
        }
        #endregion SUB EXPORT

        public void UploadExecution(IEnumerable<IEnumerable<TransportExecutionImportInput>> input, string userid)
        {
            TOMContextDB ctx = new TOMContextDB();
            foreach (var sheet in input)
            {
                foreach (var row in sheet)
                {
                    var temp = ctx.TransportExecutions.Where(f => f.TransportNo == row.TransportNo).FirstOrDefault();

                    #region SI
                    if (row.DataType == "SI")
                    {
                        //Save to log
                        if (row.SIStatus == EnumHelper.GetDescription(Enums.SIStatus.Unfullfill))
                        {
                            #region Add Log
                            if (!string.IsNullOrWhiteSpace(temp.SIStatus) || !string.IsNullOrWhiteSpace(row.SIStatus))
                            {
                                TransportVendorChangeLog trVendor = new TransportVendorChangeLog();
                                trVendor.IDTransportExecution = temp.IDTransportExecution;
                                trVendor.TransportDate = temp.TransportDate;
                                trVendor.IDVendor = !temp.IDVendor.HasValue ? 0 : temp.IDVendor.Value;
                                trVendor.SIType = !string.IsNullOrWhiteSpace(temp.SIType) ? temp.SIType : row.SIType;
                                trVendor.SIStatus = string.IsNullOrWhiteSpace(temp.SIStatus) ? row.SIStatus : temp.SIStatus;
                                trVendor.TargetOfArrival = !temp.TargetOfArrival.HasValue || temp.TargetOfArrival.Value == DateTime.MinValue ? row.TargetOfArrival : temp.TargetOfArrival;
                                trVendor.ActualArrive = temp.ActualArrive;
                                trVendor.CreatedBy = userid;
                                trVendor.CreatedDate = DateTime.Now;
                                trVendor.UpdatedBy = userid;
                                trVendor.UpdatedDate = DateTime.Now;
                                ctx.TransportVendorChangeLogs.Add(trVendor);
                            }
                            #endregion

                            var vendor = _masterVendorRepo.GetMasterVendorByVendorName(row.NewVendorName);
                            //update IDVendor with new Vendor
                            if (vendor != null)
                            {
                                temp.IDVendor = vendor.IDVendor;
                                temp.TransportMode = vendor.TransportationMode;
                            }

                            if (temp.TransportDate != DateTime.MinValue && row.NewTransportDate.HasValue)
                            {
                                temp.TransportDate = row.NewTransportDate.Value;
                                // jika transport date berubah, week dan year ikut berubah sesuai new transport date
                                MasterGenWeek week = _masterGenWeekRepo.GetGenWeekNowByDate(row.NewTransportDate.Value);
                                temp.SIWeek = (byte?)week.Week;
                                temp.Year = row.NewTransportDate.HasValue ? row.NewTransportDate.Value.Year : temp.Year;
                                temp.Month = row.NewTransportDate.HasValue ? (byte?)row.NewTransportDate.Value.Month : temp.Month;
                            }
                            //if (string.IsNullOrWhiteSpace(temp.SIType) && !string.IsNullOrWhiteSpace(row.NewSIType))
                            temp.SIType = row.NewSIType;

                            temp.SIStatus = EnumHelper.GetDescription(Enums.SIStatus.Fullfill);

                            //if ((!temp.TargetOfArrival.HasValue || temp.TargetOfArrival == DateTime.MinValue) && row.NewTargetOfArrival.HasValue)
                            temp.TargetOfArrival = row.NewTargetOfArrival;
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(temp.SIStatus))
                                temp.SIStatus = row.SIStatus;
                            if (temp.TargetOfArrival == null)
                                temp.TargetOfArrival = row.TargetOfArrival;
                            if (string.IsNullOrWhiteSpace(temp.SIType))
                                temp.SIType = row.SIType;
                        }
                    }
                    #endregion
                    if (row.DataType == "TN")
                    {
                        var newRow = new TransportExecutionDTO();
                        TransportVesselMonitoring trVesselM = _transportVesselMonitoringRepo.GetVesselMonitoringByidTE(temp.IDTransportExecution);
                        if (trVesselM == null & (temp.TransportMode == "Ship" || temp.TransportMode == "Train"))
                        {
                            TransportVesselMonitoring insertVessel = new TransportVesselMonitoring();
                            insertVessel.IDTransportExecution = temp.IDTransportExecution;
                            insertVessel.VesselName = row.VesselName;
                            insertVessel.ContainerNo = row.ContainerNumber;
                            insertVessel.ContainerSeal = row.SealNumber;
                            insertVessel.CreatedBy = userid;
                            insertVessel.UpdatedBy = userid;
                            insertVessel.IsActive = true;
                            _transportVesselMonitoringRepo.SaveData(insertVessel, true);
                        }

                        if (temp.ActualArrive == null || temp.ActualArrive == DateTime.MinValue) temp.ActualArrive = row.ActualArrive;
                        if (string.IsNullOrEmpty(temp.ServicePONo) && !String.IsNullOrEmpty(row.ServicePONumber)) temp.ServicePONo = row.ServicePONumber;
                        if (string.IsNullOrEmpty(temp.ServiceGRNo) && !String.IsNullOrEmpty(row.ServiceGRNumber)) temp.ServiceGRNo = row.ServiceGRNumber;
                        if (string.IsNullOrEmpty(temp.PoliceRegNo) && !String.IsNullOrEmpty(row.PoliceRegNumber)) temp.PoliceRegNo = row.PoliceRegNumber;
                        if (string.IsNullOrEmpty(temp.IDDriver1) && !String.IsNullOrEmpty(row.Driver1)) temp.IDDriver1 = row.Driver1;
                        if (string.IsNullOrEmpty(temp.IDDriver2) && !String.IsNullOrEmpty(row.Driver2)) temp.IDDriver2 = row.Driver2;
                        if (string.IsNullOrEmpty(temp.IDCoDriver) && !String.IsNullOrEmpty(row.CoDriver)) temp.IDCoDriver = row.CoDriver;
                    }
                    if (row.DataType == "Via")
                    {
                        if (!String.IsNullOrWhiteSpace(row.Via))
                        {
                            row.Via = row.Via.First().ToString().ToUpper() + row.Via.Substring(1).ToLower();
                        }
                        
                        temp.Via = row.Via;
                        temp.UpdatedBy = userid;
                        temp.UpdatedDate = DateTime.Now;
                    }

                    ctx.SaveChanges();
                    if (row.DataType == "Via")
                    {
                        _transportExecutionRepo.CalculateCost(temp.IDTransportExecution.ToString(), userid, 2);
                    }
                }
            }
        }
        public void UploadExecution(IEnumerable<TransportExecutionImportInput> input, string userid)
        {
            UploadExecution(new List<IEnumerable<TransportExecutionImportInput>>() { input }, userid);
        }
        public List<string> UploadExecution(HttpPostedFileBase input, string userid)
        {
            #region Old Logic
            var stream = input.InputStream;
            List<string> listError = new List<string>();
            try
            {
                using (ExcelPackage xlPackage = new ExcelPackage(stream))
                {
                    var myWorksheet = xlPackage.Workbook.Worksheets.First();
                    TOMContextDB ctx = new TOMContextDB();
                    // check validate
                    listError = CheckValidate(myWorksheet);
                    var totalColumns = myWorksheet.Dimension.End.Column;
                    var totalRows = myWorksheet.Dimension.End.Row;
                    string typeUpload = myWorksheet.Name;//getTypeUpload(totalColumns);
                    if (listError.Count == 0)
                    {
                        //var Data = new List<TransportExecutionDTO>();

                        for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                        {
                            //var xRow = myWorksheet.Cells;
                            //if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 1].Text))
                            //{
                            var TN = myWorksheet.Cells[rowNum, 1].Text;
                            if (String.IsNullOrEmpty(TN)) break;
                            var temp = ctx.TransportExecutions.Where(f => f.TransportNo == TN).FirstOrDefault(); //_transportExecutionRepo.GetTransportExecutionByTN(myWorksheet.Cells[rowNum, 1].Text);                                
                            if (typeUpload == "SI")
                            {
                                var CurRow = new TransportExecutionDTO();
                                CurRow.TransportNo = myWorksheet.Cells[rowNum, 1].Text;
                                CurRow.SIType = myWorksheet.Cells[rowNum, 2].Text;
                                CurRow.TargetOfArrival = DateTime.Parse(myWorksheet.Cells[rowNum, 3].Text);
                                CurRow.SIStatus = myWorksheet.Cells[rowNum, 4].Text;
                                if (CurRow.SIStatus == EnumHelper.GetDescription(Enums.SIStatus.Unfullfill) && String.IsNullOrEmpty(temp.SIStatus))
                                {
                                    var VendorLog = new TransportVendorChangeLog();
                                    CurRow.VendorName = myWorksheet.Cells[rowNum, 5].Text;
                                    CurRow.SIType = myWorksheet.Cells[rowNum, 6].Text;
                                    CurRow.TargetOfArrival = DateTime.Parse(myWorksheet.Cells[rowNum, 7].Text);
                                    CurRow.TransportDate = DateTime.Parse(myWorksheet.Cells[rowNum, 8].Text);
                                }

                                //Check if existing property has value then ignore
                                if (String.IsNullOrEmpty(temp.SIType)) temp.SIType = CurRow.SIType;
                                if (temp.TargetOfArrival != null) temp.TargetOfArrival = CurRow.TargetOfArrival;
                                if (String.IsNullOrEmpty(temp.SIType)) temp.SIType = CurRow.SIType;

                                //Save to log
                                if (CurRow.SIStatus == EnumHelper.GetDescription(Enums.SIStatus.Unfullfill) && String.IsNullOrEmpty(temp.SIStatus))
                                {
                                    var vendor = _masterVendorRepo.GetMasterVendorByVendorName(myWorksheet.Cells[rowNum, 5].Text);
                                    TransportVendorChangeLog trVendor = new TransportVendorChangeLog();
                                    trVendor.IDTransportExecution = temp.IDTransportExecution;
                                    trVendor.TransportDate = temp.TransportDate;
                                    trVendor.IDVendor = (int)temp.IDVendor;
                                    trVendor.SIType = temp.SIType;
                                    trVendor.SIStatus = CurRow.SIStatus;
                                    trVendor.TargetOfArrival = temp.TargetOfArrival;
                                    trVendor.ActualArrive = temp.ActualArrive;
                                    trVendor.CreatedBy = userid;
                                    trVendor.UpdatedBy = userid;
                                    _transportVendorChangeLogRepo.InsertData(trVendor);

                                    //update IDVendor with new Vendor
                                    temp.IDVendor = vendor.IDVendor;
                                    temp.TransportDate = CurRow.TransportDate;
                                    temp.SIType = CurRow.SIType;
                                    temp.SIStatus = EnumHelper.GetDescription(Enums.SIStatus.Fullfill);
                                    temp.TargetOfArrival = CurRow.TargetOfArrival;
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(temp.SIStatus)) temp.SIStatus = CurRow.SIStatus;
                                }

                                //temp.TransportDate = DateTime.ParseExact(myWorksheet.Cells[rowNum, 9].Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                //temp.PoliceRegNo = myWorksheet.Cells[rowNum, 10].Text;
                                //temp.IDDriver1 = myWorksheet.Cells[rowNum, 11].Text;
                                //temp.IDDriver2 = myWorksheet.Cells[rowNum, 12].Text;
                                //temp.IDCoDriver = myWorksheet.Cells[rowNum, 13].Text;
                                /*
                                temp.SIType = myWorksheet.Cells[rowNum, 6].Text;
                                temp.TargetOfArrival = DateTime.Parse(myWorksheet.Cells[rowNum, 7].Text);
                                //temp.TargetOfArrival = DateTime.ParseExact(myWorksheet.Cells[rowNum, 7].Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                temp.SIStatus = myWorksheet.Cells[rowNum, 8].Text;
                                temp.TransportDate = DateTime.Parse(myWorksheet.Cells[rowNum, 9].Text);
                                //temp.TransportDate = DateTime.ParseExact(myWorksheet.Cells[rowNum, 9].Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                temp.PoliceRegNo = myWorksheet.Cells[rowNum, 10].Text;
                                temp.IDDriver1 = myWorksheet.Cells[rowNum, 11].Text;
                                temp.IDDriver2 = myWorksheet.Cells[rowNum, 12].Text;
                                temp.IDCoDriver = myWorksheet.Cells[rowNum, 13].Text;
                                */
                            }
                            else if (typeUpload == "TN")
                            {
                                var newRow = new TransportExecutionDTO();
                                //newRow.TransportNo = xRow[rowNum, 1].Text;
                                //newRow.ActualArrive = xRow[rowNum, 1].Text;                                    
                                var statusSave = true;
                                TransportVesselMonitoring trVesselM = _transportVesselMonitoringRepo.GetVesselMonitoringByidTE(temp.IDTransportExecution);
                                if (trVesselM == null)
                                {
                                    TransportVesselMonitoring insertVessel = new TransportVesselMonitoring();
                                    insertVessel.IDTransportExecution = temp.IDTransportExecution;
                                    insertVessel.VesselName = myWorksheet.Cells[rowNum, 2].Text;
                                    insertVessel.ContainerNo = myWorksheet.Cells[rowNum, 3].Text;
                                    insertVessel.ContainerSeal = myWorksheet.Cells[rowNum, 4].Text;
                                    insertVessel.CreatedBy = userid;
                                    insertVessel.UpdatedBy = userid;
                                    insertVessel.IsActive = true;
                                    statusSave = true;
                                    _transportVesselMonitoringRepo.SaveData(insertVessel, statusSave);
                                }
                                /*
                                else
                                {
                                    temp.TransportVesselMonitorings.FirstOrDefault().ContainerNo = myWorksheet.Cells[rowNum, 3].Text;
                                    temp.TransportVesselMonitorings.FirstOrDefault().ContainerSeal = myWorksheet.Cells[rowNum, 4].Text;
                                    statusSave = false;
                                    //_transportVesselMonitoringRepo.SaveData(trVesselM, statusSave);
                                }
                                */

                                if (temp.ActualArrive == null) temp.ActualArrive = DateTime.Parse(myWorksheet.Cells[rowNum, 5].Text);
                                if (String.IsNullOrEmpty(temp.ServicePONo)) temp.ServicePONo = myWorksheet.Cells[rowNum, 6].Text;
                                if (String.IsNullOrEmpty(temp.ServiceGRNo)) temp.ServiceGRNo = myWorksheet.Cells[rowNum, 7].Text;
                                if (String.IsNullOrEmpty(temp.PoliceRegNo) && !String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 8].Text)) temp.PoliceRegNo = myWorksheet.Cells[rowNum, 8].Text;

                                var DriverData = (from drv in ctx.TransportDriverManagements
                                                  where drv.IsActive && drv.DrivingLicensePeriod >= temp.TransportDate && drv.IDVendor == temp.IDVendor
                                                  select new TransportDriverManagementDTO { ID = drv.ID, Name = String.Concat(drv.Name, "-", drv.ID), RoleDriver = drv.RoleDriver }
                                );
                                var Driver1 = myWorksheet.Cells[rowNum, 9].Text;
                                var Driver2 = myWorksheet.Cells[rowNum, 10].Text;
                                var CoDriver = myWorksheet.Cells[rowNum, 11].Text;
                                if (string.IsNullOrEmpty(temp.IDDriver1) && !String.IsNullOrEmpty(Driver1))
                                    temp.IDDriver1 = DriverData.Where(f => f.Name == Driver1 && f.RoleDriver == "Driver").FirstOrDefault().ID;

                                if (string.IsNullOrEmpty(temp.IDDriver2) && !String.IsNullOrEmpty(Driver2))
                                    temp.IDDriver2 = DriverData.Where(f => f.Name == Driver2 && f.RoleDriver == "Driver").FirstOrDefault().ID;

                                if (string.IsNullOrEmpty(temp.IDCoDriver) && !String.IsNullOrEmpty(CoDriver))
                                    temp.IDCoDriver = DriverData.Where(f => f.Name == CoDriver && f.RoleDriver == "CO-Driver").FirstOrDefault().ID;
                            }
                            temp.UpdatedBy = userid;
                            //_transportExecutionRepo.SaveData(temp);
                            ctx.SaveChanges();
                            //}
                        }
                    }
                }
            }
            catch (Exception e)
            {
                listError.Add(e.Message);
            }
            return listError;
            #endregion
        }

        public List<string> CheckValidate(ExcelWorksheet myWorksheet)
        {
            List<string> listError = new List<string>();
            TOMContextDB ctx = new TOMContextDB();
            //TransporExecutionInput filter = new TransporExecutionInput();
            var totalColumns = myWorksheet.Dimension.End.Column;
            var totalRows = myWorksheet.Dimension.End.Row;
            //string typeUpload = getTypeUpload(totalColumns);
            string typeUpload = myWorksheet.Name;
            if (typeUpload == null)
            {
                listError.Add("Incorrect template format.");
            }
            else
            {
                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 1].Text))
                    {
                        // validate record Transportation Execution
                        var TransportNo = myWorksheet.Cells[rowNum, 1].Text;
                        //filter.SIType = myWorksheet.Cells[rowNum, 2].Text;
                        //filter.TargetOfArrival = DateTime.Parse(myWorksheet.Cells[rowNum, 3].Text);
                        //filter.SIStatus = myWorksheet.Cells[rowNum, 4].Text;
                        TransportExecution trExe = _transportExecutionRepo.GetTransportExecutionByTN(TransportNo);
                        if (trExe == null)
                            listError.Add("Data error at row -" + rowNum + " data Transportation execution not found");
                        if (typeUpload == "SI")
                        {
                            DateTime parseDate;
                            // validate date
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 2].Text))
                            {
                                var SIType = _masterListRepo.GetMasterListCustom("SIType", myWorksheet.Cells[rowNum, 2].Text);
                                if (SIType == null)
                                    listError.Add("Data error at row -" + rowNum + " column SI Type");
                            }
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 3].Text))
                            {
                                bool parse = DateTime.TryParse(myWorksheet.Cells[rowNum, 3].Text, out parseDate);
                                if (!parse)
                                    listError.Add("Data error at row -" + rowNum + " column Target Of Arrival");
                            }
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 7].Text))
                            {
                                bool parse = DateTime.TryParse(myWorksheet.Cells[rowNum, 7].Text, out parseDate);
                                if (!parse)
                                    listError.Add("Data error at row -" + rowNum + " column New Target Of Arrival");
                            }
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 8].Text))
                            {
                                bool parse = DateTime.TryParse(myWorksheet.Cells[rowNum, 8].Text, out parseDate);
                                if (!parse)
                                    listError.Add("Data error at row -" + rowNum + " column New Transportation Date");
                            }
                            // validate vendor
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 5].Text))
                            {
                                var idvendor = _masterVendorRepo.GetMasterVendorByVendorName(myWorksheet.Cells[rowNum, 5].Text);
                                if (idvendor == null)
                                    listError.Add("Data error at row -" + rowNum + " column New Vendor");
                            }
                            // validate SI Type
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 6].Text))
                            {
                                var SIType = _masterListRepo.GetMasterListCustom("SIType", myWorksheet.Cells[rowNum, 6].Text);
                                if (SIType == null)
                                    listError.Add("Data error at row -" + rowNum + " column New SI Type");
                            }
                            // validate SI Status
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 4].Text))
                            {
                                var SIStatus = _masterListRepo.GetMasterListCustom("SIStatus", myWorksheet.Cells[rowNum, 4].Text);
                                if (SIStatus == null || trExe.IDVendor == null)
                                    listError.Add("Data error at row -" + rowNum + " column SI Status");

                                if (myWorksheet.Cells[rowNum, 4].Text.Equals(EnumHelper.GetDescription(Enums.SIStatus.Unfullfill)) && String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 5].Text))
                                {
                                    listError.Add("Data error at row -" + rowNum + " column New Vendor (Can't be empty)");
                                }
                            }
                        }
                        else if (typeUpload == "TN")
                        {
                            DateTime parseDate;
                            // validate Vessel
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 2].Text))
                            {
                                var SIStatus = _masterListRepo.GetMasterListCustom("VesselName", myWorksheet.Cells[rowNum, 2].Text);
                                if (SIStatus == null)
                                    listError.Add("Data error at row -" + rowNum + " column Vessel Name");
                            }
                            // validate date
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 5].Text))
                            {
                                bool parse = DateTime.TryParse(myWorksheet.Cells[rowNum, 5].Text, out parseDate);
                                if (!parse)
                                    listError.Add("Data error at row -" + rowNum + " column Actual Arrive");
                            }

                            // validate Vessel
                            if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 2].Text))
                            {
                                var VesselName = _masterListRepo.GetMasterListCustom("VesselName", myWorksheet.Cells[rowNum, 2].Text);
                                if (VesselName == null)
                                    listError.Add("Data error at row -" + rowNum + " column Vessel Name");
                            }

                            // validate ServicePO
                            var ServicePO = myWorksheet.Cells[rowNum, 6].Text;
                            /*
                            if (!String.IsNullOrEmpty(ServicePO))
                            {
                                var IsFound = ctx.MasterServicePoes
                                              .Where(f => f.IsActive && f.ServicePONumber == ServicePO && f.IDVendor == trExe.IDVendor 
                                              && trExe.TransportDate >= f.EffectiveStartDate && trExe.TransportDate <= f.EffectiveEndDate)
                                              .FirstOrDefault() != null;
                                if (!IsFound)
                                    listError.Add("Data error at index -" + rowNum + " column Service PO");
                            }
                            */

                            var PoliceRegNumber = myWorksheet.Cells[rowNum, 8].Text;
                            // validate Police Reg Number
                            if (!String.IsNullOrEmpty(PoliceRegNumber))
                            {
                                var IsFound = ctx.TransportVehicleDatas
                                              .Where(f => f.IDPoliceRegNumber == PoliceRegNumber && f.STNKValidityPeriod >= trExe.TransportDate
                                              && f.IsActive).FirstOrDefault() != null;
                                if (!IsFound || trExe.IDVendor == null)
                                    listError.Add("Data error at row -" + rowNum + " column Police Reg Number");
                            }
                            /*
                            var DriverData = ctx.TransportDriverManagements
                                             .Select(f => new TransportDriverManagementDTO { Name = String.Concat(f.Name, "-", f.ID) })
                                             .Where(f => f.IsActive);// && f.DrivingLicensePeriod >= trExe.TransportDate);
                            */

                            var DriverData = (from drv in ctx.TransportDriverManagements
                                              where drv.IsActive && drv.DrivingLicensePeriod >= trExe.TransportDate && drv.IDVendor == trExe.IDVendor
                                              select new TransportDriverManagementDTO { Name = String.Concat(drv.Name, "-", drv.ID), RoleDriver = drv.RoleDriver }
                                              );

                            var Driver1 = myWorksheet.Cells[rowNum, 9].Text;
                            // validate Driver 1
                            if (!String.IsNullOrEmpty(Driver1))
                            {
                                var IsFound = DriverData != null && DriverData.Where(f => f.Name == Driver1 && f.RoleDriver == "Driver").FirstOrDefault() != null; //(f => f String.Concat(f, "-", f.ID) f. == myWorksheet.Cells[rowNum, 9].Text).FirstOrDefault() != null;
                                if (!IsFound || trExe.IDVendor == null)
                                    listError.Add("Data error at row -" + rowNum + " column Driver 1");
                            }
                            var Driver2 = myWorksheet.Cells[rowNum, 10].Text;
                            // validate Driver 2
                            if (!String.IsNullOrEmpty(Driver2))
                            {
                                var IsFound = DriverData != null && DriverData.Where(f => f.Name == Driver2 && f.RoleDriver == "Driver").FirstOrDefault() != null; //(f => f String.Concat(f, "-", f.ID) f. == myWorksheet.Cells[rowNum, 9].Text).FirstOrDefault() != null;
                                if (!IsFound || trExe.IDVendor == null)
                                    listError.Add("Data error at row -" + rowNum + " column Driver 2");
                            }
                            var CoDriver = myWorksheet.Cells[rowNum, 11].Text;
                            // validate Co Driver
                            if (!String.IsNullOrEmpty(CoDriver))
                            {
                                var IsFound = DriverData != null && DriverData.Where(f => f.Name == CoDriver && f.RoleDriver == "CO-Driver").FirstOrDefault() != null; //(f => f String.Concat(f, "-", f.ID) f. == myWorksheet.Cells[rowNum, 9].Text).FirstOrDefault() != null;
                                if (!IsFound || trExe.IDVendor == null)
                                    listError.Add("Data error at row -" + rowNum + " column Co Driver");
                            }
                            // validate Service PO
                            //if (trExe.ServicePONo != null || trExe.IDVendor == null)
                            //    listError.Add("Data error at index -" + rowNum + ". Service PO Number already exist at "+myWorksheet.Cells[rowNum, 1].Text);
                            /*if (!String.IsNullOrEmpty(myWorksheet.Cells[rowNum, 5].Text))
                            {
                                var ServicePO = _masterServicePORepo.GetMasterServicePOByPONumber(myWorksheet.Cells[rowNum, 5].Text);
                                if (ServicePO == null)                                
                                    listError.Add("Data error at index -" + rowNum + " column Service PO Number");                                
                            }*/
                        }
                    }

                }
            }
            return listError;
        }
        public string getTypeUpload(int totalColumns)
        {
            string typeUpload;
            if (totalColumns == 13)
                typeUpload = "SI";
            else if (totalColumns == 6)
                typeUpload = "TN";
            else
                typeUpload = null;
            return typeUpload;
        }

        //public TransportExecutionPrintDTO PrintDocument(List<int> idTE, string username)
        public TransportExecutionPrintDTO PrintDocument(int idTE, string username)
        {
            var ctx = new TOMContextDB();
            TransportExecutionPrintDTO temp = new TransportExecutionPrintDTO();
            List<TransportOrderTransportExecutionPrintDTO> tempListTO = new List<TransportOrderTransportExecutionPrintDTO>();
            List<TransportOrderTransportExecutionPrintDTO> tempListTONOGroup = new List<TransportOrderTransportExecutionPrintDTO>();
            List<TransportOrderDetailPrintDTO> tempListDetail = new List<TransportOrderDetailPrintDTO>();
            List<TransportDriverManagementDTO> tempListDriver = GetALLTransportDriverManagementList().GroupBy(x => x.ID).Select(x => x.First()).ToList();

            List<TransportOrderTransportExecutionPrintDTO> tempTO = _transportOrderRepo.GetTransportOrderFilterByidTE(idTE);
            int totalPage = 0;
            foreach (TransportOrderTransportExecutionPrintDTO devNote in tempTO)
            {
                TransportOrderTransportExecutionPrintDTO tempTOTE = new TransportOrderTransportExecutionPrintDTO();
                tempTOTE.IDTransportExecution = devNote.TransportExecution.IDTransportExecution;
                tempTOTE.IDTransportOrder = devNote.IDTransportOrder;
                tempTOTE.STONo = devNote.STONo;
                tempTOTE.TransportNo = devNote.TransportExecution.TransportNo;
                tempTOTE.TransportDate = devNote.TransportExecution.TransportDate;
                tempTOTE.ActualVehicleType = devNote.TransportExecution.ActualVehicleType;
                tempTOTE.PoliceRegNo = devNote.TransportExecution.PoliceRegNo;
                tempTOTE.IDVendor = devNote.TransportExecution.IDVendor;
                tempTOTE.VendorName = (devNote.TransportExecution.MasterVendor == null) ? "" : devNote.TransportExecution.MasterVendor.VendorName;
                /*TransportDriverManagementDTO tempDriver1 = tempListDriver.Where(x => x.ID == devNote.IDDriver1).FirstOrDefault();
                TransportDriverManagementDTO tempDriver2 = tempListDriver.Where(x => x.ID == devNote.IDDriver2).FirstOrDefault();
                TransportDriverManagementDTO tempCoDriver = tempListDriver.Where(x => x.ID == devNote.IDCoDriver).FirstOrDefault();
                tempTOTE.DriverName1 = (tempDriver1 == null) ? "" : tempDriver1.Name;
                tempTOTE.DriverName2 = (tempDriver2 == null) ? "" : tempDriver2.Name;
                tempTOTE.CoDriverName = (tempCoDriver == null) ? "" : tempCoDriver.Name;*/
                //tempTOTE.DriverName1 = devNote.DriverName1;
                //tempTOTE.DriverName2 = devNote.DriverName2;
                //tempTOTE.CoDriverName = devNote.CoDriverName;
                #region new code Nicco
                tempTOTE.DriverName1 = devNote.DriverName1;
                tempTOTE.IDDriver1 = devNote.TransportExecution.IDDriver1;
                tempTOTE.DriverName2 = devNote.DriverName2;
                tempTOTE.IDDriver2 = devNote.TransportExecution.IDDriver2;
                tempTOTE.CoDriverName = devNote.CoDriverName;
                tempTOTE.IDCoDriver = devNote.TransportExecution.IDCoDriver;
                #endregion
                tempTOTE.StartLocation = devNote.MasterLocation2.LocationName;
                tempTOTE.SenderIDLocation = devNote.SenderIDLocation;
                tempTOTE.SenderLocationName = devNote.MasterLocation.LocationName;
                tempTOTE.ReceiverIDLocation = devNote.ReceiverIDLocation;
                tempTOTE.ReceiverLocationName = devNote.MasterLocation1.LocationName;
                tempTOTE.VesselName = devNote.TransportVesselMonitoring.VesselName;
                tempTOTE.ContainerNo = devNote.TransportVesselMonitoring.ContainerNo;
                tempTOTE.ContainerSeal = devNote.TransportVesselMonitoring.ContainerSeal;

                tempTOTE.Remarks = devNote.Remarks;
                /*
                var rmk = _transportOrderRepo.Get(to => to.IDTransportExecution == devNote.IDTransportExecution && to.IsActive).Select(to => to.Remarks).ToList();
                if (rmk.Count > 0)
                    tempTOTE.Remarks = string.Join("; ", rmk.Where(r => !string.IsNullOrWhiteSpace(r)).ToArray());
                */

                var idloc = devNote.SenderIDLocation != null ? devNote.SenderIDLocation.Trim() : null;
                var locA = ctx.GeneralBuildingFacilities.Where(gbf => gbf.IDLocation.Trim() == idloc).FirstOrDefault();
                tempTOTE.WarehouseAddressSender = locA == null ? devNote.GeneralBuildingFacility.WarehouseAddress : locA.WarehouseAddress;
                idloc = devNote.ReceiverIDLocation != null ? devNote.ReceiverIDLocation.Trim() : null;
                var locB = ctx.GeneralBuildingFacilities.Where(gbf => gbf.IDLocation.Trim() == idloc).FirstOrDefault();
                tempTOTE.WarehouseAddressReceiver = locB == null ? devNote.GeneralBuildingFacility1.WarehouseAddress : locB.WarehouseAddress;

                GenerateETransportCard tGen = new GenerateETransportCard();
                tempTOTE.TNBarcode = tGen.getPreviewBarcode(devNote.TransportExecution.TransportNo);

                //List<TransportOrderDetailPrintDTO> ListDetail = _transportOrderDetailRepo.GetTransportOrderDetailByidTO(devNote.IDTransportOrder);
                List<TransportOrderDetailPrintDTO> ListDetail = _transportOrderDetailRepo.GetTransportOrderDetailBySenderReceiverTE(devNote.SenderIDLocation, devNote.ReceiverIDLocation, devNote.TransportExecution.IDTransportExecution);
                foreach (TransportOrderDetailPrintDTO tod in ListDetail)
                {
                    TransportOrderDetailPrintDTO tempTOD = new TransportOrderDetailPrintDTO();
                    tempTOD.STONo = tod.TransportOrder.STONo;
                    tempTOD.IDTransportExecution = tod.TransportOrder.IDTransportExecution;

                    tempTOD.SenderIDLocation = tod.TransportOrder.ActualSenderIDLocation;
                    tempTOD.ReceiverIDLocation = tod.TransportOrder.ActualReceiverIDLocation;
                    tempTOD.IDTransportOrder = tod.IDTransportOrder;
                    tempTOD.MaterialType = tod.MaterialType;
                    tempTOD.Code = tod.Code;
                    tempTOD.Description = tod.Description;
                    tempTOD.Qty = tod.Qty;
                    tempTOD.UoM = tod.UoM;
                    tempTOD.Remarks = ctx.TransportOrders.Where(
                                            to => 
                                            to.STONo == tempTOD.STONo &&
                                            to.ActualSenderIDLocation == tempTOD.SenderIDLocation &&
                                            to.ActualReceiverIDLocation == tempTOD.ReceiverIDLocation)
                                        .OrderBy(lte => lte.IDTransportOrder)
                                        .Select(lte => lte.Remarks).FirstOrDefault();
                    //tempTOD.Remarks = string.Join("", ctx.TransportOrders
                    //                    .Where(to =>
                    //                        to.IDTransportExecution == tempTOD.IDTransportExecution &&
                    //                        to.ActualSenderIDLocation == tempTOD.SenderIDLocation &&
                    //                        to.ActualReceiverIDLocation == tempTOD.ReceiverIDLocation)
                    //                    .OrderBy(lte => lte.IDTransportOrder)
                    //                    .Select(lte => lte.Remarks)
                    //                    .Distinct()
                    //                    .ToList()
                    //                    .Where(i => !string.IsNullOrWhiteSpace(i)));

                    tempListDetail.Add(tempTOD);
                }
                tempListTO.Add(tempTOTE);
                totalPage++;
            }

            foreach (var lto in tempListTO)
            {
                lto.Remarks = string.Join("; ",
                    ctx.TransportOrders.
                    Where(tod => tod.IDTransportExecution == lto.IDTransportExecution &&
                            tod.ActualSenderIDLocation == lto.SenderIDLocation &&
                            tod.ActualReceiverIDLocation == lto.ReceiverIDLocation &&
                            lto.Remarks != null)
                    .OrderBy(lte => lte.IDTransportOrder)
                    .Select(lte => lte.Remarks).ToList().Where(i => !string.IsNullOrWhiteSpace(i))
                    );
            }

            int totalGatePass = 0;
            List<TransportExecution> resultTE = _transportExecutionRepo.GetListTransportExecutionByID(idTE);
            List<TransportOrderDTO> tempListTOGate = new List<TransportOrderDTO>();
            foreach (TransportExecution tempTE in resultTE)
            {
                TransportOrderTransportExecutionPrintDTO tempTONoGroup = new TransportOrderTransportExecutionPrintDTO();
                tempTONoGroup.TransportNo = tempTE.TransportNo;
                tempTONoGroup.TransportDate = tempTE.TransportDate;
                tempTONoGroup.ActualVehicleType = tempTE.ActualVehicleType;
                tempTONoGroup.PoliceRegNo = tempTE.PoliceRegNo;
                //tempTONoGroup.VendorName = tempTE.MasterVendor.VendorName;
                tempTONoGroup.VendorName = (tempTE.MasterVendor == null) ? "" : tempTE.MasterVendor.VendorName;
                //tempTONoGroup.DriverName1 = tempTE.TransportDriverManagement.Name;
                TransportDriverManagementDTO tempDriver1 = tempListDriver.Where(x => x.ID == tempTE.IDDriver1).FirstOrDefault();
                TransportDriverManagementDTO tempDriver2 = tempListDriver.Where(x => x.ID == tempTE.IDDriver2).FirstOrDefault();
                TransportDriverManagementDTO tempCoDriver = tempListDriver.Where(x => x.ID == tempTE.IDCoDriver).FirstOrDefault();
                tempTONoGroup.DriverName1 = (tempDriver1 == null) ? "" : tempDriver1.Name;
                tempTONoGroup.IDDriver1 = (tempDriver1 == null) ? "" : tempDriver1.ID;
                //tempTONoGroup.DriverName2 = tempTE.TransportDriverManagement1.Name;
                tempTONoGroup.DriverName2 = (tempDriver2 == null) ? "" : tempDriver2.Name;
                tempTONoGroup.IDDriver2 = (tempDriver2 == null) ? "" : tempDriver2.ID;
                //tempTONoGroup.CoDriverName = tempTE.TransportDriverManagement2.Name;
                tempTONoGroup.CoDriverName = (tempCoDriver == null) ? "" : tempCoDriver.Name;
                tempTONoGroup.CoDriverName = (tempCoDriver == null) ? "" : tempCoDriver.ID;
                var startLoc = _masterLocationRepo.GetMasterLocationByID(tempTE.StartLocation);
                tempTONoGroup.StartLocation = startLoc.LocationName;
                GenerateETransportCard tGen2 = new GenerateETransportCard();
                tempTONoGroup.TNBarcode = tGen2.getPreviewBarcode(tempTE.TransportNo);

                List<TransportOrder> ListTOGate = _transportOrderRepo.GetTransportOrderByidTE(tempTE.IDTransportExecution);
                foreach (TransportOrder TempTo in ListTOGate)
                {
                    TransportOrderDTO tempTOGate = new TransportOrderDTO();
                    tempTOGate.STONo = TempTo.STONo;
                    var sender = _masterLocationRepo.GetMasterLocationByID(TempTo.ActualSenderIDLocation);
                    tempTOGate.SenderLocationName = sender.LocationName;
                    var receiver = _masterLocationRepo.GetMasterLocationByID(TempTo.ActualReceiverIDLocation);
                    tempTOGate.ReceiverLocationName = receiver.LocationName;
                    tempListTOGate.Add(tempTOGate);
                }
                tempListTONOGroup.Add(tempTONoGroup);
                totalGatePass++;
            }

            temp.SampoernaPIC = username;
            temp.TotalPage = totalPage;
            temp.TotalGatePass = totalGatePass;
            temp.ListTOGate = tempListTOGate;
            temp.ListTOGroup = tempListTO;
            temp.ListTONoGroup = tempListTONOGroup;
            temp.ListTOD = tempListDetail;
            return temp;
        }

        public TransportExecutionPrintDTO PrintDocumentIPB(int idTE, string username)
        {
            TransportExecutionPrintDTO temp = new TransportExecutionPrintDTO();
            List<TransportExecutionPrintMemoDTO> tempListTE = new List<TransportExecutionPrintMemoDTO>();
            List<TransportOrderDetailPrintDTO> tempListDetail = new List<TransportOrderDetailPrintDTO>();

            //List<TransportExecutionPrintMemoDTO> listTE = _transportExecutionRepo.GetTransportExecutionByidTE(idTE);
            TransportExecutionPrintMemoDTO memo = _transportExecutionRepo.GetTransportExecutionByidTE(idTE);
            var context = new TOMContextDB();
            var tempResult = (
                            from to1 in context.TransportOrders
                            join te in context.TransportExecutions
                            on to1.IDTransportExecution equals te.IDTransportExecution
                            join ml1 in context.MasterLocations
                            on to1.ActualReceiverIDLocation equals ml1.IDLocation
                            where to1.IsActive == true && to1.IDTransportExecution == idTE && ml1.Type == "Agent"
                            orderby to1.ActualReceiverIDLocation
                            select new TransportExecutionPrintMemoDTO
                            {
                                IDTransportExecution = te.IDTransportExecution,
                                ReceiverIDLocation = to1.ActualReceiverIDLocation,
                                ReceiverName = ml1.LocationName,
                                GRDate = to1.GRDate,
                                POWeek = to1.POWeek,
                                Remarks = to1.Remarks
                            }
                         ).ToList();
            var resultTO = (from t in tempResult
                            group t by t.ReceiverIDLocation into g
                            select new TransportExecutionPrintMemoDTO
                            {
                                IDTransportExecution = g.First().IDTransportExecution,
                                ReceiverIDLocation = g.First().ReceiverIDLocation,
                                ReceiverName = g.First().ReceiverName,
                                GRDate = g.First().GRDate,
                                POWeek = g.First().POWeek,
                                Remarks = g.First().Remarks
                            }).ToList();

            int totalPage = 0;

            foreach (TransportExecutionPrintMemoDTO rowTO in resultTO)
            {
                TransportExecutionPrintMemoDTO tempTE = new TransportExecutionPrintMemoDTO();
                tempTE.IDTransportExecution = memo.IDTransportExecution;
                tempTE.TransportDate = memo.TransportDate;
                tempTE.TransportNo = memo.TransportNo;
                tempTE.ReceiverIDLocation = rowTO.ReceiverIDLocation;
                tempTE.ReceiverName = rowTO.ReceiverName;
                tempTE.Week = memo.Week;
                tempTE.Year = memo.Year;
                //tempTE.ReceiverTE = memo.ReceiverTE;

                MasterMapping mstMap = _masterMappingRepo.CheckAvailabilityMasterMappingByMapFrom(rowTO.ReceiverIDLocation);
                tempTE.CompanyName = (mstMap == null) ? "" : mstMap.MapTo;
                MasterGenWeek mstWeek = _masterGenWeekRepo.GetGenWeekNowByDate(memo.TransportDate);
                tempTE.WeekTransportDate = mstWeek.Week;
                //tempTE.dateFromWeek = mstWeek.StartDate.Value.ToString("dd");
                tempTE.dateWeek = mstWeek.StartDate.Value.ToString("dd") + " - " + mstWeek.EndDate.Value.ToString("dd MMM yy");
                tempTE.VesselName = memo.TransportVesselMonitoring.VesselName;
                tempTE.VendorName = memo.MasterVendor.VendorName;
                tempTE.ATDDate = (memo.TransportVesselMonitoring.ATD.HasValue) ? memo.TransportVesselMonitoring.ATD.Value.ToString("dd-MMM-yyyy") : "";
                tempTE.GRDateTO = (rowTO.GRDate.HasValue) ? rowTO.GRDate.Value.ToString("dd-MMM-yyyy") : "";
                //tempTE.GRDate = rowTO.GRDate;

                //List<TransportOrderDetailPrintDTO> ListDetail = _transportOrderDetailRepo.GetTransportOrderDetailByidTE(memo.IDTransportExecution);
                var dbResult = (
                                    from to1 in context.TransportOrders
                                    join tod in context.TransportOrderDetails
                                    on to1.IDTransportOrder equals tod.IDTransportOrder
                                    where to1.IsActive == true && tod.IsActive == true && to1.IDTransportExecution == rowTO.IDTransportExecution && to1.ReceiverIDLocation == rowTO.ReceiverIDLocation
                                    orderby to1.STONo
                                    select new TransportOrderDetailPrintDTO
                                    {
                                        ReceiverIDLocation = to1.ActualReceiverIDLocation,
                                        STONo = to1.STONo,
                                        Description = tod.Description,
                                        Qty = tod.Qty,
                                        UoM = tod.UoM
                                    }
                                )
                                .ToList();
                foreach (TransportOrderDetailPrintDTO tod in dbResult)
                {
                    TransportOrderDetailPrintDTO tempTOD = new TransportOrderDetailPrintDTO();
                    tempTOD.ReceiverIDLocation = tod.ReceiverIDLocation;
                    tempTOD.STONo = tod.STONo;
                    tempTOD.Description = tod.Description;
                    tempTOD.Qty = tod.Qty;
                    tempTOD.UoM = tod.UoM;
                    tempTOD.POWeek = rowTO.POWeek;
                    tempTOD.Remarks = rowTO.Remarks;
                    tempListDetail.Add(tempTOD);
                }
                tempListTE.Add(tempTE);
                totalPage++;
            }
            temp.SampoernaPIC = username;
            temp.TotalPage = totalPage;
            temp.ListTE = tempListTE;
            temp.ListTOD = tempListDetail;
            return temp;
        }

        public MasterGenWeek GetWeekNow()
        {
            //DateTime dateNow = DateTime.Now;
            DateTime dateNow = DateTime.Today;
            return _masterGenWeekRepo.GetGenWeekNowByDate(dateNow);
        }

        #region SUB SI
        public List<TransportExecutionDTO> GetVendorSI(List<UserRole> userRole)
        {
            var context = new TOMContextDB();
            List<TransportExecutionDTO> dbResult = new List<TransportExecutionDTO>();
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            if (userRole[0].RoleName == "SUPER ADMIN" || UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                dbResult = (
                                    from mv in context.MasterVendors
                                    where mv.IsActive == true && mv.ParentVendor == null
                                    orderby mv.VendorName
                                    select new TransportExecutionDTO
                                    {
                                        IDVendor = mv.IDVendor,
                                        VendorName = mv.VendorName,
                                        TransportMode = mv.TransportationMode
                                    }
                                )
                                .ToList();
            }
            else
            {
                var rolex = userRole[0].RoleName;
                dbResult = (
                                    from mc in context.MasterConfigurations
                                    join mv in context.MasterVendors
                                    on mc.Value equals mv.VendorName
                                    where mc.PageName == "RoleVendorMapping" && mc.Description == rolex
                                    orderby mv.VendorName
                                    select new TransportExecutionDTO
                                    {
                                        IDVendor = mv.IDVendor,
                                        VendorName = mv.VendorName,
                                        TransportMode = mv.TransportationMode
                                    }
                                )
                                .ToList();
            }
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                dbResult = dbResult.Where(x => UserRole.ToLower().Contains(x.TransportMode.ToLower())).ToList();
            }

            return dbResult;
            /*var dbResult = (
                        from TE in context.TransportExecutions
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN
                        from MV in RJOIN.DefaultIfEmpty()
                        where TE.IsActive == true && MV.ParentVendor == null
                        select new
                        {
                            IDVendor = TE.IDVendor,
                            VendorName = MV.VendorName
                        }
                        );

            return dbResult
                .GroupBy(g => g.IDVendor)
                .Select(f => new TransportExecutionDTO
                {
                    IDVendor = f.FirstOrDefault().IDVendor,
                    VendorName = f.FirstOrDefault().VendorName
                })
                .OrderBy(o => o.IDVendor)
                .ToList();*/
        }

        public List<MasterGenWeekDTO> GetWeekMstGen(int year)
        {
            var queryFilter = PredicateHelper.True<MasterGenWeek>();
            queryFilter = queryFilter.And(q => q.Year == year);
            var dbResult = _masterGenWeekRepo.Get(queryFilter).Select(f => new MasterGenWeekDTO { Week = f.Week }).OrderBy(o => o.Week).ToList();
            return dbResult;
        }

        public MasterGenWeek GetMasterGenWeek(int week, int year)
        {
            MasterGenWeek dateFilter = new MasterGenWeek();
            dateFilter = _masterGenWeekRepo.GetFromToDateByWeekYear(week, year);
            return dateFilter;
        }
        public MasterVendor GetVendor(int idVendor)
        {
            MasterVendor mv = _masterVendorRepo.GetMasterVendorByID(idVendor);
            return mv;
        }
        public MasterVendor GetVendorByZone(int idVendor, string StartLocation = "")
        {
            //string zoneBased = _masterLocationRepo.GetEastWestLocation(StartLocation);
            MasterVendor mv = _masterVendorRepo.GetMasterVendorByZone(idVendor);
            return mv;
        }
        public List<TransportExecutionDTO> GetExportSI(int fltrvn, int fltrwk, int fltryr)
        {
            var context = new TOMContextDB();

            var dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        from TO in LJOIN_1.DefaultIfEmpty()
                        join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder into LJOIN_2
                        from TOD in LJOIN_2.DefaultIfEmpty()
                        join MLSE in context.MasterLocations on TO.SenderIDLocation equals MLSE.IDLocation into RJOIN_1
                        from MLSE in RJOIN_1.DefaultIfEmpty()
                        join MLRE in context.MasterLocations on TO.ReceiverIDLocation equals MLRE.IDLocation into RJOIN_2
                        from MLRE in RJOIN_2.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_3
                        from MV in RJOIN_3.DefaultIfEmpty()
                        join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_4
                        from TDTE1 in RJOIN_4.DefaultIfEmpty()
                        join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_5
                        from TDTE2 in RJOIN_5.DefaultIfEmpty()
                        join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_6
                        from TDTE3 in RJOIN_6.DefaultIfEmpty()
                        where TE.IDVendor == fltrvn && TE.SIWeek == fltrwk && TE.Year == fltryr && TO.IsActive == true && TOD.IsActive == true
                        select new TransportExecutionDTO
                        {
                            IDTransportOrder = TO.IDTransportOrder,
                            IDTransportOrderDetail = TOD.IDTransportOrderDetail,
                            TransportDate = TE.TransportDate,
                            TransportNo = TE.TransportNo,
                            SIWeek = TE.SIWeek,
                            VendorName = MV.VendorName,
                            VendorAddress = MV.VendorAddress,
                            VendorCity = MV.VendorCity,
                            VendorEmail = MV.VendorEmail,
                            VendorRegion = MV.VendorRegion,
                            Sender = MLSE.LocationName,
                            Receiver = MLRE.LocationName,
                            VehicleType = TO.VehicleType,
                            MaterialType = TOD.MaterialType,
                            SIType = TE.SIType,
                            TargetOfArrival = TE.TargetOfArrival,
                            PoliceRegNo = TE.PoliceRegNo,
                            IDDriver1 = TDTE1.Name,
                            IDDriver2 = TDTE2.Name,
                            IDCoDriver = TDTE3.Name,
                            Remarks = TE.Remarks,
                            IDTransportExecution = TE.IDTransportExecution
                        }
                        );
            return dbResult
                .OrderBy(o => o.TransportDate)
                .OrderBy(o => o.TransportNo)
                .OrderBy(o => o.IDTransportOrder)
                .OrderBy(o => o.IDTransportOrderDetail)
                .ToList();
        }

        public List<TransportExecutionExportSIDTO> GetExportSI(DateTime transportDate, int vendorID, bool isTransport, List<string> regionList, List<string> tncreator)
        {
            var context = new TOMContextDB();

            List<TransportExecutionExportSIDTO> res = new List<TransportExecutionExportSIDTO>();

            var UserRole = GetListUserRole().FirstOrDefault().RoleName;

            if (regionList == null) regionList = new List<string>();
            if (!isTransport && regionList.Count <= 0 && !UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
                return res;

            #region Get ChangeLogs
            var tmp = (from TC in context.TransportVendorChangeLogs
                       join TE in context.TransportExecutions on TC.IDTransportExecution equals TE.IDTransportExecution
                       where TE.IsActive && tncreator.Contains(TE.CreatedBy) && TC.IDVendor == vendorID && TC.TransportDate == transportDate
                       select new TransportExecutionExportSIDTO()
                       {
                           TransportDate = TC.TransportDate,
                           TransportNo = TE.TransportNo,
                           SIWeek = TE.SIWeek,
                           SIStatus = TC.SIStatus,
                           SIType = TC.SIType,
                           TargetOfArrival = TC.TargetOfArrival,
                           PoliceRegNo = TE.PoliceRegNo,
                           Remarks = TE.Remarks,
                           IDTransportExecution = TC.IDTransportExecution,
                           IDVendor = TC.IDVendor,
                           VehicleType = TE.ActualVehicleType,
                           IsChangelog = true,

                           IDCoDriver = TE.IDCoDriver,
                           IDDriver1 = TE.IDDriver1,
                           IDDriver2 = TE.IDDriver2,

                           Via = TE.Via,
                           ContainerNo = TE.TransportVesselMonitorings.Count > 0 ? TE.TransportVesselMonitorings.FirstOrDefault().ContainerNo : ""
                       }
                        ).ToList().Where(d => !string.IsNullOrWhiteSpace(d.SIStatus));
            res.AddRange(tmp);
            #endregion

            #region Get Real TE
            tmp = (from TE in context.TransportExecutions
                   where TE.IsActive && tncreator.Contains(TE.CreatedBy) && TE.IDVendor == vendorID && TE.TransportDate == transportDate
                   select new TransportExecutionExportSIDTO()
                   {
                       TransportDate = TE.TransportDate,
                       TransportNo = TE.TransportNo,
                       SIWeek = TE.SIWeek,
                       SIStatus = TE.SIStatus,
                       SIType = TE.SIType,
                       TargetOfArrival = TE.TargetOfArrival,
                       PoliceRegNo = TE.PoliceRegNo,
                       Remarks = TE.Remarks,
                       IDTransportExecution = TE.IDTransportExecution,
                       IDVendor = TE.IDVendor,
                       VehicleType = TE.ActualVehicleType,
                       IsChangelog = false,

                       IDCoDriver = TE.IDCoDriver,
                       IDDriver1 = TE.IDDriver1,
                       IDDriver2 = TE.IDDriver2,

                       Via = TE.Via,
                       ContainerNo = TE.TransportVesselMonitorings.Count > 0 ? TE.TransportVesselMonitorings.FirstOrDefault().ContainerNo : ""
                   }
                        ).ToList();
            res.AddRange(tmp);
            #endregion

            // sort by transport No
            res.Sort((a, b) => a.TransportNo != null ? a.TransportNo.CompareTo(b.TransportNo) : -1);

            // fill TO and its location
            foreach (var re in res)
            {
                var tos = context.TransportOrders.Where(to => to.IsActive && to.IDTransportExecution == re.IDTransportExecution).ToList();
                re.TransportOrders = Mapper.Map<List<TransportOrderDTO>>(tos);

                var vend = context.MasterVendors.Where(v => v.IDVendor == re.IDVendor).FirstOrDefault();
                if (vend != null)
                {
                    re.VendorName = vend.VendorName;
                    re.VendorAddress = vend.VendorAddress;
                    re.VendorCity = vend.VendorCity;
                    re.VendorEmail = vend.VendorEmail;
                    re.VendorRegion = vend.VendorRegion;
                }

                var drv = context.TransportDriverManagements.Where(d => d.ID == re.IDDriver1).FirstOrDefault();
                if (drv != null) re.Driver1 = drv.Name;

                drv = context.TransportDriverManagements.Where(d => d.ID == re.IDDriver2).FirstOrDefault();
                if (drv != null) re.Driver2 = drv.Name;

                drv = context.TransportDriverManagements.Where(d => d.ID == re.IDCoDriver).FirstOrDefault();
                if (drv != null) re.CoDriver = drv.Name;

                foreach (var to in re.TransportOrders)
                {
                    var loc = context.MasterLocations.Where(l => l.IDLocation == to.ActualReceiverIDLocation).FirstOrDefault();
                    if (loc != null) to.ReceiverLocationName = loc.LocationName;

                    loc = context.MasterLocations.Where(l => l.IDLocation == to.ActualSenderIDLocation).FirstOrDefault();
                    if (loc != null) to.SenderLocationName = loc.LocationName;
                }
            }

            if (!isTransport && !UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                // filter by location
                res = res.Where(re => re.TransportOrders != null &&
                    re.TransportOrders.Where(to =>
                        regionList.Contains(to.ActualSenderIDLocation) ||
                        regionList.Contains(
                        context.MasterLocations.Where(lc => lc.IDLocation == to.ActualReceiverIDLocation)
                        .Select(lc => lc.IDLocation).FirstOrDefault()
                        )
                    ).Count() > 0

                    &&

                    re.TransportOrders.Where(to =>
                        regionList.Contains(to.ActualReceiverIDLocation) ||
                        regionList.Contains(
                        context.MasterLocations.Where(lc => lc.IDLocation == to.ActualReceiverIDLocation)
                        .Select(lc => lc.IDLocation).FirstOrDefault()
                        )
                    ).Count() > 0
                ).ToList();
            }

            return res;
        }

        public List<TransportExecutionDTO> GetTEChangelogForExportSI(DateTime transDate, int idVendor, bool isTransport, List<string> regionList, string startLoc)
        {
            var context = new TOMContextDB();
            IQueryable<TransportExecutionDTO> res = null;

            if (!isTransport)
            {
                if (regionList != null && regionList.Count > 0)
                {
                    res = (
                        from TC in context.TransportVendorChangeLogs
                        join TE in context.TransportExecutions on TC.IDTransportExecution equals TE.IDTransportExecution
                        join TO in context.TransportOrders on TC.IDTransportExecution equals TO.IDTransportExecution
                        join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation
                        join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation
                        join MV in context.MasterVendors on TC.IDVendor equals MV.IDVendor
                        join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_4
                        from TDTE1 in RJOIN_4.DefaultIfEmpty()
                        join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_5
                        from TDTE2 in RJOIN_5.DefaultIfEmpty()
                        join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_6
                        from TDTE3 in RJOIN_6.DefaultIfEmpty()
                        where TO.IsActive && regionList.Contains(MLRE.ParentLocation) && regionList.Contains(MLSE.ParentLocation) &&
                              (TC.TransportDate == transDate && TC.IDVendor == idVendor && TE.IsActive && TE.StartLocation == startLoc)
                        select new TransportExecutionDTO()
                        {
                            TransportDate = TC.TransportDate,
                            TransportNo = TE.TransportNo,
                            SIWeek = TE.SIWeek,
                            SIStatus = TC.SIStatus,
                            SIType = TC.SIType,
                            TargetOfArrival = TC.TargetOfArrival,
                            PoliceRegNo = TE.PoliceRegNo,
                            Remarks = TE.Remarks,
                            IDTransportExecution = TC.IDTransportExecution,
                            IDVendor = TC.IDVendor,

                            IDTransportOrder = TO.IDTransportOrder,
                            OrderType = TO.OrderType,
                            VendorName = MV.VendorName,
                            VendorAddress = MV.VendorAddress,
                            VendorCity = MV.VendorCity,
                            VendorEmail = MV.VendorEmail,
                            VendorRegion = MV.VendorRegion,
                            Sender = MLSE.LocationName,
                            Receiver = MLRE.LocationName,
                            VehicleType = TO.VehicleType,
                            IDDriver1 = TDTE1.Name,
                            IDDriver2 = TDTE2.Name,
                            IDCoDriver = TDTE3.Name,

                            IDCheckSave = TC.IDVendorChangeLog
                        }
                             );
                }
            }
            else
            {
                res = (
                        from TC in context.TransportVendorChangeLogs
                        join TE in context.TransportExecutions on TC.IDTransportExecution equals TE.IDTransportExecution
                        join TO in context.TransportOrders on TC.IDTransportExecution equals TO.IDTransportExecution
                        join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation
                        join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation
                        join MV in context.MasterVendors on TC.IDVendor equals MV.IDVendor
                        join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_4
                        from TDTE1 in RJOIN_4.DefaultIfEmpty()
                        join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_5
                        from TDTE2 in RJOIN_5.DefaultIfEmpty()
                        join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_6
                        from TDTE3 in RJOIN_6.DefaultIfEmpty()
                        where TO.IsActive &&
                        (TC.TransportDate == transDate && TC.IDVendor == idVendor && TE.IsActive && TE.StartLocation == startLoc)
                        select new TransportExecutionDTO()
                        {
                            TransportDate = TC.TransportDate,
                            TransportNo = TE.TransportNo,
                            SIWeek = TE.SIWeek,
                            SIStatus = TC.SIStatus,
                            SIType = TC.SIType,
                            TargetOfArrival = TC.TargetOfArrival,
                            PoliceRegNo = TE.PoliceRegNo,
                            Remarks = TE.Remarks,
                            IDTransportExecution = TC.IDTransportExecution,
                            IDVendor = TC.IDVendor,

                            IDTransportOrder = TO.IDTransportOrder,
                            OrderType = TO.OrderType,
                            VendorName = MV.VendorName,
                            VendorAddress = MV.VendorAddress,
                            VendorCity = MV.VendorCity,
                            VendorEmail = MV.VendorEmail,
                            VendorRegion = MV.VendorRegion,
                            Sender = MLSE.LocationName,
                            Receiver = MLRE.LocationName,
                            VehicleType = TO.VehicleType,
                            IDDriver1 = TDTE1.Name,
                            IDDriver2 = TDTE2.Name,
                            IDCoDriver = TDTE3.Name,

                            IDCheckSave = TC.IDVendorChangeLog
                        }
                         );
            }

            res = res.OrderBy(c => c.TransportNo);
            return res == null ? new List<TransportExecutionDTO>() : res.ToList();
        }

        public List<TransportExecutionDTO> GetDataTEByVendorTransportDate(DateTime transportDate, int idVendor, bool IsRoleTransport, List<string> RegionList, string StartLocation)
        {
            var context = new TOMContextDB();
            List<TransportExecutionDTO> dbResult = new List<TransportExecutionDTO>();
            if (!IsRoleTransport && RegionList != null)
            {
                if (RegionList.Count > 0)
                {
                    dbResult = (
                        (from TE in context.TransportExecutions
                         join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution
                         join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder
                         join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation
                         join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation
                         join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor
                         join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_4
                         from TDTE1 in RJOIN_4.DefaultIfEmpty()
                         join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_5
                         from TDTE2 in RJOIN_5.DefaultIfEmpty()
                         join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_6
                         from TDTE3 in RJOIN_6.DefaultIfEmpty()
                         where TE.IDVendor == idVendor && TE.TransportDate == transportDate && TO.IsActive == true && TOD.IsActive == true && TE.StartLocation == StartLocation && (RegionList.Contains(MLSE.ParentLocation) || RegionList.Contains(MLRE.ParentLocation))
                         select new TransportExecutionDTO
                         {
                             IDTransportOrder = TO.IDTransportOrder,
                             IDTransportOrderDetail = TOD.IDTransportOrderDetail,
                             TransportDate = TE.TransportDate,
                             TransportNo = TE.TransportNo,
                             SIWeek = TE.SIWeek,
                             SIStatus = TE.SIStatus,
                             VendorName = MV.VendorName,
                             VendorAddress = MV.VendorAddress,
                             VendorCity = MV.VendorCity,
                             VendorEmail = MV.VendorEmail,
                             VendorRegion = MV.VendorRegion,
                             Sender = MLSE.LocationName,
                             Receiver = MLRE.LocationName,
                             VehicleType = TO.VehicleType,
                             MaterialType = TOD.MaterialType,
                             SIType = TE.SIType,
                             TargetOfArrival = TE.TargetOfArrival,
                             PoliceRegNo = TE.PoliceRegNo,
                             IDDriver1 = TDTE1.Name,
                             IDDriver2 = TDTE2.Name,
                             IDCoDriver = TDTE3.Name,
                             Remarks = TE.Remarks,
                             IDTransportExecution = TE.IDTransportExecution
                         }
                        // ).Union(
                        //from TE in context.TransportVendorChangeLogs
                        //join TE1 in context.TransportExecutions on TE.IDTransportExecution equals TE1.IDTransportExecution
                        //join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        //from TO in LJOIN_1.DefaultIfEmpty()
                        //join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder into LJOIN_2
                        //from TOD in LJOIN_2.DefaultIfEmpty()
                        //join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation into RJOIN_1
                        //from MLSE in RJOIN_1.DefaultIfEmpty()
                        //join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation into RJOIN_2
                        //from MLRE in RJOIN_2.DefaultIfEmpty()
                        //join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_3
                        //from MV in RJOIN_3.DefaultIfEmpty()
                        //where TE.IDVendor == idVendor && TE.TransportDate == transportDate && TO.IsActive == true && TOD.IsActive == true
                        //select new TransportExecutionDTO
                        //{
                        //    IDTransportOrder = TO.IDTransportOrder,
                        //    IDTransportOrderDetail = TOD.IDTransportOrderDetail,
                        //    TransportDate = TE.TransportDate,
                        //    TransportNo = TE1.TransportNo,
                        //    SIWeek = TE1.SIWeek,
                        //    SIStatus = TE.SIStatus,
                        //    VendorName = MV.VendorName,
                        //    VendorAddress = MV.VendorAddress,
                        //    VendorCity = MV.VendorCity,
                        //    VendorEmail = MV.VendorEmail,
                        //    VendorRegion = MV.VendorRegion,
                        //    Sender = MLSE.LocationName,
                        //    Receiver = MLRE.LocationName,
                        //    VehicleType = TO.VehicleType,
                        //    MaterialType = TOD.MaterialType,
                        //    SIType = TE.SIType,
                        //    TargetOfArrival = TE.TargetOfArrival,
                        //    PoliceRegNo = "",
                        //    IDDriver1 = "",
                        //    IDDriver2 = "",
                        //    IDCoDriver = "",
                        //    Remarks = "",
                        //    IDTransportExecution = TE.IDTransportExecution
                        //}
                        )).ToList();
                }
            }
            else
            {
                dbResult = (
                        from TE in context.TransportExecutions
                        join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        from TO in LJOIN_1.DefaultIfEmpty()
                        join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder into LJOIN_2
                        from TOD in LJOIN_2.DefaultIfEmpty()
                        join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation into RJOIN_1
                        from MLSE in RJOIN_1.DefaultIfEmpty()
                        join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation into RJOIN_2
                        from MLRE in RJOIN_2.DefaultIfEmpty()
                        join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_3
                        from MV in RJOIN_3.DefaultIfEmpty()
                        join TDTE1 in context.TransportDriverManagements on TE.IDDriver1 equals TDTE1.ID into RJOIN_4
                        from TDTE1 in RJOIN_4.DefaultIfEmpty()
                        join TDTE2 in context.TransportDriverManagements on TE.IDDriver2 equals TDTE2.ID into RJOIN_5
                        from TDTE2 in RJOIN_5.DefaultIfEmpty()
                        join TDTE3 in context.TransportDriverManagements on TE.IDCoDriver equals TDTE3.ID into RJOIN_6
                        from TDTE3 in RJOIN_6.DefaultIfEmpty()
                        where TE.IDVendor == idVendor && TE.TransportDate == transportDate && TO.IsActive == true && TOD.IsActive == true && TE.StartLocation == StartLocation
                        select new TransportExecutionDTO
                        {
                            IDTransportOrder = TO.IDTransportOrder,
                            IDTransportOrderDetail = TOD.IDTransportOrderDetail,
                            TransportDate = TE.TransportDate,
                            TransportNo = TE.TransportNo,
                            SIWeek = TE.SIWeek,
                            SIStatus = TE.SIStatus,
                            VendorName = MV.VendorName,
                            VendorAddress = MV.VendorAddress,
                            VendorCity = MV.VendorCity,
                            VendorEmail = MV.VendorEmail,
                            VendorRegion = MV.VendorRegion,
                            Sender = MLSE.LocationName,
                            Receiver = MLRE.LocationName,
                            VehicleType = TO.VehicleType,
                            MaterialType = TOD.MaterialType,
                            SIType = TE.SIType,
                            TargetOfArrival = TE.TargetOfArrival,
                            PoliceRegNo = TE.PoliceRegNo,
                            IDDriver1 = TDTE1.Name,
                            IDDriver2 = TDTE2.Name,
                            IDCoDriver = TDTE3.Name,
                            Remarks = TE.Remarks,
                            IDTransportExecution = TE.IDTransportExecution
                        }
                        //).Union(
                        //from TE in context.TransportVendorChangeLogs
                        //join TE1 in context.TransportExecutions on TE.IDTransportExecution equals TE1.IDTransportExecution
                        //join TO in context.TransportOrders on TE.IDTransportExecution equals TO.IDTransportExecution into LJOIN_1
                        //from TO in LJOIN_1.DefaultIfEmpty()
                        //join TOD in context.TransportOrderDetails on TO.IDTransportOrder equals TOD.IDTransportOrder into LJOIN_2
                        //from TOD in LJOIN_2.DefaultIfEmpty()
                        //join MLSE in context.MasterLocations on TO.ActualSenderIDLocation equals MLSE.IDLocation into RJOIN_1
                        //from MLSE in RJOIN_1.DefaultIfEmpty()
                        //join MLRE in context.MasterLocations on TO.ActualReceiverIDLocation equals MLRE.IDLocation into RJOIN_2
                        //from MLRE in RJOIN_2.DefaultIfEmpty()
                        //join MV in context.MasterVendors on TE.IDVendor equals MV.IDVendor into RJOIN_3
                        //from MV in RJOIN_3.DefaultIfEmpty()
                        //where TE.IDVendor == idVendor && TE.TransportDate == transportDate && TO.IsActive == true && TOD.IsActive == true
                        //select new TransportExecutionDTO
                        //{
                        //    IDTransportOrder = TO.IDTransportOrder,
                        //    IDTransportOrderDetail = TOD.IDTransportOrderDetail,
                        //    TransportDate = TE.TransportDate,
                        //    TransportNo = TE1.TransportNo,
                        //    SIWeek = TE1.SIWeek,
                        //    SIStatus = TE.SIStatus,
                        //    VendorName = MV.VendorName,
                        //    VendorAddress = MV.VendorAddress,
                        //    VendorCity = MV.VendorCity,
                        //    VendorEmail = MV.VendorEmail,
                        //    VendorRegion = MV.VendorRegion,
                        //    Sender = MLSE.LocationName,
                        //    Receiver = MLRE.LocationName,
                        //    VehicleType = TO.VehicleType,
                        //    MaterialType = TOD.MaterialType,
                        //    SIType = TE.SIType,
                        //    TargetOfArrival = TE.TargetOfArrival,
                        //    PoliceRegNo = "",
                        //    IDDriver1 = "",
                        //    IDDriver2 = "",
                        //    IDCoDriver = "",
                        //    Remarks = "",
                        //    IDTransportExecution = TE.IDTransportExecution
                        //}
                        ).ToList();
            }
            var ResultGroup = dbResult
                .GroupBy(x => new { x.TransportNo, x.Sender, x.Receiver, x.VehicleType, x.MaterialType, x.SIType, x.SIStatus })
                .Select(x => x.FirstOrDefault());
            return ResultGroup
                .OrderByDescending(o => o.TransportNo)
                .ToList();
            /*return dbResult
                .OrderBy(o => o.TransportDate)
                .OrderBy(o => o.TransportNo)
                .OrderBy(o => o.IDTransportOrder)
                .OrderBy(o => o.IDTransportOrderDetail)
                .ToList();*/
        }
        public bool SendEmailSI(string mailto, string vendor, string reg, string week, string tglawal, string tglakhir, string user, string files)
        {
            var context = new TOMContextDB();
            var sendmail = context.SendEmailTransportExecutionSI(mailto, vendor, reg, week, tglawal, tglakhir, user, files);
            return true;
        }
        #endregion SUB SI

        public List<MasterVendor> GetVendorBySuggestionList(int idTE)
        {
            // get master configuration
            /*List<MasterConfigurationDTO> tempConfig = Mapper.Map<List<MasterConfigurationDTO>>(_masterConfigurationRepo.GetListMasterConfigurationByPageNameDescription(EnumHelper.GetDescription(Enums.MasterConfigurationValue.RoleVendorMapping), userRole[0].RoleName));
            if (tempConfig.Count > 0)
            {
                ///return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorByListVendorName(tempConfig.Select(x => x.Value).ToList()));
            }*/

            List<MasterVendor> tempListVendor = new List<MasterVendor>();
            TransportExecution dataTE = _transportExecutionRepo.GetTransportExecutionByID(idTE);
            List<TransportOrder> dataTO = _transportOrderRepo.GetTransportOrderFilterByTE(idTE);
            //var prevVendor = 0;            
            List<int> listVendor = new List<int>();
            foreach (TransportOrder tempTO in dataTO)
            {
                List<MasterVendorSuggestion> vendorSuggest = _masterVendorSuggestionRepo.GetMasterVendorSuggestionListByField(dataTE.StartLocation, tempTO.ReceiverIDLocation, tempTO.OrderType, dataTE.TransportCategory, dataTE.TransportMode, tempTO.VehicleType);
                if (vendorSuggest.Count > 0)
                {
                    foreach (MasterVendorSuggestion mstVendor in vendorSuggest)
                    {
                        MasterVendor tempVendor = new MasterVendor();
                        if (!listVendor.Contains(mstVendor.SuggestedVendor))
                        {
                            listVendor.Add(mstVendor.SuggestedVendor);
                            tempVendor.IDVendor = mstVendor.SuggestedVendor;
                            tempVendor.VendorName = mstVendor.MasterVendor.VendorName;
                            tempListVendor.Add(tempVendor);
                        }
                    }
                }
            }
            return tempListVendor;
        }

        public void SaveVendorTE(List<TransportationExecutionInput> input, string userid)
        {
            foreach (TransportationExecutionInput execInput in input)
            {
                TransportExecution temp = _transportExecutionRepo.GetTransportExecutionByID(execInput.IDTransportExecution);
                var servPO = _masterServicePORepo.GetMasterServicePOByVendorEffectiveDate(execInput.IDVendor.Value, temp.TransportDate);
                var mstVendor = _masterVendorRepo.GetMasterVendorByID(execInput.IDVendor.Value);
                temp.IDVendor = execInput.IDVendor;
                temp.UpdatedBy = userid;
                //decimal totalKM = 0;
                if (servPO == null)
                    temp.ServicePONo = "";
                else
                    temp.ServicePONo = servPO.ServicePONumber;
                _transportExecutionRepo.SaveData(temp);
                List<TransportOrder> dataTO = _transportOrderRepo.GetTransportOrderFilterByTE(execInput.IDTransportExecution);
                foreach (TransportOrder tempTO in dataTO)
                {
                    TransportOrder rowTO = _transportOrderRepo.GetTransportOrderByID(tempTO.IDTransportOrder);
                    MasterLeadTime mstLeadTime = _masterLeadTimeRepo.GetMasterLeadTimeFilterByTOTE(execInput.IDVendor, rowTO.SenderIDLocation, rowTO.ReceiverIDLocation, rowTO.ShipmentDate);
                    //var mstDistance = _masterDistanceRepo.GetMasterDistanceByField(rowTO.ActualSenderIDLocation, rowTO.ActualReceiverIDLocation, rowTO.ShipmentDate, mstVendor.VendorCategory);
                    //if (mstDistance != null) totalKM = Convert.ToDecimal(mstDistance.Total);
                    if (mstLeadTime != null)
                    {
                        rowTO.IDLeadTime = (mstLeadTime.IDLeadTime);
                        rowTO.LeadTime = (mstLeadTime.Time);
                    }
                    //rowTO.KM = totalKM;
                    //rowTO.UpdatedBy = userid;
                    _transportOrderRepo.SaveData(rowTO);
                }
            }
        }

        public List<TransportExecutionVendorDTO> GetTotalVendorByTN(TransportOrderInput input)
        {
            List<TransportOrder> listTE = new List<TransportOrder>();
            List<int?> listIdTE = new List<int?>();
            List<TransportExecutionVendorDTO> tempListVendor = new List<TransportExecutionVendorDTO>();
            MasterGenWeek dateFilter = new MasterGenWeek();
            if (input.weekFilter != 0 && input.yearFilter != 0)
            {
                dateFilter = _masterGenWeekRepo.GetFromToDateByWeekYear(input.weekFilter, input.yearFilter);
                if (dateFilter != null)
                {
                    input.dateFromFilter = dateFilter.StartDate;
                    input.dateToFilter = dateFilter.EndDate;
                }
            }
            listTE = _transportOrderRepo.GetIdTEFilterByTO(input);
            foreach (TransportOrder to in listTE)
            {
                listIdTE.Add(to.IDTransportExecution);
            }
            tempListVendor = _transportExecutionRepo.GetTotalVendorTE(listIdTE);
            return tempListVendor;
        }
        public List<TransportExecutionAddNewDTO> SaveDataTemp(List<TransportExecutionAddNewDTO> input, string userid)
        {
            //_transportExecutionTempRepo.DeleteDataByUser(userid);
            int viewOrder = 1;
            var context = new TOMContextDB();
            var getData = context.TransportExecutionTemps
                .Where(m => m.IDUser == userid);
            if (getData != null)
            {
                //_transportExecutionTempRepo.Delete(userid);
                //_transportExecutionTempRepo.Save();                
                var delTemp = context.TransportExecutionTemps.Where(x => x.IDUser == userid);
                context.TransportExecutionTemps.RemoveRange(delTemp);
                context.SaveChanges();
            }
            foreach (TransportExecutionAddNewDTO tempTransExe in input)
            {
                TransportExecutionTemp save = Mapper.Map<TransportExecutionAddNewDTO, TransportExecutionTemp>(tempTransExe);
                /*TransportExecutionTemp save = new TransportExecutionTemp();
                save.IDCheck = tempTransExe.IDCheck.Value;
                save.IDTransportOrder = tempTransExe.IDTransportOrder.Value;
                save.IDTransportOrderDetail = tempTransExe.IDTransportOrderDetail.Value;*/
                /*save.ConcatIDTransportOrder = tempTransExe.ConcatIDTransportOrder;
                save.OrderNumber = tempTransExe.OrderNumber;
                save.OrderType = tempTransExe.OrderType;
                save.IDSenderLoc = tempTransExe.IDSenderLoc;
                save.Sender = tempTransExe.Sender;
                save.IDReceiverLoc = tempTransExe.IDReceiverLoc;
                save.Receiver = tempTransExe.Receiver;*/
                save.IDUser = userid;
                save.CreatedDate = DateTime.Now;
                save.ViewOrder = viewOrder;
                viewOrder++;
                context.TransportExecutionTemps.Add(save);
                //_transportExecutionTempRepo.SaveData(save);
            }
            context.SaveChanges();
            var listNewTN = Mapper.Map<List<TransportExecutionTemp>, List<TransportExecutionAddNewDTO>>(_transportExecutionTempRepo.GetTransportExecutionByUser(userid).OrderBy(x => x.ViewOrder).ToList());
            return listNewTN;
        }
        public List<TransportExecutionAddNewDTO> GetDataTemp(string userid)
        {
            var listNewTN = Mapper.Map<List<TransportExecutionTemp>, List<TransportExecutionAddNewDTO>>(_transportExecutionTempRepo.GetTransportExecutionByUser(userid));
            return listNewTN;
        }
    }

    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        public Func<T, T, bool> CompareFunction { get; set; }

        public LambdaComparer(Func<T, T, bool> comparer)
        {
            CompareFunction = comparer;
        }

        public bool Equals(T x, T y)
        {
            return CompareFunction != null ? CompareFunction.Invoke(x, y) : false;
        }

        public int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }
}
