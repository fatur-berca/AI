/* Created By : Fadiyah
Date : 2018-10-29 */

ALTER VIEW [dbo].[MasterLocation] AS
SELECT A.IDLocation COLLATE Database_Default as IDLocation,
A.LocationName, A.ParentLocation, A.Type, A.IsActive, 
A.CreatedBy, A.CreatedDate, A.UpdatedBy, A.UpdatedDate, A.Remarks, A.IsRegionalOffice,
 IIF(B.IDLocation IS NULL, CAST(0 AS BIT), CAST(1 AS BIT)) AS IsAssigned
  FROM DFIS.dbo.MasterLocation AS A LEFT JOIN dbo.MasterMappingLocation AS B ON A.IDLocation = B.IDLocation