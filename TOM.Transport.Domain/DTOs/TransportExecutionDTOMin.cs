using System;
using System.Collections.Generic;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportExecutionDTOMin
    {
        public int IDTransportExecution { get; set; }
        public string TransportNo { get; set; }
        public string TransportStatus { get; set; }
        public DateTime TransportDate { get; set; }
        public string TransportCategory { get; set; }
        public string ActualVehicleType { get; set; }
        public string TransportMode { get; set; }
        public int? IDVendor { get; set; }
        public byte? SIWeek { get; set; }
        public string IDStartLocation { get; set; }
        public decimal? KMBased { get; set; }
        public bool IsActive { get; set; }
        public string VendorName { get; set; }
        public string STONo { get; set; }
        public string ZoneBased { get; set; }
        public string OrderCategory { get; set; }
        public string IDSender { get; set; }
        public string IDReceiver { get; set; }
        public decimal? BasedPrice { get; set; }
        public string MapFrom { get; set; }
    }    
}
