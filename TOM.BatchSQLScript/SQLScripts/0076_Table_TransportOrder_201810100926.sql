-- author : hakim
USE TOM
ALTER TABLE TransportOrder
ADD GIDate DATETIME null;
--EXEC sp_rename 'TransportOrder.GI', 'GIDate'; 