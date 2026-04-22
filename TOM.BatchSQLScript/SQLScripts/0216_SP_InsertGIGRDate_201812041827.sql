IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertGIGRDate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertGIGRDate]
GO 

CREATE PROCEDURE [dbo].[InsertGIGRDate]

AS
BEGIN
	--BAGIAN GI
    UPDATE transOrder
    SET transOrder.GIDate = m.PostingDate,transOrder.OrderStatus = 'On Delivery',transOrder.UpdatedDate = GETDATE()
    FROM TransportOrder AS transOrder
    INNER JOIN DFIS.dbo.MB51 AS m
    ON transOrder.STONo = m.PO
    WHERE ((m.Sloc = 1000 AND m.MvT = 641) OR (m.SLoc = 2000 AND m.MvT = 101)) AND transOrder.GIDate IS NULL AND transOrder.OrderStatus = 'In Process'
    
    --BAGIAN GR
    UPDATE transOrder
    SET transOrder.GRDate = m.PostingDate,transOrder.OrderStatus = 'Arrive At Destination and Waiting For Confirmation',transOrder.UpdatedDate = GETDATE()
    FROM TransportOrder AS transOrder
    INNER JOIN DFIS.dbo.MB51 AS m
    ON transOrder.STONo = m.PO
    WHERE ((m.Sloc = 1000 AND m.MvT = 101) OR (m.SLoc = 2000 AND m.MvT = 311)) AND transOrder.GRDate IS NULL AND transOrder.OrderStatus = 'On Delivery'
END