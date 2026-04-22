using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;

namespace TOM.Master.BusinessLogics
{
    public class MasterTransportRegionBLL : IMasterTransportRegionBLL
    {
        private readonly IMasterTransportRegionRepo _masterTransportRegionRepo;
        private readonly IGenericRepository<MasterTransportRegion> _generalRepo;

        public MasterTransportRegionBLL(IMasterTransportRegionRepo masterTransportRegionRepo, IGenericRepository<MasterTransportRegion> generalRepo)
        {
            _masterTransportRegionRepo = masterTransportRegionRepo;
            _generalRepo = generalRepo;
        }
        public List<MasterTransportRegionDTO> GetMasterTransportRegions(MasterTransportRegionInput input)
        {
            var queryFilter = PredicateHelper.True<MasterTransportRegion>();

            if (!string.IsNullOrEmpty(input.GPNumber))
                queryFilter = queryFilter.And(m => m.GPNumber == input.GPNumber);

            //if (!string.IsNullOrEmpty(input.TransportRegion))
            //{
            //    queryFilter = queryFilter.And(m => m.TransportRegion == input.TransportRegion);
            //}

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterTransportRegion>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterTransportRegionDTO>>(dbResult);
        }

        public MasterTransportRegion CheckDataAvailability(MasterTransportRegionDTO input)
        {
            var queryFilter = PredicateHelper.True<MasterTransportRegion>();
            queryFilter = queryFilter.And(m => m.GPNumber == input.GPNumber);
            return Mapper.Map<MasterTransportRegion>(_generalRepo.Get(queryFilter).SingleOrDefault());
        }

        public MasterTransportRegionDTO SaveData(MasterTransportRegionDTO input)
        {
            var validateInput = new MasterTransportRegionInput()
            {
                GPNumber = input.GPNumber,
                TransportRegion = input.TransportRegion,
                IsActive = input.IsActive
            };
            var prevData = GetMasterTransportRegions(validateInput);
            var dbMstTransportRegion = Mapper.Map<MasterTransportRegion>(input);
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                dbMstTransportRegion.GPNumber = input.GPNumber.ToUpper();
                dbMstTransportRegion.CreatedDate = DateTime.Now;
                dbMstTransportRegion.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstTransportRegion);
                _generalRepo.Save();
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterTransportRegionDTO>(dbMstTransportRegion);
        }

        public MasterTransportRegionDTO EditData(MasterTransportRegionDTO input)
        {

            var dbMstVendor = Mapper.Map<MasterTransportRegion>(input);

            var validateInput = new MasterTransportRegionInput()
            {
                GPNumber = input.GPNumber,
                TransportRegion = input.TransportRegion,
                IsActive = input.IsActive
            };

            var prevData = GetMasterTransportRegions(validateInput);
            var vendorid = _generalRepo.Get().Where(x => x.GPNumber == input.GPNumber).Select(x => x.GPNumber).FirstOrDefault();
            // var namevendor = _generalRepo.Get().Where(x => x.VendorName == input.VendorName && x.IDVendor == input.IDVendor).Select(x => x.VendorName).FirstOrDefault();

            if (prevData == null || (prevData != null && prevData.Count == 0))
            {

                var context = new TOMContextDB();


                MasterTransportRegion c = (from x in context.MasterTransportRegions
                                           where x.GPNumber == input.GPNumber
                                           select x).First();
                c.TransportRegion = input.TransportRegion;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();

            }
            else if (vendorid == input.GPNumber)
            {
                var context = new TOMContextDB();


                MasterTransportRegion c = (from x in context.MasterTransportRegions
                                           where x.GPNumber == input.GPNumber
                                           select x).First();
                c.TransportRegion = input.TransportRegion;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();
            }

            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterTransportRegionDTO>(dbMstVendor);
        }

        public MasterTransportRegionDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);

            // return result and Mapping result from entity to DTO
            // because the return type is DTO
            return Mapper.Map<MasterTransportRegionDTO>(result);
        }
    }
}
