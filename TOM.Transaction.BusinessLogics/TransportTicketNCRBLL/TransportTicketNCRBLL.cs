using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Universal.Domain.DTOs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Repositories.TransportTicketNCRRepo;

namespace TOM.Transport.BusinessLogics.TransportTicketNCRBLL
{
    public class TransportTicketNCRBLL : ITransportTicketNCRBLL
    {
        private readonly ITransportTicketNCRRepo _transportTicketNcrRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;
        private readonly IMasterVendorTOMRepo _masterVendorRepo;
        private readonly TOMGenericRepository<MasterList> _generalList;
        private TOMGenericRepository<TransportTicketNCR> _generalRepo;

        public TransportTicketNCRBLL(ITransportTicketNCRRepo transportTicketNcrRepo, IMasterListRepo masterListRepo, IMasterLocationRepo masterLocationRepo, IMasterVendorTOMRepo masterVendorRepo, TOMGenericRepository<MasterList> generalList, TOMGenericRepository<TransportTicketNCR> generalRepo)
        {
            _transportTicketNcrRepo = transportTicketNcrRepo;
            _masterListRepo = masterListRepo;
            _masterLocationRepo = masterLocationRepo;
            _masterVendorRepo = masterVendorRepo;
            _generalList = generalList;
            _generalRepo = generalRepo;
        }

        public List<MasterList> GetMasterListByFieldName(string fieldName)
        {
            return _masterListRepo.GetMasterListByFieldName(fieldName);
        }

        public List<MasterLocationDTO> GetMasterLocation()
        {
            return Mapper.Map<List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActive());
        }

        public List<MasterVendorTOMDTO> GetMasterVendor()
        {
            return Mapper.Map<List<MasterVendorTOMDTO>>(_masterVendorRepo.GetAllMasterVendorActive());
        }

        public List<MasterListDTO> GetTicketCategory()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "TicketCategory").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> Getvehicletypes()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "vehicletype").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetLocationTypes()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "TypeOfLocation").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetOffenderYOS()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "OffenderYOS").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetOffenderRoles()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "OffenderRole").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetLocationCategorys()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "LocationCategory").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetAnalystProblems()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "ProblemAnalysis").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetMainProblems()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "MainFactor").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }


        public List<MasterListDTO> GetRoadConditions()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "RoadCondition").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetWeatherConditions()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "WeatherCondition").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetAccidentCategorys()
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "AccidentCategory").And(m => m.IsActive == true);

            var dbResult = _generalList.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }
        public string getTicketnumbers()
        {
            var ticketNCR = "TIK." + DateTime.Now.ToString("MM") + DateTime.Now.ToString("yy");

            TOMContextDB context = new TOMContextDB();
            //var countnumber = (from x in context.TransportTicketNCRs where  x.TicketNumber.StartsWith(ticketNCR)  select x.TicketNumber).Count();
            int countnumber = (from x in context.TransportTicketNCRs where x.TicketNumber.StartsWith(ticketNCR) select x.TicketNumber).ToList().Count() + 1;

            //if (countnumber != null)
            //{
            if (countnumber >= 0 && countnumber <= 9)
            {
                //countnumber = countnumber +1;
                ticketNCR = ticketNCR + ".000" + countnumber;
            }

            else if (countnumber >= 10 && countnumber <= 99)
            {
                //countnumber = countnumber + 1;
                ticketNCR = ticketNCR + ".00" + countnumber;
            }
            else if (countnumber >= 100 && countnumber <= 999)
            {
                //countnumber = countnumber + 1;
                ticketNCR = ticketNCR + ".0" + countnumber;
            }
            else
            {
                //countnumber = countnumber + 1;
                ticketNCR = ticketNCR + countnumber;
            }
            //}

            return ticketNCR;
        }

        public string getNonTicketnumbers()
        {
            var ticketNCR = "NONTIK." + DateTime.Now.ToString("MM") + DateTime.Now.ToString("yy");

            TOMContextDB context = new TOMContextDB();
            int countnumber = (from x in context.TransportTicketNCRs where x.TicketNumber.StartsWith(ticketNCR) select x.TicketNumber).ToList().Count() + 1;

            //if (countnumber != null)
            //{
            if (countnumber >= 0 && countnumber <= 9)
            {
                //countnumber = countnumber + 1;
                ticketNCR = ticketNCR + ".000" + countnumber;
            }

            else if (countnumber >= 10 && countnumber <= 99)
            {
                //countnumber = countnumber + 1;
                ticketNCR = ticketNCR + ".00" + countnumber;
            }
            else if (countnumber >= 100 && countnumber <= 999)
            {
                //countnumber = countnumber + 1;
                ticketNCR = ticketNCR + ".0" + countnumber;
            }
            else
            {
                //
                countnumber = countnumber + 1;
                ticketNCR = ticketNCR + countnumber;
            }
            //}

            return ticketNCR;
        }

        public void SetActive(List<string> id, bool status)
        {
            _transportTicketNcrRepo.SetActive(id, status);
        }

        public TransportTicketNCRDTO SaveData(TransportTicketNCRDTO input)
        {
            //var validateInput = new MasterFGStackingInput()
            //{
            //    Brand = input.Brand,
            //    MaxStacking = input.MaxStacking,
            //    MinStacking = input.MaxStacking,
            //    IsActive = input.IsActive
            //};
            // var prevData = GetMasterFGStackings(validateInput);
            var dbTransportTIcketNcr = Mapper.Map<TransportTicketNCR>(input);
            //if (prevData == null || (prevData != null && prevData.Count == 0))
            //{
            //    dbMstFGStacking.CreatedDate = DateTime.Now;
            //    dbMstFGStacking.UpdatedDate = DateTime.Now;
            //    _generalRepo.Insert(dbMstFGStacking);
            //    _generalRepo.Save();
            //}
            //else
            //{
            //    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            try
            {
                dbTransportTIcketNcr.CorrectiveActionDate = Convert.ToDateTime(input.strCorrectiveActionDate);
                dbTransportTIcketNcr.CreatedDate = DateTime.Now;
                dbTransportTIcketNcr.UpdatedDate = DateTime.Now;
                if (input.TicketNumber == "NONTIK")
                {
                    dbTransportTIcketNcr.TicketNumber = getNonTicketnumbers();
                }
                else
                {
                    dbTransportTIcketNcr.TicketNumber = getTicketnumbers();
                }

                _generalRepo.Insert(dbTransportTIcketNcr);
                _generalRepo.Save();
            }
            catch
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }

            //}
            return Mapper.Map<TransportTicketNCRDTO>(dbTransportTIcketNcr);
        }

        public TransportTicketNCRDTO EditData(TransportTicketNCRDTO input)
        {

            TOMContextDB context = new TOMContextDB();


            TransportTicketNCR c = (from x in context.TransportTicketNCRs
                                    where x.TicketNumber == input.TicketNumber
                                    select x).First();
            c.TicketQuantity = input.TicketQuantity;
            c.UpdatedDate = DateTime.Now;
            //c.IsActive = input.IsActive;
            c.UpdatedBy = input.UpdatedBy;
            c.Remarks = input.Remarks;
            c.Location = input.Location;
            c.IDVendor = input.IDVendor;
            c.TicketCategory = input.TicketCategory;
            c.PoliceRegNumber = input.PoliceRegNumber;
            c.OffenderName = input.OffenderName;
            c.OffenderAge = input.OffenderAge;
            c.OffenderRole = input.OffenderRole;
            c.RemarksTicket = input.RemarksTicket;
            if (input.Attachment != null)
            {
                c.Attachment = input.Attachment;
            }

            c.CorrectiveActionDate = Convert.ToDateTime(input.strCorrectiveActionDate);
            try
            {
                context.SaveChanges();
            }
            catch
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            var dbTransportTIcketNcr = Mapper.Map<TransportTicketNCR>(input);
            return Mapper.Map<TransportTicketNCRDTO>(dbTransportTIcketNcr);

        }

        public TransportTicketNCRDTO EditDataCirf(TransportTicketNCRDTO input)
        {

            TOMContextDB context = new TOMContextDB();


            TransportTicketNCR c = (from x in context.TransportTicketNCRs
                                    where x.TicketNumber == input.TicketNumber
                                    select x).First();
            if (input.VehicleType == null)
            {
                c.VehicleType = "Tronton Long";
            }
            else
            {
                c.VehicleType = input.VehicleType;
            }

            c.UpdatedDate = DateTime.Now;
            c.IsActive = input.IsActive;
            c.UpdatedBy = input.UpdatedBy;
            c.Remarks = input.Remarks;
            c.ManufacturingYear = input.ManufacturingYear;
            c.OffenderYearOfService = input.OffenderYearOfService;
            c.LocationCategory = input.LocationCategory;
            c.TypeOfLocation = input.TypeOfLocation;
            c.RoadCondition = input.RoadCondition;
            c.Preventablility = input.Preventablility;
            c.WeatherCondition = input.WeatherCondition;
            c.AccidentCategory = input.AccidentCategory;
            c.Remarks = input.Remarks;
            c.ValueOfLoss = input.ValueOfLoss;
            c.CorrectiveAction = input.CorrectiveAction;

            c.TicketStatus = input.TicketStatus;
            c.IsActive = input.IsActive;
            c.SIRSNumber = input.SIRSNumber;
            c.NCRNumber = input.NCRNumber;
            if (input.AttachmentNRC != null)
            {
                c.AttachmentNRC = input.AttachmentNRC;
            }


            c.ProblemIssue1 = input.ProblemIssue1;
            c.ProblemIssue2 = input.ProblemIssue2;
            c.ProblemIssue3 = input.ProblemIssue3;
            c.MainFactor = input.MainFactor;

            try
            {
                context.SaveChanges();
            }
            catch
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            var dbTransportTIcketNcr = Mapper.Map<TransportTicketNCR>(input);
            return Mapper.Map<TransportTicketNCRDTO>(dbTransportTIcketNcr);

        }

        public TransportTicketNCRDTO SetInactives(TransportTicketNCRDTO input)
        {
            TOMContextDB context = new TOMContextDB();


            TransportTicketNCR c = (from x in context.TransportTicketNCRs
                                    where input.SetDelete.Contains(x.TicketNumber)
                                    select x).First();

            c.IsActive = false;



            try
            {
                context.SaveChanges();
            }
            catch
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            var dbTransportTIcketNcr = Mapper.Map<TransportTicketNCR>(input);
            return Mapper.Map<TransportTicketNCRDTO>(dbTransportTIcketNcr);
        }

        public string GetSumTickets(TransportTicketNCRInput input)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();

            if (input.DateFrom != null && input.DateTo != null)
            {
                DateTime datefrom = Convert.ToDateTime(input.DateFrom);
                DateTime dateto = Convert.ToDateTime(input.DateTo);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");



                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);


            }
            else
            {

                DateTime datefrom = Convert.ToDateTime(DateTime.Now.Date.AddDays(-30));
                DateTime dateto = Convert.ToDateTime(DateTime.Now.Date);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");


                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);
            }

            if (input.IDVendor != 0)
            {
                queryFilter = queryFilter.And(k => k.IDVendor == input.IDVendor);
                //dbResult = dbResult.Where(m => m.IDVendor == input.IDVendor).ToList();
            }

            if (!input.IncludeDeleted)
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
                else
                {
                    queryFilter = queryFilter.And(k => k.IsActive);
                }
                //dbResult = dbResult.Where(m => m.IsActive == input.IsActive).ToList();
                //queryFilter = queryFilter.And(k => k.IsActive);
            }
            else
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }

            if (!String.IsNullOrEmpty(input.TicketCategory))
            {
                //dbResult = dbResult.Where(m => m.TicketCategory == input.TicketCategory).ToList();
                queryFilter = queryFilter.And(k => k.TicketCategory == input.TicketCategory);
            }

            if (!String.IsNullOrEmpty(input.AccidentCategory))
            {
                queryFilter = queryFilter.And(k => k.AccidentCategory == input.AccidentCategory);
                //dbResult = dbResult.Where(m => m.AccidentCategory == input.AccidentCategory).ToList();
            }

            if (!String.IsNullOrEmpty(input.NCRNumber))
            {
                //dbResult = dbResult.Where(m => m.NCRNumber == input.NCRNumber).ToList();
                queryFilter = queryFilter.And(k => k.NCRNumber == input.NCRNumber);
            }

            string dbResult = _generalRepo.Get(queryFilter.And(m => m.IsActive == true).And(m => m.TicketNumber.StartsWith("TIK."))).AsEnumerable().Sum(o => o.TicketQuantity).ToString();
            return dbResult;
        }

        public string GetCountNRCNumbers(TransportTicketNCRInput input)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();

            if (input.DateFrom != null && input.DateTo != null)
            {
                DateTime datefrom = Convert.ToDateTime(input.DateFrom);
                DateTime dateto = Convert.ToDateTime(input.DateTo);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");



                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);


            }
            else
            {

                DateTime datefrom = Convert.ToDateTime(DateTime.Now.Date.AddDays(-30));
                DateTime dateto = Convert.ToDateTime(DateTime.Now.Date);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");


                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);
            }

            if (input.IDVendor != 0)
            {
                queryFilter = queryFilter.And(k => k.IDVendor == input.IDVendor);
                //dbResult = dbResult.Where(m => m.IDVendor == input.IDVendor).ToList();
            }

            if (!input.IncludeDeleted)
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
                else
                {
                    queryFilter = queryFilter.And(k => k.IsActive);
                }
                //dbResult = dbResult.Where(m => m.IsActive == input.IsActive).ToList();
                //queryFilter = queryFilter.And(k => k.IsActive);
            }
            else
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }

            if (!String.IsNullOrEmpty(input.TicketCategory))
            {
                //dbResult = dbResult.Where(m => m.TicketCategory == input.TicketCategory).ToList();
                queryFilter = queryFilter.And(k => k.TicketCategory == input.TicketCategory);
            }

            if (!String.IsNullOrEmpty(input.AccidentCategory))
            {
                queryFilter = queryFilter.And(k => k.AccidentCategory == input.AccidentCategory);
                //dbResult = dbResult.Where(m => m.AccidentCategory == input.AccidentCategory).ToList();
            }

            if (!String.IsNullOrEmpty(input.NCRNumber))
            {
                //dbResult = dbResult.Where(m => m.NCRNumber == input.NCRNumber).ToList();
                queryFilter = queryFilter.And(k => k.NCRNumber == input.NCRNumber);
            }

            string dbResult = _generalRepo.Get(queryFilter.And(m => m.NCRNumber != null).And(m => m.IsActive == true)).Count().ToString();
            return dbResult;
        }

        public string GetTotalAncidents(TransportTicketNCRInput input)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();



            if (input.DateFrom != null && input.DateTo != null)
            {
                DateTime datefrom = Convert.ToDateTime(input.DateFrom);
                DateTime dateto = Convert.ToDateTime(input.DateTo);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");



                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);


            }
            else
            {
                DateTime datefrom = Convert.ToDateTime(DateTime.Now.Date.AddDays(-30));
                DateTime dateto = Convert.ToDateTime(DateTime.Now.Date);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");


                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);
            }

            if (input.IDVendor != 0)
            {
                queryFilter = queryFilter.And(k => k.IDVendor == input.IDVendor);
                //dbResult = dbResult.Where(m => m.IDVendor == input.IDVendor).ToList();
            }

            if (!input.IncludeDeleted)
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
                else
                {
                    queryFilter = queryFilter.And(k => k.IsActive);
                }
                //dbResult = dbResult.Where(m => m.IsActive == input.IsActive).ToList();
                //queryFilter = queryFilter.And(k => k.IsActive);
            }
            else
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }

            if (!String.IsNullOrEmpty(input.TicketCategory))
            {
                //dbResult = dbResult.Where(m => m.TicketCategory == input.TicketCategory).ToList();
                queryFilter = queryFilter.And(k => k.TicketCategory == input.TicketCategory);
            }

            if (!String.IsNullOrEmpty(input.AccidentCategory))
            {
                queryFilter = queryFilter.And(k => k.AccidentCategory == input.AccidentCategory);
                //dbResult = dbResult.Where(m => m.AccidentCategory == input.AccidentCategory).ToList();
            }

            if (!String.IsNullOrEmpty(input.NCRNumber))
            {
                //dbResult = dbResult.Where(m => m.NCRNumber == input.NCRNumber).ToList();
                queryFilter = queryFilter.And(k => k.NCRNumber == input.NCRNumber);
            }

            string dbResult = _generalRepo.Get(queryFilter.And(m => m.AccidentCategory.StartsWith("Crash")).And(m => m.IsActive == true)).Count().ToString();
            return dbResult;
        }

        public string GetSumAccidents(TransportTicketNCRInput input)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();

            if (input.DateFrom != null && input.DateTo != null)
            {
                DateTime datefrom = Convert.ToDateTime(input.DateFrom);
                DateTime dateto = Convert.ToDateTime(input.DateTo);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");



                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);


            }
            else
            {
                DateTime datefrom = Convert.ToDateTime(DateTime.Now.Date.AddDays(-30));
                DateTime dateto = Convert.ToDateTime(DateTime.Now.Date);
                //DateTime dateto = input.DateTo;
                //DateTime datefrom = input.DateFrom;
                // DateTime dateto = DateTime.Now;
                datefrom = Convert.ToDateTime(datefrom.Date.ToString("yyyy-MM-dd") + " 00:00");
                dateto = Convert.ToDateTime(dateto.Date.ToString("yyyy-MM-dd") + " 23:59");


                //dbResult = dbResult.Where(m => m.CorrectiveActionDate >= datefrom && m.CorrectiveActionDate <= dateto).ToList();
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);
            }

            if (input.IDVendor != 0)
            {
                queryFilter = queryFilter.And(k => k.IDVendor == input.IDVendor);
                //dbResult = dbResult.Where(m => m.IDVendor == input.IDVendor).ToList();
            }

            if (!input.IncludeDeleted)
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
                else
                {
                    queryFilter = queryFilter.And(k => k.IsActive);
                }
                //dbResult = dbResult.Where(m => m.IsActive == input.IsActive).ToList();
                //queryFilter = queryFilter.And(k => k.IsActive);
            }
            else
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }

            if (!String.IsNullOrEmpty(input.TicketCategory))
            {
                //dbResult = dbResult.Where(m => m.TicketCategory == input.TicketCategory).ToList();
                queryFilter = queryFilter.And(k => k.TicketCategory == input.TicketCategory);
            }

            if (!String.IsNullOrEmpty(input.AccidentCategory))
            {
                queryFilter = queryFilter.And(k => k.AccidentCategory == input.AccidentCategory);
                //dbResult = dbResult.Where(m => m.AccidentCategory == input.AccidentCategory).ToList();
            }

            if (!String.IsNullOrEmpty(input.NCRNumber))
            {
                //dbResult = dbResult.Where(m => m.NCRNumber == input.NCRNumber).ToList();
                queryFilter = queryFilter.And(k => k.NCRNumber == input.NCRNumber);
            }

            string dbResult = _generalRepo.Get(queryFilter.And(m => m.AccidentCategory.StartsWith("Incident")).And(m => m.IsActive == true)).Count().ToString();
            return dbResult;
        }

        public List<TransportTicketNCRDTO> GetTicketNCRs(TransportTicketNCRInput input)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();

            if (String.IsNullOrEmpty(input.DateFrom) && String.IsNullOrEmpty(input.DateTo))
            {
                DateTime datefrom = DateTime.Now.AddMonths(-1);
                DateTime dateto = DateTime.Now;

                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);
            }

            if (!String.IsNullOrEmpty(input.DateFrom))
            {
                DateTime datefrom = Convert.ToDateTime(input.DateFrom + " 00:00");
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom);
            }

            if (!String.IsNullOrEmpty(input.DateTo))
            {
                DateTime dateto = Convert.ToDateTime(input.DateTo + " 23:59");
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate <= dateto);
            }

            if (input.IDVendor != 0)
            {
                queryFilter = queryFilter.And(k => k.IDVendor == input.IDVendor);
            }

            if (!input.IncludeDeleted)
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
                else
                {
                    queryFilter = queryFilter.And(k => k.IsActive);
                }
            }
            else
            {
                if (!input.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }

            if (!String.IsNullOrEmpty(input.TicketCategory))
            {
                queryFilter = queryFilter.And(k => k.TicketCategory == input.TicketCategory);
            }

            if (!String.IsNullOrEmpty(input.AccidentCategory))
            {
                queryFilter = queryFilter.And(k => k.AccidentCategory == input.AccidentCategory);
            }

            if (!String.IsNullOrEmpty(input.NCRNumber))
            {
                queryFilter = queryFilter.And(k => k.NCRNumber == input.NCRNumber);
            }

            var dbResult = _generalRepo.Get(queryFilter).ToList();

            TOMContextDB context = new TOMContextDB();
            var query = (from a in dbResult
                         join c in context.MasterVendors on a.IDVendor equals c.IDVendor
                         // join b in context.MasterUsers on a.UpdatedBy equals b.IDUser
                         select new TransportTicketNCRDTO()
                         {
                             IDVendor = a.IDVendor,
                             Location = a.Location,
                             TicketNumber = a.TicketNumber,
                             CorrectiveActionDate = a.CorrectiveActionDate,
                             VendorName = c.VendorName,
                             PoliceRegNumber = a.PoliceRegNumber,
                             VehicleType = a.VehicleType,
                             ManufacturingYear = a.ManufacturingYear,
                             OffenderName = a.OffenderName,
                             OffenderRole = a.OffenderRole,
                             OffenderAge = a.OffenderAge,
                             OffenderYearOfService = a.OffenderYearOfService,
                             LocationCategory = a.LocationCategory,
                             IsActive = a.IsActive,
                             TicketCategory = a.TicketCategory,
                             AccidentCategory = a.AccidentCategory,
                             NCRNumber = a.NCRNumber,
                             RemarksTicket = a.RemarksTicket,
                             TypeOfLocation = a.TypeOfLocation,
                             TicketQuantity = a.TicketQuantity,
                             // FullName = b.FullName,
                             ProblemIssue1 = a.ProblemIssue1,
                             ProblemIssue2 = a.ProblemIssue2,
                             ProblemIssue3 = a.ProblemIssue3,
                             MainFactor = a.MainFactor,
                             SIRSNumber = a.SIRSNumber
                         });
            query = query.ToList();

            return Mapper.Map<List<TransportTicketNCRDTO>>(query);
        }

        public dynamic GetTicketNCRsTable(TransportTicketNCRInput filter, DataTableModel model = null)
        {
            dynamic res = new System.Dynamic.ExpandoObject();
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();
            var queryMV = PredicateHelper.True<MasterVendor>();

            if (String.IsNullOrEmpty(filter.DateFrom) && String.IsNullOrEmpty(filter.DateTo))
            {
                DateTime datefrom = DateTime.Now.AddMonths(-1);
                DateTime dateto = DateTime.Now;

                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom && k.CorrectiveActionDate <= dateto);
            }

            if (!String.IsNullOrEmpty(filter.DateFrom))
            {
                DateTime datefrom = Convert.ToDateTime(filter.DateFrom + " 00:00");
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate >= datefrom);
            }

            if (!String.IsNullOrEmpty(filter.DateTo))
            {
                DateTime dateto = Convert.ToDateTime(filter.DateTo + " 23:59");
                queryFilter = queryFilter.And(k => k.CorrectiveActionDate <= dateto);
            }


            if (filter.IDVendor != 0)
            {
                queryFilter = queryFilter.And(q => q.IDVendor == filter.IDVendor);
            }
            if (filter.TicketCategory != null)
            {
                queryFilter = queryFilter.And(q => q.TicketCategory == filter.TicketCategory);
            }
            if (filter.AccidentCategory != null)
            {
                queryFilter = queryFilter.And(q => q.AccidentCategory == filter.AccidentCategory);
            }
            if (filter.NCRNumber != null)
            {
                queryFilter = queryFilter.And(q => filter.NCRNumber.Contains(q.NCRNumber));
            }

            if (!filter.IncludeDeleted)
            {
                if (!filter.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
                else
                {
                    queryFilter = queryFilter.And(k => k.IsActive);
                }
            }
            else
            {
                if (!filter.TicketStatus)
                {
                    queryFilter = queryFilter.And(k => !k.IsActive);
                }
            }

            var _listVM = new List<string>();
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.search.value))
                {
                    var search = model.search.value;
                    if (search != "")
                    {
                        _listVM = new List<string>();
                        queryMV = queryMV.And(q => search.Contains(q.VendorName));
                        var getMV = _masterVendorRepo.Get(queryMV);
                        _listVM.AddRange(getMV.Select(_ => _.IDVendor.ToString()).ToList());
                        _listVM = _listVM.Distinct().ToList();

                        queryFilter = queryFilter.And(w => (
                            search.Contains(w.TicketNumber) ||
                            search.Contains(w.CorrectiveActionDate.ToString()) ||
                            _listVM.Contains(w.IDVendor.ToString()) ||
                            search.Contains(w.PoliceRegNumber) ||
                            search.Contains(w.TicketCategory) ||
                            search.Contains(w.AccidentCategory) ||
                            search.Contains(w.OffenderName) ||
                            search.Contains(w.OffenderRole) ||
                            search.Contains(w.OffenderAge.ToString()) ||
                            search.Contains(w.OffenderYearOfService.ToString()) ||
                            search.Contains(w.Location) ||
                            search.Contains(w.SIRSNumber)
                       ));
                    }
                }

                foreach (var fil in model.columns.Where(_ => !String.IsNullOrEmpty(_.search.value)))
                {
                    var _col = fil.data;
                    var _val = fil.search.value;
                    switch (_col)
                    {
                        case "TicketNumber":
                            queryFilter = queryFilter.And(_ => _.TicketNumber.Contains(_val));
                            break;
                        case "CorrectiveActionDate":
                            queryFilter = queryFilter.And(_ => _.CorrectiveActionDate.ToString().Contains(_val));
                            break;
                        case "VendorName":
                            _listVM = new List<string>();
                            queryMV = queryMV.And(q => _val.Contains(q.VendorName));
                            var getMV = _masterVendorRepo.Get(queryMV);
                            _listVM.AddRange(getMV.Select(_ => _.IDVendor.ToString()).ToList());
                            _listVM = _listVM.Distinct().ToList();
                            queryFilter = queryFilter.And(_ => _listVM.Contains(_.IDVendor.ToString()));
                            //queryFilter.Or(c => c.MasterVendors.Any(c2 => _val.Contains(c2.VendorName)));
                            break;
                        case "PoliceRegNumber":
                            queryFilter = queryFilter.And(_ => _.PoliceRegNumber.Contains(_val));
                            break;
                        case "TicketCategory":
                            queryFilter = queryFilter.And(_ => _.TicketCategory.Contains(_val));
                            break;
                        case "AccidentCategory":
                            queryFilter = queryFilter.And(_ => _.AccidentCategory.Contains(_val));
                            break;
                        case "OffenderName":
                            queryFilter = queryFilter.And(_ => _.OffenderName.Contains(_val));
                            break;
                        case "OffenderRole":
                            queryFilter = queryFilter.And(_ => _.OffenderRole.Contains(_val));
                            break;
                        case "OffenderAge":
                            queryFilter = queryFilter.And(_ => _.OffenderAge.ToString().Contains(_val));
                            break;
                        case "OffenderYearOfService":
                            queryFilter = queryFilter.And(_ => _.OffenderYearOfService.Contains(_val));
                            break;
                        case "Location":
                            queryFilter = queryFilter.And(_ => _.Location.Contains(_val));
                            break;
                        case "SIRSNumber":
                            queryFilter = queryFilter.And(_ => _.SIRSNumber.Contains(_val));
                            break;
                    }
                }

            }

            var getTN = Enumerable.Empty<TransportTicketNCR>();
            if (model == null)
            {
                getTN = _generalRepo.Get(queryFilter);
            }
            else
            {
                getTN = _generalRepo.GetAllPagination(queryFilter, model.start, model.length, "TicketNumber");
            }

            var data = Mapper.Map<List<TransportTicketNCRDTO>>(getTN);

            foreach (var tm in data)
            {
                var MstVendor = _masterVendorRepo.Get(_ => _.IDVendor == tm.IDVendor).FirstOrDefault();
                tm.VendorName = MstVendor.VendorName;
            }

            var Count = _generalRepo.Count(queryFilter);
            res.data = data;
            res.total = Count;
            return res;
        }
        public List<TransportTicketNCRDTO> GetDataByIDs(string id)
        {
            //var queryFilter = PredicateHelper.True<TransportTicketNCR>();

            //queryFilter = queryFilter.And(m => m.TicketNumber == id);

            //var dbResult = _generalRepo.Get(queryFilter).ToList();

            TOMContextDB context = new TOMContextDB();
            var query = (from a in context.TransportTicketNCRs

                         join c in context.MasterVendors on a.IDVendor equals c.IDVendor
                         join b in context.MasterUsers on a.UpdatedBy equals b.IDUser
                         where a.TicketNumber == id
                         select new TransportTicketNCRDTO()
                         {
                             IDVendor = a.IDVendor,
                             Location = a.Location,
                             TicketNumber = a.TicketNumber,
                             CorrectiveActionDate = a.CorrectiveActionDate,
                             VendorName = c.VendorName,
                             PoliceRegNumber = a.PoliceRegNumber,
                             VehicleType = a.VehicleType,
                             ManufacturingYear = a.ManufacturingYear,
                             OffenderName = a.OffenderName,
                             OffenderRole = a.OffenderRole,
                             OffenderAge = a.OffenderAge,
                             OffenderYearOfService = a.OffenderYearOfService,
                             LocationCategory = a.LocationCategory,
                             IsActive = a.IsActive,
                             TicketCategory = a.TicketCategory,
                             AccidentCategory = a.AccidentCategory,
                             NCRNumber = a.NCRNumber,
                             TicketQuantity = a.TicketQuantity,
                             RemarksTicket = a.RemarksTicket,
                             TypeOfLocation = a.TypeOfLocation,
                             CreatedBy = a.CreatedBy,
                             FullName = b.FullName,
                             RoadCondition = a.RoadCondition,
                             WeatherCondition = a.WeatherCondition,
                             Remarks = a.Remarks,
                             ValueOfLoss = a.ValueOfLoss,
                             CorrectiveAction = a.CorrectiveAction,
                             SIRSNumber = a.SIRSNumber,
                             AttachmentNRC = a.AttachmentNRC,
                             ProblemIssue1 = a.ProblemIssue1,
                             ProblemIssue2 = a.ProblemIssue2,
                             ProblemIssue3 = a.ProblemIssue3,
                             MainFactor = a.MainFactor,
                             Attachment = a.Attachment,
                             CreatedDate = a.CreatedDate

                         });
            var dbResult = query.ToList();
            return Mapper.Map<List<TransportTicketNCRDTO>>(dbResult);
        }

        public List<TransportTicketNCRDTO> GetTicketNCRPDFs(string id)
        {

            TOMContextDB context = new TOMContextDB();
            var query = (from a in context.TransportTicketNCRs

                         join c in context.MasterVendors on a.IDVendor equals c.IDVendor
                         join b in context.MasterUsers on a.UpdatedBy equals b.IDUser
                         where a.TicketNumber == id
                         select new TransportTicketNCRDTO()
                         {
                             IDVendor = a.IDVendor,
                             Location = a.Location,
                             TicketNumber = a.TicketNumber,
                             CorrectiveActionDate = a.CorrectiveActionDate,
                             VendorName = c.VendorName,
                             PoliceRegNumber = a.PoliceRegNumber,
                             VehicleType = a.VehicleType,
                             ManufacturingYear = a.ManufacturingYear,
                             OffenderName = a.OffenderName,
                             OffenderRole = a.OffenderRole,
                             OffenderAge = a.OffenderAge,
                             OffenderYearOfService = a.OffenderYearOfService,
                             LocationCategory = a.LocationCategory,
                             IsActive = a.IsActive,
                             TicketCategory = a.TicketCategory,
                             AccidentCategory = a.AccidentCategory,
                             NCRNumber = a.NCRNumber,
                             TicketQuantity = a.TicketQuantity,
                             RemarksTicket = a.RemarksTicket,
                             TypeOfLocation = a.TypeOfLocation,
                             CreatedBy = a.CreatedBy,
                             FullName = b.FullName


                         });
            var dbResult = query.ToList();

            return Mapper.Map<List<TransportTicketNCRDTO>>(dbResult);
        }
    }
}
