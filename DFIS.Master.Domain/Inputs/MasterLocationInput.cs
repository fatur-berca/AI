using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Master.Domain.Inputs
{
    public class MasterLocationInput : BaseInput
    {
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public string ParentLocation { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsRegionalOffice { get; set; }
        public string ParentLocations { get; set; }
    }
}
