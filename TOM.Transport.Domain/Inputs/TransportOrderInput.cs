using System;
using System.Collections.Generic;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportOrderInput : BaseInput
    {
        public int IDTransportOrder { get; set; }
        public int? IDRequest { get; set; }
        public int? IDTransportExecution { get; set; }
        public int? IDTransportPickingListLog { get; set; }
        public string STONo { get; set; }
        public string Supplier { get; set; }
        public string DefaultSeqNo { get; set; }
        public string SeqNo { get; set; }
        public DateTime ShipmentDate { get; set; }
        public decimal? KM { get; set; }
        public string SenderIDLocation { get; set; }
        public string ActualSenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ActualReceiverIDLocation { get; set; }
        public byte? LeadTime { get; set; }
        public string VehicleType { get; set; }
        public string OrderType { get; set; }
        public string CostCenter { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? MaterialReceivedTime { get; set; }
        public bool? IsMaterialReceived { get; set; }
        public DateTime? EstArrivalDate { get; set; }
        public DateTime? GRDate { get; set; }
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
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ZoneBased { get; set; }
        public Nullable<decimal> LoadBoxperWorkingTime { get; set; }
        public Nullable<decimal> UnloadBoxperWorkingTime { get; set; }

        public virtual MasterLocationInput MasterLocation { get; set; }
        public virtual MasterLocationInput MasterLocation1 { get; set; }
        public virtual ICollection<TransportOrderDetailInput> TransportOrderDetails { get; set; }
        public virtual ICollection<TransportOrderChangeLogInput> TransportOrderChangeLogs { get; set; }

        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }

        public int weekFilter { get; set; }
        public int yearFilter { get; set; }
        public DateTime? dateFromFilter { get; set; }
        public DateTime? dateToFilter { get; set; }
        public string zoneFilter { get; set; }
        public List<string> stoNoListFilter { get; set; }
        public List<string> senderIdLocListFilter { get; set; }
        public List<string> receiverIdLocListFilter { get; set; }
        public List<string> transportStatusListFilter { get; set; }
        public List<string> transportModeListFilter { get; set; }
        public string uploadedByFilter { get; set; }

        public bool IsRoleTransport { get; set; }
        public List<string> userRegionList { get; set; }
        public List<string> userLocationList { get; set; }

        //TAMBAHAN FILTER UNTUK TRANSPORTATION EXECUTION
        //ada di tabel transport order
        public List<string> orderTypeListFilter { get; set; }
        public List<string> senderListFilter { get; set; }
        //ada di tabel transport order
        //ada di tabel transport order detail
        public List<string> materialTypeListFilter { get; set; }
        //ada di tabel transport order detail
        //ada di tabel transport execution
        public List<string> transportNumberListFilter { get; set; }
        public List<string> orderCategoryListFilter { get; set; }
        public List<string> siStatusListFilter { get; set; }
        public List<string> startLocationListFilter { get; set; }
        public List<int> idTransportExecutionList { get; set; }
        //ada di tabel transport execution
        //TAMBAHAN FILTER UNTUK TRANSPORTATION EXECUTION

        //TAMBAHAN FILTER UNTUK TRANSPORTATION EXECUTION ADD NEW
        public string senderIdLocFilter { get; set; }
        public string vehicleTypeFilter { get; set; }
        public string[] vehicleTypeFilterA { get; set; }
        public string executionTypeFilter { get; set; }
        //TAMBAHAN FILTER UNTUK TRANSPORTATION EXECUTION ADD NEW

        public string DayName { get; set; }

        //FILTER TRANSPORT ROLE PER VENDOR
        public string UserRole { get; set; }
    }


    public class TransportOrderSTOCheckInput
    {
        public string Tag { get; set; }
        public string STONo { get; set; }
        public int IDTransportOrder { get; set; }
    }
}
