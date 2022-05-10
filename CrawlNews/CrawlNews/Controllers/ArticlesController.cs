using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrawlNews.Models;
using CrawlNews.config;
using PagedList;
using CrawlNews.Services;
using Nest;
using System.Data.Entity.Infrastructure;

namespace CrawlNews.Controllers
{
    public class ArticlesController : Controller
    {
        private DBConnection db = new DBConnection();

        // GET: Articles
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder; //PageList
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.CreatedSortParm = String.IsNullOrEmpty(sortOrder) ? "create_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "cate_desc" ? "cate_asc" : "cate_desc";
            
            //PageList + search ajax
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            //Search by Linq
            var articles = db.Articles.AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.Title.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.Category.Contains(searchString)
                                       || s.Content.Contains(searchString)
                                       );
            }

          /*  //Search by ElasticSearch
            List<Article> list = new List<Article>();
            var searchRequest = new SearchRequest<Article>()
            {
                From = 0,
                Size = 10,
                QueryOnQueryString = searchString
            };
            var searchResult = ElasticSearchService.GetInstance().Search<Article>(searchRequest);
            list = searchResult.Documents.ToList();*/

            //Order by Title, Category , Created date
            switch (sortOrder)
            {
                case "title_desc":
                    articles = articles.OrderByDescending(s => s.Title);
                    break;
                case "cate_desc":
                    articles = articles.OrderByDescending(s => s.Category);
                    break;
                case "cate_asc":
                    articles = articles.OrderBy(s => s.Category);
                    break;
                case "create_desc":
                    articles = articles.OrderByDescending(s => s.CreatedAt);
                    break;
                default:
                    articles = articles.OrderBy(s => s.UrlSource);
                    break;
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);

            //list by linq
            return View(articles.ToPagedList(pageNumber, pageSize));

            //list by elastic
            //return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult IndexAjax(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder; //PageList
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.CreatedSortParm = String.IsNullOrEmpty(sortOrder) ? "create_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "cate_desc" ? "cate_asc" : "cate_desc";

            //PageList + search ajax
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            //Search by Linq
            var articles = db.Articles.AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.Title.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.Category.Contains(searchString)
                                       || s.Content.Contains(searchString)
                                       );
            }

           /* //Search by ElasticSearch
            List<Article> list = new List<Article>();
            var searchRequest = new SearchRequest<Article>()
            {
                From = 0,
                Size = 10,
                QueryOnQueryString = searchString
            };
            var searchResult = ElasticSearchService.GetInstance().Search<Article>(searchRequest);
            list = searchResult.Documents.ToList();*/

            //Order by Title, Category , Created date
            switch (sortOrder)
            {
                case "title_desc":
                    articles = articles.OrderByDescending(s => s.Title);
                    break;
                case "cate_desc":
                    articles = articles.OrderByDescending(s => s.Category);
                    break;
                case "cate_asc":
                    articles = articles.OrderBy(s => s.Category);
                    break;
                case "create_desc":
                    articles = articles.OrderByDescending(s => s.CreatedAt);
                    break;
                default:
                    articles = articles.OrderBy(s => s.UrlSource);
                    break;
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return PartialView(articles.ToPagedList(pageNumber, pageSize));
            //return View(list.ToPagedList(pageNumber, pageSize));

        }



        // GET: Articles/Details/5
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

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Categories
                                   orderby d.Name
                                   select d;
            ViewBag.SourseID = new SelectList(departmentsQuery, "SourseID", "Name", selectedDepartment);
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UrlSource,Title,Image,Description,Content,Category,CreatedAt,SourseId")] Article article)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ElasticSearchService.GetInstance().IndexDocument(article);
                    db.Articles.Add(article);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
   
            PopulateDepartmentsDropDownList(article.SourseId);
            return View(article);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UrlSource,Title,Image,Description,Content,CategoryId,CreatedAt")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
