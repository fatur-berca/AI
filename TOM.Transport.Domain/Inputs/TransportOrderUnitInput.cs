using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportOrderRequestData
    {
        public List<TransportOrderUnitInput> Units { get; set; }
        public string RequestNo { get; set; }
    }

    public class TransportOrderUnitInput
    {
        public string VehicleType { get; set; }
        public string Zone { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string RequestNo { get; set; }
        public int IDRequest { get; set; }

        public List<TransportOrderRouteInput> Routes { get; set; }
    }

    public class TransportOrderRouteInput
    {
        public bool Checked { get; set; }
        public bool WantRefresh { get; set; }
        public string CostCenter { get; set; }
        public bool Expanded { get; set; }
        public string OrderNumber { get; set; }
        public string OrderRemark { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public string ParentOrderNumber { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public string STONo { get { return OrderNumber; } }
        public string SeqNo { get; set; }
        public int? IDCostCenter { get; set; }
        public bool Saved { get; set; }
        public int IDTransportOrder { get; set; }
        public bool IsCigarette { get { return Materials != null ? Materials.Where(m => m.MaterialType == "Cigarette").Any() : false; } }

        public string ReceiverIDLocation { get { return Receiver; } }
        public string ActualReceiverIDLocation { get { return Receiver; } }
        public string SenderIDLocation { get { return Sender; } }
        public string ActualSenderIDLocation { get { return Sender; } }

        public List<TransportOrderMaterialInput> Materials { get; set; }
    }

    public class TransportOrderMaterialInput
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string MaterialType { get; set; }
        public decimal Quantity { get; set; }
        public string QtyString {
            get {
                return Quantity.ToString();
            }
            set {
                Quantity = decimal.Parse(value != null ? value.Replace("M","") : value) / 100;
            }
        }
        public int IDTransportOrderDetail { get; set; }
        public decimal? Qty { get { return (decimal)Quantity; } }
        public string UoM { get; set; }
    }

    public class TransportOrderUploadInput
    {
        public int Vehicle { get; set; }
        public string Zone { get; set; }
        public string VehicleType { get; set; }
        public DateTime ShippingDate { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string OrderRemark { get; set; }
        public string OrderStatus { get; set; }
        public string MaterialType { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialDescription { get; set; }
        public double Quantity { get; set; }
        public string UoM { get; set; }
    }
}
