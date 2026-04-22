
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TransportVesselMonitoring](
	[IDTransportVesselMonitoring] [int] IDENTITY(1,1) NOT NULL,
	[IDTransportExecution] [int] NOT NULL,
	[VesselName] [varchar](50) NOT NULL,
	[ETD1] [date] NULL,
	[ETD2] [date] NULL,
	[ATD] [date] NULL,
	[ETA1] [date] NULL,
	[ETA2] [date] NULL,
	[ATA] [date] NULL,
	[ActualTimeBerthing] [date] NULL,
	[EstReceived] [date] NULL,
	[ContainerNo] [varchar](15) NOT NULL,
	[ContainerSeal] [varchar](15) NOT NULL,
	[Remarks] [varchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [varchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](128) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_TransportVesselMonitoring_1] PRIMARY KEY CLUSTERED 
(
	[IDTransportVesselMonitoring] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TransportVesselMonitoring]  WITH CHECK ADD  CONSTRAINT [FK_TransportVesselMonitoring_TransportExecution] FOREIGN KEY([IDTransportExecution])
REFERENCES [dbo].[TransportExecution] ([IDTransportExecution])
GO

ALTER TABLE [dbo].[TransportVesselMonitoring] CHECK CONSTRAINT [FK_TransportVesselMonitoring_TransportExecution]
GO
