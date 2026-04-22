/* Created By : Fadiyah
Date : 2018-09-26 */

delete from MasterMapping where MapFrom in ('Cigarette','Tobacco')
/** Mapping Picking List **/
INSERT INTO MasterMapping VALUES('Box','CS',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Pack','PAC',1,'system',GETDATE(),'system',GETDATE(),'');

/** Mapping TO **/
INSERT INTO MasterMapping VALUES('Cigarette','Finished Good',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('DIM','Raw Material',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Tobacco','Raw Material',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Clove','Raw Material',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Bad Stock','Bad Stock',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('RIM','Other',1,'system',GETDATE(),'system',GETDATE(),'');

INSERT INTO MasterMapping VALUES('Box','Cigarette',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Pack','Cigarette',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Pallet','DIM',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Box','DIM',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Box C48','Tobacco',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Sack','Clove',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Box','Bad Stock',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Colly','Bad Stock',1,'system',GETDATE(),'system',GETDATE(),'');
INSERT INTO MasterMapping VALUES('Colly','RIM',1,'system',GETDATE(),'system',GETDATE(),'');
