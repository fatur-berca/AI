using AutoMapper;
using System;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportExecutionVendorDTO
    {
        public TransportExecutionVendorDTO()
        { }
        public TransportExecutionVendorDTO(TransportExecutionVendorDTO te, MasterVendor mv)
        {
            IDVendor = te.IDVendor;
            TotalVendor = te.TotalVendor;
            //TransportExecution = Mapper.Map<TransportExecution, TransportExecutionDTO>(to1);
            MasterVendor = Mapper.Map<MasterVendor, MasterVendorDTO>(mv);
        }

        public int IDTransportOrder { get; set; }
        public int IDTransportExecution { get; set; }
        public int? IDVendor { get; set; }
        public int TotalVendor { get; set; }

        public virtual MasterVendorDTO MasterVendor { get; set; }
    }
}
