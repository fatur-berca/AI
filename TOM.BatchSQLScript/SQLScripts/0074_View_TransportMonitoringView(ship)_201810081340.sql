--USE [TOM]
--GO

/****** Object:  View [dbo].[TransportMonitoringView]    Script Date: 08/10/2018 11:51:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- author : hakim
-- date : 2018-10-08
-- dasc : page transport monitoring (ship)
CREATE VIEW [dbo].[TransportMonitoringView] AS 

SELECT tvm.IDTransportVesselMonitoring,te.IDTransportExecution,tor.IDTransportOrder, te.TransportNo, te.TransportDate, te.StartLocation, te.IDVendor,mv.VendorName, tvm.ETD1, tvm.ETD2, tvm.ETA1, tvm.ETA2, tvm.ATD, tvm.ATA,tvm.EstReceived,tvm.ActualTimeBerthing, tvm.Remarks, te.TransportStatus,
te.ActualVehicleType,tvm.VesselName,tvm.ContainerNo,tvm.ContainerSeal,  te.IsActive , '2000-01-01' AS GIDate, 0 as CargoReceived --'2000-01-01' as GRDate , 0 as CargoReceived
, tor.STONo, tor.SenderIDLocation, tor.ReceiverIDLocation, tor.GRDate,tor.OrderStatus,tor.VehicleType
FROM TransportExecution te
INNER JOIN TransportOrder tor ON te.IDTransportExecution = tor.IDTransportExecution
INNER JOIN TransportVesselMonitoring tvm ON te.IDTransportExecution = tvm.IDTransportExecution
INNER JOIN MasterVendor mv ON te.IDVendor = mv.IDVendor
--WHERE te.IsActive = 1 AND tor.IsActive = 1 AND tvm.IsActive = 1 AND mv.IsActive = 1
GROUP BY tvm.IDTransportVesselMonitoring,te.IDTransportExecution,tor.IDTransportOrder, te.TransportNo, te.TransportDate, te.StartLocation, te.IDVendor,mv.VendorName, tvm.ETD1, tvm.ETD2, tvm.ETA1, tvm.ETA2, tvm.ATD, tvm.ATA,tvm.EstReceived,tvm.ActualTimeBerthing, tvm.Remarks, te.TransportStatus,
te.ActualVehicleType,tvm.VesselName,tvm.ContainerNo,tvm.ContainerSeal,  te.IsActive --, '2000-01-01' AS GIDate, '2000-01-01' as GRDate , 0 as CargoReceived
, tor.STONo, tor.SenderIDLocation, tor.ReceiverIDLocation, tor.GRDate,tor.OrderStatus,tor.VehicleType


GO