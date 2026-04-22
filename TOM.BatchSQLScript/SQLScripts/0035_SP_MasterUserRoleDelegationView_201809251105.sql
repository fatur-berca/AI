/****** Object:  View [dbo].[MasterUserRoleDelegationView]    Script Date: 9/25/2018 10:55:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- author : hakim
-- date : 2018-08-24
-- desc	: view delegation

CREATE VIEW [dbo].[MasterUserRoleDelegationView] AS 
SELECT a.IDUserDelegation, a.IDUserTo,b.FullName as UserToName,a.IDUserFrom,c.fullname as UserFromName,a.DelegationIDRole,d.RoleName,EffectiveStartDate,EffectiveEndDate,a.[IsActive]
      ,a.[CreatedBy],a.[CreatedDate],a.[UpdatedBy],a.[UpdatedDate] 
FROM MasterUserRoleDelegation a
INNER JOIN [dbo].[MasterUser] b ON b.[IDUser] = a.IDUserTo
INNER JOIN [dbo].[MasterUser] c ON c.[IDUser] = a.IDUserFrom
INNER JOIN [dbo].[MasterRole] d ON d.IDRole = a.DelegationIDRole
WHERE a.IsActive = 1
GO


