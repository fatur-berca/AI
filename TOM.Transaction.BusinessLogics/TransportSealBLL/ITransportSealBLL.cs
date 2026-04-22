using DFIS.Universal.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.BusinessLogics.TransportSealBLL
{
    public interface ITransportSealBLL
    {
        //List<TransportOrderDTO> GetTransportOrderDtos(TransportOrderDTO criteria);
        List<TransportOrderDTO> GetTransportOrderByTransactionNo(string id);
        List<TransportSealDTO> GetDatas(TransportSealInput criteria);
        int SaveSealDto(TransportSealDTO input);

        List<TransportSealDTO> GetValidateDataSealDtos(TransportSealDTO input);
        TransportSealDTO EditData(TransportSealDTO input);
        List<string> Upload(HttpPostedFileBase input, string userid);
        bool DeleteSealNumber(string transportationNumber, string sealNumber);
        List<MasterLocationDTO> GetLocationByTN(string TN);
    }
}
