SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[UserRolesSecurityView]
AS
SELECT DISTINCT mUsr.IDUser, mUsr.FullName, mRole.IDRole, mRole.rolename,mFunc.IDFunction, mFunc.FunctionName, mFunc.Type, mFunc.ParentIDFunction
FROM            dbo.MasterUser AS mUsr INNER JOIN
                         dbo.MasterUserRoleMapping AS mUsrMap ON mUsr.IDUser = mUsrMap.IDUser INNER JOIN
                         dbo.MasterRole AS mRole ON mRole.IDRole = mUsrMap.IDRole INNER JOIN
                         dbo.MasterRolesFunctionMapping AS mRFMap ON mRFMap.IDRole = mRole.IDRole INNER JOIN
                         dbo.MasterFunction AS mFunc ON mRFMap.IDFunction = mFunc.IDFunction
						 WHERE mUsr.isactive = 1
						 AND mUsrMap.isactive =1
						 and mRole.isactive = 1
						 and mRFMap.isactive = 1
						 and mFunc.isactive = 1;


GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "mFunc"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 219
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "mRole"
            Begin Extent = 
               Top = 6
               Left = 257
               Bottom = 136
               Right = 427
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "mRFMap"
            Begin Extent = 
               Top = 6
               Left = 465
               Bottom = 136
               Right = 688
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "mUsr"
            Begin Extent = 
               Top = 6
               Left = 726
               Bottom = 136
               Right = 896
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "mUsrMap"
            Begin Extent = 
               Top = 6
               Left = 934
               Bottom = 136
               Right = 1128
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserRolesSecurityView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserRolesSecurityView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserRolesSecurityView'
GO


