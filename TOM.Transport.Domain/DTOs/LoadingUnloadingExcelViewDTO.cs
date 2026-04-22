using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class LoadingUnloadingExcelViewDTO
    {
        public Nullable<long> Row { get; set; }
        public string TransportNo { get; set; }
        public string STONo { get; set; }
        public System.DateTime TransportDate { get; set; }
        public string SenderIDLocation { get; set; }
        public string SenderLocationName { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ReceiverLocationName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public string UoM { get; set; }
        public Nullable<System.DateTime> LoadStartTime { get; set; }
        public Nullable<System.DateTime> LoadFinishTime { get; set; }
        public Nullable<System.DateTime> UnloadStartTime { get; set; }
        public Nullable<System.DateTime> UnloadFinishTime { get; set; }
        public Nullable<decimal> LoadBoxperWorkingTime { get; set; }
        public Nullable<decimal> UnloadBoxperWorkingTime { get; set; }
    }
}
