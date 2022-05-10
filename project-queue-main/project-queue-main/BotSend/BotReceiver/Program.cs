using BotReceiver.Entity;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Sourse sourse = new Sourse();
            String sql = "INSERT INTO Articles(UrlSource,Title,Image,Description,Content,Category,CreatedAt,SourseId) values(@UrlSource,@Title,@Image,@Description,@Content,@Category,@CreatedAt,@SourseId)";

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (SqlConnection cnn = ConnectDB.GetConnectSql())
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                cnn.Open();
                Console.WriteLine("connect success");
                channel.QueueDeclare(queue: "SubSource",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    sourse = JsonConvert.DeserializeObject<Sourse>(Encoding.UTF8.GetString(body));
                    List<string> contentArticle = new List<string>();
                    var url = sourse.SubUrl;
                    var web = new HtmlWeb();
                    HtmlDocument doc = web.Load(url);
                    var title = doc.QuerySelector(sourse.SelectorTitle).InnerHtml;
                    var description = doc.QuerySelector(sourse.SelectorDescription).InnerText;
                    var img = doc.QuerySelector(sourse.SelectorImage)?.Attributes["data-src"].Value;
                    var contents = doc.QuerySelector(sourse.SelectorContent)?.InnerText;
                        Article subarticle = new Article()
                         {
                             UrlSource =  url,
                             Title = title,
                             Description = description,
                             Image = img,
                             Content = contents,
                             Category = title,
                             CreatedAt  = 12051692,
                             SourseId = sourse.Id
                        };
                     try {
                            SqlCommand command = new SqlCommand(sql, cnn);
                            command.Parameters.AddWithValue("@UrlSource", subarticle.UrlSource);
                            command.Parameters.AddWithValue("@Title", subarticle.Title);
                            command.Parameters.AddWithValue("@Image", subarticle.Image);
                            command.Parameters.AddWithValue("@Description", subarticle.Description);
                            command.Parameters.AddWithValue("@Content", subarticle.Content);
                            command.Parameters.AddWithValue("@Category", subarticle.Title);
                            command.Parameters.AddWithValue("@CreatedAt", subarticle.CreatedAt);
                            command.Parameters.AddWithValue("@SourseId",subarticle.SourseId);
                            command.ExecuteNonQuery();
                            Console.WriteLine("success");
                        } catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                };
                channel.BasicConsume(queue: "SubSource",
                         autoAck: true,
                         consumer: consumer);
                Console.WriteLine(" Press [enter] to exitw.");
                Console.ReadLine();
            }
         }
    }
}
