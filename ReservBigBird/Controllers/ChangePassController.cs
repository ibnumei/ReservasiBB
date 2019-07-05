using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReservBigBird.API_Model;
using ReservBigBird.Filters;

namespace ReservBigBird.Controllers
{

    [UserAuthenticationFilter]

    public class ChangePassController : Controller
    {
        // GET: ChangePass
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GantiPass gantiPass)
        {
            return View();
        }
    }
}