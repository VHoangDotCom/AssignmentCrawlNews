using HtmlAgilityPack;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotSend_RabbitMQ
{
    class SendSources
    {
        public static void Publish()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://vnexpress.net/bong-da");

            var nodes = document.DocumentNode.SelectNodes("//h2/a");

            channel.QueueDeclare(queue: "news",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            foreach (var item in nodes)
            {
                var message = item.GetAttributeValue("href", "");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "news",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                Thread.Sleep(10000);
            }
        }
    }
}
