/* Created By : Diyah
Date : 2018-11-10 */

delete from MasterList where FieldName='TransportationStatus'
INSERT INTO MasterList VALUES('TransportationStatus','Draft',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('TransportationStatus','Submit',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('TransportationStatus','In Process',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('TransportationStatus','On Delivery',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('TransportationStatus','Arrive At Destination and Waiting Confirmation',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('TransportationStatus','Complete',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('TransportationStatus','Close',1,'system',GETDATE(),'system',GETDATE(),'')