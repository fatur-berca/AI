IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertGIGRDate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertGIGRDate]
GO 

CREATE PROCEDURE [dbo].[InsertGIGRDate]

AS
BEGIN
	DECLARE @prosesDate DATETIME,
		  @prosesIDTransExe INT,
		  @prosesStatus VARCHAR(100)
		  
    SET @prosesDate = GETDATE();
	
	--BAGIAN GI
    UPDATE transOrder
    SET transOrder.GIDate = CAST(m.PostingDate AS DATETIME)+ CAST(m.[Time] AS DATETIME),transOrder.OrderStatus = 'On Delivery',transOrder.UpdatedDate = @prosesDate, GIBy = 'system'
    FROM TransportOrder AS transOrder
    INNER JOIN DFIS.dbo.MB51 AS m
    ON transOrder.STONo = m.PO COLLATE DATABASE_DEFAULT
    WHERE ((m.Sloc = 1000 AND m.MvT = 641) OR (m.SLoc = 2000 AND m.MvT = 101)) AND transOrder.GIDate IS NULL AND transOrder.OrderStatus = 'In Process'
    
    --BAGIAN GR
    UPDATE transOrder
    SET transOrder.GRDate = CAST(m.PostingDate AS DATETIME)+ CAST(m.[Time] AS DATETIME),transOrder.OrderStatus = 'Complete',transOrder.UpdatedDate = @prosesDate, GRBy = 'system'
    FROM TransportOrder AS transOrder
    INNER JOIN DFIS.dbo.MB51 AS m
    ON transOrder.STONo = m.PO COLLATE DATABASE_DEFAULT
    WHERE ((m.Sloc = 1000 AND m.MvT = 101) OR (m.SLoc = 2000 AND m.MvT = 311)) AND transOrder.GRDate IS NULL AND transOrder.OrderStatus = 'On Delivery'
       
    DECLARE transExe CURSOR FOR
    SELECT DISTINCT to1.IDTransportExecution
    FROM TransportOrder AS to1
    WHERE to1.UpdatedDate = @prosesDate AND (GIBy = 'system' OR GRBy = 'system')
    
    OPEN transExe
    FETCH NEXT FROM transExe INTO @prosesIDTransExe
    WHILE @@FETCH_STATUS = 0 
    BEGIN
    
	   SELECT TOP 1 @prosesStatus = a.OrderStatus 
	   FROM (
		  SELECT to1.OrderStatus,
			 CASE 
				WHEN to1.OrderStatus = 'Draft' THEN 1
				WHEN to1.OrderStatus = 'Submit' THEN 2
				WHEN to1.OrderStatus = 'In Process' THEN 3
				WHEN to1.OrderStatus = 'On Delivery' THEN 4
				WHEN to1.OrderStatus = 'Arrive At Destination and Waiting Confirmation' THEN 5
				WHEN to1.OrderStatus = 'Complete' THEN 6
				WHEN to1.OrderStatus = 'Close' THEN 7
				WHEN to1.OrderStatus IS NULL THEN 8
			 END AS 'Urutan'
		  FROM TransportOrder AS to1
		  WHERE to1.IDTransportExecution = @prosesIDTransExe) AS a
	   ORDER BY a.Urutan ASC
	   
	   UPDATE TransportExecution
	   SET TransportStatus = @prosesStatus
	   WHERE IDTransportExecution = @prosesIDTransExe
	   
	   FETCH NEXT FROM transExe INTO @prosesIDTransExe
    END  
    CLOSE transExe;
    DEALLOCATE transExe;
END