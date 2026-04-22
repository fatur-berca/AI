using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Repositories
{
    public interface ITransportOrderRequestHeaderRepo: ITOMGenericRepository<TransportOrderRequestHeader>
    {
        IEnumerable<TransportOrderRequestHeaderDTO> Get(string reqNo = null);

        string GenerateNewRequestNumber(DateTime date);
        string GenerateSequenceNumber(DateTime date, int idRequest);
        string GenerateNewOrderNumber(DateTime date, string lastUsedON = null);
    }

    public class TransportOrderRequestHeaderRepo : TOMGenericRepository<TransportOrderRequestHeader>, ITransportOrderRequestHeaderRepo
    {
        public TOMContextDB Context { get; private set; }
        public TransportOrderRequestHeaderRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            Context = contextEntities;
        }

        public IEnumerable<TransportOrderRequestHeaderDTO> Get(string reqNo = null)
        {
            return Mapper.Map<List<TransportOrderRequestHeaderDTO>>( Get(c => c.RequestNo == reqNo).ToList());
        }

        public string GenerateNewRequestNumber(DateTime date)
        {
            string RQ = "RQ" + date.ToString("yyMM");
            string ReqNumber = RQ + "0001";
            var rowTOR = this.Get(c => c.RequestNo.StartsWith(RQ)).OrderByDescending(p => p.RequestNo).FirstOrDefault();
            if (rowTOR != null)
            {
                var newNum = int.Parse(rowTOR.RequestNo.Substring(RQ.Length)) + 1;
                ReqNumber = RQ + newNum.ToString().PadLeft(4, '0');
            }
            return ReqNumber;
        }

        private Dictionary<string, string> _generatedSequences = new Dictionary<string, string>();
        public string GenerateSequenceNumber(DateTime date, int idRequest )
        {
            var oldReq = Context.TransportOrderRequests.Where(c => c.IDRequest == idRequest).FirstOrDefault();
            if (oldReq != null)
            {
                var oldOrd = oldReq.TransportOrders.Where(c => !string.IsNullOrWhiteSpace(c.SeqNo)).FirstOrDefault();
                if (oldOrd != null)
                    return oldOrd.SeqNo;
            }

            var shLow = date.Date;
            var shHigh = date.Date + new TimeSpan(1, 0, 0, 0);
            
            var rowTO = Context.TransportOrders
                .Where(c => c.IsActive && c.ShipmentDate >= shLow && c.ShipmentDate <= shHigh && !string.IsNullOrEmpty(c.SeqNo) && c.SeqNo.StartsWith("TO"))
                .OrderByDescending(p => p.SeqNo).FirstOrDefault();

            var serial = 1;
            if (rowTO != null)
            {
                var LastSeq = rowTO.SeqNo;
                serial = int.Parse(LastSeq.Substring(2)) + 1;
            }

            var lst = _generatedSequences.ContainsKey(date.ToString("yyyyMMdd")) ? int.Parse(_generatedSequences[date.ToString("yyyyMMdd")].Substring(2)) : 0;
            if (serial <= lst)
                serial = lst + 1;
            var nto = "TO" + serial.ToString().PadLeft(3, '0');
            _generatedSequences[date.ToString("yyyyMMdd")] = nto;
            return nto;
        }
        public string GenerateNewOrderNumber(DateTime date, string lastUsedON = null)
        {
            string STONo = "ON" + date.ToString("yyMM");

            /*var rowTO = Context.TransportOrders.Where(c => !string.IsNullOrEmpty(c.STONo) && c.STONo.StartsWith(STONo))
                .OrderByDescending(p => p.STONo).FirstOrDefault();  */


            /*var rowTO = Context.TransportOrders.AsEnumerable().Where(c => !string.IsNullOrEmpty(c.STONo) && c.STONo.StartsWith(STONo))
                .OrderByDescending(p => Convert.ToInt32(p.STONo.Remove(0, 6))).FirstOrDefault();*/

            var STONo_list = Context.TransportOrders
                                .Where(c => !string.IsNullOrEmpty(c.STONo) && c.STONo.StartsWith(STONo))
                                .Select(u => u.STONo)
                                .ToList();
            var rowTO = (from dt in STONo_list
                         select new
                         {
                             STONo = Convert.ToInt32(dt.Remove(0, 6))
                         }).OrderByDescending(y => y.STONo).FirstOrDefault();

            int serial = 1;
            if (rowTO != null)
            {
                var STONumberx = ""+rowTO.STONo;
                serial = Int32.Parse(STONumberx) + 1;
            }

            var latestSTO = lastUsedON != null ? int.Parse(lastUsedON.Substring(6)) : 0;
            if (serial <= latestSTO)
                serial = latestSTO + 1;
            

            if (serial.ToString().Length > 4)
            {
                return STONo + serial.ToString();
            }
            else
            {
                return STONo + serial.ToString().PadLeft(4, '0');
            }

        }
    }
}
