using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Entity;
using Test.Service;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //T_AdministratorService service = new T_AdministratorService();
            //service.Insert(new T_Administrator {
            //    LoginName="Rocher",
            //    Age=22,
            //    Name="Rocher",
            //    Password="111111"
            //});

            return View();
        }

        [HttpPost]
        public JsonResult Index(FormCollection forms)
        {
            return Json(new { message="ok"});
        }
    }
}