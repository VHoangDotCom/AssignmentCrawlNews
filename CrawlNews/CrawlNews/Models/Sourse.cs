using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CrawlNews.Models
{
    public class Sourse
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public string SelectorSubUrl { get; set; }
        public string SubUrl { get; set; }
        public string SelectorTitle { get; set; }
        public string SelectorImage { get; set; }
        public string SelectorDescription { get; set; }
        public string SelectorContent { get; set; }
        public int CategoryId { get; set; }
        public int ArticleId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [ForeignKey("ArticleId")]
        public virtual ICollection<Article> ListArticle { get; set; }



    }
}