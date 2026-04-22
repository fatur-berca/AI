using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Inputs;
using System;
using DFIS.Contracts;

namespace TOM.Transport.Repositories.TransportOrderRequestRepo
{
    public interface ITransportOrderRequestRepo : IGenericRepository<TransportOrderRequest>
    {
        TransportOrderRequest GetTransportOrderRequestByID(int idreq);
        int GetIDRequestByReqNo(string reqNo);
        List<TransportOrderRequest> GetTransportOrderRequest(TransportOrderRequestInput input);
        int SaveData(TransportOrderRequest input);

        string getLastRequestNumber(DateTime shipmentDate);
        string getNewRequestNumber(DateTime shipmentDate);
        int InsertData(TransportOrderRequest input);        
    }
}
