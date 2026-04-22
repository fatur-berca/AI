
ALTER VIEW [dbo].[TransportSummaryReclassView]
AS
	SELECT ROW_NUMBER() OVER(ORDER BY CostCenter ASC) AS [Row],CostCenter as reclass,Account, Description as Mapping,Year,January,February,March,April,May,June,July,August,September,October,November,December, SUM(January+February+March+April+May+June+July+August+September+October+November+December) as total FROM (
		SELECT mct.Description, mct.Account, mct.CostCenter 
		, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total
		, te.Year
		FROM [TransportExecution] te
		INNER JOIN MasterCostCenter mct ON te.IDCostCenter = mct.IDCostCenter
		GROUP BY mct.Description, mct.Account, mct.CostCenter,te.month , te.IDTransportExecution, te.Year
	)as src
	PIVOT
	(
		count(total)
		for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
	)as pvt
	GROUP BY Description,Account,CostCenter,Year, January,February,March,April,May,June,July,August,September,October,November,December

GO


