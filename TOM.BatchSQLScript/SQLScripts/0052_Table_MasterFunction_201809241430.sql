INSERT INTO MasterFunction
(
	FunctionName,
	ParentIDFunction,
	[Type],
	IsActive,
	CreatedBy,
	CreatedDate,
	UpdatedBy,
	UpdatedDate,
	Remarks
)
VALUES
(
	'TransportationExecution',
	3,
	'Form',
	1,
	'system',
	GETDATE(),
	'system',
	GETDATE(),
	''
)

DECLARE @idParentFunction INT

SELECT @idParentFunction = mf.IDFunction
FROM MasterFunction AS mf WHERE mf.FunctionName = 'TransportationExecution'

INSERT INTO MasterFunction
(
	FunctionName,
	ParentIDFunction,
	[Type],
	IsActive,
	CreatedBy,
	CreatedDate,
	UpdatedBy,
	UpdatedDate,
	Remarks
)
VALUES
(
	'AddNew',
	@idParentFunction,
	'Button',
	1,
	'system',
	GETDATE(),
	'system',
	GETDATE(),
	''
)
