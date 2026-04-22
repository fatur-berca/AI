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

namespace TOM.Transport.Repositories.TransportExecutionRepo
{
    public class TransportExecutionRepo : TOMGenericRepository<TransportExecution>, ITransportExecutionRepo
    {
        private TOMContextDB _context;

        public TransportExecutionRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }

        public List<TransportExecution> GetALLTransportExecutionActive()
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            return Get(queryFilter).ToList();
        }

        public TransportExecution GetTransportExecutionByID(int idexe)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idexe);
            return Get(queryFilter).FirstOrDefault();
        }

        public TransportExecution GetTransportExecutionActiveByID(int idexe)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idexe);
            return Get(queryFilter).FirstOrDefault();
        }

        public void RefreshTransportStatus(int idExecution)
        {
            List<string> _statuses = new List<string>() {
                            "In Process",
                            "On Delivery",
                            "Arrive At Destination and Waiting For Confirmation",
                            "Complete"
                        };
            var ctx = new TOMContextDB();
            var te = ctx.TransportExecutions.Where(dte => dte.IDTransportExecution == idExecution).FirstOrDefault();
            if (te != null && te.TransportOrders != null)
            {
                var stat = te.TransportOrders.Where(to => to.IsActive).Select(to => to.OrderStatus).Distinct().Select(st => _statuses.Contains(st) ? _statuses.IndexOf(st) : -1).Where(i => i >= 0).Distinct();
                if (stat.Count() > 0)
                {
                    var nstat = _statuses[stat.Min()];
                    te.TransportStatus = nstat;
                    ctx.SaveChanges();
                }
            }
        }

        public TransportExecution GetTransportExecutionByTN(string TN)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportNo == TN);
            //queryFilter = queryFilter.And(x => x.SIType == filter.SIType);
            //queryFilter = queryFilter.And(x => x.SIStatus == filter.SIStatus);
            //queryFilter = queryFilter.And(x => x.TargetOfArrival == filter.TargetOfArrival);
            return Get(queryFilter).FirstOrDefault();
        }

        public TransportExecution GetTransportExecutionByIDDriver(string idDriver, int idtranexe)
        {
            string TransportStatus = EnumHelper.GetDescription(Enums.TransportationStatus.Complete);
            string TransportStatus2 = EnumHelper.GetDescription(Enums.TransportationStatus.Close);
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDTransportExecution != idtranexe);
            queryFilter = queryFilter.And(x => x.IDDriver1 == idDriver || x.IDDriver2 == idDriver || x.IDCoDriver == idDriver);
            queryFilter = queryFilter.And(x => x.TransportStatus != TransportStatus && x.TransportStatus != TransportStatus2);
            return Get(queryFilter).OrderByDescending(x => x.TransportDate).FirstOrDefault();
        }

        public List<TransportExecution> GetTransportationNumberFilterByUserLocations(string transno, string UserId)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportNo.Contains(transno));

            TOMContextDB ctx = new TOMContextDB();

            var UserLocations = ctx.MasterUserLocationMappings.
                                Where(f => f.IDUser.ToLower() == UserId && f.IsActive).
                                Select(f => f.IDLocation);

            var temp = Mapper.Map<List<TransportExecution>>(
                           from te in ctx.TransportExecutions
                           join ul in ctx.MasterUserLocationMappings on te.CreatedBy.ToLower() equals ul.IDUser
                           where te.IsActive && te.TransportNo.Contains(transno) && UserLocations.Contains(ul.IDLocation)
                           group te by new { te.IDTransportExecution }
                           into g
                           select g.FirstOrDefault()
                       ).OrderByDescending(f => f.TransportDate).ToList();

            /*
            List<TransportExecution> temp = Get(0, 10, queryFilter)
                .Join(_context.MasterUserLocationMappings, to => to.CreatedBy.ToLower(), ul => ul.IDLocation, (to, ul) => new { to, ul })
                .Where(f => UserLocations.Contains(f.ul.IDLocation))
                .Select(f => f.to)
                .OrderByDescending(x => x.TransportDate).ToList();
            */
            return temp;
        }

        public IEnumerable<object> GetTNCreatorShipping(string tncreator)
        {
            TOMContextDB ctx = new TOMContextDB();

            var getData = from te in ctx.TransportExecutions
                          join mu in ctx.MasterUsers on te.CreatedBy.ToLower() equals mu.IDUser
                          where te.CreatedBy.ToLower().Contains(tncreator.ToLower()) || mu.FullName.ToLower().Contains(tncreator.ToLower())
                          group mu by new { mu.IDUser }
                          into g
                          select new { g.FirstOrDefault().IDUser, g.FirstOrDefault().FullName };

            var result = getData.AsEnumerable().Take(10);

            return result;
        }

        public List<TransportExecution> GetTransportExecutionFilter(string stono)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.TransportNo.Contains(stono));
            //return Get(0, 10, queryFilter).OrderByDescending(x => x.TransportDate).ToList();
            return Get(queryFilter).OrderByDescending(x => x.TransportDate).Skip(0).Take(10).ToList();
        }
        public List<TransportExecution> GetListTransportExecutionByID(int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            //queryFilter = queryFilter.And(x => idTE.Contains(x.IDTransportExecution));
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idTE);
            return Get(queryFilter).OrderByDescending(x => x.TransportDate).ToList();
        }

        public List<FN_TRANSPORT_EXECUTION_LIST_ADD_Result> GetTransportExecutionAddNew(TransportOrderInput input)
        {
            string orderType = string.Join(",", input.orderTypeListFilter);
            string sender = string.Join(",", input.senderListFilter);
            var result = _context.FN_TRANSPORT_EXECUTION_LIST_ADD(input.executionTypeFilter, input.zoneFilter, orderType, input.weekFilter, "", "ALL", input.yearFilter).ToList();
            if (input.vehicleTypeFilterA != null && input.vehicleTypeFilterA.Where(c => !string.IsNullOrWhiteSpace(c)).Count() > 0)
            {
                result = result.Where(c => input.vehicleTypeFilterA.Contains(c.OrderedVehicleType)).ToList();
            }
            if (input.executionTypeFilter == "With TN")
                result = result.OrderBy(x => String.IsNullOrEmpty(x.VendorName) ? "" : x.VendorName).ToList();

            if (input.senderListFilter != null && input.senderListFilter.Where(c => !string.IsNullOrWhiteSpace(c)).Count() > 0)
            {
                var senderList = input.senderListFilter.ToArray();
                if (input.executionTypeFilter.ToLower() == "synergy")
                {
                    result = result.Where(re => result.Where(r => r.IDCheck == re.IDCheck && senderList.Contains(r.IDSenderLoc)).Count() > 0).ToList();
                }
                else
                {
                    result = result.Where(re => senderList.Contains(re.IDSenderLoc)).ToList();
                }
            }

            if (input.executionTypeFilter == "With TN")
            {
                var subres = new List<FN_TRANSPORT_EXECUTION_LIST_ADD_Result>();
                FN_TRANSPORT_EXECUTION_LIST_ADD_Result prvData = null;
                foreach (var data in result)
                {
                    if (prvData != null)
                    {
                        if (prvData.TransportationNumber != data.TransportationNumber)
                        {
                            prvData = data;
                        }
                        else
                        {
                            if (!data.IDVendor.HasValue)
                            {
                                continue;
                            }
                        }
                    }
                    else
                        prvData = data;
                    subres.Add(data);
                }
                return subres;
            }

            return result;

        }

        public void CalculateCost(string idTransportExecution, string userid)
        {
            _context.Database.CommandTimeout = 5000;
            _context.CalculateCost(idTransportExecution, userid, 0);
        }

        public void CalculateCost(string idTransportExecution, string userid, int recalculate)
        {
            _context.Database.CommandTimeout = 5000;
            _context.CalculateCost(idTransportExecution, userid, recalculate);
        }

        public void CalculateLoadFactorCFP(int idTransportExecution)
        {
            _context.CalculateLoadFactorCFP(idTransportExecution);
        }

        public List<string> GenerateTransportationNumber(List<TransportExecutionDTO> saveList, List<TransportExecutionAddNewDTO> toList, Hashtable HashSeq)
        {
            List<string> errorList = new List<string>();
            using (TOMContextDB entities = new TOMContextDB())
            {
                entities.Database.CommandTimeout = 5000;
                using (DbContextTransaction scope = entities.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    //Lock the table during this transaction
                    //entities.Database.ExecuteSqlCommand("SELECT TOP 1 IDTransportExecution FROM TransportExecution WITH (TABLOCKX, UPDLOCK)");
                    //entities.Database.ExecuteSqlCommand("SELECT TOP 1 IDTransportOrder FROM TransportOrder WITH (TABLOCKX, UPDLOCK)");

                    entities.Database.ExecuteSqlCommand("SELECT null as dummy FROM TransportOrder WITH (tablockx, holdlock)");
                    entities.Database.ExecuteSqlCommand("SELECT null as dummy FROM TransportExecution WITH (tablockx, holdlock)");
                    int maxNumber;
                    foreach (TransportExecutionDTO tempTransExe in saveList)
                    {
                        TransportExecution temp = new TransportExecution();
                        if (tempTransExe.TransportNo.Contains("E-"))
                        {
                            temp = entities.TransportExecutions
                                .Where(x => x.SIWeek == tempTransExe.SIWeek && x.TransportDate.Year == tempTransExe.TransportDate.Year && x.TransportNo.Contains("E-"))
                                .OrderByDescending(x => x.TransportNo)
                                .FirstOrDefault();
                        }
                        else
                            temp = entities.TransportExecutions
                                .Where(x => x.TransportDate.Month == tempTransExe.TransportDate.Month && x.TransportDate.Year == tempTransExe.TransportDate.Year && (!x.TransportNo.Contains("E-")))
                                .OrderByDescending(x => x.TransportNo).FirstOrDefault();
                        if (temp != null)
                        {
                            string tnMax = temp.TransportNo;
                            maxNumber = Int32.Parse(tnMax.StartsWith("E") ? tnMax.Substring(10) : tnMax.Substring(8));
                        }
                        else
                            maxNumber = 0;

                        maxNumber++;
                        TransportExecution save = Mapper.Map<TransportExecutionDTO, TransportExecution>(tempTransExe);
                        save.TransportNo = save.TransportNo + maxNumber.ToString().PadLeft(5, '0');
                        save.CreatedDate = DateTime.Now;
                        save.UpdatedDate = DateTime.Now;
                        entities.TransportExecutions.Add(save);
                        //entities.SaveChanges();

                        foreach (TransportExecutionAddNewDTO tempTo in toList.Where(x => x.IDCheck == tempTransExe.IDCheckSave).GroupBy(x => x.IDTransportOrder).Select(x => x.FirstOrDefault()))
                        {
                            if (tempTo != null)
                            {
                                TransportOrder tempUpdateTo = entities.TransportOrders.FirstOrDefault(x => x.IDTransportOrder == tempTo.IDTransportOrder && x.IDTransportExecution == null);
                                if (tempUpdateTo == null)
                                {
                                    string error = "Exists-" + tempTo.OrderNumber;
                                    errorList.Add(error);
                                }
                                else
                                {
                                    if (errorList.Count < 1)
                                    {
                                        //tempUpdateTo.IDTransportExecution = save.IDTransportExecution;
                                        tempUpdateTo.TransportExecution = save;
                                        tempUpdateTo.KM = Convert.ToDecimal(HashSeq[tempUpdateTo.IDTransportOrder]);
                                        tempUpdateTo.OrderStatus = "In Process";
                                        tempUpdateTo.OrderCategory = "Main Order";
                                        entities.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    //Complete the scope here to commit, otherwise it will rollback
                    //The table lock will be released after we exit the TransactionScope block
                    scope.Commit();
                }
            }
            return errorList;
        }

        public int SaveData(TransportExecution input)
        {
            input.UpdatedDate = DateTime.Now;
            Update(input);
            Save();
            return 0;
        }

        public TransportExecutionPrintMemoDTO GetTransportExecutionByidTE(int idTE)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            //queryFilter = queryFilter.And(x => idTE.Contains(x.IDTransportExecution));
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idTE);
            return ReturnListObjectWithJoinView(queryFilter);
        }
        public TransportExecutionPrintMemoDTO ReturnListObjectWithJoinView(Expression<Func<TransportExecution, bool>> filter)
        {
            return Get(filter)
                .GroupJoin(_context.TransportVesselMonitorings, te => te.IDTransportExecution, tvm => tvm.IDTransportExecution, (te, tvm) => new { te, tvm })
                .GroupJoin(_context.MasterVendors, te2 => te2.te.IDVendor, mv => mv.IDVendor, (te2, mv) => new { te2.te, te2.tvm, mv })
                .Select(x => new TransportExecutionPrintMemoDTO(x.te, x.mv.ToList(), x.tvm.ToList())).FirstOrDefault();
        }
        /*public TransportExecutionPrintMemoDTO ReturnListObjectWithJoinView(Expression<Func<TransportExecution, bool>> filter)
        {
            return Get(filter)
                .GroupJoin(_context.TransportVesselMonitorings, te => te.IDTransportExecution, tvm => tvm.IDTransportExecution, (te, tvm) => new { te, tvm })
                .Join(_context.MasterVendors, te2 => te2.te.IDVendor, mv => mv.IDVendor, (te2, mv) => new { te2.te, te2.tvm, mv })
                .Select(x => new TransportExecutionPrintMemoDTO(x.te, x.mv, x.tvm.ToList())).FirstOrDefault();
        }*/
        public List<TransportExecutionVendorDTO> GetTotalVendorTE(List<int?> idTE)
        {
            var queryFilter = PredicateHelper.True<TransportExecution>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => idTE.Contains(x.IDTransportExecution));
            return Get(queryFilter)
            .GroupBy(x => new { x.IDVendor })
            .Select(x => new TransportExecutionVendorDTO()
            {
                IDVendor = x.Key.IDVendor,
                TotalVendor = x.DefaultIfEmpty().Count(),
            })
            .Join(_context.MasterVendors, te => te.IDVendor, mv => mv.IDVendor, (te, mv) => new { te, mv })
            .Select(x => new TransportExecutionVendorDTO(x.te, x.mv)).ToList();
        }

        public IEnumerable<FnTransportationSummaryCRate_Result> GetCrashRate(string year)
        {
            var dbResult = _context.FnTransportationSummaryCRate(year);
            return dbResult;
        }

        //digunakan untuk filter list transport execution
        public List<string> GetDistinctUserByRegional(List<string> regional)
        {
            return _context.MasterUserLocationMappings
                .Join(_context.MasterLocations, mulm => mulm.IDLocation, ml => ml.IDLocation, (mulm, ml) => new { mulm, ml })
                .Where(x => regional.Contains(x.ml.ParentLocation) && x.ml.Type == "Warehouse")
                .Select(x => x.mulm.IDUser).Distinct().ToList();
        }
    }
}
