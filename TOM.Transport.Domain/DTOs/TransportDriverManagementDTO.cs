using System;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportDriverManagementDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string MobilePhone { get; set; }
        public int IDVendor { get; set; }
        public string VendorName { get; set; }
        public DateTime? JoinDate { get; set; }
        public string DrivingLicenseNumber { get; set; }
        public DateTime? DrivingLicensePeriod { get; set; }
        public string BaseTown { get; set; }
        public string PerformanceLevel { get; set; }
        public string AttachmentKTP { get; set; }
        public string AttachmentSIM { get; set; }
        public string AttachmentFoto { get; set; }
        public bool BPJSKetenagaKerjaan { get; set; }
        public bool BPJSKesehatan { get; set; }
        public bool DrugFreeTest { get; set; }
        public bool FatiqueTest { get; set; }
        public bool InductionTest { get; set; }
        public bool DefensiveDrivingTest { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByFullName { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string RoleDriver { get; set; }
        public DateTime? DriverContractValidityPeriod { get; set; }

        public virtual MasterVendorTOMDTO MasterVendor { get; set; }

        public string PrevIDCardNumber { get; set; }
        public int PrevIDVendor { get; set; }

        public bool IsNewData { get; set; }
    }

    public class TransportDriverManagementFilterDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string RoleDriver { get; set; }
    }
}
