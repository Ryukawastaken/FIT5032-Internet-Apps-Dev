using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_MyCodeSnippet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            HelloWorld.Hello NewHello = new HelloWorld.Hello();

            Exercise.ExampleDictionary ED = new Exercise.ExampleDictionary();

            ED.Example();

            ViewBag.Message = NewHello.GetHello();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}