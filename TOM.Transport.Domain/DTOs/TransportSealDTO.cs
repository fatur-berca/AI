using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportSealDTO
    {
        public int IDTransportSeal { get; set; }
        public int IDTransportExecution { get; set; }
        public string SealNumber { get; set; }
        public string SealActivity { get; set; }
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public System.DateTime SealTime { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByFullName { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string TN { get; set; }

        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual TransportExecutionDTO TransportExecution { get; set; }
    }
}
