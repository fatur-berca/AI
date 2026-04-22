using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Models.Transport
{
    public class TransportTicketNCRViewModel: ViewModelBase
    {
        public string TicketNumber { get; set; }
        public string TicketCategory { get; set; }
        public Nullable<int> TicketQuantity { get; set; }
        public string Location { get; set; }
        public int IDVendor { get; set; }
        public string PoliceRegNumber { get; set; }
        public string OffenderName { get; set; }
        public string OffenderRole { get; set; }
        public Nullable<int> OffenderAge { get; set; }
        public string RemarksTicket { get; set; }
        public string Attachment { get; set; }
        public string VehicleType { get; set; }
        public Nullable<int> ManufacturingYear { get; set; }
        public string OffenderYearOfService { get; set; }
        public string LocationCategory { get; set; }
        public string TypeOfLocation { get; set; }
        public string RoadCondition { get; set; }
        public Nullable<bool> Preventablility { get; set; }
        public string WeatherCondition { get; set; }
        public string AccidentCategory { get; set; }
        public Nullable<decimal> ValueOfLoss { get; set; }
        public string CorrectiveAction { get; set; }
        public Nullable<System.DateTime> CorrectiveActionDate { get; set; }
        public string TicketStatus { get; set; }
        public string SIRSNumber { get; set; }
        public string NCRNumber { get; set; }
        public string AttachmentNRC { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string strCorrectiveActionDate { get; set; }
        public List<string> SetDelete { get; set; }

        public string ProblemIssue1 { get; set; }
        public string ProblemIssue2 { get; set; }
        public string ProblemIssue3 { get; set; }
        public string MainFactor { get; set; }

        public List<MasterVendorViewModel> ListVendor { get; set; }
        public List<ListRoadCondition> ListRoad { get; set; }


        //public virtual MasterLocationViewModel MasterLocation { get; set; }
        //public virtual MasterVendorViewModel MasterVendor { get; set; }
    }
    public class ListRoadCondition
    {
        public int IDList { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}