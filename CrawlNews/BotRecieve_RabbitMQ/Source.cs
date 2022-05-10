using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRecieve_RabbitMQ
{
    class Source
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string SelectorSubUrl { get; set; }
        public string SelectorTitle { get; set; }
        public string SelectorImage { get; set; }
        public string SelectorDescription { get; set; }
        public string SelectorContent { get; set; }
        public string Category
        {
            get; set;
        }
    }
}
