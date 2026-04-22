
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW TransportMonitoringTruckTrainList AS
	SELECT
		TE.IDTransportExecution,
		TOR.IDTransportOrder,
		TE.TransportNo,
		TE.TransportStatus,
		TE.TransportDate,
		TE.TransportMode,
		TOR.STONo,
		TE.IDVendor,
		TOR.SenderIDLocation,
		TOR.ReceiverIDLocation,
        TE.StartLocation AS IDStartLocation,
        TE.FinishLocation AS IDFinishLocation,
        TOR.VehicleType,
		TE.PoliceRegNo,
		TE.IsActive,
		TDTE1.Name AS Driver1,
		TDTE2.Name AS Driver2,
		TDTE3.Name AS CoDriver,
		MV.VendorName,
		MLTOS.LocationName AS SenderName,
		MLTOR.LocationName AS ReceiverName,
		MLTES.LocationName AS StartName,
		MLTEF.LocationName AS FinishName
	FROM
		TransportExecution AS TE
		LEFT JOIN TransportOrder AS TOR on TE.IDTransportExecution = TOR.IDTransportExecution
		RIGHT JOIN MasterVendor AS MV on TE.IDVendor = MV.IDVendor
		RIGHT JOIN MasterLocation AS MLTOS on TOR.SenderIDLocation = MLTOS.IDLocation
		RIGHT JOIN MasterLocation AS MLTOR on TOR.ReceiverIDLocation = MLTOR.IDLocation
		RIGHT JOIN MasterLocation AS MLTES on TE.StartLocation = MLTES.IDLocation
		RIGHT JOIN MasterLocation AS MLTEF on TE.StartLocation = MLTEF.IDLocation
		RIGHT JOIN TransportDriverManagement AS TDTE1 on TE.IDCardNumberDriver1 = TDTE1.IDCardNumber
		RIGHT JOIN TransportDriverManagement AS TDTE2 on TE.IDCardNumberDriver2 = TDTE2.IDCardNumber
		RIGHT JOIN TransportDriverManagement AS TDTE3 on TE.IDCardNumberCoDriver = TDTE3.IDCardNumber
	WHERE
		TE.IsActive = 1 AND
		TOR.IsActive = 1
