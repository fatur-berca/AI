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
           ('TransportOrderMailRecipientLocation','East','SKJ',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP, ''),
		   ('TransportOrderMailRecipientLocation','East','SUR',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP, ''),
		   ('TransportOrderMailRecipientLocation','West','KAR',1,'system',CURRENT_TIMESTAMP,'system',CURRENT_TIMESTAMP, '');
GO

ALTER TABLE [dbo].[TransportOrderDetail] ALTER COLUMN [MaterialType] varchar(50);
GO


