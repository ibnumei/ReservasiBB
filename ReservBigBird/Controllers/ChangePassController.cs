using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
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
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            String sessionname = Session["userid"].ToString();

            ChangePassPost changePassPost = new ChangePassPost()
            {
                usrid = sessionname,
                Pass = gantiPass.PassBaru2
            };

            var json2 = new JavaScriptSerializer().Serialize(changePassPost);
            var stringPayload = JsonConvert.SerializeObject(changePassPost);
            //String response = "";
            var credentials = new NetworkCredential("username", "password");
            var handler = new HttpClientHandler { Credentials = credentials };
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage message = client.PostAsync(url + "/api/USRs", httpContent).Result;
                    if (message.IsSuccessStatusCode)
                    {
                        ViewBag.successs = "Sukses";
                        return View();
                    }
                    else
                    {
                        ViewBag.gagal = "Gagal";
                        return View();
                    }

                }
                catch
                {
                    ViewBag.gagal = "Gagal";
                    return View();
                }
            }
        }
    }
}