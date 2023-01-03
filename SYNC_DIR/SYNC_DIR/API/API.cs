using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SYNC_DIR
{
    partial class Program
    {
        //--------------------API-----------------------------------------------
        //---ADD rmfile
        public static bool CheckFileAPI(Config _cfg, string file) // РАБОТАЕТ ЖЕЛЕЗНО
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=check&file=" + file) == "EXIST";
        }
        public static bool UploadAPI(Config _cfg, string localfile, string path) // РАБОТАЕТ ЖЕЛЕЗНО НО БЕЗ ОГРАНИЧЕНИЯ ПО РАЗМЕРУ ФАЙЛА
        {
            return Upload(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=upload&file=" + path, localfile) == "UPLOAD";
        }
        public static bool DeleteFileAPI(Config _cfg, string file) // РАБОТАЕТ ЖЕЛЕЗНО
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=rmfile&file=" + file) == "RMFILE OK";
        }
        public static string GetFileHashAPI(Config _cfg, string file) // РАБОТАЕТ ЖЕЛЕЗНО
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=hash&file=" + file);
        }


        //-----------------------------
        // add hash dir

        public static bool CheckDirAPI(Config _cfg, string file)//------  РАБОТАЕТ ЖЕЛЕЗНО
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=checkdir&file=" + file) == "EXIST";
        }
        public static bool MKDirAPI(Config _cfg, string path) //------   РАБОТАЕТ ЖЕЛЕЗНО 
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=mkdir&file=" + path) == "MKDIR OK";
        }
        public static bool RMDirAPI(Config _cfg, string path) //------   РАБОТАЕТ ЖЕЛЕЗНО 
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=rmdir&file=" + path) == "RMDIR OK";
        }
        public static bool ClsDirAPI(Config _cfg, string path) //------   РАБОТАЕТ ЖЕЛЕЗНО 
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=clsdir&file=" + path) == "CLSDIR OK";
        }
        public static bool CopyDirAPI(Config _cfg, string pathfrom, string pathto) //------   РАБОТАЕТ ЖЕЛЕЗНО 
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=copy&file=" + pathfrom + "&pathto=" + pathto) == "COPY OK";
        }

        //-----------------------------
        public static bool ZipAPI(Config _cfg, string path, string file) //------   РАБОТАЕТ ЖЕЛЕЗНО
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=zip&file=" + file + "&path=" + path) == "ZIP OK";
        }
        public static bool UnzipAPI(Config _cfg, string file) //------   РАБОТАЕТ ЖЕЛЕЗНО РАСПАКОВКА ПРЯМО ТУДА ГДЕ ЛЕЖИТ АРХИВ
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=unzip&file=" + file) == "UNZIP OK";
        }
        public static bool UnzipInZipNameAPI(Config _cfg, string file) //------   РАБОТАЕТ ЖЕЛЕЗНО СОЗДАНИЕ ПАПКИ ИМЕНЕМ АРХИВА, РАСПАКОВКА
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=unzipinzipname&file=" + file) == "UNZIPINZIPNAME OK";
        }
        public static bool UnzipInTargetAPI(Config _cfg, string file, string path) //------   РАБОТАЕТ ЖЕЛЕЗНО СОЗДАНИЕ ПАПКИ ИМЕНЕМ АРХИВА, РАСПАКОВКА
        {
            return new WebClient().DownloadString(_cfg.controller_api_url + "?root=" + _cfg.controller_root_path + "&action=unzipintarget&file=" + file + "&toextract=" + path) == "UNZIPINTARGET OK";
        }

        //-----------------------------------------------------------------------


    }
}
