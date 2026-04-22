/****** Object:  View [dbo].[MasterRoleFunctionTreeListView]    Script Date: 9/25/2018 10:53:39 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- author : hakim
-- date : 2018-03-09
-- desc : add isactive funcpar3

CREATE VIEW [dbo].[MasterRoleFunctionTreeListView] AS
SELECT ROW_NUMBER() OVER( ORDER BY funcpar.[IDFunction] ) AS id
	  ,funcpar.[IDFunction] AS IDModule
      ,funcpar.[FunctionName] AS NameModule
      ,funcpar2.[IDFunction] AS IDForm
      ,funcpar2.[FunctionName] AS NameForm
      ,funcpar3.[IDFunction] AS IDButton
      ,funcpar3.[FunctionName] AS NameButton
  FROM MasterFunction AS funcpar
  LEFT JOIN MasterFunction AS funcpar2
  ON funcpar.IDFunction = funcpar2.ParentIDFunction
  LEFT JOIN MasterFunction AS funcpar3
  ON funcpar2.IDFunction = funcpar3.ParentIDFunction
WHERE funcpar.[Type] = 'Module' AND funcpar.IsActive = '1' AND funcpar2.IsActive = '1' AND funcpar3.IsActive = '1';
GO


