using System;
using System.Collections.Generic;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportLostClaimDamageDTO
    {
        #region INITIATION

        public string GPNumber { get; set; }
        public string SJNumber { get; set; }
        public string Status { get; set; }
        public string RealStatus { get; set; }
        public DateTime? SJDate { get; set; }
        public string ClaimType { get; set; }
        public string DeliveryNumberNote { get; set; }
        public string OriginWarehouse { get; set; }
        public string DestinationWarehouse { get; set; }
        public string DestinationWarehouseIDLocation { get; set; }
        public string STONumber { get; set; }
        public DateTime DateOfDelivery { get; set; }
        public DateTime DateOfIncident { get; set; }
        public int? TransportationVendor { get; set; }
        public string TransportationVendorName { get; set; }
        public string PoliceRegNumber { get; set; }
        public string Driver { get; set; }
        public string Driver2 { get; set; }
        public string CoDriver { get; set; }
        public string Receiver { get; set; }
        public string Witness { get; set; }
        public string DNtoReverseLogistics { get; set; }
        public string Attachment { get; set; }
        public string Supervisor { get; set; }

        public List<TransportLostClaimFACodeDTO> ListFACode { get; set; }

        public string JsonCanvasUploadBox1 { get; set; }
        public string ImageUploadConditionBoxPath1 { get; set; }
        public string RemarkUploadConditionBox1 { get; set; }

        public string JsonCanvasUploadBox2 { get; set; }
        public string ImageUploadConditionBoxPath2 { get; set; }
        public string RemarkUploadConditionBox2 { get; set; }

        public string JsonCanvasUploadBox3 { get; set; }
        public string ImageUploadConditionBoxPath3 { get; set; }
        public string RemarkUploadConditionBox3 { get; set; }

        public string JsonCanvasPositionTruck1 { get; set; }
        public string ImageUploadPositionTruckPath1 { get; set; }
        public string RemarkUploadPositionTruck1 { get; set; }

        public string JsonCanvasPositionTruck2 { get; set; }
        public string ImageUploadPositionTruckPath2 { get; set; }
        public string RemarkUploadPositionTruck2 { get; set; }

        public string JsonCanvasPositionTruck3 { get; set; }
        public string ImageUploadPositionTruckPath3 { get; set; }
        public string RemarkUploadPositionTruck3 { get; set; }

        public string JsonCanvasFinalResult1 { get; set; }
        public string ImageUploadFinalResultPath1 { get; set; }
        public string RemarkUploadFinalResult1 { get; set; }

        public string JsonCanvasFinalResult2 { get; set; }
        public string ImageUploadFinalResultPath2 { get; set; }
        public string RemarkUploadFinalResult2 { get; set; }

        public string JsonCanvasFinalResult3 { get; set; }
        public string ImageUploadFinalResultPath3 { get; set; }
        public string RemarkUploadFinalResult3 { get; set; }

        #endregion

        public string ClaimExpense { get; set; }
        public string TransportationMode { get; set; }
        public string GoodsCategory { get; set; }
        public decimal? InitClaimedAmount { get; set; }
        public string ClaimCategory { get; set; }
        public int TotalClaimInPack { get; set; }
        public int TotalClaimInStick { get; set; }
        public DateTime? BSWarehouseReceiveDate { get; set; }
        public string HMSMemoNumber { get; set; }
        public DateTime? HMSMemoDate { get; set; }
        public string HMSMemoFile { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? VendorPaymentDate { get; set; }
        public decimal? GrossLossAmount { get; set; }
        public decimal NetClaim { get; set; }
        public string InsurancePaymentProof { get; set; }
        public string EDPSNumber { get; set; }
        public DateTime? EDPSDate { get; set; }
        public DateTime? IncinerationReportDate { get; set; }
        public string IncinerationReportFile { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string PrintedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string SJNumberOld { get; set; }
        public string GPNumberOld { get; set; }
        public bool IsAnyAttachment { get; set; }
    }

    public class TransportLostClaimFACodeDTO
    {
        public string FACode { get; set; }
        public string SpeakingCode { get; set; }
        public string LostOrDamage { get; set; }
        public int Pack { get; set; }
        public string Description { get; set; }
    }
}
