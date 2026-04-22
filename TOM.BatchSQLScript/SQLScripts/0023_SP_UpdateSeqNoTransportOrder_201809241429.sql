IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateSeqNoTransportOrder]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateSeqNoTransportOrder]
GO 

CREATE PROCEDURE [dbo].[UpdateSeqNoTransportOrder]

AS
BEGIN

DECLARE @latestSeqNo VARCHAR(5),
	   @shipmentDate DATE,
	   @senderIDLoc VARCHAR(50),
	   @receiverIDLoc VARCHAR(50)

SELECT @latestSeqNo = SeqNo
FROM [dbo].[TransportOrder]
WHERE ShipmentDate = CAST(GETDATE() AS DATE) AND SeqNo IS NOT NULL
ORDER BY SeqNo DESC

DECLARE reqNo CURSOR FOR
SELECT ShipmentDate,SenderIDLocation, ReceiverIDLocation
FROM [dbo].[TransportOrder]
WHERE SeqNo IS NULL

OPEN reqNo
FETCH NEXT FROM reqNo INTO @shipmentDate,@senderIDLoc,@receiverIDLoc
WHILE @@FETCH_STATUS = 0  
BEGIN

SET @latestSeqNo = CAST((CAST(@latestSeqNo AS INT) + 1) AS VARCHAR)
UPDATE [dbo].[TransportOrder]
SET SeqNo = @latestSeqNo
WHERE ShipmentDate = @shipmentDate AND SenderIDLocation = @senderIDLoc AND ReceiverIDLocation = @receiverIDLoc AND SeqNo IS NULL

FETCH NEXT FROM reqNo INTO @shipmentDate,@senderIDLoc,@receiverIDLoc
END   
CLOSE reqNo;
DEALLOCATE reqNo;

END;
GO