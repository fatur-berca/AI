using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportVehicleDataDTO
    {
        public string IDPoliceRegNumber { get; set; }
        public Nullable<int> ManufacturingYear { get; set; }
        public string Karoseri { get; set; }
        public string Merk { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Cargo { get; set; }
        public string VehicleIdentityNumber { get; set; }
        public string EngineNumber { get; set; }
        public string BaseTown { get; set; }
        public System.DateTime STNKValidityPeriod { get; set; }
        public string GPS { get; set; }
        public int IDVendor { get; set; }
        public string Status { get; set; }
        public string AttachmentSTNK { get; set; }
        public string AttachmentPhoto { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByFullName { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string AttachmentPhotoTwo { get; set; }
        public string AttachmentPhotoThree { get; set; }

        public bool IsNewData { get; set; }
        public string VendorName {  get; set; }
    }
}
