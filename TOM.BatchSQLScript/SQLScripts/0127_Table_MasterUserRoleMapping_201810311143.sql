/* Created By : Diyah
Date : 2018-10-31 */

DECLARE	@idUser VARCHAR(50),
		@idRole INT
		
SET @idUser = 'pmi\pherluki';
SET @idRole = (SELECT IDRole FROM MasterRole where RoleName = 'SUPER ADMIN');
INSERT INTO MasterUserRoleMapping VALUES(@idRole, @idUser, NULL,1,'system',GETDATE(),'system',GETDATE(),'');
