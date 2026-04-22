/* Created By : Fadiyah
Date : 2018-10-10 */

CREATE VIEW GeneralBuildingFacility AS
SELECT A.IDLocation COLLATE Database_Default as IDLocation, A.WarehouseAddress COLLATE Database_Default as WarehouseAddress
FROM DFIS.dbo.GeneralBuildingFacility as A