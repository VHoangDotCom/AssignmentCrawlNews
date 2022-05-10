using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClientNews.Data;
using ClientNews.Models;
using ClientNews.Services;
using Nest;
using PagedList;

namespace ClientNews.Controllers
{
    public class ClientController : Controller
    {
        private MyDbContext db = new MyDbContext();
        // GET: Client
        public ActionResult Index(string searchString, int? page)
        {
            List<Article> list = new List<Article>();
            var searchRequest = new SearchRequest<Article>()
            {
                From = 0,
                Size = 10,
                QueryOnQueryString = searchString
            };
            var searchResult = ElasticSearchService.GetInstance().Search<Article>(searchRequest);
            list = searchResult.Documents.ToList();
            ViewBag.CurrentFilter = searchString;
            ViewData["category"] = db.Categories.ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // GET: Client/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }          
            return View(article);
        }
    }
}
