using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrawlNews.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        public ActionResult Blog()
        {
            return View();
        }
        public ActionResult BlogDetail()
        {
            return View();
        }
    }
}