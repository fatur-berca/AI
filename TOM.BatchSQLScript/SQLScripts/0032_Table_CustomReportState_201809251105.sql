IF OBJECT_ID('[dbo].[CustomReportState]', 'U') IS NOT NULL DROP TABLE [dbo].[CustomReportState];

CREATE TABLE [dbo].[CustomReportState]
(
	[IDCustomReportLayout] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IDUser] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUser](IDUser),
	[PageName] VARCHAR(200) NOT NULL,
	[FieldName] VARCHAR(MAX) NULL,
	[LayoutName] VARCHAR(200) NOT NULL,
	[IsGlobal] BIT NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedBy] VARCHAR(128) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[UpdatedBy] VARCHAR(128) NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[Remarks] VARCHAR(500) NULL
);