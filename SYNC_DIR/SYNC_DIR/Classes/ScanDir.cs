using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNC_DIR
{
    public class ScanDir
    {
        public class Listing
        {
            public bool isdir = false;
            public string name = "";
        }
        public List<Listing> listing = new List<Listing>(); 
        public static ScanDir Deserialize(string json)
        {
            try { return JsonConvert.DeserializeObject<ScanDir>(json); }
            catch { }
            return null;
        }
        public static string Serialize(ScanDir _vkr)
        {
            try { return JsonConvert.SerializeObject(_vkr, Formatting.Indented); }
            catch { }
            return null;
        }
    }
}
