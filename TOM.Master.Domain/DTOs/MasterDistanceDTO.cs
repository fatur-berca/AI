using TOM.Master.Domain.DTOs;
using DFIS.Universal.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.DTOs
{
    public class MasterDistanceDTO
    {
        public int IDDistance { get; set; }
        public string DistanceType { get; set; }
        public string IDSender { get; set; }
        public string IDReceiver { get; set; }
        public string TransportationMode { get; set; }
        public decimal Distance { get; set; }
        public int Buffer { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Through { get; set; }
        public string Via { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string ThroughName { get; set; }
        public string SenderLocationName { get; set; }
        public string ReceiverLocationName { get; set; }


        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation1 { get; set; }
        public virtual MasterLocationDTO MasterLocation2 { get; set; }
    }
}
