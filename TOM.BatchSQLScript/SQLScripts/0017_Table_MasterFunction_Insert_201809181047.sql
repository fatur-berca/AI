/* Created By : Fadiyah
Date : 2018-09-07 10:59 */

INSERT INTO MasterFunction(FunctionName,ParentIDFunction,[Type],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Remarks)
VALUES('TransportationOrder',3,'Form',1,'system',GETDATE(),'system',GETDATE(),'');

DECLARE @ParentID INT
SELECT @ParentID = IDFunction FROM MasterFunction AS mf
WHERE mf.FunctionName = 'TransportationOrder';

INSERT INTO MasterFunction VALUES('AddNew',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterFunction VALUES('Delete',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterFunction VALUES('Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterFunction VALUES('Custom Export',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
