using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace BotRecieve_RabbitMQ
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            BotScheduler bot = new BotScheduler();
            //Thuc hien hanh dong nay 10h moi ngay
            await  bot.Execute_Everyday_10h();
        }
    }
}
