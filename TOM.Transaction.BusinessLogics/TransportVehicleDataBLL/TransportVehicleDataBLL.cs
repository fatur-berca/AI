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
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using TOM.Transport.Repositories;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.BusinessLogics
{
    public class TransportVehicleDataBLL : ITransportVehicleDataBLL
    {
        private readonly IGenericRepository<TransportVehicleData> _generalRepo;
        private readonly ITransportVehicleDataRepo _transportVehicleDataRepo;
        private static TransportVehicleData _tempAttachment = new TransportVehicleData();
        private readonly IGenericRepository<MasterVendor> _generalVendorRepo;
        private readonly IMasterVendorTOMRepo _masterVendorRepo;
        private readonly IMasterConfigurationRepo _masterConfigurationRepo;

        public TransportVehicleDataBLL(IGenericRepository<TransportVehicleData> generalRepo, ITransportVehicleDataRepo transportVehicleDataRepo, IGenericRepository<MasterVendor> generalVendroRepo, IMasterVendorTOMRepo masterVendorRepo, IMasterConfigurationRepo masterConfigurationRepo)
        {
            _generalRepo = generalRepo;
            _transportVehicleDataRepo = transportVehicleDataRepo;
            _generalVendorRepo = generalVendroRepo;
            _masterVendorRepo = masterVendorRepo;
            _masterConfigurationRepo = masterConfigurationRepo;
        }

        public TransportVehicleDataDTO GetById(string id)
        {
            var result = _generalRepo.GetByID(id);

            return Mapper.Map<TransportVehicleDataDTO>(result);
        }

        public List<TransportVehicleDataDTO> GetDataByCriteria(TransportVehicleDataInput criteria)
        {
            var queryFilter = PredicateHelper.True<TransportVehicleData>();

           if (criteria.IDVendor > 0)
           {
                queryFilter = queryFilter.And(f => f.IDVendor == criteria.IDVendor);
           }

            var dbResult = _generalRepo.Get(queryFilter).ToList();

            return Mapper.Map<List<TransportVehicleDataDTO>>(dbResult);
        }

        public List<MasterVendorTOMDTO> GetMasterVendor(List<UserRole> userRole)
        {
            if (userRole.Count == 1 && userRole[0].RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR))
            {
                List<MasterConfigurationDTO> tempConfig = Mapper.Map<List<MasterConfigurationDTO>>(_masterConfigurationRepo.GetListMasterConfigurationByPageNameDescription(EnumHelper.GetDescription(Enums.MasterConfigurationValue.RoleVendorMapping), EnumHelper.GetDescription(Enums.RoleUserList.AWR)));
                if (tempConfig.Count > 1)
                    return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorByListVendorName(tempConfig.Select(x => x.Value).ToList()));
            }
            else if (userRole.Count == 1 && userRole[0].RoleName.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorActiveNoChild().Where(x => userRole[0].RoleName.ToLower().Contains(x.TransportationMode.ToLower())).ToList());
            }
            return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorActiveNoChild());
        }

        public List<TransportVehicleDataDTO> GetTransportVehicleDatas(TransportVehicleDataInput input)
        {

            var queryFilter = PredicateHelper.True<TransportVehicleData>();

            //if (!String.IsNullOrEmpty(input.IncludeInActive) && input.IncludeInActive == "1")
            //{
            //    queryFilter = queryFilter.And(k => k.IsActive);
            //}
            if (input.IncludeInActive == "0")
            {
                queryFilter = queryFilter.And(k => !k.IsActive);
            }

            if (!String.IsNullOrEmpty(input.IDPoliceRegNumber))
            {
                queryFilter = queryFilter.And(k => k.IDPoliceRegNumber == input.IDPoliceRegNumber);
            }

            if (!String.IsNullOrEmpty(input.IDPoliceRegNumbers))
            {
                string[] theIDPoliceRegNumbers = input.IDPoliceRegNumbers.Split(',');
                queryFilter = queryFilter.And(k => theIDPoliceRegNumbers.Contains(k.IDPoliceRegNumber));
            }

            //if (!String.IsNullOrEmpty(input.ManufacturingYears))
            //{
            //    queryFilter = queryFilter.And(k => k.ManufacturingYear == Convert.ToInt32(input.ManufacturingYears));
            //}

            if (!String.IsNullOrEmpty(input.ManufacturingYears))
            {
                int[] ManufacturingYears = input.ManufacturingYears
                    .Split(',').Select(number => int.Parse(number))
                    .ToArray();
                queryFilter = queryFilter.And(k => ManufacturingYears.Contains(k.ManufacturingYear.Value));
            }

            //if (!String.IsNullOrEmpty(input.ManufacturingYears))
            //{
            //    string[] theYears = input.ManufacturingYears.Split(',');
            //    foreach (string theYear in theYears)
            //    {
            //        int year = Int32.Parse(theYear);
            //        queryFilter = queryFilter.And(k => k.ManufacturingYear == year);
            //    }
            //}

            if (!String.IsNullOrEmpty(input.Karoseri))
            {
                queryFilter = queryFilter.And(k => k.Karoseri == input.Karoseri);
            }
            if (!String.IsNullOrEmpty(input.Merk))
            {
                queryFilter = queryFilter.And(k => k.Merk == input.Merk);
            }
            if (!String.IsNullOrEmpty(input.Type))
            {
                queryFilter = queryFilter.And(k => k.Type == input.Type);
            }
            if (!String.IsNullOrEmpty(input.Model))
            {
                queryFilter = queryFilter.And(k => k.Model == input.Model);
            }
            if (!String.IsNullOrEmpty(input.Cargo))
            {
                queryFilter = queryFilter.And(k => k.Cargo == input.Cargo);
            }
            if (!String.IsNullOrEmpty(input.VehicleIdentityNumber))
            {
                queryFilter = queryFilter.And(k => k.VehicleIdentityNumber == input.VehicleIdentityNumber);
            }
            if (!String.IsNullOrEmpty(input.EngineNumber))
            {
                queryFilter = queryFilter.And(k => k.EngineNumber == input.EngineNumber);
            }
            if (!String.IsNullOrEmpty(input.BaseTown))
            {
                queryFilter = queryFilter.And(k => k.BaseTown == input.BaseTown);
            }
            if (!String.IsNullOrEmpty(input.GPS))
            {
                queryFilter = queryFilter.And(k => k.GPS == input.GPS);
            }
            if (!String.IsNullOrEmpty(input.Status))
            {
                queryFilter = queryFilter.Or(k => k.Status == input.Status);
            }
            if (!String.IsNullOrEmpty(input.AttachmentSTNK))
            {
                queryFilter = queryFilter.And(k => k.AttachmentSTNK == input.AttachmentSTNK);
            }
            if (!String.IsNullOrEmpty(input.AttachmentPhoto))
            {
                queryFilter = queryFilter.And(k => k.AttachmentPhoto == input.AttachmentPhoto);
            }
            if (!String.IsNullOrEmpty(input.AttachmentPhotoTwo))
            {
                queryFilter = queryFilter.And(k => k.AttachmentPhotoTwo == input.AttachmentPhotoTwo);
            }
            if (!String.IsNullOrEmpty(input.AttachmentPhotoThree))
            {
                queryFilter = queryFilter.And(k => k.AttachmentPhotoThree == input.AttachmentPhotoThree);
            }
            if (input.STNKValidityPeriod != DateTime.MinValue)
            {
                queryFilter = queryFilter.And(k => k.STNKValidityPeriod >= input.STNKValidityPeriod);
            }
            if (input.IDVendor > 0)
            {
                queryFilter = queryFilter.And(k => k.IDVendor >= input.IDVendor);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportVehicleData>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<TransportVehicleDataDTO>>(dbResult);
        }
        #region new code Nicco
        public List<TransportVehicleDataDTO> GetTransportVehicleDatas2(TransportVehicleDataInput input)
        {
            using (var context = new TOMContextDB())
            {
                var tplv2 = (from x in context.TransportVehicleDatas 
                             select new TransportVehicleDataDTO
                            { }).ToList();
                var tplv = ( from x in context.TransportVehicleDatas
                             join y in context.MasterUsers on x.CreatedBy equals y.IDUser into LEFTJOIN1
                             from y in LEFTJOIN1.DefaultIfEmpty()
                             join z in context.MasterVendors on x.IDVendor equals z.IDVendor into LEFTJOIN2
                             from z in LEFTJOIN2.DefaultIfEmpty()
                             select new TransportVehicleDataDTO
                            {
                                IDPoliceRegNumber = x.IDPoliceRegNumber,
                                ManufacturingYear = x.ManufacturingYear,
                                Karoseri = x.Karoseri,
                                Merk = x.Merk,
                                Type = x.Type,
                                Model = x.Model,
                                Cargo = x.Cargo,
                                VehicleIdentityNumber = x.VehicleIdentityNumber,
                                EngineNumber = x.EngineNumber,
                                BaseTown = x.BaseTown,
                                STNKValidityPeriod = x.STNKValidityPeriod,
                                GPS = x.GPS,
                                IDVendor = x.IDVendor,
                                VendorName = z.VendorName,
                                Status = x.Status,
                                AttachmentSTNK = x.AttachmentSTNK,
                                AttachmentPhoto = x.AttachmentPhoto,
                                IsActive = x.IsActive,
                                CreatedBy = x.CreatedBy,
                                CreatedByFullName = y.FullName,
                                CreatedDate = x.CreatedDate,
                                UpdatedBy = y.UpdatedBy,
                                UpdatedByFullName = y.FullName,
                                UpdatedDate = x.UpdatedDate,
                                Remarks = x.Remarks,
                                AttachmentPhotoTwo = x.AttachmentPhotoTwo,
                                AttachmentPhotoThree = x.AttachmentPhotoThree
                                //IsNewData = x.IsNewData,
                            }).OrderBy(a => a.IDPoliceRegNumber)
                            .ToList()
                            ;


                if (input.IncludeInActive == "0")
                {
                    tplv = tplv.Where(k => !k.IsActive).ToList();
                }
                if (!String.IsNullOrEmpty(input.IDPoliceRegNumber))
                {
                    tplv = tplv.Where(k => k.IDPoliceRegNumber == input.IDPoliceRegNumber).ToList();
                }

                if (!String.IsNullOrEmpty(input.IDPoliceRegNumbers))
                {
                    string[] theIDPoliceRegNumbers = input.IDPoliceRegNumbers.Split(',');
                    tplv = tplv.Where(k => theIDPoliceRegNumbers.Contains(k.IDPoliceRegNumber)).ToList();
                }
                if (!String.IsNullOrEmpty(input.ManufacturingYears))
                {
                    int[] ManufacturingYears = input.ManufacturingYears
                        .Split(',').Select(number => int.Parse(number))
                        .ToArray();
                    tplv = tplv.Where(k => ManufacturingYears.Contains(k.ManufacturingYear.Value)).ToList();
                }
                if (!String.IsNullOrEmpty(input.Karoseri))
                {
                    tplv = tplv.Where(k => k.Karoseri == input.Karoseri).ToList();
                }
                if (!String.IsNullOrEmpty(input.Merk))
                {
                    tplv = tplv.Where(k => k.Merk == input.Merk).ToList();
                }
                if (!String.IsNullOrEmpty(input.Type))
                {
                    tplv = tplv.Where(k => k.Type == input.Type).ToList();
                }
                if (!String.IsNullOrEmpty(input.Model))
                {
                    tplv = tplv.Where(k => k.Model == input.Model).ToList();
                }
                if (!String.IsNullOrEmpty(input.Cargo))
                {
                    tplv = tplv.Where(k => k.Cargo == input.Cargo).ToList();
                }
                if (!String.IsNullOrEmpty(input.VehicleIdentityNumber))
                {
                    tplv = tplv.Where(k => k.VehicleIdentityNumber == input.VehicleIdentityNumber).ToList();
                }
                if (!String.IsNullOrEmpty(input.EngineNumber))
                {
                    tplv = tplv.Where(k => k.EngineNumber == input.EngineNumber).ToList();
                }
                if (!String.IsNullOrEmpty(input.BaseTown))
                {
                    tplv = tplv.Where(k => k.BaseTown == input.BaseTown).ToList();
                }
                if (!String.IsNullOrEmpty(input.GPS))
                {
                    tplv = tplv.Where(k => k.GPS == input.GPS).ToList();
                }
                if (!String.IsNullOrEmpty(input.Status))
                {
                    tplv = tplv.Where(k => k.Status == input.Status).ToList();
                }
                if (!String.IsNullOrEmpty(input.AttachmentSTNK))
                {
                    tplv = tplv.Where(k => k.AttachmentSTNK == input.AttachmentSTNK).ToList();
                }
                if (!String.IsNullOrEmpty(input.AttachmentPhoto))
                {
                    tplv = tplv.Where(k => k.AttachmentPhoto == input.AttachmentPhoto).ToList();
                }
                if (!String.IsNullOrEmpty(input.AttachmentPhotoTwo))
                {
                    tplv = tplv.Where(k => k.AttachmentPhotoTwo == input.AttachmentPhotoTwo).ToList();
                }
                if (!String.IsNullOrEmpty(input.AttachmentPhotoThree))
                {
                    tplv = tplv.Where(k => k.AttachmentPhotoThree == input.AttachmentPhotoThree).ToList();
                }
                if (input.STNKValidityPeriod != DateTime.MinValue)
                {
                    tplv = tplv.Where(k => k.STNKValidityPeriod >= input.STNKValidityPeriod).ToList();
                }
                if (input.IDVendor > 0)
                {
                    tplv = tplv.Where(k => k.IDVendor >= input.IDVendor).ToList();
                }


                return tplv;

            }
        }
        #endregion
        public dynamic GetVehicleDataTable(TransportVehicleDataInput filter, DataTableModel model = null)
        {
            dynamic res = new System.Dynamic.ExpandoObject();
            var queryFilter = PredicateHelper.True<TransportVehicleData>();
            var queryMV = PredicateHelper.True<MasterVendor>();
            var getVDXX = Enumerable.Empty<TransportVehicleData>();
            if (filter.IncludeInActive == "0")
            {
                queryFilter = queryFilter.And(k => !k.IsActive);
            }
            if (filter.IsActive)
            {
                queryFilter = queryFilter.And(k => k.IsActive);
            }
            var manufacturingYear = filter.ManufacturingYears == null ? "" : string.Join(",", filter.ManufacturingYears);
            var idPoliceRegNumber = filter.IDPoliceRegNumbers == null ? "" : string.Join(",", filter.IDPoliceRegNumbers);

            string[] fYear = manufacturingYear.Split(',').ToArray();
            string[] pNumber = idPoliceRegNumber.Split(',').ToArray();

            if (filter.Merk != null)
            {
                queryFilter = queryFilter.And(q => filter.Merk.Contains(q.Merk));
            }
            if (filter.BaseTown != null)
            {
                queryFilter = queryFilter.And(q => filter.BaseTown.Contains(q.BaseTown));
            }

            if (manufacturingYear.Length > 1) {
                queryFilter = queryFilter.And(q => fYear.Contains(q.ManufacturingYear.ToString()));
                getVDXX = _generalRepo.GetAllPagination(queryFilter, model.start, model.length, "IDPoliceRegNumber");
            }
            if (idPoliceRegNumber.Length > 1) { 
                queryFilter = queryFilter.And(q => pNumber.Contains(q.IDPoliceRegNumber));
                getVDXX = _generalRepo.GetAllPagination(queryFilter, model.start, model.length, "IDPoliceRegNumber");
            }
            getVDXX = _generalRepo.GetAllPagination(queryFilter, model.start, model.length, "IDPoliceRegNumber");
            var _listVM = new List<string>();
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.search.value))
                {
                    var search = model.search.value;
                    if (search != "")
                    {
                        //string[] arryStr = search.Split(',').ToArray();
                        _listVM = new List<string>();
                        queryMV = queryMV.And(q => search.Contains(q.VendorName));
                        var getMV = _masterVendorRepo.Get(queryMV);
                        _listVM.AddRange(getMV.Select(_ => _.IDVendor.ToString()).ToList());
                        _listVM = _listVM.Distinct().ToList();

                        queryFilter = queryFilter.And(w => (
                            _listVM.Contains(w.IDVendor.ToString()) ||
                            search.Contains(w.IDPoliceRegNumber) ||
                            search.Contains(w.ManufacturingYear.ToString()) ||
                            search.Contains(w.Karoseri) ||
                            search.Contains(w.Merk) ||
                            search.Contains(w.Type) ||
                            search.Contains(w.Cargo) ||
                            search.Contains(w.BaseTown) ||
                            search.Contains(w.STNKValidityPeriod.ToString()) ||
                            search.Contains(w.VehicleIdentityNumber)
                       ));
                    }
                }

                foreach (var fil in model.columns.Where(_ => !String.IsNullOrEmpty(_.search.value)))
                {
                    var _col = fil.data;
                    var _val = fil.search.value;
                    switch (_col)
                    {
                        case "VendorName":
                            _listVM = new List<string>();
                            queryMV = queryMV.And(q => q.VendorName.Contains(_val));
                            var getMV = _generalVendorRepo.Get(queryMV);
                            _listVM.AddRange(getMV.Select(_ => _.IDVendor.ToString()).ToList());
                            _listVM = _listVM.Distinct().ToList();
                            queryFilter = queryFilter.And(_ => _listVM.Contains(_.IDVendor.ToString()));
                            break;
                        case "IDPoliceRegNumber":
                            queryFilter = queryFilter.And(_ => _.IDPoliceRegNumber.Contains(_val));
                            break;
                        case "ManufacturingYear":
                            queryFilter = queryFilter.And(_ => _.ManufacturingYear.ToString().Contains(_val));
                            break;
                        case "Karoseri":
                            queryFilter = queryFilter.And(_ => _.Karoseri.Contains(_val));
                            break;
                        case "Merk":
                            queryFilter = queryFilter.And(_ => _.Merk.Contains(_val));
                            break;
                        case "Type":
                            queryFilter = queryFilter.And(_ => _.Type.Contains(_val));
                            break;
                        case "Cargo":
                            queryFilter = queryFilter.And(_ => _.Cargo.Contains(_val));
                            break;
                        case "BaseTown":
                            queryFilter = queryFilter.And(_ => _.BaseTown.ToString().Contains(_val));
                            break;
                        case "STNKValidityPeriod":
                            queryFilter = queryFilter.And(_ => _.STNKValidityPeriod.ToString().Contains(_val));
                            break;
                        case "VehicleIdentityNumber":
                            queryFilter = queryFilter.And(_ => _.VehicleIdentityNumber.Contains(_val));
                            break;
                    }
                }

            }

            var getVD = Enumerable.Empty<TransportVehicleData>();
            if (model == null)
            {
                getVD = _generalRepo.Get(queryFilter);
            }
            else
            {
                getVD = _generalRepo.GetAllPagination(queryFilter, model.start, model.length, "IDPoliceRegNumber");
            }

            var data = Mapper.Map<List<TransportVehicleDataDTO>>(getVD);
            //foreach (var tm in data)
            //{
            //    var MstVendor = _masterVendorRepo.Get(_ => _.IDVendor == tm.IDVendor).FirstOrDefault();
            //    tm.VendorName = MstVendor.VendorName;
            //}

            var Count = _generalRepo.Count(queryFilter);
            res.data = data;
            res.total = Count;
            return res;
        }

        public int CountTransportVehicleDatas(TransportVehicleDataInput input, string category)
        {
            var queryFilter = PredicateHelper.True<TransportVehicleData>();

            if (String.IsNullOrEmpty(input.IncludeInActive))
            {
                queryFilter = queryFilter.And(k => k.IsActive);
            }else if (input.IncludeInActive == "0")
            {
                queryFilter = queryFilter.And(k => !k.IsActive);
            }
            if (!String.IsNullOrEmpty(input.IDPoliceRegNumbers))
            {
                string[] theIDPoliceRegNumbers = input.IDPoliceRegNumbers.Split(',');
                foreach (string v in theIDPoliceRegNumbers)
                {
                    queryFilter = queryFilter.And(k => k.IDPoliceRegNumber == v);
                }
            }
            if (!String.IsNullOrEmpty(input.ManufacturingYears))
            {
                string[] theYears = input.ManufacturingYears.Split(',');
                foreach (string theYear in theYears)
                {
                    int year = Int32.Parse(theYear);
                    queryFilter = queryFilter.And(k => k.ManufacturingYear == year);
                }
            }
            if (!String.IsNullOrEmpty(input.Merk))
            {
                queryFilter = queryFilter.And(k => k.Merk == input.Merk);
            }
            if (!String.IsNullOrEmpty(input.BaseTown))
            {
                queryFilter = queryFilter.And(k => k.BaseTown == input.BaseTown);
            }
            
            // begin to count
            if (category == "totalLightTruckUnits")
            {
                queryFilter = queryFilter.And(k => k.Type == "Light Truck");
            }
            if (category == "totalStandardUnits")
            {
                queryFilter = queryFilter.And(k => k.Type == "Standard");
            }
            if (category == "totalTrontonLongUnits")
            {
                queryFilter = queryFilter.And(k => k.Type == "Tronton Long");
            }
            if (category == "totalUnitwithMoreThanEightYearsOld")
            {
                DateTime dt = DateTime.Now;

                int y = dt.Year;
                int EightYear = y - 8;

                queryFilter = queryFilter.And(k => k.ManufacturingYear < EightYear);

            }
            if (category == "totalUnitwithGPS")
            {
                queryFilter = queryFilter.And(k => k.GPS == "Yes");
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TransportVehicleData>();

            return _generalRepo.Get(queryFilter, orderByFilter).Count();
        }

        public int UpdateTransportVehicleData(TransportVehicleDataDTO input, string controller, string userid)
        {
            return _transportVehicleDataRepo.Update(input, controller, userid);
        }

        public int SetInactiveRecovery(TransportVehicleDataDTO input, string controller, string userid)
        {
            return _transportVehicleDataRepo.SetInactiveRecovery(input, controller, userid);
        }

        public List<TransportVehicleDataDTO> exportToExcel()
        {
            return _transportVehicleDataRepo.GetRecordsForExcel();
        }
    }
}