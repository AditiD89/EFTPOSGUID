using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace EFTServer
{
    public class DataUpdateJob : IJob
    {
        private SqlConnection con;
        private SqlCommand com;
        public DataBaseConnetionDetails dbConnectionDetails;

        private void connection()
        {
            con = new SqlConnection(GetConnectionString());
        }

        private string GetConnectionString()
        {
            dbConnectionDetails = JsonConvert.DeserializeObject<DataBaseConnetionDetails>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "DBConnection.json"));

            return "Data Source=" + dbConnectionDetails.DataSource + ";" + "Initial Catalog=" + dbConnectionDetails.InitialCatalogue + ";" + "Integrated Security = " + dbConnectionDetails.IntegratedSecurity + "; ";
           
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var P = new Purchase();
            
            DataTable Dt = P.GetAllRecordsPurchase();
           
            foreach (DataRow row in Dt.Rows)
            {
                Purchase purchase = new Purchase();
                string ID = row.Field<string>("ID");               
                string Status = row.Field<string>("Status");
                purchase.ID = ID;
                purchase.Status = Status;
             
                purchase.UpdatePurchaseStatus(purchase.ID, purchase.Status);
                if (purchase.Status != "Success")
                {
                    Console.WriteLine("Purchase ID : " + ID);
                    Console.WriteLine("Purchase Status : " + Status);
                    Console.WriteLine("");
                }
                else
                {
                    //DoNothing
                }
                
            }
            
        }
    }
}
