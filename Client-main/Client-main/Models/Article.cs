using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClientNews.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string UrlSource { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public long CreatedAt { get; set; }
        public int SourseId { get; set; }
    }
}