using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hms_tom_dev.Models.Masters
{
    public class MasterVendorSuggestionViewModel
    {
        public int IDVendorSuggestion { get; set; }
        [Key, Column(Order = 2), ForeignKey("MasterLocation")]
        public string StartLocation { get; set; }
        [Key, Column(Order = 2), ForeignKey("MasterLocation1")]
        public string ReceiverIDLocation { get; set; }
        public string OrderType { get; set; }
        public string TransportationCategory { get; set; }
        public string TransportationMode { get; set; }
        public string VehicleType { get; set; }
        public int SuggestedVendor { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public virtual MasterLocation MasterLocation { get; set; }
        public virtual MasterLocation MasterLocation1 { get; set; }
        public virtual MasterVendor MasterTransportVendor { get; set; }
    }
}