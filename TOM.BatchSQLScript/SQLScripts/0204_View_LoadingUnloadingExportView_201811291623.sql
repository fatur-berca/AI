-- Author : Hakim
-- date : 2018-11-29
CREATE VIEW [dbo].[LoadingUnloadingExportView] AS
	SELECT a.TransportNo,b.STONo,a.TransportDate,c.locationname as SenderLocationName,d.locationname as ReceiverLocationName,e.Code
	,e.Description,e.Qty,e.UoM,b.LoadStartTime,b.LoadFinishTime,b.UnloadStartTime,b.UnloadFinishTime,b.LoadBoxperWorkingTime,b.UnloadBoxperWorkingTime
	FROM TransportExecution a
	INNER JOIN TransportOrder b on a.idtransportexecution = b.idtransportexecution
	INNER JOIN MasterLocation c on b.SenderIDLocation = c.IDLocation
	INNER JOIN MasterLocation d on b.ReceiverIDLocation = d.IDLocation
	INNER JOIN TransportOrderDetail e ON b.idtransportorder = e.idtransportorder
GO