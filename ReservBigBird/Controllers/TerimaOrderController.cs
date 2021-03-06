﻿using ReservBigBird.API_Model;
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
using System.Globalization;
using ReservBigBird.Filters;

namespace ReservBigBird.Controllers
{
    [UserAuthenticationFilter]

    public class TerimaOrderController : Controller
    {
        
        // GET: TerimaOrder
        public ActionResult Index()
        {
            List<string> Getbus = new List<string>();
            int status = 0;
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            //Method untuk consume api
            String response = "";
            var credentials = new NetworkCredential("ac", "123");
            var handler = new HttpClientHandler { Credentials = credentials };
            using (var client = new HttpClient(handler))
            {
                // Make your request...
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpResponseMessage message = client.GetAsync(url + "/api/JNBs").Result;

                    if (message.IsSuccessStatusCode)
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<JNB>));
                        var result = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        List<JNB> resultData = serializer.ReadObject(stream) as List<JNB>;
                        //var aa = resultData.ToList();
                        foreach (var aa in resultData.ToList())
                        {
                            Getbus.Add(aa.JNBID);
                        }
                        
                        status = 1;
                        //return Json(resultData.ToList());
                        //return PartialView("_TableDisplayPlanning", resultData.ToList());
                        //====================================================================================

                    }
                    else
                    {
                        status = 0;
                        ViewBag.error = "Tidak Dapat Respon dari Server";
                        //return PartialView("_TableDisplayPlanning");
                    }

                }
                catch (Exception ex)
                {
                    status = 0;
                    ViewBag.error = "Tidak Dapat Respon dari Server";
                    var error = ex.ToString();
                    //return PartialView("_TableDisplayPlanning");
                }
            }

            //=======================================================
            //if(status == 0)
            //{

            //}
            //else
            //{

            //}
            ViewBag.dataBus = Getbus;
            ViewBag.Current = "1";
            ViewBag.CekDataVar = "CekDataValue";
            return View();
        }

        //Submit form di pop up keep
        [HttpPost]
        public ActionResult Index(PostHeaderKeep postHeader)
        {
            DateTime now = DateTime.Now;


            postHeader.TglJemput = postHeader.TglJemput.Substring(6, 4) + "" + postHeader.TglJemput.Substring(3, 2) + "" + postHeader.TglJemput.Substring(0, 2);

            postHeader.JamJemput = postHeader.JamJemput.Substring(0, 2) + "" + postHeader.JamJemput.Substring(3, 2);

            postHeader.TglSelesei = postHeader.TglSelesei.Substring(6, 4) + "" + postHeader.TglSelesei.Substring(3, 2) + "" + postHeader.TglSelesei.Substring(0, 2);

            postHeader.JamSelesei = postHeader.JamSelesei.Substring(0, 2) + "" + postHeader.JamSelesei.Substring(3, 2);

            //Var gagal ato sukses
            String YesOrNo;

            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            //Ambil session username
            postHeader.UserId = Session["userid"].ToString();

            var json2 = new JavaScriptSerializer().Serialize(postHeader);
            var stringPayload = JsonConvert.SerializeObject(postHeader);
            //String response = "";
            var credentials = new NetworkCredential("username", "password");
            var handler = new HttpClientHandler { Credentials = credentials };
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            using (var client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage message = client.PostAsync(url + "/api/Orders", httpContent).Result;
                    if (message.IsSuccessStatusCode)
                    {
                        YesOrNo = "Yes";
                    }
                    else
                    {
                        YesOrNo = "No";
                    }

                    //Cek message gagal atau tidak
                    if(YesOrNo == "Yes")
                    {
                        var serializer = new DataContractJsonSerializer(typeof(GetAfterPostHeader));
                        var result = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        GetAfterPostHeader resultData = serializer.ReadObject(stream) as GetAfterPostHeader;

                        var aa = resultData.noorder;

                        TempData["NoOrderTemp"] = aa;
                        return Redirect("MonitorOrder/Index");
                    }
                    else
                    {
                        return View();
                    }
                    

                }
                catch (Exception ex)
                {
                    String aa = ex.ToString();
                    return View();
                }

             }

             
        }

        public ActionResult _TableDate(PenerimaOrder penerimaOrder)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            //Membuat range tanggal
            String tglawal = penerimaOrder.tglawalpilih.Substring(3, 2) + "/" + penerimaOrder.tglawalpilih.Substring(0, 2) + "/" + penerimaOrder.tglawalpilih.Substring(6, 4);

            String tglakhir = penerimaOrder.tglakhirpilih.Substring(3, 2) + "/" + penerimaOrder.tglakhirpilih.Substring(0, 2) + "/" + penerimaOrder.tglakhirpilih.Substring(6, 4);

            DateTime awal = DateTime.ParseExact(tglawal, "MM/dd/yyyy",provider);
            DateTime akhir = DateTime.ParseExact(tglakhir, "MM/dd/yyyy", provider);

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
                    
                    HttpResponseMessage message = client.GetAsync(url+"/Api/GetStock?jenisbus=" + penerimaOrder.JenisBus+"&pool=&jnbac=&tglawal="+ParamTglAwal+"&jamawal=&tglakhir="+ParamTglAkhir+"&jamakhir=").Result;

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
        //public void _TableHasilInputNotReal(ListPostTerimaOrder listPostTerimaOrder)
        public ActionResult _TableHasilInput(ListPostTerimaOrder listPostTerimaOrder)
        {
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            String sessionname = Session["userid"].ToString();

            //JIKA PARAMETER NULL digunakan untuk reload setelah delete
            if (listPostTerimaOrder.ModalNmPool != null)
            {
                

                DateTime now = DateTime.Now;

                listPostTerimaOrder.User = sessionname;

                listPostTerimaOrder.ModalTglAwal = listPostTerimaOrder.ModalTglAwal.Substring(6, 4) + "" + listPostTerimaOrder.ModalTglAwal.Substring(3, 2) + "" + listPostTerimaOrder.ModalTglAwal.Substring(0, 2);

                if(Convert.ToInt64(listPostTerimaOrder.ModalJamAwal) < 10)
                {
                    //listPostTerimaOrder.ModalJamAwal = listPostTerimaOrder.ModalJamAwal.Substring(0, 2) + "" + listPostTerimaOrder.ModalJamAwal.Substring(3, 2);
                    listPostTerimaOrder.ModalJamAwal = "0"+listPostTerimaOrder.ModalJamAwal+"00";
                }
                else
                {
                    listPostTerimaOrder.ModalJamAwal = listPostTerimaOrder.ModalJamAwal + "00";
                }
                

                listPostTerimaOrder.ModalTglAkhir = listPostTerimaOrder.ModalTglAkhir.Substring(6, 4) + "" + listPostTerimaOrder.ModalTglAkhir.Substring(3, 2) + "" + listPostTerimaOrder.ModalTglAkhir.Substring(0, 2);

                if (Convert.ToInt64(listPostTerimaOrder.ModalJamAkhir) < 10)
                {
                    //listPostTerimaOrder.ModalJamAkhir = listPostTerimaOrder.ModalJamAkhir.Substring(0, 2) + "" + listPostTerimaOrder.ModalJamAkhir.Substring(3, 2);
                    listPostTerimaOrder.ModalJamAkhir = "0" + listPostTerimaOrder.ModalJamAkhir + "00";
                }
                else
                {
                    listPostTerimaOrder.ModalJamAkhir = listPostTerimaOrder.ModalJamAkhir + "00";
                }
                

                var datenow = now.ToString("yyyyMMdd");
                var timenow = now.ToString("HHmm");
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
                    keltujuan = listPostTerimaOrder.ModalKelTujuan,
                    ac = "Y"
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

                            //=====================================================================
                            String response = "";
                            var credentials2 = new NetworkCredential("ac", "123");
                            var handler2 = new HttpClientHandler { Credentials = credentials2 };
                            using (var client2 = new HttpClient(handler2))
                            {
                                // Make your request...
                                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                try
                                {
                                    //HttpResponseMessage message = client.GetAsync("https://jsonblob.com/api/b67aac55-90af-11e9-959d-b527025f20cc").Result;
                                    HttpResponseMessage message2 = client2.GetAsync(url + "/Api/GetSbu?usrid="+ sessionname).Result;

                                    if (message2.IsSuccessStatusCode)
                                    {
                                        var serializer = new DataContractJsonSerializer(typeof(List<AfterPostTerimaOrder>));
                                        var result = message2.Content.ReadAsStringAsync().Result;
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
                            //=====================================================================
                        }
                        else
                        {
                            return PartialView("_TableHasilInput");
                        }

                    }
                    catch
                    {
                        return PartialView("_TableHasilInput");
                    }
                }
            }
            else
            {
                //=====================================================================
                String response = "";
                var credentials2 = new NetworkCredential("ac", "123");
                var handler2 = new HttpClientHandler { Credentials = credentials2 };
                using (var client2 = new HttpClient(handler2))
                {
                    // Make your request...
                    client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        //HttpResponseMessage message = client.GetAsync("https://jsonblob.com/api/b67aac55-90af-11e9-959d-b527025f20cc").Result;
                        HttpResponseMessage message2 = client2.GetAsync(url + "/Api/GetSbu?usrid="+ sessionname).Result;

                        if (message2.IsSuccessStatusCode)
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<AfterPostTerimaOrder>));
                            var result = message2.Content.ReadAsStringAsync().Result;
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
                //=====================================================================
            }



        }

        public ActionResult GetDataAfterPost(ListPostTerimaOrder listPostTerimaOrder)
        //public ActionResult _TableHasilInput(ListPostTerimaOrder listPostTerimaOrder)
        {
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

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
                    //HttpResponseMessage message = client.GetAsync("https://jsonblob.com/api/b67aac55-90af-11e9-959d-b527025f20cc").Result;
                    HttpResponseMessage message = client.GetAsync(url+ "/Api/GetSbu?usrid=Administrator").Result;

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
        public JsonResult Delete(DeleteTerimaOrder deleteTerima)
        {
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            DeleteTerimaOrder deleteTerimaOrder = new DeleteTerimaOrder()
            {
                Jenis = deleteTerima.Jenis
                
            };

            var json2 = new JavaScriptSerializer().Serialize(deleteTerimaOrder);
            var stringPayload = JsonConvert.SerializeObject(deleteTerimaOrder);
            //String response = "";
            var credentials = new NetworkCredential("username", "password");
            var handler = new HttpClientHandler { Credentials = credentials };
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage message = client.DeleteAsync(url+ "/api/GetStock?userid=Administrator&jenis="+deleteTerima.Jenis+"&ac="+deleteTerima.AC+"&pool="+deleteTerima.Pool+"&tglawal="+deleteTerima.TglAwal+"&jamawal="+deleteTerima.JamAwal+"&tglakhir="+deleteTerima.TglAkhir+"&jamakhir="+deleteTerima.JamAkhir+"&keltujuan="+ deleteTerima.KelTujuan).Result;

                    if (message.IsSuccessStatusCode)
                    {

                        //return Json(new { success = true });
                        //return "{\"msg\":\"success\"}";
                        //return Json("Delete");
                        return Json("delete", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //return Json(new { success = false });
                        return Json("error", JsonRequestBehavior.AllowGet);
                    }


                }
                catch (Exception ex)
                {
                    //return Json(new { success = false });
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }

            //return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult NewFuncDel(DeleteTerimaOrderNew deleteTerimaOrderNew)
        {
            //Ambil link url di web config
            String url = ConfigurationManager.AppSettings["UrlApi"].ToString();

            String sessionname = Session["userid"].ToString();

            deleteTerimaOrderNew.userid = sessionname;

            var json2 = new JavaScriptSerializer().Serialize(deleteTerimaOrderNew);
            var stringPayload = JsonConvert.SerializeObject(deleteTerimaOrderNew);
            //String response = "";
            var credentials = new NetworkCredential("username", "password");
            var handler = new HttpClientHandler { Credentials = credentials };
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient(handler))
            {
                try
                {
                    //url: apiUrl + "/api/GetStock?userid=Administrator&jenis=" + a + "&ac=" + b + "&pool=" + c + "&tglawal=" + d + "&jamawal=" + e + "&tglakhir=" + f + "&jamakhir=" + g + "&keltujuan=" + k,
                    HttpResponseMessage message = client.DeleteAsync(url + "/api/GetStock?userid="+deleteTerimaOrderNew.userid+"&jenis=" + deleteTerimaOrderNew.Jenis + "&ac=" + deleteTerimaOrderNew.AC + "&pool=" + deleteTerimaOrderNew.Pool + "&tglawal=" + deleteTerimaOrderNew.TglAwal + "&jamawal=" + deleteTerimaOrderNew.JamAwal + "&tglakhir=" + deleteTerimaOrderNew.TglAkhir + "&jamakhir=" + deleteTerimaOrderNew.JamAkhir + "&keltujuan=" + deleteTerimaOrderNew.KelTujuan).Result;

                    if (message.IsSuccessStatusCode)
                    {

                        //return Json(new { success = true });
                        //return "{\"msg\":\"success\"}";
                        //return Json("Delete");
                        return Json(new { success = true, responseText = "The attached file is not supported." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //return Json(new { success = false });
                        return Json(new { success = false, responseText = "The attached file is not supported." }, JsonRequestBehavior.AllowGet);
                    }


                }
                catch (Exception ex)
                {
                    //return Json(new { success = false });
                    return Json(new { success = false, responseText = "The attached file is not supported." }, JsonRequestBehavior.AllowGet);
                }
            }

        }
    }
}