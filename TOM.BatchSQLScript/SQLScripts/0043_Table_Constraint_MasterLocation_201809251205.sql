DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += N'
ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id))
    + '.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) + 
    ' DROP CONSTRAINT ' + QUOTENAME(fk.name) + ';'
FROM sys.foreign_keys AS fk INNER JOIN sys.tables AS t ON t.object_id = fk.referenced_object_id
WHERE t.name = 'MasterLocation'

EXEC sp_executesql @sql;