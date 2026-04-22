-- Created By : Fadiyah
-- Date : 2018-11-04

ALTER FUNCTION [dbo].[FN_TRANSPORT_EXECUTION_LIST_ADD]
(    
    @executionTypeFilter VARCHAR(50),
    @zoneFilter VARCHAR(50),
    @orderTypeFilter VARCHAR(MAX),
    @weekFilter INT,
    @senderFilter VARCHAR(MAX),
    @vehicleTypeFilter VARCHAR(50),
	@yearFilter INT
)
RETURNS @result TABLE 
(
	IDCheck INT,
	IDTransportOrder INT,
	IDTransportOrderDetail INT,
	ConcatIDTransportOrder VARCHAR(MAX),--digunakan untuk pengecekan pada saat generate TN(synergy,RMT PP), apakah yang di centang mempunyai id yang sama
	OrderNumber VARCHAR(50),
	OrderType VARCHAR(50),
	IDSenderLoc VARCHAR(50),
	Sender VARCHAR(100),
	IDReceiverLoc VARCHAR(50),
	Receiver VARCHAR(100),
	OrderedVehicleType VARCHAR(50),
	CostCenter VARCHAR(50),
	ShipmentDate DATE,
	MaterialType VARCHAR(50),
	[Description] VARCHAR(100),
	Qty DECIMAL(18,4),
	UoM VARCHAR(50),
	OrderRemarks VARCHAR(500),
	Sequence VARCHAR(5),
	OrderCreator VARCHAR(50),
	TransportationNumber VARCHAR(50),
	IDVendorSuggestion INT,
	VendorSuggestion VARCHAR(50),
	UpdatedBy VARCHAR(50),
	UpdatedDate DATETIME
)
AS
BEGIN
	DECLARE
		@idCheck INT,
		@idTransportOrder INT,
		@idTransportOrderDetail INT,
		@tempConcatIdTo VARCHAR(MAX),
		@orderNumber VARCHAR(50),
		@orderType VARCHAR(50),
		@costCenter VARCHAR(50),
		@idSender VARCHAR(50),
		@sender VARCHAR(100),
		@idReceiver VARCHAR(50),
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
		@updatedBy VARCHAR(50),
		@updatedDate DATETIME,
		@idTransportOrderPairing INT,
		@idTransportOrderDetailPairing INT,
		@orderNumberPairing VARCHAR(50),
		@orderTypePairing VARCHAR(50),
		@idSenderPairing VARCHAR(50),
		@senderPairing VARCHAR(100),
		@idReceiverPairing VARCHAR(50),
		@receiverPairing VARCHAR(100),
		@vehicleTypePairing VARCHAR(50),
		@costCenterPairing VARCHAR(50),
		@shipmentDatePairing DATE,
		@materialTypePairing VARCHAR(50),
		@descriptionPairing VARCHAR(100),
		@qtyPairing DECIMAL(18,4),
		@uomPairing VARCHAR(50),
		@orderRemarksPairing VARCHAR(500),
		@seqPairing VARCHAR(5),
		@createdByPairing VARCHAR(50),
		@updatedByPairing VARCHAR(50),
		@updatedDatePairing DATETIME,
		@seqBefore VARCHAR(5),--digunakan di synergy untuk penanda kapan membuat check box baru
		@dateBefore DATE,--digunakan di synergy untuk penanda kapan membuat check box baru
		@StartDate DATETIME,
		@EndDate DATETIME
		
	SELECT @StartDate = mgw.StartDate, @EndDate = mgw.EndDate 
	FROM MasterGenWeek AS mgw
	WHERE mgw.[Week] = @weekFilter AND mgw.[Year] = @yearFilter	
	
	SET @tempConcatIdTo = '';
	IF @executionTypeFilter = 'ALL' AND @senderFilter = ''
     BEGIN	   	   
			INSERT INTO @result
		   SELECT to1.IDTransportOrder,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),to1.UpdatedBy,to1.UpdatedDate
		   FROM TransportOrderDetail AS tod
		   INNER JOIN TransportOrder AS to1
		   ON tod.IDTransportOrder = to1.IDTransportOrder
		   INNER JOIN MasterLocation AS ml
		   ON to1.SenderIDLocation = ml.IDLocation
		   INNER JOIN MasterLocation AS ml2
		   ON to1.ReceiverIDLocation = ml2.IDLocation
		   LEFT JOIN (
			  SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
			  FROM TransportExecution AS te
			  LEFT JOIN MasterVendorSuggestion AS mvs
			  ON te.StartLocation = mvs.StartLocation AND te.TransportMode = mvs.TransportationMode
			  LEFT JOIN MasterVendor AS mv
			  ON mvs.SuggestedVendor = mv.IDVendor
			  WHERE te.IsActive = 1 AND (mvs.IsActive IS NULL OR mvs.IsActive = 1) AND (mv.IsActive IS NULL OR mv.IsActive = 1)
		   ) AS jte
		   ON to1.IDTransportExecution = jte.IDTransportExecution
		   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
		   to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased) AND to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType) AND to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ','))
     END
	 ELSE IF @executionTypeFilter = 'ALL' AND @senderFilter != ''
	 BEGIN
		INSERT INTO @result
		   SELECT to1.IDTransportOrder,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),to1.UpdatedBy,to1.UpdatedDate
		   FROM TransportOrderDetail AS tod
		   INNER JOIN TransportOrder AS to1
		   ON tod.IDTransportOrder = to1.IDTransportOrder
		   INNER JOIN MasterLocation AS ml
		   ON to1.SenderIDLocation = ml.IDLocation
		   INNER JOIN MasterLocation AS ml2
		   ON to1.ReceiverIDLocation = ml2.IDLocation
		   LEFT JOIN (
			  SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
			  FROM TransportExecution AS te
			  LEFT JOIN MasterVendorSuggestion AS mvs
			  ON te.StartLocation = mvs.StartLocation AND te.TransportMode = mvs.TransportationMode
			  LEFT JOIN MasterVendor AS mv
			  ON mvs.SuggestedVendor = mv.IDVendor
			  WHERE te.IsActive = 1 AND (mvs.IsActive IS NULL OR mvs.IsActive = 1) AND (mv.IsActive IS NULL OR mv.IsActive = 1)
		   ) AS jte
		   ON to1.IDTransportExecution = jte.IDTransportExecution
		   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
		   to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased) AND to1.SenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ',')) AND to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType) AND to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ','))
	 END
     ELSE IF @executionTypeFilter = 'Synergy'
     BEGIN
	   	SET @idCheck = 0;
	   SET @seqBefore = CAST(-1 AS VARCHAR(1));
	   SET @dateBefore = NULL;
     	
	   DECLARE FGSynergy CURSOR FOR
	   SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,to1.UpdatedBy,to1.UpdatedDate
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrder
	   INNER JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Finished Good' AND to1.Remarks = 'Synergy' AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND to1.ShipmentDate >= CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1
	   ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
	   OPEN FGSynergy
	   FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,@updatedBy,@updatedDate
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  
		  DECLARE PairingSynergy CURSOR FOR
		  SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,to1.UpdatedBy,to1.UpdatedDate
		  FROM TransportOrder AS to1
		  INNER JOIN TransportOrderDetail AS tod
		  ON to1.IDTransportOrder = tod.IDTransportOrder
		  INNER JOIN MasterMapping AS mm
		  ON tod.MaterialType = mm.MapFrom
		  INNER JOIN MasterLocation AS ml
		  ON to1.SenderIDLocation = ml.IDLocation
		  INNER JOIN MasterLocation AS ml2
		  ON to1.ReceiverIDLocation = ml2.IDLocation
		  WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Raw Material' AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND to1.ReceiverIDLocation = @idSender AND to1.IsActive = 1 AND tod.IsActive = 1
		  ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
		  OPEN PairingSynergy
		  FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@costCenterPairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,@updatedByPairing,@updatedDatePairing
		  WHILE @@FETCH_STATUS = 0  
		  BEGIN
			 --START: BAGIAN DATA PERTAMA PAIRING
			 IF @seqBefore = CAST(-1 AS VARCHAR(1)) AND @dateBefore IS NULL
			 BEGIN
			 	SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
			 END
			 --END: BAGIAN DATA PERTAMA PAIRING
			 
			 IF @seqBefore <> @seqPairing OR @dateBefore <> @shipmentDatePairing
			 BEGIN
			 	IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2 , 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder))
			 	BEGIN
			 	    SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
			 	END
			 	SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
			 	--START: BAGIAN INSERT SYNERGY
			 	INSERT INTO @result
			 	(
			 		IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 	)
			 	VALUES
			 	(
			 		@idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',NULL,'',@updatedBy,@updatedDate
			 	)
				--END: BAGIAN INSERT SYNERGY
				--START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
				UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
				--END: BAGIAN UPDATE CONCAT ID
				SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
				SET @tempConcatIdTo = '';
			 END
			 IF (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrderPairing)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrderPairing))
			 BEGIN
			 	SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrderPairing AS VARCHAR);
			 END
			 
			 --START: BAGIAN INSERT PAIRING
			 INSERT INTO @result
			 (
			 	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 )
			 VALUES
			 (
			 	@idCheck,@idTransportOrderPairing,@idTransportOrderDetailPairing,'',@orderNumberPairing,@orderTypePairing,@costCenterPairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,'',NULL,'',@updatedByPairing,@updatedDatePairing
			 )
			 --END: BAGIAN INSERT PAIRING
			 FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@costCenterPairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,@updatedByPairing,@updatedDatePairing
		  END
		  CLOSE PairingSynergy;
		  DEALLOCATE PairingSynergy;
		  IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder))
		  BEGIN
			 SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
		  END
		  SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
		  --START: BAGIAN INSERT SYNERGY
		  INSERT INTO @result
		  (
			 IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
			 @idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',NULL,'',@updatedBy,@updatedDate
		  )
		  --END: BAGIAN INSERT SYNERGY
		  --START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
		  UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
		  --END: BAGIAN UPDATE CONCAT ID
		  SET @tempConcatIdTo = '';
		  SET @seqBefore = CAST(-1 AS VARCHAR(1));
		  SET @dateBefore = NULL;
		  
		  FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,@updatedBy,@updatedDate
	   END
	   CLOSE FGSynergy;
	   DEALLOCATE FGSynergy;
     END
     ELSE IF @executionTypeFilter = 'RMT PP'
     BEGIN		
		SET @idCheck = 0;
	   SET @seqBefore = CAST(-1 AS VARCHAR(1));
	   SET @dateBefore = NULL;
     	
	   DECLARE RMTPP CURSOR FOR
	   SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,to1.UpdatedBy,to1.UpdatedDate
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrder
	   INNER JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Raw Material' AND ml.[Type] = 'Factory' AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND to1.ShipmentDate >= CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1
	   ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
	   OPEN RMTPP
	   FETCH NEXT FROM RMTPP INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,@updatedBy,@updatedDate
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  
		  DECLARE PairingRMTPP CURSOR FOR
		  SELECT to1.IDTransportOrder,tod.IDTransportOrderDetail,to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,to1.UpdatedBy,to1.UpdatedDate
		  FROM TransportOrder AS to1
		  INNER JOIN TransportOrderDetail AS tod
		  ON to1.IDTransportOrder = tod.IDTransportOrder
		  INNER JOIN MasterMapping AS mm
		  ON tod.MaterialType = mm.MapFrom
		  INNER JOIN MasterLocation AS ml
		  ON to1.SenderIDLocation = ml.IDLocation
		  INNER JOIN MasterLocation AS ml2
		  ON to1.ReceiverIDLocation = ml2.IDLocation
		  WHERE to1.IDTransportExecution IS NULL AND mm.MapTo = 'Raw Material' AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND to1.ReceiverIDLocation = @idSender AND to1.SenderIDLocation = @idReceiver AND to1.IsActive = 1 AND tod.IsActive = 1
		  ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC, to1.STONo ASC
	   
		  OPEN PairingRMTPP
		  FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@costCenterPairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,@updatedByPairing,@updatedDatePairing
		  WHILE @@FETCH_STATUS = 0  
		  BEGIN
			 --START: BAGIAN DATA PERTAMA PAIRING
			 IF @seqBefore = CAST(-1 AS VARCHAR(1)) AND @dateBefore IS NULL
			 BEGIN
			 	SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
			 END
			 --END: BAGIAN DATA PERTAMA PAIRING
			 
			 IF @seqBefore <> @seqPairing OR @dateBefore <> @shipmentDatePairing
			 BEGIN
			 	IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder))
			 	BEGIN
			 	    SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
			 	END
			 	SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
			 	--START: BAGIAN INSERT RMT PP
			 	INSERT INTO @result
			 	(
			 		IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 	)
			 	VALUES
			 	(
			 		@idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',NULL,'',@updatedBy,@updatedDate
			 	)
				--END: BAGIAN INSERT RMT PP
				--START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
				UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
				--END: BAGIAN UPDATE CONCAT ID
				SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
				SET @tempConcatIdTo = '';
			 END
			 IF (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrderPairing)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrderPairing))
			 BEGIN
			 	SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrderPairing AS VARCHAR);
			 END
			 --START: BAGIAN INSERT PAIRING
			 INSERT INTO @result
			 (
			 	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 )
			 VALUES
			 (
			 	@idCheck,@idTransportOrderPairing,@idTransportOrderDetailPairing,'',@orderNumberPairing,@orderTypePairing,@costCenterPairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,'',NULL,'',@updatedByPairing,@updatedDatePairing
			 )
			 --END: BAGIAN INSERT PAIRING
			 FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@idTransportOrderDetailPairing,@orderNumberPairing,@orderTypePairing,@costCenterPairing,@idSenderPairing,@senderPairing,@idReceiverPairing,@receiverPairing,@vehicleTypePairing,@shipmentDatePairing,@materialTypePairing,@descriptionPairing,@qtyPairing,@uomPairing,@orderRemarksPairing,@seqPairing,@createdByPairing,@updatedByPairing,@updatedDatePairing
		  END
		  CLOSE PairingRMTPP;
		  DEALLOCATE PairingRMTPP;
		  IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder))
		  BEGIN
			 SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
		  END
		  SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
		  --START: BAGIAN INSERT SYNERGY
		  INSERT INTO @result
		  (
			 IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
			 @idCheck,@idTransportOrder,@idTransportOrderDetail,'',@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,'',NULL,'',@updatedBy,@updatedDate
		  )
		  --END: BAGIAN INSERT SYNERGY
		  --START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
		  UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
		  --END: BAGIAN UPDATE CONCAT ID
		  SET @tempConcatIdTo = '';
		  SET @seqBefore = CAST(-1 AS VARCHAR(1));
		  SET @dateBefore = NULL;
		  
		  FETCH NEXT FROM RMTPP INTO @idTransportOrder,@idTransportOrderDetail,@orderNumber,@orderType,@costCenter,@idSender,@sender,@idReceiver,@receiver,@vehicleType,@shipmentDate,@materialType,@description,@qty,@uom,@orderRemarks,@seq,@createdBy,@updatedBy,@updatedDate
	   END
	   CLOSE RMTPP;
	   DEALLOCATE RMTPP;
     END
	ELSE IF @executionTypeFilter = 'Next Day'
     BEGIN
	   INSERT INTO @result
	   SELECT ROW_NUMBER() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,'',0,'',to1.UpdatedBy,to1.UpdatedDate
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   --LEFT JOIN (
		  --SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
		  --FROM TransportExecution AS te
		  --LEFT JOIN MasterVendorSuggestion AS mvs
		  --ON te.StartLocation = mvs.StartLocation AND te.TransportMode = mvs.TransportationMode
		  --LEFT JOIN MasterVendor AS mv
		  --ON mvs.SuggestedVendor = mv.IDVendor
		  --WHERE te.IsActive = 1 AND (mvs.IsActive IS NULL OR mvs.IsActive = 1) AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   --) AS jte
	   --ON to1.IDTransportExecution = jte.IDTransportExecution
	   WHERE to1.ShipmentDate > CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1 AND to1.IDTransportExecution IS NULL
	   ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC
     END
	 ELSE IF @executionTypeFilter = 'With TN' AND @senderFilter = ''
     BEGIN
	   INSERT INTO @result
	   SELECT jte.IDTransportExecution,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),to1.UpdatedBy,to1.UpdatedDate
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN (
		  SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
		  FROM TransportExecution AS te
		  LEFT JOIN MasterVendor AS mv
		  ON te.IDVendor = mv.IDVendor
		  WHERE te.IsActive = 1 AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   ) AS jte
	   ON to1.IDTransportExecution = jte.IDTransportExecution
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate))
	    AND to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased) AND to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType) AND to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ','))
	   ORDER BY to1.ShipmentDate DESC, to1.SeqNo ASC, to1.STONo ASC
     END	 
	 ELSE IF @executionTypeFilter = 'With TN' AND @senderFilter != ''
     BEGIN
		INSERT INTO @result
	   SELECT jte.IDTransportExecution,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),to1.UpdatedBy,to1.UpdatedDate
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN (
		  SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
		  FROM TransportExecution AS te
		  LEFT JOIN MasterVendor AS mv
		  ON te.IDVendor = mv.IDVendor
		  WHERE te.IsActive = 1 AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   ) AS jte
	   ON to1.IDTransportExecution = jte.IDTransportExecution
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate))
	    AND to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased) AND to1.SenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ',')) AND to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType) AND to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ','))
	   ORDER BY to1.ShipmentDate DESC, to1.SeqNo ASC, to1.STONo ASC
	 END
	 ELSE IF @executionTypeFilter = 'Without TN' AND @senderFilter = ''
     BEGIN
	   INSERT INTO @result
	   SELECT to1.IDTransportOrder,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),to1.UpdatedBy,to1.UpdatedDate
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN (
		  SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
		  FROM TransportExecution AS te
		  LEFT JOIN MasterVendorSuggestion AS mvs
		  ON te.StartLocation = mvs.StartLocation AND te.TransportMode = mvs.TransportationMode
		  LEFT JOIN MasterVendor AS mv
		  ON mvs.SuggestedVendor = mv.IDVendor
		  WHERE te.IsActive = 1 AND (mvs.IsActive IS NULL OR mvs.IsActive = 1) AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   ) AS jte
	   ON to1.IDTransportExecution = jte.IDTransportExecution
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased) AND to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType) AND to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ','))
	   ORDER BY to1.ShipmentDate DESC, to1.SeqNo ASC, to1.STONo ASC
     END	 
	 ELSE IF @executionTypeFilter = 'Without TN' AND @senderFilter != ''
	 BEGIN
		INSERT INTO @result
	   SELECT to1.IDTransportOrder,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),to1.UpdatedBy,to1.UpdatedDate
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN (
		  SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
		  FROM TransportExecution AS te
		  LEFT JOIN MasterVendorSuggestion AS mvs
		  ON te.StartLocation = mvs.StartLocation AND te.TransportMode = mvs.TransportationMode
		  LEFT JOIN MasterVendor AS mv
		  ON mvs.SuggestedVendor = mv.IDVendor
		  WHERE te.IsActive = 1 AND (mvs.IsActive IS NULL OR mvs.IsActive = 1) AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   ) AS jte
	   ON to1.IDTransportExecution = jte.IDTransportExecution
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND (to1.OrderStatus IS NULL OR to1.OrderStatus != 'Draft') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased) AND to1.SenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ',')) AND to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType) AND to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ','))
	   ORDER BY to1.ShipmentDate DESC, to1.SeqNo ASC, to1.STONo ASC
	 END
     RETURN;
END
