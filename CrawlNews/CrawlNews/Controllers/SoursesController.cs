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
using CrawlNews.CrawlerBot;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;
using System.Data.Entity.Infrastructure;

namespace CrawlNews.Controllers
{
    public class SoursesController : Controller
    {
        private SchedulerBot schedulerBot = new SchedulerBot();
        private DBConnection db = new DBConnection();
        public static HashSet<Sourse> setLink;

        // GET: Sourses
        public ActionResult Index()
        {
            return View(db.Sourses.ToList());
        }

        // GET: Sourses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sourse sourse = db.Sourses.Find(id);
            if (sourse == null)
            {
                return HttpNotFound();
            }
            return View(sourse);
        }


        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Categories
                                   orderby d.Name
                                   select d;
            ViewBag.CategoryId = new SelectList(departmentsQuery, "CategoryId", "Name", selectedDepartment);
        }
        // GET: Sourses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Url,SelectorSubUrl,SubUrl,SelectorTitle,SelectorImage,SelectorDescription,SelectorContent,CategoryId")] Sourse sourse)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Sourses.Add(sourse);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateDepartmentsDropDownList(sourse.CategoryId);
            return View(sourse);
        }

        // GET: Sourses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sourse sourse = db.Sourses.Find(id);
            if (sourse == null)
            {
                return HttpNotFound();
            }
            return View(sourse);
        }

        // POST: Sourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Sourse sourse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sourse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sourse);
        }

        // GET: Sourses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sourse sourse = db.Sourses.Find(id);
            if (sourse == null)
            {
                return HttpNotFound();
            }
            return View(sourse);
        }

        // POST: Sourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sourse sourse = db.Sourses.Find(id);
            db.Sourses.Remove(sourse);
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

        public ActionResult CheckSource()
        {
            return View();
        }
        public ActionResult PreviewLink()
        {
            ViewBag.ListCategory = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckLink(Sourse sourceCheck)
        {
            if (sourceCheck.Url != "" || sourceCheck.SelectorSubUrl != "")
            {

                try
                {
                    var web = new HtmlWeb();
                    HtmlDocument doc = web.Load(sourceCheck.Url);
                    var nodeList = doc.QuerySelectorAll(sourceCheck.SelectorSubUrl);//tìm đến những thẻ a nằm trong h3 có class= title-news
                    var setLinkSource = new HashSet<Sourse>(); // Đảm bảo link không giống nhau, nếu có sẽ bị ghi đè ở phần tử trước

                    foreach (var node in nodeList)
                    {
                        var link = node.Attributes["href"]?.Value;
                        if (string.IsNullOrEmpty(link)) // nếu link này null
                        {
                            continue;
                        }
                        var sourceCheck1 = new Sourse()
                        {
                            Url = link
                        };

                        setLinkSource.Add(sourceCheck1);
                    }
                    Debug.WriteLine("gia trị", sourceCheck.Url.ToString());
                    return PartialView("GetListSource", setLinkSource);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error ở đây: " + e.Message);
                    return PartialView("Faild");
                }
            }
            return PartialView("Faild");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Preview(Sourse sourceCheck, int category)
        {
            if (sourceCheck.SubUrl != "" && sourceCheck.SelectorTitle != "" && sourceCheck.SelectorDescription != ""
                && sourceCheck.SelectorContent != "" && sourceCheck.SelectorSubUrl != ""
                && sourceCheck.SelectorImage != "" && sourceCheck.Category.ToString() != "")
            {
                try
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                    var web = new HtmlWeb();
                    HtmlDocument doc = web.Load(sourceCheck.SubUrl);
                    var title = doc.QuerySelector(sourceCheck.SelectorTitle)?.InnerText ?? "";
                    var description = doc.QuerySelector(sourceCheck.SelectorDescription)?.InnerText ?? "";
                    var imageNode = doc.QuerySelector(sourceCheck.SelectorImage)?.Attributes["data-src"].Value;
                    var content = doc.QuerySelector(sourceCheck.SelectorContent)?.InnerText;
                   // var category = doc.QuerySelector(sourceCheck.CategoryId.ToString())?.InnerText ?? "";
                    string thumbnail = "";
                    if (imageNode != null)
                    {
                        thumbnail = imageNode;
                    }
                    else
                    {
                        thumbnail = "";
                    }
                    
                    Article article = new Article()
                    {
                        Title = title,
                        Description = description,
                        Content = content,
                        Image = thumbnail,
                        Category = db.Categories.Find(category).Name,
                        
                    };

                    return PartialView("GetListSelector", article);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    return PartialView("Faild");
                }
            }
            return PartialView("Faild");
        }

        [HttpPost, ActionName("CreateSourse")]
        public JsonResult CreateSourse(Sourse sourse)
        {
            db.Sourses.Add(sourse);
            db.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }

    }
}
