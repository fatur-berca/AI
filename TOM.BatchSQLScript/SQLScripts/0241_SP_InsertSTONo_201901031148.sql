SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


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
        @descriptionAfter VARCHAR(50),
        @stoNoUpdateDetail VARCHAR(20),
        @idTransportOrderUpdateDetail INT,
        @supplierTransportOrderUpdateDetail VARCHAR(100)

SET @idTransportOrderBefore = 0;-- DIPAKAI UNTUK FLAG, KAPAN NGECEK TRANSPORT ORDER

DECLARE dataUpdateCursor CURSOR FOR
SELECT tot.STONo,c.IDTransportOrder,c.IDTransportOrderDetail,tot.ShipmentDate,c.ShipmentDate,CASE WHEN tot.ShipmentDate <> c.ShipmentDate THEN 1 ELSE 0 END,
         SUBSTRING(tot.SenderIDLocation,1,4),c.SenderIDLocation,CASE WHEN SUBSTRING(tot.SenderIDLocation,1,4) <> c.SenderIDLocation THEN 1 ELSE 0 END,SUBSTRING(tot.ReceiverIDLocation,1,4),c.ReceiverIDLocation,CASE WHEN SUBSTRING(tot.ReceiverIDLocation,1,4) <> c.ReceiverIDLocation THEN 1 ELSE 0 END,
         todt.Code,c.Code,todt.Qty,c.Qty,mm.MapFrom,c.UoM
FROM TransportOrderTemp AS tot
INNER JOIN TransportOrderDetailTemp AS todt
ON todt.STONo = tot.STONo
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
LEFT JOIN(
    SELECT to1.IDTransportOrder, to1.STONo, to1.ShipmentDate,to1.SenderIDLocation, to1.ReceiverIDLocation,tod.IDTransportOrderDetail,tod.Code,tod.Qty,tod.UoM
    FROM TransportOrder AS to1
    INNER JOIN TransportOrderDetail AS tod
    ON tod.IDTransportOrder = to1.IDTransportOrder
    WHERE to1.STONo IS NOT NULL AND to1.IsActive = 1 AND tod.IsActive = 1
) AS c
ON tot.STONo = c.STONo AND todt.Code = c.Code AND mm.MapFrom = c.UoM
OPEN dataUpdateCursor

FETCH NEXT FROM dataUpdateCursor INTO @stoNoUpdateDetail,@idTransportOrder,@idTransportOrderDetail,@shipmentDateAfter,@shipmentDateBefore,@shipmentDateChange,@idLocSenderAfter,@idLocSenderBefore,@idLocSenderChange,@idLocReceiveAfter,@idLocReceiveBefore,@idLocReceiveChange,@codeAfter,@codeBefore,@qtyAfter,@qtyBefore,@uomAfter,@uomBefore
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
            SET SenderIDLocation = @idLocSenderAfter, UpdatedBy = 'system', UpdatedDate = GETDATE()
            WHERE IDTransportOrder = @idTransportOrder

            INSERT INTO TransportOrderChangeLog
            (
			 IDTransportOrder,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
            )
            VALUES
            (
			 @idTransportOrder,@version,@idLocSenderBefore,'SenderIDLocation','system',GETDATE(),'system',GETDATE()
            )
        END

        IF @idLocReceiveChange = 1
        BEGIN
		  UPDATE TransportOrder
		  SET ReceiverIDLocation = @idLocReceiveAfter, UpdatedBy = 'system', UpdatedDate = GETDATE()
		  WHERE IDTransportOrder = @idTransportOrder
                
		  INSERT INTO TransportOrderChangeLog
		  (
			 IDTransportOrder,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )
		  VALUES
		  (
			 @idTransportOrder,@version,@idLocReceiveBefore,'ReceiverIDLocation','system',GETDATE(),'system',GETDATE()
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
    	SELECT @idTransportOrderUpdateDetail = IDTransportOrder FROM TransportOrder WHERE STONo = @stoNoUpdateDetail
	   IF @idTransportOrderUpdateDetail IS NOT NULL
	   BEGIN
		  SELECT @descriptionAfter = mf.LongSpeakingCode,@supplierTransportOrderUpdateDetail = mf.Supplier
		  FROM MasterFABrand AS mf WHERE mf.FACode = @codeAfter
		   
		  INSERT INTO TransportOrderDetail
		  (
			 IDTransportOrder,Supplier,MaterialType,Code,[Description],Qty,UoM,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
		  )

		  VALUES
		  (
			 @idTransportOrderUpdateDetail,ISNULL(@supplierTransportOrderUpdateDetail,''),'Cigarette',@codeAfter,ISNULL(@descriptionAfter,''),@qtyAfter,@uomAfter,1,'system',GETDATE(),'system',GETDATE()
		  )
		  
		  SELECT TOP 1 @idTransportOrderDetail = IDTransportOrderDetail
		  FROM TransportOrderDetail
		  WHERE IDTransportOrder = @idTransportOrderUpdateDetail
		  ORDER BY CreatedDate DESC
		  
		  INSERT INTO TransportOrderDetailChangeLog(IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
		  VALUES(@idTransportOrderDetail,@version,'','MaterialType','system',GETDATE(),'system',GETDATE())
		  INSERT INTO TransportOrderDetailChangeLog(IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
		  VALUES(@idTransportOrderDetail,@version,'','Supplier','system',GETDATE(),'system',GETDATE())
		  INSERT INTO TransportOrderDetailChangeLog(IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
		  VALUES(@idTransportOrderDetail,@version,'','Code','system',GETDATE(),'system',GETDATE())
		  INSERT INTO TransportOrderDetailChangeLog(IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
		  VALUES(@idTransportOrderDetail,@version,'','Description','system',GETDATE(),'system',GETDATE())
		  INSERT INTO TransportOrderDetailChangeLog(IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
		  VALUES(@idTransportOrderDetail,@version,'','Qty','system',GETDATE(),'system',GETDATE())
		  INSERT INTO TransportOrderDetailChangeLog(IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
		  VALUES(@idTransportOrderDetail,@version,'','UoM','system',GETDATE(),'system',GETDATE())
	   END
    END
    FETCH NEXT FROM dataUpdateCursor INTO @stoNoUpdateDetail,@idTransportOrder,@idTransportOrderDetail,@shipmentDateAfter,@shipmentDateBefore,@shipmentDateChange,@idLocSenderAfter,@idLocSenderBefore,@idLocSenderChange,@idLocReceiveAfter,@idLocReceiveBefore,@idLocReceiveChange,@codeAfter,@codeBefore,@qtyAfter,@qtyBefore,@uomAfter,@uomBefore
END  
CLOSE dataUpdateCursor;
DEALLOCATE dataUpdateCursor;

--START: BAGIAN KALAU DETAIL DIHAPUS DARI SAP, TAPI DI TOM ADA
DECLARE dataDeleteCursor CURSOR FOR
SELECT c.IDTransportOrder,c.IDTransportOrderDetail,tot.ShipmentDate,c.ShipmentDate,CASE WHEN tot.ShipmentDate <> c.ShipmentDate THEN 1 ELSE 0 END,
         SUBSTRING(tot.SenderIDLocation,1,4),c.SenderIDLocation,CASE WHEN SUBSTRING(tot.SenderIDLocation,1,4) <> c.SenderIDLocation THEN 1 ELSE 0 END,SUBSTRING(tot.ReceiverIDLocation,1,4),c.ReceiverIDLocation,CASE WHEN SUBSTRING(tot.ReceiverIDLocation,1,4) <> c.ReceiverIDLocation THEN 1 ELSE 0 END,
         todt.Code,c.Code,todt.Qty,c.Qty,mm.MapFrom,c.UoM
FROM TransportOrderTemp AS tot
INNER JOIN TransportOrderDetailTemp AS todt
ON todt.STONo = tot.STONo
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
RIGHT JOIN(
    SELECT to1.IDTransportOrder, to1.STONo, to1.ShipmentDate,to1.SenderIDLocation, to1.ReceiverIDLocation,tod.IDTransportOrderDetail,tod.Code,tod.Qty,tod.UoM
    FROM TransportOrder AS to1
    INNER JOIN TransportOrderDetail AS tod
    ON tod.IDTransportOrder = to1.IDTransportOrder
    WHERE to1.STONo IS NOT NULL AND to1.IsActive = 1 AND tod.IsActive = 1
) AS c
ON tot.STONo = c.STONo AND todt.Code = c.Code AND mm.MapFrom = c.UoM
WHERE todt.Code IS NULL AND c.STONo IN(SELECT STONo FROM TransportOrderTemp)
OPEN dataDeleteCursor

FETCH NEXT FROM dataDeleteCursor INTO @idTransportOrder,@idTransportOrderDetail,@shipmentDateAfter,@shipmentDateBefore,@shipmentDateChange,@idLocSenderAfter,@idLocSenderBefore,@idLocSenderChange,@idLocReceiveAfter,@idLocReceiveBefore,@idLocReceiveChange,@codeAfter,@codeBefore,@qtyAfter,@qtyBefore,@uomAfter,@uomBefore
WHILE @@FETCH_STATUS = 0 
BEGIN
	SELECT TOP 1 @version = ISNULL(todcl.[Version],0) FROM TransportOrderDetailChangeLog AS todcl WHERE todcl.IDTransportOrderDetail = @idTransportOrderDetail ORDER BY todcl.[Version] DESC
     SET @version = @version + 1;
	
	UPDATE TransportOrderDetail
	SET Qty = 0,IsActive = 0
	WHERE IDTransportOrderDetail = @idTransportOrderDetail
	
	INSERT INTO TransportOrderDetailChangeLog
	(
	   IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
	)
     VALUES
     (
	   @idTransportOrderDetail,@version,@qtyBefore,'Qty','system',GETDATE(),'system',GETDATE()
     )
	
	INSERT INTO TransportOrderDetailChangeLog
	(
	   IDTransportOrderDetail,[Version],OldValue,ModifiedField,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
	)
     VALUES
     (
	   @idTransportOrderDetail,@version,1,'IsActive','system',GETDATE(),'system',GETDATE()
     )
	
	FETCH NEXT FROM dataDeleteCursor INTO @idTransportOrder,@idTransportOrderDetail,@shipmentDateAfter,@shipmentDateBefore,@shipmentDateChange,@idLocSenderAfter,@idLocSenderBefore,@idLocSenderChange,@idLocReceiveAfter,@idLocReceiveBefore,@idLocReceiveChange,@codeAfter,@codeBefore,@qtyAfter,@qtyBefore,@uomAfter,@uomBefore
END  
CLOSE dataDeleteCursor;
DEALLOCATE dataDeleteCursor;
--END: BAGIAN KALAU DETAIL DIHAPUS DARI SAP, TAPI DI TOM ADA


---START: BAGIAN PASANG STO---
UPDATE b
SET STONo = a.STONo,UpdatedBy = 'system',UpdatedDate = GETDATE()
FROM ( SELECT DENSE_RANK() OVER(PARTITION BY to1.ShipmentDate,to1.SenderIDLocation,to1.ReceiverIDLocation,tod.Code,tod.Qty,tod.UoM ORDER BY to1.IDTransportOrder) AS IDCheck,to1.STONo,to1.ShipmentDate,to1.SenderIDLocation,to1.ReceiverIDLocation,tod.Code,tod.Qty,tod.UoM,to1.UpdatedBy,to1.UpdatedDate
FROM TransportOrder AS to1
INNER JOIN TransportOrderDetail AS tod
ON tod.IDTransportOrder = to1.IDTransportOrder
WHERE (to1.STONo IS NULL OR to1.STONo = '') AND to1.IsActive = 1 AND tod.IsActive = 1
) AS b
INNER JOIN (
SELECT DENSE_RANK() OVER(PARTITION BY tot.ShipmentDate,SUBSTRING(tot.SenderIDLocation,1,4),SUBSTRING(tot.ReceiverIDLocation,1,4),todt.Code,todt.Qty,mm.MapFrom ORDER BY tot.STONo) AS IDCheck,tot.STONo,tot.ShipmentDate,SUBSTRING(tot.SenderIDLocation,1,4) AS 'SenderIDLocation',SUBSTRING(tot.ReceiverIDLocation,1,4) AS 'ReceiverIDLocation',todt.Code,CAST(todt.Qty AS INT) AS 'Qty', mm.MapFrom
FROM TransportOrderTemp AS tot
INNER JOIN TransportOrderDetailTemp AS todt
ON tot.STONo = todt.STONo
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
WHERE tot.STONo NOT IN (SELECT tempTO.STONo FROM TransportOrder AS tempTO WHERE tempTO.STONo IS NOT NULL OR tempTO.STONo <> '')
) AS a
ON a.ShipmentDate = CAST(b.ShipmentDate AS DATE) AND a.SenderIDLocation = b.SenderIDLocation AND a.ReceiverIDLocation = b.ReceiverIDLocation AND a.Code = b.Code AND a.Qty = CAST(b.Qty AS INT) AND a.MapFrom = b.UoM AND a.IDCheck = b.IDCheck
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
	   a.STONo,
	   NULL,
	   a.ShipmentDate,
	   NULL,
	   a.SenderIDLocation,
	   a.SenderIDLocation,
	   a.ReceiverIDLocation,
	   a.ReceiverIDLocation,
	   NULL,
	   'CBU',
	   CASE a.ReceiverIDLocation WHEN 'ZD30' THEN 'Bad Stock' ELSE 'Finished Good' END,
	   NULL,
	   'Submit',
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
	   'Area',
	   1,
	   'system',
	   GETDATE(),
	   'system',
	   GETDATE()
FROM(
SELECT tot.STONo,MAX(tot.ShipmentDate) AS 'ShipmentDate',SUBSTRING(tot.SenderIDLocation,1,4) AS 'SenderIDLocation',SUBSTRING(tot.ReceiverIDLocation,1,4) AS 'ReceiverIDLocation'
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
WHERE b.ShipmentDate IS NULL
GROUP BY tot.STONo,tot.SenderIDLocation,tot.ReceiverIDLocation) AS a
-- data di transport order tidak ada, tapi ada di SAP
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
       ISNULL(mf.Supplier,''),
       CASE to1.ReceiverIDLocation WHEN 'ZD30' THEN 'Market Return' ELSE 'Cigarette' END,
       todt.Code,
       ISNULL(mf.LongSpeakingCode,''),
       todt.Qty,
       mm.MapFrom,
       1,
       'system',
       GETDATE(),
       'system',
       GETDATE()
FROM TransportOrder AS to1
INNER JOIN TransportOrderDetailTemp AS todt
ON to1.STONo = todt.STONo
LEFT JOIN TransportOrderDetail AS tod
ON to1.IDTransportOrder = tod.IDTransportOrder AND todt.Code = tod.Code
LEFT JOIN MasterFABrand AS mf
ON todt.Code = mf.FACode COLLATE DATABASE_DEFAULT
INNER JOIN MasterMapping AS mm
ON todt.UoM = mm.MapTo
WHERE to1.CreatedBy = 'system' AND tod.IDTransportOrderDetail IS NULL AND to1.IsActive = 1
---FINISH: BAGIAN INSERT BARU DATA SAP KE TABEL TRANSPORT ORDER DETAIL---

---START: BAGIAN UPDATE TN YANG MASIH E- DEPANNYA DAN SEMUA STO NO SUDAH TIDAK NULL---
--UPDATE te
--SET te.TransportNo = SUBSTRING(te.TransportNo,3,LEN(te.TransportNo))
--FROM TransportExecution AS te
--WHERE SUBSTRING(te.TransportNo,1,1) = 'E' AND (SELECT COUNT(*) FROM TransportOrder AS to1 WHERE to1.STONo IS NOT NULL AND to1.IDTransportExecution = te.IDTransportExecution) > 0
---FINISH: BAGIAN UPDATE TN YANG MASIH E- DEPANNYA DAN SEMUA STO NO SUDAH TIDAK NULL---

END
GO