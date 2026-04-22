using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using OfficeOpenXml;

namespace TOM.Master.BusinessLogics
{
    public class MasterKilometerBLL : IMasterKilometerBLL
    {
        private readonly IMasterKilometerRepo _masterKilometerRepo;

        public MasterKilometerBLL(IMasterKilometerRepo masterKilometerRepo)
        {
            _masterKilometerRepo = masterKilometerRepo;
        }

        public List<MasterKilometerDTO> GetAllMasterKilometer()
        {
            return Mapper.Map<List<MasterKilometerDTO>>(_masterKilometerRepo.GetAllMasterKilometer());
        }

        public List<string> SaveUpload(HttpPostedFileBase input, string userid)
        {
            var stream = input.InputStream;
            List<string> listError = new List<string>();
            _masterKilometerRepo.DeleteData();//DELETE SEMUA DATA DI DATABASE, NANTI DI INSERT ULANG SEMUA
            using (ExcelPackage xlPackage = new ExcelPackage(stream))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
                var totalRows = myWorksheet.Dimension.End.Row;
                for (int rowNum = 2; rowNum <= totalRows; rowNum++) //selet starting row here
                {
                    //Penanda kalau location from/ location to tidak ada di master location, datanya tidak disave ke tabel master lead time
                    bool errorLocFrom = false;
                    bool errorLocTo = false;
                    bool errorType = false;
                    bool errorDataexist = false;
                    try
                    {
                        string idLocFrom = myWorksheet.Cells[rowNum, 1].Text;
                        string locFrom = myWorksheet.Cells[rowNum, 2].Text;
                        string longFrom = myWorksheet.Cells[rowNum, 3].Text;
                        string latFrom = myWorksheet.Cells[rowNum, 4].Text;
                        //if (!string.IsNullOrEmpty(locFrom))
                        //{
                        //    int indexlocFrom = locFrom.IndexOf("ZD", StringComparison.Ordinal);
                        //    locFrom = locFrom.Substring(indexlocFrom, 4);
                        //    MasterLocation checkLocFrom = _masterLocationRepo.GetMasterLocationByID(locFrom);
                        //    if (checkLocFrom == null)
                        //    {
                        //        listError.Add("LocationFrom-" + locFrom + "-" + rowNum);
                        //        errorLocFrom = true;
                        //    }
                        //}
                        string idLocTo = myWorksheet.Cells[rowNum, 5].Text;
                        string locTo = myWorksheet.Cells[rowNum, 6].Text;
                        string longTo = myWorksheet.Cells[rowNum, 7].Text;
                        string latTo = myWorksheet.Cells[rowNum, 8].Text;
                        //if (!string.IsNullOrEmpty(locTo))
                        //{
                        //    int indexlocTo = locTo.IndexOf("ZD", StringComparison.Ordinal);
                        //    locTo = locTo.Substring(indexlocTo, 4);
                        //    MasterLocation checkLocTo = _masterLocationRepo.GetMasterLocationByID(locTo);
                        //    if (checkLocTo == null)
                        //    {
                        //        listError.Add("LocationTo-" + locTo + "-" + rowNum);
                        //        errorLocTo = true;
                        //    }
                        //}

                        //if (errorLocFrom == false && errorLocTo == false)
                        //{
                        MasterKilometer masterKilometer = new MasterKilometer()
                        {
                            IDLocationFrom = idLocFrom,
                            LocationFrom = locFrom,
                            LongitudeFrom = longFrom,
                            LatitudeFrom = latFrom,
                            IDLocationTo = idLocTo,
                            LocationTo = locTo,
                            LongitudeTo = longTo,
                            LatitudeTo = latTo
                        };
                        string replace = myWorksheet.Cells[rowNum, 9].Text;
                        var val = new int();
                        masterKilometer.Kilometer = int.TryParse(replace.Replace(",", String.Empty), out val) ? val : 0;
                        masterKilometer.IsActive = true;
                        masterKilometer.CreatedBy = userid;
                        masterKilometer.UpdatedBy = userid;
                        _masterKilometerRepo.SaveData(masterKilometer, true);
                        //}
                    }
                    catch (Exception)
                    {
                        //kalau terjadi error dalam pembacaan excel, return list string yang isinya null
                        return null;
                    }
                }
            }
            return listError;
        }
    }
}
