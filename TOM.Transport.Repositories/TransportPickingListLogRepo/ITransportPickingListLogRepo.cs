using System;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories
{
    public interface ITransportPickingListLogRepo
    {
        List<TransportPickingListLog> GetAllBatchDate(DateTime tfrom, DateTime tto);
        void SaveDataPickingList(List<TransportPickingListLog> listPL, List<TransportOrder> listTO);
        int SaveData(TransportPickingListLog input, bool status);
        string setCurrSTOPreOrder(string STONo);
        string SendEmail(string userid);
    }
}
