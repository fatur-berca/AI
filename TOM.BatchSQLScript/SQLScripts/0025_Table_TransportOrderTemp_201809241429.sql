IF OBJECT_ID('[dbo].[TransportOrderDetailTemp]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderDetailTemp];
IF OBJECT_ID('[dbo].[TransportOrderTemp]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportOrderTemp];

CREATE TABLE [dbo].[TransportOrderTemp]
(
	[STONo] VARCHAR(10) NOT NULL,
	[ShipmentDate] DATE NOT NULL,
	[SenderIDLocation] VARCHAR(500) NOT NULL,
	[ReceiverIDLocation] VARCHAR(500) NOT NULL,
	PRIMARY KEY ([STONo], [ShipmentDate], [SenderIDLocation], [ReceiverIDLocation])
);

CREATE TABLE [dbo].[TransportOrderDetailTemp]
(
	[IDTransportOrderDetailTemp] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[STONo] VARCHAR(10) NULL,
	[Code] VARCHAR(500) NULL,
	[MaterialType] VARCHAR(25) NULL,
	[Description] VARCHAR(100) NULL,
	[Qty] DECIMAL(18, 4) NOT NULL,
	[UoM] VARCHAR(10) NULL
);