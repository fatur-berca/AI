using System;
using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterGenWeekRepo : IGenericRepository<MasterGenWeek>
    {
        List<int?> GetWeekListByYear(int year);
        MasterGenWeek GetFromToDateByWeekYear(int week, int year);
        MasterGenWeek GetGenWeekNowByDate(DateTime now);
        List<MasterGenWeek> GetGenWeeksByDate(DateTime dateNow, DateTime dateLater);
        int GetMaxWeekInYear(int year);
    }
}
