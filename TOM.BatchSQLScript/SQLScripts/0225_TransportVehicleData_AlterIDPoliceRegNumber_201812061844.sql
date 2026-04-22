ALTER TABLE TransportVehicleData DROP CONSTRAINT [PK__Transpor__850534FAB65F5973]; --the constraint could be varies. Kindly look at table TransportVehicleData and head into folder Keys.
ALTER TABLE TransportVehicleData ALTER COLUMN IDPoliceRegNumber varchar(15) NOT NULL;
GO
ALTER TABLE TransportVehicleData ADD PRIMARY KEY (IDPoliceRegNumber, IDVendor);