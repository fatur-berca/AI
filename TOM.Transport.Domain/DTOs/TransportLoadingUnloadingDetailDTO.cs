using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportLoadingUnloadingDetailDTO
    {
        public string GPNumber { get; set; }
        public string SJNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string StatusLoadUnload { get; set; }
        public System.DateTime PickInDate { get; set; }
        public System.DateTime LoadStartDate { get; set; }
        public System.DateTime LoadFinishDate { get; set; }
        public System.DateTime PickOutDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string WarehouseName { get; set; }
        public string PoliceRegNumber { get; set; }
        public string UnitType { get; set; }
        public System.DateTime Date { get; set; }
        public string Status { get; set; }
        public string IDVendor { get; set; }
         public string VendorName { get; set; }
         public int SealNumber { get; set; }
         public string Additionalinfo { get; set; }
         public string DestinationName { get; set; }


    }
}
