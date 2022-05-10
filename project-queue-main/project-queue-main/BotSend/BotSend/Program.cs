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
    class Program
    {
        public static async Task Main(string[] args)
        {
            BotSchedule bot = new BotSchedule()
;           await bot.Execute_Everyday_10h();
        }
    }
}
