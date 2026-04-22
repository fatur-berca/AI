using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System.Collections;

namespace TOM.Transport.Repositories.TransportExecutionTempRepo
{
    public interface ITransportExecutionTempRepo : IGenericRepository<TransportExecutionTemp>
    {
        List<TransportExecutionTemp> GetTransportExecutionByUser(string IDUser);
        int SaveData(TransportExecutionTemp input);
        void DeleteDataByUser(string userid);
    }
}
