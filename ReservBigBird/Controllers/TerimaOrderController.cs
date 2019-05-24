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

namespace ReservBigBird.Controllers
{
    public class TerimaOrderController : Controller
    {
        // GET: TerimaOrder
        public ActionResult Index()
        {
            ViewBag.Current = "1";
            return View();
        }

        public ActionResult _TableDate(PenerimaOrder penerimaOrder)
        {
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            //Membuat range tanggal
            String tglawal = penerimaOrder.tglawalpilih.Substring(3, 2) + "/" + penerimaOrder.tglawalpilih.Substring(0, 2) + "/" + penerimaOrder.tglawalpilih.Substring(6, 4);

            String tglakhir = penerimaOrder.tglakhirpilih.Substring(3, 2) + "/" + penerimaOrder.tglakhirpilih.Substring(0, 2) + "/" + penerimaOrder.tglakhirpilih.Substring(6, 4);

            DateTime awal = DateTime.Parse(tglawal);
            DateTime akhir = DateTime.Parse(tglakhir);

            List<String> allDates = new List<String>();

            int starting = awal.Day;
            int ending = akhir.Day;

            for (DateTime date = awal; date <= akhir; date = date.AddDays(1))
            {
                allDates.Add(date.ToString("dd-MM-yyyy"));
            }

            ViewBag.bus = penerimaOrder.JenisBus;
            ViewBag.tgl = allDates.ToList();


            //untuk Consume API
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
                    //HttpResponseMessage message = client.GetAsync("https://jsonblob.com/api/3d30a209-7c3c-11e9-8e48-3b0225e23be5").Result;
                    HttpResponseMessage message = client.GetAsync("http://10.0.19.122/BBWS/Api/GetStock?jenisbus=A54&pool=&jnbac=&tglawal=20190119&jamawal=&tglakhir=20190122&jamakhir=").Result;

                    if (message.IsSuccessStatusCode)
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<TerimaOrder>));
                        var result = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        List<TerimaOrder> resultData = serializer.ReadObject(stream) as List<TerimaOrder>;
                        //ViewBag.data = resultData.ToList();
                        //for (int i = 0; i < resultData.Count; i++)
                        //{
                        //    var bb = resultData[i].username;

                        //    ViewBag.hasil = "Sukses mendapatkan data";

                        //    return View();
                        //}

                        //ViewBag.tes1 = resultData.data.ToList();
                        return PartialView("_TableDate", resultData);
                        //====================================================================================

                    }
                    else
                    {
                        ViewBag.error = "Tidak Dapat Respon dari Server";
                        return PartialView("_TableDate");
                    }
                    //if(message.)


                }
                catch (Exception ex)
                {
                    ViewBag.error = "Tidak Dapat Respon dari Server";
                    var error = ex.ToString();
                    return PartialView("_TableDate");
                }
            }

            //return PartialView("_TableDate", allDates.ToList());
        }
    }
}