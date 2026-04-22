using System;
using System.Configuration;
using System.Data.SqlClient;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories
{
    public class TransportTruckArrivalTempRepo : TOMGenericRepository<TruckArrivalTemp>, ITransportTruckArrivalTempRepo
    {
        public TransportTruckArrivalTempRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public void DeleteTruckArrivalTemp()
        {
            //SqlConnection sqlCon = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["DFISContextDB1"].ConnectionString));
            //string queryString = "DELETE FROM TruckArrivalTemp";
            //SqlCommand command = new SqlCommand(queryString, sqlCon);
            //sqlCon.Open();
            //command.ExecuteReader();
            //sqlCon.Close();
        }

        public void ReseedTruckArrivalTemp()
        {
            //SqlConnection sqlCon = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["DFISContextDB1"].ConnectionString));
            //string queryString = "DBCC CHECKIDENT (\"TruckArrivalTemp\", RESEED, 0)";
            //SqlCommand command = new SqlCommand(queryString, sqlCon);
            //sqlCon.Open();
            //command.ExecuteReader();
            //sqlCon.Close();
        }

        public void SaveData(TruckArrivalTemp save)
        {
            Insert(save);
            Save();
        }
    }
}
