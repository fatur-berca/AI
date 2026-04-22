/****** Object:  StoredProcedure [dbo].[SP_GetTransportOrderRequest]    Script Date: 12/10/2018 11:34:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zecchan Silverlake
-- Create date: 2018-11-05
-- Description:	To select data from TO
-- =============================================

ALTER PROCEDURE [dbo].[SP_GetTransportOrderRequest] 
	-- Add the parameters for the stored procedure here
	@ONFilter varchar(500) = '',
	@ZoneFilter varchar(500) = '',
	@OTFilter varchar(500) = '',
	@OVFilter varchar(500) = '',
	@ShipDateBeginFilter Date = null,
	@ShipDateEndFilter Date = null,
	@MTFilter varchar(500) = '',
	@StatusFilter varchar(50) = '',
	@SenderFilter varchar(500) = '',
	@ReceiverFilter varchar(500) = '',
	@CreatorFilter varchar(50) = '',
	@LocationFilter varchar(50) = '',
	@AllAccess tinyint = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		RequestNo,
		MAX(IsActive + 0) AS IsActive,
		MIN(CreatedBy) AS CreatedBy,
		MIN(CreatedDate) AS CreatedDate,
		MAX(TotalVehicle) AS TotalVehicle,
		MAX(TotalRoute) AS TotalRoute,
		MAX(TotalDraft) AS TotalDraft,
		MAX(TotalSubmit) AS TotalSubmit,
		MAX(TotalInProcess) AS TotalInProcess,
		MAX(TotalOnDelivery) AS TotalOnDelivery,
		MAX(TotalArrive) AS TotalArrive,
		MAX(TotalCompleted) AS TotalCompleted,
		MAX(TotalClose) AS TotalClose,
		MAX(TotalCancel) AS TotalCancel,
		MAX(RequestNoData) AS RequestNoData
	FROM

	(
	SELECT 
	tor.RequestNo AS RequestNo, 
	MAX(tor.IsActive + 0) AS IsActive,
	MIN(tor.CreatedBy) AS CreatedBy,
	MIN(tor.CreatedDate) AS CreatedDate,
	-- Vehicle with no route is not counted
	COUNT(DISTINCT CASE WHEN tor.IsActive = 1 THEN tor.IDRequest END) AS TotalVehicle,
	-- Total active route
	COUNT(DISTINCT CASE WHEN tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalRoute

	FROM 

	-- TABLE JOIN
	dbo.TransportOrderRequest as tor
	LEFT JOIN
	dbo.TransportOrder as tot
	ON tor.IDRequest = tot.IDRequest
	LEFT JOIN
	dbo.TransportOrderDetail as tod
	ON tot.IDTransportOrder = tod.IDTransportOrder

	-- FILTER CLAUSE
	LEFT JOIN dbo.MasterUserLocationMapping as map
	ON tor.CreatedBy = map.IDUser

	LEFT JOIN dbo.MasterLocation as locP
	ON map.IDLocation = locP.IDLocation

	LEFT JOIN dbo.MasterLocation as locC
	ON locP.IDLocation = locC.IDLocation

	WHERE 
		(@ShipDateBeginFilter IS NULL OR @ShipDateBeginFilter <= tor.ShipmentDate)
		AND
		(@ShipDateEndFilter IS NULL OR @ShipDateEndFilter >= tor.ShipmentDate)
		AND 
		(
			(@ONFilter = '' OR ';' + @ONFilter + ';' LIKE ('%;' + tot.STONo + ';%'))
			AND
			(@ZoneFilter = '' OR ';' + @ZoneFilter + ';' LIKE ('%;' + tor.Zone + ';%'))
			AND
			(@OTFilter = '' OR ';' + @OTFilter + ';' LIKE ('%;' + tot.OrderType + ';%'))
			AND
			(@OVFilter = '' OR ';' + @OVFilter + ';' LIKE ('%;' + tot.VehicleType + ';%'))
			AND
			(@MTFilter = '' OR ';' + @MTFilter + ';' LIKE ('%;' + tod.MaterialType + ';%'))
			AND
			(@StatusFilter = '' OR ';' + @StatusFilter + ';' LIKE ('%;' + tot.OrderStatus + ';%'))
			AND
			(@SenderFilter = '' OR ';' + @SenderFilter + ';' LIKE ('%;' + tot.SenderIDLocation + ';%'))
			AND
			(@ReceiverFilter = '' OR ';' + @ReceiverFilter + ';' LIKE ('%;' + tot.ReceiverIDLocation + ';%'))
		)
		AND(
			';' + @LocationFilter + ';' LIKE ('%;' + locP.IDLocation + ';%')
			OR
			';' + @LocationFilter + ';' LIKE ('%;' + locC.IDLocation + ';%')
			OR
			@CreatorFilter = tor.CreatedBy
			OR
			@AllAccess = 1
		)

	GROUP BY 
		tor.RequestNo,
		tor.ShipmentDate
	)
	AS tmain
	LEFT JOIN 
	(
	SELECT
	-- Count routes based on status
	tor.RequestNo AS RequestNoData,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'Draft' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalDraft,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'Submit' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalSubmit,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'In Process' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalInProcess,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'On Delivery' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalOnDelivery,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'Arrive At Destination and Waiting Confirmation' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalArrive,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'Complete' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalCompleted,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'Close' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalClose,
	COUNT(DISTINCT CASE WHEN tot.OrderStatus = 'Cancel' AND tot.IsActive = 1 THEN tot.IDTransportOrder END) AS TotalCancel

	FROM 

	-- TABLE JOIN
	dbo.TransportOrderRequest as tor
	LEFT JOIN
	dbo.TransportOrder as tot
	ON tor.IDRequest = tot.IDRequest

	GROUP BY 
		tor.RequestNo
	)
	AS tdata

	-- Top level join
	ON tmain.RequestNo = tdata.RequestNoData

	GROUP BY tmain.RequestNo
END