ALTER TABLE TransportOrderRequest
ALTER COLUMN UpdatedDate DATETIME NOT NULL

ALTER TABLE TransportOrderRequest
ADD IsActive BIT NOT NULL
