IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CalculateCost]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CalculateCost]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CalculateCost]
	   @idTranExeParam VARCHAR(MAX),
	   @idUserParam VARCHAR(500),
	   @recalculate INT
AS
BEGIN

SET FMTONLY OFF
SET NOCOUNT ON;
 
 DECLARE @idTranExe VARCHAR(10),
 	   @transStatus VARCHAR(20),
 	   @idVendor INT,
 	   @vendorCategory VARCHAR(50),
 	   @transDate DATE,
 	   @startLocation VARCHAR(50),
 	   @finishLocation VARCHAR(50),
 	   @through VARCHAR(50),
 	   @basedPrice DECIMAL(14,2),
 	   @asdpCost DECIMAL(14,2),
 	   @spsiCost DECIMAL(14,2),
 	   @totalBox INT,
 	   @minBox INT,
 	   @totalCost DECIMAL(14,2),
 	   @flagKMBased BIT,
 	   @monthCursor INT,
 	   @yearCursor INT,
 	   @idVendorCursor INT,
 	   @minKM DECIMAL(10,2),
 	   @latestRank INT,
 	   @totalKMCalculate DECIMAL(10,2),
 	   @cumulativeKMCalculate DECIMAL(10,2),
 	   @totalKMBasedPriceCalculate DECIMAL(10,2),
 	   @totalKMDiscountPriceCalculate DECIMAL(10,2)

CREATE TABLE dbo.#tempVendorCheck
(
	[Month] INT,
	[Year] INT,
	IDVendor INT
)

CREATE TABLE dbo.#tempTabelTransExecution
(
	IDTransportExecution INT,
	TransportNo VARCHAR(50),
	TransportDate DATE,
	IDVendor INT,
	TotalKM DECIMAL(18,2)
)

CREATE TABLE dbo.#tempTabelTransExecutionByVendor
(
	[Rank] INT,
	IDTransportExecution INT,
	TransportNo VARCHAR(50),
	TransportDate DATE,
	IDVendor INT,
	TotalKM DECIMAL(18,2),
	CumulativeTotalKM DECIMAL(18,2)
)

SET @flagKMBased = 0;
 
DECLARE transportExecutionCursor CURSOR FOR 
SELECT Value FROM FN_SPLIT_STRING(@idTranExeParam, ',')
 
OPEN transportExecutionCursor
FETCH NEXT FROM transportExecutionCursor INTO @idTranExe
WHILE @@FETCH_STATUS = 0  
BEGIN
	SET @vendorCategory = NULL;
	
	SELECT @transStatus = te.TransportStatus, @idVendor = te.IDVendor, @vendorCategory = mv.VendorCategory, @transDate = te.TransportDate, @startLocation = te.StartLocation, @finishLocation = te.FinishLocation, @totalBox = CEILING (te.TotalBox)
	FROM TransportExecution AS te
	LEFT JOIN MasterVendor AS mv
	ON te.IDVendor = mv.IDVendor
	WHERE te.IDTransportExecution = @idTranExe
	
	IF @transStatus = 'Close' AND @recalculate = 0
	BEGIN
		UPDATE TransportExecution
		SET TransportStatus = 'Complete', UpdatedBy = @idUserParam, UpdatedDate = GETDATE()
		WHERE IDTransportExecution = @idTranExe
	END
	ELSE IF @transStatus = 'Complete' OR @recalculate = 1
	BEGIN
		IF @vendorCategory = 'KM Based'
		BEGIN
		--START: BAGIAN UPDATE ASDP
		SET @asdpCost = 0;
		SELECT @asdpCost = ISNULL(SUM(mc.BasedPrice),0)
		FROM TransportExecution AS te5
		INNER JOIN (SELECT t1.IDTransportExecution, t1.IDLocationFrom, t2.IDLocationTo
				    FROM(
					   SELECT RANK() OVER (ORDER BY tr.IDTransportRoute ASC) AS 'Rank', tr.IDTransportExecution, tr.IDLocation AS 'IDLocationFrom'
					   FROM TransportRoute AS tr
				    ) t1,
				    (
					   SELECT RANK() OVER (ORDER BY tr.IDTransportRoute ASC) AS 'Rank', tr.IDLocation AS 'IDLocationTo'
					   FROM TransportRoute AS tr
				    ) t2
				    WHERE t2.[Rank] = t1.[Rank] + 1) AS tr
		ON tr.IDTransportExecution = te5.IDTransportExecution
		INNER JOIN MasterCost AS mc
		ON tr.IDLocationFrom = mc.SenderIDLocation AND tr.IDLocationTo = mc.ReceiverIDLocation AND te5.IDVendor = mc.IDVendor AND te5.ActualVehicleType = mc.VehicleType
		WHERE te5.IDTransportExecution = @idTranExe AND mc.CostType = 'ASDP' AND te5.TransportDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate
		--END: BAGIAN UPDATE ASDP 
		
		--START: BAGIAN UPDATE SPSI
		SET @spsiCost = 0;
		SELECT @spsiCost = ISNULL(SUM(mc.BasedPrice),0)
		FROM TransportExecution AS te5
		INNER JOIN (SELECT t1.IDTransportExecution, t1.IDLocationFrom, t2.IDLocationTo
				    FROM(
					   SELECT RANK() OVER (ORDER BY tr.IDTransportRoute ASC) AS 'Rank', tr.IDTransportExecution, tr.IDLocation AS 'IDLocationFrom'
					   FROM TransportRoute AS tr
				    ) t1,
				    (
					   SELECT RANK() OVER (ORDER BY tr.IDTransportRoute ASC) AS 'Rank', tr.IDLocation AS 'IDLocationTo'
					   FROM TransportRoute AS tr
				    ) t2
				    WHERE t2.[Rank] = t1.[Rank] + 1) AS tr
		ON tr.IDTransportExecution = te5.IDTransportExecution
		INNER JOIN MasterCost AS mc
		ON tr.IDLocationFrom = mc.SenderIDLocation AND tr.IDLocationTo = mc.ReceiverIDLocation AND te5.IDVendor = mc.IDVendor AND te5.ActualVehicleType = mc.VehicleType
		WHERE te5.IDTransportExecution = @idTranExe AND mc.CostType = 'SPSI' AND te5.TransportDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate
		--END: BAGIAN UPDATE SPSI 
		
		UPDATE TransportExecution
		SET ASDPCost = @asdpCost,SPSICost = @spsiCost
		WHERE IDTransportExecution = @idTranExe
		
		END
		
		IF @recalculate = 0
		BEGIN
			IF @vendorCategory <> 'KM Based'
		     BEGIN
				UPDATE TransportExecution
			     SET TransportStatus = 'Close',BasedCost = @totalCost, DiscountCost = 0, ASDPCost = 0, SPSICost = 0, TotalCost = @totalCost + ISNULL(AdditionalCost,0), UpdatedBy = @idUserParam, UpdatedDate = GETDATE()
			     WHERE IDTransportExecution = @idTranExe
		     END
		     ELSE
		     BEGIN
			     UPDATE TransportExecution
			     SET TransportStatus = 'Close'
			     WHERE IDTransportExecution = @idTranExe
		     END 
		END
		
		SET @totalCost = 0;
		IF @vendorCategory IS NOT NULL 
		BEGIN	
			IF @vendorCategory = 'Trip Based'
			BEGIN
				SELECT @through = a.IDLocation
				FROM(
				SELECT ROW_NUMBER() OVER(ORDER BY tr.CreatedDate DESC) AS IDCheck, tr.IDLocation
				FROM TransportRoute AS tr
				WHERE tr.IDTransportExecution = @idTranExe) AS a
				WHERE a.IDCheck = 2
				
				SELECT @totalCost = mc.BasedPrice
				FROM MasterCost AS mc
				WHERE @transDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate AND mc.IsActive = 1 AND mc.SenderIDLocation = @startLocation AND mc.ReceiverIDLocation = @finishLocation AND mc.ThroughIDLocation = @through AND mc.CostType = 'Trip Based'
			END
			ELSE IF @vendorCategory = 'Box Based'
			BEGIN
				SELECT @basedPrice = mc.BasedPrice, @minBox = mc.MinimumBox
				FROM MasterCost AS mc
				WHERE @transDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate AND mc.IsActive = 1 AND mc.SenderIDLocation = @startLocation AND mc.ReceiverIDLocation = @finishLocation AND mc.CostType = 'Box Based'
				IF @totalBox < @minBox
				BEGIN
				    SET @totalCost = @minBox * @basedPrice;
				END
				ELSE
				BEGIN
				    SET @totalCost = @totalBox * @basedPrice;
				END
			END
			ELSE IF @vendorCategory = 'KM Based'
			BEGIN
				IF NOT EXISTS (SELECT * FROM dbo.#tempVendorCheck WHERE [Month] = MONTH(@transDate) AND [Year] = YEAR(@transDate) AND IDVendor = @idVendor) 
				BEGIN
				    INSERT INTO dbo.#tempVendorCheck VALUES(MONTH(@transDate),YEAR(@transDate),@idVendor)
				    INSERT INTO dbo.#tempTabelTransExecution
				    SELECT te.IDTransportExecution, te.TransportNo,te.TransportDate, te.IDVendor, ISNULL(te.TotalKM,0) AS 'TotalKM'
				    FROM TransportExecution AS te
				    WHERE MONTH(te.TransportDate) = MONTH(@transDate) AND YEAR(te.TransportDate) = YEAR(@transDate) AND te.IDVendor = @idVendor AND te.TransportStatus = 'Close'
				END
				SET @flagKMBased = 1;
			END
		END
	END
	
	FETCH NEXT FROM transportExecutionCursor INTO @idTranExe
END   
CLOSE transportExecutionCursor;
DEALLOCATE transportExecutionCursor;

IF @flagKMBased = 1
BEGIN
    DECLARE tempVendorCursor CURSOR FOR 
    SELECT * FROM dbo.#tempVendorCheck

    OPEN tempVendorCursor
    FETCH NEXT FROM tempVendorCursor INTO @monthCursor, @yearCursor, @idVendorCursor
    WHILE @@FETCH_STATUS = 0  
    BEGIN
	   
	   INSERT INTO dbo.#tempTabelTransExecutionByVendor
	   SELECT rank() OVER (ORDER BY tempTE1.IDTransportExecution,tempTE1.TransportDate,tempTE1.TransportNo ASC) as 'Rank',tempTE1.IDTransportExecution,tempTE1.TransportNo,tempTE1.TransportDate,tempTE1.IDVendor,tempTE1.TotalKM,SUM(tempTE2.TotalKM) AS 'Cumulative'
	   FROM 
	   (
  		  SELECT rank() OVER (ORDER BY tempTE.TransportDate,tempTE.TransportNo ASC) as 'Rank',tempTE.IDTransportExecution,tempTE.TransportNo,tempTE.TransportDate,tempTE.IDVendor,ISNULL(tempTE.TotalKM,0) AS 'TotalKM'
  		  FROM dbo.#tempTabelTransExecution tempTE
  		  WHERE MONTH(tempTE.TransportDate) = @monthCursor AND YEAR(tempTE.TransportDate) = @yearCursor AND tempTE.IDVendor = @idVendorCursor
	   ) tempTE1,
	   (
  		  SELECT rank() OVER (ORDER BY tempTE.TransportDate,tempTE.TransportNo ASC) as 'Rank',ISNULL(tempTE.TotalKM,0) AS 'TotalKM'
  		  FROM dbo.#tempTabelTransExecution tempTE
  		  WHERE MONTH(tempTE.TransportDate) = @monthCursor AND YEAR(tempTE.TransportDate) = @yearCursor AND tempTE.IDVendor = @idVendorCursor
	   ) tempTE2
	   WHERE tempTE2.[Rank] <= tempTE1.[Rank]
	   GROUP BY tempTE1.IDTransportExecution,tempTE1.TransportNo,tempTE1.TransportDate,tempTE1.IDVendor,tempTE1.TotalKM
	   
	   SET @minKM = NULL;
	   
	   SELECT TOP 1 @minKM = mc.MinimumKM
	   FROM MasterCost AS mc
	   WHERE mc.CostType = 'KM Based' AND mc.IDVendor = @idVendorCursor AND mc.IsActive = 1 AND @monthCursor BETWEEN MONTH(mc.EffectiveStartDate) AND MONTH(mc.EffectiveEndDate) AND @yearCursor = YEAR(mc.EffectiveStartDate)
	   
	   IF @minKM IS NOT NULL--KALAU MINIMUM KM TIDAK ADA DI DB, MAKA TIDAK DILAKUKAN PERHITUNGAN
	   BEGIN
		  SET @latestRank = NULL;
		  
		  SELECT TOP 1 @latestRank = [Rank]
		  FROM dbo.#tempTabelTransExecutionByVendor
		  WHERE CumulativeTotalKM < @minKM
		  ORDER BY [Rank] DESC
	   
		  IF @latestRank IS NULL--DATA YANG ADA TIDAK ADA YANG DI BAWAH MINIMUM KM, DARI AWAL DATA ADA DI ATAS MINIMUM KM
		  BEGIN
			 SET @latestRank = 1;

			 SELECT @totalKMCalculate = TotalKM, @cumulativeKMCalculate = CumulativeTotalKM
			 FROM dbo.#tempTabelTransExecutionByVendor
			 WHERE [Rank] = @latestRank
			 SET @totalKMBasedPriceCalculate = @minKM;
			 SET @totalKMDiscountPriceCalculate = @totalKMCalculate - @minKM;
	   
			 UPDATE te3
			 SET te3.BasedCost = @totalKMBasedPriceCalculate * mc.BasedPrice, te3.DiscountCost = @totalKMDiscountPriceCalculate * mc.DiscountPrice, te3.TotalCost = (@totalKMBasedPriceCalculate * mc.BasedPrice) + (@totalKMDiscountPriceCalculate * mc.DiscountPrice) + ISNULL(te3.ASDPCost,0) + ISNULL(te3.SPSICost,0) + ISNULL(te3.AdditionalCost,0), te3.UpdatedBy = @idUserParam, te3.UpdatedDate = GETDATE()
			 FROM TransportExecution AS te3
			 INNER JOIN dbo.#tempTabelTransExecutionByVendor tempTe3
			 ON te3.IDTransportExecution = tempTe3.IDTransportExecution
			 INNER JOIN MasterCost AS mc
			 ON mc.IDVendor = tempTe3.IDVendor AND te3.TransportDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate
			 WHERE tempTe3.[Rank] = @latestRank AND mc.CostType = 'KM Based'
		  END
		  ELSE
		  BEGIN
	   		 --START: BAGIAN UPDATE HARGA DI BAWAH MINIMUM KM
			 UPDATE te2
			 SET te2.BasedCost = tempTe2.TotalKM * mc.BasedPrice, te2.DiscountCost = 0, te2.TotalCost = (tempTe2.TotalKM * mc.BasedPrice) + ISNULL(te2.ASDPCost,0) + ISNULL(te2.SPSICost,0) + ISNULL(te2.AdditionalCost,0), te2.UpdatedBy = @idUserParam, te2.UpdatedDate = GETDATE()
			 FROM TransportExecution AS te2
			 INNER JOIN dbo.#tempTabelTransExecutionByVendor tempTe2
			 ON te2.IDTransportExecution = tempTe2.IDTransportExecution
			 INNER JOIN MasterCost AS mc
			 ON mc.IDVendor = tempTe2.IDVendor AND te2.TransportDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate
			 WHERE tempTe2.[Rank] <= @latestRank AND mc.CostType = 'KM Based'
			 --END: BAGIAN UPDATE HARGA DI BAWAH MINIMUM KM
	   
			 --START: BAGIAN UPDATE HARGA PERANTARA DI BAWAH MINIMUM KM DAN DI ATAS MINIMUM KM
			 SET @latestRank = @latestRank + 1;
			 SELECT @totalKMCalculate = TotalKM, @cumulativeKMCalculate = CumulativeTotalKM
			 FROM dbo.#tempTabelTransExecutionByVendor
			 WHERE [Rank] = @latestRank
			 SET @totalKMBasedPriceCalculate = @totalKMCalculate - (@cumulativeKMCalculate - @minKM);
			 SET @totalKMDiscountPriceCalculate = @totalKMCalculate - @totalKMBasedPriceCalculate;
	   
			 UPDATE te3
			 SET te3.BasedCost = @totalKMBasedPriceCalculate * mc.BasedPrice, te3.DiscountCost = @totalKMDiscountPriceCalculate * mc.DiscountPrice, te3.TotalCost = (@totalKMBasedPriceCalculate * mc.BasedPrice) + (@totalKMDiscountPriceCalculate * mc.DiscountPrice) + ISNULL(te3.ASDPCost,0) + ISNULL(te3.SPSICost,0) + ISNULL(te3.AdditionalCost,0), te3.UpdatedBy = @idUserParam, te3.UpdatedDate = GETDATE()
			 FROM TransportExecution AS te3
			 INNER JOIN dbo.#tempTabelTransExecutionByVendor tempTe3
			 ON te3.IDTransportExecution = tempTe3.IDTransportExecution
			 INNER JOIN MasterCost AS mc
			 ON mc.IDVendor = tempTe3.IDVendor AND te3.TransportDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate
			 WHERE tempTe3.[Rank] = @latestRank AND mc.CostType = 'KM Based'
 			 --END: BAGIAN UPDATE HARGA PERANTARA DI BAWAH MINIMUM KM DAN DI ATAS MINIMUM KM 	
		  END

 		  --START: BAGIAN UPDATE HARGA DI ATAS MINIMUM KM
 		  UPDATE te4
		  SET te4.BasedCost = 0, te4.DiscountCost = tempTe4.TotalKM * mc.DiscountPrice, te4.TotalCost = (tempTe4.TotalKM * mc.DiscountPrice) + ISNULL(te4.ASDPCost,0) + ISNULL(te4.SPSICost,0) + ISNULL(te4.AdditionalCost,0), te4.UpdatedBy = @idUserParam, te4.UpdatedDate = GETDATE()
		  FROM TransportExecution AS te4
		  INNER JOIN dbo.#tempTabelTransExecutionByVendor tempTe4
		  ON te4.IDTransportExecution = tempTe4.IDTransportExecution
		  INNER JOIN MasterCost AS mc
		  ON mc.IDVendor = tempTe4.IDVendor AND te4.TransportDate BETWEEN mc.EffectiveStartDate AND mc.EffectiveEndDate
		  WHERE tempTe4.[Rank] > @latestRank AND mc.CostType = 'KM Based'
 		  --END: BAGIAN UPDATE HARGA DI ATAS MINIMUM KM
	   
	   END
	   
	   FETCH NEXT FROM tempVendorCursor INTO @monthCursor, @yearCursor, @idVendorCursor
    END   
    CLOSE tempVendorCursor;
    DEALLOCATE tempVendorCursor;
	
END

END
