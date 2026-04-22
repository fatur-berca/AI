IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateTransportSummaryStatusComplete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateTransportSummaryStatusComplete]
GO 

CREATE PROCEDURE [dbo].[UpdateTransportSummaryStatusComplete]

AS
BEGIN
    UPDATE TransportOrder
    SET OrderStatus = 'Complete',GRDate = CASE WHEN GRDate IS NULL THEN DATEADD(DAY, 1, UpdatedDate) ELSE GRDate END,UpdatedBy = 'system',UpdatedDate = GETDATE()
    WHERE OrderStatus = 'Arrive At Destination and Waiting For Confirmation' AND UpdatedDate < DATEADD(DAY, -1, GETDATE())
END