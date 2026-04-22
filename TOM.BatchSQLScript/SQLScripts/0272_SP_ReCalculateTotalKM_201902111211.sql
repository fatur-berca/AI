IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReCalculateTotalKM]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReCalculateTotalKM]
GO 

CREATE PROCEDURE [dbo].[ReCalculateTotalKM]

AS
BEGIN
    DECLARE @idTranExe INT,
		  @idTranExeBefore INT,
		  @transportDate DATETIME,
		  @transportMode VARCHAR(50),
		  @idLocFrom VARCHAR(50),
		  @idLocTo VARCHAR(50),
		  @IsNonKMBasedFrom BIT,
		  @IsNonKMBasedTo BIT,
		  @TotalKMBased DECIMAL(18,4),
		  @KMBased DECIMAL(18,4),
		  @TotalKMSea DECIMAL(18,4),
		  @KMSea DECIMAL(18,4),
		  @TotalKMRail DECIMAL(18,4),
		  @KMRail DECIMAL(18,4),
		  @through VARCHAR(50),
		  @via VARCHAR(50)
    
    SET @idTranExeBefore = 0;
    SET @TotalKMBased = 0;
    SET @TotalKMSea = 0;
    SET @TotalKMRail = 0;
    SET @through = '';
    SET @via = '';
		  
    DECLARE transportExecutionCursor CURSOR FOR 

    SELECT te.IDTransportExecution,te.TransportDate,te.TransportMode,tr.IDLocation,tr2.IDLocation,tr.IsNonKMBased,tr2.IsNonKMBased
    FROM TransportExecution AS te
    INNER JOIN( 
    SELECT DENSE_RANK() OVER(PARTITION BY IDTransportExecution ORDER BY IDTransportRoute) AS IDCheck,* 
    FROM TransportRoute)
    AS tr
    ON te.IDTransportExecution = tr.IDTransportExecution
    INNER JOIN ( 
    SELECT DENSE_RANK() OVER(PARTITION BY IDTransportExecution ORDER BY IDTransportRoute) AS IDCheck,* 
    FROM TransportRoute) 
    AS tr2
    ON tr.IDCheck+1 = tr2.IDCheck AND te.IDTransportExecution = tr2.IDTransportExecution
 
    OPEN transportExecutionCursor
    FETCH NEXT FROM transportExecutionCursor INTO @idTranExe,@transportDate,@transportMode,@idLocFrom,@idLocTo,@IsNonKMBasedFrom,@IsNonKMBasedTo
    WHILE @@FETCH_STATUS = 0  
    BEGIN
	   --START: BAGIAN UPDATE TOTAL KM
	   IF @idTranExeBefore <> 0 AND @idTranExeBefore <> @idTranExe
	   BEGIN
		  UPDATE TransportExecution
		  SET
		  	TotalKM = CAST(ROUND(@TotalKMBased,0) AS INT),
		  	TotalKMRail = CAST(ROUND(@TotalKMRail,0) AS INT),
		  	TotalKMBased = CAST(ROUND(@TotalKMBased,0) AS INT),
		  	TotalKMSea = CAST(ROUND(@TotalKMSea,0) AS INT)
		  WHERE IDTransportExecution = @idTranExeBefore
		  
		  SET @TotalKMBased = 0;
		  SET @TotalKMSea = 0;
		  SET @TotalKMRail = 0;
		  SET @via = '';
		  SET @idTranExeBefore = @idTranExe;
	   END
	   --END: BAGIAN UPDATE TOTAL KM
	   IF @IsNonKMBasedFrom = 1 AND @IsNonKMBasedTo = 1
	   BEGIN  
		  IF @transportMode = 'Ship'
		  BEGIN
			 SELECT @KMSea = md.Total 
			 FROM MasterDistance AS md
			 WHERE md.DistanceType = 'Sea' AND md.IDSender = @idLocFrom AND md.IDReceiver = @idLocTo AND @transportDate BETWEEN md.EffectiveStartDate AND md.EffectiveEndDate AND md.IsActive = 1
		  
			 IF @KMSea IS NOT NULL 
			 BEGIN
				SET @TotalKMSea = @TotalKMSea + @KMSea;
			 END
		  END
		  ELSE IF @transportMode = 'Train'
		  BEGIN
			 SELECT @KMRail = md.Total 
			 FROM MasterDistance AS md
			 WHERE md.DistanceType = 'Rail' AND md.IDSender = @idLocFrom AND md.IDReceiver = @idLocTo AND @transportDate BETWEEN md.EffectiveStartDate AND md.EffectiveEndDate AND md.IsActive = 1
		  
			 IF @KMRail IS NOT NULL 
			 BEGIN
				SET @TotalKMRail = @TotalKMRail + @KMRail;
			 END
		  END 
	   END
	   ELSE
	   BEGIN
		  SELECT @KMBased = md.Total 
		  FROM MasterDistance AS md
		  WHERE md.DistanceType = 'KM Based' AND md.IDSender = @idLocFrom AND md.IDReceiver = @idLocTo AND @transportDate BETWEEN md.EffectiveStartDate AND md.EffectiveEndDate AND md.IsActive = 1
		  
		  IF @KMBased IS NOT NULL 
		  BEGIN
			 SET @TotalKMBased = @TotalKMBased + @KMBased;
		  END
	   END
	   
	   FETCH NEXT FROM transportExecutionCursor INTO @idTranExe,@transportDate,@transportMode,@idLocFrom,@idLocTo,@IsNonKMBasedFrom,@IsNonKMBasedTo
    END   
    CLOSE transportExecutionCursor;
    DEALLOCATE transportExecutionCursor;
END






