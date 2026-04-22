using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
//using System.Windows.Forms;
//using System.Xml;
//using DFIS.Transport.Domain.Inputs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Master.Domain.Inputs;
using TOM.Transport.Repositories;
using System.Diagnostics;
using TOM.Transport.Repositories.TransportOrderRequestRepo;
using DFIS.Universal.Domain.DTOs;
using System.Linq.Expressions;
using DFIS.Universal;
using System.Collections;
using TOM.Transport.Repositories.TransportExecutionRepo;
using System.IO;
using System.Web;

namespace TOM.Transport.BusinessLogics
{
    public class TransportOrderBLL : ITransportOrderBLL
    {
        private readonly IGenericRepository<TransportOrder> _genTraOrdRepo;
        private readonly ITransportOrderRequestListViewRepo _lvRepo;
        private readonly ITransportOrderRequestRepo _transportOrderRequestRepo;
        private readonly ITransportOrderRepo _transportOrderRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;
        private readonly IMasterMappingRepo _masterMappingRepo;
        private readonly IMasterCostCenterAccountRepo _masterCostCenterAccountRepo;
        private readonly IMasterUomRepo _masterUomRepo;
        private readonly IMasterFABrandRepo _masterFABrandRepo;
        private readonly ITransportOrderDetailRepo _transportOrderDetailRepo;
        private readonly IGenericRepository<TransportOrderRequest> _genTraOrdReqRepo;
        private readonly IGenericRepository<TransportOrderDetail> _genTraOrdDetRepo;
        private readonly IMasterUserRepo _masterUserRepo;
        private readonly IMasterConfigurationRepo _masterCfgRepo;
        private readonly ITransportExecutionRepo _transportExecutionRepo;
        private readonly ITransportOrderRequestHeaderRepo _transportOrReHeaderRepo;
        private readonly IMasterCostCenterAccountRepo _repoCostCenter;
        private readonly IGenericRepository<LoadingUnloadingExportView> _loadingexcelview;
        private readonly IGenericRepository<MasterUserLocationMapping> _masterUserLocationMappingRepo;

        public TransportOrderBLL(IGenericRepository<MasterUserLocationMapping> masterUserLocationMappingRepo, IMasterCostCenterAccountRepo repoCostCenter, ITransportOrderRequestHeaderRepo transportOrReHeaderRepo, IMasterConfigurationRepo masterCfgRepo, IMasterUserRepo masterUserRepo, IGenericRepository<TransportOrderDetail> genTraOrdDetRepo, IGenericRepository<TransportOrder> generalRepo, IGenericRepository<TransportOrderRequest> genTORRepo, ITransportOrderRequestListViewRepo lvRepo, ITransportOrderRequestRepo transportOrderRequestRepo, ITransportOrderRepo transportOrderRepo, IMasterListRepo masterListRepo, IMasterLocationRepo masterLocationRepo, IMasterMappingRepo masterMappingRepo, IMasterCostCenterAccountRepo masterCostCenterAccountRepo, IMasterUomRepo masterUomRepo, IMasterFABrandRepo masterFABrandRepo, ITransportOrderDetailRepo transportOrderDetailRepo, ITransportExecutionRepo transportExecutionRepo, IGenericRepository<LoadingUnloadingExportView> loadingexcelview)
        {
            _genTraOrdReqRepo = genTORRepo;
            _genTraOrdDetRepo = genTraOrdDetRepo;
            _genTraOrdRepo = generalRepo;
            _transportOrderRequestRepo = transportOrderRequestRepo;
            _transportOrderRepo = transportOrderRepo;
            _masterListRepo = masterListRepo;
            _masterLocationRepo = masterLocationRepo;
            _masterMappingRepo = masterMappingRepo;
            _masterCostCenterAccountRepo = masterCostCenterAccountRepo;
            _masterUomRepo = masterUomRepo;
            _masterFABrandRepo = masterFABrandRepo;
            _lvRepo = lvRepo;
            _transportOrderDetailRepo = transportOrderDetailRepo;
            _masterUserRepo = masterUserRepo;
            _masterCfgRepo = masterCfgRepo;
            _transportExecutionRepo = transportExecutionRepo;
            _transportOrReHeaderRepo = transportOrReHeaderRepo;
            _repoCostCenter = repoCostCenter;
            _loadingexcelview = loadingexcelview;
            _masterUserLocationMappingRepo = masterUserLocationMappingRepo;
        }

        public string UploadData(List<TransportOrderInput> input, List<string> OrderNumbers, String Mode = "Loading")
        {
            var ctx = new TOMContextDB();

            string message = "";
            try
            {
                var updateData = ctx.TransportOrders.Where(f => OrderNumbers.Contains(f.STONo)).ToList();

                foreach (var data in updateData)
                {
                    //var totalBox = _genTraOrdDetRepo.Get().Where(x => x.MaterialType == "Cigarette" && x.UoM == "Box" && x.TransportOrder.STONo == data.STONo).Sum(x => x.Qty);
                    //var totalNonBox = (from a in ctx.TransportOrderDetails
                    //                   join b in ctx.MasterFABrands on a.Code equals b.FACode
                    //                   join c in ctx.TransportOrders on a.IDTransportOrder equals c.IDTransportOrder
                    //                   where c.STONo == data.STONo && a.MaterialType == "Cigarette" && a.UoM != "Box"
                    //                   select new TransportOrderDetailDTO()
                    //                   {
                    //                       PackToBox = (b.PackPerBox / a.Qty.Value)
                    //                   }).FirstOrDefault();

                    var total = 0;
                    var totalBox = _genTraOrdDetRepo.Get().Where(x => x.MaterialType == "Cigarette" && x.UoM == "Box" && x.TransportOrder.STONo == data.STONo).Sum(x => x.Qty);
                    var totalNonBox = (from a in ctx.TransportOrderDetails
                                       join b in ctx.MasterFABrands on a.Code equals b.FACode
                                       join c in ctx.TransportOrders on a.IDTransportOrder equals c.IDTransportOrder
                                       where c.STONo == data.STONo && a.MaterialType == "Cigarette" && a.UoM != "Box"
                                       select new TransportOrderDTO()
                                       {
                                           Code = a.Code,
                                           UoM = a.UoM,
                                           Qty = a.Qty,
                                           PackPerBox = b.PackPerBox
                                       }).ToList();

                    foreach (var nb in totalNonBox)
                    {
                        total = (int)(total + Math.Ceiling(((nb.Qty.HasValue ? nb.Qty.Value : 0) / (nb.PackPerBox.HasValue ? nb.PackPerBox.Value : 0))));
                    }
                    var order = input.FirstOrDefault(f => f.STONo == data.STONo);
                    if (order != null)
                    {
                        if (Mode == "Loading")
                        {
                            data.LoadStartTime = order.LoadStartTime;
                            data.LoadFinishTime = order.LoadFinishTime;
                            if (data.LoadFinishTime != null && data.LoadStartTime != null)
                            {
                                var loadtimediff = data.LoadFinishTime.Value.Subtract(data.LoadStartTime.Value);
                                var intMinutes = loadtimediff.TotalMinutes;
                                //data.LoadBoxperWorkingTime = totalNonBox != null ? Math.Ceiling((decimal)((totalBox.Value + totalNonBox.PackToBox) / Convert.ToDecimal(intMinutes))) : Math.Ceiling((totalBox.Value) / Convert.ToDecimal(intMinutes));
                                data.LoadBoxperWorkingTime = (intMinutes > 0 ? Math.Ceiling((decimal)((total + totalBox) / Convert.ToDecimal(intMinutes))) : 0);
                            }
                        }
                        else if (Mode == "Unloading")
                        {
                            data.UnloadStartTime = order.UnloadStartTime;
                            data.UnloadFinishTime = order.UnloadFinishTime;

                            if (data.UnloadFinishTime != null && data.UnloadStartTime != null)
                            {
                                var loadtimediff = data.UnloadFinishTime.Value.Subtract(data.UnloadStartTime.Value);
                                var intMinutes = loadtimediff.TotalMinutes;
                                //data.UnloadBoxperWorkingTime = totalNonBox != null ? Math.Ceiling((decimal)((totalBox.Value + totalNonBox.PackToBox) / Convert.ToDecimal(intMinutes))) : Math.Ceiling((totalBox.Value) / Convert.ToDecimal(intMinutes));
                                data.UnloadBoxperWorkingTime = (intMinutes > 0 ? Math.Ceiling((decimal)((total + totalBox) / Convert.ToDecimal(intMinutes))) : 0);
                            }
                        }
                    }
                    //f.LoadStartTime = hashOrder[f.STONo] != null ? ((TransportOrderInput)hashOrder[f.STONo]).LoadStartTime : f.LoadStartTime;
                    //f.LoadFinishTime = hashOrder[f.STONo] != null ? ((TransportOrderInput)hashOrder[f.STONo]).LoadFinishTime : f.LoadFinishTime;
                }
                /*
                if (Mode == "Loading")
                {
                    UpdateData.ForEach(f =>
                    {
                        var a = hashOrder[f.STONo];
                        f.LoadStartTime = hashOrder[f.STONo] != null ? ((TransportOrderInput)hashOrder[f.STONo]).LoadStartTime : f.LoadStartTime;
                        f.LoadFinishTime = hashOrder[f.STONo] != null ? ((TransportOrderInput)hashOrder[f.STONo]).LoadFinishTime : f.LoadFinishTime;
                    });
                }
                else if (Mode == "Unloading")
                {
                    UpdateData.ForEach(f =>
                    {
                        f.UnloadStartTime = hashOrder[f.STONo] != null ? ((TransportOrderInput)hashOrder[f.STONo]).UnloadStartTime : f.UnloadStartTime;
                        f.UnloadFinishTime = hashOrder[f.STONo] != null ? ((TransportOrderInput)hashOrder[f.STONo]).UnloadFinishTime : f.UnloadFinishTime;
                    });
                }
                */
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                message = "Upload Failed";
            }

            return message;
        }

        public List<TransportOrderDTO> SearchSTONo(string stoNo)
        {
            return Mapper.Map<List<TransportOrder>, List<TransportOrderDTO>>(_transportOrderRepo.Get(c => c.STONo.ToLower().StartsWith(stoNo.ToLower())).ToList());
        }

        public List<TransportOrderDTO> GetSTONoFilterByRegion(string stono, bool isUserRoleTransport, List<string> parentLocation)
        {
            if (isUserRoleTransport)
                return MappingHelper.Map<TransportOrderDTO>(_transportOrderRepo.GetSTONoFilter(stono)).ToList();
            else
                return MappingHelper.Map<TransportOrderDTO>(_transportOrderRepo.GetSTONoFilterByListRegion(stono, parentLocation)).ToList();
        }

        public List<MasterListDTO> getMasterList()
        {
            List<string> temp = new List<string>(new[] { "OrderType", "VehicleType", "MaterialType", "TransportationStatus", "OrderStatus" });
            return Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByListFieldName(temp));
        }

        public List<MasterLocationDTO> getMasterLocation(bool isUserRoleTransport, List<string> parentLocation)
        {
            if (isUserRoleTransport)
                return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Warehouse"));
            else
                return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetMasterLocationListByListParentLocation(parentLocation));
        }

        public List<MasterLocationDTO> GetZoneList()
        {
            return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Zone").ToList());
        }

        public List<TransportOrderDTO> GetTransportOrder()
        {
            var dbResult = _genTraOrdRepo.Get().OrderBy(m => m.IDTransportOrder).ToList();
            return Mapper.Map<List<TransportOrderDTO>>(dbResult);
        }

        public List<TransportOrderDTO> GetShipmentDate()
        {
            var dbResult = _genTraOrdRepo.Get().OrderBy(m => m.IDTransportOrder).ToList();
            return Mapper.Map<List<TransportOrderDTO>>(dbResult);
        }

        public List<TransportOrderDTO> GetDataDtos(string transNo, string roleName)
        {
            return _transportOrderRepo.GetTOActiveByTN(transNo, roleName);
        }

        public List<TransportOrderDTO> GetDataDtos(string transNo, string senderLocation, string receiverLocation, string transportDate)
        {
            return _transportOrderRepo.GetTOActiveByTNs(transNo, senderLocation, receiverLocation, transportDate);
        }
        public List<TransportOrderDTO> GetDataDtos(string transNo, string[] senderLocation, string[] receiverLocation, DateTime? transportStartDate, DateTime? transportEndDate, string roleName)
        {
            return _transportOrderRepo.GetTOActiveByTNs(transNo, senderLocation, receiverLocation, transportStartDate, transportEndDate, roleName);
        }

        public List<LoadingUnloadingExcelViewDTO> GetDataExcelDtos(string transNo, string[] senderLocation, string[] receiverLocation, DateTime? transportStartDate, DateTime? transportEndDate)
        {
            var queryFilter = PredicateHelper.True<LoadingUnloadingExportView>();
            if (!String.IsNullOrEmpty(transNo))
            {
                queryFilter = queryFilter.And(x => x.TransportNo == transNo);
            }
            if (senderLocation != null && senderLocation.Where(s => !string.IsNullOrWhiteSpace(s)).Count() > 0)
            {
                queryFilter = queryFilter.And(x => senderLocation.Contains( x.SenderIDLocation ));
            }
            if (receiverLocation != null && receiverLocation.Where(s => !string.IsNullOrWhiteSpace(s)).Count() > 0)
            {
                queryFilter = queryFilter.And(x => receiverLocation.Contains(x.ReceiverIDLocation));
            }
            if (transportStartDate.HasValue)
            {
                queryFilter = queryFilter.And(x => x.TransportDate >= transportStartDate.Value);
            }
            if (transportEndDate.HasValue)
            {
                queryFilter = queryFilter.And(x => x.TransportDate <= transportEndDate.Value);
            }
            var dbReturn = _loadingexcelview.Get(queryFilter).ToList(); //.GroupBy(u=>u.STONo).Select(r=>r.FirstOrDefault()).ToList();
            var viewMapper = Mapper.Map<List<LoadingUnloadingExcelViewDTO>>(dbReturn);
            /*
            var vmap = new List<LoadingUnloadingExcelViewDTO>();
            foreach(var vm in viewMapper)
            {
                if (vmap.Count(v => v.STONo == vm.STONo) == 0)
                    vmap.Add(vm);
            }
            */
            return viewMapper;
        }

        public List<SP_GetTransportOrderRequestDTO> GetListViewTransportationOrder(TransportOrderRequestInput filter)
        {
            return _transportOrderRepo.GetTransportOrderRequestSummary(
                filter.stoNoListFilter == null ? "" : string.Join(";", filter.stoNoListFilter), // STO Filter
                filter.zoneListFilter == null ? "" : string.Join(";", filter.zoneListFilter), // Zone Filter
                filter.orderTypeListFilter == null ? "" : string.Join(";", filter.orderTypeListFilter), // Order Type Filter
                filter.orderVehicleTypeListFiter == null ? "" : string.Join(";", filter.orderVehicleTypeListFiter), // Order Vehicle Filter
                filter.dateFromFilter, // Ship Begin Filter
                filter.dateToFilter, // Ship End Filter
                filter.materialTypeListFilter == null ? "" : string.Join(";", filter.materialTypeListFilter), // Material Type Filter
                filter.orderStatusListFilter == null ? "" : string.Join(";", filter.orderStatusListFilter), // Order Status Filter
                filter.senderIdLocListFilter == null ? "" : string.Join(";", filter.senderIdLocListFilter), // Sender Loc Filter
                filter.receiverIdLocListFilter == null ? "" : string.Join(";", filter.receiverIdLocListFilter), // Receiver Loc Filter
                filter.CreatedBy == null ? "" : string.Join(";", filter.CreatedBy), // Receiver Loc Filter
                filter.userRegionList == null ? "" : string.Join(";", filter.userRegionList), // Owner Loc Filter
                filter.IsRoleTransport
                );
        }

        public dynamic GetTransportationOrderDataTable(TransportOrderRequestInput input, DataTableModel model = null)
        {
            dynamic res = new System.Dynamic.ExpandoObject();
            #region FILTERS
            var queryFilter = PredicateHelper.True<TransportOrderRequestHeader>();

            if (input.IsRoleTransport)
            {
                var Users = _masterUserLocationMappingRepo.Get(f => input.userRegionList.Contains(f.IDLocation)).Select(f => f.IDUser.ToLower()).Distinct();
                queryFilter = queryFilter.And(_ => _.TransportOrderRequests.Any(x => Users.All(y => x.CreatedBy.ToLower() == y || x.CreatedBy.ToLower() == input.CreatedBy.ToLower())));
            }
            else
            {
                queryFilter = queryFilter.And(_ => _.TransportOrderRequests.Any(x => x.CreatedBy.ToLower().Contains(input.CreatedBy.ToLower())));
            }
            if (input.stoNoListFilter != null && input.stoNoListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                if (input.stoNoListFilter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.SelectMany(z => z.TransportOrders).Any(x => input.stoNoListFilter.Contains(x.STONo)));
                }
            }
            if (input.zoneListFilter != null && input.stoNoListFilter.Any(i => !String.IsNullOrWhiteSpace(i)))
            {
                if (input.zoneListFilter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.Any(x => input.zoneListFilter.Contains(x.Zone)));
                }
            }
            if (input.orderTypeListFilter != null && input.orderTypeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                if (input.orderTypeListFilter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.SelectMany(z => z.TransportOrders).Any(x => input.orderTypeListFilter.Contains(x.OrderType)));
                }
            }
            if (input.orderVehicleTypeListFiter != null && input.orderVehicleTypeListFiter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                if (input.orderVehicleTypeListFiter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.SelectMany(z => z.TransportOrders).Any(x => input.orderVehicleTypeListFiter.Contains(x.VehicleType)));
                }
            }
            if (input.dateFromFilter.HasValue)
            {
                queryFilter = queryFilter.And(_ => _.TransportOrderRequests.Any(x => x.ShipmentDate >= input.dateFromFilter.Value));
            }
            if (input.dateToFilter.HasValue)
            {
                queryFilter = queryFilter.And(_ => _.TransportOrderRequests.Any(x => x.ShipmentDate <= input.dateToFilter.Value));
            }
            if (input.materialTypeListFilter != null && input.materialTypeListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                if (input.materialTypeListFilter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.SelectMany(x => x.TransportOrders).SelectMany(x => x.TransportOrderDetails).Any(c => input.materialTypeListFilter.Contains(c.MaterialType)));
                }
            }
            if (input.orderStatusListFilter != null && input.orderStatusListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                if (input.orderStatusListFilter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.SelectMany(z => z.TransportOrders).Any(x => input.orderStatusListFilter.Contains(x.OrderStatus)));
                }
            }
            if (input.senderIdLocListFilter != null && input.senderIdLocListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                if (input.senderIdLocListFilter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.SelectMany(z => z.TransportOrders).Any(x => input.senderIdLocListFilter.Contains(x.ActualSenderIDLocation)));
                }
            }
            if (input.receiverIdLocListFilter != null && input.receiverIdLocListFilter.Any(i => !string.IsNullOrWhiteSpace(i)))
            {
                if (input.receiverIdLocListFilter.Count() > 0)
                {
                    queryFilter = queryFilter.And(_ => _.TransportOrderRequests.SelectMany(z => z.TransportOrders).Any(x => input.receiverIdLocListFilter.Contains(x.ActualReceiverIDLocation)));
                }
            }
            #endregion

            #region filter from DataTable
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.search.value))
                {
                    var _us = model.search.value;
                    queryFilter = queryFilter.And(_ => _.RequestNo.Contains(_us) || _.TransportOrderRequests.LastOrDefault(x => x.IsActive).ShipmentDate.ToString().Contains(_us) || _.TransportOrderRequests.LastOrDefault(x => x.IsActive).CreatedBy.Contains(_us));
                }

                foreach (var fil in model.columns.Where(_ => !String.IsNullOrEmpty(_.search.value)))
                {
                    var _col = fil.data;
                    var _val = fil.search.value;
                    switch (_col)
                    {
                        case "RequestNo":
                            queryFilter = queryFilter.And(_ => _.RequestNo.Contains(_val));
                            break;
                        case "ShipmentDate":
                            queryFilter = queryFilter.And(_ => _.TransportOrderRequests.LastOrDefault(x => x.IsActive).ShipmentDate.ToString().Contains(_val));
                            break;
                        case "CreatedName":
                            var _getUser = _masterUserRepo.Get(_ => _.FullName.Contains(_val)).Select(_ => _.IDUser).ToList();
                            //queryFilter = queryFilter.And(_ => _getUser.Contains(_.TransportOrderRequests.LastOrDefault(x => x.IsActive).CreatedBy));
                            queryFilter = queryFilter.And(_ => _.TransportOrderRequests.Any(x => _getUser.Contains(x.CreatedBy)));
                            break;
                    }
                }
            }
            #endregion

            var getTM = Enumerable.Empty<TransportOrderRequestHeader>();
            if (model == null)
            {
                getTM = _transportOrReHeaderRepo.Get(queryFilter);
            }
            else
            {
                getTM = _transportOrReHeaderRepo.GetAllPagination(queryFilter, model.start, model.length, "RequestNo");
            }

            var _usr = new List<string>();
            var total = _transportOrReHeaderRepo.Count(queryFilter);
            if (total > 0)
            {
                _usr = getTM.SelectMany(_ => _.TransportOrderRequests).Select(_ => _.CreatedBy.ToLower()).Distinct().ToList();

            }

            var masterUserList = _masterUserRepo.Get(_ => _.IsActive && _usr.Contains(_.IDUser.ToLower()));

            
            var Data = new List<SP_GetTransportOrderRequestDTO>();

            foreach (var tm in getTM)
            {
                var dataRequest = tm.TransportOrderRequests.Where(_ => _.IsActive && _.ShipmentDate >= input.dateFromFilter && _.ShipmentDate <= input.dateToFilter).LastOrDefault();
                if (dataRequest != null)
                {
                    var _createdName = masterUserList.Where(_ => _.IDUser.ToLower() == dataRequest.CreatedBy.ToLower()).FirstOrDefault();

                    Data.Add(new SP_GetTransportOrderRequestDTO
                    {
                        RequestNo = tm.RequestNo,
                        RequestDate = dataRequest.ShipmentDate,
                        CreatedBy = dataRequest.CreatedBy,
                        IsActive = dataRequest.IsActive ? 1 : 0,
                        CreatedName = _createdName == null ? "" : _createdName.FullName,
                        TotalVehicle = tm.TransportOrderRequests.Where(_ => _.IsActive).Count()
                    });
                }
            }

            

            res.data = Data;
            res.total = total;
            return res;
        }

        public List<TransportExecutionDTO> GetListTruckTrainServerSide(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl, bool isSuperAdmin, object req, int skip, int take)
        {
            var context = new TOMContextDB();
            var dbResult =(
                         from TOR in context.TransportOrderRequests
                         join TOT in context.TransportOrders on TOR.IDRequest equals TOT.IDRequest into JOIN_1
                         from TOT in JOIN_1.DefaultIfEmpty()
                         join TOD in context.TransportOrderDetails on TOT.IDTransportOrder equals TOD.IDTransportOrder into JOIN_2
                         from TOD in JOIN_2.DefaultIfEmpty()
                         join MUR in context.MasterUsers on TOR.CreatedBy equals MUR.IDUser into JOIN_3
                         from MUR in JOIN_3.DefaultIfEmpty()
                         join MAP in context.MasterUserLocationMappings on TOR.UpdatedBy equals MAP.IDUser into JOIN_4
                         from MAP in JOIN_4.DefaultIfEmpty()
                         join locP in context.MasterLocations on MAP.IDLocation equals locP.IDLocation into JOIN_5
                         from locP in JOIN_5.DefaultIfEmpty()
                         join locC in context.MasterLocations on MAP.IDLocation equals locC.IDLocation into JOIN_6
                         from locC in JOIN_6.DefaultIfEmpty()
                         group TOR by new
                         {
                             TOR.RequestNo,
                             TOR.ShipmentDate,
                             TOR.CreatedBy,
                             TOR.CreatedDate,
                             MUR.FullName,
                         }
                         into gp
                         select new
                         {
                             gp.Key.RequestNo,
                             gp.Key.ShipmentDate,
                             gp.Key.CreatedBy,
                             gp.Key.CreatedDate,
                             gp.Key.FullName
                         }
                         );

            var tempRes = dbResult.Select(f => new TransportExecutionDTO { }).Skip(skip).Take(take).ToList();
            return tempRes;
        }

        public bool IsSTONoExists(string stono, params int[] excludesTransportOrderID)
        {
            if (string.IsNullOrWhiteSpace(stono)) return false;
            var to = _transportOrderRepo.Get(b => b.STONo == stono && !excludesTransportOrderID.Contains(b.IDTransportOrder));

            return to.Count() > 0;
        }

        public List<TransportOrderDTO> Get(TransportOrderInput filter = null, string orderBy = null)
        {
            Expression<Func<TransportOrder, bool>> src = obj => obj.IsActive == true;
            if (filter != null)
            {
                if (filter.dateFromFilter != null)
                    src = src.And(obj => obj.ShipmentDate >= filter.dateFromFilter);
                if (filter.dateToFilter != null)
                    src = src.And(obj => obj.ShipmentDate >= filter.dateToFilter);
                if (filter.IDRequest != null)
                    src = src.And(obj => obj.IDRequest == filter.IDRequest);
                if (filter.STONo != null)
                    src = src.And(obj => obj.STONo == filter.STONo);
                if (filter.IDTransportOrder != 0)
                    src = src.And(obj => obj.IDTransportOrder == filter.IDTransportOrder);
            }

            var data = _transportOrderRepo.Get(src);

            if (orderBy != null)
            {
                var pi = typeof(TransportOrder).GetProperty(orderBy);
                if (pi != null)
                    data = data.OrderBy(obj => pi.GetValue(obj));
            }
            return MappingHelper.Map<TransportOrderDTO>(data).ToList();
        }
        public bool SendMail(string subject, string body, string[] recipients, string format = "HTML")
        {
            return _transportOrderRepo.SendMail(subject, body, recipients, format);
        }
        public TransportOrderDTO GetTransportOrderById(int? IDTransportOrder)
        {
            var data = _transportOrderRepo.Get(
                d =>
                (d.IDTransportOrder == IDTransportOrder || IDTransportOrder == null)
                );
            return Mapper.Map<List<TransportOrderDTO>>(data).FirstOrDefault();
        }
        public List<TransportOrderRequestDTO> GetTransportOrderRequestById(int? IDRequest, string RequestNumber)
        {
            var data = _transportOrderRequestRepo.Get(
                d =>
                (d.IDRequest == IDRequest || IDRequest == null) &&
                (d.RequestNo == RequestNumber || RequestNumber == null)
                );
            return Mapper.Map<List<TransportOrderRequestDTO>>(data).ToList();
        }
        public TransportOrderDetailDTO GetTransportOrderDetailById(int? IDTransportOrderDetail)
        {
            var data = _genTraOrdDetRepo.Get(
                d =>
                (d.IDTransportOrderDetail == IDTransportOrderDetail || IDTransportOrderDetail == null)
                );
            return Mapper.Map<List<TransportOrderDetailDTO>>(data).FirstOrDefault();
        }
        public List<TransportOrderDetailDTO> GetTransportOrderDetails(int? IDTransportOrder)
        {
            var data = _genTraOrdDetRepo.Get(
                d =>
                (d.IDTransportOrder == IDTransportOrder || IDTransportOrder == null)
                );
            return Mapper.Map<List<TransportOrderDetailDTO>>(data).ToList();
        }

        public void SetActiveTransportOrderRequest(List<int> idTransporReq, string userid)
        {
            foreach (int id in idTransporReq)
            {
                TransportOrderRequest temp = _transportOrderRequestRepo.GetTransportOrderRequestByID(id);
                temp.IsActive = false;
                temp.UpdatedBy = userid;
                temp.TransportOrders.ToList().ForEach(x =>
                {
                    x.UpdatedBy = userid;
                    x.IsActive = false;
                });
                _transportOrderRequestRepo.SaveData(temp);
            }
        }

        public List<MasterListDTO> GetList()
        {
            List<MasterListDTO> tempList = new List<MasterListDTO>();
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("VehicleType")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("MaterialType")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("Zone")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("TransportationStatus")));
            return tempList;
        }

        public List<MasterLocationDTO> GetALLMasterLocation()
        {
            List<MasterLocationDTO> tempList = new List<MasterLocationDTO>();
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Warehouse")));
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Agent")));
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Factory")));
            return tempList;
        }

        public MasterMapping GetOrderType(String MaterialType)
        {
            MasterMapping orderType = _masterMappingRepo.CheckAvailabilityMasterMappingByMapFrom(MaterialType);
            return orderType;
        }

        public MasterCostCenter GetCostCenter(MasterCostCenterAccountInput input)
        {
            MasterCostCenter mstCostCenter = _masterCostCenterAccountRepo.GetCostCenterBySenderReceiverMaterial(input);
            return mstCostCenter;
        }
        public List<MasterUomDTO> GetUomByMaterial(String MaterialType)
        {
            List<MasterUomDTO> tempUom = new List<MasterUomDTO>();
            tempUom.AddRange(Mapper.Map<List<MasterUom>, List<MasterUomDTO>>(_masterUomRepo.GetMasterUomByMaterial(MaterialType)));
            return tempUom;
        }
        public String GetFABrandByCode(String Code)
        {
            MasterFABrand mstFABrand = _masterFABrandRepo.CheckBrandAvailabilityByCode(Code);
            if (mstFABrand == null)
                return "";
            else
                return mstFABrand.LongSpeakingCode;
        }
        public List<TransportOrderRequestDTO> SaveRequestUnit(List<TransportOrderRequestInput> input, string userid)
        {
            //string lastReqNumber = _transportOrderRequestRepo.getLastRequestNumber(input[0].ShipmentDate);
            string lastReqNumber = "";
            string prevSender = "";
            string currSender = "";
            string prevReceiver = "";
            string currReceiver = "";
            string prevSeq = "";
            string currSeq = "";
            string defaultSeq = "";
            string prevUnit = "";
            string currUnit = "";
            string currSTO = "";
            string STONo = "";
            int previdTO = 0;
            int prevparentidTO = 0;
            int prevToId = 0;
            int currToId = 0;
            DateTime prevShipmentDate = Convert.ToDateTime("2018-01-01");
            var idReq = 0;
            var idTO = 0;
            List<TransportOrderDetail> listTODetail = new List<TransportOrderDetail>();
            List<TransportOrderRequestDTO> listOutput = new List<TransportOrderRequestDTO>();
            List<int> listTODDelete = new List<int>();
            foreach (TransportOrderRequestInput reqUnit in input)
            {
                TransportOrderRequestDTO tempTOR = new TransportOrderRequestDTO();
                DateTime promiseDate = Convert.ToDateTime(reqUnit.ShipmentDate);
                currSender = reqUnit.SenderIDLocation;
                currReceiver = reqUnit.ReceiverIDLocation;
                currUnit = reqUnit.UnitID;
                currSTO = reqUnit.OrderNumber;
                currToId = reqUnit.IDTransportOrder;
                // set seq jika seqid di element contain TO
                //Debug.WriteLine("SeqID:"+reqUnit.SeqID);
                if (reqUnit.SeqID.Contains("TO")) { defaultSeq = reqUnit.SeqID; }
                // set STO / Order Number
                if (prevToId != currToId)
                {
                    if (reqUnit.OrderStatus.Equals("Submit") && String.IsNullOrEmpty(currSTO) && !reqUnit.MaterialType.Equals("Cigarette"))
                    {
                        string lastSTO = _transportOrderRepo.getLastSTONo(reqUnit.ShipmentDate);
                        STONo = lastSTO;
                    }
                    else
                    {
                        STONo = reqUnit.OrderNumber;
                    }
                    prevToId = currToId;
                }
                TransportOrderDetail tempTODetail = new TransportOrderDetail();
                TransportOrder transportOrder;
                if (reqUnit.IDRequest == 0 && reqUnit.IDTransportOrder == 0)
                {
                    // insert TO Request, TO, TO Detail                                        
                    transportOrder = new TransportOrder();
                    if (prevSeq == currSeq && prevShipmentDate == promiseDate && prevSender == currSender && prevReceiver == currReceiver)
                    {
                        // insert TO Detail
                        tempTODetail = setEntityTODetail(reqUnit, idTO, userid);
                        listTODetail.Add(tempTODetail);
                    }
                    else
                    {
                        // insert TO Request and get ID Request                      
                        if (!String.IsNullOrEmpty(reqUnit.RequestNo))
                        {
                            idReq = _transportOrderRequestRepo.GetIDRequestByReqNo(reqUnit.RequestNo);
                            lastReqNumber = reqUnit.RequestNo;
                        }
                        else if (!String.IsNullOrEmpty(lastReqNumber))
                        {
                            idReq = _transportOrderRequestRepo.GetIDRequestByReqNo(lastReqNumber);
                        }
                        else
                        {
                            lastReqNumber = _transportOrderRequestRepo.getLastRequestNumber(reqUnit.ShipmentDate);
                            TransportOrderRequest orderReq = new TransportOrderRequest();
                            orderReq.VehicleType = reqUnit.VehicleType;
                            orderReq.ShipmentDate = promiseDate;
                            orderReq.Zone = reqUnit.Zone;
                            orderReq.CreatedBy = userid;
                            orderReq.UpdatedBy = userid;
                            orderReq.RequestNo = lastReqNumber;
                            idReq = _transportOrderRequestRepo.InsertData(orderReq);
                        }
                        // set default sequence
                        if (prevUnit != currUnit && !reqUnit.SeqID.Contains("TO") && reqUnit.OrderStatus.Equals("Submit"))
                        {
                            defaultSeq = _transportOrderRepo.getSeqUnitByShipmentDate(reqUnit.ShipmentDate);
                            prevUnit = currUnit;
                        }
                        // insert TO
                        transportOrder = setEntityTO(reqUnit, idReq, promiseDate, userid, defaultSeq, STONo, true);
                        idTO = _transportOrderRepo.InsertData(transportOrder);

                        // insert TO Detail                        
                        tempTODetail = setEntityTODetail(reqUnit, idTO, userid);
                        listTODetail.Add(tempTODetail);

                        prevSeq = currSeq;
                        prevShipmentDate = promiseDate;
                        prevSender = currSender;
                        prevReceiver = currReceiver;
                    }

                    // set nilai balik STO/Order Number
                    if (!String.IsNullOrEmpty(reqUnit.OrderNumber) && reqUnit.MaterialType != "Cigarette")
                        tempTOR.STONo = reqUnit.OrderNumber;
                    else
                        tempTOR.STONo = transportOrder.STONo;

                }
                else
                {
                    // update TO Request, TO, TO Detail                    
                    idTO = reqUnit.IDTransportOrder;
                    idReq = reqUnit.IDRequest;
                    // list delete TO Detail
                    if (previdTO != idTO)
                    {
                        listTODDelete.Add(idTO);
                        previdTO = idTO;
                    }

                    // update TO Request
                    TransportOrderRequest orderReq = _transportOrderRequestRepo.GetTransportOrderRequestByID(idReq);
                    orderReq.VehicleType = reqUnit.VehicleType;
                    //orderReq.ShipmentDate = promiseDate;
                    orderReq.Zone = reqUnit.Zone;
                    orderReq.UpdatedBy = userid;
                    _transportOrderRequestRepo.SaveData(orderReq);

                    // set default sequence
                    if (prevUnit != currUnit && !reqUnit.SeqID.Contains("TO") && reqUnit.OrderStatus.Equals("Submit"))
                    {
                        defaultSeq = _transportOrderRepo.getSeqUnitByShipmentDate(reqUnit.ShipmentDate);
                        prevUnit = currUnit;
                    }
                    // update TO                        
                    transportOrder = _transportOrderRepo.GetTransportOrderByID(idTO);
                    transportOrder = setEntityTO(reqUnit, idReq, promiseDate, userid, defaultSeq, STONo, false);
                    _transportOrderRepo.SaveData(transportOrder);

                    // insert TO Detail                        
                    tempTODetail = setEntityTODetail(reqUnit, idTO, userid);
                    listTODetail.Add(tempTODetail);

                    // set nilai balik STO/Order Number
                    if (!String.IsNullOrEmpty(reqUnit.OrderNumber) && reqUnit.MaterialType != "Cigarette")
                        tempTOR.STONo = reqUnit.OrderNumber;
                    else
                        tempTOR.STONo = transportOrder.STONo;
                }
                // set nilai balik STO/Order Number
                tempTOR.IDRequest = idReq;
                tempTOR.IDTransportOrder = idTO;
                tempTOR.RequestNo = !String.IsNullOrEmpty(lastReqNumber) ? lastReqNumber : reqUnit.RequestNo;
                tempTOR.DefaultSeqNo = defaultSeq;
                if (prevparentidTO != idTO)
                {
                    listOutput.Add(tempTOR);
                    prevparentidTO = idTO;
                }
            }
            _transportOrderDetailRepo.DeleteDataByListTO(listTODDelete);
            _transportOrderDetailRepo.InsertListData(listTODetail);
            return listOutput;
        }

        public TransportOrder setEntityTO(TransportOrderRequestInput reqUnit, int idReq, DateTime promiseDate, string userid, string defaultseq, string STO, bool status)
        {
            TransportOrder transportOrder;
            if (status == false)
            {
                transportOrder = _transportOrderRepo.GetTransportOrderByID(reqUnit.IDTransportOrder);
            }
            else
            {
                transportOrder = new TransportOrder();
                transportOrder.IDRequest = idReq;
            }
            transportOrder.DefaultSeqNo = defaultseq;
            transportOrder.STONo = STO;
            transportOrder.ShipmentDate = promiseDate;
            transportOrder.OrderType = reqUnit.OrderType;
            transportOrder.SenderIDLocation = reqUnit.SenderIDLocation;
            transportOrder.ActualSenderIDLocation = reqUnit.SenderIDLocation;
            transportOrder.ReceiverIDLocation = reqUnit.ReceiverIDLocation;
            transportOrder.ActualReceiverIDLocation = reqUnit.ReceiverIDLocation;
            transportOrder.CostCenter = reqUnit.CostCenter;
            transportOrder.OrderStatus = reqUnit.OrderStatus;
            transportOrder.Remarks = reqUnit.Remarks;
            transportOrder.ZoneBased = reqUnit.Zone;
            transportOrder.IsActive = true;
            transportOrder.UpdatedBy = userid;
            transportOrder.CreatedBy = userid;
            return transportOrder;
        }

        public TransportOrderDetail setEntityTODetail(TransportOrderRequestInput reqUnit, int idTO, string userid)
        {
            TransportOrderDetail tempTODetail = new TransportOrderDetail();
            tempTODetail.IDTransportOrder = idTO;
            tempTODetail.MaterialType = reqUnit.MaterialType;
            tempTODetail.Code = reqUnit.Code;
            tempTODetail.Description = reqUnit.Description;
            tempTODetail.Qty = reqUnit.Qty;
            tempTODetail.UoM = reqUnit.UoM;
            tempTODetail.IsActive = true;
            tempTODetail.CreatedBy = userid;
            tempTODetail.UpdatedBy = userid;
            return tempTODetail;
        }
        public bool DeleteRoute(List<int> idTO)
        {
            //_transportOrderDetailRepo.DeleteDataByTO(idTO);
            //_transportOrderRepo.DeleteData(idTO);                
            foreach (var idx in idTO)
            {
                //_transportOrderDetailRepo.setInActiveByTO(idx);
                var transportOrder = _transportOrderRepo.GetTransportOrderByID(idx);
                transportOrder.IsActive = false;
                if (transportOrder.OrderStatus != "Draft" && !string.IsNullOrWhiteSpace(transportOrder.STONo))
                {
                    transportOrder.OrderStatus = "Cancel";
                    transportOrder.IsActive = true;
                }
                _transportOrderRepo.SaveData(transportOrder);
            }
            return true;
        }

        public bool DeleteRequest(string requestNo)
        {
            var ent = _transportOrderRequestRepo.Get(i => i.RequestNo == requestNo).ToList();
            if (ent.Count > 0)
            {
                _transportOrderRequestRepo.BeginTransaction();
                try
                {
                    foreach (var req in ent)
                    {
                        req.IsActive = false;
                        foreach (var w in req.TransportOrders)
                        {
                            w.IsActive = false;
                            if (w.OrderStatus != "Draft" && !string.IsNullOrWhiteSpace(w.STONo))
                            {
                                w.OrderStatus = "Cancel";
                                //w.IsActive = true;
                            }
                            else
                                foreach (var m in w.TransportOrderDetails)
                                    m.IsActive = false;
                        }
                        _transportOrderRequestRepo.Update(req);
                        _transportOrderRequestRepo.Save();
                        _transportOrderRequestRepo.EndTransaction(true);
                    }
                }
                catch
                {
                    _transportOrderRequestRepo.EndTransaction(false);
                    return false;
                }
            }
            return true;
        }
        public bool DeleteRequest(string[] requestNo)
        {
            var ent = _transportOrderRequestRepo.Get(i => requestNo.Contains(i.RequestNo)).ToList();
            if (ent.Count > 0)
            {
                _transportOrderRequestRepo.BeginTransaction();
                try
                {
                    foreach (var req in ent)
                    {
                        req.IsActive = false;
                        foreach (var w in req.TransportOrders)
                        {
                            w.IsActive = false;
                            if (w.OrderStatus != "Draft" && !string.IsNullOrWhiteSpace(w.STONo))
                            {
                                w.OrderStatus = "Cancel";
                                //req.IsActive = true;
                            }
                            else
                                foreach (var m in w.TransportOrderDetails)
                                    m.IsActive = false;
                        }
                        _transportOrderRequestRepo.Update(req);
                        _transportOrderRequestRepo.Save();
                    }
                    _transportOrderRequestRepo.EndTransaction(true);
                }
                catch
                {
                    _transportOrderRequestRepo.EndTransaction(false);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteUnit(int IDTransportOrderRequest)
        {
            var ent = _transportOrderRequestRepo.Get(i => i.IDRequest == IDTransportOrderRequest).FirstOrDefault();
            if (ent != null)
            {
                try
                {
                    ent.IsActive = false;
                    foreach (var w in ent.TransportOrders)
                    {
                        w.IsActive = false;
                        if (w.OrderStatus != "Draft" && !string.IsNullOrWhiteSpace(w.STONo))
                        {
                            w.OrderStatus = "Cancel";
                            //w.IsActive = true;
                        }
                        else
                            foreach (var m in w.TransportOrderDetails)
                                m.IsActive = false;
                    }
                    _transportOrderRequestRepo.Update(ent);
                    _transportOrderRequestRepo.Save();
                }
                catch { return false; }
            }
            return true;
        }
        public bool DeleteUnit(IEnumerable<int> IDTransportOrderRequest)
        {
            _transportOrderRequestRepo.BeginTransaction();
            foreach (var ID in IDTransportOrderRequest)
            {
                if (!DeleteUnit(ID))
                {
                    _transportOrderRequestRepo.EndTransaction(false);
                    return false;
                }
            }
            _transportOrderRequestRepo.EndTransaction(true);
            return true;
        }
        public bool DeleteUnit(string ReqNo)
        {
            var ent = _transportOrderRequestRepo.Get(i => i.RequestNo == ReqNo).FirstOrDefault();
            if (ent != null)
            {
                try
                {
                    ent.IsActive = false;
                    foreach (var w in ent.TransportOrders)
                    {
                        w.IsActive = false;
                        foreach (var m in w.TransportOrderDetails)
                            m.IsActive = false;
                    }
                    _transportOrderRequestRepo.Update(ent);
                    _transportOrderRequestRepo.Save();
                }
                catch { return false; }
            }
            return true;
        }
        public bool DeleteUnit(IEnumerable<string> ReqNo)
        {
            _transportOrderRequestRepo.BeginTransaction();
            foreach (var ID in ReqNo)
            {
                if (!DeleteUnit(ID))
                {
                    _transportOrderRequestRepo.EndTransaction(false);
                    return false;
                }
            }
            _transportOrderRequestRepo.EndTransaction(true);
            return true;
        }

        //digunakan hanya untuk loading unloading
        public TransportOrderDTO EditData(TransportOrderDTO input)
        {
            using (var dbContext = new TOMContextDB())
            {
                // ambil data cigarette
                var total = 0;
                var totalBox = _genTraOrdDetRepo.Get().Where(x => x.MaterialType == "Cigarette" && x.UoM == "Box" && x.IDTransportOrder == input.IDTransportOrder).Sum(x => x.Qty);
                var totalNonBox = (from a in dbContext.TransportOrderDetails
                                   join b in dbContext.MasterFABrands on a.Code equals b.FACode
                                   where a.IDTransportOrder == input.IDTransportOrder && a.MaterialType == "Cigarette" && a.UoM != "Box"
                                   select new TransportOrderDTO()
                                   {
                                       Code = a.Code,
                                       UoM = a.UoM,
                                       Qty = a.Qty,
                                       PackPerBox = b.PackPerBox
                                   }).ToList();

                foreach (var nb in totalNonBox)
                {
                    total = (int)(total + Math.Ceiling(((nb.Qty.HasValue ? nb.Qty.Value : 0) / (nb.PackPerBox.HasValue ? nb.PackPerBox.Value : 0))));
                }

                TransportOrder validateTn = dbContext.TransportOrders.FirstOrDefault(x => x.IDTransportOrder == input.IDTransportOrder);
                try
                {
                    if (validateTn != null)
                    {
                        validateTn.Remarks = input.Remarks;
                        validateTn.LoadStartTime = input.LoadStartTime;
                        validateTn.LoadFinishTime = input.LoadFinishTime;
                        validateTn.UnloadStartTime = input.UnloadStartTime;
                        validateTn.UnloadFinishTime = input.UnloadFinishTime;
                        //validateTn.UpdatedBy = input.UpdatedBy;
                        //validateTn.UpdatedDate = DateTime.Now;
                        validateTn.IsActive = input.IsActive;
                        if (input.LoadFinishTime != null)
                        {
                            if (input.LoadStartTime != null)
                            {
                                TimeSpan loadtimediff = input.LoadFinishTime.Value.Subtract(input.LoadStartTime.Value);
                                var intMinutes = loadtimediff.TotalMinutes;
                                validateTn.LoadBoxperWorkingTime = (intMinutes > 0 ? Math.Ceiling((decimal)((total + totalBox) / Convert.ToDecimal(intMinutes))) : 0);
                            }
                        }
                        if (input.UnloadFinishTime != null)
                        {
                            if (input.UnloadStartTime != null)
                            {
                                TimeSpan unloadtimediff = input.UnloadFinishTime.Value.Subtract(input.UnloadStartTime.Value);
                                var intMinutesunload = unloadtimediff.TotalMinutes;
                                validateTn.UnloadBoxperWorkingTime = (intMinutesunload > 0 ? Math.Ceiling((decimal)((total + totalBox) / Convert.ToDecimal(intMinutesunload))) : 0);
                            }
                        }
                        dbContext.SaveChanges();
                    }
                }
                catch (ExceptionBase ex)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.UnhandledException);
                }
            }
            return input;
        }
        public void UpdateOrderStatus(TransportOrderRequestInput input, string userid)
        {
            TransportOrder transportOrder = _transportOrderRepo.GetTransportOrderByID(input.IDTransportOrder);
            transportOrder.OrderStatus = input.OrderStatus;
            transportOrder.UpdatedBy = userid;
            _transportOrderRepo.SaveData(transportOrder);
        }

        /** Get data **/
        public List<TransportOrderRequestDTO> GetRequests(string reqNo = null, int? reqID = null, bool isActive = true)
        {
            var res = _genTraOrdReqRepo.Get(q =>
                    (q.RequestNo == reqNo || reqNo == null) &&
                    (q.IDRequest == reqID || reqID == null) &&
                    q.IsActive == isActive
                    ).ToList();
            return
                Mapper.Map<List<TransportOrderRequestDTO>>(res);
        }

        /** begin, rollback, commit **/
        public void BeginTransaction(System.Data.IsolationLevel? isoLevel = null)
        {
            //_genTraOrdRepo.BeginTransaction();
            if (isoLevel != null)
                _transportOrReHeaderRepo.BeginTransaction((System.Data.IsolationLevel)isoLevel);
            else
                _transportOrReHeaderRepo.BeginTransaction();
            //_genTraOrdDetRepo.BeginTransaction();
        }
        public void EndTransaction(bool commit = true)
        {
            //_genTraOrdRepo.EndTransaction(commit);
            //_genTraOrdReqRepo.EndTransaction(commit);
            //_genTraOrdDetRepo.EndTransaction(commit);
            _transportOrReHeaderRepo.EndTransaction(commit);
        }


        private bool IsEmailValid(string email)
        {
            try
            {
                var e = new System.Net.Mail.MailAddress(email);
                return e.Address == email;
            }
            catch { return false; }
        }

        private string CurrentUser { get; set; }
        private string LastUsedSTO { get; set; }
        private string SeqNo { get; set; }
        public string SaveData(IEnumerable<TransportOrderRequestDTO> data, string byUser, bool handleRollback = true)
        {
            //throw new Exception();
            if (handleRollback)
                BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

            // prepare for email content
            List<OrderRouteInfo> _routeInfos = new List<OrderRouteInfo>();

            // get new request number based on date
            string newReqNo = null;
            string id = null;
            CurrentUser = byUser;

            // find any assigned request number
            bool fAlreadyHaveReqNo = false;
            foreach (var d in data)
            {
                if (string.IsNullOrEmpty(d.RequestNo))
                    continue;
                newReqNo = d.RequestNo;
                fAlreadyHaveReqNo = true;
                break;
            }

            // Prepare data
            foreach (var d in data)
            {
                // no request number? create a new one!
                if (newReqNo == null)
                    newReqNo = _transportOrReHeaderRepo.GenerateNewRequestNumber(d.ShipmentDate);
                // set the request number
                d.RequestNo = newReqNo;
                d.UpdatedBy = byUser;
                d.UpdatedDate = DateTime.Now;

                // Get a sequence number
                SeqNo = _transportOrReHeaderRepo.GenerateSequenceNumber(d.ShipmentDate, d.IDRequest);

                foreach (var r in d.TransportOrders)
                {
                    #region Email Related
                    var isNewData = string.IsNullOrEmpty(r.STONo) && r.OrderStatus != "Draft";
                    var ri = _routeInfos.Where(rx => rx.Zone == d.Zone && rx.IsNewData == isNewData).FirstOrDefault();
                    if (ri == null)
                    {
                        ri = new OrderRouteInfo();
                        ri.Zone = d.Zone;
                        ri.IsNewData = isNewData;
                        _routeInfos.Add(ri);
                    }

                    if (isNewData)
                    {
                        // Generate STO Number aka Order Number (ON)
                        r.STONo = _transportOrReHeaderRepo.GenerateNewOrderNumber(d.ShipmentDate, LastUsedSTO);
                        LastUsedSTO = r.STONo;

                        // Send new order email here!
                        ri.OrderDetail.Add(r.STONo, r.OrderType);
                    }
                    else if (!string.IsNullOrWhiteSpace(r.STONo))
                    {
                        // Send updated order email here!
                        if (r.OrderStatus != "Draft" && r.StatusChanged)
                            ri.OrderDetail.Add(r.STONo, r.OrderType);
                    }
                    #endregion

                    if (r.ShipmentDate == DateTime.MinValue)
                        r.ShipmentDate = d.ShipmentDate;
                    r.SeqNo = r.OrderStatus == "Draft" ? r.SeqNo : SeqNo;
                    r.IDRequest = r.IDRequest;
                    r.UpdatedBy = byUser;
                    r.UpdatedDate = DateTime.Now;
                    foreach (var m in r.TransportOrderDetails)
                    {
                        m.IDTransportOrder = r.IDTransportOrder;
                        m.UpdatedBy = byUser;
                        m.UpdatedDate = DateTime.Now;
                    }
                }
            }


            // insert/update data
            try
            {
                var ent = InsertOrUpdateTORBatch(data, !fAlreadyHaveReqNo);

                _transportOrReHeaderRepo.InsertOrUpdate(ent);
                _transportOrReHeaderRepo.Save();

                // map entity back to dto
                foreach (var d in data)
                {
                    if (d._OldRecord is TransportOrderRequest)
                    {
                        d.IDRequest = (d._OldRecord as TransportOrderRequest).IDRequest;
                    }
                    for (var tor = 0; tor < d.TransportOrders.Count; tor++)
                    {
                        var _to = d.TransportOrders.ElementAt(tor);
                        if (_to._OldRecord is TransportOrder)
                            _to.IDTransportOrder = (_to._OldRecord as TransportOrder).IDTransportOrder;
                        for (var y = 0; y < _to.TransportOrderDetails.Count; y++)
                        {
                            var _tod = _to.TransportOrderDetails.ElementAt(y);
                            if (_tod._OldRecord is TransportOrderDetail)
                                _tod.IDTransportOrderDetail = (_tod._OldRecord as TransportOrderDetail).IDTransportOrderDetail;
                        }
                    }
                }

                if (handleRollback)
                    EndTransaction(true);
                id = data.FirstOrDefault().RequestNo;
            }
            catch (Exception ex)
            {
                if (handleRollback)
                    EndTransaction(false);
                throw ex;
            }

            //  return id;

            // send email before returning
            #region Email
            string REQNo = newReqNo;
            string mailUserID = byUser;
            var usrEnt = _masterUserRepo.Get(u => u.IDUser == byUser).FirstOrDefault();
            string mailUserName = usrEnt == null ? mailUserID : usrEnt.FullName;

            // get recipients
            Dictionary<string, string[]> _recipients = new Dictionary<string, string[]>();
            Dictionary<string, string[]> _notifs = new Dictionary<string, string[]>();

            // east recipients

            var reg = _masterCfgRepo.GetListMasterConfigurationByPageNameDescription("TransportOrderMailRecipientLocation", "East").Select(c => c.Value).Distinct().ToArray();
            var rc = _masterUserRepo.GetEmailInLocation(reg.ToArray());
            _recipients.Add("East", rc.ToArray());
            var nt = _masterUserRepo.GetIDUserInLocation(reg.ToArray());
            _notifs.Add("East", nt.ToArray());

            //_recipients.Add("East", new string[] { "zecchan@lolisoft.com" });
            // west recipients

            reg = _masterCfgRepo.GetListMasterConfigurationByPageNameDescription("TransportOrderMailRecipientLocation", "West").Select(c => c.Value).Distinct().ToArray();
            rc = _masterUserRepo.GetEmailInLocation(reg.ToArray()).Where(c => IsEmailValid(c)).ToList();
            _recipients.Add("West", rc.ToArray());
            nt = _masterUserRepo.GetIDUserInLocation(reg.ToArray()).Where(c => IsEmailValid(c)).ToList();
            _notifs.Add("West", nt.ToArray());

            //_recipients.Add("West", new string[] { "zecchan@lolisoft.com" });


            //throw new Exception("Break here: " + rc.Length);

            _routeInfos = new List<OrderRouteInfo>();
            foreach (var d in data)
            {
                foreach (var to in d.TransportOrders)
                {
                    if (!to.StatusChanged || to.OrderStatus == "Draft" || to.OrderStatus == "Cancel" || to.OrderStatus == "Close")
                        continue;

                    OrderRouteInfo ori = _routeInfos.Where(o => o.IsNewData == to.IsNewData && o.Zone == d.Zone).FirstOrDefault();
                    if (ori == null)
                    {
                        ori = new OrderRouteInfo();
                        ori.IsNewData = to.IsNewData;
                        ori.Zone = d.Zone;
                        ori.OrderDetail = new Dictionary<string, string>();
                        _routeInfos.Add(ori);
                    }

                    ori.OrderDetail.Add(to.STONo, to.OrderType);
                }
            }

            foreach (var ri in _routeInfos)
            {
                // get receiver based on ri.Zone
                var receiver = ri.Zone != null && _recipients.ContainsKey(ri.Zone) ? _recipients[ri.Zone] : null;
                var notifier = ri.Zone != null && _notifs.ContainsKey(ri.Zone) ? _notifs[ri.Zone] : null;

                if (receiver != null && receiver.Length > 0)
                {
                    if (ri.OrderDetail.Count > 0)
                    {
                        // SendMailTO
                        string body = CreateOrderEmail(ri.OrderDetail, REQNo, mailUserName, ri.IsNewData);
                        _transportOrderRepo.SendMail(ri.IsNewData ? "New Order Notification" : "Modified Order Notification", body, receiver);

                        // SendNotifTO
                        if (notifier != null && notifier.Length > 0)
                        {
                            string notif = CreateOrderNotif(ri.OrderDetail, REQNo, mailUserName, ri.IsNewData);
                            _transportOrderRepo.CreateNewNotification("TransportOrder", notif, notifier, byUser);
                        }
                    }
                }
            }

            #endregion

            return id;
        }

        public string BaseUri { get; set; }

        public string SaveData(TransportOrderRequestData data, string byUser, bool handleRollback = true)
        {
            try
            {
                var tors = new List<TransportOrderRequestDTO>();
                foreach (var u in data.Units)
                {
                    var tor = MappingHelper.Map<TransportOrderRequestDTO>(u);
                    tor.Tag = u;
                    u.RequestNo = data.RequestNo;
                    tor.RequestNo = data.RequestNo;
                    tor.TransportOrders = new List<TransportOrderDTO>();
                    foreach (var rt in u.Routes)
                    {
                        if (!rt.Checked)
                        {
                            if (rt.Saved)
                            {
                                var oldData = GetTransportOrderById(rt.IDTransportOrder);
                                if (oldData != null)
                                {
                                    oldData.Checked = false;
                                    tor.TransportOrders.Add(oldData);
                                }
                            }
                            continue;
                        }
                        // set flag to saved
                        rt.Saved = true;
                        var mattype = rt.Materials.FirstOrDefault();
                        var mtype = mattype != null ? mattype.MaterialType : null;
                        var ord = MappingHelper.Map<TransportOrderDTO>(rt);
                        ord.TransportOrderDetails = new List<TransportOrderDetailDTO>();
                        ord.Remarks = rt.OrderRemark;
                        ord.Tag = rt;
                        var costcenter = _repoCostCenter.Get(
                                c =>
                                c.Sender == ord.SenderIDLocation &&
                                c.Receiver == ord.ReceiverIDLocation &&
                                c.MaterialType == mtype &&
                                c.EffectiveStartDate <= u.ShipmentDate &&
                                c.EffectiveEndDate >= u.ShipmentDate
                            ).FirstOrDefault();
                        ord.IDCostCenter = costcenter != null ? costcenter.IDCostCenter : (int?)null;

                        foreach (var mt in rt.Materials)
                        {
                            var mat = MappingHelper.Map<TransportOrderDetailDTO>(mt);
                            mat.Tag = mt;
                            ord.TransportOrderDetails.Add(mat);
                        }

                        tor.TransportOrders.Add(ord);
                    }
                    tors.Add(tor);
                }

                var lockObj = new object();
                var reqNo = SaveData(tors, byUser, handleRollback);

                // map dto to input again (getting the id)
                foreach (var rq in tors)
                {
                    if (rq.Tag is TransportOrderUnitInput)
                    {
                        (rq.Tag as TransportOrderUnitInput).IDRequest = rq.IDRequest;
                    }
                    foreach (var to in rq.TransportOrders)
                    {
                        if (to.Tag is TransportOrderRouteInput)
                        {
                            var route = to.Tag as TransportOrderRouteInput;
                            route.IDTransportOrder = to.IDTransportOrder;
                            route.OrderNumber = to.STONo;
                            route.SeqNo = to.SeqNo;
                            route.IDCostCenter = to.IDCostCenter;
                        }
                        foreach (var tod in to.TransportOrderDetails)
                        {
                            if (tod.Tag is TransportOrderMaterialInput)
                            {
                                (tod.Tag as TransportOrderMaterialInput).IDTransportOrderDetail = tod.IDTransportOrderDetail;
                            }
                        }
                    }
                }

                return reqNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SaveData(IEnumerable<TransportOrderRequestData> data, string byUser)
        {
            BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            var res = new List<string>();
            try
            {
                foreach (var d in data)
                {
                    var r = SaveData(d, byUser, false);
                    if (r == null)
                    {
                        throw new Exception("Data save failed!");
                    }
                    res.Add(r);
                }
                EndTransaction(true);
            }
            catch (Exception ex)
            {
                EndTransaction(false);

                throw ex;
            }
            return res.ToArray();
        }

        private TransportOrderRequestHeader InsertOrUpdateTORBatch(IEnumerable<TransportOrderRequestDTO> data, bool freshNewData)
        {
            var reqNo = data.Count() > 0 ? data.First().RequestNo : null;
            var ret = freshNewData ?
                new TransportOrderRequestHeader() { RequestNo = reqNo } :
                _transportOrReHeaderRepo.Get(c => c.RequestNo == reqNo).FirstOrDefault();

            foreach (var d in data)
            {
                var req = PrepareSave(d, ret);
                ret.TransportOrderRequests.Add(req);
            }

            return ret;
        }

        public bool IsSafeToOverwrite(string stono, string updatedStatus)
        {
            if (string.IsNullOrWhiteSpace(stono)) return true;
            var to = _transportOrderRepo.Get(c => c.STONo == stono).FirstOrDefault();

            string[] allowedTransitions = new string[] {
                "Draft>Submit",
                "Submit>Draft",
                "Draft>Deleted",
                "Submit>Deleted"
            };

            if (to == null) return true;

            string transition = to.OrderStatus + ">" + updatedStatus;
            if (to.OrderStatus != updatedStatus && !allowedTransitions.Contains(transition))
            {
                return false;
            }
            return true;
        }

        private class OrderRouteInfo
        {
            public string Zone { get; set; }
            public Dictionary<string, string> OrderDetail { get; set; }
            public bool IsNewData { get; set; }

            public OrderRouteInfo()
            {
                OrderDetail = new Dictionary<string, string>();
            }
        }

        private string CreateOrderEmail(Dictionary<string, string> orderDetail, string reqNo, string user, bool isNewData)
        {
            // new data email
            string _mailBody = isNewData
                ? "Hi,<br /><br />We would like to inform you that,<br />Request Number " + reqNo + " have been created with :<br />"
                : "Hi,<br /><br />We would like to inform you that:<br />";
            // make table header
            _mailBody += "<table style='width:100%'><tr><th style='text-align:left'>Order Number</th><th style='text-align:left'>Order Type</th><th style='text-align:left'>" + (isNewData ? "Submitted" : "Modified") + " By </th></tr>";
            foreach (var od in orderDetail)
            {
                var host = HttpContext.Current.Request.Url.Host;
                _mailBody += "<tr><td><a href='" + BaseUri + "TransportationOrder/Edit?reqno=" + reqNo + "' target='_blank'>" + od.Key + "</a></td><td>" + od.Value + "</td><td>" + user + "</td></tr>";
            }
            // close table
            _mailBody += "</table><br /><br />Thank You";

            return _mailBody;
        }
        private string CreateOrderNotif(Dictionary<string, string> orderDetail, string reqNo, string user, bool isNewData)
        {
            return isNewData ?
                "Request Number <a href=\"/TransportationOrder/Edit?reqno=" + reqNo + "\" >" + reqNo + "</a> have been created by " + user :
                "Request Number <a href=\"/TransportationOrder/Edit?reqno=" + reqNo + "\" >" + reqNo + "</a> have been updated by " + user;
        }

        private void PrepareData(TransportOrderRequestDTO nvalue, TransportOrderRequest ovalue = null)
        {
            if (nvalue == null || ovalue == null) return;

            var updTime = DateTime.Now;
            var updBy = CurrentUser;

            // get old meta data
            //nvalue.IDRequest = ovalue.IDRequest != 0 ? ovalue.IDRequest : 0;
            if (ovalue.CreatedBy == null)
                ovalue.IsActive = true;
            ovalue.CreatedBy = ovalue.CreatedBy == null ? updBy : ovalue.CreatedBy;
            ovalue.CreatedDate = ovalue.CreatedDate == DateTime.MinValue ? updTime : ovalue.CreatedDate;
            ovalue.UpdatedDate = updTime;
            ovalue.UpdatedBy = updBy;
            ovalue.IsActive = true;
            ovalue.ShipmentDate = ovalue.ShipmentDate == DateTime.MinValue ? nvalue.ShipmentDate : ovalue.ShipmentDate;
            ovalue.Zone = nvalue.Zone == null ? ovalue.Zone : nvalue.Zone;
            ovalue.VehicleType = nvalue.VehicleType != null ? nvalue.VehicleType : ovalue.VehicleType;
            ovalue.RequestNo = !string.IsNullOrWhiteSpace(ovalue.RequestNo) ? ovalue.RequestNo : nvalue.RequestNo;

            nvalue._OldRecord = ovalue;

            // match route records
            foreach (var nroute in nvalue.TransportOrders)
            {
                // find old route
                //var oroute = ovalue.TransportOrders.Where(v => v.IDTransportOrder == nroute.IDTransportOrder).FirstOrDefault();
                var oroute = nroute.IDTransportOrder == 0 ? null : ovalue.TransportOrders.Where(v => v.IDTransportOrder == nroute.IDTransportOrder).FirstOrDefault();
                if (oroute == null)
                {
                    // data not found, create new data
                    oroute = MappingHelper.Map<TransportOrder>(nroute);
                    ovalue.TransportOrders.Add(oroute);
                    oroute.IDRequest = ovalue.IDRequest;
                    oroute.TransportOrderDetails = new List<TransportOrderDetail>();
                    oroute.IsActive = true;
                    oroute.ZoneBased = ovalue.Zone;
                    nroute.StatusChanged = true;
                    nroute.IsNewData = true;
                }
                else
                {
                    // data found, copy the new data values to old data                    
                    oroute.STONo = nroute.STONo;
                    oroute.OrderType = nroute.OrderType;
                    oroute.ActualSenderIDLocation = nroute.ActualSenderIDLocation;
                    oroute.ActualReceiverIDLocation = nroute.ActualReceiverIDLocation;
                    oroute.SenderIDLocation = nroute.SenderIDLocation;
                    oroute.ReceiverIDLocation = nroute.ReceiverIDLocation;
                    oroute.CostCenter = nroute.CostCenter;
                    if (oroute.OrderStatus != nroute.OrderStatus)
                        nroute.StatusChanged = true;
                    oroute.OrderStatus = nroute.OrderStatus;
                    oroute.ZoneBased = ovalue.Zone;
                    oroute.Remarks = nroute.Remarks;
                    oroute.SeqNo = nroute.SeqNo;
                    nroute.IsNewData = false;
                }

                nroute._OldRecord = oroute;

                // match all material
                foreach (var nmat in nroute.TransportOrderDetails)
                {
                    // find old material
                    var omat = nmat.IDTransportOrderDetail != 0 ? oroute.TransportOrderDetails.Where(m => m.IDTransportOrderDetail == nmat.IDTransportOrderDetail).FirstOrDefault() : null;
                    if (omat == null)
                    {
                        // data not found, create new data
                        omat = MappingHelper.Map<TransportOrderDetail>(nmat);
                        oroute.TransportOrderDetails.Add(omat);
                        omat.IDTransportOrder = oroute.IDTransportOrder;
                        omat.IsActive = true;
                    }
                    else
                    {
                        // data found, copy the new data values to old data
                        omat.MaterialType = nmat.MaterialType;
                        omat.Code = nmat.Code;
                        omat.Description = nmat.Description;
                        omat.Qty = nmat.Qty;
                        omat.UoM = nmat.UoM;
                    }

                    nmat._OldRecord = omat;
                }

                // but wait, delete material that is not listed in submitted data
                foreach(var omat in oroute.TransportOrderDetails)
                {
                    if (nroute.TransportOrderDetails.Where(tod => tod.IDTransportOrderDetail == omat.IDTransportOrderDetail).Count() <= 0)
                    {
                        omat.IsActive = false;
                    }
                }
            }

            // update common data
            foreach (var rt in ovalue.TransportOrders)
            {
                rt.CreatedBy = rt.CreatedBy == null ? updBy : rt.CreatedBy;
                rt.CreatedDate = rt.CreatedDate == DateTime.MinValue ? updTime : rt.CreatedDate;
                rt.UpdatedBy = updBy;
                rt.UpdatedDate = updTime;

                rt.VehicleType = ovalue.VehicleType;
                rt.ShipmentDate = ovalue.ShipmentDate;

                foreach (var mt in rt.TransportOrderDetails)
                {
                    mt.CreatedBy = mt.CreatedBy == null ? updBy : mt.CreatedBy;
                    mt.CreatedDate = mt.CreatedDate == DateTime.MinValue ? updTime : mt.CreatedDate;
                    mt.UpdatedBy = updBy;
                    mt.UpdatedDate = updTime;
                }
            }
        }

        private TransportOrderRequest PrepareSave(TransportOrderRequestDTO value, TransportOrderRequestHeader header)
        {
            // filters out the same record
            var existingRecords = header.TransportOrderRequests.Where(v =>
            (value.IDRequest == v.IDRequest && value.IDRequest != 0)
            );
            TransportOrderRequest record = null;
            value.UpdatedDate = DateTime.Now;

            var Routes = new List<TransportOrder>();
            if (existingRecords.Count() == 0)
            {
                // setup unit (TOR)
                value.CreatedDate = DateTime.Now;
                value.CreatedBy = value.UpdatedBy;
                value.IsActive = true;

                // Map Unit to transport order request
                record = MappingHelper.Map<TransportOrderRequest>(value);
                record.TransportOrders = new List<TransportOrder>();
                // Fill missing values
                PrepareData(value, record);

                // insert should be done here
                /*
                _genTraOrdReqRepo.Insert(record);
                _genTraOrdReqRepo.Save();
                */
                value.IDRequest = record.IDRequest;
            }
            else
            {
                // update mode
                record = existingRecords.Where(mtl => mtl.IDRequest == value.IDRequest).FirstOrDefault();
                if (record != null)
                {
                    PrepareData(value, record);

                    // update should be done here
                    /*
                    _genTraOrdReqRepo.Update(record);
                    _genTraOrdReqRepo.Save();
                    */
                    value.IDRequest = record.IDRequest;
                }
                else
                {
                    // this is actually a new data with the same request number
                    record = new TransportOrderRequest();
                    PrepareData(value, record);

                    /*
                    _genTraOrdReqRepo.Insert(record);
                    _genTraOrdReqRepo.Save();
                    */
                    value.IDRequest = record.IDRequest;
                }
            }

            return record;
        }

        /** Insert or Update  **/
        public void InsertOrUpdate(TransportOrderRequestDTO value, bool canUpdate = true)
        {
        }

        public void InsertOrUpdate(TransportOrderDTO value, bool canUpdate = true)
        {
            var extRecord = _genTraOrdRepo.Get(v => v.IDTransportOrder == value.IDTransportOrder).FirstOrDefault();
            var Materials = new List<TransportOrderDetail>();
            if (extRecord == null)
            {
                // new data
                extRecord = Mapper.Map<TransportOrder>(value);
                Materials.AddRange(extRecord.TransportOrderDetails);
                extRecord.TransportOrderDetails.Clear();
                _genTraOrdRepo.Insert(extRecord);
                _genTraOrdRepo.Save();
            }
            else
            {
                // update
                Materials.AddRange(extRecord.TransportOrderDetails);
                Mapper.Map(value, extRecord);
                extRecord.TransportOrderDetails.Clear();
                _genTraOrdRepo.Update(extRecord);
                _genTraOrdRepo.Save();
            }
            // insert or update materials separately
            foreach (var mate in Materials)
            {
                InsertOrUpdate(Mapper.Map<TransportOrderDetailDTO>(mate));
            }
        }

        public void InsertOrUpdate(TransportOrderDetailDTO value, bool canUpdate = true)
        {
            var extRecord = _genTraOrdDetRepo.Get(v => v.IDTransportOrderDetail == value.IDTransportOrderDetail).FirstOrDefault();
            if (extRecord == null)
            {
                // new data
                extRecord = Mapper.Map<TransportOrderDetail>(value);
                _genTraOrdDetRepo.Insert(extRecord);
                _genTraOrdDetRepo.Save();
            }
            else
            {
                // update
                Mapper.Map(value, extRecord);
                _genTraOrdDetRepo.Update(extRecord);
                _genTraOrdDetRepo.Save();
            }
        }

        public int CountVehicle(string requestNumber, bool activeOnly = true)
        {
            return _genTraOrdReqRepo.Get(c => c.RequestNo == requestNumber && (c.IsActive == activeOnly || activeOnly == false)).Count();
        }

        //public List<TransportOrderDTO> GetLocatioByTn(string transportno)
        //{
        //    using (TOMContextDB dbContext = new TOMContextDB())
        //    {
        //        var dbResult = (from a in dbContext.TransportOrders
        //                        join b in dbContext.TransportExecutions on a.IDTransportExecution equals b.IDTransportExecution
        //                        join c in dbContext.MasterLocations on a.SenderIDLocation equals c.IDLocation
        //                        join d in dbContext.MasterLocations on a.ReceiverIDLocation equals d.IDLocation
        //                        where
        //                        b.TransportNo.ToUpper() == transportno.ToUpper() &&
        //                        (a.LoadStartTime == null || a.LoadFinishTime == null ||
        //                        a.UnloadStartTime == null || a.UnloadFinishTime == null)
        //                        select new TransportOrderDTO()
        //                        {
        //                            SenderIDLocation = a.SenderIDLocation,
        //                            ReceiverIDLocation = a.ReceiverIDLocation,
        //                            SenderLocationName = c.LocationName,
        //                            ReceiverLocationName = d.LocationName
        //                        }).ToList();
        //        var result = new List<TransportOrderDTO>();
        //        foreach (var data in dbResult)
        //        {
        //            var list = new TransportOrderDTO();
        //            var list2 = new TransportOrderDTO();
        //            list.IdLocation = data.SenderIDLocation;
        //            list.LocationName = data.SenderLocationName;
        //            result.Add(list);
        //            list2.IdLocation = data.ReceiverIDLocation;
        //            list2.LocationName = data.ReceiverLocationName;
        //            result.Add(list2);
        //        }
        //        var sort = result.GroupBy(x => new { x.IdLocation, x.LocationName }).Select(x => new TransportOrderDTO
        //        {
        //            IdLocation = x.Key.IdLocation,
        //            LocationName = x.Key.LocationName
        //        }).ToList();
        //        return sort;
        //    }

        //}
        public IEnumerable<DateTime> GetTransportDate()
        {
            var listTransportDate = _transportOrderRepo.GetTransportDateEnumerable();
            return listTransportDate;
        }
        public List<TransportOrderDTO> GetSenderLocationByTn()
        {
            var result = new List<TransportOrderDTO>();
            using (var context = new TOMContextDB())
            {
                var senderLocationList = (from a in context.TransportOrders
                                          join b in context.TransportExecutions on a.IDTransportExecution equals b.IDTransportExecution
                                          join c in context.MasterLocations on a.SenderIDLocation equals c.IDLocation
                                          //where b.TransportNo.ToUpper() == transportno.ToUpper() //&& (a.LoadStartTime == null || a.LoadFinishTime == null)
                                          select new
                                          {
                                              a.SenderIDLocation,
                                              c.LocationName
                                          }).Distinct().ToList(); // bdg,sby
                result.AddRange(senderLocationList.Select(data => new TransportOrderDTO
                {
                    IdLocation = data.SenderIDLocation,
                    LocationName = data.LocationName
                }));
            }
            return result;
        }
        public List<TransportOrderDTO> GetReceiverLocationByTn()
        {
            var result = new List<TransportOrderDTO>();
            using (var context = new TOMContextDB())
            {
                var receiverLocationList = (from a in context.TransportOrders
                                            join b in context.TransportExecutions on a.IDTransportExecution equals b.IDTransportExecution
                                            join c in context.MasterLocations on a.ReceiverIDLocation equals c.IDLocation
                                            //where b.TransportNo.ToUpper() == transportno.ToUpper() //&& (a.UnloadStartTime == null || a.UnloadFinishTime == null)
                                            select new
                                            {
                                                a.ReceiverIDLocation,
                                                c.LocationName
                                            }).Distinct().ToList(); // sby,malang
                result.AddRange(receiverLocationList.Select(data => new TransportOrderDTO
                {
                    IdLocation = data.ReceiverIDLocation,
                    LocationName = data.LocationName
                }));
            }
            return result;
        }
        public List<TransportOrderDTO> GetLocatioByTn(string transportno)
        {
            var result = new List<TransportOrderDTO>();
            using (var context = new TOMContextDB())
            {
                var senderLocationList = (from a in context.TransportOrders
                                          join b in context.TransportExecutions on a.IDTransportExecution equals b.IDTransportExecution
                                          join c in context.MasterLocations on a.SenderIDLocation equals c.IDLocation
                                          where
                                              b.TransportNo.ToUpper() == transportno.ToUpper() &&
                                              (a.LoadStartTime == null || a.LoadFinishTime == null)
                                          select new
                                          {
                                              a.SenderIDLocation,
                                              c.LocationName
                                          }).Distinct().ToList(); // bdg,sby
                //var senderLocationList = GetSenderLocationByTn(transportno);
                result.AddRange(senderLocationList.Select(data => new TransportOrderDTO
                {
                    IdLocation = data.SenderIDLocation,
                    LocationName = data.LocationName
                }));
                //result.AddRange(senderLocationList);
                var receiverLocationList = (from a in context.TransportOrders
                                            join b in context.TransportExecutions on a.IDTransportExecution equals b.IDTransportExecution
                                            join c in context.MasterLocations on a.ReceiverIDLocation equals c.IDLocation
                                            where
                                                b.TransportNo.ToUpper() == transportno.ToUpper() &&
                                                (a.UnloadStartTime == null || a.UnloadFinishTime == null)
                                            select new
                                            {
                                                a.ReceiverIDLocation,
                                                c.LocationName
                                            }).Distinct().ToList(); // sby,malang
                result.AddRange(receiverLocationList.Select(data => new TransportOrderDTO
                {
                    IdLocation = data.ReceiverIDLocation,
                    LocationName = data.LocationName
                }));
                //var receiverLocationList = GetReceiverLocationByTn(transportno);
                //result.AddRange(receiverLocationList);
                var dbReturn = result.GroupBy(x => new { x.IdLocation, x.LocationName }).Select(x => new TransportOrderDTO
                {
                    IdLocation = x.Key.IdLocation,
                    LocationName = x.Key.LocationName
                }).ToList();
                return dbReturn;
            }
        }

        public List<TransportExecutionDTO> GetTransportationNumberFilterByRegion(string stono, bool isUserRoleTransport, List<string> parentLocation)
        {
            return Mapper.Map<IEnumerable<TransportExecution>, List<TransportExecutionDTO>>(_transportExecutionRepo.GetTransportExecutionFilter(stono));
            /*
            if (isUserRoleTransport)
                return Mapper.Map<List<TransportExecution>, List<TransportExecutionDTO>>(_transportExecutionRepo.GetTransportExecutionFilter(stono));
            else
                return Mapper.Map<List<TransportExecution>, List<TransportExecutionDTO>>(_transportOrderRepo.GetTransportationNumberFilterByListRegion(stono, parentLocation));
             */
        }

        public List<TransportOrderDTO> GetTransportOrderByTransactionNo(string id, string state)
        {
            var dbResult = new List<TransportOrderDTO>();
            var context = new TOMContextDB();
            if (state == "new")
            {
                //existingstate 1 = data ada perlu dilanjutkan
                //existingstate 2 = data sudah lengkap
                //existingstate 3 = data tn tidak ada

                var existing =
                    context.TransportExecutions.Where(x => x.TransportNo == id)
                        .Select(x => x.TransportNo)
                        .FirstOrDefault();
                if (existing != null)
                {
                    dbResult = (from a in context.TransportExecutions
                                join b in context.TransportOrders on a.IDTransportExecution equals b.IDTransportExecution
                                join c in context.MasterLocations on b.SenderIDLocation equals c.IDLocation
                                join d in context.MasterLocations on b.ReceiverIDLocation equals d.IDLocation
                                where
                                    a.TransportNo.ToUpper() == id.ToUpper() &&
                                    (b.LoadStartTime == null || b.LoadFinishTime == null ||
                                     b.UnloadStartTime == null || b.UnloadFinishTime == null)
                                //where a.TransportNo.ToUpper() == id.ToUpper()
                                select new TransportOrderDTO()
                                {
                                    IDTransportOrder = b.IDTransportOrder,
                                    STONo = b.STONo,
                                    ActualSenderIDLocation = b.ActualSenderIDLocation,
                                    SenderIDLocation = b.SenderIDLocation,
                                    ActualReceiverIDLocation = b.ActualReceiverIDLocation,
                                    ReceiverIDLocation = b.ReceiverIDLocation,
                                    SenderLocationName = c.LocationName,
                                    ReceiverLocationName = d.LocationName,
                                    IDTransportExecution = a.IDTransportExecution,
                                    LoadStartTime = b.LoadStartTime,
                                    LoadFinishTime = b.LoadFinishTime,
                                    UnloadStartTime = b.UnloadStartTime,
                                    UnloadFinishTime = b.UnloadFinishTime,
                                    LoadBoxperWorkingTime = b.LoadBoxperWorkingTime,
                                    UnloadBoxperWorkingTime = b.UnloadBoxperWorkingTime,
                                    ExistingState = 1,
                                    EstArrivalDate = b.EstArrivalDate,
                                    GIDate = b.GIDate,
                                    GRDate = b.GRDate,
                                    OrderStatus = b.OrderStatus,
                                    CreatedBy = b.CreatedBy,
                                    UpdatedBy = b.UpdatedBy,
                                    CreatedDate = b.CreatedDate,
                                    UpdatedDate = b.UpdatedDate,
                                    GRBy = b.GRBy,
                                    GIBy = b.GIBy
                                }).ToList();

                    if (!dbResult.Any())
                    {
                        var result = new TransportOrderDTO();
                        result.ExistingState = 2;
                        dbResult.Add(result);
                    }
                }
                else
                {
                    // data muncul tapi ditandai state false
                    // data hanya satu baris
                    var data = new TransportOrderDTO();
                    data.ExistingState = 3;
                    dbResult.Add(data);
                }
            }
            else
            {
                dbResult = (from a in context.TransportExecutions
                            join b in context.TransportOrders on a.IDTransportExecution equals b.IDTransportExecution
                            join c in context.MasterLocations on b.SenderIDLocation equals c.IDLocation
                            join d in context.MasterLocations on b.ReceiverIDLocation equals d.IDLocation
                            where a.TransportNo.ToUpper() == id.ToUpper()
                            select new TransportOrderDTO()
                            {
                                IDTransportOrder = b.IDTransportOrder,
                                STONo = b.STONo,
                                LeadTime = b.LeadTime,
                                ActualSenderIDLocation = b.ActualSenderIDLocation,
                                SenderIDLocation = b.SenderIDLocation,
                                ActualReceiverIDLocation = b.ActualReceiverIDLocation,
                                ReceiverIDLocation = b.ReceiverIDLocation,
                                SenderLocationName = c.LocationName,
                                ReceiverLocationName = d.LocationName,
                                IDTransportExecution = a.IDTransportExecution,
                                LoadStartTime = b.LoadStartTime,
                                LoadFinishTime = b.LoadFinishTime,
                                UnloadStartTime = b.UnloadStartTime,
                                UnloadFinishTime = b.UnloadFinishTime,
                                LoadBoxperWorkingTime = b.LoadBoxperWorkingTime,
                                UnloadBoxperWorkingTime = b.UnloadBoxperWorkingTime,
                                EstArrivalDate = b.EstArrivalDate,
                                GIDate = b.GIDate,
                                GRDate = b.GRDate,
                                OrderStatus = b.OrderStatus,
                                CreatedBy = b.CreatedBy,
                                UpdatedBy = b.UpdatedBy,
                                CreatedDate = b.CreatedDate,
                                UpdatedDate = b.UpdatedDate,
                                GRBy = b.GRBy,
                                GIBy = b.GIBy
                            }).ToList();
                return dbResult;
            }
            return dbResult;
        }

        public List<TransportOrderDTO> GetTransportOrderByIDTransportExecution(int id)
        {
            return Mapper.Map<List<TransportOrderDTO>>(_genTraOrdRepo.Get(c => c.IDTransportExecution == id));
        }
    }
}

