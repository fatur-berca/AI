-- rename column IDCardNumber in TransportDriverManagement to ID
EXEC sp_rename 'TransportDriverManagement.IDCardNumber', 'ID', 'COLUMN';