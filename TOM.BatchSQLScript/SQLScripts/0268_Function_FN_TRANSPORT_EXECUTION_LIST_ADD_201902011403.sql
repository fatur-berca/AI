-- Created By : Fadiyah
-- Date : 2018-11-27

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

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
	OrderCreator VARCHAR(500),
	TransportationNumber VARCHAR(50),
	IDVendor INT,
	VendorName VARCHAR(50),
	IDVendorSuggestion INT,
	VendorSuggestion VARCHAR(50),
	UpdatedBy VARCHAR(50),
	UpdatedDate DATETIME,
	TNCreator VARCHAR(500)
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
		@idParentLocationSender VARCHAR(100),--dipakai di bagian RMT PP, karena RMT PP melihat parent locationnya, bukan locationnya
		@idParentLocationReceiver VARCHAR(100),--dipakai di bagian RMT PP, karena RMT PP melihat parent locationnya, bukan locationnya
		@idReceiver VARCHAR(50),
		@receiver VARCHAR(100),
		@shipmentDate DATE,
		@seq VARCHAR(5),
		@idTransportOrderPairing INT,
		@orderNumberPairing VARCHAR(50),
		@shipmentDatePairing DATE,
		@seqPairing VARCHAR(5),
		@seqBefore VARCHAR(5),--digunakan untuk memisahkan/membuat header baru yang akan di pasangkan
		@dateBefore DATE,--digunakan untuk memisahkan/membuat header baru yang akan di pasangkan
		@locationBefore VARCHAR(50),--digunakan untuk memisahkan/membuat header baru yang akan di pasangkan
		@parentLocationBefore VARCHAR(100),--digunakan untuk memisahkan/membuat header baru yang akan di pasangkan (khusus rmt pp)
		@concatIdToBefore VARCHAR(MAX),
		@seqBeforePairing VARCHAR(5),--digunakan untuk memisahkan/membuat pairing baru yang akan di pasangkan
		@dateBeforePairing DATE,--digunakan untuk memisahkan/membuat pairing baru yang akan di pasangkan
		@concatIdToBeforePairing VARCHAR(MAX),
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
		   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck ,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,ISNULL(mu.FullName,to1.CreatedBy),ISNULL(jte.TransportNo,''),ISNULL(jte.IDVendor,''),ISNULL(jte.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate,''
		   FROM TransportOrderDetail AS tod
		   INNER JOIN TransportOrder AS to1
		   ON tod.IDTransportOrder = to1.IDTransportOrder
		   INNER JOIN MasterLocation AS ml
		   ON to1.ActualSenderIDLocation = ml.IDLocation
		   INNER JOIN MasterLocation AS ml2
		   ON to1.ActualReceiverIDLocation = ml2.IDLocation
		   LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
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
		SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,ISNULL(mu.FullName,to1.CreatedBy),'',t.IDVendor,ISNULL(t.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate,''
		from
		(
		SELECT to1.SeqNo, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor 
		FROM TransportOrderDetail AS tod
		INNER JOIN TransportOrder AS to1
		ON tod.IDTransportOrder = to1.IDTransportOrder		
		LEFT JOIN (
			SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
			FROM TransportExecution AS te
			LEFT JOIN MasterVendor AS mv
			ON te.IDVendor = mv.IDVendor
			WHERE te.IsActive = 1 AND (mv.IsActive IS NULL OR mv.IsActive = 1)
		) AS jte
		ON to1.IDTransportExecution = jte.IDTransportExecution
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'Submit') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.ActualSenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY to1.SeqNo
		) t
		INNER JOIN TransportOrder AS to1 ON t.SeqNo = to1.SeqNo
		JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.ActualSenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ActualReceiverIDLocation = ml2.IDLocation
		LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
		UNION 
		SELECT t.IDTransportExecution, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,ISNULL(mu.FullName,to1.CreatedBy),ISNULL(jte.TransportNo,''),t.IDVendor,ISNULL(t.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate,''
		from
		(
		SELECT jte.IDTransportExecution, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor
		FROM TransportOrderDetail AS tod
		INNER JOIN TransportOrder AS to1
		ON tod.IDTransportOrder = to1.IDTransportOrder		
		LEFT JOIN (
			SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
			FROM TransportExecution AS te
			LEFT JOIN MasterVendor AS mv
			ON te.IDVendor = mv.IDVendor
			WHERE te.IsActive = 1 AND (mv.IsActive IS NULL OR mv.IsActive = 1)
		) AS jte
		ON to1.IDTransportExecution = jte.IDTransportExecution
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'In Process') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.ActualSenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY jte.IDTransportExecution
		) t
		JOIN TransportExecution jte ON t.IDTransportExecution = jte.IDTransportExecution
		INNER JOIN TransportOrder AS to1 ON t.IDTransportExecution = to1.IDTransportExecution
		JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.ActualSenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ActualReceiverIDLocation = ml2.IDLocation
		LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser

	   END
     ELSE IF @executionTypeFilter = 'Synergy'
     BEGIN
	   SET @idCheck = 0;
	   SET @tempConcatIdTo = '';
	   SET @dateBefore = NULL;
	   SET @seqBefore = CAST(-1 AS VARCHAR(1));
	   SET @locationBefore = '-1';
     	
	   DECLARE FGSynergy CURSOR FOR
	   SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.ActualSenderIDLocation,ml.LocationName,to1.ActualReceiverIDLocation,ml2.LocationName
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrder
	   LEFT JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.ActualSenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ActualReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Finished Good' OR tod.MaterialType = 'Finished Good') AND LOWER(to1.Remarks) LIKE '%synergy%' AND (to1.OrderStatus = 'Submit') AND to1.IsActive = 1 AND tod.IsActive = 1 AND ISNULL(to1.SeqNo,to1.DefaultSeqNo) is not null AND to1.ShipmentDate >= CAST(GETDATE() AS DATE)
	   ORDER BY to1.ShipmentDate ASC, ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.ActualSenderIDLocation ASC
	   
	   OPEN FGSynergy
	   FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idReceiver,@receiver
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  --ADA IF INI, SUPAYA KALAU DALAM 1 SHIPMENT DATE & SEQ YANG SAMA, TIDAK ADA DUPLIKAT HEADER KECUALI KALAU DALAM 1 SHIPMENT DATE DAN SEQ YANG SAMA ADA SENDER LOCATION YANG BERBEDA
		  IF @dateBefore <> @shipmentDate OR @seqBefore <> @seq OR @locationBefore <> @idSender
		  BEGIN
			 SET @dateBefore = @shipmentDate;
			 SET @seqBefore = @seq;
			 SET @locationBefore = @idSender;
			 
			 --START: BAGIAN MENGSELECT SEMUA ID TRANSPORT ORDER HEADER DAN DI PISAHKAN KOMA, UNTUK MEMUDAHKAN ASSIGN ID TRANSPORT EXECUTION PADA SAAT GENERATE TN
			 SELECT @concatIdToBefore = STUFF((
				SELECT CAST(to1.IDTransportOrder AS VARCHAR) + ','
				FROM TransportOrder AS to1
				WHERE to1.ShipmentDate = @shipmentDate AND to1.SeqNo = @seq
				ORDER BY to1.IDTransportOrder ASC
				FOR XML PATH ('')), 1, 1, '')
			 --END: BAGIAN MENGSELECT SEMUA ID TRANSPORT ORDER HEADER DAN DI PISAHKAN KOMA, UNTUK MEMUDAHKAN ASSIGN ID TRANSPORT EXECUTION PADA SAAT GENERATE TN
			 
			 SET @synergyRMTPairing = 0;
			 SET @dateBeforePairing = NULL;
			 SET @seqBeforePairing = CAST(-1 AS VARCHAR(1));
			 	  
			 DECLARE PairingSynergy CURSOR FOR
			 SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo)
			 FROM TransportOrder AS to1
			 INNER JOIN TransportOrderDetail AS tod
			 ON to1.IDTransportOrder = tod.IDTransportOrder
			 LEFT JOIN MasterMapping AS mm
			 ON tod.MaterialType = mm.MapFrom
			 INNER JOIN MasterLocation AS ml
			 ON to1.ActualSenderIDLocation = ml.IDLocation
			 INNER JOIN MasterLocation AS ml2
			 ON to1.ActualReceiverIDLocation = ml2.IDLocation 
			 WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material') AND (to1.OrderStatus = 'Submit') AND to1.ActualReceiverIDLocation = @idSender AND to1.IsActive = 1 AND tod.IsActive = 1 AND ISNULL(to1.SeqNo,to1.DefaultSeqNo) is not null AND to1.ShipmentDate >= CAST(GETDATE() AS DATE) AND (to1.ShipmentDate = @shipmentDate OR to1.ShipmentDate = DATEADD(DAY, -1, @shipmentDate))
			 ORDER BY to1.ShipmentDate ASC, ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.STONo ASC
	   
			 OPEN PairingSynergy
			 FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
			 WHILE @@FETCH_STATUS = 0  
			 BEGIN
		  		SET @synergyRMTPairing = 1;
		  	    
		  		IF @dateBeforePairing <> @shipmentDatePairing OR @seqBeforePairing <> @seqPairing
		  		BEGIN
		  		    SET @dateBeforePairing = @shipmentDatePairing;
		  		    SET @seqBeforePairing = @seqPairing;
		  		    
		  		    --START: BAGIAN INSERT PAIRING
		  		    INSERT INTO @result
				    (
				    	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate,TNCreator
				    )
				    SELECT DISTINCT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.ActualSenderIDLocation,ml.LocationName,to1.ActualReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),ISNULL(mu.FullName,to1.CreatedBy),'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate,''
				    FROM TransportOrder AS to1
				    INNER JOIN TransportOrderDetail AS tod
				    ON to1.IDTransportOrder = tod.IDTransportOrder
				    LEFT JOIN MasterMapping AS mm
				    ON tod.MaterialType = mm.MapFrom
				    INNER JOIN MasterLocation AS ml
				    ON to1.ActualSenderIDLocation = ml.IDLocation
				    INNER JOIN MasterLocation AS ml2
				    ON to1.ActualReceiverIDLocation = ml2.IDLocation
				    LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
				    WHERE to1.ShipmentDate = @shipmentDatePairing AND to1.SeqNo = @seqPairing
		  		    --END  : BAGIAN INSERT PAIRING
		  		    
		  		    --START: BAGIAN INSERT HEADER
		  		    INSERT INTO @result
				    (
					   IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate,TNCreator
				    )
				    SELECT DISTINCT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.ActualSenderIDLocation,ml.LocationName,to1.ActualReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),ISNULL(mu.FullName,to1.CreatedBy),'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate,''
				    FROM TransportOrder AS to1
				    INNER JOIN TransportOrderDetail AS tod
				    ON to1.IDTransportOrder = tod.IDTransportOrder
				    LEFT JOIN MasterMapping AS mm
				    ON tod.MaterialType = mm.MapFrom
				    INNER JOIN MasterLocation AS ml
				    ON to1.ActualSenderIDLocation = ml.IDLocation
				    INNER JOIN MasterLocation AS ml2
				    ON to1.ActualReceiverIDLocation = ml2.IDLocation
				    LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
				    WHERE to1.ShipmentDate = @shipmentDate AND to1.SeqNo = @seq
		  		    --END  : BAGIAN INSERT HEDAER
		  		    
		  		    --START: BAGIAN UPDATE CONCATIDTO
		  		    SELECT @concatIdToBeforePairing = STUFF((
						  SELECT CAST(to1.IDTransportOrder AS VARCHAR) + ','
						  FROM TransportOrder AS to1
						  WHERE to1.ShipmentDate = @shipmentDatePairing AND to1.SeqNo = @seqPairing
						  ORDER BY to1.IDTransportOrder ASC
						  FOR XML PATH ('')), 1, 1, '')
				    SET @tempConcatIdTo = @concatIdToBefore + @concatIdToBeforePairing;
		  		    UPDATE @result SET ConcatIDTransportOrder = SUBSTRING(@tempConcatIdTo,1,LEN(@tempConcatIdTo)) WHERE IDCheck = @idCheck
		  		    --END  : BAGIAN UPDATE CONCATIDTO
		  		    SET @idCheck = @idCheck + 1;
		  		END
		  	    
		  		FETCH NEXT FROM PairingSynergy INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
			 END
			 CLOSE PairingSynergy;
			 DEALLOCATE PairingSynergy;
		  END
		  
		  FETCH NEXT FROM FGSynergy INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idReceiver,@receiver
	   END
	   CLOSE FGSynergy;
	   DEALLOCATE FGSynergy;
     END
     ELSE IF @executionTypeFilter = 'RMT PP'
     BEGIN		
	   SET @idCheck = 0;
	   SET @tempConcatIdTo = '';
	   SET @dateBefore = NULL;
	   SET @seqBefore = CAST(-1 AS VARCHAR(1));
	   SET @locationBefore = '-1';
	   SET @parentLocationBefore = '-1';
     	
	   DECLARE RMTPP CURSOR FOR
	   SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo),to1.ActualSenderIDLocation,ml.LocationName,ml.ParentLocation,to1.ActualReceiverIDLocation,ml2.LocationName,ml2.ParentLocation
	   FROM TransportOrder AS to1
	   INNER JOIN TransportOrderDetail AS tod
	   ON to1.IDTransportOrder = tod.IDTransportOrder
	   LEFT JOIN MasterMapping AS mm
	   ON tod.MaterialType = mm.MapFrom
	   INNER JOIN MasterLocation AS ml
	   ON to1.ActualSenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ActualReceiverIDLocation = ml2.IDLocation
	   WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material') AND ml2.[Type] = 'Factory' AND (to1.OrderStatus = 'Submit') AND to1.IsActive = 1 AND tod.IsActive = 1 AND to1.SeqNo is not null AND to1.ShipmentDate >= CAST(GETDATE() AS DATE)
	   ORDER BY to1.ShipmentDate ASC,ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.STONo ASC
	   
	   OPEN RMTPP
	   FETCH NEXT FROM RMTPP INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idParentLocationSender,@idReceiver,@receiver,@idParentLocationReceiver
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN
		  --ADA IF INI, SUPAYA KALAU DALAM 1 SHIPMENT DATE & SEQ YANG SAMA, TIDAK ADA DUPLIKAT HEADER KECUALI KALAU DALAM 1 SHIPMENT DATE DAN SEQ YANG SAMA ADA SENDER LOCATION YANG BERBEDA
		  IF @dateBefore <> @shipmentDate OR @seqBefore <> @seq OR @locationBefore <> @idSender OR @parentLocationBefore <> @idParentLocationReceiver
		  BEGIN
			 SET @dateBefore = @shipmentDate;
			 SET @seqBefore = @seq;
			 SET @locationBefore = @idSender;
			 SET @parentLocationBefore = @idParentLocationReceiver;
			 
			 --START: BAGIAN MENGSELECT SEMUA ID TRANSPORT ORDER HEADER DAN DI PISAHKAN KOMA, UNTUK MEMUDAHKAN ASSIGN ID TRANSPORT EXECUTION PADA SAAT GENERATE TN
			 SELECT @concatIdToBefore = STUFF((
				SELECT CAST(to1.IDTransportOrder AS VARCHAR) + ','
				FROM TransportOrder AS to1
				WHERE to1.ShipmentDate = @shipmentDate AND to1.SeqNo = @seq
				ORDER BY to1.IDTransportOrder ASC
				FOR XML PATH ('')), 1, 1, '')
			 --END: BAGIAN MENGSELECT SEMUA ID TRANSPORT ORDER HEADER DAN DI PISAHKAN KOMA, UNTUK MEMUDAHKAN ASSIGN ID TRANSPORT EXECUTION PADA SAAT GENERATE TN
			 
			 SET @synergyRMTPairing = 0;
			 SET @dateBeforePairing = NULL;
			 SET @seqBeforePairing = CAST(-1 AS VARCHAR(1));
			 
			 DECLARE PairingRMTPP CURSOR FOR
			 SELECT DISTINCT to1.IDTransportOrder,to1.STONo,to1.ShipmentDate,ISNULL(to1.SeqNo,to1.DefaultSeqNo)
			 FROM TransportOrder AS to1
			 INNER JOIN TransportOrderDetail AS tod
			 ON to1.IDTransportOrder = tod.IDTransportOrder
			 LEFT JOIN MasterMapping AS mm
			 ON tod.MaterialType = mm.MapFrom
			 INNER JOIN MasterLocation AS ml
			 ON to1.ActualSenderIDLocation = ml.IDLocation
			 INNER JOIN MasterLocation AS ml2
			 ON to1.ActualReceiverIDLocation = ml2.IDLocation
			 WHERE to1.IDTransportExecution IS NULL AND (mm.MapTo = 'Raw Material' OR tod.MaterialType = 'Raw Material') AND (to1.OrderStatus = 'Submit') AND ml2.IDLocation = @idSender AND ml.ParentLocation = @idParentLocationReceiver  AND to1.IsActive = 1 AND tod.IsActive = 1 AND to1.SeqNo is NOT NULL AND to1.IDTransportOrder <> @idTransportOrder AND to1.ShipmentDate >= CAST(GETDATE() AS DATE) AND (to1.ShipmentDate = @shipmentDate OR to1.ShipmentDate = DATEADD(DAY, -1, @shipmentDate))
			 ORDER BY to1.ShipmentDate ASC,ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC, to1.STONo ASC
	   
			 OPEN PairingRMTPP
			 FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
			 WHILE @@FETCH_STATUS = 0  
			 BEGIN
			 	SET @synergyRMTPairing = 1;
		  	    
		  		IF @dateBeforePairing <> @shipmentDatePairing OR @seqBeforePairing <> @seqPairing
		  		BEGIN
		  		    SET @dateBeforePairing = @shipmentDatePairing;
		  		    SET @seqBeforePairing = @seqPairing;
		  		    
		  		    --START: BAGIAN INSERT PAIRING
		  		    INSERT INTO @result
				    (
				    	IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate,TNCreator
				    )
				    SELECT DISTINCT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.ActualSenderIDLocation,ml.LocationName,to1.ActualReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),ISNULL(mu.FullName,to1.CreatedBy),'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate,''
				    FROM TransportOrder AS to1
				    INNER JOIN TransportOrderDetail AS tod
				    ON to1.IDTransportOrder = tod.IDTransportOrder
				    LEFT JOIN MasterMapping AS mm
				    ON tod.MaterialType = mm.MapFrom
				    INNER JOIN MasterLocation AS ml
				    ON to1.ActualSenderIDLocation = ml.IDLocation
				    INNER JOIN MasterLocation AS ml2
				    ON to1.ActualReceiverIDLocation = ml2.IDLocation
				    LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
				    WHERE to1.ShipmentDate = @shipmentDatePairing AND to1.SeqNo = @seqPairing
		  		    --END  : BAGIAN INSERT PAIRING
		  		    
		  		    --START: BAGIAN INSERT HEADER
		  		    INSERT INTO @result
				    (
					   IDCheck,IDTransportOrder,IDTransportOrderDetail,ConcatIDTransportOrder,OrderNumber,OrderType,CostCenter,IDSenderLoc,Sender,IDReceiverLoc,Receiver,OrderedVehicleType,ShipmentDate,MaterialType,[Description],Qty,UoM,OrderRemarks,Sequence,OrderCreator,TransportationNumber,IDVendor,VendorName,IDVendorSuggestion,VendorSuggestion,UpdatedBy,UpdatedDate,TNCreator
				    )
				    SELECT DISTINCT @idCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,to1.CostCenter,to1.ActualSenderIDLocation,ml.LocationName,to1.ActualReceiverIDLocation,ml2.LocationName,to1.VehicleType,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),ISNULL(mu.FullName,to1.CreatedBy),'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate,''
				    FROM TransportOrder AS to1
				    INNER JOIN TransportOrderDetail AS tod
				    ON to1.IDTransportOrder = tod.IDTransportOrder
				    LEFT JOIN MasterMapping AS mm
				    ON tod.MaterialType = mm.MapFrom
				    INNER JOIN MasterLocation AS ml
				    ON to1.ActualSenderIDLocation = ml.IDLocation
				    INNER JOIN MasterLocation AS ml2
				    ON to1.ActualReceiverIDLocation = ml2.IDLocation
				    LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
				    WHERE to1.ShipmentDate = @shipmentDate AND to1.SeqNo = @seq
		  		    --END  : BAGIAN INSERT HEDAER
		  		    
		  		    --START: BAGIAN UPDATE CONCATIDTO
		  		    SELECT @concatIdToBeforePairing = STUFF((
						  SELECT CAST(to1.IDTransportOrder AS VARCHAR) + ','
						  FROM TransportOrder AS to1
						  WHERE to1.ShipmentDate = @shipmentDatePairing AND to1.SeqNo = @seqPairing
						  ORDER BY to1.IDTransportOrder ASC
						  FOR XML PATH ('')), 1, 1, '')
				    SET @tempConcatIdTo = @concatIdToBefore + @concatIdToBeforePairing;
		  		    UPDATE @result SET ConcatIDTransportOrder = SUBSTRING(@tempConcatIdTo,1,LEN(@tempConcatIdTo)) WHERE IDCheck = @idCheck
		  		    --END  : BAGIAN UPDATE CONCATIDTO
		  		    SET @idCheck = @idCheck + 1;
		  		END

			 	FETCH NEXT FROM PairingRMTPP INTO @idTransportOrderPairing,@orderNumberPairing,@shipmentDatePairing,@seqPairing
			 END
			 CLOSE PairingRMTPP;
			 DEALLOCATE PairingRMTPP;
		  END
	   	
		  FETCH NEXT FROM RMTPP INTO @idTransportOrder,@orderNumber,@shipmentDate,@seq,@idSender,@sender,@idParentLocationSender,@idReceiver,@receiver,@idParentLocationReceiver
	   END
	   CLOSE RMTPP;
	   DEALLOCATE RMTPP;
     END
	ELSE IF @executionTypeFilter = 'Next Day'
     BEGIN
	   INSERT INTO @result
	   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, ISNULL(to1.SeqNo,to1.DefaultSeqNo) ASC) AS IDCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,ISNULL(to1.SeqNo,to1.DefaultSeqNo),ISNULL(mu.FullName,to1.CreatedBy),'',NULL,'',NULL,'',to1.UpdatedBy,to1.UpdatedDate,''
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.ActualSenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ActualReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
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
	   SELECT to1.IDTransportExecution,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,ISNULL(mu.FullName,to1.CreatedBy),ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),ISNULL(mvste.IDVendor,''),ISNULL(mvste.VendorName,''),to1.UpdatedBy,to1.UpdatedDate, ISNULL(mu2.FullName,jte.CreatedBy)
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.ActualSenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ActualReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
	   LEFT JOIN (
		  SELECT te.IDTransportExecution,te.TransportNo,te.CreatedBy, mv.IDVendor, mv.VendorName
		  FROM TransportExecution AS te
		  LEFT JOIN MasterVendor AS mv
		  ON te.IDVendor = mv.IDVendor
		  WHERE te.IsActive = 1 AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   ) AS jte
	   ON to1.IDTransportExecution = jte.IDTransportExecution
	   LEFT JOIN (
		  SELECT te.IDTransportExecution, mv.IDVendor, mv.VendorName
		  FROM TransportExecution AS te
		  INNER JOIN (
			 select *
			   from
			   (
				   select IDTransportExecution, ActualReceiverIDLocation as ReceiverIDLocation, OrderType,
				   ROW_NUMBER() OVER(PARTITION BY IDTransportExecution ORDER BY  StoNo ASC) as rowno
				   from transportorder 
					where IDTransportExecution is not null
				) t where t.rowno = 1 
		  ) AS to1
	      ON to1.IDTransportExecution = te.IDTransportExecution
		  INNER JOIN MasterVendorSuggestion AS mvs
		  ON te.StartLocation = mvs.StartLocation AND to1.ReceiverIDLocation = mvs.ReceiverIDLocation AND to1.OrderType = mvs.OrderType AND te.TransportCategory = mvs.TransportationCategory AND te.TransportMode = mvs.TransportationMode AND te.ActualVehicleType = mvs.VehicleType
		  INNER JOIN MasterVendor AS mv
		  ON mvs.SuggestedVendor = mv.IDVendor
		  WHERE te.IsActive = 1 AND (mvs.IsActive IS NULL OR mvs.IsActive = 1) AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   ) AS mvste
	   ON to1.IDTransportExecution = mvste.IDTransportExecution	   
	   LEFT JOIN MasterUser AS mu2 ON jte.CreatedBy = mu2.IDUser
	   WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND (to1.OrderStatus = 'In Process') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))
	   --ORDER BY to1.ShipmentDate DESC
     END	 
	 ELSE IF @executionTypeFilter = 'With TN' AND @senderFilter != ''
     BEGIN
		INSERT INTO @result
	   SELECT t.IDTransportExecution, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,ISNULL(mu.FullName,to1.CreatedBy),ISNULL(jte.TransportNo,''),t.IDVendor,ISNULL(t.VendorName,''),ISNULL(mvste.IDVendor,''),ISNULL(mvste.VendorName,''),to1.UpdatedBy,to1.UpdatedDate,ISNULL(mu2.FullName,jte.CreatedBy)
		from
		(
		SELECT jte.IDTransportExecution, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor
		FROM TransportOrderDetail AS tod
		INNER JOIN TransportOrder AS to1
		ON tod.IDTransportOrder = to1.IDTransportOrder
		LEFT JOIN (
			SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
			FROM TransportExecution AS te
			LEFT JOIN MasterVendor AS mv
			ON te.IDVendor = mv.IDVendor
			WHERE te.IsActive = 1 AND (mv.IsActive IS NULL OR mv.IsActive = 1)
		) AS jte
		ON to1.IDTransportExecution = jte.IDTransportExecution
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NOT NULL AND (to1.OrderStatus = 'In Process') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.ActualSenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY jte.IDTransportExecution
		) t
		INNER JOIN TransportExecution jte ON t.IDTransportExecution = jte.IDTransportExecution
		INNER JOIN TransportOrder AS to1 ON t.IDTransportExecution = to1.IDTransportExecution
		INNER JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.ActualSenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ActualReceiverIDLocation = ml2.IDLocation
		LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
		LEFT JOIN MasterUser AS mu2 ON jte.CreatedBy = mu2.IDUser
		LEFT JOIN (
		  SELECT te.IDTransportExecution, mv.IDVendor, mv.VendorName
		  FROM TransportExecution AS te
		  INNER JOIN (
			 select *
			   from
			   (
				   select IDTransportExecution, ActualReceiverIDLocation as ReceiverIDLocation, OrderType,
				   ROW_NUMBER() OVER(PARTITION BY IDTransportExecution ORDER BY  StoNo ASC) as rowno
				   from transportorder 
					where IDTransportExecution is not null
				) t where t.rowno = 1 
		  ) AS to1
	      ON to1.IDTransportExecution = te.IDTransportExecution
		  INNER JOIN MasterVendorSuggestion AS mvs
		  ON te.StartLocation = mvs.StartLocation AND to1.ReceiverIDLocation = mvs.ReceiverIDLocation AND to1.OrderType = mvs.OrderType AND te.TransportCategory = mvs.TransportationCategory AND te.TransportMode = mvs.TransportationMode AND te.ActualVehicleType = mvs.VehicleType
		  INNER JOIN MasterVendor AS mv
		  ON mvs.SuggestedVendor = mv.IDVendor
		  WHERE te.IsActive = 1 AND (mvs.IsActive IS NULL OR mvs.IsActive = 1) AND (mv.IsActive IS NULL OR mv.IsActive = 1)
	   ) AS mvste
	   ON to1.IDTransportExecution = mvste.IDTransportExecution
		ORDER BY t.IDVendor ASC
	 END
	 ELSE IF @executionTypeFilter = 'Without TN' AND @senderFilter = ''
     BEGIN
	   INSERT INTO @result
	   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck,to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,ISNULL(mu.FullName,to1.CreatedBy),ISNULL(jte.TransportNo,''),jte.IDVendor,ISNULL(jte.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate,''
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.ActualSenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ActualReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
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
	   SELECT DENSE_RANK() OVER(ORDER BY to1.ShipmentDate ASC, to1.SeqNo ASC) AS IDCheck, to1.IDTransportOrder,tod.IDTransportOrderDetail,'',to1.STONo,to1.OrderType,ml.IDLocation,ml.LocationName,ml2.IDLocation,ml2.LocationName,to1.VehicleType,to1.CostCenter,to1.ShipmentDate,tod.MaterialType,tod.[Description],tod.Qty,tod.UoM,to1.Remarks,to1.SeqNo,ISNULL(mu.FullName,to1.CreatedBy),'',t.IDVendor,ISNULL(t.VendorName,''),'','',to1.UpdatedBy,to1.UpdatedDate,''
		from
		(
		SELECT to1.SeqNo, MAX(VendorName) as VendorName, MAX(IDVendor) as IDVendor 
		FROM TransportOrderDetail AS tod
		INNER JOIN TransportOrder AS to1
		ON tod.IDTransportOrder = to1.IDTransportOrder		
		LEFT JOIN (
			SELECT te.IDTransportExecution,te.TransportNo,mv.IDVendor, mv.VendorName
			FROM TransportExecution AS te
			LEFT JOIN MasterVendor AS mv
			ON te.IDVendor = mv.IDVendor
			WHERE te.IsActive = 1 AND (mv.IsActive IS NULL OR mv.IsActive = 1)
		) AS jte
		ON to1.IDTransportExecution = jte.IDTransportExecution
		WHERE to1.IsActive = 1 AND tod.IsActive = 1 AND jte.TransportNo IS NULL AND to1.SeqNo is not null AND (to1.OrderStatus = 'Submit') AND (to1.ShipmentDate >= CONVERT(DATE, @StartDate) AND to1.ShipmentDate <= CONVERT(DATE, @EndDate)) AND
	   (@zoneFilter = 'ALL' OR to1.ZoneBased = COALESCE(@zoneFilter,to1.ZoneBased)) AND (@senderFilter = 'ALL' OR to1.ActualSenderIDLocation IN (SELECT Value FROM FN_SPLIT_STRING(@senderFilter, ','))) AND (@vehicleTypeFilter ='ALL' OR to1.VehicleType = COALESCE(@vehicleTypeFilter,to1.VehicleType)) AND (@orderTypeFilter ='ALL' OR to1.OrderType IN (SELECT Value FROM FN_SPLIT_STRING(@orderTypeFilter, ',')))	   
		GROUP BY to1.SeqNo
		) t		
		INNER JOIN TransportOrder AS to1 ON t.SeqNo = to1.SeqNo
		JOIN TransportOrderDetail AS tod ON tod.IDTransportOrder = to1.IDTransportOrder
		INNER JOIN MasterLocation AS ml ON to1.ActualSenderIDLocation = ml.IDLocation
		INNER JOIN MasterLocation AS ml2 ON to1.ActualReceiverIDLocation = ml2.IDLocation
		LEFT JOIN MasterUser AS mu ON to1.CreatedBy = mu.IDUser
	 END
     RETURN;
END
GO
