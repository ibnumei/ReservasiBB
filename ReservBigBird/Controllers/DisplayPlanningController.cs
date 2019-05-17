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
    public class DisplayPlanningController : Controller
    {
        // GET: DisplayPlanning
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _TableDisplayPlanning(ParamPlanning paramPlanning)
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
                    HttpResponseMessage message = client.GetAsync("http://10.0.19.165/BBWS/Api/Plans?popordid=" + paramPlanning.NoOrder + "&popnpk=" + paramPlanning.NamaPemakai + "&popnpm=" + paramPlanning.NamaPemesan + "&popid=&poppolid=" + paramPlanning.Pool + "&popdaow=").Result;

                    if (message.IsSuccessStatusCode)
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<DisplayPlanning>));
                        var result = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        List<DisplayPlanning> resultData = serializer.ReadObject(stream) as List<DisplayPlanning>;
                        ViewBag.data = resultData.ToList();


                        return PartialView("_TableDisplayPlanning", resultData.ToList());
                        //====================================================================================

                    }
                    else
                    {
                        return PartialView("_TableDisplayPlanning");
                    }
                    //if(message.)


                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                    return PartialView("_TableDisplayPlanning");
                }
            }

        }
    }
}