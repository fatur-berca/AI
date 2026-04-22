
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Models.Transport
{
    public class TransportExecutionPrintMemoViewModel
    {
        public int IDTransportExecution { get; set; }
        public DateTime TransportDate { get; set; }
        public string TransportNo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string CompanyName { get; set; }
        public string VesselName { get; set; }
        public string VendorName { get; set; }
        public int Week { get; set; }
        public int? WeekTransportDate { get; set; }
        public int Year { get; set; }
        public string dateWeek { get; set; }
        //public string dateToWeek { get; set; }
        public string ATDDate { get; set; }        
        public string GRDateTO { get; set; }
        public string SONumber { get; set; }
        public int POWeek { get; set; }
        public string Remarks { get; set; }
        public int MAX_ROW_PER_PAGE = 25;

        public virtual MasterVendorViewModel MasterVendor { get; set; }
        public virtual TransportVesselMonitoringViewModel TransportVesselMonitoring { get; set; }
    }
}