using AutoMapper;
using DFIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using Remotion.FunctionalProgramming;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Repositories;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Repositories.TransportLostClaimDamageRepo;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.Enums;

namespace TOM.Transport.BusinessLogics.TransportLostClaimDamageBLL
{
    public class TransportLostClaimDamageBLL : ITransportLostClaimDamageBLL
    {
        private readonly ITransportLostClaimDamageRepo _repo;
        private TOMGenericRepository<MasterUserRoleMapping> _masterUserRoleMappingRepo;
        private readonly TOMGenericRepository<MasterVendor> _generalRepo;
        private readonly TOMGenericRepository<TransportLostClaimDamage> _transportRepo;
        private INotificationSystemRepo _notificationRepo;
        private readonly IMasterUserRepo _mUserRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;

        public TransportLostClaimDamageBLL(ITransportLostClaimDamageRepo repo, TOMGenericRepository<MasterVendor> generalRepo, TOMGenericRepository<TransportLostClaimDamage> transportRepo, INotificationSystemRepo NotificationRepo, TOMGenericRepository<MasterUserRoleMapping> MasterUserRoleMappingRepo, IMasterUserRepo mUserRepo, IMasterLocationRepo masterLocationRepo)
        {
            _repo = repo;
            _generalRepo = generalRepo;
            _transportRepo = transportRepo;
            _notificationRepo = NotificationRepo;
            _masterUserRoleMappingRepo = MasterUserRoleMappingRepo;
            _mUserRepo = mUserRepo;
            _masterLocationRepo = masterLocationRepo;
        }

        //public TransportLostClaimDamageBLL(ITransportLostClaimDamageRepo repo, TOMGenericRepository<MasterUserRoleMapping> MasterUserRoleMappingRepo, TOMGenericRepository<MasterVendor> generalRepo, TOMGenericRepository<TransportLostClaimDamage> transportRepo, INotificationSystemRepo NotificationRepo, IMasterUserRepo mUserRepo)
        //{
        //    _repo = repo;
        //    _generalRepo = generalRepo;
        //    _transportRepo = transportRepo;
        //    _notificationRepo = NotificationRepo;
        //    _masterUserRoleMappingRepo = MasterUserRoleMappingRepo;
        //    _mUserRepo = mUserRepo;
        //}

        #region INITIATION
        public TransportLostClaimDamageDTO SaveInitiation(TransportLostClaimDamageDTO newData, string userID)
        {
            var dateNow = DateTime.Now;
            int saveNumber = 0;//dipakai untuk mishandling & other
            int looping = 0;//digunakan pada waktu looping sebelum save untuk mencari nomor
            bool saveSuccess = false;//digunakan pada waktu looping sebelum save untuk mencari nomor
            string generateNumber = "";
            string prefixtn = "";
            string prefixon = "";

            newData.CreatedDate = dateNow;
            newData.UpdatedDate = dateNow;
            newData.CreatedBy = userID;
            newData.UpdatedBy = userID;
            newData.IsActive = true;
            newData.Status = TransportEnums.LostClaimProgress.INITIATION.ToString();
            newData.RealStatus = TransportEnums.LostClaimProgress.INITIATION.ToString();

            if (newData.IsAnyAttachment)
                newData.RealStatus = TransportEnums.LostClaimProgress.VERIFICATION.ToString();

            using (var dbContext = new TOMContextDB())
            {
                //claim type mishandling & other membuat nomor tn dan sto sendiri, pertama cari nomor paling akhir, kemudian di tambah 1
                if (newData.ClaimType == "Misshandling" || newData.ClaimType == "Other")
                {
                    prefixtn = newData.GPNumber;
                    prefixon = newData.SJNumber;
                    string tn = "";
                    if (prefixtn.Contains("MSH"))
                    {
                        tn = dbContext.TransportLostClaimDamages
                            .Where(x => x.CreatedDate.Month == dateNow.Month && x.CreatedDate.Year == dateNow.Year && x.GPNumber.Contains("MSH")).OrderByDescending(x => x.GPNumber)
                            .Select(x => x.GPNumber).FirstOrDefault();
                    }
                    else if (prefixtn.Contains("OTH"))
                    {
                        tn = dbContext.TransportLostClaimDamages
                            .Where(x => x.CreatedDate.Month == dateNow.Month && x.CreatedDate.Year == dateNow.Year && x.GPNumber.Contains("OTH")).OrderByDescending(x => x.GPNumber)
                            .Select(x => x.GPNumber).FirstOrDefault();
                    }
                    if (!string.IsNullOrEmpty(tn))
                        saveNumber = Convert.ToInt32(tn.Substring(12, 3));
                    saveNumber++;
                    generateNumber = saveNumber.ToString();
                    generateNumber = generateNumber.PadLeft(3, '0');
                    newData.GPNumber = prefixtn + generateNumber;
                    newData.SJNumber = prefixon + generateNumber;
                }

                var checkGP = dbContext.TransportLostClaimDamages.FirstOrDefault(y => y.SJNumber == newData.SJNumber);
                if (checkGP != null)//DATANYA ADA DI DATABASE
                {
                    if (checkGP.IsActive)//KALAU DATANYA AKTIF
                        throw new Exception("Data with Order Number/STO " + newData.SJNumber + " and Transportation Number " + newData.GPNumber + " is already exists");
                    //KALAU DATANYA TIDAK AKTIF, LANSUNG DI DELETE
                    dbContext.TransportLostClaimUploadBoxes.RemoveRange(dbContext.TransportLostClaimUploadBoxes.Where(c => c.SJNumber == newData.SJNumber));
                    dbContext.TransportLostClaimFACodes.RemoveRange(dbContext.TransportLostClaimFACodes.Where(c => c.SJNumber == newData.SJNumber));
                    dbContext.TransportLostClaimDamages.Remove(checkGP);
                }

                if (newData.ListFACode.Any())
                {
                    newData.TotalClaimInPack = newData.ListFACode.Sum(c => c.Pack);
                    var totalInStick = 0;
                    var hje = 0;
                    foreach (var brand in newData.ListFACode)
                    {
                        var FA = dbContext.MasterFABrands.Find(brand.FACode.ToUpper());
                        if (FA != null) { 
                            totalInStick += brand.Pack * Convert.ToInt32(FA.StickPerPack);
                            hje += brand.Pack * FA.HJE;
                        }
                    }
                    newData.TotalClaimInStick = totalInStick;
                    newData.InitClaimedAmount = hje;
                }

                var newInsertData = Mapper.Map<TransportLostClaimDamage>(newData);
                newInsertData.BSWarehouseReceiveDate = null;
                newInsertData.HMSMemoDate = null;
                newInsertData.InvoiceDate = null;
                newInsertData.VendorPaymentDate = null;
                newInsertData.EDPSDate = null;
                newInsertData.IncinerationReportDate = null;
                dbContext.TransportLostClaimDamages.Add(newInsertData);

                #region Save Brand
                //var listMasterFACode = dbContext.MasterFABrands.Select(c => c.FACode).Distinct().ToList();
                // Save FA Code
                foreach (var brand in newData.ListFACode)
                { 
                    //var upperBrandCode = brand.FACode.ToUpper();
                    //if (!listMasterFACode.Contains(upperBrandCode))
                    //{
                    //    throw new Exception("FA Code does not exist in Master");
                    //}

                    var newBrand = new TransportLostClaimFACode();
                    newBrand.FACode = brand.FACode;
                    newBrand.SpeakingCode = brand.SpeakingCode;
                    newBrand.GPNumber = newData.GPNumber;
                    newBrand.SJNumber = newData.SJNumber;
                    newBrand.LostOrDamage = brand.LostOrDamage;
                    newBrand.Pack = Convert.ToInt32(brand.Pack);
                    newBrand.Description = brand.Description;
                    newBrand.IsActive = true;
                    newBrand.CreatedBy = userID;
                    newBrand.UpdatedBy = userID;
                    newBrand.CreatedDate = dateNow;
                    newBrand.UpdatedDate = dateNow;

                    dbContext.TransportLostClaimFACodes.Add(newBrand);
                }
                #endregion

                #region Save Upload Boxes
                if (!String.IsNullOrEmpty(newData.ImageUploadConditionBoxPath1))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadConditionBoxPath1, newData.JsonCanvasUploadBox1, newData.RemarkUploadConditionBox1, "UploadBoxesCondition", 1, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadConditionBoxPath2))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadConditionBoxPath2, newData.JsonCanvasUploadBox2, newData.RemarkUploadConditionBox2, "UploadBoxesCondition", 2, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadConditionBoxPath3))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadConditionBoxPath3, newData.JsonCanvasUploadBox3, newData.RemarkUploadConditionBox3, "UploadBoxesCondition", 3, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadPositionTruckPath1))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadPositionTruckPath1, newData.JsonCanvasPositionTruck1, newData.RemarkUploadPositionTruck1, "TruckCondition", 1, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadPositionTruckPath2))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadPositionTruckPath2, newData.JsonCanvasPositionTruck2, newData.RemarkUploadPositionTruck2, "TruckCondition", 2, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadPositionTruckPath3))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadPositionTruckPath3, newData.JsonCanvasPositionTruck3, newData.RemarkUploadPositionTruck3, "TruckCondition", 3, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadFinalResultPath1))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadFinalResultPath1, newData.JsonCanvasFinalResult1, newData.RemarkUploadFinalResult1, "FinalResult", 1, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadFinalResultPath2))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadFinalResultPath2, newData.JsonCanvasFinalResult2, newData.RemarkUploadFinalResult2, "FinalResult", 2, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadFinalResultPath3))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadFinalResultPath3, newData.JsonCanvasFinalResult3, newData.RemarkUploadFinalResult3, "FinalResult", 3, userID, dateNow));
                #endregion

                if (newData.ClaimType == "Misshandling" || newData.ClaimType == "Other")
                {
                    while (saveSuccess == false || looping < 10)
                    {
                        try
                        {
                            dbContext.SaveChanges();
                            saveSuccess = true;
                            looping = 11;
                        }
                        catch
                        {
                            looping++;
                            if (looping == 11)
                                saveSuccess = true;
                            else { 
                                saveNumber++;
                                generateNumber = saveNumber.ToString();
                                generateNumber = generateNumber.PadLeft(3, '0');
                                newInsertData.GPNumber = prefixtn + generateNumber;
                                newInsertData.SJNumber = prefixon + generateNumber;
                                dbContext.TransportLostClaimDamages.AddOrUpdate(newInsertData);
                            }
                        }
                    }
                }
                else
                    dbContext.SaveChanges();
            }
            return newData;
        }

        private TransportLostClaimUploadBox SetTransportLostClaimUploadBox(string gpNumber, string sjNumber, string imagesBox, string canvas, string remarks, string tag, int tagNumber, string userID, DateTime dateNow)
        {
            TransportLostClaimUploadBox newUploadBox = new TransportLostClaimUploadBox();
            newUploadBox.GPNumber = gpNumber;
            newUploadBox.SJNumber = sjNumber;
            newUploadBox.ImageBoxes = imagesBox;
            newUploadBox.JsonCanvas = canvas;
            newUploadBox.Tag = tag;
            newUploadBox.TagNumber = tagNumber;
            newUploadBox.Remarks = remarks;
            newUploadBox.IsActive = true;
            newUploadBox.CreatedBy = userID;
            newUploadBox.UpdatedBy = userID;
            newUploadBox.CreatedDate = dateNow;
            newUploadBox.UpdatedDate = dateNow;
            return newUploadBox;
        }

        public void SaveInitiationAttachment(string pathFile, string SJNumber, string GPNumber)
        {
            var dateNow = DateTime.Now;

            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(GPNumber, SJNumber);
                if (existingData == null) throw new Exception("Data with SJNumber " + SJNumber + " and GPNumber " + GPNumber + " is already exists");

                existingData.Attachment = pathFile;
                dbContext.SaveChanges();
            }
        }

        public void SaveFile(string pathFile, string SJNumber, string GPNumber, string status, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(GPNumber, SJNumber);
                if (existingData == null) throw new Exception("Data with SJNumber " + SJNumber + " and GPNumber " + GPNumber + " is already exists");

                if (status == TransportEnums.LostClaimProgress.PROCESS.ToString())
                {
                    existingData.HMSMemoFile = pathFile;
                    existingData.Status = TransportEnums.LostClaimProgress.PROCESS.ToString();
                }
                else if (status == TransportEnums.LostClaimProgress.INVOICING.ToString())
                {
                    existingData.InsurancePaymentProof = pathFile;
                }
                else if (status == TransportEnums.LostClaimProgress.DISPOSE.ToString())
                {
                    existingData.IncinerationReportFile = pathFile;
                    existingData.Status = TransportEnums.LostClaimProgress.DISPOSE.ToString();
                }

                existingData.UpdatedBy = userID;
                existingData.UpdatedDate = DateTime.Now;
                dbContext.SaveChanges();
            }
        }

        public TransportLostClaimDamageDTO EditInitiation(TransportLostClaimDamageDTO newData, string userID)
        {
            var dateNow = DateTime.Now;

            newData.UpdatedDate = dateNow;
            newData.UpdatedBy = userID;
            newData.IsActive = true;
            newData.Status = TransportEnums.LostClaimProgress.INITIATION.ToString();
            newData.RealStatus = TransportEnums.LostClaimProgress.INITIATION.ToString();

            if (newData.IsAnyAttachment)
                newData.RealStatus = TransportEnums.LostClaimProgress.VERIFICATION.ToString();

            using (var dbContext = new TOMContextDB())
            {
                if (newData.ListFACode.Any())
                {
                    newData.TotalClaimInPack = newData.ListFACode.Sum(c => c.Pack);
                    var totalInStick = 0;
                    var hje = 0;
                    foreach (var brand in newData.ListFACode)
                    {
                        var FA = dbContext.MasterFABrands.Find(brand.FACode);
                        if (FA != null) { 
                            totalInStick += brand.Pack * Convert.ToInt32(FA.StickPerPack);
                            hje += brand.Pack * FA.HJE;
                        }
                    }
                    newData.TotalClaimInStick = totalInStick;
                    newData.InitClaimedAmount = hje;
                }

                dbContext.TransportLostClaimUploadBoxes.RemoveRange(dbContext.TransportLostClaimUploadBoxes.Where(c => c.SJNumber == newData.SJNumber));
                dbContext.TransportLostClaimFACodes.RemoveRange(dbContext.TransportLostClaimFACodes.Where(c => c.SJNumber == newData.SJNumber));
                var existingData = dbContext.TransportLostClaimDamages.FirstOrDefault(c => c.SJNumber == newData.SJNumber);
                TransportLostClaimDamage oldData = new TransportLostClaimDamage();
                if (newData.SJNumberOld != null)
                    oldData = dbContext.TransportLostClaimDamages.FirstOrDefault(c => c.SJNumber == newData.SJNumberOld);
                if (existingData != null)
                {
                    if (newData.GPNumberOld != newData.GPNumber)
                    {
                        if (existingData.IsActive)
                            throw new Exception("Data with Order Number/STO " + newData.SJNumber + " and Transportation Number " + newData.GPNumber + " is already exists");
                        dbContext.TransportLostClaimDamages.Remove(existingData);
                        var newInsertData = Mapper.Map<TransportLostClaimDamage>(newData);
                        if (!string.IsNullOrEmpty(oldData.CreatedBy))
                        {
                            newInsertData.CreatedBy = oldData.CreatedBy;
                            newInsertData.CreatedDate = oldData.CreatedDate;
                        }
                        else
                        {
                            newInsertData.CreatedBy = userID;
                            newInsertData.CreatedDate = dateNow;
                        }
                        dbContext.TransportLostClaimUploadBoxes.RemoveRange(dbContext.TransportLostClaimUploadBoxes.Where(c => c.SJNumber == newData.SJNumberOld));
                        dbContext.TransportLostClaimFACodes.RemoveRange(dbContext.TransportLostClaimFACodes.Where(c => c.SJNumber == newData.SJNumberOld));
                        dbContext.TransportLostClaimDamages.Remove(oldData);
                        dbContext.SaveChanges();
                        dbContext.TransportLostClaimDamages.AddOrUpdate(newInsertData);
                    }
                    else
                    {
                        existingData.ClaimType = newData.ClaimType;
                        existingData.SJDate = newData.SJDate;
                        existingData.DeliveryNumberNote = newData.DeliveryNumberNote;
                        existingData.OriginWarehouse = newData.OriginWarehouse;
                        existingData.DestinationWarehouse = newData.DestinationWarehouse;
                        existingData.STONumber = newData.STONumber;
                        existingData.DateOfDelivery = newData.DateOfDelivery;
                        existingData.DateOfIncident = newData.DateOfIncident;
                        existingData.TransportationVendor = newData.TransportationVendor;
                        existingData.PoliceRegNumber = newData.PoliceRegNumber;
                        existingData.Driver = newData.Driver;
                        existingData.Driver2 = newData.Driver2;
                        existingData.CoDriver = newData.CoDriver;
                        existingData.Supervisor = newData.Supervisor;
                        existingData.Receiver = newData.Receiver;
                        existingData.Witness = newData.Witness;
                        //existingData.DNtoReverseLogistics = newData.DNtoReverseLogistics;
                        existingData.UpdatedBy = userID;
                        existingData.UpdatedDate = dateNow;
                        existingData.Status = TransportEnums.LostClaimProgress.INITIATION.ToString();
                        existingData.RealStatus = newData.RealStatus;
                        existingData.TransportationMode = newData.TransportationMode;
                        existingData.TotalClaimInPack = newData.TotalClaimInPack;
                        existingData.TotalClaimInStick = newData.TotalClaimInStick;
                        existingData.InitClaimedAmount = newData.InitClaimedAmount;
                        dbContext.TransportLostClaimDamages.AddOrUpdate(existingData);
                    }
                }
                else
                {
                    var newInsertData = Mapper.Map<TransportLostClaimDamage>(newData);
                    if (!string.IsNullOrEmpty(oldData.CreatedBy))
                    {
                        newInsertData.CreatedBy = oldData.CreatedBy;
                        newInsertData.CreatedDate = oldData.CreatedDate;
                    }
                    else
                    {
                        newInsertData.CreatedBy = userID;
                        newInsertData.CreatedDate = dateNow;
                    }
                    dbContext.TransportLostClaimUploadBoxes.RemoveRange(dbContext.TransportLostClaimUploadBoxes.Where(c => c.SJNumber == newData.SJNumberOld));
                    dbContext.TransportLostClaimFACodes.RemoveRange(dbContext.TransportLostClaimFACodes.Where(c => c.SJNumber == newData.SJNumberOld));
                    dbContext.TransportLostClaimDamages.Remove(oldData);
                    dbContext.TransportLostClaimDamages.AddOrUpdate(newInsertData);
                }

                #region Save Brand
                //var listMasterFACode = dbContext.MasterFABrands.Select(c => c.FACode).Distinct().ToList();
                // Save FA Code
                foreach (var brand in newData.ListFACode)
                {
                    //var upperBrandCode = brand.FACode.ToUpper();
                    //if (!listMasterFACode.Contains(upperBrandCode))
                    //{
                    //    throw new Exception("FA Code does not exist in Master");
                    //}

                    var newBrand = new TransportLostClaimFACode();
                    newBrand.FACode = brand.FACode;
                    newBrand.SpeakingCode = brand.SpeakingCode;
                    newBrand.GPNumber = newData.GPNumber;
                    newBrand.SJNumber = newData.SJNumber;
                    newBrand.LostOrDamage = brand.LostOrDamage;
                    newBrand.Pack = Convert.ToInt32(brand.Pack);
                    newBrand.Description = brand.Description;
                    newBrand.IsActive = true;
                    newBrand.CreatedBy = userID;
                    newBrand.UpdatedBy = userID;
                    newBrand.CreatedDate = dateNow;
                    newBrand.UpdatedDate = dateNow;

                    dbContext.TransportLostClaimFACodes.Add(newBrand);
                }
                #endregion

                #region Save Upload Boxes
                if (!String.IsNullOrEmpty(newData.ImageUploadConditionBoxPath1))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadConditionBoxPath1, newData.JsonCanvasUploadBox1, newData.RemarkUploadConditionBox1, "UploadBoxesCondition", 1, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadConditionBoxPath2))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadConditionBoxPath2, newData.JsonCanvasUploadBox2, newData.RemarkUploadConditionBox2, "UploadBoxesCondition", 2, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadConditionBoxPath3))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadConditionBoxPath3, newData.JsonCanvasUploadBox3, newData.RemarkUploadConditionBox3, "UploadBoxesCondition", 3, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadPositionTruckPath1))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadPositionTruckPath1, newData.JsonCanvasPositionTruck1, newData.RemarkUploadPositionTruck1, "TruckCondition", 1, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadPositionTruckPath2))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadPositionTruckPath2, newData.JsonCanvasPositionTruck2, newData.RemarkUploadPositionTruck2, "TruckCondition", 2, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadPositionTruckPath3))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadPositionTruckPath3, newData.JsonCanvasPositionTruck3, newData.RemarkUploadPositionTruck3, "TruckCondition", 3, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadFinalResultPath1))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadFinalResultPath1, newData.JsonCanvasFinalResult1, newData.RemarkUploadFinalResult1, "FinalResult", 1, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadFinalResultPath2))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadFinalResultPath2, newData.JsonCanvasFinalResult2, newData.RemarkUploadFinalResult2, "FinalResult", 2, userID, dateNow));

                if (!String.IsNullOrEmpty(newData.ImageUploadFinalResultPath3))
                    dbContext.TransportLostClaimUploadBoxes.Add(SetTransportLostClaimUploadBox(newData.GPNumber, newData.SJNumber, newData.ImageUploadFinalResultPath3, newData.JsonCanvasFinalResult3, newData.RemarkUploadFinalResult3, "FinalResult", 3, userID, dateNow));
                #endregion

                dbContext.SaveChanges();
            }
            return newData;
        }
        #endregion

        #region VERIFICATION
        public void SaveVerification(TransportLostClaimDamageDTO newData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(newData.GPNumber, newData.SJNumber);
                if (existingData == null) throw new Exception("Cannot find data with SJNumber " + newData.SJNumber + " and GPNumber " + newData.GPNumber);

                existingData.ClaimExpense = newData.ClaimExpense;
                existingData.TransportationMode = newData.TransportationMode;
                existingData.GoodsCategory = newData.GoodsCategory;
                existingData.InitClaimedAmount = newData.InitClaimedAmount;
                existingData.ClaimCategory = newData.ClaimCategory;
                existingData.TotalClaimInPack = newData.TotalClaimInPack;
                existingData.TotalClaimInStick = newData.TotalClaimInStick;
                existingData.Status = TransportEnums.LostClaimProgress.VERIFICATION.ToString();
                existingData.RealStatus = TransportEnums.LostClaimProgress.PROCESS.ToString();
                existingData.UpdatedDate = DateTime.Now;
                existingData.UpdatedBy = userID;

                dbContext.SaveChanges();
            }
        }

        public void RevokeVerification(TransportLostClaimDamageDTO revokeData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(revokeData.GPNumber, revokeData.SJNumber);
                if (existingData != null)
                {
                    existingData.UpdatedBy = userID;
                    existingData.UpdatedDate = DateTime.Now;
                    var currStatus = existingData.Status;
                    var currEnumsVal = (int)EnumHelper.GetEnumValue<TransportEnums.LostClaimProgress>(currStatus.ToUpper());
                    var revokeEnums = ((TransportEnums.LostClaimProgress)(currEnumsVal - 1)).ToString().ToUpper();
                    existingData.Status = revokeEnums;
                    existingData.ClaimExpense = null;
                    //existingData.TransportationMode = null;
                    existingData.GoodsCategory = null;
                    //existingData.InitClaimedAmount = null;
                    existingData.ClaimCategory = null;
                    //existingData.TotalClaimInPack = null;
                    //existingData.TotalClaimInStick = null;
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region PROCESS
        public void SaveProcess(TransportLostClaimDamageDTO newData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(newData.GPNumber, newData.SJNumber);
                if (existingData == null) throw new Exception("Cannot find data with SJNumber " + newData.SJNumber + " and GPNumber " + newData.GPNumber);

                existingData.HMSMemoNumber = newData.HMSMemoNumber;
                existingData.BSWarehouseReceiveDate = newData.BSWarehouseReceiveDate;
                existingData.HMSMemoDate = newData.HMSMemoDate;
                //if (String.IsNullOrEmpty(newData.HMSMemoNumber))
                //{
                //    existingData.Status = "PROCESS";
                //}                                
                //existingData.Status = TransportEnums.LostClaimProgress.PROCESS.ToString();
                if (existingData.HMSMemoNumber != null)
                {
                    existingData.Status = TransportEnums.LostClaimProgress.PROCESS.ToString();
                    existingData.RealStatus = TransportEnums.LostClaimProgress.INVOICING.ToString();
                }
                else
                {
                    existingData.Status = TransportEnums.LostClaimProgress.PROCESS.ToString();
                    existingData.RealStatus = TransportEnums.LostClaimProgress.PROCESS.ToString();
                }
                existingData.UpdatedDate = DateTime.Now;
                existingData.UpdatedBy = userID;

                dbContext.SaveChanges();
            }
        }

        public void RevokeProcess(TransportLostClaimDamageDTO revokeData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(revokeData.GPNumber, revokeData.SJNumber);
                if (existingData != null)
                {
                    existingData.UpdatedBy = userID;
                    existingData.UpdatedDate = DateTime.Now;
                    var currStatus = existingData.Status;
                    var currEnumsVal = (int)EnumHelper.GetEnumValue<TransportEnums.LostClaimProgress>(currStatus.ToUpper());
                    var revokeEnums = ((TransportEnums.LostClaimProgress)(currEnumsVal - 2)).ToString().ToUpper();
                    existingData.Status = revokeEnums;
                    existingData.BSWarehouseReceiveDate = null;
                    existingData.HMSMemoNumber = null;
                    existingData.HMSMemoDate = null;
                    existingData.HMSMemoFile = null;

                    dbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region INVOICING
        public void SaveInvoicing(TransportLostClaimDamageDTO newData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(newData.GPNumber, newData.SJNumber);
                if (existingData == null) throw new Exception("Cannot find data with SJNumber " + newData.SJNumber + " and GPNumber " + newData.GPNumber);

                existingData.InvoiceNumber = newData.InvoiceNumber;
                existingData.InvoiceDate = newData.InvoiceDate;
                existingData.InvoiceAmount = newData.InvoiceAmount;
                existingData.VendorPaymentDate = newData.VendorPaymentDate;
                existingData.GrossLossAmount = newData.GrossLossAmount;
                existingData.NetClaim = newData.NetClaim;
                if (existingData.VendorPaymentDate != null && existingData.VendorPaymentDate > new DateTime(0001, 01, 01))
                {
                    existingData.Status = TransportEnums.LostClaimProgress.INVOICING.ToString();
                    existingData.RealStatus = TransportEnums.LostClaimProgress.DISPOSE.ToString();
                }
                else
                {
                    existingData.RealStatus = TransportEnums.LostClaimProgress.INVOICING.ToString();
                }
                existingData.Status = (newData.VendorPaymentDate == DateTime.MinValue) ? TransportEnums.LostClaimProgress.PROCESS.ToString() : TransportEnums.LostClaimProgress.INVOICING.ToString();
                existingData.UpdatedDate = DateTime.Now;
                existingData.UpdatedBy = userID;

                dbContext.SaveChanges();
            }
        }

        public void RevokeInvoicing(TransportLostClaimDamageDTO revokeData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(revokeData.GPNumber, revokeData.SJNumber);
                if (existingData != null)
                {
                    existingData.UpdatedBy = userID;
                    existingData.UpdatedDate = DateTime.Now;
                    var currStatus = existingData.Status;
                    var currEnumsVal = (int)EnumHelper.GetEnumValue<TransportEnums.LostClaimProgress>(currStatus.ToUpper());
                    var revokeEnums = ((TransportEnums.LostClaimProgress)(currEnumsVal - 2)).ToString().ToUpper();
                    existingData.Status = revokeEnums;

                    existingData.InvoiceNumber = null;
                    existingData.InvoiceDate = null;
                    existingData.InvoiceAmount = null;
                    existingData.VendorPaymentDate = null;
                    existingData.GrossLossAmount = null;
                    existingData.NetClaim = null;
                    existingData.InsurancePaymentProof = null;

                    dbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region DISPOSE
        public void SaveDispose(TransportLostClaimDamageDTO newData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(newData.GPNumber, newData.SJNumber);
                if (existingData == null) throw new Exception("Cannot find data with SJNumber " + newData.SJNumber + " and GPNumber " + newData.GPNumber);

                existingData.EDPSNumber = newData.EDPSNumber;
                existingData.EDPSDate = newData.EDPSDate;
                existingData.IncinerationReportDate = newData.IncinerationReportDate;
                //existingData.Status = TransportEnums.LostClaimProgress.DISPOSE.ToString();
                existingData.RealStatus = TransportEnums.LostClaimProgress.COMPLETED.ToString();
                existingData.UpdatedDate = DateTime.Now;
                existingData.Status = TransportEnums.LostClaimProgress.DISPOSE.ToString();
                existingData.UpdatedBy = userID;

                dbContext.SaveChanges();
            }
        }

        public void RevokeDispose(TransportLostClaimDamageDTO revokeData, string userID)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(revokeData.GPNumber, revokeData.SJNumber);
                if (existingData != null)
                {
                    existingData.UpdatedBy = userID;
                    existingData.UpdatedDate = DateTime.Now;
                    var currStatus = existingData.Status;
                    var currEnumsVal = (int)EnumHelper.GetEnumValue<TransportEnums.LostClaimProgress>(currStatus.ToUpper());
                    var revokeEnums = ((TransportEnums.LostClaimProgress)(currEnumsVal - 2)).ToString().ToUpper();
                    existingData.Status = revokeEnums;
                    existingData.RealStatus = TransportEnums.LostClaimProgress.INVOICING.ToString();

                    existingData.EDPSNumber = null;
                    existingData.EDPSDate = null;
                    existingData.IncinerationReportDate = null;
                    existingData.IncinerationReportFile = null;

                    dbContext.SaveChanges();
                }
            }
        }

        public void SetActive(List<string> id, bool status)
        {
            _repo.SetActive(id, status);
        }

        #endregion

        public IEnumerable<string> GetDropDownOriginWarehouse()
        {
            var listOriginWarehouse = _repo.GetListOriginWarehouse();
            return listOriginWarehouse;
        }
        public IEnumerable<string> GetDropDownDestWarehouse()
        {
            var listDestnWarehouse = _repo.GetListDestinationWarehouse();
            return listDestnWarehouse;
        }

        public List<string> GetTransportationNumberFilter(string stono)
        {
            return _repo.GetListSTONumber(stono);
        }
        /*
        public IEnumerable<string> GetDropDownSTONumber()
        {
            var listSTONumber = _repo.GetListSTONumber();
            return listSTONumber;
        }
        */

        public IEnumerable<string> GetDropDownDeliveryNoteNumber()
        {
            var listDeliveryNoteNumber = _repo.GetListDeliveryNoteNumber();
            return listDeliveryNoteNumber;
        }
        public IEnumerable<string> GetDropDownPoliceRegNumber()
        {
            var listPoliceRegNumber = _repo.GetListPoliceRegNumber();
            return listPoliceRegNumber;
        }
        public IEnumerable<string> GetDropDownVendor()
        {
            var listVendor = _repo.GetListVendor();
            return listVendor;
        }
        public IEnumerable<string> GetDropDownClaimType()
        {
            var listClaimType = _repo.GetListClaimType();
            return listClaimType;
        }
        public IEnumerable<string> GetDropDownClaimExpense()
        {
            var listClaimType = _repo.GetListClaimExpense();
            return listClaimType;
        }
        public IEnumerable<string> GetDropDownClaimCategory()
        {
            var listClaimCategory = _repo.GetListClaimCategory();
            return listClaimCategory;
        }
        public IEnumerable<string> GetDropDownSJNumber()
        {
            var listSJNumber = _repo.GetListSJNumber();
            return listSJNumber;
        }
        //public IEnumerable<string> GetDropDownGPNumber()
        //{
        //    var listGPNumber = _repo.GetListGPNumber();
        //    return listGPNumber;
        //}

        public IEnumerable<TransportLostClaimDamageDTO> GetListLostClaimDamage(TransportLostClaimDamageInput input)
        {
            var listLostClaimDamage = _repo.GetListTransportLostClaimDamage(input);
            var vendorlist = _generalRepo.Get();
            var dtoMapper = Mapper.Map<List<TransportLostClaimDamageDTO>>(listLostClaimDamage);
            foreach (var data in dtoMapper)
            {
                var vendorName = vendorlist.Where(x => x.IDVendor == data.TransportationVendor).Select(x => x.VendorName).FirstOrDefault();
                data.TransportationVendorName = vendorName;
            }
            return dtoMapper;
        }

        public IEnumerable<TransportLostClaimDamageDTO> GetListDefault()
        {
            var listResultDefault = _repo.GetListDefault();
            var vendorlist = _generalRepo.Get();
            var dtoMapper = Mapper.Map<List<TransportLostClaimDamageDTO>>(listResultDefault);
            foreach (var data in dtoMapper)
            {
                var vendorName = vendorlist.Where(x => x.IDVendor == data.TransportationVendor).Select(x => x.VendorName).FirstOrDefault();
                data.TransportationVendorName = vendorName;
            }
            return dtoMapper;
        }

        public TransportLostClaimDamageDTO GetTransportLostClaimDamageByKey(string GPNumber, string SJNumber)
        {
            var resultDB = _repo.GetTransportLostClaimDamage(GPNumber, SJNumber);
            var dTO = Mapper.Map<TransportLostClaimDamageDTO>(resultDB);
            var mstVendor = new MasterVendor();
            if(dTO.DestinationWarehouse != null)
            {
                var temp = _masterLocationRepo.GetMasterLocationByLocationName(dTO.DestinationWarehouse);
                if(temp != null)
                    dTO.DestinationWarehouseIDLocation = temp.IDLocation;
            }
            if (dTO.TransportationVendor != null)
                mstVendor = _generalRepo.GetByID(dTO.TransportationVendor);
            if (mstVendor != null)
                dTO.TransportationVendorName = mstVendor.VendorName;
            var listFACodes = _repo.GetListTransportLostClaimFACode(GPNumber, SJNumber);
            var listBoxes = _repo.GetListTransportLostClaimUploadBoxes(GPNumber, SJNumber);

            dTO.ListFACode = new List<TransportLostClaimFACodeDTO>();

            foreach (var brand in listFACodes)
            {
                var brandFACode = new TransportLostClaimFACodeDTO();
                brandFACode.FACode = brand.FACode;
                brandFACode.LostOrDamage = brand.LostOrDamage;
                brandFACode.Pack = brand.Pack;
                brandFACode.SpeakingCode = brand.SpeakingCode;
                brandFACode.Description = brand.Description;

                dTO.ListFACode.Add(brandFACode);
            }
            var createdbyname = _mUserRepo.GetUserNameById(dTO.CreatedBy);
            dTO.CreatedByName = createdbyname.FullName;

            foreach (var box in listBoxes)
            {
                if (box.TagNumber.Value == 1)
                {
                    if (box.Tag == "UploadBoxesCondition")
                    {
                        dTO.JsonCanvasUploadBox1 = box.JsonCanvas;
                        dTO.ImageUploadConditionBoxPath1 = box.ImageBoxes;
                        dTO.RemarkUploadConditionBox1 = box.Remarks;
                    }
                    else if (box.Tag == "TruckCondition")
                    {
                        dTO.JsonCanvasPositionTruck1 = box.JsonCanvas;
                        dTO.ImageUploadPositionTruckPath1 = box.ImageBoxes;
                        dTO.RemarkUploadPositionTruck1 = box.Remarks;
                    }
                    else if (box.Tag == "FinalResult")
                    {
                        dTO.JsonCanvasFinalResult1 = box.JsonCanvas;
                        dTO.ImageUploadFinalResultPath1 = box.ImageBoxes;
                        dTO.RemarkUploadFinalResult1 = box.Remarks;
                    }
                }

                if (box.TagNumber.Value == 2)
                {
                    if (box.Tag == "UploadBoxesCondition")
                    {
                        dTO.JsonCanvasUploadBox2 = box.JsonCanvas;
                        dTO.ImageUploadConditionBoxPath2 = box.ImageBoxes;
                        dTO.RemarkUploadConditionBox2 = box.Remarks;
                    }
                    else if (box.Tag == "TruckCondition")
                    {
                        dTO.JsonCanvasPositionTruck2 = box.JsonCanvas;
                        dTO.ImageUploadPositionTruckPath2 = box.ImageBoxes;
                        dTO.RemarkUploadPositionTruck2 = box.Remarks;
                    }
                    else if (box.Tag == "FinalResult")
                    {
                        dTO.JsonCanvasFinalResult2 = box.JsonCanvas;
                        dTO.ImageUploadFinalResultPath2 = box.ImageBoxes;
                        dTO.RemarkUploadFinalResult2 = box.Remarks;
                    }
                }

                if (box.TagNumber.Value == 3)
                {
                    if (box.Tag == "UploadBoxesCondition")
                    {
                        dTO.JsonCanvasUploadBox3 = box.JsonCanvas;
                        dTO.ImageUploadConditionBoxPath3 = box.ImageBoxes;
                        dTO.RemarkUploadConditionBox3 = box.Remarks;
                    }
                    else if (box.Tag == "TruckCondition")
                    {
                        dTO.JsonCanvasPositionTruck3 = box.JsonCanvas;
                        dTO.ImageUploadPositionTruckPath3 = box.ImageBoxes;
                        dTO.RemarkUploadPositionTruck3 = box.Remarks;
                    }
                    else if (box.Tag == "FinalResult")
                    {
                        dTO.JsonCanvasFinalResult3 = box.JsonCanvas;
                        dTO.ImageUploadFinalResultPath3 = box.ImageBoxes;
                        dTO.RemarkUploadFinalResult3 = box.Remarks;
                    }
                }
            }
            return dTO;
        }

        public TransportLostClaimDamageDTO GetAddNewDetail(string SJNumber)
        {
            //var dataRepo = _repo.GetNewDataDetail(SJNumber);
            //var mstVendor = new MasterVendor();
            //if (dataRepo.TransportationVendor != null)
            //{
            //    mstVendor = _generalRepo.GetByID(dataRepo.TransportationVendor);
            //}

            //var dto = Mapper.Map<TransportLostClaimDamageDTO>(dataRepo);
            //dto.TransportationVendorName = mstVendor.VendorName;
            //return dto;
            return _repo.GetNewDataDetail(SJNumber);
        }

        public string GetSpeakingCode(string FACode)
        {
            using (var context = new TOMContextDB())
            {
                var db = context.MasterFABrands.Find(FACode);
                if (db != null)
                    return db.LongSpeakingCode;
                else return string.Empty;
            }
        }

        public void RemoveAttachment(string GPNumber, string SJNumber)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(GPNumber, SJNumber);

                if (existingData != null)
                {
                    existingData.Attachment = null;
                    dbContext.SaveChanges();
                }
            }
        }

        public void RemoveMemoFile(string GPNumber, string SJNumber)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(GPNumber, SJNumber);

                if (existingData != null)
                {
                    existingData.HMSMemoFile = null;
                    dbContext.SaveChanges();
                }
            }
        }

        public void RemovePaymentProof(string GPNumber, string SJNumber)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(GPNumber, SJNumber);

                if (existingData != null)
                {
                    existingData.InsurancePaymentProof = null;
                    dbContext.SaveChanges();
                }
            }
        }

        public void RemoveIncinerationReportFile(string GPNumber, string SJNumber)
        {
            using (var dbContext = new TOMContextDB())
            {
                var existingData = dbContext.TransportLostClaimDamages.Find(GPNumber, SJNumber);

                if (existingData != null)
                {
                    existingData.IncinerationReportFile = null;
                    dbContext.SaveChanges();
                }
            }
        }
        public void SaveNotification(TransportLostClaimDamageDTO Dto, List<string> listuser, string username)
        {
            foreach (var userid in listuser)
            {

                var dbResult = _masterUserRoleMappingRepo.Get(x => x.IsActive).ToList();
                var input = new NotificationSystem();
                var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
                string pageName = "";

                if (Dto.Status == null || Dto.Status == TransportEnums.LostClaimProgress.INITIATION.ToString())
                {
                    pageName = TransportEnums.LostClaimProgress.INITIATION + " Lost Claim Damage by " + username + " - " + String.Format("{0:dd-MMM-yyyy HH:mm}", DateTime.Now);

                }
                else if (Dto.Status == TransportEnums.LostClaimProgress.VERIFICATION.ToString())
                {
                    pageName = TransportEnums.LostClaimProgress.VERIFICATION + " Lost Claim Damage by " + username + " - " + String.Format("{0:dd-MMM-yyyy HH:mm}", DateTime.Now);

                }
                else if (Dto.Status == TransportEnums.LostClaimProgress.PROCESS.ToString())
                {
                    pageName = TransportEnums.LostClaimProgress.PROCESS + " Lost Claim Damage by " + username + " - " + String.Format("{0:dd-MMM-yyyy HH:mm}", DateTime.Now);

                }
                else if (Dto.Status == TransportEnums.LostClaimProgress.INVOICING.ToString())
                {
                    pageName = TransportEnums.LostClaimProgress.INVOICING + " Lost Claim Damage by " + username + " - " + String.Format("{0:dd-MMM-yyyy HH:mm}", DateTime.Now);

                }
                else if (Dto.Status == TransportEnums.LostClaimProgress.DISPOSE.ToString())
                {
                    pageName = TransportEnums.LostClaimProgress.DISPOSE + " Lost Claim Damage by " + username + " - " + String.Format("{0:dd-MMM-yyyy HH:mm}", DateTime.Now);

                }
                var newchar = "~";
                var arrGpNumber = Dto.GPNumber.Split('/');
                var GPNumber = string.Join(newchar, arrGpNumber);
                var arrSJNumber = Dto.SJNumber.Split('/');
                var SJNumber = string.Join(newchar, arrSJNumber);
                input.IDUser = userid;
                input.PageName = pageName;
                input.Description = webRootUrl + "TransportLostDamageClaim/Edit?id=" + GPNumber + '$' + SJNumber;
                input.IsOpen = false;
                input.IsRead = false;
                input.CreatedBy = username;
                input.CreatedDate = DateTime.Now;
                input.UpdatedBy = username;
                input.UpdatedDate = DateTime.Now;
                _notificationRepo.SaveData(input);

            }
        }

        public string checkDataUpload(TransportLostClaimDamageDTO input, string sheetName)
        {
            var context = new TOMContextDB();
            string result = "";

            if (sheetName == "Process")
            {
                var check = context.TransportLostClaimDamages
                    .Where(x => x.SJNumber == input.SJNumber)
                    .Where(x => x.RealStatus == "PROCESS")
                    .Select(y => new { y.SJNumber, y.HMSMemoNumber }).FirstOrDefault();

                if (check != null)
                {
                    if (check.HMSMemoNumber != null)
                    {
                        if (input.HMSMemoNumber == "" || input.HMSMemoNumber == null)
                        {
                            result = "conflict";
                        }
                        else
                        {
                            result = "aman";
                        }
                    }
                    else
                    {
                        result = "aman";
                    }
                }
                else
                {
                    result = "conflict";
                }
            }

            else if (sheetName == "Invoicing")
            {
                var check = context.TransportLostClaimDamages
                    .Where(x => x.SJNumber == input.SJNumber)
                    .Where(x => x.RealStatus == "INVOICING")
                    .Select(y => new { y.SJNumber, y.VendorPaymentDate }).FirstOrDefault();

                if (check != null)
                {
                    if (check.VendorPaymentDate != null)
                    {
                        if (input.VendorPaymentDate.ToString() == "" || input.VendorPaymentDate.ToString() == null)
                        {
                            result = "conflict";
                        }
                        else
                        {
                            result = "aman";
                        }
                    }
                    else
                    {
                        result = "aman";
                    }
                }
                else
                {
                    result = "conflict";
                }
            }

            else if (sheetName == "Dispose")
            {
                var check = context.TransportLostClaimDamages
                    .Where(x => x.SJNumber == input.SJNumber)
                    .Where(x => x.RealStatus == "DISPOSE")
                    .Select(y => new { y.SJNumber, y.EDPSDate }).FirstOrDefault();

                if (check != null)
                {
                    if (check.EDPSDate != null)
                    {
                        if (input.EDPSDate.ToString() == "" || input.EDPSDate.ToString() == null)
                        {
                            result = "conflict";
                        }
                        else
                        {
                            result = "aman";
                        }
                    }
                    else
                    {
                        result = "aman";
                    }
                }
                else
                {
                    result = "conflict";
                }
            }

            return result;
        }
        public TransportLostClaimDamageDTO UpdateDataTransport(TransportLostClaimDamageDTO input, string sheetName)
        {
            var context = new TOMContextDB();

            if (sheetName == "Process")
            {
                var dbRes = context.TransportLostClaimDamages
                    .Where(m => m.SJNumber == input.SJNumber)
                    .Where(m => m.RealStatus == "PROCESS").FirstOrDefault();

                if (dbRes != null)
                {
                    dbRes.BSWarehouseReceiveDate = input.BSWarehouseReceiveDate;
                    dbRes.HMSMemoNumber = input.HMSMemoNumber;
                    dbRes.HMSMemoDate = input.HMSMemoDate;

                    if (input.HMSMemoNumber != "")
                    {
                        dbRes.Status = "PROCESS";
                        dbRes.RealStatus = "INVOICING";
                    }

                    context.SaveChanges();
                }

                return Mapper.Map<TransportLostClaimDamageDTO>(dbRes);
            }
            else if (sheetName == "Invoicing")
            {
                var dbRes = context.TransportLostClaimDamages
                    .Where(m => m.SJNumber == input.SJNumber)
                    .Where(m => m.RealStatus == "INVOICING").FirstOrDefault();

                if (dbRes != null)
                {
                    dbRes.InvoiceNumber = input.InvoiceNumber;
                    dbRes.InvoiceDate = input.InvoiceDate;
                    dbRes.InvoiceAmount = input.InvoiceAmount;
                    dbRes.VendorPaymentDate = input.VendorPaymentDate;
                    if (input.VendorPaymentDate.ToString() != "")
                    {
                        if (dbRes.ClaimType != "Product Lost")
                        {
                            dbRes.Status = "INVOICING";
                            dbRes.RealStatus = "DISPOSE";
                        }
                        else
                        {
                            dbRes.Status = "DISPOSE";
                            dbRes.RealStatus = "COMPLETED";
                        }
                    }
                    context.SaveChanges();

                }
                return Mapper.Map<TransportLostClaimDamageDTO>(dbRes);
            }
            else if (sheetName == "Dispose")
            {
                var dbRes = context.TransportLostClaimDamages
                    .Where(m => m.SJNumber == input.SJNumber)
                    .Where(m => m.ClaimType != "Product Lost")
                    .Where(m => m.RealStatus == "DISPOSE").FirstOrDefault();
                if (dbRes != null)
                {
                    dbRes.EDPSNumber = input.EDPSNumber;
                    dbRes.EDPSDate = input.EDPSDate;
                    dbRes.IncinerationReportDate = input.IncinerationReportDate;
                    if (input.EDPSDate.ToString() != "")
                    {
                        dbRes.Status = "DISPOSE";
                        dbRes.RealStatus = "COMPLETED";
                    }
                    context.SaveChanges();

                }
                return Mapper.Map<TransportLostClaimDamageDTO>(dbRes);
            }
            else
            {
                return Mapper.Map<TransportLostClaimDamageDTO>(null);
            }
        }

        public void UpdateCloneFile(string path)
        {
            //UPDATE FILE UNTUK PROCESS DAN DISPOSE
            var context = new TOMContextDB();
            var queryFilter = PredicateHelper.True<TransportLostClaimDamage>();
            var queryFilter2 = PredicateHelper.True<TransportLostClaimDamage>();

            #region PROCESS
            queryFilter = queryFilter.And(p => p.HMSMemoFile != null).And(q => q.RealStatus == "PROCESS");
            var dbRes = _transportRepo.Get(queryFilter).ToList();

            //get all memo number
            var tempMemoNumber = dbRes
                .GroupBy(c => c.HMSMemoNumber)
                .Where(d => d.Count() == 1)
                .Select(d => d.Key).ToList();

            if (tempMemoNumber.Count > 0)
            {
                foreach (var HMSMemoNumber in tempMemoNumber)
                {
                    //get memo file path for the particular memo number
                    var MemoFilePath = dbRes
                        .Where(d => d.HMSMemoNumber == HMSMemoNumber)
                        .Select(e => e.HMSMemoFile).First().ToString();

                    var tempExt = MemoFilePath.Split('.');
                    var ext = tempExt[(tempExt.Length) - 1];

                    //get sjNumber and status where the HMSMemoNumber is the same and HMSMemoFile is null
                    var SJNumbernStat = context.TransportLostClaimDamages
                        .Where(x => x.HMSMemoNumber == HMSMemoNumber)
                        .Where(z => z.HMSMemoFile == null)
                        .Select(y => new { y.SJNumber, y.RealStatus }).ToList();

                    if (SJNumbernStat.Count() > 0)
                    {
                        foreach (var SJ in SJNumbernStat)
                        {
                            string fileSourceName = MemoFilePath;
                            string fileDestName = SJ.SJNumber.Replace("/", "~");
                            fileDestName += "_" + SJ.RealStatus + "." + ext;

                            var path2 = Directory.EnumerateFiles(path);

                            string sourceFile = System.IO.Path.Combine(path, fileSourceName);
                            string destFile = System.IO.Path.Combine(path, fileDestName);

                            var cek = 0;
                            try
                            {
                                System.IO.File.Copy(sourceFile, destFile, true);
                                cek = 1;
                            }
                            catch (FileNotFoundException ex)
                            {
                                cek = 0;
                            }

                            if (cek == 1)
                            {
                                //update HMSMemoFile for the particular SJNumber
                                var dbResult = context.TransportLostClaimDamages.Where(m => m.SJNumber == SJ.SJNumber).FirstOrDefault();
                                if (dbResult != null)
                                {
                                    dbResult.HMSMemoFile = fileDestName;
                                    try
                                    {
                                        context.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }

                    }
                }
            }
            #endregion

            #region DISPOSE
            queryFilter2 = queryFilter2.And(p => p.IncinerationReportFile != null).And(q => q.Status == "DISPOSE").And(r => r.ClaimType != "Product Lost");
            var dbRes2 = _transportRepo.Get(queryFilter2).ToList();

            //get all EDPS number
            var tempEDPSNumber = dbRes2
                .GroupBy(c => c.EDPSNumber)
                .Where(d => d.Count() == 1)
                .Select(d => d.Key).ToList();

            if (tempEDPSNumber.Count > 0)
            {
                foreach (var EDPSNumber in tempEDPSNumber)
                {
                    //get Incineration file path for the particular EDPS number
                    var IncinerationFilePath = dbRes2
                        .Where(d => d.EDPSNumber == EDPSNumber)
                        .Select(e => e.IncinerationReportFile).First().ToString();

                    var tempExtD = IncinerationFilePath.Split('.');
                    var extD = tempExtD[(tempExtD.Length) - 1];

                    //get SJNumber where the HMSMemoNumber is the same and HMSMemoFile is null
                    var SJNumnStat = context.TransportLostClaimDamages
                        .Where(x => x.EDPSNumber == EDPSNumber)
                        .Where(z => z.IncinerationReportFile == null)
                        .Select(y => new { y.SJNumber, y.Status }).ToList();

                    if (SJNumnStat.Count() > 0)
                    {
                        foreach (var SJ in SJNumnStat)
                        {
                            string fileSourceNameD = IncinerationFilePath;
                            string fileDestNameD = SJ.SJNumber.Replace("/", "~");
                            fileDestNameD += "_" + SJ.Status + "." + extD;

                            var path2 = Directory.EnumerateFiles(path);

                            string sourceFileD = System.IO.Path.Combine(path, fileSourceNameD);
                            string destFileD = System.IO.Path.Combine(path, fileDestNameD);

                            var cek = 0;
                            try
                            {
                                System.IO.File.Copy(sourceFileD, destFileD, true);
                                cek = 1;
                            }
                            catch (FileNotFoundException ex)
                            {
                                cek = 0;
                            }

                            if (cek == 1)
                            {
                                //update IncinerationFile for the particular SJNumber
                                var dbResultD = context.TransportLostClaimDamages.Where(m => m.SJNumber == SJ.SJNumber).FirstOrDefault();
                                if (dbResultD != null)
                                {
                                    dbResultD.IncinerationReportFile = fileDestNameD;
                                    try
                                    {
                                        context.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }

                    }
                }
            }
            #endregion
        }

        public List<TransportLostClaimFACode> GetTransporftLostClaimFACodeBySTO(string transno, string stono)
        {
            return _repo.GetListTransportLostClaimFACode(transno, stono).ToList();
        }    
    }
}
