using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportExecutionImportInput
    {
        public string TransportNo { get; set; }
        public string SIType { get; set; }
        public DateTime? TargetOfArrival { get; set; }
        public string SIStatus { get; set; }
        public string NewVendorName { get; set; }
        public string NewSIType { get; set; }
        public DateTime? NewTargetOfArrival { get; set; }
        public DateTime? NewTransportDate { get; set; }
        public string Via { get; set; }


        public string VesselName { get; set; }
        public string ContainerNumber { get; set; }
        public string SealNumber { get; set; }
        public DateTime? ActualArrive { get; set; }
        public string ServicePONumber { get; set; }
        public string ServiceGRNumber { get; set; }
        public string PoliceRegNumber { get; set; }
        public string Driver1 { get; set; }
        public string Driver2 { get; set; }
        public string CoDriver { get; set; }

        /// <summary>
        /// Gets or sets the data type, either SI or TN
        /// </summary>
        public string DataType { get; set; }
    }
}
