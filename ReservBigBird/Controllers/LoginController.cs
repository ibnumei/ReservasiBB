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
using ReservBigBird.API_Model;

namespace ReservBigBird.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Login paramlogin)
        {
            if(ModelState.IsValid)
            {
               

                //Ambil link url di web config
                String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

                //Method untuk consume api
                String response = "";
                var credentials = new NetworkCredential("ac", "123");
                var handler = new HttpClientHandler { Credentials = credentials }; // for validation
                                                                                   //    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };// allow domain checker
                using (var client = new HttpClient(handler))
                {
                    // Make your request...
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        HttpResponseMessage message = client.GetAsync(url + "api/USRs?username="+paramlogin.username+"&password="+paramlogin.pass).Result;

                        if (message.IsSuccessStatusCode)
                        {
                            var serializer = new DataContractJsonSerializer(typeof(Login));
                            var result = message.Content.ReadAsStringAsync().Result;
                            byte[] byteArray = Encoding.UTF8.GetBytes(result);
                            MemoryStream stream = new MemoryStream(byteArray);
                            Login resultData = serializer.ReadObject(stream) as Login;

                            Session["user"] = resultData.username;

                            return RedirectToAction("Index", "TerimaOrder");
                            //====================================================================================

                        }
                        else
                        {
                            return View();
                        }
                      


                    }
                    catch (Exception ex)
                    {
                       
                        var error = ex.ToString();
                        return View();
                    }
                }

            }

            return View();
        }

        //=======================================================================================
    }
}