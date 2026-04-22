using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace hms_tom_dev.Models.Masters
{
    public class MasterUserLocationMappingViewModel : ViewModelBase
    {
        public int IDUserLocationMapping { get; set; }
        public string IDUser { get; set; }
        public string IDLocation { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public List<string> ListIDLocation { get; set; }
        public string fullName { get; set; }

        public virtual MasterLocationViewModel MasterLocation { get; set; }
        public string MasterRole { get; set; }
        public virtual MasteUserViewModel MasterUser { get; set; }
    }
}