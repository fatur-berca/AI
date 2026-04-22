IF OBJECT_ID('[dbo].[MasterTransportRegion]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterTransportRegion]; 

CREATE TABLE [dbo].[MasterTransportRegion]
(
	[IDTransportRegion] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[GPNumber] VARCHAR(100) NOT NULL,
	[TransportRegion] VARCHAR(200) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);