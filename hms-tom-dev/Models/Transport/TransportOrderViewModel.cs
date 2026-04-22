using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Models.Transport
{
    public class TransportOrderViewModel : ViewModelBase
    {
        public int IDTransportOrder { get; set; }
        public int? IDRequest { get; set; }
        public int? IDTransportExecution { get; set; }
        public int? IDTransportPickingListLog { get; set; }
        public string STONo { get; set; }
        public string Supplier { get; set; }
        public string SupplierColor { get; set; }
        public string DefaultSeqNo { get; set; }
        public string SeqNo { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string ShipmentDateColor { get; set; }
        public decimal? KM { get; set; }
        public string SenderIDLocation { get; set; }
        public string SenderLocationColor { get; set; }
        public string SenderLocationName { get; set; }
        public string ActualSenderIDLocation { get; set; }
        public string ActualSenderName { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ReceiverLocationColor { get; set; }
        public string ReceiverLocationName { get; set; }
        public string ActualReceiverIDLocation { get; set; }
        public string ActualReceiverName { get; set; }
        public byte? LeadTime { get; set; }
        public string VehicleType { get; set; }
        public string OrderType { get; set; }
        public string CostCenter { get; set; }
        public string OrderStatus { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public DateTime? MaterialReceivedTime { get; set; }
        public bool? IsMaterialReceived { get; set; }
        public DateTime? EstArrivalDate { get; set; }
        public DateTime? GRDate { get; set; }
        public DateTime? GIDate { get; set; }
        public DateTime? LoadStartTime { get; set; }
        public DateTime? LoadFinishTime { get; set; }
        public DateTime? UnloadStartTime { get; set; }
        public DateTime? UnloadFinishTime { get; set; }
        public string Remarks { get; set; }
        public byte? FlagEmail { get; set; }
        public int? IDLeadTime { get; set; }
        public int? IDCostCenter { get; set; }
        public string ChangeLogFields { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByFullName { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string TransportNo { get; set; }
        public string ZoneBased { get; set; }
        public string OrderCategory { get; set; }
        public string IdLocation { get; set; }
        public string LocationName { get; set; }
        public int ExistingState { get; set; }
        public Nullable<decimal> LoadBoxperWorkingTime { get; set; }
        public Nullable<decimal> UnloadBoxperWorkingTime { get; set; }

        public virtual MasterLocationViewModel MasterLocation { get; set; }
        public virtual MasterLocationViewModel MasterLocation1 { get; set; }
        public virtual MasterLocationViewModel MasterLocation2 { get; set; }
        public virtual MasterLocationViewModel MasterLocation3 { get; set; }
        public virtual List<TransportOrderDetailViewModel> TransportOrderDetails { get; set; }
        public virtual ICollection<TransportOrderChangeLogViewModel> TransportOrderChangeLogs { get; set; }
        public virtual TransportExecutionViewModel TransportExecution { get; set; }
        public virtual TransportPickingListLogViewModel TransportPickingListLogLog { get; set; }
        public virtual TransportOrderRequestViewModel TransportOrderRequest { get; set; }


        public virtual IEnumerable<SelectListItem> VehicleTypeList { get; set; }
        public virtual IEnumerable<SelectListItem> UoMList { get; set; }
        public virtual IEnumerable<SelectListItem> SupplierList { get; set; }

        public string Zone { get; set; }
        public int? weekNow { get; set; }
        public int yearNow { get; set; }
        public virtual List<SelectListItem> zoneList { get; set; }
        public virtual List<SelectListItem> senderList { get; set; }
        public virtual List<SelectListItem> receiveList { get; set; }
        public List<SelectListItem> UploadedBy { get; set; }
        public List<PickingListTotalUnit> pickingListTotalUnit { get; set; }

        public string weekDayNameShipmentDateEnglish
        {
            get
            {
                var dateTimeFormats = new CultureInfo("en-US").DateTimeFormat;
                return ShipmentDate.ToString("dddd", dateTimeFormats);
            }
        }

        public string weekDayNameShipmentDateIndonesia
        {
            get
            {
                var dateTimeFormats = new CultureInfo("id").DateTimeFormat;
                return ShipmentDate.ToString("dddd", dateTimeFormats);
            }
        }
    }

    public class PickingListTotalUnit
    {
        public string SenderIDLocation { get; set; }
        public int Total { get; set; }
    }
}