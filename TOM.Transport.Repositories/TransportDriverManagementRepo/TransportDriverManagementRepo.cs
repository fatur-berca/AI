using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using DFIS.Utils;
using TOM.EntitiesDAL;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public class TransportDriverManagementRepo : TOMGenericRepository<TransportDriverManagement>, ITransportDriverManagementRepo
    {
        private TOMContextDB _context;
        public TransportDriverManagementRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }
        public int UpdateInactive(TransportDriverManagementDTO input, string controller)
        {
            var context = new TOMContextDB();
            TransportDriverManagement dbRes = context.TransportDriverManagements.FirstOrDefault(x => x.ID == input.ID);
            try
            {
                dbRes.ID = input.ID;
                dbRes.IsActive = input.IsActive;
                dbRes.CreatedBy = input.CreatedBy;
                dbRes.CreatedDate = input.CreatedDate;
                dbRes.UpdatedBy = input.UpdatedBy;
                dbRes.UpdatedDate = input.UpdatedDate;
            }
            catch (Exception ex)
            {
                var message = ex;
            }
            return context.SaveChanges();
        }

        public int Update(TransportDriverManagementDTO input, string controller, string userid)
        {
            TOMContextDB context = new TOMContextDB();
            //if(input.PrevIDCardNumber == input.ID)
            TransportDriverManagement dbRes = context.TransportDriverManagements.FirstOrDefault(x => x.ID == input.ID && x.IDVendor == input.IDVendor);
            try
            {
                // update
                if (dbRes != null)
                {
                    dbRes.ID = input.ID;
                    dbRes.Name = input.Name;
                    dbRes.DateOfBirth = input.DateOfBirth;
                    dbRes.Gender = input.Gender;
                    dbRes.Address = input.Address;
                    dbRes.MobilePhone = input.MobilePhone;
                    dbRes.IDVendor = input.IDVendor;
                    dbRes.JoinDate = input.JoinDate;
                    dbRes.DrivingLicenseNumber = input.DrivingLicenseNumber;
                    dbRes.DrivingLicensePeriod = input.DrivingLicensePeriod;
                    dbRes.BaseTown = input.BaseTown;
                    dbRes.PerformanceLevel = input.PerformanceLevel;
                    dbRes.AttachmentKTP = input.AttachmentKTP;
                    dbRes.AttachmentSIM = input.AttachmentSIM;
                    dbRes.AttachmentFoto = input.AttachmentFoto;
                    dbRes.BPJSKetenagaKerjaan = input.BPJSKetenagaKerjaan;
                    dbRes.BPJSKesehatan = input.BPJSKesehatan;
                    dbRes.DrugFreeTest = input.DrugFreeTest;
                    dbRes.FatiqueTest = input.FatiqueTest;
                    dbRes.InductionTest = input.InductionTest;
                    dbRes.DefensiveDrivingTest = input.DefensiveDrivingTest;
                    dbRes.IsActive = true;
                    dbRes.Remarks = input.Remarks;
                    dbRes.UpdatedBy = userid;
                    dbRes.UpdatedDate = DateTime.Now;
                    dbRes.RoleDriver = input.RoleDriver;
                    dbRes.DriverContractValidityPeriod = input.DriverContractValidityPeriod;
                }
                else // save
                {
                    TransportDriverManagement inputTransportDriverManagement = new TransportDriverManagement();

                    inputTransportDriverManagement.ID = GetAutoNumberId();
                    inputTransportDriverManagement.Name = input.Name;
                    inputTransportDriverManagement.DateOfBirth = input.DateOfBirth;
                    inputTransportDriverManagement.Gender = input.Gender;
                    inputTransportDriverManagement.Address = input.Address;
                    inputTransportDriverManagement.MobilePhone = input.MobilePhone;
                    inputTransportDriverManagement.IDVendor = input.IDVendor;
                    inputTransportDriverManagement.JoinDate = input.JoinDate;
                    inputTransportDriverManagement.DrivingLicenseNumber = input.DrivingLicenseNumber;
                    inputTransportDriverManagement.DrivingLicensePeriod = input.DrivingLicensePeriod;
                    inputTransportDriverManagement.BaseTown = input.BaseTown;
                    inputTransportDriverManagement.PerformanceLevel = input.PerformanceLevel;
                    inputTransportDriverManagement.AttachmentKTP = input.AttachmentKTP;
                    inputTransportDriverManagement.AttachmentSIM = input.AttachmentSIM;
                    inputTransportDriverManagement.AttachmentFoto = input.AttachmentFoto;
                    inputTransportDriverManagement.BPJSKetenagaKerjaan = input.BPJSKetenagaKerjaan;
                    inputTransportDriverManagement.BPJSKesehatan = input.BPJSKesehatan;
                    inputTransportDriverManagement.DrugFreeTest = input.DrugFreeTest;
                    inputTransportDriverManagement.FatiqueTest = input.FatiqueTest;
                    inputTransportDriverManagement.InductionTest = input.InductionTest;
                    inputTransportDriverManagement.DefensiveDrivingTest = input.DefensiveDrivingTest;
                    inputTransportDriverManagement.IsActive = true;
                    inputTransportDriverManagement.CreatedBy = userid;
                    inputTransportDriverManagement.CreatedDate = DateTime.Now;
                    inputTransportDriverManagement.UpdatedBy = userid;
                    inputTransportDriverManagement.UpdatedDate = DateTime.Now;
                    inputTransportDriverManagement.Remarks = input.Remarks;
                    inputTransportDriverManagement.RoleDriver = input.RoleDriver;
                    inputTransportDriverManagement.DriverContractValidityPeriod = input.DriverContractValidityPeriod;

                    context.TransportDriverManagements.Add(inputTransportDriverManagement);
                    if (input.PrevIDCardNumber != null || input.PrevIDVendor != null)
                    {
                        if (input.PrevIDCardNumber != input.ID || input.PrevIDVendor != input.IDVendor)
                        {
                            context.TransportDriverManagements.Remove(
                                context.TransportDriverManagements.FirstOrDefault(
                                    x => x.ID == input.PrevIDCardNumber || x.IDVendor == input.PrevIDVendor));
                        }    
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var message = ex;
            }

            return context.SaveChanges();
        }

        public List<TransportDriverManagementDTO> GetRecordsForExcel()
        {
            TOMContextDB Context = new TOMContextDB();

            List<TransportDriverManagement> Records = Context.TransportDriverManagements.SqlQuery("SELECT * FROM [DFIS].[dbo].[TransportDriverManagement]", '1').ToList();

            return Records.Select(x => new TransportDriverManagementDTO {
                ID = x.ID,
                Name = x.Name,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                Address = x.Address,
                MobilePhone = x.MobilePhone,
                IDVendor =  x.IDVendor,
                JoinDate = x.JoinDate,
                DrivingLicenseNumber = x.DrivingLicenseNumber,
                DrivingLicensePeriod = x.DrivingLicensePeriod.Value,
                BaseTown = x.BaseTown,
                PerformanceLevel = x.PerformanceLevel,
                AttachmentKTP = x.AttachmentKTP,
                AttachmentSIM = x.AttachmentSIM,
                AttachmentFoto = x.AttachmentFoto,
                BPJSKetenagaKerjaan = x.BPJSKetenagaKerjaan,
                BPJSKesehatan = x.BPJSKesehatan,
                DrugFreeTest = x.DrugFreeTest,
                FatiqueTest = x.FatiqueTest,
                InductionTest = x.InductionTest,
                DefensiveDrivingTest = x.DefensiveDrivingTest,
                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                Remarks = x.Remarks }).ToList();
        }

        public void SetActive(List<string> parameters, bool status)
        {
            var context = new TOMContextDB();
            List<string> ids = new List<string>();
            List<string> idVendors = new List<string>();
            foreach(var param in parameters)
            {
                string id = param.Split('&')[0];
                string vendor = param.Split('&')[1];
                ids.Add(id);
                idVendors.Add(vendor);
            }

            #warning [Migration] ID Vendor types are different !!
            var dbResult = context.TransportDriverManagements.Where(x => ids.Contains(x.ID) && idVendors.Contains(x.IDVendor.ToString())).ToList();
            foreach (var transportDriverManagement in dbResult)
            {
                transportDriverManagement.IsActive = status;
            }
            context.SaveChanges();

        }

        public TransportDriverManagement GetTransportDriverManagement(string IDCard)
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.ID == IDCard);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<TransportDriverManagement> GetALLTransportDriverManagementActiveList()
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();
            queryFilter = queryFilter.And(x => x.IsActive);
            return Get(queryFilter).ToList();
        }

        public List<TransportDriverManagement> GetALLTransportDriverManagementList()
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();
            return Get(queryFilter).ToList();
        }

        public List<TransportDriverManagement> GetDataByCriteriaActive(TransportDriverManagementInput criteria)
        {
            var filter = PredicateHelper.True<TransportDriverManagement>();
            filter = filter.And(f => f.IsActive);
            if (criteria.IDVendor != null)
            {
                filter = filter.And(f => f.IDVendor == criteria.IDVendor);
            }

            return Get(filter).ToList();
        }

        public string GetAutoNumberId()
        {
            var today = DateTime.Now;

            var data = GetALLTransportDriverManagementList().Where(x => x.CreatedDate.Year == today.Year && x.CreatedDate.Month == today.Month && x.CreatedDate.Day == today.Day).Count();

            var count = 1;

            if(data > 0)
            {
                count = data + 1;
            }

            string id = string.Format("{0}{1}{2}", "DM", today.ToString("yyyyMMdd"), count.ToString().PadLeft(4, '0'));

            return id;
        }
    }
}