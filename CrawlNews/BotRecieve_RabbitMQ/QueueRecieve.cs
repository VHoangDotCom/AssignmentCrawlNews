using HtmlAgilityPack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRecieve_RabbitMQ
{
    class QueueRecieve
    {
        public async void Consume()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var cnn = DBConnect.connect())
            {
                cnn.Open();
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "news_send",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
                    HtmlWeb web = new HtmlWeb();

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    string truncateSQL = "Truncate TABLE [dbo].[Sourses]";
                    SqlCommand commandTruncate = new SqlCommand(truncateSQL, cnn);
                    commandTruncate.ExecuteNonQuery();

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var link = Encoding.UTF8.GetString(body);
                        HtmlDocument doc = web.Load(link);
                        var title = doc.QuerySelector("h1.title-detail").InnerText;
                        var description = doc.QuerySelector("p.description").InnerText;
                        var imageNode = doc.QuerySelector("img").Attributes["src"].Value;
                        var content = doc.QuerySelector("article.fck_detail").InnerText;

                        try
                        {
                            string query = "INSERT INTO Sourses (title, description, image) VALUES (@title, @description, @image)";
                            var source = new Source()
                            {
                                Url = link,
                                SelectorTitle = title,
                                SelectorImage = imageNode,
                                SelectorDescription = description,
                                SelectorContent = content,
                            };

                            SqlCommand command = new SqlCommand(query, cnn);
                            command.Prepare();
                            command.Parameters.AddWithValue("@SelectorTitle", source.SelectorTitle);
                            command.Parameters.AddWithValue("@SelectorDescription", source.SelectorDescription);
                            command.Parameters.AddWithValue("@SelectorContent", source.SelectorContent);
                            command.Parameters.AddWithValue("@SelectorImage", source.SelectorImage);
                            command.Parameters.AddWithValue("@Url", source.Url);
                            command.ExecuteNonQuery();

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }

                        Console.WriteLine(title.Trim());
                        Console.WriteLine(description.Trim());
                        Console.WriteLine(imageNode);
                        Console.WriteLine(content);
                    };
                    channel.BasicConsume(queue: "news_send",
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }          
          
        }
    }
}
