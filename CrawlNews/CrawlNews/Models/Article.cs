using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CrawlNews.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string UrlSource { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public long CreatedAt { get; set; }
        public int SourseId { get; set; }
        [ForeignKey("SourseId")]
        public virtual Sourse Sourse { get; set; } //Link Bài viết này có nguồn từ trang nào VD lấy từ trang VN express


    }
}