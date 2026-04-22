
CREATE TABLE [dbo].[TransportPositionDetail](
	[IDTransportPositionDetail] [int] IDENTITY(1,1) NOT NULL,
	[IDTransportExecution] [int] NULL,
	[PositionDate] [datetime] NULL,
	[PositionName] [varchar](500) NULL,
	[CreatedBy] [varchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](128) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TransportationPositionDetail] PRIMARY KEY CLUSTERED 
(
	[IDTransportPositionDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TransportPositionDetail]  WITH CHECK ADD  CONSTRAINT [FK_TransportationPositionDetail_TransportationExecution] FOREIGN KEY([IDTransportExecution])
REFERENCES [dbo].[TransportExecution] ([IDTransportExecution])
GO

ALTER TABLE [dbo].[TransportPositionDetail] CHECK CONSTRAINT [FK_TransportationPositionDetail_TransportationExecution]
GO
