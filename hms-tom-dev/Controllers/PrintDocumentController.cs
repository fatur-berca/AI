using System;
using System.Web.Mvc;
using AutoMapper;
using TOM.Transport.BusinessLogics.TransportExecutionBLL;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Models.Transport;
using TOM.Transport.BusinessLogics;
using TOM.Transport.BusinessLogics.TransportLostClaimDamageBLL;

namespace hms_tom_dev.Controllers
{
    public class PrintDocumentController : Controller
    {
        private readonly ITransportExecutionBLL _transportExecutionBll;
        private readonly ITransportOrderBLL _transportOrderBll;
        private readonly ITransportLostClaimDamageBLL _transportLostClaimBll;
        public PrintDocumentController(ITransportExecutionBLL transportExecutionBll, ITransportOrderBLL transportOrderBll, ITransportLostClaimDamageBLL transportLostClaimBll)
        {
            _transportExecutionBll = transportExecutionBll;
            _transportOrderBll = transportOrderBll;
            _transportLostClaimBll = transportLostClaimBll;
        }

        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult PrintTransportExecution(string id, string uname, string typePrint)
        public ActionResult PrintTransportExecution(string id, string uname, string typePrint)
        {
            var viewName = "";
            /*List<int> idTE = new List<int>();
            var data = id;
            var stringId = data.Split(',');
            foreach (string VARIABLE in stringId)
            {
                idTE.Add(Int32.Parse(VARIABLE));
            }*/
            int idTN = Int32.Parse(id);
            TransportExecutionPrintDTO dto = new TransportExecutionPrintDTO();
            if (typePrint == "delivery")
            {
                dto = _transportExecutionBll.PrintDocument(idTN, uname);
                viewName = "PrintDeliveryNote";
            }
            else
            {
                dto = _transportExecutionBll.PrintDocumentIPB(idTN, uname);
                viewName = "PrintIPB";
            }            
            TransportExecutionPrintViewModel viewModel = Mapper.Map<TransportExecutionPrintViewModel>(dto);    
            return View(viewName, viewModel);
        }

        public ActionResult PrintDocumentLostClaim(string id, string uname)
        {
            var gpNumber = id.Replace("~", "/").Split('-')[0];
            var sjNumber = id.Replace("~", "/").Split('-')[1];
            var dto = _transportLostClaimBll.GetTransportLostClaimDamageByKey(gpNumber, sjNumber);
            var viewModel = Mapper.Map<TransportLostClaimDamageViewModel>(dto);
            var year = dto.DateOfDelivery.Year.ToString();
            var monthNumber = dto.DateOfDelivery.Month.ToString();

            viewModel.PrintedBy = uname;
            if (!String.IsNullOrEmpty(dto.SJNumber))
                viewModel.SJNumberOld = dto.SJNumber;
            if (!String.IsNullOrEmpty(dto.GPNumber))
                viewModel.GPNumberOld = dto.GPNumber;

            string destinationWarehouseCode;
            string originWarehouseCode;
            if (!String.IsNullOrEmpty(dto.DestinationWarehouse))
                viewModel.renumber = dto.DestinationWarehouseIDLocation + '/' + year + '/' + monthNumber + '/' + dto.SJNumber;

            viewModel.DNtoReverseLogistics = String.IsNullOrEmpty(dto.DNtoReverseLogistics) ? "-" : dto.DNtoReverseLogistics;

            if (!String.IsNullOrEmpty(dto.OriginWarehouse) && dto.OriginWarehouse.Contains("-"))
            {
                int count = dto.OriginWarehouse.Split('-').Length;
                if (count > 0)
                {
                    originWarehouseCode = dto.OriginWarehouse.Split('-')[0].Trim();
                    viewModel.originWarehouseName = dto.OriginWarehouse.Split('-')[1].Trim();
                }
            }

            viewModel.DateOfDelivery = (dto.DateOfDelivery > DateTime.MinValue) ? dto.DateOfDelivery.ToString("dd-MMM-yyyy") : "-";
            viewModel.DateOfIncident = (dto.DateOfIncident > DateTime.MinValue) ? dto.DateOfIncident.ToString("dd-MMM-yyyy") : "-";
            // https://stackoverflow.com/questions/33371527/no-overload-for-method-tostring-takes-1-arguments-when-casting-date
            viewModel.SJDate = (dto.SJDate.HasValue) ? dto.SJDate.Value.ToString("dd-MMM-yyyy") : "-";

            if (!String.IsNullOrEmpty(viewModel.ImageUploadPositionTruckPath1))
                viewModel.ImageUploadPositionTruckPath1_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadPositionTruckPath1);
            if (!String.IsNullOrEmpty(viewModel.ImageUploadPositionTruckPath2))
                viewModel.ImageUploadPositionTruckPath2_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadPositionTruckPath2);
            if (!String.IsNullOrEmpty(viewModel.ImageUploadPositionTruckPath3))
                viewModel.ImageUploadPositionTruckPath3_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadPositionTruckPath3);

            if (!String.IsNullOrEmpty(viewModel.ImageUploadFinalResultPath1))
                viewModel.ImageUploadFinalResultPath1_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadFinalResultPath1);
            if (!String.IsNullOrEmpty(viewModel.ImageUploadFinalResultPath2))
                viewModel.ImageUploadFinalResultPath2_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadFinalResultPath2);
            if (!String.IsNullOrEmpty(viewModel.ImageUploadFinalResultPath3))
                viewModel.ImageUploadFinalResultPath3_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadFinalResultPath3);

            if (!String.IsNullOrEmpty(viewModel.ImageUploadConditionBoxPath1))
                viewModel.ImageUploadConditionBoxPath1_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadConditionBoxPath1);
            if (!String.IsNullOrEmpty(viewModel.ImageUploadConditionBoxPath2))
                viewModel.ImageUploadConditionBoxPath2_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadConditionBoxPath2);
            if (!String.IsNullOrEmpty(viewModel.ImageUploadConditionBoxPath3))
                viewModel.ImageUploadConditionBoxPath3_Img64Base = ConvertPathImgToBase64Url(viewModel.ImageUploadConditionBoxPath3);

            return View("PrintDocumentLostClaim", viewModel);
        }

        private string ConvertPathImgToBase64Url(string path)
        {
            byte[] imageByteData = System.IO.File.ReadAllBytes(Server.MapPath(path));
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);

            return imageDataURL;
        }
    }
}