IF NOT EXISTS(SELECT * FROM [dbo].[MasterRole]
			WHERE RoleName = 'ADMIN VENDOR TRANSPORT TRUCK')
BEGIN
	INSERT INTO [dbo].[MasterRole](RoleName, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES('ADMIN VENDOR TRANSPORT TRUCK', 1, 'system', GETDATE(), 'system', GETDATE())
END


IF NOT EXISTS(SELECT * FROM [dbo].[MasterRole]
			WHERE RoleName = 'ADMIN VENDOR TRANSPORT TRUCK EXECUTIVE')
BEGIN
	INSERT INTO [dbo].[MasterRole](RoleName, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES('ADMIN VENDOR TRANSPORT TRUCK EXECUTIVE', 1, 'system', GETDATE(), 'system', GETDATE())
END


IF NOT EXISTS(SELECT * FROM [dbo].[MasterRole]
			WHERE RoleName = 'ADMIN VENDOR TRANSPORT SHIP')
BEGIN
	INSERT INTO [dbo].[MasterRole](RoleName, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES('ADMIN VENDOR TRANSPORT SHIP', 1, 'system', GETDATE(), 'system', GETDATE())
END


IF NOT EXISTS(SELECT * FROM [dbo].[MasterRole]
			WHERE RoleName = 'ADMIN VENDOR TRANSPORT SHIP EXECUTIVE')
BEGIN
	INSERT INTO [dbo].[MasterRole](RoleName, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES('ADMIN VENDOR TRANSPORT SHIP EXECUTIVE', 1, 'system', GETDATE(), 'system', GETDATE())
END


IF NOT EXISTS(SELECT * FROM [dbo].[MasterRole]
			WHERE RoleName = 'ADMIN VENDOR TRANSPORT TRAIN')
BEGIN
	INSERT INTO [dbo].[MasterRole](RoleName, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES('ADMIN VENDOR TRANSPORT TRAIN', 1, 'system', GETDATE(), 'system', GETDATE())
END


IF NOT EXISTS(SELECT * FROM [dbo].[MasterRole]
			WHERE RoleName = 'ADMIN VENDOR TRANSPORT TRAIN EXECUTIVE')
BEGIN
	INSERT INTO [dbo].[MasterRole](RoleName, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES('ADMIN VENDOR TRANSPORT TRAIN EXECUTIVE', 1, 'system', GETDATE(), 'system', GETDATE())
END