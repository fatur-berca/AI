/* Created By : Fadiyah
Date : 2018-10-29 */

ALTER TABLE TransportOrder
ALTER COLUMN SenderIDLocation VARCHAR(50) COLLATE Database_Default NOT NULL

ALTER TABLE TransportOrder
ALTER COLUMN ReceiverIDLocation VARCHAR(50) COLLATE Database_Default NOT NULL

ALTER TABLE TransportOrder
ALTER COLUMN ActualSenderIDLocation VARCHAR(50) COLLATE Database_Default NOT NULL

ALTER TABLE TransportOrder
ALTER COLUMN ActualReceiverIDLocation VARCHAR(50) COLLATE Database_Default NOT NULL
