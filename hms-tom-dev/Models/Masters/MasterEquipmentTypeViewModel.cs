using System;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterEquipmentTypeViewModel : ViewModelBase
    {
        public int IDEquipmentType { get; set; }
        public string IDLocation { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public virtual MasterLocationViewModel MasterLocation { get; set; }
    }
}