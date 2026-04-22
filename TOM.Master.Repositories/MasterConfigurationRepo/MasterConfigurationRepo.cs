using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Universal.Repositories;
using DFIS.Contracts;

namespace TOM.Master.Repositories
{
    public interface IMasterConfigurationRepo : IMasterConfigurationRepo<MasterConfiguration> { }
    public class MasterConfigurationRepo : TOMGenericRepository<MasterConfiguration>, IMasterConfigurationRepo
    {
        public MasterConfigurationRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<MasterConfiguration> GetAllMasterConfiguration()
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            return Get(queryFilter).ToList();
        }

        public List<string> GetDistinctPageName()
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            return Get(queryFilter).ToList().Select(x => x.PageName).Distinct().ToList();
        }

        public MasterConfiguration GetMasterConfigurationByPageNameDescription(string pageName, string description)
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.PageName.Equals(pageName));
            queryFilter = queryFilter.And(x => x.Description.Equals(description));
            return Get(queryFilter).FirstOrDefault();
        }

        public List<MasterConfiguration> GetMasterConfigurationListByPageNameDescription(string pageName, List<string> description)
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.PageName.Equals(pageName));
            queryFilter = queryFilter.And(x => description.Contains(x.Description));
            return Get(queryFilter).ToList();
        }

        public List<MasterConfiguration> GetListMasterConfigurationByPageNameDescription(string pageName, string description)
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.PageName.Equals(pageName));
            queryFilter = queryFilter.And(x => x.Description.Contains(description));
            return Get(queryFilter).ToList();
        }

        public MasterConfiguration Checkavailability(MasterConfiguration input)
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            queryFilter = queryFilter.And(m => m.PageName == input.PageName);
            queryFilter = queryFilter.And(m => m.Description == input.Description);
            queryFilter = queryFilter.And(m => m.Value == input.Value);
            return Get(queryFilter).SingleOrDefault();
        }

        public MasterConfiguration SaveData(MasterConfiguration input, bool status)
        {
            MasterConfiguration temp = Checkavailability(input);
            if (status)
            {
                if (temp == null)
                {
                    input.CreatedDate = DateTime.Now;
                    input.UpdatedDate = DateTime.Now;
                    Insert(input);
                    Save();
                }
                else
                {
                    temp.Description = input.Description;
                    temp.Value = input.Value;
                    temp.IsActive = input.IsActive;
                    temp.UpdatedBy = input.UpdatedBy;
                    temp.UpdatedDate = DateTime.Now;
                    Update(temp);
                    Save();
                }
            }
            else
            {
                if (temp != null)
                {
                    temp.Description = input.Description;
                    temp.Value = input.Value;
                    temp.IsActive = input.IsActive;
                    temp.UpdatedBy = input.UpdatedBy;
                    temp.UpdatedDate = DateTime.Now;
                    Update(temp);
                }
                else
                {
                    input.UpdatedDate = DateTime.Now;
                    Update(input);
                }
                Save();
            }
            return input;
        }
    }
}
