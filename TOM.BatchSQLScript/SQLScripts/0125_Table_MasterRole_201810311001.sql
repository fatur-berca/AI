/* Created By : Diyah
Date : 2018-10-31 */

TRUNCATE TABLE MasterUserRoleMapping
TRUNCATE TABLE UserLocationDelegation
TRUNCATE TABLE MasterRolesFunctionMapping
DELETE FROM MasterUserRoleDelegation
DBCC CHECKIDENT ('MasterUserRoleDelegation', RESEED, 0)
DELETE FROM MasterRole
DBCC CHECKIDENT ('MasterRole', RESEED, 0)

INSERT INTO MasterRole VALUES('SUPER ADMIN',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterRole VALUES('ADMIN WAREHOUSE',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterRole VALUES('ADMIN TRANSPORT',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterRole VALUES('CUSTOMER',1,'system',GETDATE(),'system',GETDATE(),'');



