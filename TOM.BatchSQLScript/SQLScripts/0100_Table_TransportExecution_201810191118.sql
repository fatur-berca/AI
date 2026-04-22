/* Created By : Fadiyah
Date : 2018-10-19 */

ALTER TABLE TransportExecution
DROP COLUMN KMRail, KMBased, KMSea

ALTER TABLE TransportExecution
ADD TotalKMRail decimal(10,2) NULL;

ALTER TABLE TransportExecution
ADD TotalKMBased decimal(10,2) NULL;

ALTER TABLE TransportExecution
ADD TotalKMSea decimal(10,2) NULL;