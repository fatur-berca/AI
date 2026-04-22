using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace TOM.Master.Repositories
{
    public class MasterConfigurationEmailRepo : TOMGenericRepository<MasterConfigurationEmail>, IMasterConfigurationEmailRepo
    {
        public MasterConfigurationEmailRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<MasterConfigurationEmail> GetAllMasterConfigurationEmail()
        {
            var queryFilter = PredicateHelper.True<MasterConfigurationEmail>();
            return Get(queryFilter).ToList();
        }

        public MasterConfigurationEmail CheckAvailability(string pageName)
        {
            var queryFilter = PredicateHelper.True<MasterConfigurationEmail>();
            queryFilter = queryFilter.And(x => x.PageName == pageName);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterConfigurationEmail SaveData(MasterConfigurationEmail input, bool status)
        {
            MasterConfigurationEmail temp = CheckAvailability(input.PageName);
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
                    input = null;
            }
            else
            {
                if (input.Receiver == null)
                    temp.Receiver = "";
                else
                    temp.Receiver = input.Receiver;
                temp.BodyEmail = input.BodyEmail;
                temp.IsActive = input.IsActive;
                temp.UpdatedBy = input.UpdatedBy;
                temp.UpdatedDate = DateTime.Now;
                Update(temp);
                Save();
            }
            return input;
        }
    }
}
