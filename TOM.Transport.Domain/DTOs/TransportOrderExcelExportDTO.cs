using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    /*
            defMap.Add("RequestNo", "Request Number");
            defMap.Add("Zone", "Zone");
            defMap.Add("VehicleType", "Vehicle Type");
            defMap.Add("ShipmentDate", "Shipment Date");
            defMap.Add("OrderNo", "Order No");
            defMap.Add("Sender", "Sender");
            defMap.Add("Receiver", "Receiver");
            defMap.Add("OrderType", "Order Type");
            defMap.Add("OrderStatus", "Order Status");
            defMap.Add("OrderRemark", "Order Remark");
            defMap.Add("MaterialType", "Material Type");
            defMap.Add("Code", "Material Code");
            defMap.Add("Qty", "Quantity");
            defMap.Add("UoM", "UoM");
            defMap.Add("CreatedBy", "Order Creator");
            defMap.Add("CreatedDate", "Request Date");
     */
    public class TransportOrderExcelExportDTO
    {
        public string SequenceNumber { get; set; }
        public int TotalVehicle { get; set; }
        public string RequestNo { get; set; }
        public string Zone { get; set; }
        public string VehicleType { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string OrderNo { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderRemark { get; set; }
        public string MaterialType { get; set; }
        public string Code { get; set; }
        public decimal Qty { get; set; }
        public string UoM { get; set; }
        public string CostCenter { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string MaterialDescription { get; set; }
    }
}
