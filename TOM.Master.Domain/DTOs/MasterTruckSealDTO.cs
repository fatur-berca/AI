using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.DTOs
{
    public class MasterTruckSealDTO
    {
        public int IDTruckSealStock { get; set; }
        public string SealNumberFrom { get; set; }
        public string SealNumberTo { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

        #region export transport seal
        public string Status { get; set; }
        public int? SealNumber { get; set; }
        public string SealNo { get; set; }
        public DateTime? UsedOn { get; set; }
        public string TN { get; set; }
        #endregion
    }
}
