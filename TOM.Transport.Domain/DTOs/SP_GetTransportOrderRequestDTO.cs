using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class SP_GetTransportOrderRequestDTO
    {
        public string RequestNo { get; set; }
        public DateTime? RequestDate { get; set; }
        public Nullable<int> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> TotalVehicle { get; set; }
        public Nullable<int> TotalRoute { get; set; }
        public Nullable<int> TotalDraft { get; set; }
        public Nullable<int> TotalSubmit { get; set; }
        public Nullable<int> TotalInProcess { get; set; }
        public Nullable<int> TotalOnDelivery { get; set; }
        public Nullable<int> TotalArrive { get; set; }
        public Nullable<int> TotalCompleted { get; set; }
        public Nullable<int> TotalClose { get; set; }
        public Nullable<int> TotalCancel { get; set; }

        public bool Deleteable { get {
                return (TotalInProcess + TotalOnDelivery + TotalCompleted + TotalSubmit + TotalCancel + TotalArrive + TotalClose) <= 0;
            } }
    }
}

