using System;

namespace TOM.Master.Domain.DTOs
{
    public class MasterKilometerDTO
    {
        public int IDMasterKilometer { get; set; }
        public string IDLocationFrom { get; set; }
        public string LocationFrom { get; set; }
        public string LongitudeFrom { get; set; }
        public string LatitudeFrom { get; set; }
        public string LocationTo { get; set; }
        public string IDLocationTo { get; set; }
        public string LongitudeTo { get; set; }
        public string LatitudeTo { get; set; }
        public int Kilometer { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}
