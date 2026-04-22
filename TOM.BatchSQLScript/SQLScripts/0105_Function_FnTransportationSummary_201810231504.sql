-- Author      : HAKIM
-- Create Date : 2018-10-23
-- Description : Summary Report tab TRIP

IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[FnTransportationSummary]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[FnTransportationSummary]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[FnTransportationSummary]
(
	@filterTab VARCHAR(100),
	@filterRole INT,
	@filterSort VARCHAR(100)
)
RETURNS @Ret TABLE
(
	[p1] nvarchar(100),
	[p2] nvarchar(100),
	[p3] nvarchar(100),
	Jan int,
	Feb int,
	Mar int,
	Apr int,
	May int,
	Jun int,
	Jul int,
	Aug int,
	Sep int,
	Oct int,
	Nov int,
	Dec int
)
AS
BEGIN
	--SET @filterSort = 'Vendor';
	--SET @filterSort = 'StartLocation';
	IF(@filterSort = 'Vendor')
	BEGIN
		INSERT INTO @Ret
		SELECT TransportMode, IDVendor ,VendorName, January,February,March,April,May,June,July,August,September,October,November,December FROM (
			SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
			te.TransportMode, mv.IDVendor, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total, mv.VendorName
			FROM [dbo].[TransportExecution] te
			INNER JOIN MasterVendor mv on te.IDVendor = mv.IDVendor
			GROUP BY te.TransportMode,mv.IDVendor, te.month, mv.VendorName,te.IDTransportExecution
		)as src
		PIVOT
		(
			count(total)
			for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
		)as pvt
		ORDER BY Sorting ASC
	END
	ELSE IF(@filterSort = 'StartLocation')
	BEGIN
		INSERT INTO @Ret
		SELECT TransportMode, IDLocation ,LocationName, January,February,March,April,May,June,July,August,September,October,November,December FROM (
			SELECT (CASE WHEN te.TransportMode = 'Truck' THEN 1 ELSE (CASE WHEN te.TransportMode = 'Ship' THEN 2 ELSE 3 END) END) as [Sorting],
			te.TransportMode, ml.IDLocation, DateName( month , DateAdd( month , te.[month] , 0 ) - 1 ) as [month], te.IDTransportExecution as total, ml.LocationName
			FROM [dbo].[TransportExecution] te
			INNER JOIN MasterLocation ml on te.StartLocation = ml.IDLocation COLLATE DATABASE_DEFAULT
			GROUP BY te.TransportMode,ml.IDLocation, te.month, ml.LocationName,te.IDTransportExecution
		)as src
		PIVOT
		(
			count(total)
			for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
		)as pvt
	END
RETURN;
END;