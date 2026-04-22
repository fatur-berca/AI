using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using TOM.Master.Domain.Inputs;
using TOM.EntitiesDAL;

namespace TOM.Master.Repositories
{
    public class MasterCostCenterAccountRepo : TOMGenericRepository<MasterCostCenter>, IMasterCostCenterAccountRepo
    {
        public MasterCostCenterAccountRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }
        
        public List<MasterCostCenter> GetAllMasterCostCenterAccount(MasterCostCenterAccountInput input)
        {
            var queryFilter = PredicateHelper.True<MasterCostCenter>();
            if (!string.IsNullOrEmpty(input.filterDate))
            {
                DateTime filterDate = Convert.ToDateTime(input.filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveStartDate <= filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveEndDate >= filterDate);
            }
            if (input.filterSenderLocation != null && !String.IsNullOrEmpty(input.filterSenderLocation[0]))
            {
                queryFilter = queryFilter.And(x => input.filterSenderLocation.Contains(x.Sender));
            }
            if (input.IDCostCenter > 0)
            {
                queryFilter = queryFilter.And(x => x.IDCostCenter == input.IDCostCenter);
            }
            queryFilter = queryFilter.And(m => m.IsActive == true);
            return Get(queryFilter).ToList();           
        }

        public MasterCostCenter GetMasterCostCenterBySenderReceiveMaterial(DateTime date, string sender, string receive, string material)
        {
            var queryFilter = PredicateHelper.True<MasterCostCenter>();
            queryFilter = queryFilter.And(x => x.EffectiveStartDate <= date);
            queryFilter = queryFilter.And(x => x.EffectiveEndDate >= date);
            queryFilter = queryFilter.And(x => x.Sender == sender);
            queryFilter = queryFilter.And(x => x.Receiver == receive);
            if (!String.IsNullOrEmpty(material))
            {
                queryFilter = queryFilter.And(x => x.MaterialType == material);
            }
            queryFilter = queryFilter.And(m => m.IsActive);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<MasterCostCenter> GetAllMasterCostCenterAccountActive()
        {
            var queryFilter = PredicateHelper.True<MasterCostCenter>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).ToList();
        }

        public List<string> GetDistinctCostCenterListActive()
        {
            var queryFilter = PredicateHelper.True<MasterCostCenter>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).GroupBy(x => x.CostCenter).Select(x=> x.FirstOrDefault().CostCenter).ToList();
        }  

        public void SaveData(MasterCostCenter input, bool status)
        {
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();
        }
        public MasterCostCenter GetCostCenterBySenderReceiverMaterial(MasterCostCenterAccountInput input)
        {
            var queryFilter = PredicateHelper.True<MasterCostCenter>();
            queryFilter = queryFilter.And(x => x.Sender == input.SenderIDLocation);
            queryFilter = queryFilter.And(x => x.Receiver == input.ReceiverIDLocation);
            queryFilter = queryFilter.And(x => x.MaterialType == input.MaterialType).And(x => x.IsActive);
            if (input.filterShipmentDate != null)
            {
                queryFilter = queryFilter.And(x => x.EffectiveEndDate >= input.filterShipmentDate);
                queryFilter = queryFilter.And(x => x.EffectiveStartDate <= input.filterShipmentDate);
            }
            return Get(queryFilter).FirstOrDefault();
        }
    }
}
