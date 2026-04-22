-- Author      : HAKIM
-- Create Date : 2018-11-05
-- Description : add return tab

IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[FnTransportationSummary]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[FnTransportationSummary]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[FnTransportationSummary]
(
	@filterTab VARCHAR(100),
	@filterRole VARCHAR(100),
	@filterSort VARCHAR(100),
	@filteryear VARCHAR(4)
)
RETURNS  @Ret TABLE
(
	[p1] nvarchar(100),
	[p2] nvarchar(100),
	[p3] nvarchar(100),
	[p4] nvarchar(100),
	val nvarchar(100),
	res nvarchar(100)
)
AS
BEGIN
--SET @filterTab = 'Trip'
--SET @filterRole = 'ALL'
--SET @filterSort = 'StartLocation'

--SET @filterTab = 'COST'
--SET @filterRole = 'ADMIN TRANSPORT'
--SET @filterSort = 'VENDOR'

--SET @filterTab = 'Trip'
--SET @filterRole = 'ADMIN TRANSPORT'
--SET @filterSort = 'Vendor'

--SET @filterTab = 'Cost'
--SET @filterRole = 'ALL'
--SET @filterSort = 'Vendor'

--SET @filterTab = 'Cost'
--SET @filterRole = 'ALL'
--SET @filterSort = 'StartLocation'
SET @filterTab = UPPER(@filterTab);
SET @filterRole = UPPER(@filterRole);
SET @filterSort = UPPER(@filterSort);

	IF(@filterSort = 'VENDOR') -- lvl 1 filter by sort
	BEGIN 
		-- =================================================================================== FILTER BY TAB : BEGIN * TAB TRIP * =================================================================================
		-- lvl 2 filter by role
		IF(@filterRole = 'ADMIN WAREHOUSE')
		BEGIN
			
			-- lvl 3 filter by role
			IF(@filterTab = 'TRIP') -- 1
			BEGIN
				-- TAB TRIP 
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December, SUM(January+February+March+April+May+June+July+August+September+October+November+December) as total FROM (
						SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
						te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total, mv.VendorName
						FROM [dbo].[TransportExecution] te
						INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
						INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
						INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
						WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
						GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.IDTransportExecution
					)as src
					PIVOT
					(
						count(total)
						for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
					)as pvt
					GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
				-- TAB TRIP 
			END
			ELSE IF(@filterTab = 'COST') -- 2
			BEGIN
			--	-- TAB COST 
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.BasedCost as total, mv.VendorName--,te.IDTransportExecution
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.BasedCost
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			--	-- TAB COST 
			END
			ELSE IF(@filterTab = 'KMDRIVEN') -- 3
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKMBased as total, mv.VendorName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.TotalKMBased
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			ELSE IF(@filterTab = 'KMTOTAL') -- 4
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKM as total, mv.VendorName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.TotalKM
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			--ELSE IF(@filterTab = 'STICK') -- 5
			--BEGIN
				
			--END
			
		END
		ELSE -- ALL
		BEGIN
			IF(@filterTab = 'TRIP') -- 1
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December, SUM(January+February+March+April+May+June+July+August+September+October+November+December) as total FROM (
						SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
						te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total, mv.VendorName
						FROM [dbo].[TransportExecution] te
						INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
						INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
						INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
						WHERE te.[Year] = @filteryear
						GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.IDTransportExecution
					)as src
					PIVOT
					(
						count(total)
						for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
					)as pvt
					GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			ELSE IF(@filterTab = 'COST') -- 2
			BEGIN
				-- TAB COST 
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.BasedCost as total, mv.VendorName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							WHERE te.[Year] = @filteryear
							GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.BasedCost
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
				-- TAB COST 

			END
			ELSE IF(@filterTab = 'KMDRIVEN') -- 3
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKMBased as total, mv.VendorName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							WHERE te.[Year] = @filteryear
							GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.TotalKMBased
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			ELSE IF(@filterTab = 'KMTOTAL') -- 4
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDVendor ,VendorName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDVendor ,VendorName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKM as total, mv.VendorName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							WHERE te.[Year] = @filteryear
							GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.TotalKM
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			--ELSE IF(@filterTab = 'STICK') -- 5
			--BEGIN
				
			--END
		END
		
	END
	ELSE IF(@filterSort = 'STARTLOCATION')
	BEGIN
		IF(@filterRole ='ADMIN WAREHOUSE')
		BEGIN
			IF(@filterTab = 'TRIP') -- 1
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December, SUM(January+February+March+April+May+June+July+August+September+October+November+December) as total FROM (
						SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
						te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total, ml.LocationName
						FROM [dbo].[TransportExecution] te
						INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
						INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
						INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
						INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
						WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
						GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.IDTransportExecution
					)as src
					PIVOT
					(
						count(total)
						for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
					)as pvt
					GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			ELSE IF(@filterTab = 'COST') -- 2
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDLocation ,LocationName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.BasedCost as total, ml.LocationName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.MapFrom = 'ADMIN WAREHOUSE' --@filterRole
							GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.BasedCost
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			ELSE IF(@filterTab = 'KMDRIVEN') -- 3
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDLocation ,LocationName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKMBased as total, ml.LocationName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.MapFrom = 'ADMIN WAREHOUSE' --@filterRole
							GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.TotalKMBased
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			ELSE IF(@filterTab = 'KMTOTAL') -- 4
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDLocation ,LocationName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKM as total, ml.LocationName
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.MapFrom = 'ADMIN WAREHOUSE' --@filterRole
							GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.TotalKM
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
		END
		ELSE
		BEGIN
			IF(@filterTab = 'TRIP') -- 1
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December, SUM(January+February+March+April+May+June+July+August+September+October+November+December) as total FROM (
						SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
						te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total, ml.LocationName
						FROM [dbo].[TransportExecution] te
						INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
						WHERE te.[Year] = @filteryear
						GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.IDTransportExecution
					)as src
					PIVOT
					(
						count(total)
						for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
					)as pvt
					GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			--ELSE IF(@filterTab = 'COST') -- 2
			--BEGIN
			--	-- TAB COST 

			--	-- TAB COST 

			--END
			ELSE IF(@filterTab = 'KMDRIVEN') -- 3
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDLocation ,LocationName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKMBased as total, ml.LocationName ,te.IDTransportExecution
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							WHERE te.[Year] = @filteryear
							GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.TotalKMBased ,te.IDTransportExecution
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
			ELSE IF(@filterTab = 'KMTOTAL') -- 4
			BEGIN
				INSERT INTO @Ret
				SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
					SELECT [Sorting],TransportMode, IDLocation ,LocationName
						,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
						,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
						,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
						,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
						,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
						,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
						,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
						,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
						,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
						,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
						,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
						,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
						,CASE WHEN SUM(
								  (CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) is null 
							THEN 0 ELSE 
							SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
								+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
								+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
								+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
								+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
								+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
								+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
								+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
								+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
								+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
								+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
								+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
							) END as total 
						FROM (
							SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
							te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.TotalKM as total, ml.LocationName ,te.IDTransportExecution
							FROM [dbo].[TransportExecution] te
							INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
							INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
							INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
							INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
							--WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
							WHERE te.[Year] = @filteryear
							GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.TotalKM ,te.IDTransportExecution
						)as src
						PIVOT
						(
							sum(total)
							for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
						)as pvt
						GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
				) piv
				UNPIVOT
				(
					val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
				) UNPIV;
			END
		END
	END
	

	IF(@filterTab = 'CARBONFOOTPRINT')
	BEGIN
		-- =================================================== declare variable ===================================================
		--DECLARE @filterYear INT;
		DECLARE @Today DATE, @currentmonth INT, @currentYear INT,@total VARCHAR(100)
		DECLARE @sorting INT, @TransportMode Varchar(100), @IDLocation VARCHAR(10), @LocationName Varchar(100) -- declare loop variable
		DECLARE @temp2 TABLE(Sorting INT, TransportMode Varchar(100), IDLocation VARCHAR(10), LocationName Varchar(100),
		January decimal(14,2),February decimal(14,2),March decimal(14,2), April decimal(14,2), May decimal(14,2), June decimal(14,2), July decimal(14,2), August decimal(14,2), September decimal(14,2), October decimal(14,2), November decimal(14,2), December decimal(14,2),  total decimal(14,2))

		-- =================================================== set variable ===================================================
		--SET @filterYear = 2017;
		SET @Today = GETDATE()
		SET @currentmonth = Month(@Today);
		SET @currentYear = YEAR(@Today);
		-- =================================================== insert tabel temp2 ===================================================
		IF(@filterRole = 'ADMIN WAREHOUSE')
		BEGIN
			INSERT INTO @temp2
			SELECT [Sorting],TransportMode, IDLocation ,LocationName
			,SUM(ISNULL(January,0)) as January
			,SUM(ISNULL(February,0)) as February
			,SUM(ISNULL(March,0)) as March
			,SUM(ISNULL(April,0)) as April
			,SUM(ISNULL(May,0)) as May
			,SUM(ISNULL(June,0)) as June
			,SUM(ISNULL(July,0)) as July
			,SUM(ISNULL(August,0)) as August
			,SUM(ISNULL(September,0)) as September
			,SUM(ISNULL(October,0)) as October
			,SUM(ISNULL(November,0)) as November
			,SUM(ISNULL(December,0)) as December
			, 0 as total
			FROM (
				SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
				te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.KGCO2 as total, ml.LocationName
				FROM [dbo].[TransportExecution] te
				INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
				INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
				INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
				INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
				WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
				GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.KGCO2
			)as src
			PIVOT
			(
				sum(total)
				for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
			)as pvt
			GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
			-- =================================================== loop for total(YTD) ===================================================

			
			DECLARE cursor_data CURSOR LOCAL FOR
			SELECT sorting,TransportMode, IDLocation,LocationName from @temp2
			OPEN cursor_data

			FETCH NEXT FROM cursor_data
			INTO @sorting , @TransportMode , @IDLocation , @LocationName 
			WHILE @@FETCH_STATUS = 0
			BEGIN
				
				IF(@filterYear >= @currentYear)
				BEGIN
					IF(@currentmonth = 1)
					BEGIN
						SET @total = (SELECT SUM(January) FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 2)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 3)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 4)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0)+ ISNULL(March,0) + ISNULL(April,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 5)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 6)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 7)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 8)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 9)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) )  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 10)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 11)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 12)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0) + ISNULL(December,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
				END
				ELSE
				BEGIN
					SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0) + ISNULL(December,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
				END

				-- =================================================== update tota(YTD) in table @temp2 based on current month ===================================================
				--select @total, @IDLocation, @TransportMode
				UPDATE @temp2
				SET total = @total
				WHERE IDLocation = @IDLocation
				AND TransportMode = @TransportMode
				
				
			FETCH NEXT FROM cursor_data
			INTO @sorting , @TransportMode , @IDLocation , @LocationName;
			END
			CLOSE cursor_data;

			INSERT INTO @Ret
			SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
				SELECT [Sorting],TransportMode, IDLocation ,LocationName
				, CONVERT(VARCHAR(100),total) as total 
				,CONVERT(VARCHAR(100),January) as January
				,CONVERT(VARCHAR(100),February) as February
				,CONVERT(VARCHAR(100),March) as March
				,CONVERT(VARCHAR(100),April) as April
				,CONVERT(VARCHAR(100),May) as May
				,CONVERT(VARCHAR(100),June) as June
				,CONVERT(VARCHAR(100),July) as July
				,CONVERT(VARCHAR(100),August) as August
				,CONVERT(VARCHAR(100),September) as September
				,CONVERT(VARCHAR(100),October) as October
				,CONVERT(VARCHAR(100),November) as November
				,CONVERT(VARCHAR(100),December) as December
				FROM @temp2
			) piv
			UNPIVOT
			(
				val for [res] in (total,January,February,March,April,May,June,July,August,September,October,November,December)
			) UNPIV;
		END
		ELSE
		BEGIN
			INSERT INTO @temp2
			SELECT [Sorting],TransportMode, IDLocation ,LocationName
			,SUM(ISNULL(January,0)) as January
			,SUM(ISNULL(February,0)) as February
			,SUM(ISNULL(March,0)) as March
			,SUM(ISNULL(April,0)) as April
			,SUM(ISNULL(May,0)) as May
			,SUM(ISNULL(June,0)) as June
			,SUM(ISNULL(July,0)) as July
			,SUM(ISNULL(August,0)) as August
			,SUM(ISNULL(September,0)) as September
			,SUM(ISNULL(October,0)) as October
			,SUM(ISNULL(November,0)) as November
			,SUM(ISNULL(December,0)) as December
			, 0 as total
			FROM (
				SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
				te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.KGCO2 as total, ml.LocationName
				FROM [dbo].[TransportExecution] te
				INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
				INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
				INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
				INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
				WHERE te.[Year] = @filteryear
				GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.KGCO2
			)as src
			PIVOT
			(
				sum(total)
				for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
			)as pvt
			GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
			-- =================================================== loop for total(YTD) ===================================================

			--DECLARE @sorting INT, @TransportMode Varchar(100), @IDLocation VARCHAR(10), @LocationName Varchar(100) -- declare loop variable
			DECLARE cursor_data CURSOR LOCAL FOR
			SELECT sorting,TransportMode, IDLocation,LocationName from @temp2
			OPEN cursor_data

			FETCH NEXT FROM cursor_data
			INTO @sorting , @TransportMode , @IDLocation , @LocationName 
			WHILE @@FETCH_STATUS = 0
			BEGIN
				IF(@filterYear >= @currentYear)
				BEGIN
					IF(@currentmonth = 1)
					BEGIN
						SET @total = (SELECT SUM(January) FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 2)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 3)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 4)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0)+ ISNULL(March,0) + ISNULL(April,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 5)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 6)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 7)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 8)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 9)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) )  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 10)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 11)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
					ELSE IF(@currentmonth = 12)
					BEGIN
						SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0) + ISNULL(December,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
					END
				END
				ELSE
				BEGIN
					SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0) + ISNULL(December,0))  FROM @temp2 WHERE TransportMode = @TransportMode AND IDLocation = @IDLocation)
				END

				-- =================================================== update tota(YTD) in table @temp2 based on current month ===================================================
				--select @total, @IDLocation, @TransportMode
				UPDATE @temp2
				SET total = @total
				WHERE IDLocation = @IDLocation
				AND TransportMode = @TransportMode
				
				
			FETCH NEXT FROM cursor_data
			INTO @sorting , @TransportMode , @IDLocation , @LocationName;
			END
			CLOSE cursor_data;

			INSERT INTO @Ret
			SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
				SELECT [Sorting],TransportMode, IDLocation ,LocationName
				, CONVERT(VARCHAR(100),total) as total 
				,CONVERT(VARCHAR(100),January) as January
				,CONVERT(VARCHAR(100),February) as February
				,CONVERT(VARCHAR(100),March) as March
				,CONVERT(VARCHAR(100),April) as April
				,CONVERT(VARCHAR(100),May) as May
				,CONVERT(VARCHAR(100),June) as June
				,CONVERT(VARCHAR(100),July) as July
				,CONVERT(VARCHAR(100),August) as August
				,CONVERT(VARCHAR(100),September) as September
				,CONVERT(VARCHAR(100),October) as October
				,CONVERT(VARCHAR(100),November) as November
				,CONVERT(VARCHAR(100),December) as December
				FROM @temp2
			) piv
			UNPIVOT
			(
				val for [res] in (total,January,February,March,April,May,June,July,August,September,October,November,December)
			) UNPIV;
		END
		
		
	END

	IF((@filterTab = 'ASDPNSPSI') OR (@filterTab = 'SPSI')) -- 
	BEGIN
		IF(@filterRole = 'ADMIN WAREHOUSE')
		BEGIN
			INSERT INTO @Ret
			SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
				SELECT [Sorting],TransportMode, IDLocation ,LocationName
					,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
					,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
					,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
					,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
					,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
					,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
					,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
					,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
					,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
					,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
					,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
					,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
					,CASE WHEN SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
							+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
							+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
							+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
							+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
							+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
							+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
							+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
							+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
							+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
							+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
							+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
						) is null 
						THEN 0 ELSE 
						SUM(
							(CASE WHEN January IS NULL THEN 0 ELSE January END)
							+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
							+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
							+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
							+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
							+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
							+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
							+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
							+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
							+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
							+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
							+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
						) END as total 
					FROM (
						SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
						te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], CASE WHEN @filterSort = 'ASDP' THEN te.ASDPCost ELSE te.SPSICost END as total, ml.LocationName ,te.IDTransportExecution
						FROM [dbo].[TransportExecution] te
						INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
						INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
						INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
						INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
						WHERE mm.[Description] = @filterRole AND te.[Year] = @filteryear
						GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,CASE WHEN @filterSort = 'ASDP' THEN te.ASDPCost ELSE te.SPSICost END ,te.IDTransportExecution
					)as src
					PIVOT
					(
						sum(total)
						for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
					)as pvt
					GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
			) piv
			UNPIVOT
			(
				val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
			) UNPIV;
		END
		ELSE
		BEGIN
			INSERT INTO @Ret
			SELECT [Sorting],TransportMode, IDLocation ,LocationName,REPLACE(val, '.',',') as val,  [res] FROM (
				SELECT [Sorting],TransportMode, IDLocation ,LocationName
					,CASE WHEN SUM(January) IS NULL THEN 0 ELSE SUM(January) END as January
					,CASE WHEN SUM(February) IS NULL THEN 0 ELSE SUM(February) END as February
					,CASE WHEN SUM(March) IS NULL THEN 0 ELSE SUM(March) END as March
					,CASE WHEN SUM(April) IS NULL THEN 0 ELSE SUM(April) END as April
					,CASE WHEN SUM(May) IS NULL THEN 0 ELSE SUM(May) END as May
					,CASE WHEN SUM(June) IS NULL THEN 0 ELSE SUM(June) END as June
					,CASE WHEN SUM(July) IS NULL THEN 0 ELSE SUM(July) END as July
					,CASE WHEN SUM(August) IS NULL THEN 0 ELSE SUM(August) END as August
					,CASE WHEN SUM(September) IS NULL THEN 0 ELSE SUM(September) END as September
					,CASE WHEN SUM(October) IS NULL THEN 0 ELSE SUM(October) END as October
					,CASE WHEN SUM(November) IS NULL THEN 0 ELSE SUM(November) END as November
					,CASE WHEN SUM(December) IS NULL THEN 0 ELSE SUM(December) END as December
					,CASE WHEN SUM(
								(CASE WHEN January IS NULL THEN 0 ELSE January END)
							+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
							+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
							+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
							+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
							+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
							+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
							+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
							+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
							+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
							+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
							+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
						) is null 
						THEN 0 ELSE 
						SUM(
							(CASE WHEN January IS NULL THEN 0 ELSE January END)
							+ (CASE WHEN February IS NULL THEN 0 ELSE February END)
							+ (CASE WHEN March IS NULL THEN 0 ELSE March END)
							+ (CASE WHEN April IS NULL THEN 0 ELSE April END)
							+ (CASE WHEN May IS NULL THEN 0 ELSE May END)
							+ (CASE WHEN June IS NULL THEN 0 ELSE June END)
							+ (CASE WHEN July IS NULL THEN 0 ELSE July END)
							+ (CASE WHEN August IS NULL THEN 0 ELSE August END)
							+ (CASE WHEN September IS NULL THEN 0 ELSE September END)
							+ (CASE WHEN October IS NULL THEN 0 ELSE October END)
							+ (CASE WHEN November IS NULL THEN 0 ELSE November END)
							+ (CASE WHEN December IS NULL THEN 0 ELSE December END)
						) END as total 
					FROM (
						SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
						te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], CASE WHEN @filterSort = 'ASDP' THEN te.ASDPCost ELSE te.SPSICost END as total, ml.LocationName ,te.IDTransportExecution
						FROM [dbo].[TransportExecution] te
						INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
						INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
						INNER JOIN MasterConfiguration mm on mv.VendorName = mm.Value
						INNER JOIN MasterRole mr on mr.RoleName = mm.[Description]
						WHERE te.[Year] = @filteryear
						GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,CASE WHEN @filterSort = 'ASDP' THEN te.ASDPCost ELSE te.SPSICost END ,te.IDTransportExecution
					)as src
					PIVOT
					(
						sum(total)
						for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
					)as pvt
					GROUP BY [Sorting],TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December
			) piv
			UNPIVOT
			(
				val for [res] in (January,February,March,April,May,June,July,August,September,October,November,December, total)
			) UNPIV;
		END
	END
	RETURN;
END;