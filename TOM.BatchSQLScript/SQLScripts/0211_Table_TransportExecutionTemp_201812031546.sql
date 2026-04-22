-- Created By : Fadiyah
-- Date : 2018-12-03

ALTER TABLE TransportExecutionTemp 
DROP CONSTRAINT PK__Transpor__AFE9E217409A21F6;

ALTER TABLE TransportExecutionTemp
ALTER COLUMN IDTransportOrderDetail INT NOT NULL;

ALTER TABLE TransportExecutionTemp
ADD PRIMARY KEY (IDUser,IDCheck,IDTransportOrder,IDTransportOrderDetail);