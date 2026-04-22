/* Created By : Fadiyah
Date : 2018-10-03 */

IF OBJECT_ID('[dbo].[TransportVendorChangeLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransportVendorChangeLog];

CREATE TABLE [dbo].[TransportVendorChangeLog](
[IDVendorChangeLog] [int] IDENTITY(1,1) NOT NULL,
[IDTransportExecution] [int] NOT NULL,
[TransportDate] [date] NOT NULL,
[IDVendor] [int] NOT NULL,
[SIType] [varchar](6) NULL,
[SIStatus] [varchar](13) NULL,
[TargetOfArrival] [datetime] NULL,
[ActualArrive] [datetime] NULL,
[IsActive] [bit] NULL,
[CreatedBy] [varchar](128) NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[UpdatedBy] [varchar](128) NOT NULL,
[UpdatedDate] [datetime] NOT NULL,
CONSTRAINT [PK_ExecutionVendorChangeLog] PRIMARY KEY CLUSTERED 
(
[IDVendorChangeLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[TransportVendorChangeLog] WITH CHECK ADD CONSTRAINT [FK_ExecutionVendorChangeLog_TransportExecution] FOREIGN KEY([IDTransportExecution])
REFERENCES [dbo].[TransportExecution] ([IDTransportExecution])

ALTER TABLE [dbo].[TransportVendorChangeLog] CHECK CONSTRAINT [FK_ExecutionVendorChangeLog_TransportExecution]
