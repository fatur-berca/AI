using System;
using DFIS.Utils;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportPickingListLogInput : UtilBaseInput
    {
        public int IDTransportPickingListLog { get; set; }
        public DateTime ShipmentDate { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }        
               
        public System.DateTime DocDate { get; set; }    
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string uploadedBy { get; set; }
    }
}
