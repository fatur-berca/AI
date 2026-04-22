/****** Object:  StoredProcedure [dbo].[GenTransportUnitFreq]    Script Date: 9/25/2018 11:53:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Arman Budi Mahendra
-- Create date: 07-12-2017 12:20
-- Description:	Modify SP For View Unit Freq
-- =============================================
CREATE PROCEDURE [dbo].[GenTransportUnitFreq]
	-- Add the parameters for the stored procedure here
@day int,
@month int,
@year int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select a.policenumber, a.VehicleType, a.Vendor, 
(select count(PoliceNumber) from TransportUnitFrequentData where month(gpdate) = @month and year(gpdate) = @year And day(gpdate) = @day and Vendor = a.Vendor and VehicleType = a.VehicleType) '1',
(select count(PoliceNumber) from TransportUnitFrequentData where  month(gpdate) = @month and year(gpdate) = @year And day(gpdate) = @day-1  and Vendor = a.Vendor and VehicleType = a.VehicleType) '2',
(select count(PoliceNumber) from TransportUnitFrequentData where  month(gpdate) = @month and year(gpdate) = @year And day(gpdate) = @day-2  and Vendor = a.Vendor and VehicleType = a.VehicleType) '3',
(select count(PoliceNumber) from TransportUnitFrequentData where  month(gpdate) = @month and year(gpdate) = @year And day(gpdate) = @day-3 and Vendor = a.Vendor and VehicleType = a.VehicleType) '4',
(select count(PoliceNumber) from TransportUnitFrequentData where  month(gpdate) = @month and year(gpdate) = @year And day(gpdate) = @day-4 and Vendor = a.Vendor and VehicleType = a.VehicleType) '5',
(select count(PoliceNumber) from TransportUnitFrequentData where  month(gpdate) = @month and year(gpdate) = @year And day(gpdate) = @day-5 and Vendor = a.Vendor and VehicleType = a.VehicleType) '6',
(select count(PoliceNumber) from TransportUnitFrequentData where  month(gpdate) = @month and year(gpdate) = @year And day(gpdate) = @day-6 and Vendor = a.Vendor and VehicleType = a.VehicleType) '7',
b.IDPoliceRegNumber
from TransportUnitFrequentData a
LEFT JOIN TransportVehicleData b ON a.PoliceNumber = b.IDPoliceRegNumber
group by a.policenumber, a.VehicleType, a.vendor, b.IDPoliceRegNumber
END

GO


