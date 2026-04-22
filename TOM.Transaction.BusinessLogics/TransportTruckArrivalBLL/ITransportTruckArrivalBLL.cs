using System.Collections.Generic;
using System.IO;
using DFIS.Universal.Domain.DTOs;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.Inputs;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.BusinessLogics
{
    public interface ITransportTruckArrivalBLL
    {
        List<MasterListDTO> GetAllETACategory();
        List<TransportTruckArrivalDTO> GetAllTransportTruckArrival(TransportTruckArrivalInput filter);
        MasterConfigurationDTO GetMaxSpeed();
        void UpdateCalculate(TransportTruckArrival save, string userid);
        void DeleteTruckArrivalTemp();
        void SaveUpload(string fileLocation);
        void SaveUploadXLS(string fileLocation);
        void MoveFile(string source, string des);
        MemoryStream ReportExport(TransportTruckArrivalInput filter);
    }
}
