using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using DFIS.Utils;
using TOM.Master.Domain.Inputs;
using System.Diagnostics;

namespace TOM.Master.Repositories
{
    public class MasterCostRepo : TOMGenericRepository<MasterCost>, IMasterCostRepo
    {
        public MasterCostRepo(TOMContextDB contextEntities) : base(contextEntities)
        { 
        }

        public List<MasterCost> SelectAllCostByListCostType(List<string> costType)
        {
            var queryFilter = PredicateHelper.True<MasterCost>();
            queryFilter = queryFilter.And(x => costType.Contains(x.CostType));
            return Get(queryFilter).ToList();
        }

        public List<MasterCostDTO> ListMasterCostWithLatestEffectiveStartDate(string type)
        {
             TOMContextDB context = new TOMContextDB();
            List<MasterCostLatestEffectiveDateDataView> c = (from x in context.MasterCostLatestEffectiveDateDataViews
                where x.CostType == type
                select x).ToList();
            return Mapper.Map<List<MasterCostLatestEffectiveDateDataView>,List<MasterCostDTO>>(c);
        }

        public bool CheckUpdateEffectiveDate(DateTime startDate, DateTime? endDate, string costType, int vendorName, string receiver)
        {
            var queryFilter = PredicateHelper.True<MasterCost>();
            //queryFilter = queryFilter.And(x => x.EffectiveEndDate == null);
            if (receiver != null)
                queryFilter = queryFilter.And(x => x.ReceiverIDLocation == receiver);
            queryFilter = queryFilter.And(x => x.CostType == costType);
            queryFilter = queryFilter.And(x => x.IDVendor == vendorName);
            queryFilter = queryFilter.And(x => x.EffectiveStartDate <= startDate && x.EffectiveEndDate >= startDate);
            queryFilter = queryFilter.Or(x => x.EffectiveStartDate <= endDate && x.EffectiveEndDate >= endDate);
            MasterCost temp = Get(queryFilter).OrderByDescending(x => x.IDCost).FirstOrDefault();
            if (temp != null)
                return false;
            return true;
        }

        public string UpdateLastEffectiveDate(DateTime startDate, DateTime? endDate, string costType, int vendorName, string receiver, string lastUpdateBy)
        {
            var queryFilter = PredicateHelper.True<MasterCost>();
            //queryFilter = queryFilter.And(x => x.EffectiveEndDate == null);
            if (receiver != null)
                queryFilter = queryFilter.And(x => x.ReceiverIDLocation == receiver);
            queryFilter = queryFilter.And(x => x.CostType == costType);
            queryFilter = queryFilter.And(x => x.IDVendor == vendorName);
            queryFilter = queryFilter.And(x => x.EffectiveStartDate < startDate && x.EffectiveEndDate >= startDate);
            MasterCost temp = Get(queryFilter).OrderByDescending(x => x.IDCost).FirstOrDefault();
            if (temp != null)
            {
                var queryFilter2 = PredicateHelper.True<MasterCost>();
                if (receiver != null)
                    queryFilter2 = queryFilter2.And(x => x.ReceiverIDLocation == receiver);
                queryFilter2 = queryFilter2.And(x => x.CostType == costType);
                queryFilter2 = queryFilter2.And(x => x.IDVendor == vendorName);
                queryFilter2 = queryFilter2.And(x => x.EffectiveEndDate >= startDate);
                List<MasterCost> temp2 = Get(queryFilter2).ToList();
                if (temp2.Count > 1) //ada data yang bersinggungan, dan bukan data yang paling baru
                {
                    return "1";
                }
                //ada data yang bersinggungan, dan datanya adalah data terbaru. maka update end datenya
                temp.EffectiveEndDate = startDate.Date.AddDays(-1);
                temp.UpdatedDate = DateTime.Now;
                temp.UpdatedBy = lastUpdateBy;
                Update(temp);
                Save();
                return "0";
            }
            //data yang effective start date tidak bersinggungan
            //ngecek apakah effective end datenya bersinggungan dengan effective start date data yang lain
            var queryFilter3 = PredicateHelper.True<MasterCost>();
            if (receiver != null)
                queryFilter3 = queryFilter3.And(x => x.ReceiverIDLocation == receiver);
            queryFilter3 = queryFilter3.And(x => x.CostType == costType);
            queryFilter3 = queryFilter3.And(x => x.IDVendor == vendorName);
            queryFilter3 = queryFilter3.And(x => x.EffectiveStartDate <= endDate);
            queryFilter3 = queryFilter3.And(x => x.EffectiveStartDate > startDate);
            List<MasterCost> temp3 = Get(queryFilter3).OrderBy(x => x.EffectiveStartDate).ToList();
            if (temp3.Count > 0)//jika ada, maka ambil effective start date yang paling lama(paling mendekati end date)
            {
                return temp3[0].EffectiveStartDate.Date.AddDays(-1).ToString();//kemudian set end date H-1 dari start date
            }
            //jika tidak ada, save seperti biasa
            return "0";
        }

        public int SaveData(MasterCost input, bool status)
        {
            int result = -1;
            List<MasterCost> SavedData = new List<MasterCost>();
            List<MasterCost> UpdatedData = new List<MasterCost>();

            if (status)
            {
                var queryFilter = PredicateHelper.True<MasterCost>();
                queryFilter = queryFilter.And(x => x.CostType == input.CostType);
                queryFilter = queryFilter.And(x => x.IDVendor == input.IDVendor);
                List<MasterCost> similarDatas = Get(queryFilter).OrderByDescending(x => x.IDCost).ToList();
                if (similarDatas.Count() > 0)
                {
                    foreach (var data in similarDatas)
                    {
                        //var queryFilter2 = PredicateHelper.True<MasterCost>();
                        //queryFilter2 = queryFilter2.And(x => x.CostType == input.CostType);
                        //queryFilter2 = queryFilter2.And(x => x.IDVendor == input.IDVendor);
                        //queryFilter2 = queryFilter2.Where(x => x.EffectiveStartDate == input.EffectiveStartDate);
                        //MasterCost rejectData = Get(queryFilter2).OrderByDescending(x => x.IDCost).FirstOrDefault();
                        //if (rejectData == null)
                        if (data.EffectiveStartDate != input.EffectiveStartDate)
                        {
                            // cek if new start date < list data
                            //var queryFilter3 = PredicateHelper.True<MasterCost>();
                            //queryFilter3 = queryFilter3.And(x => x.EffectiveStartDate > input.EffectiveStartDate);
                            //MasterCost dataExist = Get(queryFilter3).OrderBy(x => x.EffectiveStartDate).FirstOrDefault();
                            //if (dataExist != null)
                            if (data.EffectiveStartDate > input.EffectiveStartDate)
                            {
                                //Debug.WriteLine("ID exist:" + dataExist.IDCost);
                                if (data.EffectiveEndDate > input.EffectiveEndDate && data.EffectiveStartDate > input.EffectiveEndDate)
                                {
                                    input.CreatedDate = DateTime.Now;
                                    input.UpdatedDate = DateTime.Now;
                                    SavedData.Add(input);
                                    //Insert(input);
                                    //Save();
                                    if (result <= 0)
                                    {
                                        result = 0;
                                    }
                                }
                                else
                                {
                                    result = 1;
                                }
                            }
                            else
                            {
                                // cek if new start date between from list data
                                //var queryFilter4 = PredicateHelper.True<MasterCost>();
                                //queryFilter4 = queryFilter4.And(x => x.CostType == input.CostType);
                                //queryFilter4 = queryFilter4.And(x => x.IDVendor == input.IDVendor);
                                //queryFilter4 = queryFilter4.And(x => x.EffectiveStartDate < input.EffectiveStartDate && x.EffectiveEndDate >= input.EffectiveStartDate
                                //&& x.EffectiveEndDate == new DateTime(2999, 12, 31) && x.EffectiveEndDate == input.EffectiveEndDate);
                                //MasterCost dataExist2 = Get(queryFilter4).OrderBy(x => x.EffectiveStartDate).FirstOrDefault();
                                //if (dataExist2 != null)
                                if (data.EffectiveStartDate < input.EffectiveStartDate && data.EffectiveEndDate >= input.EffectiveStartDate
                                    && data.EffectiveEndDate == new DateTime(2999, 12, 31) && data.EffectiveEndDate == input.EffectiveEndDate)
                                {
                                    // update existing data
                                    //Debug.WriteLine("ID Exist:"+dataExist2.IDCost);
                                    data.EffectiveEndDate = input.EffectiveStartDate.AddDays(-1);
                                    data.UpdatedDate = DateTime.Now;
                                    data.UpdatedBy = input.UpdatedBy;
                                    UpdatedData.Add(data);
                                    //Update(data);
                                    input.CreatedDate = DateTime.Now;
                                    input.UpdatedDate = DateTime.Now;
                                    SavedData.Add(input);
                                    //Insert(input);
                                    //Save();
                                    if (result <= 0)
                                    {
                                        result = 0;
                                    }
                                }
                                else if (data.EffectiveEndDate < input.EffectiveStartDate)
                                {
                                    input.CreatedDate = DateTime.Now;
                                    input.UpdatedDate = DateTime.Now;
                                    SavedData.Add(input);
                                    //Insert(input);
                                    //Save();
                                    if (result <= 0)
                                    {
                                        result = 0;
                                    }
                                }
                                else
                                {
                                    result = 2;
                                }
                            }
                        }
                        else
                        {
                            result = 1;
                        }
                    }
                }
                else
                {
                    input.CreatedDate = DateTime.Now;
                    input.UpdatedDate = DateTime.Now;
                    SavedData.Add(input);
                    //Insert(input);
                    //Save();
                    if (result <= 0)
                    {
                        result = 0;
                    }
                }

                /*string temp = UpdateLastEffectiveDate(input.EffectiveStartDate, input.EffectiveEndDate, input.CostType, input.VendorName, input.Receiver, input.UpdatedBy);
                DateTime tempDate;
                if (temp == "1")
                {
                    return 1;
                }
                if (DateTime.TryParse(temp, out tempDate))
                {
                    input.EffectiveEndDate = DateTime.Parse(temp);
                }
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
                Save();
                return 0;*/
            }
            else
            {
                var queryFilter = PredicateHelper.True<MasterCost>();
                queryFilter = queryFilter.And(x => x.CostType == input.CostType);
                queryFilter = queryFilter.And(x => x.IDVendor == input.IDVendor);
                queryFilter = queryFilter.And(x => x.IDCost != input.IDCost);
                List<MasterCost> similarDatas = Get(queryFilter).OrderByDescending(x => x.IDCost).ToList();
                var queryFilter2 = PredicateHelper.True<MasterCost>();
                queryFilter2 = queryFilter2.And(x => x.CostType == input.CostType);
                queryFilter2 = queryFilter2.And(x => x.IDVendor == input.IDVendor);
                queryFilter2 = queryFilter2.And(x => x.IDCost == input.IDCost);
                MasterCost updatedData = Get(queryFilter2).OrderByDescending(x => x.IDCost).FirstOrDefault();
                if (similarDatas.Count() > 0)
                {
                    foreach (var data in similarDatas)
                    {
                        if (data.EffectiveStartDate != input.EffectiveStartDate)
                        {
                            // cek if new start date < list data
                            if (data.EffectiveStartDate > input.EffectiveStartDate)
                            {
                                //Debug.WriteLine("ID exist:" + dataExist.IDCost);
                                if (data.EffectiveEndDate > input.EffectiveEndDate && data.EffectiveStartDate > input.EffectiveEndDate)
                                {
                                    //input.UpdatedDate = DateTime.Now;
                                    //input.IDVendor = updatedData.IDVendor;
                                    //input.ReceiverIDLocation = updatedData.ReceiverIDLocation;
                                    //input.SenderIDLocation = updatedData.SenderIDLocation;
                                    //input.ThroughIDLocation = updatedData.ThroughIDLocation;
                                    //UpdatedData.Add(input);
                                    updatedData.UpdatedDate = DateTime.Now;
                                    updatedData.AdditionalUnitPrice = input.AdditionalUnitPrice;
                                    updatedData.BasedPrice = input.BasedPrice;
                                    updatedData.DiscountPrice = input.DiscountPrice;
                                    updatedData.EffectiveEndDate = input.EffectiveEndDate;
                                    updatedData.EffectiveStartDate = input.EffectiveStartDate;
                                    updatedData.MinimumBox = input.MinimumBox;
                                    updatedData.MinimumKM = input.MinimumKM;
                                    updatedData.OrderType = input.OrderType;
                                    updatedData.Remarks = input.Remarks;
                                    updatedData.Via = input.Via;
                                    UpdatedData.Add(updatedData);
                                    if (result <= 0)
                                    {
                                        result = 0;
                                    }
                                }
                                else
                                {
                                    result = 1;
                                }
                            }
                            else
                            {
                                // cek if new start date between from list data
                                if (data.EffectiveStartDate < input.EffectiveStartDate && data.EffectiveEndDate >= input.EffectiveStartDate
                                    && data.EffectiveEndDate == new DateTime(2999, 12, 31) && data.EffectiveEndDate == input.EffectiveEndDate)
                                {
                                    // update existing data
                                    //Debug.WriteLine("ID Exist:"+dataExist2.IDCost);
                                    data.EffectiveEndDate = input.EffectiveStartDate.AddDays(-1);
                                    data.UpdatedDate = DateTime.Now;
                                    data.UpdatedBy = input.UpdatedBy;
                                    UpdatedData.Add(data);
                                    //input.UpdatedDate = DateTime.Now;
                                    //input.IDVendor = updatedData.IDVendor;
                                    //input.ReceiverIDLocation = updatedData.ReceiverIDLocation;
                                    //input.SenderIDLocation = updatedData.SenderIDLocation;
                                    //input.ThroughIDLocation = updatedData.ThroughIDLocation;
                                    //UpdatedData.Add(input);
                                    updatedData.UpdatedDate = DateTime.Now;
                                    updatedData.AdditionalUnitPrice = input.AdditionalUnitPrice;
                                    updatedData.BasedPrice = input.BasedPrice;
                                    updatedData.DiscountPrice = input.DiscountPrice;
                                    updatedData.EffectiveEndDate = input.EffectiveEndDate;
                                    updatedData.EffectiveStartDate = input.EffectiveStartDate;
                                    updatedData.MinimumBox = input.MinimumBox;
                                    updatedData.MinimumKM = input.MinimumKM;
                                    updatedData.OrderType = input.OrderType;
                                    updatedData.Remarks = input.Remarks;
                                    updatedData.Via = input.Via;
                                    UpdatedData.Add(updatedData);
                                    if (result <= 0)
                                    {
                                        result = 0;
                                    }
                                }
                                else if (data.EffectiveEndDate < input.EffectiveStartDate)
                                {
                                    //input.UpdatedDate = DateTime.Now;
                                    //input.IDVendor = updatedData.IDVendor;
                                    //input.ReceiverIDLocation = updatedData.ReceiverIDLocation;
                                    //input.SenderIDLocation = updatedData.SenderIDLocation;
                                    //input.ThroughIDLocation = updatedData.ThroughIDLocation;
                                    //UpdatedData.Add(input);
                                    updatedData.UpdatedDate = DateTime.Now;
                                    updatedData.AdditionalUnitPrice = input.AdditionalUnitPrice;
                                    updatedData.BasedPrice = input.BasedPrice;
                                    updatedData.DiscountPrice = input.DiscountPrice;
                                    updatedData.EffectiveEndDate = input.EffectiveEndDate;
                                    updatedData.EffectiveStartDate = input.EffectiveStartDate;
                                    updatedData.MinimumBox = input.MinimumBox;
                                    updatedData.MinimumKM = input.MinimumKM;
                                    updatedData.OrderType = input.OrderType;
                                    updatedData.Remarks = input.Remarks;
                                    updatedData.Via = input.Via;
                                    UpdatedData.Add(updatedData);
                                    if (result <= 0)
                                    {
                                        result = 0;
                                    }
                                }
                                else
                                {
                                    result = 2;
                                }
                            }
                        }
                        else
                        {
                            result = 1;
                        }
                    }
                }
                else
                {
                    //input.UpdatedDate = DateTime.Now;
                    //input.IDVendor = updatedData.IDVendor;
                    //input.ReceiverIDLocation = updatedData.ReceiverIDLocation;
                    //input.SenderIDLocation = updatedData.SenderIDLocation;
                    //input.ThroughIDLocation = updatedData.ThroughIDLocation;
                    updatedData.UpdatedDate = DateTime.Now;
                    updatedData.AdditionalUnitPrice = input.AdditionalUnitPrice;
                    updatedData.BasedPrice = input.BasedPrice;
                    updatedData.DiscountPrice = input.DiscountPrice;
                    updatedData.EffectiveEndDate = input.EffectiveEndDate;
                    updatedData.EffectiveStartDate = input.EffectiveStartDate;
                    updatedData.MinimumBox = input.MinimumBox;
                    updatedData.MinimumKM = input.MinimumKM;
                    updatedData.OrderType = input.OrderType;
                    updatedData.Remarks = input.Remarks;
                    updatedData.Via = input.Via;
                    UpdatedData.Add(updatedData);
                    if (result <= 0)
                    {
                        result = 0;
                    }
                }
            }

            /*if (CheckUpdateEffectiveDate(input.EffectiveStartDate, input.EffectiveEndDate, input.CostType, input.VendorName, input.Receiver))
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
                Save();
                return 0;
            }*/
            if (result == 0)
            {
                foreach (var data in UpdatedData)
                {
                    Update(data);
                }
                foreach (var data in SavedData)
                {
                    Insert(data);
                }
            }
            return result;//error pada saat update data, dan start / end date bersinggungan dengan data yang sudah ada
        }

        public int CommitSave()
        {
            try
            {
                Save();
            }
            catch(Exception e)
            {
                return 3;
            }
            return 0;
        }

        public void SaveDataUpload(MasterCost input, bool status)
        {
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
                Save();
            }
            else { 
                input.UpdatedDate = DateTime.Now;
                Update(input);
                Save();
            }
        }
        public List<MasterCost> GetAllMasterCost(MasterCostInput input)
        {
            var queryFilter = PredicateHelper.True<MasterCost>();
            if (!string.IsNullOrEmpty(input.filterDate))
            {
                DateTime filterDate = Convert.ToDateTime(input.filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveStartDate <= filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveEndDate >= filterDate);
            }
            if (input.filterSenderLocation != null && !String.IsNullOrEmpty(input.filterSenderLocation[0]))
            {
                queryFilter = queryFilter.And(x => input.filterSenderLocation.Contains(x.SenderIDLocation));
            }
            queryFilter = queryFilter.And(m => m.IsActive == true);
            return Get(queryFilter).ToList();
        }
    }
}
