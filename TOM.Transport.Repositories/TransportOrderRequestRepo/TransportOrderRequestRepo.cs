using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Inputs;
using DFIS.Utils;
using DFIS.Contracts;

namespace TOM.Transport.Repositories.TransportOrderRequestRepo
{
    public class TransportOrderRequestRepo : TOMGenericRepository<TransportOrderRequest>, ITransportOrderRequestRepo
    {
        public TransportOrderRequestRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public TransportOrderRequest GetTransportOrderRequestByID(int idreq)
        {
            var queryFilter = PredicateHelper.True<TransportOrderRequest>();
            queryFilter = queryFilter.And(x => x.IDRequest == idreq);
            return Get(queryFilter).FirstOrDefault();
        }
        public int GetIDRequestByReqNo(string reqNo)
        {
            var queryFilter = PredicateHelper.True<TransportOrderRequest>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.RequestNo.Contains(reqNo));
            var orderRequest = Get(queryFilter).FirstOrDefault();
            return orderRequest.IDRequest;
        }
        public List<TransportOrderRequest> GetTransportOrderRequest(TransportOrderRequestInput input)
        {
            var queryFilter = PredicateHelper.True<TransportOrderRequest>();
            queryFilter = queryFilter.And(x => x.IsActive);
            if (!input.IsRoleTransport)
            {
                /*
                if (input.userRegionList.Count > 0)
                {
                    queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.userRegionList.Contains(y.ParentLocation) || input.userRegionList.Contains(y.MasterLocation2.ParentLocation)));
                }
                */
            }
            if (input.stoNoListFilter != null && input.stoNoListFilter.Count > 1)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.stoNoListFilter.Contains(y.STONo)));
            }
            if (input.zoneListFilter != null && input.zoneListFilter.Count > 1)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.zoneListFilter.Contains(y.ZoneBased)));
            }
            if (input.orderTypeListFilter != null && input.orderTypeListFilter.Count > 1)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.orderTypeListFilter.Contains(y.OrderType)));
            }
            if (input.orderVehicleTypeListFiter != null && input.orderVehicleTypeListFiter.Count > 1)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.orderVehicleTypeListFiter.Contains(y.VehicleType)));
            }
            /*
            if (input.materialTypeListFilter != null && input.materialTypeListFilter.Count > 0)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.materialTypeListFilter.Contains(y)));
            }
            \*/
            if (input.orderStatusListFilter != null && input.orderStatusListFilter.Count > 1)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.orderStatusListFilter.Contains(y.OrderStatus)));
            }
            if (input.senderIdLocListFilter != null && input.senderIdLocListFilter.Count > 1)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.senderIdLocListFilter.Contains(y.SenderIDLocation)));
            }
            if (input.receiverIdLocListFilter != null && input.receiverIdLocListFilter.Count > 1)
            {
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => input.senderIdLocListFilter.Contains(y.ReceiverIDLocation)));
            }
            if (input.dateFromFilter != null && input.dateToFilter != null)
                queryFilter = queryFilter.And(x => x.TransportOrders.Any(y => y.ShipmentDate >= input.dateFromFilter && y.ShipmentDate <= input.dateToFilter)); 
            return Get(queryFilter).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public int SaveData(TransportOrderRequest input)
        {
            input.UpdatedDate = DateTime.Now;
            Update(input);
            Save();
            return 0;
        }

        public string getLastRequestNumber(DateTime shipmentDate)
        {
            string ReqNumber = "";
            var promiseDate = shipmentDate.Date.ToString("yy-MM");
            var queryFilter = PredicateHelper.True<TransportOrderRequest>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.ShipmentDate.ToString().Contains(promiseDate));
            queryFilter = queryFilter.And(p => !String.IsNullOrEmpty(p.RequestNo));
            var rowTOR = Get(queryFilter).OrderByDescending(p => p.IDRequest).FirstOrDefault();
            string RQ = "RQ" + DateTime.Now.ToString("MMyy");
            if (rowTOR == null)
            {
                ReqNumber = RQ + "0001";
            }
            else
            {
                var reqNumberx = rowTOR.RequestNo;
                var strReq = Int16.Parse(reqNumberx.Substring(reqNumberx.Length - 4)) + 1;
                var lengthRQ = strReq.ToString().Length;
                if (lengthRQ == 1)
                {
                    ReqNumber = RQ + "000" + strReq;
                }
                else if (lengthRQ == 2)
                {
                    ReqNumber = RQ + "00" + strReq;
                }
                else if (lengthRQ == 3)
                {
                    ReqNumber = RQ + "0" + strReq;
                }
                else
                {
                    ReqNumber = RQ + strReq;
                }

            }
            return ReqNumber;
        }
        public string getNewRequestNumber(DateTime shipmentDate)
        {
            string ReqNumber = "";
            string RQ = "RQ" + shipmentDate.ToString("yyMM");
            var queryFilter = PredicateHelper.True<TransportOrderRequest>();
            queryFilter = queryFilter.And(p => p.RequestNo.StartsWith(RQ));
            var rowTOR = Get(queryFilter).OrderByDescending(p => p.RequestNo).FirstOrDefault();
            
            if (rowTOR == null)
            {
                ReqNumber = RQ + "0001";
            }
            else
            {
                var reqNumberx = rowTOR.RequestNo;
                var newNum = int.Parse(reqNumberx.Substring(RQ.Length)) + 1;
                ReqNumber = RQ + newNum.ToString().PadLeft(4, '0');
            }
            return ReqNumber;
        }

        public int InsertData(TransportOrderRequest input)
        {
            input.IsActive = true;
            input.CreatedDate = DateTime.Now;
            input.UpdatedDate = DateTime.Now;
            Insert(input);
            Save();
            return input.IDRequest;
        }
    }
}
