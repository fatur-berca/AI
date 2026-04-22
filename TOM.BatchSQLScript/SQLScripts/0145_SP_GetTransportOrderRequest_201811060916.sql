SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zecchan Silverlake
-- Create date: 2018-11-05
-- Description:	To select data from TO
-- =============================================

CREATE PROCEDURE SP_GetTransportOrderRequest 
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
	@LocationFilter varchar(50) = ''

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
	tor.RequestNo AS RequestNo, 
	MAX(tor.IsActive + 0) AS IsActive,
	MIN(tor.CreatedBy) AS CreatedBy,
	MIN(tor.CreatedDate) AS CreatedDate,
	COUNT(DISTINCT tor.IDRequest) AS TotalVehicle,
	COUNT(CASE WHEN tot.OrderStatus = 'In Process' THEN tot.OrderStatus END) AS TotalInProcess,
	COUNT(CASE WHEN tot.OrderStatus = 'On Delivery' THEN tot.OrderStatus END) AS TotalOnDelivery,
	COUNT(CASE WHEN tot.OrderStatus = 'Completed' THEN tot.OrderStatus END) AS TotalCompleted

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
			';' + @ONFilter + ';' LIKE ('%;' + tot.STONo + ';%')
			OR
			';' + @ZoneFilter + ';' LIKE ('%;' + tor.Zone + ';%')
			OR
			';' + @OTFilter + ';' LIKE ('%;' + tot.OrderType + ';%')
			OR
			';' + @OVFilter + ';' LIKE ('%;' + tot.OrderType + ';%')
			OR
			';' + @MTFilter + ';' LIKE ('%;' + tod.MaterialType + ';%')
			OR
			@StatusFilter = tot.OrderStatus 
			OR
			';' + @SenderFilter + ';' LIKE ('%;' + tot.SenderIDLocation + ';%')
			OR
			';' + @ReceiverFilter + ';' LIKE ('%;' + tot.ReceiverIDLocation + ';%')
			OR
			@CreatorFilter = tor.CreatedBy
			OR
			';' + @LocationFilter + ';' LIKE ('%;' + locP.IDLocation + ';%')
			OR
			';' + @LocationFilter + ';' LIKE ('%;' + locC.IDLocation + ';%')
			OR
			(
				@ONFilter = '' 
				AND @ZoneFilter = ''
				AND @OTFilter = ''
				AND @OVFilter = ''
				AND @MTFilter = ''
				AND @StatusFilter = ''
				AND @SenderFilter = ''
				AND @ReceiverFilter = ''
				AND @CreatorFilter = ''
				AND @LocationFilter = ''
			)
		)

	GROUP BY 
		tor.RequestNo
END
GO
