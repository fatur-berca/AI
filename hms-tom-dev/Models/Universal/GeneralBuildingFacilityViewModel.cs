using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class GeneralBuildingFacilityViewModel : ViewModelBase
    {
        public string IDLocation { get; set; }
        public string WarehouseAddress { get; set; }       
    }
}