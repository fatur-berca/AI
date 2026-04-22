# TOM Project

## Project Structure
### **DFIS.Universal.BusinessLogics**
This project holds a standardized interfaces for business logics.
Please never add non-shared object here for future abstraction sake. Including a reference to EDMX!
### **DFIS.Universal.Domain**
This project holds all shared domain object of TOM and DFIS, and also future projects. Please never add non-shared object here for future abstraction sake. Including a reference to EDMX!
### **DFIS.Universal.Repositories**
This project holds a standardized interfaces for repositories.
Please never add non-shared object here for future abstraction sake. Including a reference to EDMX!

### **TOM.Master.[BusinessLogics|Domain|Repositories]**
This project holds all private TOM objects and logics that does not need to be shared to other project.

### To Do:
- Add missing tables to TOM EDMX and fix all incosistencies.

## Developer Note
> A lot of missing reference and missing Data Model, since TOM database currently missing some tables.

## Changelog
### 2018/09/21
<hr>

- Added missing tables.
- Fixed EDMX
- Added several mapping to Ninject
- Tested several modules

### 2018/09/21
<hr>

- Removed DFIS EDMX from all class libraries and also WebUI.
- Used Generics for interfaces in Universal.
- WebUI has no error (for now), need testing.

### 2018/09/20
<hr>

- Initial build, mixing and separating classes from DFIS.

## Database Log
### 2018/09/23
<hr>

- Added view: MasterRoleFunctionTreeListView
- Clone table from DFIS: TransactionLog
- Added view: [MasterCostLatestEffectiveDateDataView]

### 2018/09/21
<hr>

- Added SP: [GenTransportUnitFreq]

### 2018/09/20
<hr>

- Renamed MasterLocation to MasterMappingLocation
- Added view: MasterUserLocationMapTreeListView
- Added view: MasterLocation
- Added view: [MasterUserRoleDelegationView]
- Added view: [HRD_EMP_V2]
- Added view: [UserRolesSecurityView]
- Added function: [CheckSNTruckSeal]
- Clone Tables from DFIS: [MasterGuideline], [MasterKilometer]

### 2018/09/21
<hr>

- Clone Tables from DFIS: [CustomReportState], [GPHeader], ADSI_User, vNOTES_DATA_FLOW