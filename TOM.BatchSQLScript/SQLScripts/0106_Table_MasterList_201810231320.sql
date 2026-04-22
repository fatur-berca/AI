/* Created By : Fadiyah
Date : 2018-10-23 */

DELETE from MasterList where FieldName='OrderType'
INSERT INTO MasterList VALUES('OrderType','Finished Good',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('OrderType','Raw Material',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('OrderType','Bad Stock',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('OrderType','Other',1,'system',GETDATE(),'system',GETDATE(),'')
