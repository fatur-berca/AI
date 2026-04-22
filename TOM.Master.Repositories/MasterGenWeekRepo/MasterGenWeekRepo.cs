using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace TOM.Master.Repositories
{
    public class MasterGenWeekRepo : TOMGenericRepository<MasterGenWeek>, IMasterGenWeekRepo
    {
        public MasterGenWeekRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<int?> GetWeekListByYear(int year)
        {
            var queryFilter = PredicateHelper.True<MasterGenWeek>();
            queryFilter = queryFilter.And(x => x.Year == year);
            return Get(queryFilter).Select(x =>x.Week).ToList();
        }

        public MasterGenWeek GetFromToDateByWeekYear(int week, int year)
        {
            var queryFilter = PredicateHelper.True<MasterGenWeek>();
            queryFilter = queryFilter.And(x => x.Week == week);
            queryFilter = queryFilter.And(x => x.Year == year);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterGenWeek GetGenWeekNowByDate(DateTime now)
        {
            var queryFilter = PredicateHelper.True<MasterGenWeek>();
            queryFilter = queryFilter.And(x => x.StartDate <= now && x.EndDate >= now);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<MasterGenWeek> GetGenWeeksByDate(DateTime dateNow, DateTime dateLater)
        {
            var queryFilter = PredicateHelper.True<MasterGenWeek>();
            queryFilter = queryFilter.And(x => x.EndDate >= dateNow && x.EndDate <= dateLater);
            return Get(queryFilter).ToList();
        }

        public int GetMaxWeekInYear(int year)
        {
            var queryFilter = PredicateHelper.True<MasterGenWeek>();
            queryFilter = queryFilter.And(x => x.Year == year);
            var first = Get(queryFilter).OrderByDescending(x => x.Week).Select(x => x.Week).First();
            if (first != null)
                return (int) first;
            else
                return 0;
        }
    }
}
