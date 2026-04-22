using System;

namespace hms_tom_dev.Models.Masters
{
    public class MasterSupportWarehouseViewModel
    {
        public int IDSupportWarehouse { get; set; }
        public string SourceWarehouse { get; set; }
        public string SupportWarehouse { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}