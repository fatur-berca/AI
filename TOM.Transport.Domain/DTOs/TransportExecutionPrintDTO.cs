using System;
using System.Collections.Generic;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportExecutionPrintDTO
    {
        public int TotalPage { get; set; }
        public int TotalGatePass { get; set; }
        public string SampoernaPIC { get; set; }
        public List<TransportOrderTransportExecutionPrintDTO> ListTOGroup { get; set; }
        public List<TransportOrderTransportExecutionPrintDTO> ListTONoGroup { get; set; }
        public List<TransportOrderDetailPrintDTO> ListTOD { get; set; }
        public List<TransportOrderDTO> ListTOGate { get; set; }
        public List<TransportExecutionPrintMemoDTO> ListTE { get; set; }
        //public TransportExecutionPrintMemoDTO ListTE { get; set; }
        //public List<TransportExecutionPrintMemoDTO> ListTOAgent { get; set; }
        public string FileName { get; set; }
        public string Uid { get; set; }
    }
}
