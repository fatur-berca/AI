ALTER TABLE TransportExecution
ADD BasedCost DECIMAL(18,4);

ALTER TABLE TransportExecution
ADD DiscountCost DECIMAL(18,4);

ALTER TABLE TransportExecution
ADD TotalCost DECIMAL(18,4);