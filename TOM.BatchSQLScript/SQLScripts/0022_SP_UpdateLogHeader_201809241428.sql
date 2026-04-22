IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVersionTransportOrderChangeLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateVersionTransportOrderChangeLog]
GO 

CREATE PROCEDURE [dbo].[UpdateVersionTransportOrderChangeLog]

AS
BEGIN

DECLARE @idLog INT,
	   @latestID INT,
	   @version INT

DECLARE versionLog CURSOR FOR
SELECT tocl.IDTransportOrderChangeLog,B.IDTransportOrder, B.ver
FROM TransportOrderChangeLog AS tocl
LEFT JOIN (
	SELECT DISTINCT a.IDTransportOrder, a.ModifiedField, MAX(a.[Version]) AS ver
	FROM TransportOrderChangeLog AS a
	GROUP BY a.IDTransportOrder, a.ModifiedField
) AS B
ON tocl.IDTransportOrder = B.IDTransportOrder AND tocl.ModifiedField = B.ModifiedField
WHERE tocl.[Version] = 0
OPEN versionLog
FETCH NEXT FROM versionLog INTO @idLog,@latestID,@version
WHILE @@FETCH_STATUS = 0  
BEGIN

    IF @latestID IS NULL
    BEGIN
	   UPDATE TransportOrderChangeLog
	   SET [Version] = @version
	   WHERE IDTransportOrderChangeLog = @idLog
    END
    ELSE
    BEGIN
	   UPDATE TransportOrderChangeLog
	   SET [Version] = 1
	   WHERE IDTransportOrderChangeLog = @idLog
    END

FETCH NEXT FROM versionLog INTO @idLog,@latestID,@version
END   
CLOSE versionLog;
DEALLOCATE versionLog;

END;
GO

--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVersionTransportOrderChangeLog]') AND type in (N'P', N'PC'))
--DROP PROCEDURE [dbo].[UpdateVersionTransportOrderChangeLog]
--GO 

--CREATE PROCEDURE [dbo].[UpdateVersionTransportOrderChangeLog]

--AS
--BEGIN
--    MERGE INTO TransportOrderChangeLog AS new
--    USING (SELECT DISTINCT a.TransportOrderID, a.ModifiedField, MAX(a.[Version]) AS 'ver'
--	   FROM TransportOrderChangeLog AS a
--	   GROUP BY a.TransportOrderID, a.ModifiedField
--    ) AS old
--    ON new.TransportOrderID = old.TransportOrderID AND new.ModifiedField = old.ModifiedField
--    WHEN Matched 
--    AND new.[Version] = 0
--    THEN
--	   UPDATE SET new.[Version] = (old.ver+1)
--    WHEN Not Matched BY SOURCE
--    THEN
--	   UPDATE SET new.[Version] = 1;
--END;	
--GO
