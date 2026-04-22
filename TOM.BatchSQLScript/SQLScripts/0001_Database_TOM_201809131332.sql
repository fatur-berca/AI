USE MASTER

IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'TOM')
BEGIN
    ALTER DATABASE TOM SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE TOM
END

CREATE DATABASE TOM
COLLATE Latin1_General_CI_AS;

USE TOM

IF OBJECT_ID('[dbo].[MasterConfiguration]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterConfiguration]; 

CREATE TABLE [dbo].[MasterConfiguration]
(
	[IDConfiguration] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[PageName] VARCHAR(200) NOT NULL,
	[Description] VARCHAR(MAX) NOT NULL,
	[Value] VARCHAR(200) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL,
);

IF OBJECT_ID('[dbo].[MasterConfigurationEmail]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterConfigurationEmail]; 

CREATE TABLE [dbo].[MasterConfigurationEmail]
(
	[IDConfigMail] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[PageName] VARCHAR(200) NOT NULL,
	[Receiver] VARCHAR(200) NOT NULL,
	[BodyEmail] VARCHAR(max) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL,
);

IF OBJECT_ID('[dbo].[MasterList]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterList]; 

CREATE TABLE [dbo].[MasterList]
(
	[IDList] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[FieldName] VARCHAR(200) NOT NULL,
	[FieldValue] VARCHAR(200) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterFABrand]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterFABrand]; 

CREATE TABLE [dbo].[MasterFABrand]
(
	[FACode] VARCHAR(50) NOT NULL PRIMARY KEY,
	[SpeakingCode] VARCHAR(10) NOT NULL,
	[Type] VARCHAR(200) NOT NULL,
	[StickPerBox] DECIMAL(18, 4) NOT NULL,
	[PackPerBox] DECIMAL(18, 4) NOT NULL,
	[StickPerPack] DECIMAL(18, 4) NOT NULL,
	[HJE] INT NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterFunction]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterFunction]; 

CREATE TABLE [dbo].[MasterFunction]
(
	[IDFunction] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[FunctionName] VARCHAR(200) NOT NULL,
	[ParentIDFunction] INT NULL,
	[Type] VARCHAR(10) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL	
);

IF OBJECT_ID('[dbo].[MasterGenWeek]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterGenWeek]; 

CREATE TABLE [dbo].[MasterGenWeek]
(
	[IDMstWeek] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[StartDate] DATETIME NULL,
	[EndDate] DATETIME NULL,
	[Week] INT NULL,
	[Month] INT NULL,
	[Year] INT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[CreatedBy] VARCHAR(64) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(64) NOT NULL,
);

IF OBJECT_ID('[dbo].[MasterLocation]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterLocation]; 

CREATE TABLE [dbo].[MasterLocation]
(
	[IDLocation] VARCHAR(50) NOT NULL PRIMARY KEY,
	[LocationName] VARCHAR(100) NOT NULL,
	[ParentLocation] VARCHAR(50) NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[Type] VARCHAR(50) NULL,
	[IsRegionalOffice] BIT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterRole]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterRole]; 

CREATE TABLE [dbo].[MasterRole]
(
	[IDRole] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[RoleName] VARCHAR(100) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterIMDLRole]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterIMDLRole]; 

CREATE TABLE [dbo].[MasterIMDLRole]
(
	[IMDLRole] VARCHAR(128) NOT NULL PRIMARY KEY,
	[IDRole] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterRole](IDRole),
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(50) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(50) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL	
);

IF OBJECT_ID('[dbo].[MasterIMDLRoleLocation]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterIMDLRoleLocation]; 

CREATE TABLE [dbo].[MasterIMDLRoleLocation]
(
	[IDIMDLRoleLocation] INT IDENTITY(1,1) NOT NULL,
	[IMDLRole] VARCHAR(128) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterIMDLRole](IMDLRole),
	[IDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(50) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(50) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[MasterRolesFunctionMapping]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterRolesFunctionMapping]; 

CREATE TABLE [dbo].[MasterRolesFunctionMapping]
(
	[IDRolesFunctionMapping] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[IDRole] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterRole](IDRole),
	[IDFunction] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterFunction](IDFunction),
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NOT NULL
);


IF OBJECT_ID('[dbo].[MasterUser]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterUser]; 

CREATE TABLE [dbo].[MasterUser]
(
	[IDUser] VARCHAR(50) NOT NULL PRIMARY KEY,
	[FullName] VARCHAR(500) NOT NULL,
	[Email] VARCHAR(100) NOT NULL,
	[Address] VARCHAR(500) NOT NULL,
	[Phone] VARCHAR(25) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterUserLocationMapping]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterUserLocationMapping]; 

CREATE TABLE [dbo].[MasterUserLocationMapping]
(
	[IDUserLocationMapping] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[IDUser] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUser](IDUser),
	[IDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[status] VARCHAR(50) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[UserLocationDelegation]', 'U') IS NOT NULL DROP TABLE [dbo].[UserLocationDelegation]; 

IF OBJECT_ID('[dbo].[MasterUserRoleDelegation]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterUserRoleDelegation]; 

CREATE TABLE [dbo].[MasterUserRoleDelegation]
(
	[IDUserDelegation] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[IDUserFrom] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUser](IDUser),
	[IDUserTo] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUser](IDUser),
	[DelegationIDRole] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterRole](IDRole),
	[EffectiveStartDate] DATE NOT NULL,
	[EffectiveEndDate] DATE NOT NULL,
	[IsActive] BIT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

CREATE TABLE [dbo].[UserLocationDelegation]
(
	[IDUserLocationDelegation] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDUserDelegation] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUserRoleDelegation](IDUserDelegation),
	[DelegationIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[MasterUserRoleMapping]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterUserRoleMapping]; 

CREATE TABLE [dbo].[MasterUserRoleMapping]
(
     [IDUserRoleMapping] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[IDRole] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterRole](IDRole),
	[IDUser] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUser](IDUser),
	[RoleXml] VARCHAR(128) NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL	
);

IF OBJECT_ID('[dbo].[MasterMapping]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterMapping]; 

CREATE TABLE [dbo].[MasterMapping]
(
	[IDMasterMapping] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[MapFrom] VARCHAR(200) NOT NULL,
	[MapTo] VARCHAR(200) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterLoadFactorCFP]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterLoadFactorCFP]; 

CREATE TABLE [dbo].[MasterLoadFactorCFP]
(
	[IDLoadFactorCFP] INT IDENTITY(1,1) NOT FOR REPLICATION NOT NULL PRIMARY KEY,
	[VehicleType] VARCHAR(100) NOT NULL,
	[Mode] VARCHAR(50) NULL,
	[KMperLiter] DECIMAL(18, 2) NULL,
	[KgCO2perLiter] FLOAT NULL,
	[BrandCategory] VARCHAR(100) NOT NULL,
	[MaxQty] INT NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL,
	[WeightperStick] FLOAT NULL
);

IF OBJECT_ID('[dbo].[NotificationSystem]', 'U') IS NOT NULL DROP TABLE [dbo].[NotificationSystem]; 

CREATE TABLE [dbo].[NotificationSystem]
(
	[IDNotification] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDUser] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUser](IDUser),
	[PageName] VARCHAR(200) NOT NULL,
	[Description] VARCHAR(500) NOT NULL,
	[IsRead] BIT NOT NULL,
	[IsOpen] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[MasterVendor]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterVendor]; 

CREATE TABLE [dbo].[MasterVendor]
(
	[IDVendor] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[VendorName] VARCHAR(500) NOT NULL,
	[ParentVendor] INT NULL FOREIGN KEY REFERENCES [dbo].[MasterVendor](IDVendor),
	[VendorAddress] VARCHAR(100) NULL,
	[VendorRegion] VARCHAR(50) NULL,
	[VendorProvince] VARCHAR(50) NULL,
	[VendorCity] VARCHAR(50) NULL,
	[VendorEmail] VARCHAR(1000) NULL,
	[CC] VARCHAR(1000) NULL,
	[VendorCategory] VARCHAR(50) NULL,
	[TransportationMode] VARCHAR(50) NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterLeadTime]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterLeadTime]; 

CREATE TABLE [dbo].[MasterLeadTime]
(
	[IDLeadTime] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDVendor] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterVendor](IDVendor),
	[SenderIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[ReceiverIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[ThroughIDLocation] VARCHAR(50) NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[Via] VARCHAR(50) NULL,
	[Time] TINYINT NOT NULL,
	[EffectiveStartDate] DATE NOT NULL,
	[EffectiveEndDate] DATE NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterDistance]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterDistance]; 

CREATE TABLE [dbo].[MasterDistance]
(
	[IDDistance] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[DistanceType] VARCHAR(64) NOT NULL,
	[IDSender] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[IDReceiver] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[TransportationMode] VARCHAR(200) NOT NULL,
	[Distance] DECIMAL(10, 2) NOT NULL,
	[Buffer] INT NOT NULL,
	[Total] DECIMAL(10, 2) NULL,
	[Through] VARCHAR(50) NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[Via] VARCHAR(200) NULL,
	[EffectiveStartDate] DATE NOT NULL,
	[EffectiveEndDate] DATE NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterCost]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterCost]; 

CREATE TABLE [dbo].[MasterCost]
(
	[IDCost] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CostType] VARCHAR(50) NOT NULL,
	[IDVendor] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterVendor](IDVendor),
	[VehicleType] VARCHAR(50) NULL,
	[OrderType] VARCHAR(50) NULL,
	[SenderIDLocation] VARCHAR(50) NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
     [ThroughIDLocation] VARCHAR(50) NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
     [ReceiverIDLocation] VARCHAR(50) NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
     [Via] VARCHAR(50) NULL,
     [MinimumKM] DECIMAL(10, 2) NULL,
     [MinimumBox] INT NULL,
     [BasedPrice] DECIMAL(14, 2) NOT NULL,
     [DiscountPrice] DECIMAL(14, 2) NULL,
     [AdditionalUnitPrice] DECIMAL(14, 2) NULL,
     [EffectiveStartDate] DATE NOT NULL,
     [EffectiveEndDate] DATE NOT NULL,
     [IsActive] BIT NOT NULL,
     [CreatedBy] VARCHAR(128) NOT NULL,
     [CreatedDate] DATETIME NOT NULL,
     [UpdatedBy] VARCHAR(128) NOT NULL,
     [UpdatedDate] DATETIME NOT NULL,
     [Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterVendorSuggestion]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterVendorSuggestion]; 

CREATE TABLE [dbo].[MasterVendorSuggestion]
(
	[IDVendorSuggestion] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[StartLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[ReceiverIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[OrderType] VARCHAR(50) NOT NULL,
	[TransportationCategory] VARCHAR(50) NOT NULL,
	[TransportationMode] VARCHAR(50) NOT NULL,
	[VehicleType] VARCHAR(50) NOT NULL,
	[SuggestedVendor] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterVendor](IDVendor),
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterDataCostCenter]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterDataCostCenter]; 

CREATE TABLE [dbo].[MasterDataCostCenter]
(
	[IDCostCenter] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SenderIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[ReceiverIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[MaterialType] VARCHAR(20) NOT NULL,
	[Description] VARCHAR(500) NOT NULL,
	[CostCenter] VARCHAR(20) NOT NULL,
	[Account] VARCHAR(20) NOT NULL,
	[EffectiveStartDate] DATETIME NOT NULL,
	[EffectiveEndDate] DATETIME NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[MasterUom]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterUom]; 

CREATE TABLE [dbo].[MasterUom]
(
	[IDUoM] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MaterialType] VARCHAR(50) NOT NULL,
	[UoM] VARCHAR(50) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(200) NULL
);

IF OBJECT_ID('[dbo].[MasterServicePo]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterServicePo]; 

CREATE TABLE [dbo].[MasterServicePo]
(
	[IDServicePO] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDVendor] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterVendor](IDVendor),
	[ServicePONumber] VARCHAR(10) NOT NULL,
	[EffectiveStartDate] DATE NOT NULL,
	[EffectiveEndDate] DATE NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[MasterTruckSealStock]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterTruckSealStock]; 

CREATE TABLE [dbo].[MasterTruckSealStock]
(
	[IDTruckSealStock] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SealNumberFrom] VARCHAR(8) NOT NULL,
	[SealNumberTo] VARCHAR(8) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[TransportExecutionStatusLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportExecutionStatusLog];
IF OBJECT_ID('[dbo].[TransportSeal]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportSeal]; 
IF OBJECT_ID('[dbo].[TransportExecution]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportExecution]; 

CREATE TABLE [dbo].[TransportExecution]
(
	[IDTransportExecution] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[TransportNo] VARCHAR(13) NOT NULL,
	[TransportStatus] VARCHAR(20) NOT NULL,
	[TransportDate] DATE NOT NULL,
	[TransportCategory] VARCHAR(15) NOT NULL,
	[ActualVehicleType] VARCHAR(13) NOT NULL,
	[TransportMode] VARCHAR(4) NOT NULL,
	[VesselName] VARCHAR(50) NULL,
	[ETD1] DATE NULL,
	[ETD2] DATE NULL,
	[ATD] DATE NULL,
	[ETA1] DATE NULL,
	[ETA2] DATE NULL,
	[ATA] DATE NULL,
	[ActualTimeBerthing] DATETIME NULL,
	[ContainerNo] VARCHAR(15) NULL,
	[ContainerSeal] VARCHAR(15) NULL,
	[IDVendor] INT NULL FOREIGN KEY REFERENCES [dbo].[MasterVendor](IDVendor),
	[SIType] VARCHAR(6) NULL,
	[SIStatus] VARCHAR(13) NULL,
	[SIWeek] TINYINT NULL,
	[TargetOfArrival] DATETIME NULL,
	[ActualArrive] DATETIME NULL,
	[StartLocation] VARCHAR(50) NULL,
	[FinishLocation] VARCHAR(50) NULL,
	[ServicePONo] VARCHAR(10) NULL,
	[SerivceGRNo] VARCHAR(10) NULL,
	[ActualCostCenter] VARCHAR(10) NULL,
	[PoliceRegNo] VARCHAR(7) NULL,
	[IDCardNumberDriver1] VARCHAR(25) NULL,
	[IDCardNumberDriver2] VARCHAR(25) NULL,
	[IDCardNumberCoDriver] VARCHAR(25) NULL,
	[IDDistanceKMBased] INT NULL,
	[IDDistanceBoxTripBased] INT NULL,
	[IDCost] INT NULL,
	[TotalKM] DECIMAL(18, 2) NULL,
	[KMRail] DECIMAL(10, 2) NULL,
	[KMBased] DECIMAL(10, 2) NULL,
	[KMSea] DECIMAL(10, 2) NULL,
	[TotalBox] INT NULL,
	[ASDPCost] DECIMAL(14, 2) NULL,
	[SPSICost] DECIMAL(14, 2) NULL,
	[AdditionalCost] DECIMAL(14, 2) NULL,
	[Via] VARCHAR(200) NULL,
	[Month] TINYINT NULL,
	[Year] INT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

CREATE TABLE [dbo].[TransportExecutionStatusLog]
(
	[IDTransportExecutionStatusLog] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDTransportExcecution] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TransportExecution](IDTransportExecution),
	[Status] VARCHAR(20) NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

CREATE TABLE [dbo].[TransportSeal]
(
	[IDTransportSeal] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDTransportExecution] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TransportExecution](IDTransportExecution),
	[SealNumber] VARCHAR(8) NOT NULL,
	[SealActivity] VARCHAR(6) NOT NULL,
	[IDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[SealTime] DATETIME NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);

IF OBJECT_ID('[dbo].[TransportPickingListLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportPickingListLog]; 

CREATE TABLE [dbo].[TransportPickingListLog]
(
	[IDTransportPickingListLog] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ShipmentDate] DATE NOT NULL,
	[IsActive] BIT NULL,
	[Remarks] VARCHAR(500) NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[TransportOrderDetailTemp]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderDetailTemp];
IF OBJECT_ID('[dbo].[TransportOrderTemp]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderTemp];

CREATE TABLE [dbo].[TransportOrderTemp]
(
	[STONo] VARCHAR(10) NOT NULL PRIMARY KEY,
	[ShipmentDate] DATE NOT NULL,
	[SenderIDLocation] VARCHAR(50) NOT NULL,
	[ReceiverIDLocation] VARCHAR(50) NOT NULL
);

CREATE TABLE [dbo].[TransportOrderDetailTemp]
(
	[IDTransportOrderDetailTemp] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[STONo] VARCHAR(10) NOT NULL FOREIGN KEY REFERENCES [dbo].[TransportOrderTemp](STONo),
	[Code] VARCHAR(50) NULL,
	[MaterialType] VARCHAR(25) NULL,
	[Description] VARCHAR(10) NULL,
	[Qty] DECIMAL(18, 4) NOT NULL,
	[UoM] VARCHAR(10) NULL
);

IF OBJECT_ID('[dbo].[TransportOrderChangeLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderChangeLog];
IF OBJECT_ID('[dbo].[TransportOrderDetailChangeLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderDetailChangeLog];
IF OBJECT_ID('[dbo].[TransportOrderDetail]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderDetail]; 
IF OBJECT_ID('[dbo].[TransportOrder]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrder]; 

CREATE TABLE [dbo].[TransportOrder]
( 
	[IDTransportOrder] INT NOT NULL IDENTITY PRIMARY KEY,
	[IDRequest] INT NULL,
	[IDTransportExecution] INT NULL,
	[IDTransportPickingListLog] INT NULL,
	[STONo] VARCHAR(10) NULL,
	[DefaultSeqNo] VARCHAR(5) NULL,
	[SeqNo] VARCHAR(5) NULL,
	[ShipmentDate] DATE NOT NULL,
	[KM] DECIMAL(10, 4) NULL,
	[SenderIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[ActualSenderIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[ReceiverIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[ActualReceiverIDLocation] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterLocation](IDLocation),
	[LeadTime] TINYINT NULL,
	[VehicleType] VARCHAR(50) NOT NULL,
	[OrderType] VARCHAR(25) NOT NULL,
	[CostCenter] VARCHAR(20) NULL,
	[OrderStatus] VARCHAR(20) NULL,
	[MaterialReceivedTime] DATETIME NULL,
	[IsMaterialReceived] BIT NULL,
	[EstArrivalDate] DATE NULL,
	[GRDate] DATETIME NULL,
	[LoadStartTime] DATETIME NULL,
	[LoadFinishTime] DATETIME NULL,
	[UnloadStartTime] DATETIME NULL,
	[UnloadFinishTime] DATETIME NULL,
	[Remarks] VARCHAR(500) NULL,
	[FlagEmail] TINYINT NULL,
	[IDLeadTime] INT NULL,
	[IDCostCenter] INT NULL,
	[ChangeLogFields] VARCHAR(MAX),
	[ZoneBased] VARCHAR(50) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
);

CREATE TABLE [dbo].[TransportOrderDetail]
( 
	[IDTransportOrderDetail] INT NOT NULL IDENTITY PRIMARY KEY,
	[IDTransportOrder] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TransportOrder](IDTransportOrder),
	[Supplier] VARCHAR(100) NULL,
	[MaterialType] VARCHAR(25) NULL,
	[Code] VARCHAR(50) NULL,
	[Description] VARCHAR(50) NULL,
	[Qty] DECIMAL(18, 4) NULL,
	[UoM] VARCHAR(10) NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
);

CREATE TABLE [dbo].[TransportOrderChangeLog]
( 
	[IDTransportOrderChangeLog] INT NOT NULL IDENTITY PRIMARY KEY,
	[IDTransportOrder] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TransportOrder](IDTransportOrder),
	[Version] INT NULL,
	[OldValue] VARCHAR(200) NULL,
	[ModifiedField] VARCHAR(50) NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
	
);

CREATE TABLE [dbo].[TransportOrderDetailChangeLog]
( 
	[IDTransportOrderDetailChangeLog] INT NOT NULL IDENTITY PRIMARY KEY,
	[IDTransportOrderDetail] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TransportOrderDetail](IDTransportOrderDetail),
	[Version] INT NULL,
	[OldValue] VARCHAR(200) NULL,
	[ModifiedField] VARCHAR(50) NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[TransportOrderRequest]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderRequest]; 

CREATE TABLE [dbo].[TransportOrderRequest]
(
	[IDRequest] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[RequestNo] VARCHAR(10) NULL,
	[VehicleType] VARCHAR(5) NULL,
	[ShipmentDate] DATE NOT NULL,
	[Zone] VARCHAR(10) NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NULL,
	[UpdatedDate] DATETIME NOT NULL
);

IF OBJECT_ID('[dbo].[TransportDriverManagement]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportDriverManagement]; 

CREATE TABLE [dbo].[TransportDriverManagement]
(
	[IDCardNumber] VARCHAR(20) NOT NULL PRIMARY KEY,
	[Name] VARCHAR(100) NOT NULL,
	[DateOfBirth] DATE NULL,
	[Gender] VARCHAR(10) NULL,
	[Address] VARCHAR(500) NULL,
	[MobilePhone] VARCHAR(100) NULL,
	[IDVendor] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterVendor](IDVendor),
	[JoinDate] DATE NULL,
	[DrivingLicenseNumber] VARCHAR(32) NULL,
	[DrivingLicensePeriod] DATE NULL,
	[BaseTown] VARCHAR(50) NULL,
	[PerformanceLevel] VARCHAR(50) NULL,
	[AttachmentKTP] VARCHAR(500) NULL,
	[AttachmentSIM] VARCHAR(500) NULL,
	[AttachmentFoto] VARCHAR(500) NULL,
	[BPJSKetenagaKerjaan] BIT NOT NULL,
	[BPJSKesehatan] BIT NOT NULL,
	[DrugFreeTest] BIT NOT NULL,
	[FatiqueTest] BIT NOT NULL,
	[InductionTest] BIT NOT NULL,
	[DefensiveDrivingTest] BIT NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL,
	[RoleDriver] VARCHAR(100) NULL,
	[DriverContractValidityPeriod] DATETIME NULL
);

IF OBJECT_ID('[dbo].[TransportTicketNCR]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportTicketNCR]; 

CREATE TABLE [dbo].[TransportTicketNCR]
(
	[TicketNumber] VARCHAR(20) NOT NULL PRIMARY KEY,
	[TicketCategory] VARCHAR(50) NOT NULL,
	[TicketQuantity] INT NULL,
	[Location] VARCHAR(50) NOT NULL,
	[IDVendor] VARCHAR(50) NOT NULL,
	[PoliceRegNumber] VARCHAR(10) NULL,
	[OffenderName] VARCHAR(50) NULL,
	[OffenderRole] VARCHAR(50) NULL,
	[OffenderAge] INT NULL,
	[RemarksTicket] VARCHAR(MAX) NULL,
	[Attachment] VARCHAR(MAX) NULL,
	[VehicleType] VARCHAR(50) NULL,
	[ManufacturingYear] INT NULL,
	[OffenderYearOfService] VARCHAR(100) NULL,
	[LocationCategory] VARCHAR(50) NULL,
	[TypeOfLocation] VARCHAR(50) NULL,
	[RoadCondition] VARCHAR(50) NULL,
	[Preventablility] BIT NULL,
	[WeatherCondition] VARCHAR(50) NULL,
	[AccidentCategory] VARCHAR(50) NULL,
	[ValueOfLoss] MONEY NULL,
	[CorrectiveAction] VARCHAR(50) NULL,
	[CorrectiveActionDate] DATETIME NULL,
	[TicketStatus] VARCHAR(50) NULL,
	[SIRSNumber] VARCHAR(50) NULL,
	[NCRNumber] VARCHAR(50) NULL,
	[AttachmentNRC] VARCHAR(MAX) NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL,
	[ProblemIssue1] VARCHAR(100) NULL,
	[ProblemIssue2] VARCHAR(100) NULL,
	[ProblemIssue3] VARCHAR(100) NULL,
	[MainFactor] VARCHAR(100) NULL
);


IF OBJECT_ID('[dbo].[TransportTruckArrival]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportTruckArrival]; 

CREATE TABLE [dbo].[TransportTruckArrival]
(
	[IDTransportTruckArrival] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PoliceNumber] VARCHAR(20) NOT NULL,
	[GPNo] VARCHAR(20) NOT NULL,
	[Date] DATETIME NOT NULL,
	[LongitudeDestination] VARCHAR(50) NULL,
	[LatitudeDestination] VARCHAR(50) NULL,
	[LongitudeTruck] VARCHAR(50) NULL,
	[LatitudeTruck] VARCHAR(50) NULL,
	[ETA] DATETIME NULL,
	[ETACategory] VARCHAR(20) NULL,
	[ETACalculate] DATETIME NULL,
	[ETACategoryCalculate] VARCHAR(20) NULL,
	[UpdatedBy] VARCHAR(128) NULL,
	[UpdatedDate] DATETIME NULL
);

IF OBJECT_ID('[dbo].[TruckArrivalHistory]', 'U') IS NOT NULL DROP TABLE [dbo].[TruckArrivalHistory]; 

CREATE TABLE [dbo].[TruckArrivalHistory]
(
	[IDTruckArrivalHistory] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PoliceNumber] VARCHAR(20) NOT NULL,
	[GPNo] VARCHAR(20) NOT NULL,
	[Date] DATETIME NOT NULL,
	[Longitude] VARCHAR(50) NOT NULL,
	[Latitude] VARCHAR(50) NOT NULL,
	[IDLocation] VARCHAR(MAX) NOT NULL,
	[Location] VARCHAR(MAX) NOT NULL
);

IF OBJECT_ID('[dbo].[TruckArrivalTemp]', 'U') IS NOT NULL DROP TABLE [dbo].[TruckArrivalTemp]; 

CREATE TABLE [dbo].[TruckArrivalTemp]
(
	[IDTruckArrivalTemp] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PoliceNumber] VARCHAR(20) NOT NULL,
	[Date] DATETIME NOT NULL,
	[Longitude] VARCHAR(50) NOT NULL,
	[Latitude] VARCHAR(50) NOT NULL,
	[Location] VARCHAR(5000) NULL,
	[Speed] INT NOT NULL,
	[Satellite] INT NOT NULL,
	[SOS] VARCHAR(10) NOT NULL,
	[DoorRear] VARCHAR(10) NOT NULL,
	[DoorLeft] VARCHAR(10) NOT NULL,
	[DoorRight] VARCHAR(10) NOT NULL,
	[Kabupaten] VARCHAR(5000) NULL,
	[Kecamatan] VARCHAR(5000) NULL,
	[INTPower] INT NULL,
	[EXTPower] INT NULL,
	[Status] VARCHAR(5000) NULL,
	[Engine] VARCHAR(10) NULL,
	[Fatiqeu] VARCHAR(10) NULL,
	[CreatedDate] DATETIME NULL
);

IF OBJECT_ID('[dbo].[TransportVehicleData]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportVehicleData]; 

CREATE TABLE [dbo].[TransportVehicleData]
(
	[IDPoliceRegNumber] VARCHAR(10) NOT NULL PRIMARY KEY,
	[ManufacturingYear] INT NULL,
	[Karoseri] VARCHAR(50) NULL,
	[Merk] VARCHAR(50) NULL,
	[Type] VARCHAR(50) NOT NULL,
	[Model] VARCHAR(50) NULL,
	[Cargo] VARCHAR(50) NULL,
	[VehicleIdentityNumber] VARCHAR(50) NULL,
	[EngineNumber] VARCHAR(50) NULL,
	[BaseTown] VARCHAR(50) NOT NULL,
	[STNKValidityPeriod] DATE NOT NULL,
	[GPS] VARCHAR(50) NULL,
	[Status] VARCHAR(50) NULL,
	[AttachmentSTNK] VARCHAR(500) NULL,
	[AttachmentPhoto] VARCHAR(500) NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL,
	[AttachmentPhotoTwo] VARCHAR(500) NULL,
	[AttachmentPhotoThree] VARCHAR(500) NULL
);

--VIEW MasterCostLatestEffectiveDateDataView
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].MasterCostLatestEffectiveDateDataView'))
DROP VIEW [dbo].MasterCostLatestEffectiveDateDataView
GO
CREATE VIEW MasterCostLatestEffectiveDateDataView AS
WITH cte AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY CostType,IDVendor,VehicleType,OrderType,SenderIDLocation,ThroughIDLocation,ReceiverIDLocation,Via ORDER BY EffectiveStartDate DESC) AS rn
   FROM MasterCost
)
SELECT *
FROM cte
WHERE rn = 1
GO

--FUNCTION CheckSNTruckSeal
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CheckSNTruckSeal]'))
DROP FUNCTION [dbo].[CheckSNTruckSeal]
GO 
CREATE FUNCTION [dbo].[CheckSNTruckSeal] 
(	
	@snfrom int, 
	@snto int
)
RETURNS TABLE
AS
RETURN 
(
	SELECT
		[MasterTruckSealStock].[SealNumberFrom],
		[MasterTruckSealStock].[SealNumberTo]
	FROM
		[MasterTruckSealStock]
	WHERE
		(CONVERT(int,[MasterTruckSealStock].[SealNumberFrom]) BETWEEN @snfrom AND @snto)
		OR
		(CONVERT(int,[MasterTruckSealStock].[SealNumberTo]) BETWEEN @snfrom AND @snto)
		OR
		(CONVERT(int,[MasterTruckSealStock].[SealNumberFrom]) <= @snfrom AND CONVERT(int,[MasterTruckSealStock].[SealNumberTo]) >= @snto)
		OR
		(CONVERT(int,[MasterTruckSealStock].[SealNumberFrom]) >= @snfrom AND CONVERT(int,[MasterTruckSealStock].[SealNumberTo]) <= @snto)
)
GO
