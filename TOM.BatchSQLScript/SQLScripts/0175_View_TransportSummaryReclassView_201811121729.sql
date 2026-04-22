-- author : Hakim
-- date : 2018-11-12
-- desc : create view for transport summary report tab re-class
CREATE VIEW TransportSummaryReclassView
AS
	SELECT ROW_NUMBER() OVER(ORDER BY CostCenter ASC) AS [Row],CostCenter as reclass,Account, Description as Mapping, January,February,March,April,May,June,July,August,September,October,November,December, SUM(January+February+March+April+May+June+July+August+September+October+November+December) as total FROM (
		SELECT mct.Description, mct.Account, mct.CostCenter 
		, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total
		FROM [TransportExecution] te
		INNER JOIN MasterCostCenter mct ON te.IDCostCenter = mct.IDCostCenter
		WHERE [Year] = 2018 --@filteryear
		GROUP BY mct.Description, mct.Account, mct.CostCenter,te.month , te.IDTransportExecution
	)as src
	PIVOT
	(
		count(total)
		for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
	)as pvt
	GROUP BY Description,Account,CostCenter, January,February,March,April,May,June,July,August,September,October,November,December
GO