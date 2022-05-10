using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BotRecieve_RabbitMQ
{
    class DBConnect
    {
       
        private static SqlConnection cnn;

        public static SqlConnection connect()
        {
            if (cnn == null)
            {
                //your connection string 
                string connString = @"Data Source=crawldb.database.windows.net;Initial Catalog=CrawDB;User ID=crawshit;Password=Crawlmyass123;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                cnn = new SqlConnection(connString);
            }
            return cnn;
        }
    }
}
