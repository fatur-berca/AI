using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterServicePoTomViewModel 
    {
        public int IDServicePO { get; set; }
        public int IDVendor { get; set; }
        public string ServicePONumber { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

        //public virtual MasterVendorTOMViewModel MasterTransportVendor { get; set; }
        public virtual MasterVendor MasterTransportVendor { get; set; }
    }
}