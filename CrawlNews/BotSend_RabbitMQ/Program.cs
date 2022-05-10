using RabbitMQ.Client;
using System;

namespace BotSend_RabbitMQ
{
    class Program
    {
        public static void Main(string[] args)
        {
            SendSources ss = new SendSources();
            SendSources.Publish();
        }
    }
}
