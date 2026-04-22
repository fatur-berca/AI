IF OBJECT_ID('[dbo].[MasterCostCenter]', 'U') IS NOT NULL DROP TABLE [dbo].[MasterCostCenter];

CREATE TABLE [dbo].[MasterCostCenter]
(
	[IDCostCenter] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Sender] VARCHAR(500) NOT NULL,
	[Receiver] VARCHAR(500) NOT NULL,
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