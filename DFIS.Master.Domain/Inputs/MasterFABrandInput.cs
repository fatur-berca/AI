using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Master.Domain.Inputs
{
    public class MasterFABrandInput : BaseInput
    {
        public string FACode { get; set; }
        public string SpeakingCode { get; set; }
        public string Type { get; set; }
        public decimal StickPerBox { get; set; }
        public decimal PackPerBox { get; set; }
        public decimal StickPerPack { get; set; }
        public int HJE { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}
