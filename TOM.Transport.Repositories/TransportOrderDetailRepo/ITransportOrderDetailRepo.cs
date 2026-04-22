using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public interface ITransportOrderDetailRepo
    {
        List<TransportOrderDetailDTO> GetAllTransportOrderDetail(TransportOrderInput criteria);
        void SaveData(TransportOrderDetail input, bool status);
        void SaveDataTODetail(List<TransportOrderDetail> listTODetail);
        void InsertListData(List<TransportOrderDetail> listTODetail);
        TransportOrderDetail GetDataByID(int idTOD);
        void DeleteDataByListTO(List<int> listTODDelete);
        void DeleteDataByTO(int idTO);
        void setInActiveByTO(int idTO);
        List<TransportOrderDetailPrintDTO> GetTransportOrderDetailByidTO(int idTO);
        List<TransportOrderDetailPrintDTO> GetTransportOrderDetailBySenderReceiverTE(string sender, string receiver, int idTE);
        List<TransportOrderDetailPrintDTO> GetTransportOrderDetailByidTE(int idTE);
    }
}
