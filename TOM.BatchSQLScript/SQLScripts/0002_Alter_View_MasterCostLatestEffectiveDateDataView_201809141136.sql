IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].MasterCostLatestEffectiveDateDataView'))
DROP VIEW [dbo].MasterCostLatestEffectiveDateDataView
GO
CREATE VIEW MasterCostLatestEffectiveDateDataView AS
WITH cte AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY CostType,IDVendor,VehicleType,OrderType,SenderIDLocation,ThroughIDLocation,ReceiverIDLocation,Via ORDER BY EffectiveStartDate DESC) AS rn
   FROM MasterCost
)
SELECT *
FROM cte                             
                               