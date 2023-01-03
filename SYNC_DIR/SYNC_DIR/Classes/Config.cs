using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SYNC_DIR
{
    public class Config
    {
        public string controller_api_url = "";
        public string controller_root_path = "";
        public string local_sync_dir = "";
        public bool always_rewrite_all = false;
        public bool clear_api_sync_dir = false;
        public string hash_history_file = "";
        public bool sync_name_api_dir = true;

        public static Config Deserialize(string json)
        {
            try { return JsonConvert.DeserializeObject<Config>(json); }
            catch { }
            return null;
        }
        public static string Serialize(Config _vkr)
        {
            try { return JsonConvert.SerializeObject(_vkr, Formatting.Indented); }
            catch { }
            return null;
        }
        public bool CheckUrl()
        {
            try { new WebClient().DownloadString(this.controller_api_url); return true; }
            catch { }
            return false;
        }
        public void FixPathes()
        {
            if (this.IsEmpty()) { return; }
            this.controller_root_path = this.controller_root_path[this.controller_root_path.Length - 1] == '/' ? this.controller_root_path : this.controller_root_path + '/'; // not critical path fix
            this.local_sync_dir = this.local_sync_dir[this.local_sync_dir.Length - 1] == '\\' ? this.local_sync_dir.Remove(this.local_sync_dir.Length - 1, 1) : this.local_sync_dir; // path get dir name fix -> Path.GetFileName
        }

        public bool IsEmpty()
         => (this.controller_api_url == "") || (this.controller_root_path == "") || (this.local_sync_dir == "") || (this.hash_history_file == "");
    }
}
