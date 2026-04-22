/****** Object:  View [dbo].[MasterLocation]    Script Date: 25/09/2018 11:28:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[MasterLocation] AS
SELECT A.*, IIF(B.IDLocation IS NULL, CAST(0 AS BIT), CAST(1 AS BIT)) AS IsAssigned
  FROM DFIS.dbo.MasterLocation AS A LEFT JOIN dbo.MasterMappingLocation AS B ON A.IDLocation = B.IDLocation
GO


