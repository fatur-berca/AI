
-- author : hakim
-- date : 2018-10-15
-- desc : INSERT TABLE MasterRolesFunctionMapping

DECLARE @temp TABLE (idfunction INT);

INSERT INTO @temp
SELECT IDFunction FROM MasterFunction Where Type != 'Module'

DECLARE @idfunction INT
DECLARE cursor_mst CURSOR LOCAL FOR
SELECT idfunction FROM @temp
OPEN cursor_mst

FETCH NEXT FROM cursor_mst
INTO @idfunction
WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO MasterRolesFunctionMapping VALUES(1,@idfunction,1,'system',GETDATE(),'system',GETDATE(),'');
FETCH NEXT FROM cursor_mst
INTO @idfunction;
END

CLOSE cursor_mst;
