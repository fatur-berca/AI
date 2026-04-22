ALTER TABLE TransportOrder
  ALTER COLUMN SenderIDLocation
    VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL

ALTER TABLE TransportOrder
  ALTER COLUMN ActualSenderIDLocation
    VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL

ALTER TABLE TransportOrder
  ALTER COLUMN ReceiverIDLocation
    VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL

ALTER TABLE TransportOrder
  ALTER COLUMN ActualReceiverIDLocation
    VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL