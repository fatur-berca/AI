ALTER TABLE TransportExecution
ADD IsActive BIT NOT NULL;

ALTER TABLE TransportExecution
ALTER COLUMN PoliceRegNo VARCHAR(10);

ALTER TABLE TransportExecution
ADD FOREIGN KEY (PoliceRegNo) REFERENCES TransportVehicleData(IDPoliceRegNumber);

ALTER TABLE TransportExecution
ALTER COLUMN IDCardNumberDriver1 VARCHAR(20);

ALTER TABLE TransportExecution
ALTER COLUMN IDCardNumberDriver2 VARCHAR(20);

ALTER TABLE TransportExecution
ALTER COLUMN IDCardNumberCoDriver VARCHAR(20);

ALTER TABLE TransportExecution
ADD FOREIGN KEY (IDDistanceKMBased) REFERENCES MasterDistance(IDDistance);

ALTER TABLE TransportExecution
ADD FOREIGN KEY (IDDistanceBoxTripBased) REFERENCES MasterDistance(IDDistance);

ALTER TABLE TransportExecution
ADD FOREIGN KEY (IDCost) REFERENCES MasterCost(IDCost);