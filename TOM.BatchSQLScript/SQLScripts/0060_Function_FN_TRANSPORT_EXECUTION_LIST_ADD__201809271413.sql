IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'FN_TRANSPORT_EXECUTION_LIST_ADD') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION FN_TRANSPORT_EXECUTION_LIST_ADD
GO

CREATE FUNCTION [dbo].[FN_TRANSPORT_EXECUTION_LIST_ADD]
(    
    @executionType VARCHAR(50),
    @zone VARCHAR(50),
    @orderType VARCHAR(MAX),
    @week INT,
    @sender VARCHAR(500),
    @vehicleType VARCHAR(50)
)
RETURNS @result TABLE 
(
	OrderNumber VARCHAR(50),
	OrderType VARCHAR(50),
	Sender VARCHAR(50),
	Receiver VARCHAR(50),
	OrderedVehicleType VARCHAR(50),
	ShipmentDate DATE,
	MaterialType VARCHAR(50),
	[Description] VARCHAR(50),
	Qty DECIMAL(18,4),
	UoM VARCHAR(50),
	OrderRemarks VARCHAR(500),
	Sequence VARCHAR(5),
	OrderCreator VARCHAR(50),
	TransportationNumber VARCHAR(50),
	Vendor VARCHAR(50)
)
AS
BEGIN
	IF @executionType = 'ALL'
     BEGIN
	   INSERT INTO @result
	   SELECT TOP 100 to1.STONo, to1.OrderType, ml.LocationName, ml2.LocationName, to1.VehicleType,
	          to1.ShipmentDate,tod.MaterialType, tod.[Description], tod.Qty,
	          tod.UoM,to1.Remarks, to1.SeqNo, to1.CreatedBy,te.TransportNo,tod.Supplier
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN TransportExecution AS te
	   ON to1.IDTransportExecution = te.IDTransportExecution
     END
	ELSE 
     BEGIN
	   INSERT INTO @result
	   SELECT TOP 100 to1.STONo, to1.OrderType, ml.LocationName, ml2.LocationName, to1.VehicleType,
	          to1.ShipmentDate,tod.MaterialType, tod.[Description], tod.Qty,
	          tod.UoM,to1.Remarks, to1.SeqNo, to1.CreatedBy,te.TransportNo,tod.Supplier
	   FROM TransportOrderDetail AS tod
	   INNER JOIN TransportOrder AS to1
	   ON tod.IDTransportOrder = to1.IDTransportOrder
	   INNER JOIN MasterLocation AS ml
	   ON to1.SenderIDLocation = ml.IDLocation
	   INNER JOIN MasterLocation AS ml2
	   ON to1.ReceiverIDLocation = ml2.IDLocation
	   LEFT JOIN TransportExecution AS te
	   ON to1.IDTransportExecution = te.IDTransportExecution
     END
     RETURN;
END