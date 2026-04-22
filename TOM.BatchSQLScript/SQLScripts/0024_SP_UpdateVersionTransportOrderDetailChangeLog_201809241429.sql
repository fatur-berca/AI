IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVersionTransportOrderDetailChangeLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateVersionTransportOrderDetailChangeLog]
GO 

CREATE PROCEDURE [dbo].[UpdateVersionTransportOrderDetailChangeLog]

AS
BEGIN

DECLARE @idLog INT,
	   @latestID INT,
	   @version INT

DECLARE versionLog CURSOR FOR
SELECT tocl.IDTransportOrderDetailChangeLog,B.IDTransportOrderDetail, B.ver
FROM TransportOrderDetailChangeLog AS tocl
LEFT JOIN (
	SELECT DISTINCT a.IDTransportOrderDetail, a.ModifiedField, MAX(a.[Version]) AS ver
	FROM TransportOrderDetailChangeLog AS a
	GROUP BY a.IDTransportOrderDetail, a.ModifiedField
) AS B
ON tocl.IDTransportOrderDetail = B.IDTransportOrderDetail AND tocl.ModifiedField = B.ModifiedField
WHERE tocl.[Version] = 0
OPEN versionLog
FETCH NEXT FROM versionLog INTO @idLog,@latestID,@version
WHILE @@FETCH_STATUS = 0  
BEGIN

    IF @latestID IS NULL
    BEGIN
	   UPDATE TransportOrderDetailChangeLog
	   SET [Version] = @version
	   WHERE IDTransportOrderDetailChangeLog = @idLog
    END
    ELSE
    BEGIN
	   UPDATE TransportOrderDetailChangeLog
	   SET [Version] = 1
	   WHERE IDTransportOrderDetailChangeLog = @idLog
    END

FETCH NEXT FROM versionLog INTO @idLog,@latestID,@version
END   
CLOSE versionLog;
DEALLOCATE versionLog;

END;
GO

--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVersionTransportOrderDetailChangeLog]') AND type in (N'P', N'PC'))
--DROP PROCEDURE [dbo].[UpdateVersionTransportOrderDetailChangeLog]
--GO 

--CREATE PROCEDURE [dbo].[UpdateVersionTransportOrderDetailChangeLog]

--AS
--BEGIN
--    MERGE INTO TransportOrderDetailChangeLog AS new
--    USING (SELECT DISTINCT a.TransportOrderDetailID, a.ModifiedField, MAX(a.[Version]) AS 'ver'
--	   FROM TransportOrderDetailChangeLog AS a
--	   GROUP BY a.TransportOrderDetailID, a.ModifiedField
--    ) AS old
--    ON new.TransportOrderDetailID = old.TransportOrderDetailID AND new.ModifiedField = old.ModifiedField
--    WHEN Matched 
--    AND new.[Version] = 0
--    THEN
--	   UPDATE SET new.[Version] = (old.ver+1)
--    WHEN Not Matched BY SOURCE
--    THEN
--	   UPDATE SET new.[Version] = 1;
--END;
--GO