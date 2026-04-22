ALTER TABLE dbo.TransportExecution
ADD CONSTRAINT FK_Driver1
FOREIGN KEY (IDCardNumberDriver1) REFERENCES TransportDriverManagement(IDCardNumber); 

ALTER TABLE dbo.TransportExecution
ADD CONSTRAINT FK_Driver2
FOREIGN KEY (IDCardNumberDriver2) REFERENCES TransportDriverManagement(IDCardNumber); 

ALTER TABLE dbo.TransportExecution
ADD CONSTRAINT FK_CoDriver
FOREIGN KEY (IDCardNumberCoDriver) REFERENCES TransportDriverManagement(IDCardNumber); 

ALTER TABLE dbo.TransportExecution
ADD Remarks VARCHAR(500)