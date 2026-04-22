DECLARE @idFunction INT

SELECT @idFunction = mf.IDFunction
FROM MasterFunction AS mf
WHERE mf.FunctionName = 'TransportPickingList'

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
	'Edit',
	@idFunction,
	'Button',
	1,
	'system',
	GETDATE(),
	'system',
	GETDATE(),
	''
);


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
	'Save',
	@idFunction,
	'Button',
	1,
	'system',
	GETDATE(),
	'system',
	GETDATE(),
	''
);