using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Schema;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using AutoMapper;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Configuration;

namespace TOM.Transport.Repositories.TransportOrderRepo
{
    public class TransportOrderRepo : TOMGenericRepository<TransportOrder>, ITransportOrderRepo
    {
        private TOMContextDB _context;
        private readonly IGenericRepository<LoadingUnloadingExcelViewDTO> _loadexcelview;

        public TransportOrderRepo(TOMContextDB contextEntities, ITOMGenericRepository<LoadingUnloadingExcelViewDTO> loadexcelview)
            : base(contextEntities)
        {
            _context = contextEntities;
            _loadexcelview = loadexcelview;
        }

        public List<SP_GetTransportOrderRequestDTO> GetTransportOrderRequestSummary(string oNFilter = "", string zoneFilter = "", string oTFilter = "", string oVFilter = "", Nullable<System.DateTime> shipDateBeginFilter = null, Nullable<System.DateTime> shipDateEndFilter = null, string mTFilter = "", string statusFilter = "", string senderFilter = "", string receiverFilter = "", string creatorFilter = "", string locationFilter = "", bool allAccess = false)
        {
            var res = _context.SP_GetTransportOrderRequest(oNFilter, zoneFilter, oTFilter, oVFilter, shipDateBeginFilter, shipDateEndFilter, mTFilter, statusFilter, senderFilter, receiverFilter, creatorFilter, locationFilter, allAccess ? (byte)1 : (byte)0).ToList();
            
            return Mapper.Map<List<SP_GetTransportOrderRequestDTO>>(res);
        }

        public List<TransportOrderTransportExecutionPrintDTO> GetTransportOrderFilterByidTE(int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            //queryFilter = queryFilter.And(x => idTE.Contains(x.TransportExecution.IDTransportExecution));
            queryFilter = queryFilter.And(x => x.TransportExecution.IDTransportExecution == idTE);
            queryFilter = queryFilter.And(x => x.IsActive);
            return ReturnListObjectWithJoinView2(queryFilter);
        }
        public List<TransportOrderTransportExecutionPrintDTO> ReturnListObjectWithJoinView2(Expression<Func<TransportOrder, bool>> filter)
        {
            return Get(filter)
                .GroupBy(x => new { x.IDTransportExecution, x.ActualSenderIDLocation, x.ActualReceiverIDLocation })
                .Select(x => x.FirstOrDefault())
                .Join(_context.MasterLocations, to => to.ActualSenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to2 => to2.to.ActualReceiverIDLocation, ml2 => ml2.IDLocation, (to2, ml2) => new { to2.to, to2.ml, ml2 })
                .GroupJoin(_context.TransportDriverManagements, to3 => to3.to.TransportExecution.IDDriver1, tdm => tdm.ID, (to3, tdm) => new { to3.to, to3.ml, to3.ml2, tdm })
                .GroupJoin(_context.TransportDriverManagements, to4 => to4.to.TransportExecution.IDDriver2, tdm1 => tdm1.ID, (to4, tdm1) => new { to4.to, to4.tdm, to4.ml, to4.ml2, tdm1 })
                .GroupJoin(_context.TransportDriverManagements, to5 => to5.to.TransportExecution.IDCoDriver, tdm2 => tdm2.ID, (to5, tdm2) => new { to5.to, to5.tdm, to5.ml, to5.ml2, to5.tdm1, tdm2 })
                .GroupJoin(_context.TransportVesselMonitorings, to6 => to6.to.TransportExecution.IDTransportExecution, tvm => tvm.IDTransportExecution, (to6, tvm) => new { to6.to, to6.tdm2, to6.tdm, to6.tdm1, to6.ml, to6.ml2, tvm })
                .GroupJoin(_context.TransportVesselMonitorings, to6 => to6.to.TransportExecution.IDTransportExecution, tvm => tvm.IDTransportExecution, (to6, tvm) => new { to6.to, to6.tdm2, to6.tdm, to6.tdm1, to6.ml, to6.ml2, tvm })
                .GroupJoin(_context.GeneralBuildingFacilities, to7 => to7.to.SenderIDLocation, gbf => gbf.IDLocation, (to7, gbf) => new { to7.to, to7.tdm2, to7.tdm, to7.tdm1, to7.ml, to7.ml2, to7.tvm, gbf })
                .GroupJoin(_context.GeneralBuildingFacilities, to8 => to8.to.ReceiverIDLocation, gbf1 => gbf1.IDLocation, (to8, gbf1) => new { to8.to, to8.tdm2, to8.tdm, to8.tdm1, to8.ml, to8.ml2, to8.tvm, to8.gbf, gbf1 })
                .GroupJoin(_context.MasterVendors, to9 => to9.to.IDTransportExecution, mv => mv.IDVendor, (to9, mv) => new { to9.to, to9.tdm2, to9.tdm, to9.tdm1, to9.ml, to9.ml2, to9.tvm, to9.gbf, to9.gbf1, mv })
                .Join(_context.MasterLocations, to10 => to10.to.TransportExecution.StartLocation, ml3 => ml3.IDLocation, (to10, ml3) => new { to10.to, to10.tdm2, to10.tdm, to10.tdm1, to10.ml, to10.ml2, to10.tvm, to10.gbf, to10.gbf1, to10.mv, ml3 })
                .Select(x => new TransportOrderTransportExecutionPrintDTO(x.to, x.ml, x.ml2, x.to.TransportExecution, x.tdm.ToList(), x.tdm1.ToList(), x.tdm2.ToList(), x.tvm.ToList(), x.gbf.ToList(), x.gbf1.ToList(), x.mv.ToList(), x.ml3)).ToList();

        }

        public TransportOrderDTO ReturnObjectWithJoinView(Expression<Func<TransportOrder, bool>> filter)
        {
            return Get(filter)
                .Join(_context.MasterLocations, to => to.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to2 => to2.to.ReceiverIDLocation, ml2 => ml2.IDLocation, (to2, ml2) => new { to2.to, to2.ml, ml2 })
                .Join(_context.MasterLocations, to3 => to3.to.ActualSenderIDLocation, ml3 => ml3.IDLocation, (to3, ml3) => new { to3.to, to3.ml, to3.ml2, ml3 })
                .Join(_context.MasterLocations, to4 => to4.to.ActualReceiverIDLocation, ml4 => ml4.IDLocation, (to4, ml4) => new { to4.to, to4.ml, to4.ml2, to4.ml3, ml4 })
                .Select(x => new TransportOrderDTO(x.to, x.to.TransportOrderDetails.ToList(), x.to.TransportExecution, x.ml, x.ml2, x.ml3, x.ml4)).FirstOrDefault();
        }

        public List<TransportOrderDTO> ReturnListObjectWithJoinView(Expression<Func<TransportOrder, bool>> filter)
        {
            return Get(filter)
                .Join(_context.MasterLocations, to => to.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to2 => to2.to.ReceiverIDLocation, ml2 => ml2.IDLocation, (to2, ml2) => new { to2.to, to2.ml, ml2 })
                .Join(_context.MasterLocations, to3 => to3.to.ActualSenderIDLocation, ml3 => ml3.IDLocation, (to3, ml3) => new { to3.to, to3.ml, to3.ml2, ml3 })
                .Join(_context.MasterLocations, to4 => to4.to.ActualReceiverIDLocation, ml4 => ml4.IDLocation, (to4, ml4) => new { to4.to, to4.ml, to4.ml2, to4.ml3, ml4 })
                .Select(x => new TransportOrderDTO(x.to, x.to.TransportOrderDetails.ToList(), x.to.TransportExecution, x.ml, x.ml2, x.ml3, x.ml4)).ToList();
        }

        public List<TransportOrderDTO> ReturnListObjectWithJoinView1(Expression<Func<TransportOrder, bool>> filter)
        {
            return Get(filter)
                .Join(_context.MasterLocations, to => to.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to2 => to2.to.ReceiverIDLocation, ml2 => ml2.IDLocation, (to2, ml2) => new { to2.to, to2.ml, ml2 })
                .Select(x => new TransportOrderDTO(x.to, x.to.TransportOrderDetails.ToList(), x.to.TransportExecution, x.ml, x.ml2,null,null)).ToList();
        }

        public List<TransportOrderDTO> GetTOActiveByTN(string transNo, string roleName)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportExecution.TransportNo == transNo);
            if (roleName.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                queryFilter = queryFilter.And(x => roleName.Contains(x.TransportExecution.TransportMode));
            }
            return ReturnListObjectWithJoinView(queryFilter);
        }

        public List<TransportOrderDTO> GetTOActiveByTNs(string transNo, string senderLocation, string receiverLocation, string transportDate)
        {
            return GetTOActiveByTNs(transNo, new string[] { senderLocation }, new string[] { receiverLocation }, transportDate);
        }


        public List<TransportOrderDTO> GetTOActiveByTNs(string transNo, string[] senderLocation, string[] receiverLocation, string transportDate)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportExecution.IDTransportExecution != 0);
            if (!String.IsNullOrEmpty(transNo))
            {
                queryFilter = queryFilter.And(x => x.TransportExecution.TransportNo == transNo);
            }
            if (senderLocation.Length > 0)
            {
                queryFilter = queryFilter.And(x => senderLocation.Contains( x.SenderIDLocation));
            }
            if (receiverLocation.Length > 0)
            {
                queryFilter = queryFilter.And(x => receiverLocation.Contains( x.ReceiverIDLocation ));
            }
            if (!String.IsNullOrEmpty(transportDate))
            {
                var tDate = Convert.ToDateTime(transportDate).Date;
                queryFilter = queryFilter.And(x => x.TransportExecution.TransportDate == tDate);
            }

            return ReturnListObjectWithJoinView1(queryFilter);
        }


        public List<TransportOrderDTO> GetTOActiveByTNs(string transNo, string[] senderLocation, string[] receiverLocation, DateTime? transportStartDate, DateTime? transportEndDate, string roleName)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportExecution.IDTransportExecution != 0);
            if (!String.IsNullOrEmpty(transNo))
            {
                queryFilter = queryFilter.And(x => x.TransportExecution.TransportNo == transNo);
            }
            if (senderLocation.Length > 0)
            {
                queryFilter = queryFilter.And(x => senderLocation.Contains(x.SenderIDLocation));
            }
            if (receiverLocation.Length > 0)
            {
                queryFilter = queryFilter.And(x => receiverLocation.Contains(x.ReceiverIDLocation));
            }
            if (transportStartDate.HasValue)
            {
                queryFilter = queryFilter.And(x => x.TransportExecution.TransportDate <= transportStartDate.Value);
            }
            if (transportEndDate.HasValue)
            {
                queryFilter = queryFilter.And(x => x.TransportExecution.TransportDate >= transportEndDate.Value);
            }
            if (roleName.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                queryFilter = queryFilter.And(x => roleName.Contains(x.TransportExecution.TransportMode));
            }

            return ReturnListObjectWithJoinView1(queryFilter);
        }

        public List<TransportOrder> GetSTONoFilter(string stono)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.STONo.ToLower().Contains(stono));
            //return Get(0, 10, queryFilter).OrderByDescending(x => x.ShipmentDate).ToList();
            return Get(queryFilter).Skip(0).Take(10).ToList();
        }

        public List<TransportOrder> GetSTONoFilterByListRegion(string stono, List<string> region)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.STONo.Contains(stono));
            return Get(0, 10, queryFilter)
                .Join(_context.MasterLocations, to => to.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to1 => to1.to.ReceiverIDLocation, ml2 => ml2.IDLocation, (to1, ml2) => new { to1.to, to1.ml, ml2 })
                .Where(x => region.Contains(x.ml.ParentLocation) || region.Contains(x.ml2.ParentLocation))
                .GroupBy(x => x.to.STONo)
                .Select(x => x.FirstOrDefault().to)
                .OrderByDescending(x => x.ShipmentDate)
                .ToList();
        }

        public List<TransportOrderDTO> GetAllTransportOrder(TransportOrderInput criteria)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive && x.IDTransportPickingListLog != null);
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
                queryFilter = queryFilter.And(x => criteria.senderIdLocListFilter.Contains(x.SenderIDLocation));
            }
            if (criteria.receiverIdLocListFilter != null && !String.IsNullOrEmpty(criteria.receiverIdLocListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.receiverIdLocListFilter.Contains(x.ReceiverIDLocation));
            }
            if (criteria.uploadedByFilter != null && criteria.uploadedByFilter != "")
            {
                queryFilter = queryFilter.And(x => criteria.uploadedByFilter.Contains(x.CreatedBy));
            }
            return ReturnListObjectWithJoinView(queryFilter).ToList();
        }

        public List<string> GetAllUploadedBy(TransportOrderInput criteria)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
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
                queryFilter = queryFilter.And(x => criteria.senderIdLocListFilter.Contains(x.SenderIDLocation));
            }
            if (criteria.receiverIdLocListFilter != null && !String.IsNullOrEmpty(criteria.receiverIdLocListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.receiverIdLocListFilter.Contains(x.ReceiverIDLocation));
            }

            var listAllresult = Get(queryFilter).Where(x => x.IDRequest == null).Select(x => x.CreatedBy).ToList();

            return listAllresult.Distinct().ToList();
        }

        public TransportOrderDTO GetTransportOrderBySTONo(string stono)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.STONo == stono);
            return ReturnObjectWithJoinView(queryFilter);
        }

        public TransportOrder GetTransportOrderBySTONo2(string stono)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.STONo == stono);
            return Get(queryFilter).FirstOrDefault();
        }

        public TransportOrderDTO GetTransportOrderActiveBySTONo(string stono)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.STONo == stono);
            return ReturnObjectWithJoinView(queryFilter);
        }

        //filter by region, karena lokasi cuma ada di transport order, maka querynya di taruh di transport order
        //functionnya di pakai untuk memanggil list transportation execution
        public List<TransportExecution> GetTransportationNumberFilterByListRegion(string transno, List<string> region)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.TransportExecution.IsActive);
            queryFilter = queryFilter.And(x => x.TransportExecution.TransportNo.Contains(transno));
            //queryFilter = queryFilter.And(x => region.Contains(x.MasterLocation3.ParentLocation) || region.Contains(x.MasterLocation2.ParentLocation));
            List<TransportOrder> temp = Get(0, 10, queryFilter)
                .Join(_context.MasterLocations, to => to.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to1 => to1.to.ReceiverIDLocation, ml2 => ml2.IDLocation, (to1, ml2) => new { to1.to, to1.ml, ml2 })
                .Where(x => region.Contains(x.ml.ParentLocation) || region.Contains(x.ml2.ParentLocation))
                .GroupBy(x => x.to.TransportExecution)
                .Select(x => x.FirstOrDefault().to)
                .OrderByDescending(x => x.TransportExecution.TransportDate).ToList();
            return temp.Select(x => x.TransportExecution).ToList();
        }

        public int SaveData(TransportOrder input)
        {
            //var updatedData = Mapper.Map<TransportOrderDTO, TransportOrder>(GetTransportOrderBySTONo(input.STONo));
            //updatedData.SeqNo = input.SeqNo;
            //updatedData.VehicleType = input.VehicleType;
            //updatedData.Remarks = input.Remarks;
            input.UpdatedDate = DateTime.Now;
            Update(input);
            Save();
            return 0;
        }
        public int InsertData(TransportOrder input)
        {
            input.UpdatedDate = DateTime.Now;
            input.CreatedDate = DateTime.Now;
            Insert(input);
            Save();
            return input.IDTransportOrder;
        }
        public int SaveDataTOFromUpload(TransportOrder input)
        {
            /*Debug.WriteLine("insert TO");
            Insert(input);                      
            Save();                            
            return input.IDTransportOrder;*/
            TOMContextDB context = null;
            try
            {
                context = new TOMContextDB();
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                int count = 0;
                context = AddToContext(context, input, count, 100, true);
                context.SaveChanges();
                return input.IDTransportOrder;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        private TOMContextDB AddToContext(TOMContextDB context, TransportOrder entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<TransportOrder>().Add(entity);
            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new TOMContextDB();
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                }
            }
            return context;
        }

        //public int SaveData(TransportOrder input)
        //{
        //    /*Debug.WriteLine("insert TO");
        //    Insert(input);                      
        //    Save();                            
        //    return input.IDTransportOrder;*/
        //    DFISContextDB context = null;
        //    try
        //    {
        //        context = new DFISContextDB();
        //        context.Configuration.AutoDetectChangesEnabled = false;
        //        context.Configuration.ValidateOnSaveEnabled = false;
        //        int count = 0;
        //        context = AddToContext(context, input, count, 100, true);
        //        context.SaveChanges();
        //        return input.IDTransportOrder;
        //    }
        //    finally
        //    {
        //        if (context != null)
        //            context.Dispose();
        //    }
        //}

        public string getNewSeqNo(DateTime shipmentDate)
        {
            var lastSeqNo = Get(d => d.ShipmentDate == shipmentDate
            && d.SeqNo != null && d.SeqNo.StartsWith("TO"))
            .OrderByDescending(p => p.SeqNo).Select(s => s.SeqNo).FirstOrDefault();
            int nextSeq = 1;
            if (lastSeqNo != null)
            {
                nextSeq = int.Parse(lastSeqNo.Substring(2)) + 1;
            }
            return "TO" + nextSeq.ToString().PadLeft(3, '0');
        }

        public int getLastSequenceByShipmentDate(DateTime shipmentDate)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            //queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.ShipmentDate == shipmentDate);
            queryFilter = queryFilter.And(p => p.IDTransportPickingListLog != null);
            queryFilter = queryFilter.And(p => !string.IsNullOrEmpty(p.DefaultSeqNo));
            var rowTO = Get(queryFilter).OrderByDescending(p => p.IDTransportOrder).FirstOrDefault();
            if (rowTO == null)
            {
                return 1000;
            }
            else
            {
                return Int32.Parse(rowTO.DefaultSeqNo);
            }
        }
        public string getSeqUnitByShipmentDate(DateTime shipmentDate)
        {
            var promiseDate = Convert.ToDateTime(shipmentDate.Date.ToString("yyyy-MM-dd"));
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.ShipmentDate == promiseDate);
            queryFilter = queryFilter.And(p => !String.IsNullOrEmpty(p.DefaultSeqNo));
            queryFilter = queryFilter.And(p => p.IDTransportPickingListLog == null);
            var rowTO = Get(queryFilter).OrderByDescending(p => p.IDTransportOrder).FirstOrDefault();
            if (rowTO == null)
            {
                return "TO1";
            }
            else
            {
                var DefaultSeq = "";
                var LastSeq = rowTO.DefaultSeqNo;
                var newSeq = Int16.Parse(LastSeq.Substring(2)) + 1;
                DefaultSeq = "TO" + newSeq.ToString();
                return DefaultSeq;
            }
        }

        private Dictionary<string, string> _generatedSerials = new Dictionary<string, string>();
        public string getSequenceNo(DateTime shipmentDate, int RequestID = 0)
        {
            if (RequestID > 0)
            {
                var q = PredicateHelper.True<TransportOrder>();
                q = q.And(to => to.IDRequest == RequestID);
                q = q.And(p => !String.IsNullOrEmpty(p.SeqNo));
                var old = Get(q).FirstOrDefault();
                if (old != null)
                    return old.SeqNo;
            }

            var shLow = shipmentDate.Date;
            var shHigh = shipmentDate.Date + new TimeSpan(1, 0, 0, 0);

            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.ShipmentDate >= shLow && p.ShipmentDate <= shHigh);
            queryFilter = queryFilter.And(p => !String.IsNullOrEmpty(p.SeqNo));
            var rowTO = Get(queryFilter).OrderByDescending(p => p.IDTransportOrder).FirstOrDefault();

            var serial = 1;
            if (rowTO != null)
            {
                var LastSeq = rowTO.SeqNo;
                serial = int.Parse(LastSeq.Substring(2)) + 1;
            }

            var lst = _generatedSerials.ContainsKey(shipmentDate.ToString("yyyyMMdd")) ? int.Parse(_generatedSerials[shipmentDate.ToString("yyyyMMdd")].Substring(2)) : 0;
            if (serial <= lst)
                serial = lst + 1;
            var nto = "TO" + serial.ToString().PadLeft(3, '0');
            _generatedSerials[shipmentDate.ToString("yyyyMMdd")] = nto;
            return nto;
        }
        public string getLastSTOPreOrder(DateTime shipmentDate)
        {
            string STOx = "";
            //var promiseDate = shipmentDate.Date.ToString("yy-MM");
            //var promiseDate = shipmentDate.Date.ToString("yy-MM");
            var queryFilter = PredicateHelper.True<TransportOrder>();
            //queryFilter = queryFilter.And(p => p.ShipmentDate.ToString().Contains(promiseDate));
            queryFilter = queryFilter.And(p => p.ShipmentDate.Month == shipmentDate.Month);
            queryFilter = queryFilter.And(p => p.ShipmentDate.Year == shipmentDate.Year);
            //queryFilter = queryFilter.And(p => p.STONo.Contains("PO" + shipmentDate.Date.ToString("yyMM")));
            queryFilter = queryFilter.And(p => string.IsNullOrEmpty(p.DefaultSeqNo));
            queryFilter = queryFilter.And(p => p.IDTransportPickingListLog != null);
            //var rowTO2 =  Get(queryFilter).Where(x => x.STONo == null).FirstOrDefault().
            var rowTO = Get(queryFilter).Where(x => x.STONo != null && x.STONo.Contains("PO" + shipmentDate.Date.ToString("yyMM"))).OrderByDescending(p => p.IDTransportOrder).FirstOrDefault();
            //string STONo = "PO-" + DateTime.Now.ToString("MMyy") + "-";
            //string STONo = "PO" + DateTime.Now.ToString("yyMM");
            string STONo = "PO" + shipmentDate.Date.ToString("yyMM");
            if (rowTO == null)
                STOx = STONo + "0000";
            else
                STOx = rowTO.STONo;

            return STOx;
        }
        /*public string getLastSTOPreOrder(DateTime shipmentDate)
        {
            string STOx = "";
            var promiseDate = shipmentDate.Date.ToString("yy-MM");
            var queryFilter = PredicateHelper.True<TransportOrder>();
            //queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.ShipmentDate.ToString().Contains(promiseDate));
            queryFilter = queryFilter.And(p => string.IsNullOrEmpty(p.DefaultSeqNo));
            queryFilter = queryFilter.And(p => p.IDTransportPickingListLog != null);
            var rowTO = Get(queryFilter).OrderByDescending(p => p.IDTransportOrder).FirstOrDefault();
            string STONo = "PO-" + DateTime.Now.ToString("MMyy") + "-";
            if (rowTO == null)
            {
                //STONo = "empty";
                STOx = STONo + "01";
            }
            else
            {
                //Debug.WriteLine("Get Last STO:" + rowTO.STONo);
                //int strSTO = 0;
                var splitSTO = Int16.Parse(rowTO.STONo.Split('-').Last()) + 1;
                if (splitSTO.ToString().Length == 1)
                {
                    STOx = STONo + "0" + splitSTO;
                }
                else
                {
                    STOx = STONo + splitSTO;
                }
                //STONo = rowTO.IDTransportOrder.ToString();
            }
            //Debug.WriteLine("STOx");
            return STOx;
        }*/
        public string getLastSTONo(DateTime shipmentDate, string minSTO = null)
        {
            string STONo = "ON" + shipmentDate.ToString("yyMM");
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(p => p.STONo.StartsWith(STONo));
            queryFilter = queryFilter.And(p => !String.IsNullOrEmpty(p.STONo));
            var rowTO = Get(queryFilter).OrderByDescending(p => p.STONo).FirstOrDefault();

            int serial = 1;
            if (rowTO != null)
            {
                var STONumberx = rowTO.STONo;
                serial = Int16.Parse(STONumberx.Substring(STONumberx.Length - 4)) + 1;
            }
            var latestSTO = minSTO != null ? int.Parse(minSTO.Substring(6)) : 0;
            if (serial <= latestSTO)
                serial = latestSTO + 1;
            //Debug.WriteLine("STOx");
            return STONo + serial.ToString().PadLeft(4, '0');
        }
        public TransportOrder GetTransportOrderByID(int idTO)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IDTransportOrder == idTO);
            return Get(queryFilter).FirstOrDefault();
        }
        public void DeleteDataList(List<int> idTO)
        {
            foreach (var idTOx in idTO)
            {
                var queryFilter = PredicateHelper.True<TransportOrder>();
                queryFilter = queryFilter.And(x => x.IDTransportOrder == idTOx);
                var dataTO = Get(queryFilter).ToList();
                foreach (var TO in dataTO)
                {
                    Delete(TO);
                }
            }
            Save();
        }
        public void DeleteData(int idTO)
        {
            /*var queryFilter = PredicateHelper.True<TransportOrder>();
                queryFilter = queryFilter.And(x => x.IDTransportOrder == idTOx);
                var dataTO = Get(queryFilter).ToList();
                foreach (var TO in dataTO)
                {*/
            Delete(idTO);
            //}
            Save();
        }
        public List<TransportOrderTransportExecutionPrintDTO> GetTransportOrderFilterByTESenderReceiver(int idTE, string sender, string receiver)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idTE);
            queryFilter = queryFilter.And(x => x.SenderIDLocation == sender);
            queryFilter = queryFilter.And(x => x.ReceiverIDLocation == receiver);
            return ReturnListObjectWithJoinView2(queryFilter);
        }
        public List<TransportOrder> GetTransportOrderFilterByTE(int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idTE);
            return Get(queryFilter).ToList();
        }
        public List<TransportOrder> GetIdTEFilterByTO(TransportOrderInput input)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            if (input.zoneFilter != "ALL")
            {
                queryFilter = queryFilter.And(x => x.ZoneBased == input.zoneFilter);
            }
            if (input.orderTypeListFilter != null)
            {
                queryFilter = queryFilter.And(x => input.orderTypeListFilter.Contains(x.OrderType));
            }
            if (input.dateFromFilter != null && input.dateToFilter != null)
            {
                queryFilter = queryFilter.And(x => x.ShipmentDate >= input.dateFromFilter && x.ShipmentDate <= input.dateToFilter);
            }
            if (input.senderIdLocFilter != null)
            {
                queryFilter = queryFilter.And(x => x.SenderIDLocation == input.senderIdLocFilter);
            }
            if (input.vehicleTypeFilter != "ALL")
            {
                queryFilter = queryFilter.And(x => x.VehicleType == input.vehicleTypeFilter);
            }
            return Get(queryFilter)
                .GroupBy(x => new { x.IDTransportExecution })
                .Select(x => x.FirstOrDefault()).ToList();
        }
        public List<TransportPickingListExportDTO> GetRawDataTODPL(TransportOrderInput input)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDTransportPickingListLog != null);
            if (input.stoNoListFilter != null && !String.IsNullOrEmpty(input.stoNoListFilter[0]))
            {
                var stoNoList = input.stoNoListFilter[0].Split(',');
                queryFilter = queryFilter.And(x => stoNoList.Contains(x.STONo));
            }
            if (input.zoneFilter != null)
            {
                queryFilter = queryFilter.And(x => x.ZoneBased == input.zoneFilter);
            }
            if (input.dateFromFilter != null && input.dateToFilter != null)
            {
                queryFilter = queryFilter.And(x => x.ShipmentDate >= input.dateFromFilter && x.ShipmentDate <= input.dateToFilter);
            }
            if (input.senderIdLocListFilter != null && !String.IsNullOrEmpty(input.senderIdLocListFilter[0]))
            {
                queryFilter = queryFilter.And(x => input.senderIdLocListFilter.Contains(x.SenderIDLocation));
            }
            if (input.receiverIdLocListFilter != null && !String.IsNullOrEmpty(input.receiverIdLocListFilter[0]))
            {
                queryFilter = queryFilter.And(x => input.receiverIdLocListFilter.Contains(x.ReceiverIDLocation));
            }
            if (input.vehicleTypeFilter != null)
            {
                queryFilter = queryFilter.And(x => x.VehicleType == input.vehicleTypeFilter);
            }
            if (!String.IsNullOrEmpty(input.uploadedByFilter))
            {
                queryFilter = queryFilter.And(x => input.uploadedByFilter.Contains(x.CreatedBy));
            }
            //return ReturnListObjectWithJoinView3(queryFilter);
            return ReturnListObjectWithJoinView3(queryFilter).OrderBy(x => x.SeqNo == null ? x.DefaultSeqNo : x.SeqNo != null ? x.SeqNo : "ZZZ").ThenBy(x => x.ActualReceiverName).ThenBy(x => x.ActualSenderName).ToList();
        }
        public List<TransportPickingListExportDTO> ReturnListObjectWithJoinView3(Expression<Func<TransportOrder, bool>> filter)
        {
            /*return Get(filter)
                .Join(_context.TransportOrderDetails, to => to.IDTransportOrder, tod => tod.IDTransportOrder, (to, tod) => new { to, tod })
                .Select(x => new TransportPickingListExportDTO(x.to, x.tod)).ToList();*/

            return Get(filter)
                .Join(_context.TransportOrderDetails, to => to.IDTransportOrder, tod => tod.IDTransportOrder, (to, tod) => new { to, tod })
                .Join(_context.MasterLocations, to2 => to2.to.SenderIDLocation, ml1 => ml1.IDLocation, (to2, ml1) => new { to2.to, to2.tod, ml1 })
                .Join(_context.MasterLocations, to3 => to3.to.ReceiverIDLocation, ml2 => ml2.IDLocation, (to3, ml2) => new { to3.to, to3.tod, to3.ml1, ml2 })
                .Join(_context.MasterLocations, to4 => to4.to.ActualSenderIDLocation, ml3 => ml3.IDLocation, (to4, ml3) => new { to4.to, to4.tod, to4.ml1, to4.ml2, ml3 })
                .Join(_context.MasterLocations, to5 => to5.to.ActualReceiverIDLocation, ml4 => ml4.IDLocation, (to5, ml4) => new { to5.to, to5.tod, to5.ml1, to5.ml2, to5.ml3, ml4 })
                .Select(x => new TransportPickingListExportDTO(x.to, x.tod, x.ml1, x.ml2, x.ml3, x.ml4)).ToList();
        }
        public List<TransportOrder> GetTransportOrderByidTE(int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportOrder>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idTE);
            return Get(queryFilter).OrderByDescending(x => x.STONo).ToList();
        }
        public bool SendMail(string subject, string body, IEnumerable<string> recipients, string format = "HTML", bool batch = false)
        {
            if (recipients != null)
                recipients = recipients.Distinct();
            
            // versi pake DB
            var ctx = new TOMContextDB();
            try
            {
                if (!batch)
                {
                    foreach (var email in recipients)
                    {
                        ctx.spSendEmail("TOM_Mail", email, null, body, format, subject, null);
                    }
                }
                else
                    ctx.spSendEmail("TOM_Mail", String.Join(";", recipients), null, body, format, subject, null);
                
                return true;
            }
            catch { }
            return false;

            // versi pake smtp
            try
            {
                var port = ConfigurationManager.AppSettings["SMTPMailPort"];
                var host = ConfigurationManager.AppSettings["SMTPMailHost"];
                var from = ConfigurationManager.AppSettings["MailFrom"];
                if (batch)
                {
                    MailMessage mail = new MailMessage(from, String.Join(";", recipients));
                    SmtpClient client = new SmtpClient();

                    int p = 0;
                    client.Port = int.TryParse( port, out p) ? p : 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = host;

                    mail.Subject = subject;
                    mail.Body = body;
                    client.Send(mail);
                }
                else
                {
                    foreach(var email in recipients)
                    {
                        MailMessage mail = new MailMessage(from, email);
                        SmtpClient client = new SmtpClient();

                        int p = 0;
                        client.Port = int.TryParse(port, out p) ? p : 25;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Host = host;

                        mail.Subject = subject;
                        mail.Body = body;
                        client.Send(mail);
                    }
                }
                return true;
            }
            catch { }
            return false;
        }

        public IEnumerable<DateTime> GetTransportDateEnumerable()
        {
            return _context.TransportExecutions.AsNoTracking().Select(c => new { tDate = c.TransportDate}).Distinct().Select(c => c.tDate).ToList();
        }

        public int updateOrderFromShip(TransportMonitoringViewInput input, string userid)
        {
            TOMContextDB dbContext = new TOMContextDB();
            var statuses = new string[] { "In Process", "On Delivery", "Arrive At Destination and Waiting For Confirmation", "Complete" };
            var transportationStatus = statuses[statuses.Length - 1];

            foreach (var rec in input.TransportOrderList)
            {
                var dbres = dbContext.TransportOrders.FirstOrDefault(x => x.IDTransportOrder == rec.IDTransportOrder && x.IsActive);

                if (dbres != null)
                {
                    dbres.OrderStatus = rec.OrderStatus;
                    dbres.GIBy = rec.GIDate != null ? dbres.GIDate != rec.GIDate ? userid : dbres.GIBy : null;
                    dbres.GRBy = rec.GRDate != null ? dbres.GRDate != rec.GRDate ? userid : dbres.GRBy : null;
                    dbres.POWeek = rec.POWeek;
                    dbres.GIDate = rec.GIDate;
                    dbres.GRDate = rec.GRDate;

                    dbres.UpdatedBy = userid;
                    dbres.UpdatedDate = DateTime.Now;

                    if (transportationStatus != rec.OrderStatus)
                    {
                        var comp1 = Array.IndexOf(statuses, transportationStatus);
                        var comp2 = Array.IndexOf(statuses, rec.OrderStatus);
                        if (comp1 > comp2) transportationStatus = rec.OrderStatus;
                    }
                }
                dbContext.SaveChanges();
            }

            var resEx = dbContext.TransportExecutions.FirstOrDefault(x => x.IDTransportExecution == input.IDTransportExecution && x.IsActive);
            resEx.TransportStatus = transportationStatus;
            dbContext.SaveChanges();

            return 1;
        }

        public bool CreateNewNotification(string pageName, string description, string toUser, string creatorUser, bool isSeen = false, bool isRead = false)
        {
            try
            {
                TOMContextDB dbContext = new TOMContextDB();

                var ns = dbContext.NotificationSystems.Create();
                ns.PageName = pageName;
                ns.Description = description;
                ns.IDUser = toUser;
                ns.CreatedBy = ns.UpdatedBy = creatorUser;
                ns.CreatedDate = ns.UpdatedDate = DateTime.Now;
                ns.IsOpen = isSeen;
                ns.IsRead = isRead;

                dbContext.NotificationSystems.Add(ns);
                dbContext.SaveChanges();
                return true;
            }
            catch { }
            return false;
        }

        public bool CreateNewNotification(string pageName, string description, string[] toUser, string creatorUser, bool isSeen = false, bool isRead = false)
        {
            bool x = true;
            foreach(var user in toUser)
            {
                x &= CreateNewNotification(pageName, description, user, creatorUser, isSeen, isRead);
            }

            return true;
        }
    }
}
