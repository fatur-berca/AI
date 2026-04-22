using DFIS.Universal.Domain.Inputs;
using System;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportOrderChangeLogInput : BaseInput
    {
        public int IDTransportOrderChangeLog { get; set; }
        public int IDTransportOrder { get; set; }
        public int? Version { get; set; }
        public string OldValue { get; set; }
        public string ModifiedField { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
