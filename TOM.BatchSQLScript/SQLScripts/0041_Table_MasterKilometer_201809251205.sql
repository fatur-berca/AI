IF OBJECT_ID('[dbo].[MasterKilometer]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterKilometer]; 

CREATE TABLE [dbo].[MasterKilometer]
(
	[IDMasterKilometer] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDLocationFrom] VARCHAR(50) NOT NULL,
	[LocationFrom] VARCHAR(500) NOT NULL,
	[LongitudeFrom] VARCHAR(50) NULL,
	[LatitudeFrom] VARCHAR(50) NULL,
	[LocationTo] VARCHAR(500) NOT NULL,
	[IDLocationTo] VARCHAR(50) NOT NULL,
	[LongitudeTo] VARCHAR(50) NULL,
	[LatitudeTo] VARCHAR(50) NULL,
	[Kilometer] INT NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);