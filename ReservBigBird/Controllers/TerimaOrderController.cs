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
using System.Web.Script.Serialization;
using Newtonsoft.Json;

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
            ViewBag.JmlBus = penerimaOrder.JumlahBus;
            //===========================================
            ViewBag.TglAwal = penerimaOrder.tglawalpilih;
            ViewBag.JamAwal = penerimaOrder.jamawalpilih;
            ViewBag.TglAkhir = penerimaOrder.tglakhirpilih;
            ViewBag.JamAkhir = penerimaOrder.jamakhirpilih;
            ViewBag.KelTujuan = penerimaOrder.KelTujuan;
            //===========================================

            //untuk Consume API

            String ParamTglAwal = penerimaOrder.tglawalpilih.Substring(6, 4) + penerimaOrder.tglawalpilih.Substring(3, 2) + penerimaOrder.tglawalpilih.Substring(0, 2);
            String ParamTglAkhir = penerimaOrder.tglakhirpilih.Substring(6, 4) + penerimaOrder.tglakhirpilih.Substring(3, 2) + penerimaOrder.tglakhirpilih.Substring(0, 2);

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
                    HttpResponseMessage message = client.GetAsync(url+"/Api/GetStock?jenisbus="+penerimaOrder.JenisBus+"&pool=&jnbac=&tglawal="+ParamTglAwal+"&jamawal=&tglakhir="+ParamTglAkhir+"&jamakhir=").Result;

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

        //method with no return
        public void _TableHasilInputReal(ListPostTerimaOrder listPostTerimaOrder)
        //public void _TableHasilInput(ListPostTerimaOrder listPostTerimaOrder)
        {
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            DateTime now = DateTime.Now;

            listPostTerimaOrder.User = "Administrator";
            var datenow = now.ToString("dd/MM/yyyy");
            var timenow = now.ToString("HH:mm");
            listPostTerimaOrder.TglTrans = datenow;
            listPostTerimaOrder.JamTrans = timenow;

            ParamPostTerimaOrder postTerimaOrder = new ParamPostTerimaOrder()
            {
                userid = listPostTerimaOrder.User,
                jenis = listPostTerimaOrder.ModalJenis,
                pool = listPostTerimaOrder.ModalNmPool,
                tglawal = listPostTerimaOrder.ModalTglAwal,
                jamawal = listPostTerimaOrder.ModalJamAwal,
                tglakhir = listPostTerimaOrder.ModalTglAkhir,
                jamakhir = listPostTerimaOrder.ModalJamAkhir,
                jumlah = listPostTerimaOrder.ModalJmlBus,
                tgltrans = listPostTerimaOrder.TglTrans,
                jamtrans = listPostTerimaOrder.JamTrans,
                keltujuan = listPostTerimaOrder.ModalKelTujuan
            };

            var json2 = new JavaScriptSerializer().Serialize(postTerimaOrder);
            var stringPayload = JsonConvert.SerializeObject(postTerimaOrder);
            //String response = "";
            var credentials = new NetworkCredential("username", "password");
            var handler = new HttpClientHandler { Credentials = credentials };
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage message = client.PostAsync(url + "/api/GetStock", httpContent).Result;
                    if (message.IsSuccessStatusCode)
                    {
                        //Action jika mendapat balasan sukses dari post api, panggil method GetDataAfterPost
                        //GetDataAfterPost(listPostTerimaOrder);
                    }

                }
                catch
                {

                }
            }


        }

        //public ActionResult GetDataAfterPost(ListPostTerimaOrder listPostTerimaOrder)
        public ActionResult _TableHasilInput(ListPostTerimaOrder listPostTerimaOrder)
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
                    HttpResponseMessage message = client.GetAsync("https://jsonblob.com/api/b67aac55-90af-11e9-959d-b527025f20cc").Result;

                    if (message.IsSuccessStatusCode)
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<AfterPostTerimaOrder>));
                        var result = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        List<AfterPostTerimaOrder> resultData = serializer.ReadObject(stream) as List<AfterPostTerimaOrder>;

                        ViewBag.ResulData = resultData;
                        return PartialView("_TableHasilInput");
                        //====================================================================================

                    }
                    else
                    {
                        ViewBag.error = "Tidak Dapat Respon dari Server";
                        return PartialView("_TableHasilInput");
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.error = "Tidak Dapat Respon dari Server";
                    var error = ex.ToString();
                    return PartialView("_TableHasilInput");
                }
            }

        }

        [HttpPost]
        public ActionResult Delete(DeleteTerimaOrder deleteTerima)
        {
            //DeleteTerimaOrder deleteTerimaOrder = new DeleteTerimaOrder()
            //{
            //    Jenis = deleteTerima.Jenis
            //};

            //var json2 = new JavaScriptSerializer().Serialize(deleteTerimaOrder);
            //var stringPayload = JsonConvert.SerializeObject(deleteTerimaOrder);
            ////String response = "";
            //var credentials = new NetworkCredential("username", "password");
            //var handler = new HttpClientHandler { Credentials = credentials };
            //var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            //using (var client = new HttpClient(handler))
            //{
            //    try
            //    {
            //        HttpResponseMessage message = client.DeleteAsync("http://localhost:55300/api/ApiLoginNew/" + deleteTerima.Jenis).Result;
            //        if (message.IsSuccessStatusCode)
            //        {
            //            //Action jika mendapat balasan sukses dari post api, panggil method GetDataAfterPost
            //            //GetDataAfterPost(listPostTerimaOrder);
            //            var a = 10;
            //            var b = a;
            //        }


            //    }
            //    catch (Exception ex)
            //    {
            //        var aa = ex.ToString();
            //        var bb = aa;
            //    }
            //}

            return Json(new { success = true });
        }
    }
}