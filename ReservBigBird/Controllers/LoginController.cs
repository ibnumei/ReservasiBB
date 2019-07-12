using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ReservBigBird.API_Model;
using ReservBigBird.Filters;

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
                //Hashing Pass
                string hashedPass = ComputeSha256Hash(paramlogin.pass);

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
                        HttpResponseMessage message = client.GetAsync(url + "/api/USRs?usrid=" + paramlogin.username+"&password="+ hashedPass).Result;

                        if (message.IsSuccessStatusCode)
                        {
                            var serializer = new DataContractJsonSerializer(typeof(HasilLogin));
                            var result = message.Content.ReadAsStringAsync().Result;
                            byte[] byteArray = Encoding.UTF8.GetBytes(result);
                            MemoryStream stream = new MemoryStream(byteArray);
                            HasilLogin resultData = serializer.ReadObject(stream) as HasilLogin;

                            Session["userid"] = resultData.USRNM;
                            Session["usernm"] = resultData.USRNM;

                            return Redirect("TerimaOrder/Index");
                            //====================================================================================

                        }
                        else
                        {
                            ViewBag.gagalLogin = "Username atau Password Salah";
                            return View();
                        }
                      


                    }
                    catch (Exception ex)
                    {
                        ViewBag.server = "Gagal Menghubungi Server, Silahkan coba lagi !";
                        var error = ex.ToString();
                        return View();
                    }
                }

            }

            return View();
        }

        //=======================================================================================
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        //=======================================================================================
        [HttpGet]
        public ActionResult Logout()
        {
            //Session.Remove("userid");
            //Session.Remove("usernm");
            Session.RemoveAll();
            Session.Clear();
            return Redirect("Index");
        }
    }
}