using System;
using System.Collections.Generic;
using System.Web.Mvc;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Models.Transport
{
    public class TransportExecutionPrintViewModel
    {
        public int TotalPage { get; set; }
        public int TotalGatePass { get; set; }
        public string SampoernaPIC { get; set; }
        public int MAX_ROW_PER_PAGE = 22;
        public List<TransportOrderTransportExecutionPrintViewModel> ListTOGroup { get; set; }
        public List<TransportOrderTransportExecutionPrintViewModel> ListTONoGroup { get; set; }
        public List<TransportOrderDetailPrintViewModel> ListTOD { get; set; }
        public List<TransportOrderViewModel> ListTOGate { get; set; }
        public List<TransportExecutionPrintMemoViewModel> ListTE { get; set; }
    }
}