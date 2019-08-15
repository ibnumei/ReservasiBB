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
using System.Configuration;
using ReservBigBird.Filters;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace ReservBigBird.Controllers
{
    [UserAuthenticationFilter]

    public class MonitorOrderController : Controller
    {
        // GET: MonitorOrder
        public ActionResult Index()
        {
            ViewBag.Current = "2";
            if (TempData["NoOrderTemp"] != null)
            {
                String data = TempData["NoOrderTemp"].ToString();
                ViewBag.NoOrderParsing = data;

                return View();
            }
            else
            {
                return View();
            }
            
            
        }

        public ActionResult _TableMonitorOrder(ParamMonitorOrder paramMonitor)
        {
            if(paramMonitor != null)
            {
                ViewBag.value = "Yes";
            }
            return PartialView("_TableMonitorOrder");
            //================================================================================================================

            /*
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
                    HttpResponseMessage message = client.GetAsync(url+"/Api/Orders?ordid=" + paramMonitor.NoOrder + "&ordnpt=" + paramMonitor.Perusahaan + "&ordnpm=" + paramMonitor.Pemesan + "&kondisi=" + paramMonitor.KondisiOrder).Result;

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
                        ViewBag.error = "Tidak Dapat Respon dari Server";
                        return PartialView("_TableMonitorOrder");
                    }
                    //if(message.)


                }
                catch (Exception ex)
                {
                    ViewBag.error = "Tidak Dapat Respon dari Server";
                    var error = ex.ToString();
                    return PartialView("_TableMonitorOrder");
                }
            }
            */
        }

        [HttpPost]
        public JsonResult GetDetail(string ordid)
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
                    //url: apiUrl + "/Api/Plans?popordid=&popnpk=&popnpm=&popid=" + b + "&poppolid=" + a + "&popdaow=" + c,

                    HttpResponseMessage message = client.GetAsync(url + "/Api/Orders?ordid=" + ordid + "&ordnpt=&ordnpm=&kondisi=").Result;

                    if (message.IsSuccessStatusCode)
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<PopupMonitorOrder>));
                        var result = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        List<PopupMonitorOrder> resultData = serializer.ReadObject(stream) as List<PopupMonitorOrder>;
                        //ViewBag.data = resultData.ToList();

                        return Json(resultData.ToList(), JsonRequestBehavior.AllowGet);
                        //return PartialView("_TableDisplayPlanning", resultData.ToList());
                        //====================================================================================

                    }
                    else
                    {
                        ViewBag.error = "Tidak Dapat Respon dari Server";
                        //return PartialView("_TableDisplayPlanning");
                        return Json(new { success = true, responseText = "The attached file is not supported." }, JsonRequestBehavior.AllowGet);
                    }
                    //if(message.)


                }
                catch (Exception ex)
                {
                    ViewBag.error = "Tidak Dapat Respon dari Server";
                    var error = ex.ToString();
                    //return PartialView("_TableDisplayPlanning");
                    return Json(new { success = true, responseText = "The attached file is not supported." }, JsonRequestBehavior.AllowGet);
                }
            }

        }

        [HttpPost]
        public JsonResult GetDatatab(ModelPostOrderDTAjax dt, string ordid, string ordnpt, string ordnpm, string kondisi)
        {
            String YesOrNo;
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            var json2 = new JavaScriptSerializer().Serialize(dt);
            var stringPayload = JsonConvert.SerializeObject(dt);
            //String response = "";
            var credentials = new NetworkCredential("username", "password");
            var handler = new HttpClientHandler { Credentials = credentials };
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            using (var client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage message = client.PostAsync(url + "/Api/PostOrdersDT?ordid=" + ordid + "&ordnpt=" + ordnpt + "&ordnpm=" + ordnpm + "&kondisi=" + kondisi, httpContent).Result;

                    var contentString = message.Content.ReadAsStringAsync().Result;
                    var serializer = new JavaScriptSerializer().DeserializeObject(contentString);
                    

                    if (message.IsSuccessStatusCode)
                    {
                        YesOrNo = "Yes";
                    }
                    else
                    {
                        YesOrNo = "No";
                    }

                    return Json(serializer);


                }
                catch (Exception ex)
                {
                    String aa = ex.ToString();
                    return Json(aa);
                }

            }

        }
    }
}