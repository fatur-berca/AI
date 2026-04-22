-- authoor : hakim
-- date : 2018-09-25
-- desc : add new column

ALTER TABLE TransportOrder
ADD LoadBoxperWorkingTime DECIMAL(15,2) NULL;

ALTER TABLE TransportOrder
ADD UnloadBoxperWorkingTime DECIMAL(15,2) NULL;