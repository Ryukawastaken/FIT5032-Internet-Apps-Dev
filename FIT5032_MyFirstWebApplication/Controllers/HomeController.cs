using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_MyFirstWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is a changed application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "This is a changed contact page.";

            return View();
        }
    }
}