/****** Object:  View [dbo].[MasterUserLocationMapTreeListView]    Script Date: 9/25/2018 10:55:23 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[MasterUserLocationMapTreeListView] AS
SELECT locpar.[IDLocation] AS IDZone
      ,locpar.[LocationName] AS NameZone
      ,locpar2.[IDLocation] AS IDRegion
      ,locpar2.[LocationName] AS NameRegion
      ,locpar3.[IDLocation] AS IDWarehouse
      ,locpar3.[LocationName] AS NameWarehouse
  FROM MasterLocation AS locpar
  LEFT JOIN MasterLocation AS locpar2
  ON locpar.IDLocation = locpar2.ParentLocation
  LEFT JOIN MasterLocation AS locpar3
  ON locpar2.IDLocation = locpar3.ParentLocation
WHERE locpar.[Type] = 'Zone' AND locpar.IsActive = '1' AND locpar2.IsActive = '1' AND locpar3.IsActive = '1';
GO


