ALTER TABLE TransportExecution DROP CONSTRAINT FK_Driver1;
ALTER TABLE TransportExecution DROP CONSTRAINT FK_Driver2;
ALTER TABLE TransportExecution DROP CONSTRAINT FK_CoDriver;

ALTER TABLE TransportDriverManagement DROP CONSTRAINT PK__Transpor__2CEB98373D42B754;--nama constraint di sesuaikan db lokal masing-masing

ALTER TABLE TransportDriverManagement
ADD PRIMARY KEY (IDCardNumber,IDVendor);

EXEC sp_RENAME 'TransportVehicleData.VendorID' , 'IDVendor', 'COLUMN';

ALTER TABLE TransportExecution DROP CONSTRAINT FK__Transport__Polic__2CF2ADDF;--nama constraint di sesuaikan db lokal masing-masing

ALTER TABLE TransportVehicleData DROP CONSTRAINT PK__Transpor__BF6869FDDD69FA97;--nama constraint di sesuaikan db lokal masing-masing

DELETE FROM TransportVehicleData

ALTER TABLE TransportVehicleData
ALTER COLUMN IDVendor INT NOT NULL;

ALTER TABLE TransportVehicleData
ADD PRIMARY KEY (IDPoliceRegNumber,IDVendor);
