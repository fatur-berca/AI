using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportMonitoringUploadInput
    {
        public string OrderNumber { get; set; }

        public string  OrderStatus { get; set; }

        public DateTime? GIDate { get; set; }
        public DateTime? GRDate { get; set; }

        public string TransportationNumber { get; set; }
        public string UpdatePosition { get; set; }
        public DateTime? DateTime { get; set; }
    }


}
