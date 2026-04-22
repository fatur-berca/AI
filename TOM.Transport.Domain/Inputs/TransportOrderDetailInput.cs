using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportOrderDetailInput : BaseInput
    {
        public int IDTransportOrderDetail { get; set; }
        public int IDTransportOrder { get; set; }
        public string MaterialType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Supplier { get; set; }
        
        public virtual ICollection<TransportOrderDetailChangeLogInput> TransportOrderDetailChangeLogs { get; set; }
    }
}
