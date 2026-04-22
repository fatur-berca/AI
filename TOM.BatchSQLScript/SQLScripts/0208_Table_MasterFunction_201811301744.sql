-- Created By : Diyah
-- Date : 2018-11-30

DECLARE @ParentID INT
SELECT @ParentID = IDFunction FROM MasterFunction AS mf
WHERE mf.FunctionName = 'TransportationOrder';

INSERT INTO MasterFunction VALUES('Upload',@ParentID,'Button',1,'system',GETDATE(),'system',GETDATE(),'');
