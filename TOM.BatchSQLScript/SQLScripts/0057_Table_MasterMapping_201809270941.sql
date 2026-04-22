/* Created By : Fadiyah
Date : 2018-09-27 */

delete from MasterMapping where MapFrom in ('Box','Pack','Cigarette','DIM','Tobacco','Clove','Bad Stock','RIM','Pallet','Sack','Colly')

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