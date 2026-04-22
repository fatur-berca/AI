USE [TOM]
GO

INSERT INTO [dbo].[MasterConfiguration]
           ([PageName]
           ,[Description]
           ,[Value]
           ,[IsActive]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Remarks])
     VALUES
           ('TOMLocationFilter', 'Location filter for TOM modules','Warehouse',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP,null),
		   ('TOMLocationFilter', 'Location filter for TOM modules','Transport',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP,null),
		   ('TOMLocationFilter', 'Location filter for TOM modules','Factory',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP,null),
		   ('TOMLocationFilter', 'Location filter for TOM modules','TPO',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP,null),
		   ('TOMLocationFilter', 'Location filter for TOM modules','Agent',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP,null),
		   ('TOMLocationFilter', 'Location filter for TOM modules','Other',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP,null);
GO


