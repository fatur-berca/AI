using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DFIS.Utils;

namespace hms_tom_dev.Models.Transport
{
    public class TransportOrderRequestViewModel
    {
        public int IDRequest { get; set; }
        public string RequestNo { get; set; }
        public string VehicleType { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string Zone { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string STONo { get; set; }

        public virtual ICollection<TransportOrderViewModel> TransportOrders { get; set; }

        public bool InProcess
        {
            get
            {
                if (TransportOrders == null) return false;
                if (TransportOrders.Any(x => x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.Draft)))
                    // only draft is considered not in process
                //|| x.OrderStatus == EnumHelper.GetDescription(Enums.TransportationStatus.Submit)))
                    return false;
                return true;
            }
        }
        public int TotalUnit { get; set; }

        public bool IsActive { get; set; }

        #region bagiandefaultvalue
        public virtual List<SelectListItem> zone { get; set; }
        public virtual List<SelectListItem> orderType { get; set; }
        public virtual List<SelectListItem> vehicleType { get; set; }
        public virtual List<SelectListItem> materialType { get; set; }
        public virtual List<SelectListItem> orderStatus { get; set; }
        public virtual List<SelectListItem> location { get; set; }
        #endregion
    }
}