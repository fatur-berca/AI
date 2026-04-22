USE [TOM]
GO

CREATE TABLE [dbo].[TransportOrderRequestHeader] (
	[RequestNo] varchar(10) NOT NULL,
	PRIMARY KEY CLUSTERED ([RequestNo] ASC)
	WITH 
	(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
	ON [PRIMARY]
)
GO

INSERT INTO [dbo].[TransportOrderRequestHeader] ([RequestNo])
	(SELECT DISTINCT([RequestNo]) FROM [dbo].[TransportOrderRequest] WHERE [RequestNo] IS NOT NULL)
GO

ALTER TABLE [dbo].[TransportOrderRequest] 
ADD CONSTRAINT FK_RequestNo_TOR_TORH FOREIGN KEY ([RequestNo])     
    REFERENCES [dbo].[TransportOrderRequestHeader] ([RequestNo])     
    ON DELETE CASCADE    
    ON UPDATE CASCADE    
;
GO