using System;
using System.Collections.Generic;
using DFIS.Utils;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportOrderRequestInput : UtilBaseInput
    {
        public int IDRequest { get; set; }
        public string RequestNo { get; set; }
        public string VehicleType { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string Zone { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<TransportOrderInput> TransportOrders { get; set; }

        public bool IsRoleTransport { get; set; }
        public List<string> userRegionList { get; set; }

        public List<string> stoNoListFilter { get; set; }
        public List<string> zoneListFilter { get; set; }
        public List<string> orderTypeListFilter { get; set; }
        public List<string> orderVehicleTypeListFiter { get; set; }
        public DateTime? dateFromFilter { get; set; }
        public DateTime? dateToFilter { get; set; }
        public List<string> materialTypeListFilter { get; set; }
        public List<string> orderStatusListFilter { get; set; }
        public List<string> senderIdLocListFilter { get; set; }
        public List<string> receiverIdLocListFilter { get; set; }
        public List<string> userIdLocListFilter { get; set; }

        public string STONo { get; set; }
        public string OrderType { get; set; }
        public string OrderNumber { get; set; }
        public string SenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string CostCenter { get; set; }
        public string OrderStatus { get; set; }
        public string Remarks { get; set; }
        public string MaterialType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public string UnitID { get; set; }
        public int IDTransportOrder { get; set; }
        public string SeqID { get; set; }
    }
}
