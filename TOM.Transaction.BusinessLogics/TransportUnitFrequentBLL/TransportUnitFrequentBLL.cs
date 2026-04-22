using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Repositories;
using DFIS.Contracts;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Universal.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.BusinessLogics
{
    public class TransportUnitFrequentBLL : ITransportUnitFrequentBLL
    {
        private readonly IGenericRepository<MasterList> _generalMasterListRepo;

        public TransportUnitFrequentBLL(IGenericRepository<MasterList> generalMasterListRepo)
        {
            _generalMasterListRepo = generalMasterListRepo;
        }

        public List<TransportUnitFrequentDTO> GetViewUnitFrequent(TransportUnitFrequentInput input)
        {
            var context = new TOMContextDB();
            var trdate = Convert.ToDateTime(input.TransactionDate);
            #warning [Migration] Function is disabled
            return null;

            /*
            var dbResult = context.GenTransportUnitFreq(trdate.Day, trdate.Month, trdate.Year).ToList();
          
            if (input.Vendor != null)
            {
                dbResult = dbResult.Where(x => x.Vendor == input.Vendor).ToList();
            }
            if (input.VehicleType != null)
            {
                dbResult = dbResult.Where(x => x.VehicleType == input.VehicleType).ToList();
            }
            return Mapper.Map<List<TransportUnitFrequentDTO>>(dbResult);
            
          */
        }

        public List<MasterListDTO> GetMasterLists()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            var dbResult = _generalMasterListRepo.Get(queryFilter).ToList();
            dbResult = dbResult.Where(x => x.FieldName == "vehicletype" && x.IsActive == true).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }
    }
}
