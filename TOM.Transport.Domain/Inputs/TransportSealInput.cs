using System;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
namespace TOM.Transport.Domain.Inputs
{
    public class TransportSealInput : BaseInput
    {
        public int IDTransportSeal { get; set; }
        public int IDTransportExecution { get; set; }
        public string SealNumber { get; set; }
        public string SealActivity { get; set; }
        public string IDLocation { get; set; }
        public System.DateTime SealTime { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public string TransportNo { get; set; }

        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual TransportExecutionDTO TransportExecution { get; set; }
    }
}
