using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSend
{
    class ConnectDB
    {
        private static SqlConnection _connection;
        //private const string ConnectionString = @"Server=tcp:testwebserverclientdbserver.database.windows.net,1433;Initial Catalog=RabbitMQAssignment_db;Persist Security Info=False;User ID=sqladmin;Password=Bach@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private const string ConnectionString = @"Data Source=crawldb.database.windows.net;Initial Catalog=CrawDB;User ID=crawshit;Password=Crawlmyass123;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static SqlConnection GetConnectSql()
        {
            if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
            {
                _connection = new SqlConnection(
                    string.Format(ConnectionString));
            }
            return _connection;
        }
    }
}
