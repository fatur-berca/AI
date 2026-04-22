using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using AutoMapper;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public class TransportTruckArrivalRepo : TOMGenericRepository<TransportTruckArrival>, ITransportTruckArrivalRepo
    {
        private TOMContextDB _context;
        public TransportTruckArrivalRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }

        public List<TransportTruckArrivalDTO> GetAllTransportTruckArrival(TransportTruckArrivalInput filter)
        {
            List<TransportTruckArrivalDTO> listTemp = new List<TransportTruckArrivalDTO>();
            var queryFilter = PredicateHelper.True<TransportTruckArrival>();
            if (filter.PoliceNumber == null)
                filter.PoliceNumber = "";
            queryFilter = queryFilter.And(p => p.PoliceNumber.Contains(filter.PoliceNumber));
            if (filter.GPNo == null)
                filter.GPNo = "";    
            queryFilter = queryFilter.And(p => p.GPNo.Contains(filter.GPNo));
            if(filter.ETACategory != "ALL")
                queryFilter = queryFilter.And(p => p.ETACategory == filter.ETACategory);
            foreach (TransportTruckArrival tta in Get(queryFilter).ToList())
            {
                //GPHeader gph = _context.GPHeaders.Where(x => x.GPNo == tta.GPNo).AsNoTracking().First();
                TransportTruckArrivalDTO temp = Mapper.Map<TransportTruckArrivalDTO>(tta);
                temp.FinishAddress = "";//
                listTemp.Add(temp);
            }
            return listTemp;
        }

        public void UpdateCalculate(TransportTruckArrival input, string userid)
        {
            TransportTruckArrival save = Get().FirstOrDefault(x => x.PoliceNumber == input.PoliceNumber);
            save.ETACalculate = input.ETACalculate;
            save.ETACategoryCalculate = input.ETACategoryCalculate;
            save.UpdatedDate = DateTime.Now;
            save.UpdatedBy = userid;
            Update(save);
            Save();
        }
    }
}
