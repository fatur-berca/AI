using System.Collections.Generic;
using System.Linq;
using DFIS.EntitiesDAL;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace DFIS.Master.Repositories
{
    public class MasterFABrandRepo : GenericRepository<MasterFABrand>, IMasterFABrandRepo
    {
        public MasterFABrandRepo(DFISContextDB contextEntities) : base(contextEntities)
        {
        }

        public MasterFABrand CheckBrandAvailabilityBySpeakingCode(string speakingCode)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();
            queryFilter = queryFilter.And(x => x.SpeakingCode.Contains(speakingCode)).And(x => x.IsActive);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<string> ListAvailableSpeakingCode()
        {
            var query = from w in Get()
                select new
                {
                    speakingCode = (w.SpeakingCode.Substring(5, 1) == "R"
                        ? w.SpeakingCode.Substring(0, 5)
                        : w.SpeakingCode.Substring(0, 6))
                };
            List<string> distinct = (from c in query
                group c by new
                {
                    c.speakingCode
                }
                into grp
                select grp.FirstOrDefault()).OrderBy(x => x.speakingCode).Select(x => x.speakingCode).ToList();
            return distinct;
        }
    }
}
