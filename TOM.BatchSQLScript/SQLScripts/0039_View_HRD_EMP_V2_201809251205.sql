/****** Object:  View [dbo].[HRD_EMP_V2]    Script Date: 9/25/2018 11:56:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- author : hakim
-- date : 2017-11-08
-- description : add field location

CREATE VIEW [dbo].[HRD_EMP_V2] AS
SELECT ISNULL(CAST((row_number() OVER (ORDER BY vnotes.ID)) AS int), 0) as Row_ID, ad.FULL_NAME,vnotes.ID,vnotes.NAME, vnotes.TITLE_NAME, ad.EMAIL,vnotes.DIVISION_Q, vnotes.LOCATION
FROM [dbo].[ADSI_user] ad
INNER JOIN [dbo].[vNOTES_DATA_FLOW] vnotes
ON ad.ID = 'ID'+vnotes.ID where vnotes.division_Q like '%Logistics%'
GO


