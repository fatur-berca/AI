ALTER PROCEDURE [dbo].[SendEmailPickingList]

AS
BEGIN
DECLARE @TableData TABLE(
	TOId INT,
	STONo VARCHAR(10),
	ShipmentDate DATE,
	SenderRegion VARCHAR(50),
	ReceiverRegion VARCHAR(50),
	UpdatedField VARCHAR(MAX),
	FLagEmail INT,
	CreatedBy VARCHAR(128),
	UpdatedBy VARCHAR(128));
DECLARE @stoNo VARCHAR(10),
	   @shipmentDate DATE,
	   @updatedField VARCHAR(MAX),
	   @createdBy VARCHAR(128),
	   @updatedBy VARCHAR(128),
	   @region VARCHAR(50),
	   @bodyEmail VARCHAR(MAX),
	   @bodyNewEmail VARCHAR(MAX), 
	   @bodyEditEmail VARCHAR(MAX),
	   @textEmailNewData VARCHAR(MAX),
	   @textEmailEditData VARCHAR(MAX),
	   @flagNewDataTransport INT,
	   @flagEditDataTransport INT,
	   @flagNewDataAdmin INT,
	   @flagEditDataAdmin INT;

DECLARE @listEmailTransport VARCHAR(MAX),
	   @listEmailAdmin VARCHAR(MAX),
	   @recipient VARCHAR(100);

SET @flagNewDataTransport = 0;
SET @flagEditDataTransport = 0;
SET @flagNewDataAdmin = 0;
SET @flagEditDataAdmin = 0;

IF OBJECT_ID('dbo.EmailList', 'U') IS NOT NULL DROP TABLE EmailList; 

DECLARE @EmailList TABLE(
		Receiver VARCHAR(100)
		);

INSERT INTO @TableData
SELECT to1.IDTransportOrder, to1.STONo, to1.ShipmentDate, mlSender.ParentLocation,mlReceiver.ParentLocation,to1.ChangeLogFields,to1.FlagEmail, ISNULL(muCreated.FullName,to1.CreatedBy), ISNULL(muUpdated.FullName,to1.UpdatedBy)
FROM TransportOrder AS to1
INNER JOIN MasterLocation AS mlSender
ON to1.SenderIDLocation = mlSender.IDLocation
INNER JOIN MasterLocation AS mlReceiver
ON to1.ReceiverIDLocation = mlReceiver.IDLocation
LEFT JOIN MasterUser AS muCreated
ON to1.CreatedBy = muCreated.IDUser
LEFT JOIN MasterUser AS muUpdated
ON to1.UpdatedBy = muUpdated.IDUser
WHERE (to1.FlagEmail = 0 OR to1.FlagEmail = 1) AND to1.IsActive = 1 AND (to1.STONo IS NOT NULL OR to1.STONo <> '') AND to1.CreatedBy <> 'system'

/*
SELECT @listEmailTransport = COALESCE(@listEmailTransport + '; ' + Email, Email) FROM MasterUserRoleMapping AS murm
INNER JOIN MasterUser AS mu
ON murm.IDUser = mu.IDUser
WHERE murm.IDRole = 3 AND mu.IsActive = 1
*/
INSERT INTO @EmailList (Receiver)
SELECT DISTINCT Email  
FROM MasterUserRoleMapping urm
JOIN MasterUser u ON urm.IDUser = u.IDUser
WHERE urm.IDRole = 3 AND u.IsActive = 1

--START : BAGIAN SEND EMAIL UNTUK USER TRANSPORT NEW PICKING LIST
DECLARE emailTransport CURSOR FOR
SELECT td.ShipmentDate, td.CreatedBy
FROM @TableData AS td
WHERE td.FLagEmail = 0
GROUP BY td.ShipmentDate,td.CreatedBy

OPEN emailTransport
FETCH NEXT FROM emailTransport INTO @shipmentDate,@createdBy

SET @bodyNewEmail = '';
WHILE @@FETCH_STATUS = 0  
BEGIN
	SET @flagNewDataTransport = 1;
    SET @bodyNewEmail = @bodyNewEmail + 'Picking List for <strong>'+DATENAME(DW, @shipmentDate)+', '+CONVERT(VARCHAR(11), @shipmentDate, 106) + '</strong> have been created by <strong>'+@createdBy+'</strong><br/>';
    FETCH NEXT FROM emailTransport INTO @shipmentDate,@createdBy
END   
CLOSE emailTransport;
DEALLOCATE emailTransport;
SET @textEmailNewData ='Hi,<br/>We would like to inform you that,<br/>'+@bodyNewEmail+'<br/>Please access following link http://hmstomdev.id.pmi/TOMDEV/TransportPickingList <br/>Thank You';
SET @bodyemail ='<html><body>'+@textEmailNewData+'</body></html>';

IF @flagNewDataTransport = 1
BEGIN

	DECLARE EMAILTOCURSOR CURSOR FOR
	SELECT * FROM @EmailList

	OPEN EMAILTOCURSOR
	FETCH NEXT FROM EMAILTOCURSOR INTO @recipient

	WHILE @@FETCH_STATUS = 0  
	BEGIN
		EXEC [msdb].[dbo].[sp_send_dbmail] 
			@profile_name='DFIS_Mail',
			@body = @bodyemail,
			@body_format='HTML',
			@recipients=@recipient,
			@subject = 'DFIS Email Picking List'
		FETCH NEXT FROM EMAILTOCURSOR INTO @recipient
	END

	CLOSE EMAILTOCURSOR;
	DEALLOCATE EMAILTOCURSOR;
END
--END   : BAGIAN SEND EMAIL UNTUK USER TRANSPORT NEW PICKING LIST

--START : BAGIAN SEND EMAIL UNTUK USER TRANSPORT EDIT PICKING LIST
DECLARE emailEditTransport CURSOR FOR
SELECT DISTINCT td.STONo,'',td.UpdatedBy
FROM @TableData AS td
WHERE td.FLagEmail = 1

OPEN emailEditTransport
FETCH NEXT FROM emailEditTransport INTO @stoNo,@updatedField,@updatedBy

SET @bodyEditEmail = '';
WHILE @@FETCH_STATUS = 0  
BEGIN
	SET @flagEditDataTransport = 1;
    SET @bodyEditEmail = @bodyEditEmail + 'STO <strong>'+@stoNo+'</strong> have been updated in '+@updatedField + ' by <strong>'+@updatedBy+'</strong><br/>';
    FETCH NEXT FROM emailEditTransport INTO @stoNo,@updatedField,@updatedBy
END   
CLOSE emailEditTransport;
DEALLOCATE emailEditTransport;
SET @textEmailEditData = 'Hi,<br/>We would like to inform you that,<br/>'+@bodyEditEmail+'<br/>Thank You';
SET @bodyemail ='<html><body>'+@textEmailEditData+'</body></html>';

IF @flagEditDataTransport = 1
BEGIN
	DECLARE EMAILTOCURSOR CURSOR FOR
	SELECT * FROM @EmailList

	OPEN EMAILTOCURSOR
	FETCH NEXT FROM EMAILTOCURSOR INTO @recipient

	WHILE @@FETCH_STATUS = 0  
	BEGIN
	EXEC [msdb].[dbo].[sp_send_dbmail] 
		@profile_name='DFIS_Mail',
		@body = @bodyemail,
		@body_format='HTML',
		@recipients=@recipient,
		@subject = 'DFIS Email Picking List'
		FETCH NEXT FROM EMAILTOCURSOR INTO @recipient
	END

	CLOSE EMAILTOCURSOR;
	DEALLOCATE EMAILTOCURSOR;
END
--END   : BAGIAN SEND EMAIL UNTUK USER TRANSPORT EDIT PICKING LIST

DELETE FROM @EmailList;

DECLARE locationRegion CURSOR FOR
SELECT DISTINCT ml.IDLocation
FROM MasterLocation AS ml
WHERE ml.IsActive = 1 AND ml.[Type] = 'Warehouse'

OPEN locationRegion
FETCH NEXT FROM locationRegion INTO @region

WHILE @@FETCH_STATUS = 0  
BEGIN
	--SELECT @listEmailAdmin = COALESCE(@listEmailAdmin + '; ' + mu.Email, mu.Email)
	INSERT INTO @EmailList (Receiver)
	SELECT DISTINCT mu.Email
	FROM MasterUserLocationMapping AS mulm
	INNER JOIN MasterLocation AS ml
	ON mulm.IDLocation = ml.IDLocation COLLATE database_default
	INNER JOIN MasterUserRoleMapping AS murm
	ON mulm.IDUser = murm.IDUser
	INNER JOIN MasterUser AS mu
	ON mulm.IDUser = mu.IDUser
	WHERE mulm.IsActive = 1 AND mu.IsActive = 1 AND ml.ParentLocation = @region AND ml.IsActive = 1 AND murm.IsActive = 1 AND (murm.IDRole = 2)

    --START : BAGIAN SEND EMAIL UNTUK ADMIN NEW PICKING LIST
    DECLARE emailAdmin CURSOR FOR
    SELECT td.ShipmentDate, td.CreatedBy
    FROM @TableData AS td
    WHERE td.FLagEmail = 0 AND(td.SenderRegion = @region OR td.ReceiverRegion = @region)
    GROUP BY td.ShipmentDate,td.CreatedBy

    OPEN emailAdmin
    FETCH NEXT FROM emailAdmin INTO @shipmentDate,@createdBy
	SET @bodyNewEmail = '';
    WHILE @@FETCH_STATUS = 0  
    BEGIN
		SET @flagNewDataAdmin = 1;
		SET @bodyNewEmail = @bodyNewEmail + 'Picking List for '+DATENAME(DW, @shipmentDate)+', '+CONVERT(VARCHAR(11), @shipmentDate, 106) + 'have been created by '+@createdBy+'<br/>';
		FETCH NEXT FROM emailAdmin INTO @shipmentDate,@createdBy
    END   
    CLOSE emailAdmin;
    DEALLOCATE emailAdmin;
    SET @textEmailNewData ='Hi,<br/>We would like to inform you that,<br/>'+@bodyNewEmail+'<br/>Please access following link http://hmstomdev.id.pmi/TOMDEV/TransportPickingList <br/>Thank You';
    SET @bodyemail ='<html><body>'+@textEmailNewData+'</body></html>';

	IF @flagNewDataAdmin = 1
	BEGIN
		DECLARE EMAILTOCURSOR CURSOR FOR
		SELECT * FROM @EmailList

		OPEN EMAILTOCURSOR
		FETCH NEXT FROM EMAILTOCURSOR INTO @recipient

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			EXEC [msdb].[dbo].[sp_send_dbmail] 
			   @profile_name='DFIS_Mail',
			   @body = @bodyemail,
			   @body_format='HTML',
			   @recipients=@recipient,
			   @subject = 'DFIS Email Picking List'
		END
		CLOSE EMAILTOCURSOR;
		DEALLOCATE EMAILTOCURSOR;
	END
    --END   : BAGIAN SEND EMAIL UNTUK ADMIN NEW PICKING LIST

    --START : BAGIAN SEND EMAIL UNTUK ADMIN EDIT PICKING LIST
    DECLARE emailEditAdmin CURSOR FOR
    SELECT DISTINCT td.STONo,'',td.UpdatedBy
    FROM @TableData AS td
    WHERE td.FLagEmail = 1 AND(td.SenderRegion = @region OR td.ReceiverRegion = @region)

    OPEN emailEditAdmin
    FETCH NEXT FROM emailEditAdmin INTO @stoNo,@updatedField,@updatedBy
	SET @bodyEditEmail = '';
    WHILE @@FETCH_STATUS = 0  
    BEGIN
		SET @flagEditDataAdmin = 1;
		SET @bodyEditEmail = @bodyEditEmail + 'STO '+@stoNo+' have been updated in '+@updatedField + ' by '+@updatedBy+'<br/>';
		FETCH NEXT FROM emailEditTransport INTO @stoNo,@updatedField,@updatedBy
    END   
    CLOSE emailEditAdmin;
    DEALLOCATE emailEditAdmin;
    SET @textEmailEditData = 'Hi,<br/>We would like to inform you that,<br/>'+@bodyEditEmail+'<br/>Thank You';
    SET @bodyemail ='<html><body>'+@textEmailEditData+'</body></html>';

	IF @flagEditDataAdmin = 1
	BEGIN
		DECLARE EMAILTOCURSOR CURSOR FOR
		SELECT * FROM EmailList

		OPEN EMAILTOCURSOR
		FETCH NEXT FROM EMAILTOCURSOR INTO @recipient

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			EXEC [msdb].[dbo].[sp_send_dbmail] 
			   @profile_name='DFIS_Mail',
			   @body = @bodyemail,
			   @body_format='HTML',
			   @recipients=@listEmailAdmin,
			   @subject = 'DFIS Email Picking List'
		END
		CLOSE EMAILTOCURSOR;
		DEALLOCATE EMAILTOCURSOR;
	END
    --END   : BAGIAN SEND EMAIL UNTUK ADMIN EDIT PICKING LIST
    FETCH NEXT FROM locationRegion INTO @region
END   
CLOSE locationRegion;
DEALLOCATE locationRegion;
 
--START : BAGIAN UPDATE FLAG EMAIL
MERGE [dbo].TransportOrder as to2
USING (SELECT td.TOId FROM @TableData AS td) AS tempTable
ON to2.IDTransportOrder = tempTable.TOId
WHEN Matched THEN
    UPDATE SET to2.ChangeLogFields = '', to2.FlagEmail = 2, to2.UpdatedBy = 'Email', to2.UpdatedDate = GETDATE();
--END   : BAGIAN UPDATE FLAG EMAIL 
 
END;



