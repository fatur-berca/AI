/* Created By : Diyah
Date : 2018-09-30 */

-- delete Master Function & RoleFunctionMapping
truncate table MasterRolesFunctionMapping
delete from MasterFunction where ParentIDFunction is not null and [Type] = 'Button'
delete from MasterFunction where [Type] = 'Button'
delete from MasterFunction where ParentIDFunction is not null and [Type] = 'Form'
delete from MasterFunction where [Type] = 'Form'
delete from MasterFunction
DBCC CHECKIDENT ('MasterFunction', RESEED, 0)

-- Insert Module
	INSERT INTO MasterFunction VALUES('Master',0,'Module',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Transaction',0,'Module',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Report',0,'Module',1,'system',GETDATE(),'system',GETDATE(),'');


DECLARE @temp TABLE(id VARCHAR(50));
DECLARE	@idmaster INT,
		@ParentID int,
		@ParentName VARCHAR(50)
	
-- Insert Form Master
	SET @idmaster = (SELECT IDFunction FROM MasterFunction WHERE FunctionName = 'Master');
	INSERT INTO MasterFunction VALUES('MstCost',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstCostCenterAccount',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstDistance',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstLeadTimeTom',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstServicePoTom',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstTruckSealStock',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstUom',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstUserRoleDelegation',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstVendorTOM',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstVendorSuggestion',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstLocation',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
		
INSERT INTO @temp
	SELECT FunctionName FROM MasterFunction WHERE [Type] = 'Form' and ParentIDFunction = 1 order by CreatedDate desc
		
	DECLARE cursor_mst CURSOR LOCAL FOR
	SELECT id FROM @temp
	OPEN cursor_mst

	FETCH NEXT FROM cursor_mst
	INTO @ParentName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @idmaster = (SELECT top 1 IDFunction FROM MasterFunction WHERE FunctionName = @ParentName order by CreatedDate desc)
		if @ParentName = 'MstConfiguration'
		BEGIN
			INSERT INTO MasterFunction VALUES('SaveChanges',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
			INSERT INTO MasterFunction VALUES('Close',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		END
		ELSE IF @ParentName = 'MstCost' OR @ParentName = 'MstDistance' OR @ParentName = 'MstLeadTimeTom' OR @ParentName = 'MstVendorSuggestion'
		BEGIN
			INSERT INTO MasterFunction VALUES('AddNew',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
			INSERT INTO MasterFunction VALUES('SaveChanges',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
			INSERT INTO MasterFunction VALUES('Upload',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
			INSERT INTO MasterFunction VALUES('Close',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		END
		ELSE
		BEGIN
			INSERT INTO MasterFunction VALUES('AddNew',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
			INSERT INTO MasterFunction VALUES('SaveChanges',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
			INSERT INTO MasterFunction VALUES('Close',@idmaster,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		END		
	FETCH NEXT FROM cursor_mst
	INTO @ParentName;
	END
	CLOSE cursor_mst;
	
-- Insert Form Transport Picking List
	SET @idmaster = (SELECT IDFunction FROM MasterFunction WHERE FunctionName = 'Transaction');

	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('TransportPickingList',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'TransportPickingList';

	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Upload',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Email',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('GenerateSAP',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Log',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Transportation Order
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('TransportationOrder',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'TransportationOrder';

	INSERT INTO MasterFunction VALUES('AddNew',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Delete',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomExport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('AddNewUnit',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('SaveAsDraft',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Submit',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Transportation Execution
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('TransportationExecution',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'TransportationExecution';

	INSERT INTO MasterFunction VALUES('AddNew',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Delete',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('PrintDocument',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Upload',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('ShippingInstruction',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('GenerateTN',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Save',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Transportation Monitoring
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('TransportationMonitoring',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'TransportationMonitoring';

	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Upload',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Edit',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Save',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Transportation Seal
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('TrTransportationSeal',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'TrTransportationSeal';

	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Upload',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Edit',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Save',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Loading Unloading History
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('LoadingUnloadingHistory',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'LoadingUnloadingHistory';

	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Upload',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Edit',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Save',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form DriverManagement
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('DriverManagement',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'DriverManagement';

	INSERT INTO MasterFunction VALUES('AddNew',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Delete',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Recovery',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Edit',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Save',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Vehicle Data
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('VehicleData',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'VehicleData';

	INSERT INTO MasterFunction VALUES('AddNew',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Delete',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Recovery',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Edit',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Save',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Transport Lost Damage Claim
	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('TransportLostDamageClaim',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'TransportLostDamageClaim';

	INSERT INTO MasterFunction VALUES('AddNew',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Upload',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Delete',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Recovery',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('PrintDocument',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Save',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
-- Insert Form Report TransportationSummary
	SET @idmaster = (SELECT IDFunction FROM MasterFunction WHERE FunctionName = 'Report');

	INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
	VALUES('TransportationSummary',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	
	SELECT @ParentID = IDFunction FROM MasterFunction AS mf	WHERE mf.FunctionName = 'TransportationSummary';

	INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('CustomReport',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('UpdateStatus',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Validation',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	
