ALTER TABLE TransportOrder
ADD CONSTRAINT FK_TransportOrder_MasterCostCenter FOREIGN KEY (IDCostCenter) REFERENCES MasterCostCenter (IDCostCenter)