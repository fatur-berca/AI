using DFIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public class TransportVehicleDataRepo : TOMGenericRepository<TransportVehicleData>, ITransportVehicleDataRepo
    {
        private TOMContextDB _context;
        public TransportVehicleDataRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }
        public int Update(TransportVehicleDataDTO input, string controller, string userid)
        {
            var context = new TOMContextDB();

            TransportVehicleData dbRes = context.TransportVehicleDatas.FirstOrDefault(x => x.IDPoliceRegNumber == input.IDPoliceRegNumber && x.IDVendor == input.IDVendor);

            // update
            if (dbRes != null)
            {
                if (!input.IsNewData) { 
                    //dbRes.IDPoliceRegNumber = input.IDPoliceRegNumber;
                    dbRes.ManufacturingYear = input.ManufacturingYear;
                    dbRes.Karoseri = input.Karoseri;
                    dbRes.Merk = input.Merk;
                    dbRes.Type = input.Type;
                    dbRes.Model = input.Model;
                    dbRes.Cargo = input.Cargo;
                    dbRes.VehicleIdentityNumber = input.VehicleIdentityNumber;
                    dbRes.EngineNumber = input.EngineNumber;
                    dbRes.BaseTown = input.BaseTown;
                    dbRes.STNKValidityPeriod = input.STNKValidityPeriod;
                    dbRes.GPS = input.GPS;
                    //dbRes.IDVendor = input.IDVendor;
                    dbRes.Status = input.Status;
                    dbRes.AttachmentSTNK = input.AttachmentSTNK;
                    dbRes.AttachmentPhoto = input.AttachmentPhoto;
                    dbRes.AttachmentPhotoTwo = input.AttachmentPhotoTwo;
                    dbRes.AttachmentPhotoThree = input.AttachmentPhotoThree;
                    dbRes.IsActive = input.IsActive;
                    dbRes.Remarks = input.Remarks;
                    dbRes.UpdatedBy = userid;
                    dbRes.UpdatedDate = DateTime.Now;
                    context.SaveChanges();
                    return 0;
                }
                return 1;
            }
            else // save
            {
                TransportVehicleData inputTransportVehicleData = new TransportVehicleData();
                
                inputTransportVehicleData.IDPoliceRegNumber = input.IDPoliceRegNumber.TrimStart().TrimEnd();
                inputTransportVehicleData.ManufacturingYear = input.ManufacturingYear;
                inputTransportVehicleData.Karoseri = input.Karoseri;
                inputTransportVehicleData.Merk = input.Merk;
                inputTransportVehicleData.Type = input.Type;
                inputTransportVehicleData.Model = input.Model;
                inputTransportVehicleData.Cargo = input.Cargo;
                inputTransportVehicleData.VehicleIdentityNumber = input.VehicleIdentityNumber;
                inputTransportVehicleData.EngineNumber = input.EngineNumber;
                inputTransportVehicleData.BaseTown = input.BaseTown;
                inputTransportVehicleData.STNKValidityPeriod = input.STNKValidityPeriod;
                inputTransportVehicleData.GPS = input.GPS;
                inputTransportVehicleData.IDVendor = input.IDVendor;
                inputTransportVehicleData.Status = input.Status;
                inputTransportVehicleData.AttachmentSTNK = input.AttachmentSTNK;
                inputTransportVehicleData.AttachmentPhoto = input.AttachmentPhoto;
                inputTransportVehicleData.AttachmentPhotoTwo = input.AttachmentPhotoTwo;
                inputTransportVehicleData.AttachmentPhotoThree = input.AttachmentPhotoThree;
                inputTransportVehicleData.IsActive = input.IsActive;
                inputTransportVehicleData.CreatedBy = userid;
                inputTransportVehicleData.CreatedDate = DateTime.Now;
                inputTransportVehicleData.UpdatedBy = userid;
                inputTransportVehicleData.UpdatedDate = DateTime.Now;
                inputTransportVehicleData.Remarks = input.Remarks;

                context.TransportVehicleDatas.Add(inputTransportVehicleData);
                context.SaveChanges();
                return 0;
            }
        }

        public int SetInactiveRecovery(TransportVehicleDataDTO input, string controller, string userid)
        {
            var context = new TOMContextDB();

            TransportVehicleData dbRes = context.TransportVehicleDatas.FirstOrDefault(x => x.IDPoliceRegNumber == input.IDPoliceRegNumber);

            if (dbRes != null)
            {
                dbRes.IDPoliceRegNumber = input.IDPoliceRegNumber;
                dbRes.IsActive = input.IsActive;
                dbRes.UpdatedBy = userid;
                dbRes.UpdatedDate = DateTime.Now;
            }

            return context.SaveChanges();
        }

        public List<TransportVehicleDataDTO> GetRecordsForExcel()
        {
            var dFISContextDB = new TOMContextDB();

            List<TransportVehicleData> Records = dFISContextDB.TransportVehicleDatas.SqlQuery("SELECT * FROM [DFIS].[dbo].[TransportVehicleData]", '1').ToList();

            return Records.Select(x => new TransportVehicleDataDTO { IDPoliceRegNumber = x.IDPoliceRegNumber, ManufacturingYear = x.ManufacturingYear.Value, Karoseri = x.Karoseri, Merk = x.Merk, Type = x.Type, Model = x.Model, Cargo = x.Cargo, VehicleIdentityNumber = x.VehicleIdentityNumber, EngineNumber = x.EngineNumber, BaseTown = x.BaseTown, STNKValidityPeriod = x.STNKValidityPeriod, GPS = x.GPS, Status = x.Status, AttachmentSTNK = x.AttachmentSTNK, AttachmentPhoto = x.AttachmentPhoto, IsActive = x.IsActive, CreatedBy = x.CreatedBy, CreatedDate = x.CreatedDate, UpdatedBy = x.UpdatedBy, UpdatedDate = x.UpdatedDate, Remarks = x.Remarks }).ToList();
        }

        public TransportVehicleData GetTransportVehicleData(string idVD)
        {
            var queryFilter = PredicateHelper.True<TransportVehicleData>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDPoliceRegNumber == idVD);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<TransportVehicleData> GetALLTransportVehicleDataActiveList()
        {
            var queryFilter = PredicateHelper.True<TransportVehicleData>();
            queryFilter = queryFilter.And(x => x.IsActive);
            return Get(queryFilter).ToList();
        }

        public List<TransportVehicleData> GetALLTransportVehicleDataList()
        {
            var queryFilter = PredicateHelper.True<TransportVehicleData>();
            return Get(queryFilter).ToList();
        }

        public List<TransportVehicleData> GetDataByCriteriaActive(TransportVehicleDataInput criteria)
        {
            var queryFilter = PredicateHelper.True<TransportVehicleData>();
            queryFilter = queryFilter.And(f => f.IsActive);
            if (criteria.IDVendor > 0)
            {
                queryFilter = queryFilter.And(f => f.IDVendor == criteria.IDVendor);
            }
            return Get(queryFilter).ToList();
        }
    }
}