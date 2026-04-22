-- Date & Time : Thursday, 07 March 2019
-- Author	   : Farid Hartono Gunawan
-- Description : Add StartDate and End Date To MasterLoadFactorCFP
--               For Handle Effective Date Requirement

alter table MasterLoadFactorCFP
add StartDate DATE not null

alter table MasterLoadFactorCFP
add EndDate DATE not null

