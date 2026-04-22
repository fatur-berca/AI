using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public interface ITransportOrderRequestListViewRepo
    {
        IEnumerable<TransportOrderRequestSummaryDTO> Get(TransportOrderRequestInput filter);
    }
    public class TransportOrderRequestListViewRepo : ITransportOrderRequestListViewRepo
    {
        protected TOMContextDB Context { get; private set; }
        public TransportOrderRequestListViewRepo()
        {
            Context = new TOMContextDB();
        }

        public IEnumerable<TransportOrderRequestSummaryDTO> Get(TransportOrderRequestInput filter)
        {
            var stono = filter.stoNoListFilter == null ? new string[0] : filter.stoNoListFilter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            var zone = filter.zoneListFilter == null ? new string[0] : filter.zoneListFilter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            var vehiType = filter.orderVehicleTypeListFiter == null ? new string[0] : filter.orderVehicleTypeListFiter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            var mateType = filter.materialTypeListFilter == null ? new string[0] : filter.materialTypeListFilter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            var orderType = filter.orderTypeListFilter == null ? new string[0] : filter.orderTypeListFilter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            var sender = filter.senderIdLocListFilter == null ? new string[0] : filter.senderIdLocListFilter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            var receiver = filter.receiverIdLocListFilter == null ? new string[0] : filter.receiverIdLocListFilter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            var status = filter.orderStatusListFilter == null ? new string[0] : filter.orderStatusListFilter.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            
            var toe = Context.TransportOrderRequestSummaries.Where(o =>
            (stono.Contains(o.STONo) || stono.Count() == 0)
            && (zone.Contains(o.Zone) || zone.Count() == 0)
            && (vehiType.Contains(o.VehicleType) || vehiType.Count() == 0)
            && (mateType.Contains(o.MaterialType) || mateType.Count() == 0)
            && (orderType.Contains(o.OrderType) || orderType.Count() == 0)
            && (sender.Contains(o.SenderIDLocation) || sender.Count() == 0)
            && (receiver.Contains(o.ReceiverIDLocation) || receiver.Count() == 0)
            && (status.Contains(o.OrderStatus) || status.Count() == 0)
            && (filter.dateFromFilter == null || filter.dateFromFilter <= o.ShipmentDate)
            && (filter.dateToFilter == null || filter.dateToFilter >= o.ShipmentDate)
            );

            var qry = toe.ToList();
            var temp =AutoMapper.Mapper.Map<List<TransportOrderRequestSummaryDTO>>(qry);

            return temp;
        }
    }
}
