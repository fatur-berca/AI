using System;
using System.Collections.Generic;
using hms_tom_dev.Models.Common;
using System.Web.Mvc;

namespace hms_tom_dev.Models.Transport
{
    public class TransportOrderDetailPrintViewModel : ViewModelBase
    {
        public int IDTransportOrderDetail { get; set; }
        public int IDTransportOrder { get; set; }
        public int? IDTransportExecution { get; set; }
        public string MaterialType { get; set; }
        public string MaterialTypeColor { get; set; }
        public string Code { get; set; }
        public string CodeColor { get; set; }
        public string Description { get; set; }
        public string DescriptionColor { get; set; }
        public decimal? Qty { get; set; }
        public string QtyColor { get; set; }
        public string UoM { get; set; }
        public string UoMColor { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Supplier { get; set; }
        public string STONo { get; set; }
        public string SenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public int POWeek { get; set; }
        public string Remarks { get; set; }
        public virtual TransportOrderTransportExecutionViewModel TransportOrder { get; set; }        
    }
}