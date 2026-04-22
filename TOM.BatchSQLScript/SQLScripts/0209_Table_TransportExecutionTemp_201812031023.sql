IF OBJECT_ID('[dbo].[TransportExecutionTemp]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportExecutionTemp]; 

CREATE TABLE [dbo].[TransportExecutionTemp]
(	
	[IDUser] VARCHAR(50) NOT NULL,
	[IDCheck] INT NOT NULL,
	[IDTransportOrder] INT NOT NULL,
	[IDTransportOrderDetail] INT NULL,
	[ConcatIDTransportOrder] VARCHAR(MAX) NULL,
	[OrderNumber] VARCHAR(50) NULL,
	[OrderType] VARCHAR(50) NULL,
	[IDSenderLoc] VARCHAR(50) NULL,
	[Sender] VARCHAR(100) NULL,
	[IDReceiverLoc] VARCHAR(50) NULL,
	[Receiver] VARCHAR(100) NULL,
	[OrderedVehicleType] VARCHAR(50) NULL,
	[CostCenter] VARCHAR(50) NULL,
	[ShipmentDate] DATE NULL,
	[MaterialType] VARCHAR(50) NULL,
	[Description] VARCHAR(100) NULL,
	[Qty] DECIMAL(18,4) NULL,
	[UoM] VARCHAR(50) NULL,
	[OrderRemarks] VARCHAR(500) NULL,
	[Sequence] VARCHAR(5) NULL,
	[OrderCreator] VARCHAR(500) NULL,
	[TransportationNumber] VARCHAR(50) NULL,
	[IDVendor] INT NULL,
	[VendorName] VARCHAR(50) NULL,
	[IDVendorSuggestion] INT NULL,
	[VendorSuggestion] VARCHAR(50) NULL,
	[UpdatedBy] VARCHAR(50) NULL,
	[UpdatedDate] DATETIME NULL,
	[TNCreator] VARCHAR(500) NULL
);
