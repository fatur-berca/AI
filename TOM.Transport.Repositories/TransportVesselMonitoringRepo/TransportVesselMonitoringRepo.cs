using System;
using System.Linq;
using DFIS.Utils;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories.TransportVesselMonitoringRepo
{
    public class TransportVesselMonitoringRepo : TOMGenericRepository<TransportVesselMonitoring>, ITransportVesselMonitoringRepo
    {
        private TOMContextDB _context;

        public TransportVesselMonitoringRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }
        
        public TransportVesselMonitoring GetTransportVesselMonitoringByIDTransactionExecution(int idtransexe)
	{
            var queryFilter = PredicateHelper.True<TransportVesselMonitoring>();
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idtransexe);
            return Get(queryFilter).FirstOrDefault();
        }
   
        public int UpdateVessel(TransportMonitoringViewInput input, string userid)
        {
            TOMContextDB dbContext = new TOMContextDB();

            TransportVesselMonitoring dbres = dbContext.TransportVesselMonitorings.FirstOrDefault(x => x.IDTransportVesselMonitoring == input.IDTransportVesselMonitoring && x.IDTransportExecution == input.IDTransportExecution);
            var getConfig = dbContext.MasterConfigurations.FirstOrDefault(x => x.PageName.Contains("VesselMonitoringDetail") && x.Description.Contains("est. received") && x.IsActive);
            var config = getConfig != null ? Convert.ToInt16(getConfig.Value) : 0;

            if (dbres != null)
            {
                if (input.ETD2 != null && input.ETD2.Value.Year != 1) input.ETA2 = dbres.ETA1 != null ? dbres.ETA1 + (input.ETD2 - dbres.ETD1) : null;
                dbres.ETD2 = input.ETD2;
                dbres.ETA2 = input.ETA2;
                dbres.ATD = input.ATD;
                dbres.ATA = input.ATA;
                dbres.ActualTimeBerthing = input.ActualTimeBerthing;
                dbres.EstReceived = input.ETA2.HasValue ? input.ETA2.Value.AddDays(config) : (input.ETA1.HasValue ? input.ETA1.Value.AddDays(config) : (DateTime?) null);
                dbres.Remarks = input.Remarks;

                dbres.UpdatedBy = userid;
                dbres.UpdatedDate = DateTime.Now;
            }

            return dbContext.SaveChanges();
        }

        //public int UpdateOrder(TransportMonitoringViewInput input, string userid)
        //{
        //    //using (var dbContext = new TOMContextDB())

        //    TOMContextDB dbContext = new TOMContextDB();
        //    //{
        //    TransportOrder dbres = dbContext.TransportOrders.FirstOrDefault(x => x.STONo == input.STONo && x.IDTransportExecution == input.IDTransportExecution);

        //    if (dbres != null)
        //    {
        //        dbres.OrderStatus = input.OrderStatus;

        //        dbres.UpdatedBy = userid;
        //        dbres.UpdatedDate = DateTime.Now;
        //    }

        //    return dbContext.SaveChanges();
        //    //}
        //}

        public TransportVesselMonitoring GetVesselMonitoringByidTE(int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportVesselMonitoring>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idTE);
            return Get(queryFilter).FirstOrDefault();
        }

        public void SaveData(TransportVesselMonitoring input, bool status)
        {
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();
        }
    }
}
