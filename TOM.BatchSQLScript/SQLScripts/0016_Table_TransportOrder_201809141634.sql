-- author : fendi
-- date : 2018-09-14
-- desc : create ulang tabel TransportOrder, TransportOrderDetail, TransportOrderChangeLog, TransportOrderDetailChangeLog

IF OBJECT_ID('[dbo].[TransportOrderChangeLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderChangeLog];
IF OBJECT_ID('[dbo].[TransportOrderDetailChangeLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderDetailChangeLog];
IF OBJECT_ID('[dbo].[TransportOrderDetail]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderDetail]; 
IF OBJECT_ID('[dbo].[TransportOrder]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrder]; 

CREATE TABLE [dbo].[TransportOrder]
( 
	[IDTransportOrder] INT NOT NULL IDENTITY PRIMARY KEY,
	[IDRequest] INT NULL FOREIGN KEY REFERENCES [dbo].[TransportOrderRequest](IDRequest),
	[IDTransportExecution] INT NULL FOREIGN KEY REFERENCES [dbo].[TransportExecution](IDTransportExecution),
	[IDTransportPickingListLog] INT NULL FOREIGN KEY REFERENCES [dbo].[TransportPickingListLog](IDTransportPickingListLog),
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
	[VehicleType] VARCHAR(50) NULL,
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