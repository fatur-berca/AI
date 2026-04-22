/* Created By : Fadiyah
Date : 2018-10-03 */

ALTER TABLE TransportExecution
ALTER COLUMN TransportMode varchar(15) NOT NULL;

ALTER TABLE TransportExecution
DROP COLUMN SerivceGRNo;

ALTER TABLE TransportExecution
ADD ServiceGRNo varchar(10) NULL;

ALTER TABLE TransportExecution
DROP COLUMN VesselName, ETD1, ETD2, ATD, ETA1, ETA2, ATA, ActualTimeBerthing, ContainerNo, ContainerSeal;

