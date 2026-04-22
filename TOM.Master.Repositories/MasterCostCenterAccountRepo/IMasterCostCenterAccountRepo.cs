using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using System;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public interface IMasterCostCenterAccountRepo : IGenericRepository<MasterCostCenter>
    {
        List<MasterCostCenter> GetAllMasterCostCenterAccount(MasterCostCenterAccountInput input);
        MasterCostCenter GetMasterCostCenterBySenderReceiveMaterial(DateTime date, string sender, string receive, string material);
        List<MasterCostCenter> GetAllMasterCostCenterAccountActive();
        List<string> GetDistinctCostCenterListActive();
        void SaveData(MasterCostCenter input, bool status);
        MasterCostCenter GetCostCenterBySenderReceiverMaterial(MasterCostCenterAccountInput input);
    }
}
