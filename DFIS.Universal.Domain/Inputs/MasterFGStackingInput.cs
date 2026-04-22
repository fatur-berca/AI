using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.Inputs
{
    public class MasterFGStackingInput : BaseInput
    {
        public int IDFGStacking { get; set; }
        public string IDLocation { get; set; }
        public string Brand { get; set; }
        public int MaxStacking { get; set; }
        public int MinStacking { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> DoubleStacking { get; set; }
    }
}
