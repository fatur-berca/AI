/* Created By : Fadiyah
Date : 2018-09-26 */

/**** SUPPLIER ***/
delete from MasterList where FieldName='Supplier'
INSERT INTO MasterList VALUES('Supplier','Asia Tembakau PT',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('Supplier','HM Sampoerna PT',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('Supplier','Philip Morris Indonesia PT',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('Supplier','Yogyakarta Tembakau Indonesia, PT',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('Supplier','Persada Makmur Indonesia PT',1,'system',GETDATE(),'system',GETDATE(),'')

/**** VehicleType ***/
delete from MasterList where FieldName='VehicleType'
INSERT INTO MasterList VALUES('VehicleType','Blind Van',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','L-300',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','CDE',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Light, PT',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Light (Modif)',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Standard',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','CBU',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Cont 10FT',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Cont 20FT',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Cont 40FT HC',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Cont 20FT (Pelni)',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Standard (Roro)',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('VehicleType','Light (Roro)',1,'system',GETDATE(),'system',GETDATE(),'')

/**** UOM ****/
delete from MasterList where FieldName='UoM'
INSERT INTO MasterList VALUES('UoM','Box',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('UoM','Pack',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('UoM','Pallet',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('UoM','Sack',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('UoM','Colly',1,'system',GETDATE(),'system',GETDATE(),'')

/**** Zone ****/
delete from MasterList where FieldName='Zone'
INSERT INTO MasterList VALUES('Zone','East',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('Zone','West',1,'system',GETDATE(),'system',GETDATE(),'')

/**** MaterialType ****/
delete from MasterList where FieldName='MaterialType'
INSERT INTO MasterList VALUES('MaterialType','Cigarette',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('MaterialType','DIM',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('MaterialType','Tobacco',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('MaterialType','Clove',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('MaterialType','Bad Stock',1,'system',GETDATE(),'system',GETDATE(),'')
INSERT INTO MasterList VALUES('MaterialType','RIM',1,'system',GETDATE(),'system',GETDATE(),'')
