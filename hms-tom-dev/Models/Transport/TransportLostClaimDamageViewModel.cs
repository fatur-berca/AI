using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hms_tom_dev.Models.Transport
{
    public class TransportLostClaimDamageViewModel
    {
        public TransportLostClaimDamageViewModel()
        {
            this.ListFACode = new List<TransportLostClaimFACodeViewModel>() { new TransportLostClaimFACodeViewModel() };
            ClaimTypeDropDown = new List<SelectListItem>();
        }

        public string Status { get; set; }
        public string RealStatus { get; set; }
        public string CreatedByName { get; set; }
        public string PrintedBy { get; set; }

        #region INITIATION

        [Required(ErrorMessage = "Please Select Claim Type")]
        public string ClaimType { get; set; }
        public IList<SelectListItem> ClaimTypeDropDown { get; set; }

        public IList<SelectListItem> ClaimCategoryDropDown { get; set; }

        [Required(ErrorMessage = "Please Input STO Number")]
        public string SJNumber { get; set; }
        public string SJNumberHidden { get; set; }
        public IList<SelectListItem> SJNumberDropDown { get; set; }

        [Required(ErrorMessage = "Please Input GPNumber")]
        public string GPNumber { get; set; }
        public string GPNumberHidden { get; set; }
        //public IList<SelectListItem> GPNumberDropDown { get; set; }

        public string SJDate { get; set; }
        public string SJDateHidden { get; set; }

        [Required(ErrorMessage = "Please Input Delivery Number")]
        public string DeliveryNumberNote { get; set; }

        [Required(ErrorMessage = "Please Input Origin Warehouse")]
        public string OriginWarehouse { get; set; }
        public string OriginWarehouseHidden { get; set; }

        [Required(ErrorMessage = "Please Input Destination Warehouse")]
        public string DestinationWarehouse { get; set; }
        public string DestinationWarehouseHidden { get; set; }

        [Required(ErrorMessage = "Please Input STO Number")]
        public string STONumber { get; set; }
        public string STONumberHidden { get; set; }

        public string DateOfDelivery { get; set; }
        public string DateOfDeliveryHidden { get; set; }

        [Required(ErrorMessage = "Please Input Date of Incident")]
        public string DateOfIncident { get; set; }
    
        public int? TransportationVendor { get; set; }
        [Required(ErrorMessage = "Please Input Transportation Vendor")]
        public string TransportationVendorName { get; set; }
        public string TransportationVendorHidden { get; set; }

        [Required(ErrorMessage = "Please Input Police Reg Number")]
        public string PoliceRegNumber { get; set; }
        public string PoliceRegNumberHidden { get; set; }

        [Required(ErrorMessage = "Please Input Driver 1")]
        public string Driver { get; set; }
        public string Driver2 { get; set; }

        [Required(ErrorMessage = "Please Input Co-Driver")]
        public string CoDriver { get; set; }

        [Required(ErrorMessage = "Please Input Receiver")]
        public string Receiver { get; set; }

        [Required(ErrorMessage = "Please Input Witness")]
        public string Witness { get; set; }

        //[Required(ErrorMessage = "Please Input DN to Reserve Logistics")]
        public string DNtoReverseLogistics { get; set; }

        public bool IsAnyAttachment { get; set; }
        public string Attachment { get; set; }

        [Required(ErrorMessage = "Please Input Supervisor")]
        public string Supervisor { get; set; }

        public List<TransportLostClaimFACodeViewModel> ListFACode { get; set; }

        public string JsonCanvasUploadBox1 { get; set; }
        public string ImageUploadConditionBoxPath1 { get; set; }
        public string ImageUploadConditionBoxPath1_Img64Base { get; set; }
        public string RemarkUploadConditionBox1 { get; set; }

        public string JsonCanvasUploadBox2 { get; set; }
        public string ImageUploadConditionBoxPath2 { get; set; }
        public string ImageUploadConditionBoxPath2_Img64Base { get; set; }
        public string RemarkUploadConditionBox2 { get; set; }

        public string JsonCanvasUploadBox3 { get; set; }
        public string ImageUploadConditionBoxPath3 { get; set; }
        public string ImageUploadConditionBoxPath3_Img64Base { get; set; }
        public string RemarkUploadConditionBox3 { get; set; }


        public string JsonCanvasPositionTruck1 { get; set; }
        public string ImageUploadPositionTruckPath1 { get; set; }
        public string ImageUploadPositionTruckPath1_Img64Base { get; set; }
        public string RemarkUploadPositionTruck1 { get; set; }

        public string JsonCanvasPositionTruck2 { get; set; }
        public string ImageUploadPositionTruckPath2 { get; set; }
        public string ImageUploadPositionTruckPath2_Img64Base { get; set; }
        public string RemarkUploadPositionTruck2 { get; set; }

        public string JsonCanvasPositionTruck3 { get; set; }
        public string ImageUploadPositionTruckPath3 { get; set; }
        public string ImageUploadPositionTruckPath3_Img64Base { get; set; }
        public string RemarkUploadPositionTruck3 { get; set; }


        public string JsonCanvasFinalResult1 { get; set; }
        public string ImageUploadFinalResultPath1 { get; set; }
        public string ImageUploadFinalResultPath1_Img64Base { get; set; }
        public string RemarkUploadFinalResult1 { get; set; }

        public string JsonCanvasFinalResult2 { get; set; }
        public string ImageUploadFinalResultPath2 { get; set; }
        public string ImageUploadFinalResultPath2_Img64Base { get; set; }
        public string RemarkUploadFinalResult2 { get; set; }

        public string JsonCanvasFinalResult3 { get; set; }
        public string ImageUploadFinalResultPath3 { get; set; }
        public string ImageUploadFinalResultPath3_Img64Base { get; set; }
        public string RemarkUploadFinalResult3 { get; set; }
        #endregion INITIATION

        #region VERIFICATION
        [Required(ErrorMessage = "Please Input Claim Expense")]
        public string ClaimExpense { get; set; }
        public IList<SelectListItem> ClaimExpenseDropDown { get; set; }

        [Required(ErrorMessage = "Please Input Transportation Mode")]
        public string TransportationMode { get; set; }
        public string TransportationModeHidden { get; set; }

        [Required(ErrorMessage = "Please Input Goods Category")]
        public string GoodsCategory { get; set; }

        [Required(ErrorMessage = "Please Input Init Claimed Amount")]
        public string InitClaimedAmount { get; set; }
        public string InitClaimedAmountHidden { get; set; }

        [Required(ErrorMessage = "Please Input Claim Category")]
        public string ClaimCategory { get; set; }

        //[Required(ErrorMessage = "Please Input Total Claim In Pack")]
        public string TotalClaimInPack { get; set; }
        public string TotalClaimInPackHidden { get; set; }

        //[Required(ErrorMessage = "Please Input Total Claim In Stick")]
        public string TotalClaimInStick { get; set; }
        public string TotalClaimInStickHidden { get; set; }
        #endregion

        #region PROCESS
        //[Required(ErrorMessage = "Please Input BS Warehouse Receive Date")]
        public string BSWarehouseReceiveDate { get; set; }

        //[Required(ErrorMessage = "Please Input HMS Memo Number")]
        public string HMSMemoNumber { get; set; }

        //[Required(ErrorMessage = "Please Input HMS Memo Date")]
        public string HMSMemoDate { get; set; }

        public string HMSMemoFile { get; set; }
        #endregion

        #region INVOICING
        [Required(ErrorMessage = "Please Input Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Please Input Invoice Date")]
        public string InvoiceDate { get; set; }

        [Required(ErrorMessage = "Please Input Invoice Amount")]
        public string InvoiceAmount { get; set; }

        //[Required(ErrorMessage = "Please Input Vendor Payment Date")]
        public string VendorPaymentDate { get; set; }

        //[Required(ErrorMessage = "Please Input Gross Loss Amount")]
        public string GrossLossAmount { get; set; }

        //[Required(ErrorMessage = "Please Input Net Claim")]
        public string NetClaim { get; set; }

        public string InsurancePaymentProof { get; set; }
        #endregion

        #region DISPOSE
        [Required(ErrorMessage = "Please Input EDPS Number")]
        public string EDPSNumber { get; set; }

        [Required(ErrorMessage = "Please Input EDPS Date")]
        public string EDPSDate { get; set; }

        //[Required(ErrorMessage = "Please Input Incineration Date")]
        public string IncinerationReportDate { get; set; }

        [Required(ErrorMessage = "Please Input Incineration File")]
        public string IncinerationReportFile { get; set; }
        #endregion
        
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string SJNumberOld { get; set; }
        public string GPNumberOld { get; set; }
        public string renumber { get; set; }
        public string originWarehouseName { get; set; }
        public string deliveryDate { get; set; }

        public bool Success { get; set; }
    }

    public class TransportLostClaimFACodeViewModel 
    {
        [Required(ErrorMessage = "Please Input FA Code")]
        public string FACode { get; set; }

        [Required(ErrorMessage = "Please Input Speaking Code")]
        public string SpeakingCode { get; set; }

        public string LostOrDamage { get; set; }

        [Required(ErrorMessage = "Please Input Pack")]
        public string Pack { get; set; }

        [Required(ErrorMessage = "Please Input Description")]
        public string Description { get; set; }
        public SelectList CategoryTemplates { get; set; }
    }
}