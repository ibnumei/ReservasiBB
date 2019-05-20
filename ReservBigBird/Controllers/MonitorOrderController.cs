using ReservBigBird.API_Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ReservBigBird.Controllers
{
    public class MonitorOrderController : Controller
    {
        // GET: MonitorOrder
        public ActionResult Index()
        {
            ViewBag.Current = "2";
            return View();
        }

        public ActionResult _TableMonitorOrder(ParamMonitorOrder paramMonitor)
        {
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
                    HttpResponseMessage message = client.GetAsync("http://10.0.19.165/BBWS/Api/Orders?ordid=" + paramMonitor.NoOrder + "&ordnpt=" + paramMonitor.Perusahaan + "&ordnpm=" + paramMonitor.Pemesan + "&kondisi=" + paramMonitor.KondisiOrder).Result;

                    if (message.IsSuccessStatusCode)
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<MonitorOrder>));
                        var result = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        List<MonitorOrder> resultData = serializer.ReadObject(stream) as List<MonitorOrder>;
                        ViewBag.data = resultData.ToList();
                        //for (int i = 0; i < resultData.Count; i++)
                        //{
                        //    var bb = resultData[i].username;

                        //    ViewBag.hasil = "Sukses mendapatkan data";

                        //    return View();
                        //}

                        return PartialView("_TableMonitorOrder", resultData.ToList());
                        //====================================================================================

                    }
                    else
                    {
                        return PartialView("_TableMonitorOrder");
                    }
                    //if(message.)


                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                    return PartialView("_TableMonitorOrder");
                }
            }

        }
    }
}