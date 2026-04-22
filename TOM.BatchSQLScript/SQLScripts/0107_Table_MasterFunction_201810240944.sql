/* Created By : Diyah
Date : 2018-09-24 */

INSERT INTO MasterFunction VALUES('MstUserRoleDelegation',5,'Form',1,'system',GETDATE(),'system',GETDATE(),'');

DECLARE @ParentID INT
SELECT @ParentID = IDFunction FROM MasterFunction AS mf
WHERE mf.FunctionName = 'MstUserRoleDelegation';

INSERT INTO MasterFunction VALUES('AddNew',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterFunction VALUES('SaveChanges',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterFunction VALUES('Close',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
