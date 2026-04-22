-- Author : Hakim
-- date : 2018-11-07
-- desc : modify column idvendor on ticketncr from DFIS to TOM

ALTER TABLE TransportTicketNCR
ALTER COLUMN IDVendor INT NOT NULL