-- rename column IDCardNumberDriver1 in TransportExecution to IDDriver1
EXEC sp_rename 'TransportExecution.IDCardNumberDriver1', 'IDDriver1', 'COLUMN';

-- rename column IDCardNumberDriver2 in TransportExecution to IDDriver2
EXEC sp_rename 'TransportExecution.IDCardNumberDriver2', 'IDDriver2', 'COLUMN';

-- rename column IDCardNumberCoDriver in TransportExecution to IDCoDriver
EXEC sp_rename 'TransportExecution.IDCardNumberCoDriver', 'IDCoDriver', 'COLUMN';