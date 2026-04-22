-- Created By : Fadiyah
-- Date : 2018-11-13

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
	IDVendor INT,
	VendorName VARCHAR(50),
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
		@tempConcatIdTo VARCHAR(MAX),
		@orderNumber VARCHAR(50),
		@idSender VARCHAR(50),
		@sender VARCHAR(100),
		@idReceiver VARCHAR(50),
		@receiver VARCHAR(100),
		@shipmentDate DATE,
		@seq VARCHAR(5),
		@idTransportOrderPairing INT,
		@orderNumberPairing VARCHAR(50),
		@shipmentDatePairing DATE,
		@seqPairing VARCHAR(5),
		@seqBefore VARCHAR(5),--digunakan di synergy untuk penanda kapan membuat check box baru
		@dateBefore DATE,--digunakan di synergy untuk penanda kapan membuat check box baru
		@StartDate DATETIME,
		@EndDate DATETIME,
		@synergyRMTPairing BIT--digunakan untuk mengecek, apakah synergy/RMT PP ada pairingnya, kalau tidak ada, tidak muncul
		
	SELECT @StartDate = mgw.StartDate, @EndDate = mgw.EndDate 
	FROM MasterGenWeek AS mgw
	WHERE mgw.[Week] = @weekFilter AND mgw.[Year] = @yearFilter	
	
	SET @tempConcatIdTo = '';
	IF @executionTypeFilter = 'ALL' AND @senderFilter = ''
     BEGIN	   	   
		INSERT INTO @result
		   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck ,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),ISNULL(jte.IDVendor,''),ISNULL(jte.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
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
		   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND to1.SeqNo is not null AND (to1.OrderStatus = 'Submit' OR to1.OrderStatus = 'In Process') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
		   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))
		   ORDER BY to1.ShipmentDate DESC, to1.SeqNo ASC, to1.STONo ASC
     END
	   ELSE IF @executionTypeFilter = 'ALL' AND @senderFilter != ''
	   BEGIN
		  INSERT INTO @result
			 SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,'',t.IDVendor,ISNULL(t.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
		from
		(
		SELECT to1.SeqNo, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor 
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
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'Submit') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.SenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY to1.SeqNo
		) t
		--JOIN TransportExecution jte ON t.IDTransportExecution = jte.IDTransportExecution
		INNER JOIN TransportOrder AS to1 ON t.SeqNo = to1.SeqNo
		JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.SenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ReceiverIDLocation = ml2.IDLocation
		UNION 
		SELECT t.IDTransportExecution, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),t.IDVendor,ISNULL(t.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
		from
		(
		SELECT jte.IDTransportExecution, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor
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
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'In Process') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.SenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY jte.IDTransportExecution
		) t
		JOIN TransportExecution jte ON t.IDTransportExecution = jte.IDTransportExecution
		INNER JOIN TransportOrder AS to1 ON t.IDTransportExecution = to1.IDTransportExecution
		JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.SenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ReceiverIDLocation = ml2.IDLocation


	   END
     ELSE IF @executionTypeFilter = 'Synergy'
     BEGIN
	   SET @idCheck = 0;
	   SET @seqBefore = CAST(-1 AS VARCHAR(1));
	   SET @dateBefore = NULL;
     	
	   DECLARE FGSynergy CURSOR FOR
	   SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrder
	   INNER JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Finished Good' OR tod.MaterialType = 'Finished Good') AND LOWER(to1.Remarks) LIKE '%synergy%' AND (to1.OrderStatus = 'Submit') AND to1.ShipmentDate >= CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1 AND ISNULL(to1.SeqNo,to1.DefaultSeqNo) is not null
	   ORDER BY to1.ShipmentDate ASC, ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.STONo ASC
	   
	   OPEN FGSynergy
	   FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idReceiver,@receiver
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  SET @synergyRMTPairing = 0;
		  DECLARE PairingSynergy CURSOR FOR
		  SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo)
		  FROM TransportOrder AS to1
		  INNER JOIN TransportOrderDetail AS tod
		  ON to1.IDTransportOrder = tod.IDTransportOrder
		  INNER JOIN MasterMapping AS mm
		  ON tod.MaterialType = mm.MapFrom
		  INNER JOIN MasterLocation AS ml
		  ON to1.SenderIDLocation = ml.IDLocation
		  INNER JOIN MasterLocation AS ml2
		  ON to1.ReceiverIDLocation = ml2.IDLocation
		  WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material') AND (to1.OrderStatus = 'Submit') AND to1.ReceiverIDLocation = @idSender AND to1.IsActive = 1 AND tod.IsActive = 1 AND ISNULL(to1.SeqNo,to1.DefaultSeqNo) is not null
		  ORDER BY to1.ShipmentDate ASC, ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.STONo ASC
	   
		  OPEN PairingSynergy
		  FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
		  WHILE @@FETCH_STATUS = 0  
		  BEGIN
		  	SET @synergyRMTPairing = 1;
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
			 	IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2 , 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) OR (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) = 0)
			 	BEGIN
			 	    SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
			 	END
			 	SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
			 	--START: BAGIAN INSERT SYNERGY
			 	INSERT INTO @result
			 	(
			 		IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 	)
			 	SELECT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.CreatedBy,'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate
				FROM TransportOrder AS to1
				INNER JOIN TransportOrderDetail AS tod
				ON to1.IDTransportOrder = tod.IDTransportOrder
				INNER JOIN MasterMapping AS mm
				ON tod.MaterialType = mm.MapFrom
				INNER JOIN MasterLocation AS ml
				ON to1.SenderIDLocation = ml.IDLocation
				INNER JOIN MasterLocation AS ml2
				ON to1.ReceiverIDLocation = ml2.IDLocation
			 	WHERE to1.IDTransportOrder = @idTransportOrder AND (mm.MapTo = 'Finished Good' OR tod.MaterialType = 'Finished Good')
				--END: BAGIAN INSERT SYNERGY
				--START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
				UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
				--END: BAGIAN UPDATE CONCAT ID
				SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
				SET @tempConcatIdTo = '';
			 END
			 IF (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrderPairing)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrderPairing)) OR (LEN(@tempConcatIdTo)-LEN(@idTransportOrderPairing)) = 0)
			 BEGIN
			 	SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrderPairing AS VARCHAR);
			 END
			 
			 --START: BAGIAN INSERT PAIRING
			 INSERT INTO @result
			 (
			 	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 )
			 SELECT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.CreatedBy,'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate
			 FROM TransportOrder AS to1
			 INNER JOIN TransportOrderDetail AS tod
			 ON to1.IDTransportOrder = tod.IDTransportOrder
			 INNER JOIN MasterMapping AS mm
			 ON tod.MaterialType = mm.MapFrom
			 INNER JOIN MasterLocation AS ml
			 ON to1.SenderIDLocation = ml.IDLocation
			 INNER JOIN MasterLocation AS ml2
			 ON to1.ReceiverIDLocation = ml2.IDLocation
			 WHERE to1.IDTransportOrder = @idTransportOrderPairing AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material')
			 --END: BAGIAN INSERT PAIRING
			 FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
		  END
		  CLOSE PairingSynergy;
		  DEALLOCATE PairingSynergy;
		  
		  IF @synergyRMTPairing = 1
		  BEGIN	
		  	 IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) OR (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) = 0)
			 BEGIN
				SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
			 END
			 SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
			 --START: BAGIAN INSERT SYNERGY
			 INSERT INTO @result
			 (
				IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 )
			 SELECT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.CreatedBy,'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate
			 FROM TransportOrder AS to1
			 INNER JOIN TransportOrderDetail AS tod
			 ON to1.IDTransportOrder = tod.IDTransportOrder
			 INNER JOIN MasterMapping AS mm
			 ON tod.MaterialType = mm.MapFrom
			 INNER JOIN MasterLocation AS ml
			 ON to1.SenderIDLocation = ml.IDLocation
			 INNER JOIN MasterLocation AS ml2
			 ON to1.ReceiverIDLocation = ml2.IDLocation
			 WHERE to1.IDTransportOrder = @idTransportOrder AND (mm.MapTo = 'Finished Good' OR tod.MaterialType = 'Finished Good')
			 --END: BAGIAN INSERT SYNERGY
			 --START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
			 UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
			 --END: BAGIAN UPDATE CONCAT ID
			 SET @tempConcatIdTo = '';
			 SET @seqBefore = CAST(-1 AS VARCHAR(1));
			 SET @dateBefore = NULL;
		  END

		  FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idReceiver,@receiver
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
	   SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrder
	   INNER JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material') AND ml.[Type] = 'Factory' AND (to1.OrderStatus = 'Submit') AND to1.ShipmentDate >= CONVERT(DATE, GETDATE()) AND to1.IsActive = 1 AND tod.IsActive = 1 AND to1.SeqNo is not null
	   ORDER BY to1.ShipmentDate ASC,ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.STONo ASC
	   
	   OPEN RMTPP
	   FETCH NEXT FROM RMTPP INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idReceiver,@receiver
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  SET @synergyRMTPairing = 0;
		  DECLARE PairingRMTPP CURSOR FOR
		  SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo)
		  FROM TransportOrder AS to1
		  INNER JOIN TransportOrderDetail AS tod
		  ON to1.IDTransportOrder = tod.IDTransportOrder
		  INNER JOIN MasterMapping AS mm
		  ON tod.MaterialType = mm.MapFrom
		  INNER JOIN MasterLocation AS ml
		  ON to1.SenderIDLocation = ml.IDLocation
		  INNER JOIN MasterLocation AS ml2
		  ON to1.ReceiverIDLocation = ml2.IDLocation
		  WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material') AND (to1.OrderStatus = 'Submit') AND to1.ReceiverIDLocation = @idSender AND to1.SenderIDLocation = @idReceiver AND to1.IsActive = 1 AND tod.IsActive = 1 AND to1.SeqNo is not null
		  ORDER BY to1.ShipmentDate ASC,ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.STONo ASC
	   
		  OPEN PairingRMTPP
		  FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
		  WHILE @@FETCH_STATUS = 0  
		  BEGIN
		  	SET @synergyRMTPairing = 1;
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
			 	IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) OR (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) = 0)
			 	BEGIN
			 	    SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
			 	END
			 	SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
			 	--START: BAGIAN INSERT RMT PP
			 	INSERT INTO @result
			 	(
			 		IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 	)
			 	SELECT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.CreatedBy,'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate
				FROM TransportOrder AS to1
				INNER JOIN TransportOrderDetail AS tod
				ON to1.IDTransportOrder = tod.IDTransportOrder
				INNER JOIN MasterMapping AS mm
				ON tod.MaterialType = mm.MapFrom
				INNER JOIN MasterLocation AS ml
				ON to1.SenderIDLocation = ml.IDLocation
				INNER JOIN MasterLocation AS ml2
				ON to1.ReceiverIDLocation = ml2.IDLocation
			 	WHERE to1.IDTransportOrder = @idTransportOrder AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material')
				--END: BAGIAN INSERT RMT PP
				--START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
				UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
				--END: BAGIAN UPDATE CONCAT ID
				SET @idCheck = @idCheck + 1;
				SET @seqBefore = @seqPairing;
				SET @dateBefore = @shipmentDatePairing;
				SET @tempConcatIdTo = '';
			 END
			 IF (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrderPairing)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND (CHARINDEX('-'+CAST(@idTransportOrderPairing AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrderPairing)) OR (LEN(@tempConcatIdTo)-LEN(@idTransportOrderPairing)) = 0)
			 BEGIN
			 	SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrderPairing AS VARCHAR);
			 END
			 --START: BAGIAN INSERT PAIRING
			 INSERT INTO @result
			 (
			 	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 )
			 SELECT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.CreatedBy,'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate
			 FROM TransportOrder AS to1
			 INNER JOIN TransportOrderDetail AS tod
			 ON to1.IDTransportOrder = tod.IDTransportOrder
			 INNER JOIN MasterMapping AS mm
			 ON tod.MaterialType = mm.MapFrom
			 INNER JOIN MasterLocation AS ml
			 ON to1.SenderIDLocation = ml.IDLocation
			 INNER JOIN MasterLocation AS ml2
			 ON to1.ReceiverIDLocation = ml2.IDLocation
			 WHERE to1.IDTransportOrder = @idTransportOrderPairing AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material')
			 --END: BAGIAN INSERT PAIRING
			 FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
		  END
		  CLOSE PairingRMTPP;
		  DEALLOCATE PairingRMTPP;
		  
		  IF @synergyRMTPairing = 1
		  BEGIN
			 IF (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> 1 OR (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) = 1 AND SUBSTRING(@tempConcatIdTo, LEN(@idTransportOrder)+2, 1) <> '-')) AND CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR)+'-',@tempConcatIdTo) = 0 AND (CHARINDEX('-'+CAST(@idTransportOrder AS VARCHAR),@tempConcatIdTo) <> (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) OR (LEN(@tempConcatIdTo)-LEN(@idTransportOrder)) = 0)
			 BEGIN
				SET @tempConcatIdTo = @tempConcatIdTo + '-' + CAST(@idTransportOrder AS VARCHAR);
			 END
			 SET @tempConcatIdTo = SUBSTRING(@tempConcatIdTo,2,LEN(@tempConcatIdTo))
			 --START: BAGIAN INSERT SYNERGY
			 INSERT INTO @result
			 (
				IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate
			 )
			 SELECT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.SenderIDLocation,ml.LocationName,to1.ReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.CreatedBy,'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate
			 FROM TransportOrder AS to1
			 INNER JOIN TransportOrderDetail AS tod
			 ON to1.IDTransportOrder = tod.IDTransportOrder
			 INNER JOIN MasterMapping AS mm
			 ON tod.MaterialType = mm.MapFrom
			 INNER JOIN MasterLocation AS ml
			 ON to1.SenderIDLocation = ml.IDLocation
			 INNER JOIN MasterLocation AS ml2
			 ON to1.ReceiverIDLocation = ml2.IDLocation
			 WHERE to1.IDTransportOrder = @idTransportOrder AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material')
			 --END: BAGIAN INSERT SYNERGY
			 --START: BAGIAN UPDATE CONCAT ID(update where IDCheck = @idCheck)
			 UPDATE @result SET ConcatIDTransportOrder = @tempConcatIdTo WHERE IDCheck = @idCheck
			 --END: BAGIAN UPDATE CONCAT ID
			 SET @tempConcatIdTo = '';
			 SET @seqBefore = CAST(-1 AS VARCHAR(1));
			 SET @dateBefore = NULL;
		  END

		  FETCH NEXT FROM RMTPP INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idReceiver,@receiver
	   END
	   CLOSE RMTPP;
	   DEALLOCATE RMTPP;
     END
	ELSE IF @executionTypeFilter = 'Next Day'
     BEGIN
	   INSERT INTO @result
	   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC) AS IDCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.CreatedBy,'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate
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
	   WHERE to1.ShipmentDate > CONVERT(DATE, GETDATE())  AND (to1.OrderStatus = 'Submit') AND to1.IsActive = 1 AND tod.IsActive = 1 AND to1.IDTransportExecution IS NULL AND ISNULL(to1.SeqNo,to1.DefaultSeqNo) is not null
	   ORDER BY to1.ShipmentDate ASC, ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC
     END
	 ELSE IF @executionTypeFilter = 'With TN' AND @senderFilter = ''
     BEGIN
	   INSERT INTO @result
	   SELECT to1.IDTransportExecution,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
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
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'In Process') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))
	   --ORDER BY to1.ShipmentDate DESC
     END	 
	 ELSE IF @executionTypeFilter = 'With TN' AND @senderFilter != ''
     BEGIN
		INSERT INTO @result
	   SELECT t.IDTransportExecution, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),t.IDVendor,ISNULL(t.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
		from
		(
		SELECT jte.IDTransportExecution, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor --DENSE_RANK() OVER(ORDER BY to1.IDTransportExecution ASC) AS IDCheck, to1.IDTransportExecution , to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
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
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'In Process') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.SenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY jte.IDTransportExecution
		) t
		JOIN TransportExecution jte ON t.IDTransportExecution = jte.IDTransportExecution
		INNER JOIN TransportOrder AS to1 ON t.IDTransportExecution = to1.IDTransportExecution
		JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.SenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ReceiverIDLocation = ml2.IDLocation
		ORDER BY t.IDVendor ASC
	 END
	 ELSE IF @executionTypeFilter = 'Without TN' AND @senderFilter = ''
     BEGIN
	   INSERT INTO @result
	   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
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
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'Submit') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND 
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))
	   ORDER BY to1.ShipmentDate DESC, to1.SeqNo ASC, to1.STONo ASC
     END	 
	 ELSE IF @executionTypeFilter = 'Without TN' AND @senderFilter != ''
	 BEGIN
	   INSERT INTO @result
	   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,to1.CreatedBy,'',t.IDVendor,ISNULL(t.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate
		from
		(
		SELECT to1.SeqNo, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor 
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
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'Submit') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.SenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY to1.SeqNo
		) t
		--JOIN TransportExecution jte ON t.IDTransportExecution = jte.IDTransportExecution
		INNER JOIN TransportOrder AS to1 ON t.SeqNo = to1.SeqNo
		JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.SenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ReceiverIDLocation = ml2.IDLocation

	 END
     RETURN;
END
GO


