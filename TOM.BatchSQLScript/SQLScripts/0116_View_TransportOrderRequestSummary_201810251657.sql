USE [TOM]
GO

/****** Object:  View [dbo].[TransportOrderRequestSummary]    Script Date: 10/25/2018 4:56:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[TransportOrderRequestSummary]
AS
SELECT				tor.IDRequest AS IDRequest,
						tor.RequestNo AS RequestNo, 
						tor.Zone AS [Zone], 
						tor.ShipmentDate AS ShipmentDate,
						tor.IsActive AS IsActive,
						
						tot.IDTransportOrder,
						tod.IDTransportOrderDetail,

						tot.SenderIDLocation,
						tot.ReceiverIDLocation,
						tot.OrderStatus,
						tot.OrderType,
						tot.VehicleType,
						tot.STONo,
						tod.MaterialType,

						tor.CreatedBy AS CreatedBy,
						tor.CreatedDate AS CreatedDate,
						tot.IsActive AS OrderIsActive

						/*
						MIN(tot.IDTransportOrder) AS IDTransportOrder,
						MIN(tod.IDTransportOrderDetail) AS IDTransportOrderDetail,
					
						(';' + STRING_AGG(tot.SenderIDLocation, ';') + ';') AS Senders,
						(';' + STRING_AGG(tot.ReceiverIDLocation, ';') + ';') AS Receivers,
						(';' + STRING_AGG(tot.OrderStatus, ';') + ';') AS OrderStatuses,
						(';' + STRING_AGG(tot.OrderType, ';') + ';') AS OrderTypes,
						(';' + STRING_AGG(tot.VehicleType, ';') + ';') AS VehicleTypes,
						(';' + STRING_AGG(tod.MaterialType, ';') + ';') AS MaterialTypes,
						(';' + STRING_AGG(tot.STONo, ';') + ';') AS STONumbers,
						(COUNT_BIG(tot.IDTransportOrder)) AS TotalUnits,
						(SUM(tot.IsActive + 0)) AS TotalActiveUnits
						*/

	FROM            dbo.TransportOrderRequest AS tor
					INNER JOIN dbo.TransportOrder AS tot
					ON tot.IDRequest = tor.IDRequest 
					INNER JOIN dbo.TransportOrderDetail AS tod
					ON tot.IDTransportOrder = tod.IDTransportOrder 
GO


