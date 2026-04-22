using System;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterLocationDTO
    {
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public string ParentLocation { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public bool? IsRegionalOffice { get; set; }
        public bool? IsAssigned { get; set; }
    }
}
