using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservBigBird.API_Model
{
    public class ModelAPI
    {
    }

    public class MonitorOrder
    {
        public string NoItem { get; set; }
        public string NoOrder { get; set; }
        public string Sts { get; set; }
        public string Disc { get; set; }
        public string Jenis { get; set; }
        public string Pool { get; set; }
        public string KelTujuan { get; set; }
        public string AlamatJemput { get; set; }
        public string TglJemput { get; set; }
        public string JamJemput { get; set; }
        public string TglSelesei { get; set; }
        public string JamSelesei { get; set; }
    }

    public class ParamMonitorOrder
    {
        public String KondisiOrder { get; set; }
        public String NoOrder { get; set; }
        public String Perusahaan { get; set; }
        public String Pemesan { get; set; }
    }

    public class DisplayPlanning
    {
        public int NoDetail { get; set; }
        public string NoOrder { get; set; }
        public string TglPakai { get; set; }
        public string JamJemput { get; set; }
        public string Bus { get; set; }
        public string Nip1 { get; set; }
        public string Hp { get; set; }
        public string Popnpk { get; set; }
        public int Popid { get; set; }
        public string Popnpm { get; set; }
        public string Poppolid { get; set; }
        public string popdaow { get; set; }
    }

    public class ParamPlanning
    {
        public String NoOrder { get; set; }
        public String NamaPemakai { get; set; }
        public String NamaPemesan { get; set; }
        public String Pool { get; set; }
    }
}