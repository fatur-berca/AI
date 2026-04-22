using System;
using System.Collections.Generic;
using TOM.Master.Domain.DTOs;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public interface IMasterDistanceRepo
    {
        List<MasterDistanceDTO> GetAllMasterDistanceDTO(MasterDistanceInput input);
        List<MasterDistance> GetAllMasterDistance(MasterDistanceInput input);
        MasterDistance GetMasterDistanceActiveByTypeSenderReceiverModeVia(string distanceType, DateTime transDate, string sender, string receiver, string transMode, string via);
        void SaveData(MasterDistance input, bool status);
        MasterDistance GetMasterDistanceByField(string idSenderLoc, string idReceiveLoc, DateTime? ShipmentDate);
    }
}
