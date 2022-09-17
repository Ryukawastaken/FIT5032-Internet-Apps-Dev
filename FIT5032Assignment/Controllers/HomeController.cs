using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIT5032Assignment.Models;

namespace FIT5032Assignment.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Clients(Client client)
        {
            try
            {
                string clientFirstName = client.clientFirstName;
                string clientLastName = client.GetClientLastName();

                ViewBag.CurrentClient = clientFirstName + " " +  clientLastName;
            }
            catch
            {

            }
            if (User.IsInRole("Reception"))
            {
                ViewBag.Message = "List of all Clients";
            }
            else if (User.IsInRole("Doctor"))
            {
                ViewBag.Message = "List of your current Clients";
            }
            else
            {
                ViewBag.Message = "";
            }
            return View();
        }

        public ActionResult Appointments()
        {
            if (User.IsInRole("Reception"))
            {
                ViewBag.Message = "List of all Appointments";
            }
            else if (User.IsInRole("Doctor") || User.IsInRole("Client"))
            {
                ViewBag.Message = "List of all of your Appointments";
            }
            else
            {
                ViewBag.Message = "";
            }
            return View();
        }

    }
}