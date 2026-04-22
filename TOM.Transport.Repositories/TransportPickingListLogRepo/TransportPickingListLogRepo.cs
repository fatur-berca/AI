using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL.EDMX;
using TOM.EntitiesDAL;

namespace TOM.Transport.Repositories
{
    public class TransportPickingListLogRepo : TOMGenericRepository<TransportPickingListLog>, ITransportPickingListLogRepo
    {

        public TransportPickingListLogRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<TransportPickingListLog> GetAllBatchDate(DateTime tfrom, DateTime tto)
        {
            List<TransportPickingListLog> dbResult = new List<TransportPickingListLog>();
            var getData = Get().Where(w => w.ShipmentDate >= tfrom && w.ShipmentDate <= tto && (bool)w.IsActive);
            var listData = getData.ToList();
            foreach (var dats in getData)
            {
                if(dats.TransportOrders.Any(y => y.STONo != null && y.STONo != "" && !y.STONo.Contains("PO") && !y.STONo.Contains("ON")))
                {
                    listData.RemoveAll(x => x.IDTransportPickingListLog == dats.IDTransportPickingListLog);
                }
            }
            if (listData.ToList().Count > 0)
            {
                dbResult = listData
                .GroupBy(g => new { g.CreatedDate, g.CreatedBy })
                .Select(s => s.First()).ToList();
            }
            //if (dbResult.Count > 0)
            //{
            //    var dataGet = dbResult.Select(x => x.TransportOrders);
            //    var datas = dataGet.ToList()[0].Where(x => x.STONo == null || x.STONo == "" || x.STONo.Contains("PO")).ToList();
            //    if (dataGet.ToList()[0].Count != datas.Count) dbResult.Add(null);
            //}
            return dbResult;
        }

        public int SaveData(TransportPickingListLog input, bool status)
        {
            if (status)
            {                
                input.IsActive = true;                                
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();
            return input.IDTransportPickingListLog;
        }

        public void SaveDataPickingList(List<TransportPickingListLog> listPL, List<TransportOrder> listTO)
        {
            for (int i = 0; i < listPL.Count; i++)
            {
                var idPL = SaveData(listPL[i], true);
            }            
        }
        public string setCurrSTOPreOrder(string STONo)
        {
            /*var splitSTO = Int16.Parse(STONo.Split('-').Last()) + 1;
            var STOx = "";
            //string startSTONo = "PO-" + DateTime.Now.ToString("MMyy") + "-";
            string startSTONo = "PO" + DateTime.Now.ToString("yyMM");
            if (splitSTO.ToString().Length == 1)
                STOx = startSTONo + "0" + splitSTO;
            else
                STOx = startSTONo + splitSTO;
            return STOx;*/

            /*int serial = 1;
            if (STONo != null)
            {
                var STONumberx = STONo;
                serial = Int16.Parse(STONumberx.Substring(STONumberx.Length - 4)) + 1;
            }
            /*var latestSTO = minSTO != null ? int.Parse(minSTO.Substring(6)) : 0;
            if (serial <= latestSTO)
                serial = latestSTO + 1;*/
            //Debug.WriteLine("STOx");
            //return STONo + serial.ToString().PadLeft(4, '0');
            //string STONo = "ON" + DateTime.Now.ToString("yyMM");
            /*if (rowTO == null)
            {
                STOx = STONo + "0001";
            }
            else
            {*/
            var dateNow = System.DateTime.Now;
            var year = dateNow.Year.ToString().Substring(2);
            var month = dateNow.Month.ToString();
            string STOx = "PO" + year + month + "0001";
            if (STONo != "")
            {
                var STONumberx = STONo;
                string ON = STONo.Substring(0, 6);
                var newSTO = Int64.Parse(STONumberx.Substring(STONumberx.Length - 4)) + 1;
                var lengthON = newSTO.ToString().Length;
                if (lengthON == 1)
                {
                    STOx = ON + "000" + newSTO;
                }
                else if (lengthON == 2)
                {
                    STOx = ON + "00" + newSTO;
                }
                else if (lengthON == 3)
                {
                    STOx = ON + "0" + newSTO;
                }
                else
                {
                    STOx = ON + newSTO;
                }
            }
            
            //}
            //Debug.WriteLine("STOx");
            return STOx;
        }

        public string SendEmail(string userid)
        {
            string result = "true";
            var context = new TOMContextDB();
            try {
                context.SendEmailPickingList(userid);
            }
            catch (Exception ex)
            {
                result = ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString();
            }
            return result;
        }
    }
}
