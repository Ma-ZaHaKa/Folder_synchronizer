using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNC_DIR
{
    public class HashMap
    {
        public List<string> FileHashes = new List<string>(); // можно ассоциативный сделать, с именами файлов

        public static HashMap Deserialize(string json)
        {
            try { return JsonConvert.DeserializeObject<HashMap>(json); }
            catch { }
            return null;
        }
        public static string Serialize(HashMap _vkr)
        {
            try { return JsonConvert.SerializeObject(_vkr, Formatting.Indented); }
            catch { }
            return null;
        }
    }
}
