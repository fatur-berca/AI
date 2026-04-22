using AutoMapper;
using System;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportExecutionPrintMemoDTO
    {
        public TransportExecutionPrintMemoDTO()
        { }

        public TransportExecutionPrintMemoDTO(TransportExecution transportExecution, List<MasterVendor> mstVendor, List<TransportVesselMonitoring> transportVessel)
        {
            IDTransportExecution = transportExecution.IDTransportExecution;
            TransportDate = transportExecution.TransportDate;
            //MasterVendor = Mapper.Map<MasterVendor, MasterVendorDTO>(mstVendor);
            TransportVesselMonitoring = Mapper.Map<TransportVesselMonitoring, TransportVesselMonitoringDTO>(transportVessel.Count == 0 ? new TransportVesselMonitoring() : transportVessel[0]);
            MasterVendor = Mapper.Map<MasterVendor, MasterVendorDTO>(mstVendor.Count == 0 ? new MasterVendor() : mstVendor[0]);
        }

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
        public Nullable<System.DateTime> ATD { get; set; }                
        public Nullable<System.DateTime> GRDate { get; set; }
        public string ATDDate { get; set; }
        public string GRDateTO { get; set; }
        public string SONumber { get; set; }
        public int? POWeek { get; set; }
        public string Remarks { get; set; }

        public virtual MasterVendorDTO MasterVendor { get; set; }
        public virtual TransportVesselMonitoringDTO TransportVesselMonitoring { get; set; }
    }
}
