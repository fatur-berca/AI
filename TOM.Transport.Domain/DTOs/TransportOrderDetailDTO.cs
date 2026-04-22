using System;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportOrderDetailDTO
    {
        public TransportOrderDetailDTO() { }
        public TransportOrderDetailDTO(TransportOrderDetail input, TransportOrder to, MasterLocation senderLoc, MasterLocation receiveLoc, MasterLocation actualSenderLoc, MasterLocation actualReceiveLoc, TransportExecution transexe)
        {
            IDTransportOrderDetail = input.IDTransportOrderDetail;
            IDTransportOrder = input.IDTransportOrder;
            MaterialType = input.MaterialType;
            Code = input.Code;
            Description = input.Description;
            Qty = input.Qty;
            UoM = input.UoM;
            IsActive = IsActive;
            CreatedBy = input.CreatedBy;
            CreatedDate = input.CreatedDate;
            UpdatedBy = input.UpdatedBy;
            UpdatedDate = input.UpdatedDate;
            Supplier = input.Supplier;
            TransportOrder = new TransportOrderTransportExecutionDTO(to, senderLoc, receiveLoc, actualSenderLoc, actualReceiveLoc, transexe);
        }

        public int IDTransportOrderDetail { get; set; }
        public int IDTransportOrder { get; set; }
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
        public string SupplierColor { get; set; }
        public decimal? PackToBox { get; set; }

        public virtual TransportOrderTransportExecutionDTO TransportOrder { get; set; }
        public virtual ICollection<TransportOrderDetailChangeLogDTO> TransportOrderDetailChangeLogs { get; set; }

        public object Tag { get; set; }
        public object _OldRecord { get; set; }
    }
}
