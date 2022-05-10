using CrawlNews.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CrawlNews.config
{
    public class DBConnection : DbContext
    {
        public DBConnection() : base("name=CrawDB")
        {
        }

        public DbSet<Sourse> Sourses { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }


    }
}