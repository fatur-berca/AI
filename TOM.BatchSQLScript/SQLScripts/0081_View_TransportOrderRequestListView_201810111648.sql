USE [TOM]
GO

/****** Object:  View [dbo].[TransportOrderRequestListView]    Script Date: 10/11/2018 4:46:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TransportOrderRequestListView]
AS
SELECT        dbo.TransportOrder.IDTransportOrder, dbo.TransportOrderRequest.IDRequest, dbo.TransportOrder.STONo, dbo.TransportOrderRequest.RequestNo, dbo.TransportOrderRequest.Zone, dbo.TransportOrder.OrderType, 
                         dbo.TransportOrderRequest.VehicleType, dbo.TransportOrderRequest.ShipmentDate, dbo.TransportOrderDetail.MaterialType, dbo.TransportOrder.OrderStatus, dbo.TransportOrder.SenderIDLocation, 
                         dbo.TransportOrder.ActualSenderIDLocation, dbo.TransportOrder.ReceiverIDLocation, dbo.TransportOrder.ActualReceiverIDLocation
FROM            dbo.TransportOrder RIGHT OUTER JOIN
                         dbo.TransportOrderDetail ON dbo.TransportOrder.IDTransportOrder = dbo.TransportOrderDetail.IDTransportOrder LEFT OUTER JOIN
                         dbo.TransportOrderRequest ON dbo.TransportOrder.IDRequest = dbo.TransportOrderRequest.IDRequest
WHERE        (NOT (dbo.TransportOrderRequest.IDRequest IS NULL))
GO



