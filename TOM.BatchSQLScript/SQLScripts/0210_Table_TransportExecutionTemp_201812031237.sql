-- Created By : Fadiyah
-- Date : 2018-12-03

ALTER TABLE TransportExecutionTemp
ADD PRIMARY KEY (IDUser,IDCheck);

ALTER TABLE TransportExecutionTemp
ADD CreatedDate DATETIME NOT NULL;