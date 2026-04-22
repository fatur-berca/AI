USE TOM
IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[FnTransportationSummaryCRate]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[FnTransportationSummaryCRate]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[FnTransportationSummaryCRate]
(
	@filteryear VARCHAR(4)
)
RETURNS @temptable TABLE
(
	RoW VARCHAR(5), ValueType VARCHAR(100), Description VARCHAR(256), UoM VARCHAR(100), YTD DECIMAL(14,2),January decimal(14,2),February decimal(14,2),March decimal(14,2), April decimal(14,2), May decimal(14,2), June decimal(14,2), July decimal(14,2), August decimal(14,2), September decimal(14,2), October decimal(14,2), November decimal(14,2), December decimal(14,2)
)
AS
BEGIN
DECLARE @Today DATE, @currentmonth INT, @currentYear INT,@total VARCHAR(100)
-- ================================================================== Get 2 row of data ==============================================================================================
	INSERT INTO @temptable
	SELECT '1' as Row
	,'Crash' as ValueType
	,'Number of Crash Accident' as Description
	,'Cases' as UoM
	,0 as ytd
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
	FROM (
		SELECT ncr.TicketNumber as total, DATENAME(MONTH, DATEADD( MONTH, MONTH(ncr.CorrectiveActionDate) , 0 ) - 1 ) as [month]
		FROM TransportTicketNCR ncr 
		WHERE ncr.AccidentCategory like '%CRASH%' AND YEAR(CorrectiveActionDate) = @filteryear
		GROUP BY ncr.TicketNumber,MONTH(CorrectiveActionDate)
	)as src
	PIVOT
	(
		COUNT(total)
		for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
	)as pvt
	GROUP BY January,February,March,April,May,June,July,August,September,October,November,December
	UNION ALL
	SELECT '2' as Row
	,'Total KM Driven' as ValueType
	,'Total KM Driven' as Description
	,'KM' as UoM
	,0 as ytd
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
	FROM (
			SELECT DATENAME(MONTH, DATEADD( MONTH, MONTH(ncr.CorrectiveActionDate) , 0 ) - 1 ) as [month]
			,ISNULL(te.TotalKMBased,0) as KmDriven , te.IDVendor, te.IDTransportExecution
			FROM TransportTicketNCR ncr
			INNER JOIN MasterVendor mv ON ncr.IDVendor = mv.IDVendor
			INNER JOIN TransportExecution te ON ncr.IDVendor = te.IDVendor AND te.Month = MONTH(ncr.CorrectiveActionDate)
			WHERE ncr.AccidentCategory like '%CRASH%' AND YEAR(CorrectiveActionDate) = @filteryear
			AND te.TransportMode = 'Truck'
			GROUP BY MONTH(ncr.CorrectiveActionDate),te.TotalKMBased ,te.IDVendor, te.IDTransportExecution
	)as src
	PIVOT
	(
		SUM(KmDriven)
		for [month] in (January,February,March,April,May,June,July,August,September,October,November,December)
	)as pvt
	-- ================================================================== Get 2 row of data ==============================================================================================
	-- ================================================================== Get and Update YTD column ==============================================================================================
	DECLARE @UoM Varchar(100);
	SET @filteryear = '2018'
	SET @Today = GETDATE()
	SET @currentmonth = Month(@Today);
	SET @currentYear = YEAR(@Today);
	DECLARE cursor_data CURSOR LOCAL FOR
	SELECT UoM from @temptable
	OPEN cursor_data

	FETCH NEXT FROM cursor_data
	INTO @UoM
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF(@filterYear >= @currentYear)
		BEGIN
			IF(@currentmonth = 1)
			BEGIN
				SET @total = (SELECT SUM(January) FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 2)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 3)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 4)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0)+ ISNULL(March,0) + ISNULL(April,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 5)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 6)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 7)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 8)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 9)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) )  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 10)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 11)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0))  FROM @temptable WHERE UoM = @UoM )
			END
			ELSE IF(@currentmonth = 12)
			BEGIN
				SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0) + ISNULL(December,0))  FROM @temptable WHERE UoM = @UoM )
			END
		END
		ELSE
		BEGIN
			SET @total = (SELECT SUM(ISNULL(January,0) + ISNULL(February,0) + ISNULL(March,0) + ISNULL(April,0) + ISNULL(May,0) + ISNULL(June,0) + ISNULL(July,0) + ISNULL(August,0) + ISNULL(September,0) + ISNULL(October,0) + ISNULL(November,0) + ISNULL(December,0))  FROM @temptable WHERE UoM = @UoM )
		END

		-- =================================================== update tota(YTD) in table @temptable based on current month ===================================================
		--select @total, @IDLocation, @UoM
		UPDATE @temptable
		SET YTD = @total
		WHERE UoM = @UoM
				
				
	FETCH NEXT FROM cursor_data
	INTO @UoM;
	END
	CLOSE cursor_data;
	-- ================================================================== Get and Update YTD column ==============================================================================================
	-- ================================================================== Create Crash Rate row ==============================================================================================
	DECLARE @CJan DECIMAL(14,2), @KJan DECIMAL(14,2)
	,@CFeb DECIMAL(14,2), @KFeb DECIMAL(14,2)
	,@CMar DECIMAL(14,2), @KMar DECIMAL(14,2)
	,@CApr DECIMAL(14,2), @KApr DECIMAL(14,2)
	,@CMay DECIMAL(14,2), @KMay DECIMAL(14,2)
	,@CJun DECIMAL(14,2), @KJun DECIMAL(14,2)
	,@CJul DECIMAL(14,2), @KJul DECIMAL(14,2)
	,@CAug DECIMAL(14,2), @KAug DECIMAL(14,2)
	,@CSep DECIMAL(14,2), @KSep DECIMAL(14,2)
	,@COct DECIMAL(14,2), @KOct DECIMAL(14,2)
	,@CNov DECIMAL(14,2), @KNov DECIMAL(14,2)
	,@CDec DECIMAL(14,2), @KDec DECIMAL(14,2)
	, @CRate1 DECIMAL(14,2), @CRate2 DECIMAL(14,2)
	, @CRate3 DECIMAL(14,2), @CRate4 DECIMAL(14,2)
	, @CRate5 DECIMAL(14,2), @CRate6 DECIMAL(14,2)
	, @CRate7 DECIMAL(14,2), @CRate8 DECIMAL(14,2)
	, @CRate9 DECIMAL(14,2), @CRate10 DECIMAL(14,2)
	, @CRate11 DECIMAL(14,2), @CRate12 DECIMAL(14,2)

	SELECT @CJan = January from @temptable WHERE uom = 'Cases'
	SELECT @KJan = January from @temptable WHERE uom = 'KM'

	SELECT @CFeb = February from @temptable WHERE uom = 'Cases'
	SELECT @KFeb = February from @temptable WHERE uom = 'KM'

	SELECT @CMar = March from @temptable WHERE uom = 'Cases'
	SELECT @KMar = March from @temptable WHERE uom = 'KM'

	SELECT @CApr = April from @temptable WHERE uom = 'Cases'
	SELECT @KApr = April from @temptable WHERE uom = 'KM'

	SELECT @CMay = May from @temptable WHERE uom = 'Cases'
	SELECT @KMay = May from @temptable WHERE uom = 'KM'

	SELECT @CJun = June from @temptable WHERE uom = 'Cases'
	SELECT @KJun = June from @temptable WHERE uom = 'KM'

	SELECT @CJul = July from @temptable WHERE uom = 'Cases'
	SELECT @KJul = July from @temptable WHERE uom = 'KM'

	SELECT @CAug = August from @temptable WHERE uom = 'Cases'
	SELECT @KAug = August from @temptable WHERE uom = 'KM'

	SELECT @CSep = September from @temptable WHERE uom = 'Cases'
	SELECT @KSep = September from @temptable WHERE uom = 'KM'

	SELECT @COct = October from @temptable WHERE uom = 'Cases'
	SELECT @KOct = October from @temptable WHERE uom = 'KM'

	SELECT @CNov = November from @temptable WHERE uom = 'Cases'
	SELECT @KNov = November from @temptable WHERE uom = 'KM'

	SELECT @CDec = December from @temptable WHERE uom = 'Cases'
	SELECT @KDec = December from @temptable WHERE uom = 'KM'

	SET @CRate1 = ((@CJan * 1000000)/@KJan)
	SET @CRate2 = ((@CFeb * 1000000)/@KFeb)
	SET @CRate3 = ((@CMar * 1000000)/@KMar)
	SET @CRate4 = ((@CApr * 1000000)/@KApr)
	SET @CRate5 = ((@CMay * 1000000)/@KMay)
	SET @CRate6 = ((@CJun * 1000000)/@KJun)
	SET @CRate7 = ((@CJul * 1000000)/@KJul)
	SET @CRate8 = ((@CAug * 1000000)/@KAug)
	SET @CRate9 = ((@CSep * 1000000)/@KSep)
	SET @CRate10 = ((@COct * 1000000)/@KOct)
	SET @CRate11 = ((@CNov * 1000000)/@KNov)
	SET @CRate12 = ((@CDec * 1000000)/@KDec)

	INSERT INTO @temptable
	SELECT '3' as Row,'Crash Rate','','',null, @CRate1 , @CRate2 , @CRate3 , @CRate4 , @CRate5 , @CRate6 , @CRate7 , @CRate8 , @CRate9 , @CRate10 , @CRate11 , @CRate12
	RETURN;
END;