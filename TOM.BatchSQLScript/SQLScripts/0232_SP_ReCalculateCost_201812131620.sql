IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReCalculateCost]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReCalculateCost]
GO 

CREATE PROCEDURE [dbo].[ReCalculateCost]

AS
BEGIN
    DECLARE @effectiveStartDate DATETIME,
		  @effectiveEndDate DATETIME,
		  @idTranExe VARCHAR(10)
		  
    SELECT DISTINCT @effectiveStartDate=MIN(mc.EffectiveStartDate),@effectiveEndDate=MAX(mc.EffectiveEndDate)
    FROM MasterCost AS mc
    WHERE CAST(mc.UpdatedDate AS DATE) = CAST(DATEADD(DAY,DATEDIFF(DAY,1,GETDATE()),0) AS DATE) AND mc.IsActive = 1
    
    DECLARE transportExecutionCursor CURSOR FOR 
    SELECT te.IDTransportExecution
    FROM TransportExecution AS te
    WHERE te.TransportDate BETWEEN @effectiveStartDate AND @effectiveEndDate AND te.IsActive = 1
 
    OPEN transportExecutionCursor
    FETCH NEXT FROM transportExecutionCursor INTO @idTranExe
    WHILE @@FETCH_STATUS = 0  
    BEGIN
	   
	   EXEC [dbo].[CalculateCost] @idTranExeParam = @idTranExe, @idUserParam = N'system', @recalculate = 1
	   
	   FETCH NEXT FROM transportExecutionCursor INTO @idTranExe
    END   
    CLOSE transportExecutionCursor;
    DEALLOCATE transportExecutionCursor;
    
END
