using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using DFIS.Utils;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System.Linq.Expressions;
using System.Collections;

namespace TOM.Transport.Repositories.TransportExecutionTempRepo
{
    public class TransportExecutionTempRepo : TOMGenericRepository<TransportExecutionTemp>, ITransportExecutionTempRepo
    {
        private TOMContextDB _context;

        public TransportExecutionTempRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }

        public List<TransportExecutionTemp> GetTransportExecutionByUser(string IDUser)
        {
            var queryFilter = PredicateHelper.True<TransportExecutionTemp>();
            queryFilter = queryFilter.And(x => x.IDUser == IDUser);
            return Get(queryFilter).ToList();
        }

        public int SaveData(TransportExecutionTemp input)
        {            
            Insert(input);
            Save();
            return 0;
        }
        public void DeleteDataByUser(string userid)
        {
            Delete(userid);
            Save();            
        }
    }
}
