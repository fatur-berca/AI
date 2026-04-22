IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CalculateLoadFactorCFP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CalculateLoadFactorCFP]
GO 

CREATE PROCEDURE [dbo].[CalculateLoadFactorCFP]
	   @idTranExeParam INT
AS
BEGIN

SET FMTONLY OFF
SET NOCOUNT ON;

DECLARE @originQty DECIMAL (18,4),
	   @Liter DECIMAL(18,4),
	   @TKM DECIMAL(18,4),
	   @KGCO2 DECIMAL(18,4),
	   @AVGLoadFactor DECIMAL(18,4)

SELECT @originQty = SUM(CASE oq.Uom WHEN 'Box' THEN oq.qty * oq.StickPerBox WHEN 'Pack' THEN oq.qty * oq.StickPerPack ELSE oq.qty END)
FROM (
SELECT tod.Qty,ISNULL(mm.MapFrom,tod.UoM) AS 'UoM',mf.StickPerBox, mf.PackPerBox, mf.StickPerPack
FROM TransportOrderDetail AS tod
INNER JOIN TransportOrder AS transOrder
ON tod.IDTransportOrder = transOrder.IDTransportOrder
INNER JOIN MasterFABrand AS mf
ON tod.Code = mf.FACode
LEFT JOIN MasterMapping AS mm
ON tod.UoM = mm.MapTo
WHERE tod.IsActive = 1 AND transOrder.IsActive = 1 AND mf.IsActive = 1 AND transOrder.IDTransportExecution = @idTranExeParam) AS oq

SET @Liter = 0;
SET @TKM = 0;

SELECT @TKM = SUM(tod.Qty * mlfc.WeightperStick)
FROM TransportOrderDetail AS tod
INNER JOIN TransportOrder AS transOrder
ON tod.IDTransportOrder = transOrder.IDTransportOrder
INNER JOIN TransportExecution AS te
ON transOrder.IDTransportExecution = te.IDTransportExecution
INNER JOIN MasterFABrand AS mf
ON tod.Code = mf.FACode
INNER JOIN MasterLoadFactorCFP AS mlfc
ON te.ActualVehicleType = mlfc.VehicleType AND te.TransportMode = mlfc.Mode AND SUBSTRING(mf.SpeakingCode,1,5) = mlfc.BrandCategory
WHERE tod.IsActive = 1 AND transOrder.IsActive = 1 AND mlfc.IsActive = 1 AND te.IDTransportExecution = @idTranExeParam AND te.TransportMode = 'Vessel'

SELECT @Liter = (te.TotalKM/(SELECT TOP 1 M.[KMperLiter] FROM [dbo].[MasterLoadFactorCFP] AS M WHERE M.[VehicleType] = te.ActualVehicleType AND M.Mode = te.TransportMode AND M.IsActive = 1))
FROM TransportOrderDetail AS tod
INNER JOIN TransportOrder AS transOrder
ON tod.IDTransportOrder = transOrder.IDTransportOrder
INNER JOIN TransportExecution AS te
ON transOrder.IDTransportExecution = te.IDTransportExecution
INNER JOIN MasterFABrand AS mf
ON tod.Code = mf.FACode
INNER JOIN MasterLoadFactorCFP AS mlfc
ON te.ActualVehicleType = mlfc.VehicleType AND te.TransportMode = mlfc.Mode AND SUBSTRING(mf.SpeakingCode,1,5) = mlfc.BrandCategory
WHERE tod.IsActive = 1 AND transOrder.IsActive = 1 AND mlfc.IsActive = 1 AND te.IDTransportExecution = @idTranExeParam AND te.TransportMode <> 'Vessel'

SELECT @KGCO2 = (CASE WHEN te.TransportMode = 'Vessel' THEN SUM(tod.Qty * mlfc.WeightperStick) ELSE te.TotalKM/(SELECT TOP 1 M.[KMperLiter] FROM [dbo].[MasterLoadFactorCFP] AS M WHERE M.[VehicleType] = te.ActualVehicleType AND M.Mode = te.TransportMode AND M.IsActive = 1) END *
	  (SELECT TOP 1 O.[KgCO2perLiter] FROM [dbo].[MasterLoadFactorCFP] AS O WHERE O.[Mode] = te.TransportMode AND O.VehicleType = te.ActualVehicleType AND O.IsActive = 1))
FROM TransportOrderDetail AS tod
INNER JOIN TransportOrder AS transOrder
ON tod.IDTransportOrder = transOrder.IDTransportOrder
INNER JOIN TransportExecution AS te
ON transOrder.IDTransportExecution = te.IDTransportExecution
INNER JOIN MasterFABrand AS mf
ON tod.Code = mf.FACode
INNER JOIN MasterLoadFactorCFP AS mlfc
ON te.ActualVehicleType = mlfc.VehicleType AND te.TransportMode = mlfc.Mode AND SUBSTRING(mf.SpeakingCode,1,5) = mlfc.BrandCategory
WHERE tod.IsActive = 1 AND transOrder.IsActive = 1 AND mlfc.IsActive = 1 AND te.IDTransportExecution = @idTranExeParam
GROUP BY Te.TransportMode,te.ActualVehicleType,te.TotalKM

SELECT @AVGLoadFactor = SUM(lf.Persen)
FROM(
SELECT bq.SpeakingCode, (SUM(CASE bq.Uom WHEN 'Stick' THEN bq.qty/bq.StickPerBox WHEN 'Pack' THEN bq.qty/bq.PackPerBox ELSE bq.qty END)/mlfc.MaxQty)*100 AS 'Persen'
FROM (
SELECT transOrder.IDTransportExecution, mf.SpeakingCode, tod.Qty,ISNULL(mm.MapFrom,tod.UoM) AS 'UoM',mf.StickPerBox, mf.PackPerBox, mf.StickPerPack
FROM TransportOrderDetail AS tod
INNER JOIN TransportOrder AS transOrder
ON tod.IDTransportOrder = transOrder.IDTransportOrder
INNER JOIN MasterFABrand AS mf
ON tod.Code = mf.FACode
LEFT JOIN MasterMapping AS mm
ON tod.UoM = mm.MapTo
WHERE tod.IsActive = 1 AND transOrder.IsActive = 1 AND mf.IsActive = 1 AND transOrder.IDTransportExecution = 6) AS bq
INNER JOIN TransportExecution te
ON bq.IDTransportExecution = te.IDTransportExecution
INNER JOIN MasterLoadFactorCFP AS mlfc
ON te.ActualVehicleType = mlfc.VehicleType AND SUBSTRING(bq.SpeakingCode,1,5) = mlfc.BrandCategory AND te.TransportMode = mlfc.Mode
WHERE mlfc.IsActive = 1
GROUP BY bq.SpeakingCode,mlfc.MaxQty) AS lf

UPDATE TransportExecution
SET
	DeliveryQty = @originQty,
	CFPLiter = @Liter,
	CFPTKM = @TKM,
	KGCO2 = @KGCO2,
	AVGLoadFactor = @AVGLoadFactor
WHERE IDTransportExecution = @idTranExeParam

END