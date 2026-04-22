using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportRouteDTO
    {
        public int IDTransportRoute { get; set; }
        public int IDTransportExecution { get; set; }
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public bool IsMain { get; set; }
        public bool IsAssigned { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool? IsNonKMBased { get; set; }

        public string OrderCategory
        {
            get
            {
                if (IsMain)
                    return "Main Order";
                return "Additional Order";
            }
        }
    }

    public class TransportRouteDistance
    {
        public TransportRouteDistance(string type, decimal km)
        {
            DistanceType = type;
            KM = km;
        }

        public string DistanceType { get; set; }
        public decimal KM { get; set; }
    }
}
