using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Contracts;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.Transport.Repositories;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.BusinessLogics
{
    public class TransportDriverManagementBLL : ITransportDriverManagementBLL
    {
        private readonly IGenericRepository<TransportDriverManagement> _generalRepo;
        private readonly ITransportDriverManagementRepo _transportDriverManagementRepo;
        private readonly IGenericRepository<MasterVendor> _generalVendorRepo;
        private readonly IMasterVendorTOMRepo _masterVendorRepo;
        private readonly IMasterConfigurationRepo _masterConfigurationRepo;

        public TransportDriverManagementBLL(IGenericRepository<TransportDriverManagement> generalRepo, ITransportDriverManagementRepo transportDriverManagementRepo, IGenericRepository<MasterVendor> generalVendroRepo, IMasterVendorTOMRepo masterVendorRepo, IMasterConfigurationRepo masterConfigurationRepo)
        {
            _generalRepo = generalRepo;
            _transportDriverManagementRepo = transportDriverManagementRepo;
            _generalVendorRepo = generalVendroRepo;
            _masterVendorRepo = masterVendorRepo;
            _masterConfigurationRepo = masterConfigurationRepo;
        }

        public int SetInactive(TransportDriverManagementDTO input, string controller)
        {
            //return _customReportStateRepo.Insert(input, controller, userid);
            return _transportDriverManagementRepo.UpdateInactive(input, controller);
            //throw new NotImplementedException();
        }

        public TransportDriverManagementDTO GetById(string id)
        {
            var result = _generalRepo.Get(c => c.ID == id).FirstOrDefault() ;
            return Mapper.Map<TransportDriverManagementDTO>(result);
        }
        public string GetNameById(string id)
        {
            var result = _generalRepo.Get(c => c.ID == id).FirstOrDefault();
            return result == null ? null : result.Name;
        }
        public TransportDriverManagementDTO GetByIdAndVendor(string id, int vendorID)
        {
            var result = _generalRepo.Get(c => c.ID == id && c.IDVendor == vendorID);
            return Mapper.Map<TransportDriverManagementDTO>(result);
        }

        public TransportDriverManagementDTO CheckDriver(string idcard, int idvendor)
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();
            queryFilter = queryFilter.And(k => k.ID == idcard);
            queryFilter = queryFilter.And(k => k.IDVendor == idvendor);
            return Mapper.Map<TransportDriverManagement,TransportDriverManagementDTO>(_generalRepo.Get(queryFilter).FirstOrDefault());
        }

        public List<TransportDriverManagementDTO> getSearchResult(TransportDriverManagerSearchnput input)
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();
            if (!String.IsNullOrEmpty(input.ID))
            {
                queryFilter = queryFilter.And(k => k.ID == input.ID || k.DrivingLicenseNumber == input.ID);
            }
            if (!String.IsNullOrEmpty(input.IdVendor.IdVendor))
            {
                queryFilter = queryFilter.And(k => k.IDVendor.ToString() == input.IdVendor.IdVendor); // k.IDVendor.Contains(input.IdVendor.IdVendor));
            }
            if (!String.IsNullOrEmpty(input.BaseTown))
            {
                queryFilter = queryFilter.And(k => k.BaseTown == input.BaseTown);
            }
            if (!String.IsNullOrEmpty(input.Name.Name))
            {
                queryFilter = queryFilter.And(k => k.Name.Contains(input.Name.Name));
            }
            if (!String.IsNullOrEmpty(input.PerformanceLevel))
            {
                queryFilter = queryFilter.And(k => k.PerformanceLevel == input.PerformanceLevel);
            }
            
            if (input.IncompleteRequirement.IncompliteName == "BPJSKetenagaKerjaan")
            {
                queryFilter = queryFilter.And(k => k.BPJSKetenagaKerjaan == true);   
            }
            if (input.IncompleteRequirement.IncompliteName == "BPJSKesehatan")
            {
                queryFilter = queryFilter.And(k => k.BPJSKesehatan == true);
            }
            if (input.IncompleteRequirement.IncompliteName == "DrugFreeTest")
            {
                queryFilter = queryFilter.And(k => k.DrugFreeTest == true);
            }
            if (input.IncompleteRequirement.IncompliteName == "FatiqueTest")
            {
                queryFilter = queryFilter.And(k => k.FatiqueTest == true);
            }
            if (input.IncompleteRequirement.IncompliteName == "InductionTest")
            {
                queryFilter = queryFilter.And(k => k.InductionTest == true);
            }
            if (input.IncompleteRequirement.IncompliteName == "DefensiveDrivingTest")
            {
                queryFilter = queryFilter.And(k => k.DefensiveDrivingTest == true);
            }
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportDriverManagement>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<TransportDriverManagementDTO>>(dbResult);
        }

        public void SetActive(List<string> id, bool status)
        {
            _transportDriverManagementRepo.SetActive(id, status);
        }

        public List<TransportDriverManagementDTO> GetTransportDriverManagements(TransportDriverManagementInput input)
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();
            if(!String.IsNullOrEmpty(input.IncompleteRequirement))
            {
                string[] arrayIncomplete = input.IncompleteRequirement.Split(',');
                foreach (var s in arrayIncomplete)
                {
                    if (s == "BPJSKetenagaKerjaan")
                    {
                        queryFilter = queryFilter.And(k => !k.BPJSKetenagaKerjaan);
                    }
                    else if (s == "BPJSKesehatan")
                    {
                        queryFilter = queryFilter.And(k => !k.BPJSKesehatan);
                    }
                    else if (s == "DrugFreeTest")
                    {
                        queryFilter = queryFilter.And(k => !k.DrugFreeTest);
                    }
                    else if (s == "FatiqueTest")
                    {
                        queryFilter = queryFilter.And(k => !k.FatiqueTest);
                    }
                    else if (s == "InductionTest")
                    {
                        queryFilter = queryFilter.And(k => !k.InductionTest);
                    }
                    else if (s == "DefensiveDrivingTest")
                    {
                        queryFilter = queryFilter.And(k => !k.DefensiveDrivingTest);
                    }
                }
            }

            if (!String.IsNullOrEmpty(input.ID))
            {
                string[] namesArray = input.ID.Split(',');
                queryFilter = queryFilter.And(k => namesArray.Contains(k.ID));
            }
            if (!String.IsNullOrEmpty(input.Name))
            {
                string[] namesArray = input.Name.Split(',');
                queryFilter = queryFilter.And(k => namesArray.Contains(k.Name));
            }
            if (!String.IsNullOrEmpty(input.VendorID))
            {
                var convertedNumbers = input.VendorID.Split(',').Where(t => !string.IsNullOrWhiteSpace(t)).Select(m => Convert.ToInt32(m)).ToList();
                queryFilter = queryFilter.And(k => convertedNumbers.Contains(k.IDVendor));
            }
            if (!String.IsNullOrEmpty(input.BaseTown))
            {
                queryFilter = queryFilter.And(k => k.BaseTown == input.BaseTown);
            }
            if (!String.IsNullOrEmpty(input.PerformanceLevel))
            {
                queryFilter = queryFilter.And(k => k.PerformanceLevel == input.PerformanceLevel);
            }
            if (input.IsExportOrSearch) 
            {                
               if (input.IncludeInActive == "0")
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }
            //else
            //{
            //    queryFilter = queryFilter.And(k => k.IsActive);
            //}
            if (!String.IsNullOrWhiteSpace(input.RoleDriver))
            {
                var role = input.RoleDriver.ToLower().Trim();
                queryFilter = queryFilter.And(k => k.RoleDriver != null && k.RoleDriver.ToLower() == role);
            }
            if(input.UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                queryFilter = queryFilter.And(k => input.UserRole.Contains(k.MasterVendor.TransportationMode));
            }
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportDriverManagement>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<TransportDriverManagementDTO>>(dbResult);
        }
        #region new code Nicco
        public List<TransportDriverManagementDTO> GetTransportDriverManagements2(TransportDriverManagementInput input)
        {
            using (var context = new TOMContextDB())
            {
                var tplv = (from x in context.TransportDriverManagements
                            join y in context.MasterUsers on x.CreatedBy equals y.IDUser
                            join z in context.MasterVendors on x.IDVendor equals z.IDVendor
                            select new TransportDriverManagementDTO
                            {
                                ID = x.ID,
                                Name = x.Name,
                                DateOfBirth = x.DateOfBirth,
                                Gender = x.Gender,
                                Address = x.Address,
                                MobilePhone = x.MobilePhone,
                                IDVendor = x.IDVendor,
                                VendorName = z.VendorName,
                                JoinDate = x.JoinDate,
                                DrivingLicenseNumber = x.DrivingLicenseNumber,
                                DrivingLicensePeriod = x.DrivingLicensePeriod,
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
                                IsActive = x.IsActive,
                                CreatedByFullName = y.FullName,
                                CreatedDate = x.CreatedDate,
                                UpdatedBy = y.FullName,
                                UpdatedDate = x.UpdatedDate,
                                Remarks = x.Remarks,
                                RoleDriver = x.RoleDriver,
                                DriverContractValidityPeriod = x.DriverContractValidityPeriod,
                                //MasterVendor = x.MasterVendor,
                                //PrevIDCardNumber = x.PrevIDCardNumber,
                                //PrevIDVendor = x.PrevIDVendor,
                                //IsNewData = x.IsNewData,
                            }).ToList();

                if (!String.IsNullOrEmpty(input.IncompleteRequirement))
                {
                    string[] arrayIncomplete = input.IncompleteRequirement.Split(',');
                    foreach (var s in arrayIncomplete)
                    {
                        if (s == "BPJSKetenagaKerjaan")
                        {
                            tplv = tplv.Where(k => !k.BPJSKetenagaKerjaan).ToList();
                        }
                        else if (s == "BPJSKesehatan")
                        {
                            tplv = tplv.Where(k => !k.BPJSKesehatan).ToList();
                        }
                        else if (s == "DrugFreeTest")
                        {
                            tplv = tplv.Where(k => !k.DrugFreeTest).ToList();
                        }
                        else if (s == "FatiqueTest")
                        {
                            tplv = tplv.Where(k => !k.FatiqueTest).ToList();
                        }
                        else if (s == "InductionTest")
                        {
                            tplv = tplv.Where(k => !k.InductionTest).ToList();
                        }
                        else if (s == "DefensiveDrivingTest")
                        {
                            tplv = tplv.Where(k => !k.DefensiveDrivingTest).ToList();
                        }
                    }
                }


                if (!String.IsNullOrEmpty(input.ID))
                {
                    string[] namesArray = input.ID.Split(',');
                    tplv = tplv.Where(k => namesArray.Contains(k.ID)).ToList();
                }
                if (!String.IsNullOrEmpty(input.Name))
                {
                    string[] namesArray = input.Name.Split(',');
                    tplv = tplv.Where(k => namesArray.Contains(k.Name)).ToList();
                }
                if (!String.IsNullOrEmpty(input.VendorID))
                {
                    var convertedNumbers = input.VendorID.Split(',').Where(t => !string.IsNullOrWhiteSpace(t)).Select(m => Convert.ToInt32(m)).ToList();
                    tplv = tplv.Where(k => convertedNumbers.Contains(k.IDVendor)).ToList();
                }
                if (!String.IsNullOrEmpty(input.BaseTown))
                {
                    tplv = tplv.Where(k => k.BaseTown == input.BaseTown).ToList();
                }
                if (!String.IsNullOrEmpty(input.PerformanceLevel))
                {
                    tplv = tplv.Where(k => k.PerformanceLevel == input.PerformanceLevel).ToList();
                }
                if (input.IsExportOrSearch)
                {
                    if (input.IncludeInActive == "0")
                    {
                        tplv = tplv.Where(k => !k.IsActive).ToList();
                    }
                }
                if (!String.IsNullOrWhiteSpace(input.RoleDriver))
                {
                    var role = input.RoleDriver.ToLower().Trim();
                    tplv = tplv.Where(k => k.RoleDriver != null && k.RoleDriver.ToLower() == role).ToList();
                }
                if (input.UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
                {
                    tplv = tplv.Where(k => input.UserRole.Contains(k.MasterVendor.TransportationMode)).ToList();
                }

                return tplv;

            }
        }
        #endregion
        public dynamic GetTransportationDriverManagementsDataTable(TransportDriverManagementInput input, DataTableModel model = null)
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();
            //queryFilter = queryFilter.And(_ => _.IsActive == input.IsActive);
            if (!String.IsNullOrEmpty(input.IncompleteRequirement))
            {
                string[] arrayIncomplete = input.IncompleteRequirement.Split(',');
                foreach (var s in arrayIncomplete)
                {
                    if (s == "BPJSKetenagaKerjaan")
                    {
                        queryFilter = queryFilter.And(k => !k.BPJSKetenagaKerjaan);
                    }
                    else if (s == "BPJSKesehatan")
                    {
                        queryFilter = queryFilter.And(k => !k.BPJSKesehatan);
                    }
                    else if (s == "DrugFreeTest")
                    {
                        queryFilter = queryFilter.And(k => !k.DrugFreeTest);
                    }
                    else if (s == "FatiqueTest")
                    {
                        queryFilter = queryFilter.And(k => !k.FatiqueTest);
                    }
                    else if (s == "InductionTest")
                    {
                        queryFilter = queryFilter.And(k => !k.InductionTest);
                    }
                    else if (s == "DefensiveDrivingTest")
                    {
                        queryFilter = queryFilter.And(k => !k.DefensiveDrivingTest);
                    }
                }
            }

            if (!String.IsNullOrEmpty(input.ID))
            {
                string[] namesArray = input.ID.Split(',');
                queryFilter = queryFilter.And(k => namesArray.Contains(k.ID));
            }
            if (!String.IsNullOrEmpty(input.Name))
            {
                string[] namesArray = input.Name.Split(',');
                queryFilter = queryFilter.And(k => namesArray.Contains(k.Name));
            }
            if (!String.IsNullOrEmpty(input.VendorID))
            {
                var convertedNumbers = input.VendorID.Split(',').Where(t => !string.IsNullOrWhiteSpace(t)).Select(m => Convert.ToInt32(m)).ToList();
                queryFilter = queryFilter.And(k => convertedNumbers.Contains(k.IDVendor));
            }
            if (!String.IsNullOrEmpty(input.BaseTown))
            {
                queryFilter = queryFilter.And(k => k.BaseTown == input.BaseTown);
            }
            if (!String.IsNullOrEmpty(input.PerformanceLevel))
            {
                queryFilter = queryFilter.And(k => k.PerformanceLevel == input.PerformanceLevel);
            }
            if (input.IsExportOrSearch)
            {
                if (input.IncludeInActive == "0")
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }
            //else
            //{
            //    queryFilter = queryFilter.And(k => k.IsActive);
            //}
            if (!String.IsNullOrWhiteSpace(input.RoleDriver))
            {
                var role = input.RoleDriver.ToLower().Trim();
                queryFilter = queryFilter.And(k => k.RoleDriver != null && k.RoleDriver.ToLower() == role);
            }
            if (input.UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                queryFilter = queryFilter.And(k => input.UserRole.Contains(k.MasterVendor.TransportationMode));
            }
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportDriverManagement>();

            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.search.value))
                {
                    var search = model.search.value;
                    if (search != "")
                    {
                        queryFilter = queryFilter.And(w => (
                            search.Contains(w.Name) ||
                            search.Contains(w.MasterVendor.VendorName) ||
                            search.Contains(w.BaseTown) ||
                            search.Contains(w.DrivingLicenseNumber) ||
                            search.Contains(w.DrivingLicensePeriod.ToString()) ||
                            search.Contains(w.IsActive.ToString()) ||
                            search.Contains(w.PerformanceLevel)
                          ));
                    }
                }

                foreach (var fil in model.columns.Where(_ => !String.IsNullOrEmpty(_.search.value)))
                {
                    var _col = fil.data;
                    var _val = fil.search.value;
                    switch (_col)
                    {
                        case "Name":
                            queryFilter = queryFilter.And(_ => _.Name.Contains(_val));
                            break;
                        case "IDVendor":
                            queryFilter = queryFilter.And(c => c.MasterVendor.VendorName.Contains(_val));
                            break;
                        case "BaseTown":
                            queryFilter = queryFilter.And(_ => _.BaseTown.Contains(_val));
                            break;
                        case "DrivingLicenseNumber":
                            queryFilter = queryFilter.And(_ => _.DrivingLicenseNumber.Contains(_val));
                            break;
                        case "DrivingLicensePeriod":
                            queryFilter = queryFilter.And(_ => _.DrivingLicensePeriod.ToString().Contains(_val));
                            break;
                        case "IsActive":
                            queryFilter = queryFilter.And(_ => _.IsActive.ToString().Contains(_val));
                            break;
                        case "PerformanceLevel":
                            queryFilter = queryFilter.And(_ => _.PerformanceLevel.Contains(_val));
                            break;
                    }
                }
            }

            var dbResult = Enumerable.Empty<TransportDriverManagement>();
            if (model == null)
            {
                dbResult = _generalRepo.Get(queryFilter);
            }
            else
            {
                dbResult = _generalRepo.GetAllPagination(queryFilter, model.start, model.length, "ID");
            }
            var count = _generalRepo.Count(queryFilter);
            //var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            dynamic res = new System.Dynamic.ExpandoObject();
            var Data = Mapper.Map<List<TransportDriverManagementDTO>>(dbResult);
            res.data = Data;
            res.total = count;
            return res;

        }

        public int CountTransportDriverManagements(TransportDriverManagementInput input, string criteria)
        {
            var queryFilter = PredicateHelper.True<TransportDriverManagement>();

            queryFilter = queryFilter.And(k => k.IsActive == true);

            if (criteria == "driversWithExpiredDriverLicense")
            {
                queryFilter = queryFilter.And(k => k.DrivingLicensePeriod < DateTime.Today);
            }
            if (criteria == "driversWithMore5YearsExperience")
            {
                DateTime today = DateTime.Today;
                var fiveYearsAgo = today.AddYears(-5);

                queryFilter = queryFilter.And(k => k.JoinDate < fiveYearsAgo);
            }
            if (criteria == "driversWithoutDrugTest")
            {
                queryFilter = queryFilter.And(k => k.DrugFreeTest == false);
            }
            if (criteria == "driversWithoutFatigueTest")
            {
                queryFilter = queryFilter.And(k => k.FatiqueTest == false);
            }
            if (criteria == "driversWithoutInductionTest")
            {
                queryFilter = queryFilter.And(k => k.InductionTest == false);
            }
            if (criteria == "driversWithoutDefensiveDriversTest")
            {
                queryFilter = queryFilter.And(k => k.DefensiveDrivingTest == false);
            }
            if (criteria == "driversWithoutBpjsKetenagakerjaan")
            {
                queryFilter = queryFilter.And(k => k.BPJSKetenagaKerjaan == false);
            }
            if (criteria == "driversWithoutBpjsKesehatan")
            {
                queryFilter = queryFilter.And(k => k.BPJSKesehatan == false);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportDriverManagement>();

            return _generalRepo.Get(queryFilter, orderByFilter).Count();
        }

        public int UpdateTransportDriverManagement(TransportDriverManagementInput input, string controller, string userid)
        {
            return 0; //_transportDriverManagementRepo.Update(input, controller, userid);
        }

        public List<TransportDriverManagementDTO> exportToExcel()
        {
            return _transportDriverManagementRepo.GetRecordsForExcel();
        }

        public int Save(TransportDriverManagementDTO input, string controller, string userid)
        {
            try
            {
                return _transportDriverManagementRepo.Update(input, controller, userid);
                
            }
            catch (ExceptionBase ex)
            {
                var message = ex;
                return 0;
            }
            
        }

        public TransportDriverManagementDTO GetByIdCardNumberAndIDVendor(string idCardNumber, int idVendor)
        {
            var filter = PredicateHelper.True<TransportDriverManagement>()
                .And(x => x.ID.Equals(idCardNumber, StringComparison.InvariantCultureIgnoreCase))
                .And(x => x.IDVendor == idVendor);
            var result = _generalRepo.Get(filter);
            return Mapper.Map<TransportDriverManagementDTO>(result);
        }

        public List<TransportDriverManagementDTO> GetDataByCriteria(TransportDriverManagementInput criteria)
        {
            var filter = PredicateHelper.True<TransportDriverManagement>();
            
            if (criteria.IDVendor != null)
            {
                filter = filter.And(f => f.IDVendor == criteria.IDVendor);
            }

            var result = _generalRepo.Get(filter);
            return Mapper.Map<List<TransportDriverManagementDTO>>(result);
        }
        public List<MasterVendorTOMDTO> GetMasterVendor(List<UserRole> userRole)
        {
            if (userRole.Count == 1 && userRole[0].RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR))
            {
                List<MasterConfigurationDTO> tempConfig = Mapper.Map<List<MasterConfigurationDTO>>(_masterConfigurationRepo.GetListMasterConfigurationByPageNameDescription(EnumHelper.GetDescription(Enums.MasterConfigurationValue.RoleVendorMapping), EnumHelper.GetDescription(Enums.RoleUserList.AWR)));
                if (tempConfig.Count > 1)
                    return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorByListVendorName(tempConfig.Select(x => x.Value).ToList()));
            }
            return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorActiveNoChild());
        }
    }
}