using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using System;

namespace TOM.Master.Repositories
{
    public class MasterFABrandRepo : TOMGenericRepository<MasterFABrand>, IMasterFABrandRepo
    {
        public MasterFABrandRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public MasterFABrand CheckBrandAvailabilityBySpeakingCode(string speakingCode)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();
            queryFilter = queryFilter.And(x => x.SpeakingCode.Contains(speakingCode)).And(x => x.IsActive);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterFABrand GetMasterFABrandByFaCode(string facode)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();
            queryFilter = queryFilter.And(x => x.FACode == facode);
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

        public MasterFABrand CheckBrandAvailabilityByCode(string Code)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();
            queryFilter = queryFilter.And(x => x.FACode.Equals(Code)).And(x => x.IsActive);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterFABrand CheckBrand(string FACode, string LongSpeakingCode)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();
            queryFilter = queryFilter.And(x => x.FACode.Equals(FACode)).And(x => x.LongSpeakingCode.Equals(LongSpeakingCode));
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterFABrand CheckBrandFACode(string FACode)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();
            queryFilter = queryFilter.And(x => x.FACode.Equals(FACode));
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterFABrand CheckBrandLONGCode(string LongSpeakingCode)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();
            queryFilter = queryFilter.And(x => x.LongSpeakingCode.Equals(LongSpeakingCode));
            return Get(queryFilter).FirstOrDefault();
        }
    }
}
