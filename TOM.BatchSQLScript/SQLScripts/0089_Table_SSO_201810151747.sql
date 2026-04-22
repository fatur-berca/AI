
-- author : hakim
-- date : 2018
-- desc : reset identity table SSO

DELETE FROM MasterRolesFunctionMapping
DELETE FROM MasterUserRoleMapping
DELETE FROM MasterRole
DELETE FROM MasterFunction

DBCC CHECKIDENT ('MasterRolesFunctionMapping', RESEED, 0); 
DBCC CHECKIDENT ('MasterUserRoleMapping', RESEED, 0); 
DBCC CHECKIDENT ('MasterRole', RESEED, 0); 
DBCC CHECKIDENT ('MasterFunction', RESEED, 0); 