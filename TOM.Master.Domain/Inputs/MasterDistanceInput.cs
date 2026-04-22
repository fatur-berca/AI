using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.Inputs
{
    public class MasterDistanceInput : BaseInput
    {
        public int IDDistance { get; set; }
        public string DistanceType { get; set; }
        public string IDSender { get; set; }
        public string IDReceiver { get; set; }
        public string TransportationMode { get; set; }
        public decimal Distance { get; set; }
        public int Buffer { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Through { get; set; }
        public string Via { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public string filterDate { get; set; }
        public List<string> filterSenderLocation { get; set; }
    }
}
