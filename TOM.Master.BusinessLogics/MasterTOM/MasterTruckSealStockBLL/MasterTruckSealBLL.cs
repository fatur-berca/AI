using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Universal.Domain.DTOs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;

namespace TOM.Master.BusinessLogics
{
    public class MasterTruckSealBLL : IMasterTruckSealBLL
    {
        private readonly IGenericRepository<MasterTruckSealStock> _generalRepo;
        private readonly IGenericRepository<TransportSeal> _transportSeal;

        public MasterTruckSealBLL(IGenericRepository<MasterTruckSealStock> generalRepo, IGenericRepository<TransportSeal> transportSeal)
        {
            _generalRepo = generalRepo;
            _transportSeal = transportSeal;
        }

        public List<MasterTruckSealDTO> GetMasterTruckSeal()
        {
            var dbResult = _generalRepo.Get().OrderBy(m => m.SealNumberFrom).ToList();
            return Mapper.Map<List<MasterTruckSealDTO>>(dbResult);
        }

        public bool SaveData(MasterTruckSealDTO input)
        {
            var mstTruckSeal = Mapper.Map<MasterTruckSealStock>(input);
            var context = new TOMContextDB();
            var validateData = context.CheckSNTruckSeal(int.Parse(input.SealNumberFrom),int.Parse(input.SealNumberTo)).ToList();

            if (validateData.Count == 0)
            {
                _generalRepo.Insert(mstTruckSeal);
                _generalRepo.Save();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DelData(int keyID)
        {
            var context = new TOMContextDB();
            var getData = context.MasterTruckSealStocks
                .Where(m => m.IDTruckSealStock == keyID);

            if (getData != null)
            {
                _generalRepo.Delete(keyID);
                _generalRepo.Save();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckAvailSealNumber(string sealNumber)
        {
            var dbResult = false;
            try
            {
                var listMasterTruckSeal = _generalRepo.GetAll().Where(x => x.IsActive).ToList();
                var isExistSN = listMasterTruckSeal.ToList().Where(x => Int32.Parse(x.SealNumberFrom) <= Int32.Parse(sealNumber) && Int32.Parse(x.SealNumberTo) >= Int32.Parse(sealNumber)).ToList();
                return isExistSN.Count > 0;
            }
            catch (ExceptionBase ex)
            {
                var message = ex.Message;
            }
            return dbResult;
        }

        
    }
}
