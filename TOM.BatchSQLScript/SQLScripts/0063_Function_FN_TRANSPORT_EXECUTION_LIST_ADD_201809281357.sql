IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'FN_TRANSPORT_EXECUTION_LIST_ADD') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION FN_TRANSPORT_EXECUTION_LIST_ADD
GO

CREATE FUNCTION [dbo].[FN_TRANSPORT_EXECUTION_LIST_ADD]
(    
    @executionTypeFilter VARCHAR(50),
    @zoneFilter VARCHAR(50),
    @orderTypeFilter VARCHAR(MAX),
    @weekFilter INT,
    @senderFilter VARCHAR(500),
    @vehicleTypeFilter VARCHAR(50)
)
RETURNS @result TABLE 
(
	IDCheck INT,
	IDTransportOrder INT,
	IDTransportOrderDetail INT,
	ConcatIDTransportOrderDetail VARCHAR(MAX),--digunakan untuk pengecekan pada saat generate TN(synergy,RMT PP), apakah yang di centang mempunyai id yang sama
	OrderNumber VARCHAR(50),
	OrderType VARCHAR(50),
	Sender VARCHAR(100),
	Receiver VARCHAR(100),
	OrderedVehicleType VARCHAR(50),
	ShipmentDate DATE,
	MaterialType VARCHAR(50),
	[Description] VARCHAR(100),
	Qty DECIMAL(18,4),
	UoM VARCHAR(50),
	OrderRemarks VARCHAR(500),
	Sequence VARCHAR(5),
	OrderCreator VARCHAR(50),
	TransportationNumber VARCHAR(50),
	Vendor VARCHAR(50)
)
AS
BEGIN
	DECLARE
		@idCheck INT,
		@idTransportOrder INT,
		@idTransportOrderDetail INT,
		@tempConcatIdTod VARCHAR(MAX),
		@orderNumber VARCHAR(50),
		@orderType VARCHAR(50),
		@idSender INT,
		@sender VARCHAR(100),
		@idReceiver INT,
		@receiver VARCHAR(100),
		@vehicleType VARCHAR(50),
		@shipmentDate DATE,
		@materialType VARCHAR(50),
		@description VARCHAR(100),
		@qty DECIMAL(18,4),
		@uom VARCHAR(50),
		@orderRemarks VARCHAR(500),
		@seq VARCHAR(5),
		@createdBy VARCHAR(50),
		@idTransportOrderPairing INT,
		@idTransportOrderDetailPairing INT,
		@orderNumberPairing VARCHAR(50),
		@orderTypePairing VARCHAR(50),
		@idSenderPairing INT,
		@senderPairing VARCHAR(100),
		@idReceiverPairing INT,
		@receiverPairing VARCHAR(100),
		@vehicleTypePairing VARCHAR(50),
		@shipmentDatePairing DATE,
		@materialTypePairing VARCHAR(50),
		@descriptionPairing VARCHAR(100),
		@qtyPairing DECIMAL(18,4),
		@uomPairing VARCHAR(50),
		@orderRemarksPairing VARCHAR(500),
		@seqPairing VARCHAR(5),
		@createdByPairing VARCHAR(50),
		@seqBefore VARCHAR(5),--digunakan di synergy untuk penanda kapan membuat check box baru
		@dateBefore DATE--digunakan di synergy untuk penanda kapan membuat check box baru
		
	
	IF @executionTypeFilter = 'ALL'
     BEGIN
	   INSERT INTO @result
	   SELECT TOP 100 1,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.LocationName,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,te.TransportNo,mv.VendorName
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN TransportExecution AS te
	   ON to1.IDTransportExecution = te.IDTransportExecution
	   LEFT JOIN MasterVendorSuggestion AS mvs
	   ON te.StartLocation = mvs.StartLocation AND te.TransportMode = mvs.TransportationMode
	   LEFT JOIN MasterVendor AS mv
	   ON mvs.SuggestedVendor = mv.IDVendor
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND te.IsActive = 1 AND mvs.IsActive = 1 AND mv.IsActive = 1
     END
     ELSE IF @executionTypeFilter = 'Synergy'
     BEGIN
	   SET @idCheck = 0;
	   SET @seqBefore = -1;
	   SET @dateBefore = NULL;
     	
	   DECLARE FGSynergy CURSOR FOR
	   SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrderDetail
	   INNER JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Finished Goods' AND to1.Remarks = 'Synergy' AND to1.ShipmentDate >= CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1
	   ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
	   OPEN FGSynergy
	   FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  
		  DECLARE PairingSynergy CURSOR FOR
		  SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy
		  FROM TransportOrder AS to1
		  INNER JOIN TransportOrderDetail AS tod
		  ON to1.IDTransportOrder = tod.IDTransportOrderDetail
		  INNER JOIN MasterMapping AS mm
		  ON tod.MaterialType = mm.MapFrom
		  INNER JOIN MasterLocation AS ml
		  ON to1.SenderIDLocation = ml.IDLocation
		  INNER JOIN MasterLocation AS ml2
		  ON to1.ReceiverIDLocation = ml2.IDLocation
		  WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Raw Material' AND to1.ReceiverIDLocation = @idSender AND to1.IsActive = 1 AND tod.IsActive = 1
		  ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
		  OPEN PairingSynergy
		  FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing
		  WHILE @@FETCH_STATUS = 0  
		  BEGIN
			 --START: BAGIAN DATA PERTAMA PAIRING
			 IF @seqBefore <> -1 AND @dateBefore IS NULL
			 BEGIN
			 	SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
			 END
			 --END: BAGIAN DATA PERTAMA PAIRING
			 
			 IF @seqBefore <> @seqPairing OR @dateBefore <> @shipmentDatePairing
			 BEGIN
			 	SET @tempConcatIdTod = @tempConcatIdTod + '-' + @idTransportOrderDetail;
			 	SET @tempConcatIdTod = SUBSTRING(@tempConcatIdTod,2,LEN(@tempConcatIdTod))
			 	--START: BAGIAN INSERT SYNERGY
			 	INSERT INTO @result
			 	(
			 		IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrderDetail,OrderNumber,OrderType,Sender,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,Vendor
			 	)
			 	VALUES
			 	(
			 		@idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@sender,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',''
			 	)
				--END: BAGIAN INSERT SYNERGY
				--START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
				UPDATE @result SET ConcatIDTransportOrderDetail = @tempConcatIdTod WHERE IDCheck = @idCheck
				--END: BAGIAN UPDATE CONCAT ID
				SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
				SET @tempConcatIdTod = '';
			 END
			 SET @tempConcatIdTod = @tempConcatIdTod + '-' + @idTransportOrderDetailPairing;
			 --START: BAGIAN INSERT PAIRING
			 INSERT INTO @result
			 (
			 	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrderDetail,OrderNumber,OrderType,Sender,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,Vendor
			 )
			 VALUES
			 (
			 	@idCheck,@idTransportOrderPairing,@idTransportOrderDetailPairing,'',@orderNumberPairing,@orderTypePairing,@senderPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,'',''
			 )
			 --END: BAGIAN INSERT PAIRING
			 FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing
		  END
		  CLOSE PairingSynergy;
		  DEALLOCATE PairingSynergy;
		  
		  SET @tempConcatIdTod = @tempConcatIdTod + '-' + @idTransportOrderDetail;
		  SET @tempConcatIdTod = SUBSTRING(@tempConcatIdTod,2,LEN(@tempConcatIdTod))
		  --START: BAGIAN INSERT SYNERGY
		  INSERT INTO @result
		  (
			 IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrderDetail,OrderNumber,OrderType,Sender,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,Vendor
		  )
		  VALUES
		  (
			 @idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@sender,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',''
		  )
		  --END: BAGIAN INSERT SYNERGY
		  --START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
		  UPDATE @result SET ConcatIDTransportOrderDetail = @tempConcatIdTod WHERE IDCheck = @idCheck
		  --END: BAGIAN UPDATE CONCAT ID
		  SET @tempConcatIdTod = '';
		  SET @seqBefore = -1;
		  SET @dateBefore = NULL;
		  
		  FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy
	   END
	   CLOSE FGSynergy;
	   DEALLOCATE FGSynergy;
     END
     ELSE IF @executionTypeFilter = 'RMT PP'
     BEGIN
	   SET @idCheck = 0;
	   SET @seqBefore = -1;
	   SET @dateBefore = NULL;
     	
	   DECLARE RMTPP CURSOR FOR
	   SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrderDetail
	   INNER JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Raw Material' AND ml.[Type] = 'Factory' AND ml2.[Type] = 'Agent' AND to1.ShipmentDate >= CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1
	   ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
	   OPEN RMTPP
	   FETCH NEXT FROM RMTPP INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  
		  DECLARE PairingRMTPP CURSOR FOR
		  SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy
		  FROM TransportOrder AS to1
		  INNER JOIN TransportOrderDetail AS tod
		  ON to1.IDTransportOrder = tod.IDTransportOrderDetail
		  INNER JOIN MasterMapping AS mm
		  ON tod.MaterialType = mm.MapFrom
		  INNER JOIN MasterLocation AS ml
		  ON to1.SenderIDLocation = ml.IDLocation
		  INNER JOIN MasterLocation AS ml2
		  ON to1.ReceiverIDLocation = ml2.IDLocation
		  WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Raw Material' AND ml.[Type] = 'Agent' AND ml2.[Type] = 'Factory' AND to1.ReceiverIDLocation = @idSender AND to1.IsActive = 1 AND tod.IsActive = 1
		  ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
		  OPEN PairingRMTPP
		  FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing
		  WHILE @@FETCH_STATUS = 0  
		  BEGIN
			 --START: BAGIAN DATA PERTAMA PAIRING
			 IF @seqBefore <> -1 AND @dateBefore IS NULL
			 BEGIN
			 	SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
			 END
			 --END: BAGIAN DATA PERTAMA PAIRING
			 
			 IF @seqBefore <> @seqPairing OR @dateBefore <> @shipmentDatePairing
			 BEGIN
			 	SET @tempConcatIdTod = @tempConcatIdTod + '-' + @idTransportOrderDetail;
			 	SET @tempConcatIdTod = SUBSTRING(@tempConcatIdTod,2,LEN(@tempConcatIdTod))
			 	--START: BAGIAN INSERT SYNERGY
			 	INSERT INTO @result
			 	(
			 		IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrderDetail,OrderNumber,OrderType,Sender,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,Vendor
			 	)
			 	VALUES
			 	(
			 		@idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@sender,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',''
			 	)
				--END: BAGIAN INSERT SYNERGY
				--START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
				UPDATE @result SET ConcatIDTransportOrderDetail = @tempConcatIdTod WHERE IDCheck = @idCheck
				--END: BAGIAN UPDATE CONCAT ID
				SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
				SET @tempConcatIdTod = '';
			 END
			 SET @tempConcatIdTod = @tempConcatIdTod + '-' + @idTransportOrderDetailPairing;
			 --START: BAGIAN INSERT PAIRING
			 INSERT INTO @result
			 (
			 	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrderDetail,OrderNumber,OrderType,Sender,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,Vendor
			 )
			 VALUES
			 (
			 	@idCheck,@idTransportOrderPairing,@idTransportOrderDetailPairing,'',@orderNumberPairing,@orderTypePairing,@senderPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,'',''
			 )
			 --END: BAGIAN INSERT PAIRING
			 FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing
		  END
		  CLOSE PairingRMTPP;
		  DEALLOCATE PairingRMTPP;
		  
		  SET @tempConcatIdTod = @tempConcatIdTod + '-' + @idTransportOrderDetail;
		  SET @tempConcatIdTod = SUBSTRING(@tempConcatIdTod,2,LEN(@tempConcatIdTod))
		  --START: BAGIAN INSERT SYNERGY
		  INSERT INTO @result
		  (
			 IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrderDetail,OrderNumber,OrderType,Sender,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,Vendor
		  )
		  VALUES
		  (
			 @idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@sender,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',''
		  )
		  --END: BAGIAN INSERT SYNERGY
		  --START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
		  UPDATE @result SET ConcatIDTransportOrderDetail = @tempConcatIdTod WHERE IDCheck = @idCheck
		  --END: BAGIAN UPDATE CONCAT ID
		  SET @tempConcatIdTod = '';
		  SET @seqBefore = -1;
		  SET @dateBefore = NULL;
		  
		  FETCH NEXT FROM RMTPP INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy
	   END
	   CLOSE RMTPP;
	   DEALLOCATE RMTPP;
     END
	ELSE IF @executionTypeFilter = 'Next Day'
     BEGIN
	   INSERT INTO @result
	   SELECT TOP 100 1,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.LocationName,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,te.TransportNo,mv.VendorName
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN TransportExecution AS te
	   ON to1.IDTransportExecution = te.IDTransportExecution
	   LEFT JOIN MasterVendorSuggestion AS mvs
	   ON te.StartLocation = mvs.StartLocation AND te.TransportMode = mvs.TransportationMode
	   LEFT JOIN MasterVendor AS mv
	   ON mvs.SuggestedVendor = mv.IDVendor
	   WHERE to1.ShipmentDate > CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1 AND te.IsActive = 1 AND mvs.IsActive = 1 AND mv.IsActive = 1
	   ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC
     END
     RETURN;
END