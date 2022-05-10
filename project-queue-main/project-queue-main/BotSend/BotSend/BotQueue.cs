using HtmlAgilityPack;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSend
{
   public class BotQueue
    {
        public void SendQueue()
        {
            List<string> ListArticleLink = new List<string>();
            String sql = "SELECT * FROM Sourses";
            String sqlar = "SELECT UrlSource FROM [Articles]";
            List<Sourse> sourses = new List<Sourse>();

            using (SqlConnection cnn = ConnectDB.GetConnectSql())
            {
                try
                {
                    cnn.Open();
                    Console.WriteLine("connect success");
                    using (SqlCommand command = new SqlCommand(sql, cnn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Sourse sourse = new Sourse()

                                {
                                    Id = reader.GetInt32(0),
                                    Url = reader.GetString(1),
                                    SelectorSubUrl = reader.GetString(2),
                                    SubUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    SelectorTitle = reader.GetString(4),
                                    SelectorImage = reader.GetString(5),
                                    SelectorDescription = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    SelectorContent = reader.GetString(7),
                                };
                                sourses.Add(sourse);
                            }
                        }
                        using (SqlCommand commandline = new SqlCommand(sqlar, cnn))
                        {
                            using (SqlDataReader reader = commandline.ExecuteReader())
                            {
                                Console.WriteLine("accesss");
                                while (reader.Read())
                                {
                                    string articlelink = reader.GetString(0);
                                    ListArticleLink.Add(articlelink);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            foreach (var sourse in sourses)
            {

                HashSet<string> ListString = new HashSet<string>();
                HashSet<Sourse> ListSubSource = new HashSet<Sourse>();
                var url = sourse.Url;

                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                var nodeList = doc.QuerySelectorAll("." + sourse.SelectorSubUrl + " a");
                foreach (var node in nodeList)
                {
                    ListString.Add(node.GetAttributeValue("href", null));
                }
                var result = ListString.Except(ListArticleLink).ToArray();
                for (int i = 0; i < result.Length; i++)
                {
                    var re = result[i];
                    if (re == null)
                    {
                        continue;
                    }
                    if (i > 0)
                    {
                        var rebe = result[i - 1];
                        if (!re.Contains(rebe))
                        {
                            Sourse subsourse = new Sourse()
                            {
                                Id = sourse.Id,
                                SelectorSubUrl = sourse.SelectorSubUrl,
                                SubUrl = re,
                                SelectorDescription = sourse.SelectorDescription,
                                SelectorTitle = sourse.SelectorTitle,
                                SelectorImage = sourse.SelectorImage,
                                SelectorContent = sourse.SelectorContent
                            };
                            Console.WriteLine(re);
                            var factory = new ConnectionFactory() { HostName = "localhost" };
                            using (var connection = factory.CreateConnection())
                            using (var channel = connection.CreateModel())
                            {
                                channel.QueueDeclare(queue: "SubSource",
                                                    durable: false,
                                                    exclusive: false,
                                                    autoDelete: false,
                                                    arguments: null);
                                var yourObject = JsonConvert.SerializeObject(subsourse);
                                var body = Encoding.UTF8.GetBytes(yourObject);
                                channel.BasicPublish(exchange: "",
                                                    routingKey: "SubSource",
                                                    basicProperties: null,
                                                    body: body);
                            }
                        }
                    }
                    else
                    {
                        Sourse subsourse = new Sourse()
                        {
                            SelectorSubUrl = sourse.SelectorSubUrl,
                            SubUrl = re,
                            SelectorDescription = sourse.SelectorDescription,
                            SelectorTitle = sourse.SelectorTitle,
                            SelectorImage = sourse.SelectorImage,
                            SelectorContent = sourse.SelectorContent
                        };
                        Console.WriteLine(re);
                        var factory = new ConnectionFactory() { HostName = "localhost" };
                        using (var connection = factory.CreateConnection())
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "SubSource",
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);
                            var yourObject = JsonConvert.SerializeObject(subsourse);
                            var body = Encoding.UTF8.GetBytes(yourObject);
                            channel.BasicPublish(exchange: "",
                                                routingKey: "SubSource",
                                                basicProperties: null,
                                                body: body);
                        }
                    }
                }
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
