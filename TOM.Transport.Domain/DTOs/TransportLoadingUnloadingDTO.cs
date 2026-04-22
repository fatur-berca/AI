using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportLoadingUnloadingDTO
    {
        public string GPNumber { get; set; }
        public string PoliceRegNumber { get; set; }
        public string UnitType { get; set; }
        public string IDVendor { get; set; }
        public string Additionalinfo { get; set; }
        public string Status { get; set; }
        public System.DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}
