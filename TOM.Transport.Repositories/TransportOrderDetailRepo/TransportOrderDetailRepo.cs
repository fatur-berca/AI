using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TOM.EntitiesDAL.EDMX;
using TOM.EntitiesDAL;
using DFIS.Utils;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public class TransportOrderDetailRepo : TOMGenericRepository<TransportOrderDetail>, ITransportOrderDetailRepo
    {
        private TOMContextDB _context;

        public TransportOrderDetailRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }

        public List<TransportOrderDetailDTO> ReturnListObjectWithJoinView(Expression<Func<TransportOrderDetail, bool>> filter)
        {
            return Get(filter)
                .Join(_context.MasterLocations, to => to.TransportOrder.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to2 => to2.to.TransportOrder.ReceiverIDLocation, ml2 => ml2.IDLocation, (to2, ml2) => new { to2.to, to2.ml, ml2 })
                .Join(_context.MasterLocations, to3 => to3.to.TransportOrder.ActualSenderIDLocation, ml3 => ml3.IDLocation, (to3, ml3) => new { to3.to, to3.ml, to3.ml2, ml3 })
                .Join(_context.MasterLocations, to4 => to4.to.TransportOrder.ActualReceiverIDLocation, ml4 => ml4.IDLocation, (to4, ml4) => new { to4.to, to4.to.TransportOrder, to4.ml, to4.ml2, to4.ml3, ml4 })
                .Select(x => new TransportOrderDetailDTO(x.to, x.TransportOrder, x.ml, x.ml2, x.ml3, x.ml4, x.to.TransportOrder.TransportExecution)).ToList();
        }

        //dipakai di transportation execution
        //karena berdasarkan detail, tapi filternya berdasarkan Transport Ordernya
        public List<TransportOrderDetailDTO> GetAllTransportOrderDetail(TransportOrderInput criteria)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetail>();
            queryFilter = queryFilter.And(x => x.IsActive);
            //if (!criteria.IsRoleTransport)
            //{
            //    if (criteria.userRegionList.Count > 0)
            //    {
            //        queryFilter = queryFilter.And(x => criteria.userRegionList.Contains(x.TransportOrder.MasterLocation3.ParentLocation) || criteria.userRegionList.Contains(x.TransportOrder.MasterLocation2.ParentLocation));
            //    }
            //}
            if(criteria.IDTransportOrder != null && criteria.IDTransportOrder > 0)
            {
                queryFilter = queryFilter.And(x => x.IDTransportOrder == criteria.IDTransportOrder);
            }
            /*if (criteria.dateFromFilter != null && criteria.dateToFilter != null)
            {
                queryFilter = queryFilter.And(x => x.TransportOrder.ShipmentDate >= criteria.dateFromFilter && x.TransportOrder.ShipmentDate <= criteria.dateToFilter);
            }*/
            /*if (criteria.zoneFilter != null)
            {
                queryFilter = queryFilter.And(x => x.TransportOrder.ZoneBased == criteria.zoneFilter);
            }*/
            if (criteria.stoNoListFilter != null && !String.IsNullOrEmpty(criteria.stoNoListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.stoNoListFilter.Contains(x.TransportOrder.STONo));
            }
            if (criteria.senderIdLocListFilter != null && !String.IsNullOrEmpty(criteria.senderIdLocListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.senderIdLocListFilter.Contains(x.TransportOrder.SenderIDLocation));
            }
            if (criteria.receiverIdLocListFilter != null && !String.IsNullOrEmpty(criteria.receiverIdLocListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.receiverIdLocListFilter.Contains(x.TransportOrder.ReceiverIDLocation));
            }
            if (criteria.orderTypeListFilter != null && !String.IsNullOrEmpty(criteria.orderTypeListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.orderTypeListFilter.Contains(x.TransportOrder.OrderType));
            }
            if (criteria.orderCategoryListFilter != null && !String.IsNullOrEmpty(criteria.orderCategoryListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.orderCategoryListFilter.Contains(x.TransportOrder.OrderCategory));
            }
            if (criteria.materialTypeListFilter != null && !String.IsNullOrEmpty(criteria.materialTypeListFilter[0]))
            {
                queryFilter = queryFilter.And(x => criteria.materialTypeListFilter.Contains(x.MaterialType));
            }
            if (!criteria.IsRoleTransport && criteria.userRegionList != null)
            {
                if (criteria.userRegionList.Count > 0)
                {
                    return Get(queryFilter)
                            .Join(_context.MasterLocations, to => to.TransportOrder.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                            .Join(_context.MasterLocations, to2 => to2.to.TransportOrder.ReceiverIDLocation, ml2 => ml2.IDLocation, (to2, ml2) => new { to2.to, to2.ml, ml2 })
                            .Join(_context.MasterLocations, to3 => to3.to.TransportOrder.ActualSenderIDLocation, ml3 => ml3.IDLocation, (to3, ml3) => new { to3.to, to3.ml, to3.ml2, ml3 })
                            .Join(_context.MasterLocations, to4 => to4.to.TransportOrder.ActualReceiverIDLocation, ml4 => ml4.IDLocation, (to4, ml4) => new { to4.to, to4.to.TransportOrder, to4.ml, to4.ml2, to4.ml3, ml4 })
                            .Where(x => criteria.userRegionList.Contains(x.ml.ParentLocation) || criteria.userRegionList.Contains(x.ml2.ParentLocation))
                            .Select(x => new TransportOrderDetailDTO(x.to, x.TransportOrder, x.ml, x.ml2, x.ml3, x.ml4, x.to.TransportOrder.TransportExecution))
                            .ToList();
                }
            }
            return ReturnListObjectWithJoinView(queryFilter);
        }

        public void SaveData(TransportOrderDetail input, bool status)
        {
            if (status)
            {                
                input.CreatedDate = DateTime.Now;                
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();            
        }
        /*public void SaveDataTODetail(List<TransportOrderDetail> listTODetail)
        {
            Debug.WriteLine("Count List TO:" + listTODetail.Count);
            for (int i = 0; i < listTODetail.Count; i++)
            {
                SaveData(listTODetail[i], true);                
            }
        }*/

        public void SaveDataTODetail(List<TransportOrderDetail> listTODetail)
        {
            //Debug.WriteLine("Count List TO Detail:" + listTODetail.Count);
            TOMContextDB context = null;
            try
            {
                context = new TOMContextDB();
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                int count = 0;
                foreach (var entityToInsert in listTODetail)
                {
                    ++count;
                    context = AddToContext(context, entityToInsert, count, 100, true);
                }
                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }           
        }

        private TOMContextDB AddToContext(TOMContextDB context, TransportOrderDetail entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<TransportOrderDetail>().Add(entity);
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
        public void InsertListData(List<TransportOrderDetail> listTODetail)
        {
            foreach (var saveData in listTODetail)
            {
                saveData.IsActive = true;
                saveData.CreatedDate = DateTime.Now;
                saveData.UpdatedDate = DateTime.Now;
                Insert(saveData);
            }
            Save();
        }
        
        public TransportOrderDetail GetDataByID(int idTOD)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetail>();
            queryFilter = queryFilter.And(x => x.IDTransportOrderDetail == idTOD);
            return Get(queryFilter).FirstOrDefault();
        }
        public void DeleteDataByListTO(List<int> listTODDelete)
        {
            foreach (var idTO in listTODDelete)
            {
                var queryFilter = PredicateHelper.True<TransportOrderDetail>();
                queryFilter = queryFilter.And(x => x.IDTransportOrder == idTO);
                var dataDetail = Get(queryFilter).ToList();
                foreach (var TOD in dataDetail)
                {
                    Delete(TOD);
                }
            }
            Save();
        }
        public void DeleteDataByTO(int idTO)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetail>();
            queryFilter = queryFilter.And(x => x.IDTransportOrder == idTO);
            var dataDetail = Get(queryFilter).ToList();
            foreach (var TOD in dataDetail)
            {
                Delete(TOD);
            }
            Save();
        }
        public void setInActiveByTO(int idTO)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetail>();
            queryFilter = queryFilter.And(x => x.IDTransportOrder == idTO);
            var dataDetail = Get(queryFilter).ToList();
            foreach (var TOD in dataDetail)
            {
                TransportOrderDetail rowTOD = GetDataByID(TOD.IDTransportOrderDetail);
                rowTOD.IsActive = false;
                SaveData(rowTOD, false);
            }
        }
        public List<TransportOrderDetailPrintDTO> GetTransportOrderDetailByidTO(int idTO)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetail>();
            queryFilter = queryFilter.And(x => x.IDTransportOrder == idTO);
            return ReturnListObjectWithJoinView2(queryFilter);
        }

        public List<TransportOrderDetailPrintDTO> GetTransportOrderDetailByidTE(int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetail>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportOrder.IDTransportExecution == idTE);            
            return ReturnListObjectWithJoinView2(queryFilter);
        }

        public List<TransportOrderDetailPrintDTO> ReturnListObjectWithJoinView2(Expression<Func<TransportOrderDetail, bool>> filter)
        {
            return Get(filter)
                .Join(_context.MasterLocations, to => to.TransportOrder.SenderIDLocation, ml => ml.IDLocation, (to, ml) => new { to, ml })
                .Join(_context.MasterLocations, to2 => to2.to.TransportOrder.ReceiverIDLocation, ml2 => ml2.IDLocation, (to2, ml2) => new { to2.to, to2.ml, ml2 })
                .Join(_context.MasterLocations, to3 => to3.to.TransportOrder.ActualSenderIDLocation, ml3 => ml3.IDLocation, (to3, ml3) => new { to3.to, to3.ml, to3.ml2, ml3 })
                .Join(_context.MasterLocations, to4 => to4.to.TransportOrder.ActualReceiverIDLocation, ml4 => ml4.IDLocation, (to4, ml4) => new { to4.to, to4.to.TransportOrder, to4.ml, to4.ml2, to4.ml3, ml4 })
                .Select(x => new TransportOrderDetailPrintDTO(x.to, x.TransportOrder, x.ml, x.ml2, x.ml3, x.ml4, x.to.TransportOrder.TransportExecution)).ToList();
        }

        public List<TransportOrderDetailPrintDTO> GetTransportOrderDetailBySenderReceiverTE(string sender, string receiver, int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetail>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportOrder.ActualSenderIDLocation == sender);
            queryFilter = queryFilter.And(x => x.TransportOrder.ActualReceiverIDLocation == receiver);
            queryFilter = queryFilter.And(x => x.TransportOrder.IDTransportExecution == idTE);
            return ReturnListObjectWithJoinView2(queryFilter);
        }

    }
}
