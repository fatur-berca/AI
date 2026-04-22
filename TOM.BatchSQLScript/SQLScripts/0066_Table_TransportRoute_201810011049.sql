
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TransportRoute](
	[IDTransportRoute] [int] IDENTITY(1,1) NOT NULL,
	[IDTransportExecution] [int] NOT NULL,
	[IDLocation] [varchar](50) NOT NULL,
	[IsMain] [bit] NOT NULL,
	[IsAssigned] [bit] NOT NULL,
	[CreatedBy] [varchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](128) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TransportationRoute] PRIMARY KEY CLUSTERED 
(
	[IDTransportRoute] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TransportRoute]  WITH CHECK ADD  CONSTRAINT [FK_TransportationRoute_TransportationExecution] FOREIGN KEY([IDTransportExecution])
REFERENCES [dbo].[TransportExecution] ([IDTransportExecution])
GO

ALTER TABLE [dbo].[TransportRoute] CHECK CONSTRAINT [FK_TransportationRoute_TransportationExecution]
GO
