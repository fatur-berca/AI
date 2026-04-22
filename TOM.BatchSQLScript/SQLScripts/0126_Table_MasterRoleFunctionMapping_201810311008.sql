/* Created By : Diyah
Date : 2018-10-31 */

-- Mapping Role Super Admin

DECLARE @temp TABLE(id INT);
DECLARE	@idFunction INT,
		@idRole INT

SET @idRole = (SELECT IDRole FROM MasterRole WHERE RoleName = 'SUPER ADMIN')

INSERT INTO @temp
	SELECT IDFunction FROM MasterFunction order by CreatedDate
	
	DECLARE cursor_mst CURSOR LOCAL FOR
	SELECT id FROM @temp
	OPEN cursor_mst

	FETCH NEXT FROM cursor_mst
	INTO @idFunction
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO MasterRolesFunctionMapping VALUES(@idRole,@idFunction,1,'system',GETDATE(),'system',GETDATE(),'');		
	FETCH NEXT FROM cursor_mst
	INTO @idFunction;
	END
	CLOSE cursor_mst;



