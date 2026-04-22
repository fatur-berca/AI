using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Contracts;
using DFIS.Utils;
using AutoMapper;
using DFIS.Utils.Exceptions;

namespace TOM.Master.BusinessLogics
{
    public class MasterUomBLL : IMasterUomBLL
    {
        private readonly IGenericRepository<MasterUom> _generalRepo;

        public MasterUomBLL(IGenericRepository<MasterUom> generalRepo)
        {
            _generalRepo = generalRepo;
        }

        public MasterUomDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);

            // return result and Mapping result from entity to DTO
            // because the return type is DTO
            return Mapper.Map<MasterUomDTO>(result);
        }
        public List<MasterUomDTO> GetMasterUom(MasterUomInput input)
        {
            var queryFilter = PredicateHelper.True<MasterUom>();
            if (!String.IsNullOrEmpty(input.MaterialType))
            {
                queryFilter = queryFilter.And(m => m.MaterialType.Equals(input.MaterialType, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!String.IsNullOrEmpty(input.UoM))
            {
                queryFilter = queryFilter.And(m => m.UoM.Equals(input.UoM, StringComparison.InvariantCultureIgnoreCase));
            }
            //queryFilter = queryFilter.And(m => m.IsActive == true);
            

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterUom>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            var result = dbResult.OrderByDescending(x => x.MaterialType);

            return Mapper.Map<List<MasterUomDTO>>(result);
        }

        public MasterUomDTO SaveData(MasterUomDTO input, string controller, string userid)
        {
            var validateInput = new MasterUomInput()
            {
                MaterialType = input.MaterialType,
                UoM = input.UoM,
                IsActive = true
            };
            var prevData = GetMasterUom(validateInput);
            var dbMst = Mapper.Map<MasterUom>(input);

            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                dbMst.CreatedDate = DateTime.Now;
                dbMst.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMst);
                _generalRepo.Save();
                //_generalRepo.Save(controller, userid);
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterUomDTO>(dbMst);
        }

        public MasterUomDTO EditData(MasterUomDTO input, string controller, string userid)
        {
            var dbMasterList = Mapper.Map<MasterUom>(input);
            var validateInput = new MasterUomInput()
            {
                //IdUom = input.IdUom,
                MaterialType = input.MaterialType,
                UoM = input.UoM
            };

            var prevData = GetMasterUom(validateInput);
            var materialtype = _generalRepo.Get().Where(x => x.MaterialType.Equals(input.MaterialType, StringComparison.InvariantCultureIgnoreCase) && x.UoM.Equals(input.UoM, StringComparison.InvariantCultureIgnoreCase) && x.IDUoM == input.IDUoM).Select(x => x.MaterialType).FirstOrDefault();
            var uomvalue = _generalRepo.Get().Where(x => x.UoM.Equals(input.UoM, StringComparison.InvariantCultureIgnoreCase) && x.MaterialType.Equals(input.MaterialType, StringComparison.InvariantCultureIgnoreCase) && x.IDUoM == input.IDUoM).Select(x => x.UoM).FirstOrDefault();
            var iduom = _generalRepo.Get().Where(x => x.UoM.Equals(input.UoM, StringComparison.InvariantCultureIgnoreCase) && x.MaterialType.Equals(input.MaterialType, StringComparison.InvariantCultureIgnoreCase) && x.IDUoM == input.IDUoM).Select(x => x.IDUoM).FirstOrDefault();
            
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {

                TOMContextDB context = new TOMContextDB();
                MasterUom c = (from x in context.MasterUoms
                               where x.IDUoM == input.IDUoM
                                select x).First();
                c.MaterialType = input.MaterialType;
                c.UoM = input.UoM;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();

            }
            //else if (materialtype.Equals(input.MaterialType, StringComparison.InvariantCultureIgnoreCase) && uomvalue.Equals(input.UoMValue, StringComparison.InvariantCultureIgnoreCase) && iduom == input.IdUom)
            //{
            //    DFISContextDB context = new DFISContextDB();
            //    MasterUom c = (from x in context.MasterUoms
            //                   where x.IdUom == input.IdUom
            //                    select x).First();
            //    c.MaterialType = input.MaterialType;
            //    c.UoMValue = input.UoMValue;
            //    c.UpdatedDate = DateTime.Now;
            //    c.IsActive = input.IsActive;
            //    c.UpdatedBy = input.UpdatedBy;
            //    context.SaveChanges();
            //}

            else
            {

                TOMContextDB context = new TOMContextDB();
                MasterUom c = (from x in context.MasterUoms
                               where x.IDUoM == input.IDUoM
                               select x).First();
                //c.MaterialType = input.MaterialType;
                //c.UoM = input.UoM;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();
            }
            return Mapper.Map<MasterUomDTO>(dbMasterList);
        }
    }
}
