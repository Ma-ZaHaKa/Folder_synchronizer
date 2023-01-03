using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNC_DIR
{
    public class HashMapPair
    {
        public List<KeyValuePair<string, string>> Files = new List<KeyValuePair<string, string>>();

        public static HashMapPair Deserialize(string json)
        {
            try { return JsonConvert.DeserializeObject<HashMapPair>(json); }
            catch { }
            return null;
        }
        public static string Serialize(HashMapPair _vkr)
        {
            try { return JsonConvert.SerializeObject(_vkr, Formatting.Indented); }
            catch { }
            return null;
        }

    }
}
