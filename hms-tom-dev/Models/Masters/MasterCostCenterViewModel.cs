using System;
using System.Collections.Generic;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterCostCenterAccountViewModel : ViewModelBase
    {
        public int IDCostCenter { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
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

        public virtual MasterLocationViewModel MasterLocation { get; set; }
        public virtual MasterLocationViewModel MasterLocation1 { get; set; }
    }
}