ALTER TABLE TransportExecution
ADD IDCostCenter INT;
ALTER TABLE TransportExecution
ADD CONSTRAINT FK_TransportExecution_MasterCostCenter FOREIGN KEY (IDCostCenter) REFERENCES MasterCostCenter (IDCostCenter);
ALTER TABLE TransportExecution
ALTER COLUMN ActualCostCenter VARCHAR(50);