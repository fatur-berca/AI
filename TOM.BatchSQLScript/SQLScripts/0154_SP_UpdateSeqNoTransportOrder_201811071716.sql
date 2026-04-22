SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[UpdateSeqNoTransportOrder]

AS
BEGIN

DECLARE @latestSeqNo VARCHAR(10),
	   @idTransOrder INT,
	   @shipmentDate DATE,
	   @senderIDLoc VARCHAR(50),
	   @receiverIDLoc VARCHAR(50)

DECLARE reqNo CURSOR FOR
SELECT IDTransportOrder,ShipmentDate,SenderIDLocation, ReceiverIDLocation
FROM [dbo].[TransportOrder]
WHERE SeqNo IS NULL

OPEN reqNo
FETCH NEXT FROM reqNo INTO @idTransOrder,@shipmentDate,@senderIDLoc,@receiverIDLoc
WHILE @@FETCH_STATUS = 0  
BEGIN

SELECT TOP 1 @latestSeqNo = SeqNo
FROM [dbo].[TransportOrder]
WHERE ShipmentDate = @shipmentDate AND SeqNo LIKE 'OS%'
ORDER BY SeqNo DESC

--contoh @latestSeqNo yang di dapat adalah OS001
--SUBSTRING(@latestSeqNo,3,LEN(@latestSeqNo)) mendapatkan nilai 001, kemudian di cast jadi int, dan di tambah 1 menjadi 2
--REPLACE(STR(2(latest seq no),3),SPACE(1),'0') menjadikan angka 2, menjadi 002
SET @latestSeqNo = REPLACE(STR(CAST((CAST(SUBSTRING(@latestSeqNo,3,LEN(@latestSeqNo)) AS INT) + 1) AS VARCHAR),3),SPACE(1),'0')

UPDATE [dbo].[TransportOrder]
SET SeqNo = 'OS'+@latestSeqNo
WHERE IDTransportOrder = @idTransOrder

FETCH NEXT FROM reqNo INTO @idTransOrder,@shipmentDate,@senderIDLoc,@receiverIDLoc
END   
CLOSE reqNo;
DEALLOCATE reqNo;

END;
GO


