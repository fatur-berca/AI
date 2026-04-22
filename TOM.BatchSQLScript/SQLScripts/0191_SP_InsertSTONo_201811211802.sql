ALTER PROCEDURE [dbo].[InsertSTONo]

AS
BEGIN

DECLARE @idTransportOrderBefore INT,
	   @idTransportOrder INT,
	   @idTransportOrderDetail INT,
	   @shipmentDateBefore DATE,
	   @shipmentDateAfter DATE,
	   @shipmentDateChange BIT,
	   @idLocSenderBefore VARCHAR(50),
	   @idLocSenderAfter VARCHAR(50),
	   @idLocSenderChange BIT,
	   @idLocReceiveBefore VARCHAR(50),
	   @idLocReceiveAfter VARCHAR(50),
	   @idLocReceiveChange BIT,
	   @codeBefore VARCHAR(50),
	   @codeAfter VARCHAR(50),
	   @qtyBefore DECIMAL(18,4),
	   @qtyAfter DECIMAL(18,4),
	   @uomBefore VARCHAR(10),
	   @uomAfter VARCHAR(10),
	   @version INT,
	   @descriptionAfter VARCHAR(50)

SET @idTransportOrderBefore = 0;-- DIPAKAI UNTUK FLAG, KAPAN NGECEK TRANSPORT ORDER 

DECLARE dataUpdateCursor CURSOR FOR 
SELECT c.IDTransportOrder,c.IDTransportOrderDetail,tot.ShipmentDate,c.ShipmentDate,CASE WHEN tot.ShipmentDate <> c.ShipmentDate THEN 1 ELSE 0 END,
	  tot.SenderIDLocation,c.ActualSenderIDLocation,CASE WHEN tot.SenderIDLocation <> c.ActualSenderIDLocation THEN 1 ELSE 0 END,tot.ReceiverIDLocation,c.ActualReceiverIDLocation,CASE WHEN tot.ReceiverIDLocation <> c.ActualReceiverIDLocation THEN 1 ELSE 0 END,
	  todt.Code,c.Code,todt.Qty,c.Qty,mm.MapFrom,c.UoM
FROM TransportOrderTemp AS tot
INNER JOIN TransportOrderDetailTemp AS todt
ON todt.STONo = tot.STONo
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
LEFT JOIN(
	SELECT to1.IDTransportOrder, to1.STONo, to1.ShipmentDate,to1.ActualSenderIDLocation, to1.ActualReceiverIDLocation,tod.IDTransportOrderDetail,tod.Code,tod.Qty,tod.UoM
	FROM TransportOrder AS to1
	INNER JOIN TransportOrderDetail AS tod
	ON tod.IDTransportOrder = to1.IDTransportOrder
	WHERE to1.STONo IS NOT NULL AND to1.IsActive = 1 AND tod.IsActive = 1
) AS c
ON tot.STONo = c.STONo AND todt.Code = c.Code

OPEN dataUpdateCursor
FETCH NEXT FROM dataUpdateCursor INTO @idTransportOrder,@idTransportOrderDetail,@shipmentDateBefore,@shipmentDateAfter,@shipmentDateChange,@idLocSenderBefore,@idLocSenderAfter,@idLocSenderChange,@idLocReceiveBefore,@idLocReceiveAfter,@idLocReceiveChange,@codeBefore,@codeAfter,@qtyBefore,@qtyAfter,@uomBefore,@uomAfter
WHILE @@FETCH_STATUS = 0  
BEGIN
    IF @idTransportOrderBefore = 0 OR @idTransportOrderBefore <> @idTransportOrder
    BEGIN
	   SET @idTransportOrderBefore = @idTransportOrder;
	   SELECT TOP 1 @version = ISNULL(tocl.[Version],0) FROM TransportOrderChangeLog AS tocl WHERE tocl.IDTransportOrder = @idTransportOrder ORDER BY tocl.[Version] DESC
	   SET @version = @version + 1;
	   IF @shipmentDateChange = 1
	   BEGIN
		  UPDATE TransportOrder
		  SET ShipmentDate = @shipmentDateAfter, UpdatedBy = 'system', UpdatedDate = GETDATE()
		  WHERE IDTransportOrder = @idTransportOrder
		  INSERT INTO TransportOrderChangeLog
		  (
		  	IDTransportOrder,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
		  	@idTransportOrder,@version,@shipmentDateBefore,'ShipmentDate','system',GETDATE(),'system',GETDATE()
		  )
	   END
	   IF @idLocSenderChange = 1
	   BEGIN
		  UPDATE TransportOrder
		  SET SenderIDLocation = @idLocSenderAfter, ActualSenderIDLocation = @idLocSenderAfter, UpdatedBy = 'system', UpdatedDate = GETDATE()
		  WHERE IDTransportOrder = @idTransportOrder
		  INSERT INTO TransportOrderChangeLog
		  (
		  	IDTransportOrder,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
		  	@idTransportOrder,@version,@idLocSenderBefore,'SenderIDLocation','system',GETDATE(),'system',GETDATE()
		  )
		  INSERT INTO TransportOrderChangeLog
		  (
		  	IDTransportOrder,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
		  	@idTransportOrder,@version,@idLocSenderBefore,'ActualSenderIDLocation','system',GETDATE(),'system',GETDATE()
		  )
	   END
	   IF @idLocReceiveChange = 1
	   BEGIN
		  UPDATE TransportOrder
		  SET ReceiverIDLocation = @idLocReceiveAfter, ActualReceiverIDLocation = @idLocReceiveAfter, UpdatedBy = 'system', UpdatedDate = GETDATE()
		  WHERE IDTransportOrder = @idTransportOrder
		  INSERT INTO TransportOrderChangeLog
		  (
		  	IDTransportOrder,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
		  	@idTransportOrder,@version,@idLocReceiveBefore,'ReceiverIDLocation','system',GETDATE(),'system',GETDATE()
		  )
		  INSERT INTO TransportOrderChangeLog
		  (
		  	IDTransportOrder,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
		  	@idTransportOrder,@version,@idLocReceiveBefore,'ActualReceiverIDLocation','system',GETDATE(),'system',GETDATE()
		  )
	   END
    END
    
    IF @codeBefore IS NOT NULL
    BEGIN
	   SELECT TOP 1 @version = ISNULL(todcl.[Version],0) FROM TransportOrderDetailChangeLog AS todcl WHERE todcl.IDTransportOrderDetail = @idTransportOrderDetail ORDER BY todcl.[Version] DESC
	   SET @version = @version + 1;
	   IF @uomBefore <> @uomAfter 
	   BEGIN
		  UPDATE TransportOrderDetail
		  SET
		  	UoM = @uomAfter, UpdatedBy = 'system', UpdatedDate = GETDATE()
		  WHERE IDTransportOrderDetail = @idTransportOrderDetail
		  INSERT INTO TransportOrderDetailChangeLog
		  (
		  	IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
		  	@idTransportOrderDetail,@version,@uomBefore,'UoM','system',GETDATE(),'system',GETDATE()
		  )
	   END
	   
	   IF @qtyBefore <> @qtyAfter
	   BEGIN
		  UPDATE TransportOrderDetail
		  SET
		  	Qty = @qtyAfter, UpdatedBy = 'system', UpdatedDate = GETDATE()
		  WHERE IDTransportOrderDetail = @idTransportOrderDetail
		  INSERT INTO TransportOrderDetailChangeLog
		  (
		  	IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
		  	@idTransportOrderDetail,@version,@qtyBefore,'Qty','system',GETDATE(),'system',GETDATE()
		  )
	   END
    END
    ELSE
    BEGIN
	   SELECT @descriptionAfter = mf.LongSpeakingCode FROM MasterFABrand AS mf WHERE mf.FACode = @codeAfter
	   INSERT INTO TransportOrderDetail
	   (
		   IDTransportOrder,Supplier,MaterialType,Code,[Description],Qty,UoM,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
	   )
	   VALUES
	   (
		  @idTransportOrder,'','Cigarette',@codeAfter,@descriptionAfter,@qtyAfter,@uomAfter,1,'system',GETDATE(),'system',GETDATE()
	   )
    END
    
    FETCH NEXT FROM dataUpdateCursor INTO @idTransportOrder,@idTransportOrderDetail,@shipmentDateBefore,@shipmentDateAfter,@shipmentDateChange,@idLocSenderBefore,@idLocSenderAfter,@idLocSenderChange,@idLocReceiveBefore,@idLocReceiveAfter,@idLocReceiveChange,@codeBefore,@codeAfter,@qtyBefore,@qtyAfter,@uomBefore,@uomAfter
END   
CLOSE dataUpdateCursor;
DEALLOCATE dataUpdateCursor;

---START: BAGIAN PASANG STO---
UPDATE to1
SET STONo = a.STONo
FROM TransportOrder AS to1
INNER JOIN TransportOrderDetail AS tod
ON tod.IDTransportOrder = to1.IDTransportOrder
INNER JOIN (
SELECT tot.STONo,tot.ShipmentDate,SUBSTRING(tot.SenderIDLocation,1,4) AS 'SenderIDLocation',SUBSTRING(tot.ReceiverIDLocation,1,4) AS 'ReceiverIDLocation',todt.Code,CAST(todt.Qty AS INT) AS 'Qty', mm.MapFrom
FROM TransportOrderTemp AS tot
INNER JOIN TransportOrderDetailTemp AS todt
ON tot.STONo = todt.STONo
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
) AS a
ON a.ShipmentDate = CAST(to1.ShipmentDate AS DATE) AND a.SenderIDLocation = to1.ActualSenderIDLocation AND a.ReceiverIDLocation = to1.ActualReceiverIDLocation AND a.Code = tod.Code AND a.Qty = CAST(tod.Qty AS INT) AND a.MapFrom = tod.UoM
WHERE to1.STONo IS NULL OR to1.STONo = '' AND to1.IsActive = 1 AND tod.IsActive = 1
---FINISH: BAGIAN PASANG STO---

---START: BAGIAN INSERT BARU DATA SAP KE TABEL TRANSPORT ORDER---
INSERT INTO TransportOrder
(
	IDRequest,
	IDTransportExecution,
	IDTransportPickingListLog,
	STONo,
	SeqNo,
	ShipmentDate,
	KM,
	SenderIDLocation,
	ActualSenderIDLocation,
	ReceiverIDLocation,
	ActualReceiverIDLocation,
	LeadTime,
	VehicleType,
	OrderType,
	CostCenter,
	OrderStatus,
	MaterialReceivedTime,
	IsMaterialReceived,
	EstArrivalDate,
	GRDate,
	LoadStartTime,
	LoadFinishTime,
	UnloadStartTime,
	UnloadFinishTime,
	Remarks,
	FlagEmail,
     IDLeadTime,
	IDCostCenter,
	ChangeLogFields,
	ZoneBased,
	IsActive,
	CreatedBy,
	CreatedDate,
	UpdatedBy,
	UpdatedDate
)
SELECT DISTINCT NULL,
	NULL,
	NULL,
	tot.STONo,
	NULL,
	tot.ShipmentDate,
	NULL,
	SUBSTRING(tot.SenderIDLocation,1,4),
	SUBSTRING(tot.SenderIDLocation,1,4),
	SUBSTRING(tot.ReceiverIDLocation,1,4),
	SUBSTRING(tot.ReceiverIDLocation,1,4),
	NULL,
	NULL,
	'Finished Good',
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	'',
	0,
     NULL,
     NULL,
     '',
	'',
	1,
	'system',
	GETDATE(),
	'system',
	GETDATE()
FROM TransportOrderTemp AS tot
INNER JOIN TransportOrderDetailTemp AS todt
ON tot.STONo = todt.STONo
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
LEFT JOIN (
	SELECT to1.STONo,to1.ShipmentDate
	FROM TransportOrder AS to1
	WHERE to1.STONo IS NOT NULL AND to1.IsActive = 1 
) AS b
ON tot.STONo = b.STONo
WHERE b.ShipmentDate IS NULL-- data di transport order tidak ada, tapi ada di SAP
---FINISH: BAGIAN INSERT BARU DATA SAP KE TABEL TRANSPORT ORDER---

---START: BAGIAN INSERT BARU DATA SAP KE TABEL TRANSPORT ORDER DETAIL---
INSERT INTO TransportOrderDetail
(
	IDTransportOrder,
	Supplier,
	MaterialType,
	Code,
	[Description],
	Qty,
	UoM,
	IsActive,
	CreatedBy,
	CreatedDate,
	UpdatedBy,
	UpdatedDate
)
SELECT to1.IDTransportOrder,
	'',
	'Cigarette',
	todt.Code,
	mf.LongSpeakingCode,
	todt.Qty,
	mm.MapFrom,
	1,
	'system',
	GETDATE(),
	'system',
	GETDATE()
FROM TransportOrder AS to1
INNER JOIN TransportOrderDetail AS tod
ON to1.IDTransportOrder = tod.IDTransportOrder
RIGHT JOIN TransportOrderDetailTemp AS todt
ON todt.Code = tod.Code
INNER JOIN MasterFABrand AS mf
ON todt.Code = mf.FACode
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
WHERE to1.CreatedBy = 'system' AND tod.IDTransportOrderDetail IS NULL AND to1.IsActive = 1
---FINISH: BAGIAN INSERT BARU DATA SAP KE TABEL TRANSPORT ORDER DETAIL---

---START: BAGIAN UPDATE TN YANG MASIH E- DEPANNYA DAN SEMUA STO NO SUDAH TIDAK NULL---
UPDATE te
SET te.TransportNo = SUBSTRING(te.TransportNo,3,LEN(te.TransportNo))
FROM TransportExecution AS te
WHERE SUBSTRING(te.TransportNo,1,1) = 'E' AND (SELECT COUNT(*) FROM TransportOrder AS to1 WHERE to1.STONo IS NOT NULL AND to1.IDTransportExecution = te.IDTransportExecution) > 0
---FINISH: BAGIAN UPDATE TN YANG MASIH E- DEPANNYA DAN SEMUA STO NO SUDAH TIDAK NULL---

END
GO


