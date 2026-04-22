
-- author : hakim
-- date : 2018-10-15
-- desc : INSERT TABLE MasterFunction

DECLARE @temp TABLE(id int);
DECLARE @idmaster INT;
DECLARE @idtr INT;
DECLARE @idreport INT;
DECLARE @totalmodule INT;

SET @totalmodule = (SELECT COUNT(IDFunction) FROM MasterFunction WHERE Type = 'Module')

if(@totalmodule = 0)
BEGIN
	
	-- Insert Module
	INSERT INTO MasterFunction VALUES('Master',0,'Module',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Transaction',0,'Module',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('Report',0,'Module',1,'system',GETDATE(),'system',GETDATE(),'');

	-- Insert Form Master
	SET @idmaster = (SELECT IDFunction FROM MasterFunction WHERE FunctionName = 'Master');
	INSERT INTO MasterFunction VALUES('MstList',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstLocation',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstDataLoadFactorCFP',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstConfiguration',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstConfigurationEmail',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstMapping',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstRoleFunction',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstDynamicField',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstGuideline',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstUserLocationMap',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstUser',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstUserRole',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstVendorTOM',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstDistance',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstLeadTimeTom',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstUom',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstDistance',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstLeadTimeTom',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstVendorSuggestion',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstCostCenterAccount',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstServicePoTom',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstVendorSuggestion',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstCost',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('MstTruckSealStock',@idmaster,'Form',1,'system',GETDATE(),'system',GETDATE(),'');

	-- Insert Form Transaction
	SET @idtr = (SELECT IDFunction FROM MasterFunction WHERE FunctionName = 'Transaction');
	INSERT INTO MasterFunction VALUES('TransportPickingList',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('TransportationOrder',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('TransportationExecution',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('TransportationMonitoring',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('TrTransportationSeal',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('LoadingUnloadingHistory',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('TransportLostDamageClaim',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('DriverManagement',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');
	INSERT INTO MasterFunction VALUES('VehicleData',@idtr,'Form',1,'system',GETDATE(),'system',GETDATE(),'');

	-- Insert Form Report
	SET @idreport = (SELECT IDFunction FROM MasterFunction WHERE FunctionName = 'Report');
	INSERT INTO MasterFunction VALUES('TransportationSummary',@idreport,'Form',1,'system',GETDATE(),'system',GETDATE(),'');

	-- Insert Button to Every Form
	INSERT INTO @temp
	SELECT IDFunction FROM MasterFunction WHERE Type = 'Form'


	DECLARE @idfunction INT
	DECLARE cursor_mst CURSOR LOCAL FOR
	SELECT id FROM @temp
	OPEN cursor_mst

	FETCH NEXT FROM cursor_mst
	INTO @idfunction
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO MasterFunction VALUES('AddNew',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Back',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Cancel',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Close',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('CustomExport',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('CustomReport',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Delete',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Download',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Edit',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Execute',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Export',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('No',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Ok',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Print',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('PrintDocument',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('PrintProfile',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Recovery',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Reset',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Revoke',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Save',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('SaveChanges',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('SaveLayout',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Search',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('SetInactive',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Update',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('Upload',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
		INSERT INTO MasterFunction VALUES('UploadFile',@idfunction,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
	FETCH NEXT FROM cursor_mst
	INTO @idfunction;
	END

	CLOSE cursor_mst;

END







