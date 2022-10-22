using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalAssignment.Controllers
{
    //Home page controller
    [RequireHttps]
    public class HomeController : Controller
    {
        //Return index page (home page)
        public ActionResult Index()
        {
            return View();
        }

        //Return about page
        public ActionResult About()
        {

            return View();
        }

    }
}