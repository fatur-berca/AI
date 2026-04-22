-- author : hakim 
-- date : 2018-09-28
-- desc : create mstfunction for LoadingUnloadingHistory

--select * from masterfunction
DECLARE @dataCount INT, @id INT;
SET @dataCount = (SELECT COUNT(*) FROM masterfunction WHERE functionname = 'LoadingUnloadingHistory' AND ParentIDFunction = 3)

IF(@dataCount = 0)
BEGIN
	INSERT INTO masterfunction VALUES ('LoadingUnloadingHistory','3','Form','1','system','2018-09-20','system','2018-09-20','NULL')
	SET @id = (SELECT IDFunction FROM masterfunction WHERE functionname = 'LoadingUnloadingHistory' AND ParentIDFunction = 3 AND type = 'form')
	IF(@id != 0)
	BEGIN
		INSERT INTO masterfunction VALUES ('Upload',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
		INSERT INTO masterfunction VALUES ('Export',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
		INSERT INTO masterfunction VALUES ('CustomReport',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
		INSERT INTO masterfunction VALUES ('Edit',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
		INSERT INTO masterfunction VALUES ('Save',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
		INSERT INTO masterfunction VALUES ('Search',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
		INSERT INTO masterfunction VALUES ('Back',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
		INSERT INTO masterfunction VALUES ('Close',@id,'Button','1','system','2018-09-20','system','2018-09-20','NULL')
	END
END
