using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DFIS.EntitiesDAL.EDMX;
//using DFIS.Transport.Domain.Inputs;
//using DFIS.Transport.Repositories.TransportLostClaimDamageRepo;
using DFIS.Utils;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Inputs;
using LinqKit;
using TOM.Transport.Domain.DTOs;
using AutoMapper;

namespace TOM.Transport.Repositories.TransportLostClaimDamageRepo
{
    public class TransportLostClaimDamageRepo : ITransportLostClaimDamageRepo
    {
        public IEnumerable<string> GetListOriginWarehouse()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.MasterLocations.AsNoTracking().
                    Where(c => (c.Type.ToUpper() == "WAREHOUSE" || c.Type.ToUpper() == "FACTORY" || c.Type.ToUpper() == "TRANSPORT" || c.Type.ToUpper() == "AGENT" || c.Type == "OTHER")
                        && (c.IsAssigned == true) && (c.IsActive == true)).
                    Select(c => new { Loc = c.IDLocation + " - " + c.LocationName }).
                    Distinct().
                    Select(c => c.Loc).ToList();
            }
        }

        public IEnumerable<string> GetListDestinationWarehouse()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.MasterLocations.AsNoTracking().
                    Where(c => (c.Type.ToUpper() == "WAREHOUSE" || c.Type.ToUpper() == "FACTORY" || c.Type.ToUpper() == "TRANSPORT" || c.Type.ToUpper() == "AGENT" || c.Type == "OTHER")
                        && (c.IsAssigned == true) && (c.IsActive == true)).
                    Select(c => new { Loc = c.IDLocation + " - " + c.LocationName }).
                    Distinct().
                    Select(c => c.Loc).ToList();
            }
        }
        
        // Get Transport Number / STO Repository
        public List<string> GetListSTONumber(string transno)
        {
            using (var dbContext = new TOMContextDB())
            {
                var temp = dbContext.TransportLostClaimDamages.AsNoTracking().
                    Where(x => x.IsActive && x.SJNumber.Contains(transno)).Select(y => y.SJNumber).Distinct().ToList();
                //var temp = Mapper.Map<List<TransportLostClaimDamage>>(
                //        from tc in dbContext.TransportLostClaimDamages
                //        where tc.IsActive && tc.STONumber.Contains(transno)
                //        select( new { tc.GPNumber })
                //    ).
                //    OrderBy(y => y.DateOfIncident).ToList();
                
                return temp;
                // return dbContext.TransportLostClaimDamages.AsNoTracking().Where(c => c.STONumber != null).Select(c => c.STONumber).Distinct().ToList();
            }
        }

        public IEnumerable<string> GetListDeliveryNoteNumber()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.TransportLostClaimDamages.AsNoTracking().Where(c => c.DeliveryNumberNote != null).Select(c => c.DeliveryNumberNote).Distinct().ToList();
            }
        }

        public IEnumerable<string> GetListPoliceRegNumber()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.TransportLostClaimDamages.AsNoTracking().Where(c => c.PoliceRegNumber != null).Select(c => c.PoliceRegNumber).Distinct().ToList();
            }
        }

        public IEnumerable<string> GetListVendor()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.MasterVendors.AsNoTracking().Where(c => c.IDVendor != null).Select(c => c.VendorName).Distinct().ToList();
            }
        }

        public TransportLostClaimDamage GetTransportLostClaimDamage(string GPNumber, string SJNumber)
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.TransportLostClaimDamages.Find(GPNumber, SJNumber);
            }
        }

        public IEnumerable<string> GetListClaimType()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.MasterLists.Where(c => c.FieldName == "ClaimType").Select(c => c.FieldValue).ToList();
            }
        }

        public IEnumerable<string> GetListClaimExpense()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.MasterLists.Where(c => c.FieldName == "ClaimExpense").Select(c => c.FieldValue).ToList();
            }
        }

        public IEnumerable<string> GetListClaimCategory()
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.MasterLists.Where(c => c.FieldName == "ClaimCategory").Select(c => c.FieldValue).ToList();
            }
        }

        public IEnumerable<string> GetListSJNumber()
        {
            var dateMonthAgo = DateTime.Now.AddMonths(-2);
            using (var dbContext = new TOMContextDB())
            {
                var dbResult = from s in dbContext.GPDetails
                               where !dbContext.TransportLostClaimDamages.Any(c => (c.SJNumber == s.SJNo)) && (s.SJDate >= dateMonthAgo)
                               select s.SJNo;

                return dbResult.ToList().Distinct();
            }
        }

        //public IEnumerable<string> GetListGPNumber()
        //{
        //    using (var dbContext = new TOMContextDB())
        //    {
        //        var dbResult = from s in dbContext.GPHeaders
        //               where !dbContext.TransportLostClaimDamages.Any(c => (c.GPNumber == s.GPNo))
        //               select s.GPNo;

        //        return dbResult.ToList().Distinct();
        //    }
        //}

        public IEnumerable<TransportLostClaimDamage> GetListDefault()
        {
            var oneMonthAgo = DateTime.Now.AddMonths(-1);

            using (var dbContext = new TOMContextDB())
            {
                return dbContext.TransportLostClaimDamages.AsNoTracking().Where(c => c.DateOfIncident >= oneMonthAgo && c.IsActive == true).Distinct().ToList();
            }
        }

        public IEnumerable<TransportLostClaimDamage> GetListTransportLostClaimDamage(TransportLostClaimDamageInput input)
        {
            DateTime dateReceiptFrom = input.DateReceiptFrom == null || input.DateReceiptFrom == "" ? DateTime.Now.Date : DateTime.ParseExact(input.DateReceiptFrom, "d/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //DateTime dateReceiptFrom = DateTime.ParseExact(input.DateReceiptFrom, "d/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dateReceiptTo = input.DateReceiptTo == null || input.DateReceiptTo == "" ? DateTime.Now.Date : DateTime.ParseExact(input.DateReceiptTo, "d/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            string[] origWH = String.IsNullOrEmpty(input.OriginWarehouse) ? new string[0] : input.OriginWarehouse.Split(',');
            string[] destWH = String.IsNullOrEmpty(input.DestinationWarehouse) ? new string[0] : input.DestinationWarehouse.Split(',');
            string[] stoNum = String.IsNullOrEmpty(input.STONumber) ? new string[0] : input.STONumber.Split(',');
            string[] delvNote = String.IsNullOrEmpty(input.DeliveryNoteNumber) ? new string[0] : input.DeliveryNoteNumber.Split(',');
            string[] polRegNum = String.IsNullOrEmpty(input.PoliceRegNumber) ? new string[0] : input.PoliceRegNumber.Split(',');
            string[] vendor = String.IsNullOrEmpty(input.Vendor) ? new string[0] : input.Vendor.Split(',');

            using (var dbContext = new TOMContextDB())
            {
                var queryFilter = PredicateBuilder.New<TransportLostClaimDamage>();

                if (!string.IsNullOrEmpty(input.DateReceiptTo)) queryFilter.And(c => c.DateOfIncident <= dateReceiptTo);

                if (!string.IsNullOrEmpty(input.DateReceiptFrom)) queryFilter.And(c => c.DateOfIncident >= dateReceiptFrom);
                //queryFilter.And(c => c.DateOfIncident >= dateReceiptFrom && c.DateOfIncident <= dateReceiptTo);

                if (origWH.Any()) queryFilter = queryFilter.And(c => origWH.Contains(c.OriginWarehouse));

                if (destWH.Any()) queryFilter = queryFilter.And(c => destWH.Contains(c.DestinationWarehouse));

                if (stoNum.Any()) queryFilter = queryFilter.And(c => stoNum.Contains(c.STONumber));

                if (delvNote.Any()) queryFilter = queryFilter.And(c => delvNote.Contains(c.DeliveryNumberNote));

                if (polRegNum.Any()) queryFilter = queryFilter.And(c => polRegNum.Contains(c.PoliceRegNumber));

                if (vendor.Any())
                {
                    var listVendorID = dbContext.MasterVendors.Where(c => vendor.Contains(c.VendorName)).Select(c => c.IDVendor).ToList();
                    queryFilter = queryFilter.And(c => listVendorID.Contains(c.TransportationVendor.Value));
                }

                //if (!input.IncludeDeleted) queryFilter = queryFilter.And(c => c.IsActive);
                if (String.IsNullOrEmpty(input.IncludeInActive))
                {
                    queryFilter = queryFilter.And(c => c.IsActive);
                }
                else if (input.IncludeInActive == "0")
                {
                    queryFilter = queryFilter.And(c => !c.IsActive);
                }

                var dbresult = dbContext.TransportLostClaimDamages.AsNoTracking().AsExpandable()
                        .Where(queryFilter).Distinct().ToList();
                return dbresult;
            }
        }

        public IEnumerable<TransportLostClaimFACode> GetListTransportLostClaimFACode(string GPNumber, string SJNumber)
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.TransportLostClaimFACodes.Where(c => c.GPNumber == GPNumber && c.SJNumber == SJNumber).ToList();
            }
        }

        public IEnumerable<TransportLostClaimUploadBox> GetListTransportLostClaimUploadBoxes(string GPNumber, string SJNumber)
        {
            using (var dbContext = new TOMContextDB())
            {
                return dbContext.TransportLostClaimUploadBoxes.Where(c => c.GPNumber == GPNumber && c.SJNumber == SJNumber).ToList();
            }
        }

        public void SetActive(List<string> id, bool status)
        {
            using (var dbContext = new TOMContextDB())
            {
                for (var i = 0; i < id.Count; i++)
                {
                    var listGPNumber = id[i].Split('-')[0];
                    var listSJNumber = id[i].Split('-')[1];

                    var dbResult = dbContext.TransportLostClaimDamages.Where(x => listGPNumber.Contains(x.GPNumber) && listSJNumber.Contains(x.SJNumber)).ToList();

                    foreach (var data in dbResult)
                    {
                        data.IsActive = status;
                    }
                    dbContext.SaveChanges();
                }
            }
        }

        public TransportLostClaimDamageDTO GetNewDataDetail(string SJNumber)
        {
            var result = new TransportLostClaimDamageDTO();
            using (var dbContext = new TOMContextDB())
            {
                //var dbResultGPDetail = dbContext.GPDetails.AsNoTracking().FirstOrDefault(c => c.SJNo == SJNumber);
                var dbResultTO = dbContext.TransportOrders.AsNoTracking().FirstOrDefault(c => c.STONo == SJNumber);
                var dbResultMstLoc = dbContext.MasterLocations.AsNoTracking().ToList();

                if (dbResultTO != null)
                {
                    var transportExecution = dbResultTO.TransportExecution;

                    if (transportExecution != null)
                    {
                        if (transportExecution.MasterVendor != null)
                        {
                            result.TransportationVendor = transportExecution.MasterVendor.IDVendor;
                            result.TransportationVendorName = transportExecution.MasterVendor.VendorName;
                        }
                        else
                        {
                            result.TransportationVendor = null;
                            result.TransportationVendorName = "";
                        }
                        result.TransportationMode = transportExecution.TransportMode;
                        result.PoliceRegNumber = transportExecution.PoliceRegNo;
                        result.GPNumber = transportExecution.TransportNo;
                        result.DateOfDelivery = transportExecution.TransportDate;
                    }

                    result.OriginWarehouse = dbResultMstLoc.Where(x => x.IDLocation == dbResultTO.ActualSenderIDLocation).Select(x => x.LocationName).FirstOrDefault();
                    result.DestinationWarehouse = dbResultMstLoc.Where(x => x.IDLocation == dbResultTO.ActualReceiverIDLocation).Select(x => x.LocationName).FirstOrDefault();

                    result.SJDate = dbResultTO.ShipmentDate;
                    result.STONumber = SJNumber;
                    //try
                    //{
                    //    var masterLoc = dbContext.MasterLocations.AsNoTracking().ToList();
                    //    var origWH = masterLoc.FirstOrDefault(c => c.IDLocation == originWH);
                    //    var destiWH = masterLoc.FirstOrDefault(c => c.IDLocation == destWH);
                    //    result.OriginWarehouse = origWH == null ? "" : origWH.IDLocation + " - " + origWH.LocationName;
                    //    result.DestinationWarehouse = destiWH == null ? "" : destiWH.IDLocation + " - " + destiWH.LocationName;
                    //}
                    //catch (Exception)
                    //{
                    //    throw new Exception("Warehouse does not exist in Master Location");
                    //}

                }
            }

            return result;
        }
    }
}
