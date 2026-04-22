using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using DFIS.Utils;
using TOM.EntitiesDAL;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public class MasterLeadTimeTomRepo : TOMGenericRepository<MasterLeadTime>, IMasterLeadTimeTomRepo
    {
        public MasterLeadTimeTomRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        /*public List<MasterLeadTimeTOM> GetAllMasterLeadTimeTom()
        {
            var queryFilter = PredicateHelper.True<MasterLeadTimeTOM>();
            return Get(queryFilter).ToList();
        }*/

        public List<MasterLeadTime> GetAllMasterLeadTimeTOM(MasterLeadTimeTomInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLeadTime>();
            if (!string.IsNullOrEmpty(input.filterDate))
            {
                DateTime filterDate = Convert.ToDateTime(input.filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveStartDate <= filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveEndDate >= filterDate);
            }
            if (input.filterSenderLocation != null && !String.IsNullOrEmpty(input.filterSenderLocation[0]))
            {
                queryFilter = queryFilter.And(x => input.filterSenderLocation.Contains(x.SenderIDLocation));
            }
            queryFilter = queryFilter.And(m => m.IsActive == true);
            return Get(queryFilter).ToList();
        }
        
        public MasterLeadTime GetMasterLeadTimeFilterByTOTE(int? idVendor, string sender, string receiver, DateTime ShipmentDate)
        {
            var queryFilter = PredicateHelper.True<MasterLeadTime>();
            queryFilter = queryFilter.And(m => m.IsActive == true);
            queryFilter = queryFilter.And(m => m.IDVendor == idVendor);
            queryFilter = queryFilter.And(m => m.SenderIDLocation == sender);
            queryFilter = queryFilter.And(m => m.ReceiverIDLocation == receiver);
            //DateTime filterDate = DateTime.Now;
            queryFilter = queryFilter.And(x => x.EffectiveStartDate <= ShipmentDate);
            queryFilter = queryFilter.And(x => x.EffectiveEndDate >= ShipmentDate);
            return Get(queryFilter).FirstOrDefault();
        }
    }
}
