/* Alter TO adds GIBy, GRBy as varchar */

ALTER TABLE [TransportOrder] 
ADD GIBy varchar(50);
GO

ALTER TABLE [TransportOrder] 
ADD GRBy varchar(50);
GO