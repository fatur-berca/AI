using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using TOM.Master.Domain.DTOs;
using DFIS.Contracts;
using System.Linq.Expressions;

namespace TOM.Master.Repositories
{
    public class MasterListRepo : TOMGenericRepository<MasterList>, IMasterListRepo
    {
        public MasterListRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        bool IGenericRepository<MasterList>.AllowLazyLoading { get; set; }

        /// <summary>
        /// Mengambil isian dropdown Type dari master list dengan where param field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>List MasterList</returns>
        public List<MasterList> GetMasterListByFieldName(string fieldName)
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.FieldName.Equals(fieldName));
            return Get(queryFilter).ToList();
        }

        public List<MasterList> GetMasterListByListFieldName(List<string> listFieldName)
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => listFieldName.Contains(p.FieldName));
            return Get(queryFilter).OrderBy(p => p.FieldName).ToList();
        }

        public MasterList GetMasterListCustom(string fieldname, string value)
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(p => p.FieldName.Equals(fieldname.Trim()));
            queryFilter = queryFilter.And(p => p.FieldValue.Equals(value.Trim()));
            return Get(queryFilter).FirstOrDefault();
        }

        public List<MasterList> GetMasterListByFieldNameListFieldValue(string fieldName, List<string> fieldValue)
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.FieldName.Equals(fieldName));
            queryFilter = queryFilter.And(p => fieldValue.Contains(p.FieldValue));
            return Get(queryFilter).ToList();
        }

        public List<string> GetMasterListFieldNameValue(string fieldName)
        {
            List<string> listSup = new List<string>();
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.FieldName.Equals(fieldName));
            var masterList = Get(queryFilter).ToList();
            foreach (MasterList mstList in masterList)
            {
                listSup.Add(mstList.FieldValue);
            }
            return listSup;
        }

        public bool IsMasterListValid(string fieldName, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldValue))
                return false;
            string propName = fieldValue.Trim().Replace(" ", "").Replace("\t", "").ToLower();
            var res = Get(ml =>
            (ml.FieldName != null && ml.FieldName.ToLower() == propName)
            && ml.FieldValue == fieldValue);

            return res != null && res.Count() > 0;
        }
    }
}
