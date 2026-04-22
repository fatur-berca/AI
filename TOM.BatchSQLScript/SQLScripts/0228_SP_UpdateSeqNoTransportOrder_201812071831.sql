ALTER PROCEDURE [dbo].[UpdateSeqNoTransportOrder]
AS
BEGIN

DECLARE @latestSeqNo VARCHAR(10),
        @idTransOrder INT,
        @shipmentDate DATE,
        @senderIDLoc VARCHAR(50),
        @receiverIDLoc VARCHAR(50),
        @createdBy VARCHAR(128)

DECLARE reqNo CURSOR FOR
SELECT IDTransportOrder,ShipmentDate,SenderIDLocation, ReceiverIDLocation,CreatedBy
FROM [dbo].[TransportOrder]
WHERE SeqNo IS NULL AND (STONo IS NOT NULL OR STONo <> '') AND STONo NOT LIKE 'PO%'--AND CreatedBy = 'system'

OPEN reqNo
FETCH NEXT FROM reqNo INTO @idTransOrder,@shipmentDate,@senderIDLoc,@receiverIDLoc,@createdBy
WHILE @@FETCH_STATUS = 0 
BEGIN
    IF @createdBy = 'system'
    BEGIN
	
	   SELECT @latestSeqNo = MAX (CAST(SUBSTRING(SeqNo,3,LEN(SeqNo)) AS INT))
	   FROM [dbo].[TransportOrder]
	   WHERE ShipmentDate = @shipmentDate AND SeqNo LIKE 'OS%'
	   --ORDER BY SeqNo DESC

	   IF @latestSeqNo IS NULL
	   BEGIN
		  SET @latestSeqNo = 0;
	   END

	   --contoh @latestSeqNo yang di dapat adalah OS001
	   --SUBSTRING(@latestSeqNo,3,LEN(@latestSeqNo)) mendapatkan nilai 001, kemudian di cast jadi int, dan di tambah 1 menjadi 2
	   --REPLACE(STR(2(latest seq no),3),SPACE(1),'0') menjadikan angka 2, menjadi 002
	   SET @latestSeqNo = REPLACE(STR(CAST((CAST(@latestSeqNo AS INT) + 1) AS VARCHAR),3),SPACE(1),'0')

	   UPDATE [dbo].[TransportOrder]
	   SET SeqNo = 'OS'+@latestSeqNo
	   WHERE IDTransportOrder = @idTransOrder

    END
    ELSE
    BEGIN

	   SELECT @latestSeqNo = MAX (CAST(SeqNo AS INT))
	   FROM [dbo].[TransportOrder]
	   WHERE ShipmentDate = @shipmentDate AND SeqNo NOT LIKE 'OS%' AND IDRequest IS NULL
	   --ORDER BY SeqNo DESC

	   IF @latestSeqNo IS NULL
	   BEGIN
		  SET @latestSeqNo = 0;
	   END

	   --contoh @latestSeqNo yang di dapat adalah OS001
	   --SUBSTRING(@latestSeqNo,3,LEN(@latestSeqNo)) mendapatkan nilai 001, kemudian di cast jadi int, dan di tambah 1 menjadi 2
	   --REPLACE(STR(2(latest seq no),3),SPACE(1),'0') menjadikan angka 2, menjadi 002
	   SET @latestSeqNo = CAST((CAST (@latestSeqNo AS INT) + 1) AS VARCHAR);

	   UPDATE [dbo].[TransportOrder]
	   SET SeqNo = @latestSeqNo
	   WHERE IDTransportOrder = @idTransOrder

    END
    FETCH NEXT FROM reqNo INTO @idTransOrder,@shipmentDate,@senderIDLoc,@receiverIDLoc,@createdBy

END  
CLOSE reqNo;
DEALLOCATE reqNo;

END;
GO