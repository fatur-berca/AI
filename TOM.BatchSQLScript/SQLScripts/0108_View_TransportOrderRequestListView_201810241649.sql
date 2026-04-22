USE [TOM]
GO

/****** Object:  View [dbo].[TransportOrderRequestListView]    Script Date: 2018/10/24 16:28:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[TransportOrderRequestListView]
AS
SELECT        dbo.TransportOrder.IDTransportOrder, dbo.TransportOrderRequest.IDRequest, dbo.TransportOrder.STONo, dbo.TransportOrderRequest.RequestNo, dbo.TransportOrderRequest.Zone, dbo.TransportOrder.OrderType, 
                         dbo.TransportOrderRequest.VehicleType, dbo.TransportOrderRequest.ShipmentDate, dbo.TransportOrderDetail.MaterialType, dbo.TransportOrder.OrderStatus, dbo.TransportOrder.SenderIDLocation, 
                         dbo.TransportOrder.ActualSenderIDLocation, dbo.TransportOrder.ReceiverIDLocation, dbo.TransportOrder.ActualReceiverIDLocation, dbo.TransportOrderRequest.IsActive, dbo.TransportOrderRequest.CreatedBy
FROM            dbo.TransportOrder RIGHT OUTER JOIN
                         dbo.TransportOrderDetail ON dbo.TransportOrder.IDTransportOrder = dbo.TransportOrderDetail.IDTransportOrder LEFT OUTER JOIN
                         dbo.TransportOrderRequest ON dbo.TransportOrder.IDRequest = dbo.TransportOrderRequest.IDRequest
WHERE        (NOT (dbo.TransportOrderRequest.IDRequest IS NULL))
GO


