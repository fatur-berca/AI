using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.Inputs
{
    public class MasterCostCenterAccountInput : BaseInput
    {
        public int IDCostCenter { get; set; }
        public string SenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string MaterialType { get; set; }
        public string Description { get; set; }
        public string CostCenter { get; set; }
        public string Account { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public DateTime? filterShipmentDate { get; set; }
        public string filterDate { get; set; }
        public List<string> filterSenderLocation { get; set; }
    }
}
