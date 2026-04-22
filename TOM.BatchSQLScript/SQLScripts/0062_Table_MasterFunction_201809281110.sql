-- author : hakim 
-- date : 2018-09-28
-- desc : create mstfunction for Transportation Seal

--select * from masterfunction
DECLARE @dataCount INT,@buttonCount int, @id INT;
SET @dataCount = (SELECT COUNT(*) FROM masterfunction WHERE functionname = 'TrTransportationSeal' AND ParentIDFunction = 3)

IF(@dataCount = 0)
BEGIN
	INSERT INTO masterfunction VALUES ('TrTransportationSeal','3','Form','1','system','2018-09-06','system','2018-09-06','NULL')
	SET @id = (SELECT IDFunction FROM masterfunction WHERE functionname = 'TrTransportationSeal' AND ParentIDFunction = 3 AND type = 'form')
	SET @buttonCount = (SELECT COUNT(*) FROM masterfunction WHERE ParentIDFunction = @id)
	IF(@id != 0 AND @buttonCount = 0)
	BEGIN
		INSERT INTO masterfunction VALUES ('Upload',@id,'Button','1','system','2018-09-06','system','2018-09-06','NULL')
		INSERT INTO masterfunction VALUES ('CustomReport',@id,'Button','1','system','2018-09-06','system','2018-09-06','NULL')
		INSERT INTO masterfunction VALUES ('Export',@id,'Button','1','system','2018-09-06','system','2018-09-06','NULL')
		INSERT INTO masterfunction VALUES ('Back',@id,'Button','1','system','2018-09-06','system','2018-09-06','NULL')
		INSERT INTO masterfunction VALUES ('Save',@id,'Button','1','system','2018-09-06','system','2018-09-06','NULL')
		INSERT INTO masterfunction VALUES ('Edit',@id,'Button','1','system','2018-09-06','system','2018-09-06','NULL')
	END
END
