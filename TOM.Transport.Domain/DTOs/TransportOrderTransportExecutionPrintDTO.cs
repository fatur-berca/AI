using System;
using AutoMapper;
using DFIS.Universal.Domain.DTOs;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using System.Collections.Generic;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportOrderTransportExecutionPrintDTO
    {        
        private TransportOrder to;
        private MasterLocation ml;
        private MasterLocation ml2;
        private TransportExecution transportExecution;
        private MasterVendor masterVendor;
        private TransportDriverManagement tdm;
        private TransportDriverManagement tdm1;
        private TransportDriverManagement tdm2;
        private TransportVesselMonitoring tvm;

        public TransportOrderTransportExecutionPrintDTO()
        {}                

        public TransportOrderTransportExecutionPrintDTO(TransportOrder to, MasterLocation ml, MasterLocation ml2, TransportExecution transportExecution, List<TransportDriverManagement> tdm, List<TransportDriverManagement> tdm1, List<TransportDriverManagement> tdm2, List<TransportVesselMonitoring> tvm, List<GeneralBuildingFacility> gbf, List<GeneralBuildingFacility> gbf1, List<MasterVendor> mv, MasterLocation ml3)
        {            
            IDTransportOrder = to.IDTransportOrder;
            IDTransportExecution = to.IDTransportExecution;
            STONo = to.STONo;
            DefaultSeqNo = to.DefaultSeqNo;
            SeqNo = to.SeqNo;
            ShipmentDate = to.ShipmentDate;
            SenderIDLocation = to.ActualSenderIDLocation;
            ReceiverIDLocation = to.ActualReceiverIDLocation;
            Remarks = to.Remarks;
            IsActive = to.IsActive;
            CreatedBy = to.CreatedBy;
            CreatedDate = to.CreatedDate;
            UpdatedBy = to.UpdatedBy;
            UpdatedDate = to.UpdatedDate;
            ZoneBased = to.ZoneBased;
            MasterLocation = Mapper.Map<MasterLocation, MasterLocationDTO>(ml);
            MasterLocation1 = Mapper.Map<MasterLocation, MasterLocationDTO>(ml2);
            MasterLocation2 = Mapper.Map<MasterLocation, MasterLocationDTO>(ml3);
            TransportExecution = Mapper.Map<TransportExecution, TransportExecutionDTO>(transportExecution);
            //MasterVendor = Mapper.Map<MasterVendor, MasterVendorDTO>(masterVendor);
            TransportDriverManagement = Mapper.Map<TransportDriverManagement, TransportDriverManagementDTO>(tdm.Count == 0 ? new TransportDriverManagement() : tdm[0]);
            TransportDriverManagement1 = Mapper.Map<TransportDriverManagement, TransportDriverManagementDTO>(tdm1.Count == 0 ? new TransportDriverManagement() : tdm1[0]);
            TransportDriverManagement2 = Mapper.Map<TransportDriverManagement, TransportDriverManagementDTO>(tdm2.Count == 0 ? new TransportDriverManagement() : tdm2[0]);
            TransportVesselMonitoring = Mapper.Map<TransportVesselMonitoring, TransportVesselMonitoringDTO>(tvm.Count == 0 ? new TransportVesselMonitoring() : tvm[0]);
            GeneralBuildingFacility = Mapper.Map<GeneralBuildingFacility, GeneralBuildingFacilityDTO>(gbf.Count == 0 ? new GeneralBuildingFacility() : gbf[0]);
            GeneralBuildingFacility1 = Mapper.Map<GeneralBuildingFacility, GeneralBuildingFacilityDTO>(gbf1.Count == 0 ? new GeneralBuildingFacility() : gbf1[0]);
            MasterVendor = Mapper.Map<MasterVendor, MasterVendorDTO>(mv.Count == 0 ? new MasterVendor() : mv[0]);
            IDDriver1 = to.TransportExecution.IDDriver1;
            DriverName1 = (TransportDriverManagement == null) ? "" : TransportDriverManagement.Name;
            IDDriver2 = to.TransportExecution.IDDriver2;
            DriverName2 = (TransportDriverManagement1 == null) ? "" : TransportDriverManagement1.Name;
            IDCoDriver = to.TransportExecution.IDCoDriver;
            CoDriverName = (TransportDriverManagement2 == null) ? "" : TransportDriverManagement2.Name;

        }

        public int IDTransportOrder { get; set; }
        public int? IDTransportExecution { get; set; }
        public string STONo { get; set; }
        public string DefaultSeqNo { get; set; }
        public string SeqNo { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string SenderIDLocation { get; set; }
        public string SenderLocationName { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ReceiverLocationName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ZoneBased { get; set; }
        public string TransportNo { get; set; }
        public DateTime TransportDate { get; set; }
        public string ActualVehicleType { get; set; }
        public string PoliceRegNo { get; set; }
        public int? IDVendor { get; set; }
        public string IDDriver1 { get; set; }
        public string DriverName1 { get; set; }
        public string IDDriver2 { get; set; }
        public string DriverName2 { get; set; }
        public string IDCoDriver { get; set; }
        public string CoDriverName { get; set; }
        public string StartLocation { get; set; }
        public string VendorName { get; set; }        
        public string VesselName { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }
        public string WarehouseAddressSender { get; set; }
        public string WarehouseAddressReceiver { get; set; }
        public string TNBarcode { get; set; }

        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation1 { get; set; }
        public virtual MasterLocationDTO MasterLocation2 { get; set; }
        public virtual TransportExecutionDTO TransportExecution { get; set; }
        public virtual MasterVendorDTO MasterVendor { get; set; }
        public virtual TransportDriverManagementDTO TransportDriverManagement { get; set; }
        public virtual TransportDriverManagementDTO TransportDriverManagement1 { get; set; }
        public virtual TransportDriverManagementDTO TransportDriverManagement2 { get; set; }
        public virtual TransportVesselMonitoringDTO TransportVesselMonitoring { get; set; }
        public virtual GeneralBuildingFacilityDTO GeneralBuildingFacility { get; set; }
        public virtual GeneralBuildingFacilityDTO GeneralBuildingFacility1 { get; set; }
    }
}
