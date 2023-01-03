using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SYNC_DIR
{
    partial class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // check inet
            // check file controller
            Config cfg = DeserealizeConfig("Config.json");
            if ((cfg == null) || (cfg.IsEmpty())) { throw new Exception("Config Error!!"); }
            try { SyncDirStart(cfg); }
            catch (Exception e) { Console.WriteLine($"Error! \n {e.Message}\n{e.StackTrace}"); Clipboard.SetText($"{e.Message}\r\n{e.StackTrace}"); Console.WriteLine("Press any key..."); Console.ReadKey(); }

            #region EXAMPLES
            //------------------EXAMPLES   BY    DIKTOR
            //Console.WriteLine(CheckFileAPI("222/15.php"));
            //Console.WriteLine(UploadAPI(dir + "\\" + "testfile.php", "222/15"));
            //Console.WriteLine(DeleteFileAPI("222/15.php"));
            //Console.WriteLine(GetFileHashAPI("222/15.php"));


            //Console.WriteLine(CheckDirAPI("222/15"));
            //Console.WriteLine(MKDirAPI("222/15/"));
            //Console.WriteLine(RMDirAPI("222/15"));
            //Console.WriteLine(CopyDirAPI("222/798/", "222/799/"));


            //Console.WriteLine(ZipAPI("222/798", "222/798.zip"));
            //Console.WriteLine(UnzipAPI("222/test.zip"));
            //Console.WriteLine(UnzipInZipNameAPI("222/test.zip"));
            //Console.WriteLine(UnzipInTargetAPI("222/test.zip", "222/3333"));
            #endregion
        }

        public static Config DeserealizeConfig(string _config = "Config.json")
        {
            if (!File.Exists(_config)) { throw new Exception($"Config file \"{_config}\" not found!"); }
            Config cfg_ = Config.Deserialize(File.ReadAllText(_config));
            cfg_.FixPathes();
            return cfg_;
        }

        public static void SyncDirStart(Config _cfg)
        {
            if (!Internet.isOK()) { throw new Exception("No Internet Connection!"); }
            if (!_cfg.CheckUrl()) { throw new Exception("Bad API URL in Config!"); }

            //Console.WriteLine(CheckRootDir("D:\\0SYNC\\22\\1.php", "D:\\0SYNC\\"));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Loading...\n");

            List<string> changes = SyncDir(_cfg); // true create syncdir name dir in server, false copy files syncdir to root
            foreach (var item in changes)
            {
                Console.WriteLine(item);
            }
            if (changes.Count == 0) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Nothing Update..."); }
            else { Console.WriteLine($"\nUpdate End! Total: {changes.Count}"); }
            System.Threading.Thread.Sleep(500);
            //Console.ReadKey();
        }

        public static List<string> SyncDir(Config _cfg) // return list changes
        { // синхронизация только содержимого относительно папки
            bool IsIdential(List<string> l1, List<string> l2)
            {
                if (l1.Count != l2.Count) { return false; }
                l1 = l1.OrderBy(x => x).ToList();
                l2 = l2.OrderBy(x => x).ToList();
                for (int i = 0; i < l1.Count; i++) { if (l1[i] != l2[i]) { return false; } }
                return true;
            }
            string ConvertPath(string filepath, Config __cfg) // подготовить путь для загрузки
            {
                string ReplaceRootDir(string path, string rootpath) //D:\\0SYNC\\123\\1.php     D:\\0SYNC\\  => 123\\1.php
                {
                    if (!path.Contains(rootpath) || (path == rootpath)) { throw new Exception($"BROKEN PATHS!! path -> {path}, rootpath -> {rootpath}"); }
                    else
                    {
                        rootpath = rootpath[rootpath.Length - 1] == '\\' ? rootpath : (rootpath + '\\'); // for remove slash for api root
                        return path.Remove(0, rootpath.Length); // or replace by diktor
                    }
                }
                string _serverdir = Path.GetFileName(__cfg.local_sync_dir); //D:\\0SYNC\\123 => 123
                string server_dir_path = __cfg.sync_name_api_dir ? $"{_serverdir}/{ReplaceRootDir(filepath, __cfg.local_sync_dir).Replace("\\", "/")}" : $"{ReplaceRootDir(filepath, __cfg.local_sync_dir).Replace("\\", "/")}";
                string tmp_filename = Path.GetFileName(filepath);  // filename.ext Path.GetDirectoryName()
                return server_dir_path.Remove(server_dir_path.Length - tmp_filename.Length, tmp_filename.Length); //srv path witout file
                //return Path.GetDirectoryName(server_dir_path); // разворачивает / на \
            }

            // sync_local_dir => D:\\0SYNC
            if (!Directory.Exists(_cfg.local_sync_dir)) { throw new Exception("DIR NOT EXISTS!!"); }
            string serverdir = Path.GetFileName(_cfg.local_sync_dir);
            HashMapPair current_map = HashAllDir(_cfg.local_sync_dir);

            if (_cfg.sync_name_api_dir) { MKDirAPI(_cfg, serverdir); }

            HashMapPair server_map = File.Exists(_cfg.hash_history_file) ? HashMapPair.Deserialize(File.ReadAllText(_cfg.hash_history_file)) : null;
            if ((server_map == null) || (_cfg.always_rewrite_all)) // ПОЛНЫЙ АПДЕЙТ
            {
                if (_cfg.clear_api_sync_dir) { ClsDirAPI(_cfg, _cfg.sync_name_api_dir ? $"{serverdir}/" : "./"); }
                for (int i = 0; i < current_map.Files.Count; i++)
                {
                    // Удаляю с полного локального пути основную папку и конвертирую её в сетевой путь
                    string server_dir_path = ConvertPath(current_map.Files[i].Key, _cfg);
                    /*string tmp_filename = Path.GetFileName(current_map.Files[i].Key);  // filename.ext
                    string server_dir_path = $"{serverdir}/{ReplaceRootDir(current_map.Files[i].Key, sync_local_dir).Replace("\\", "/")}";
                    server_dir_path = server_dir_path.Remove(server_dir_path.Length - tmp_filename.Length, tmp_filename.Length); //srv path witout file*/
                    UploadAPI(_cfg, current_map.Files[i].Key, server_dir_path);
                }
                File.WriteAllText(_cfg.hash_history_file, HashMapPair.Serialize(current_map));
                return current_map.Files.Select(x => $"Update: {x.Key}").ToList(); // WORK -- TESTING BY DIKTOR
            }
            else
            {
                bool edit = false;
                List<string> changes = new List<string>();

                // если разница только в хешах но иерархия не менялась
                if (IsIdential(current_map.Files.Select(x => x.Key).ToList(), server_map.Files.Select(x => x.Key).ToList()))
                {
                    current_map.Files = current_map.Files.OrderBy(x => x.Key).ToList();
                    server_map.Files = server_map.Files.OrderBy(x => x.Key).ToList();
                    for (int i = 0; i < current_map.Files.Count; i++)
                    {
                        if (server_map.Files[i].Value != current_map.Files[i].Value) // можно переделать под индекс и сравнение путей
                        {
                            // Удаляю с полного локального пути основную папку и конвертирую её в сетевой путь
                            string server_dir_path = ConvertPath(current_map.Files[i].Key, _cfg);
                            UploadAPI(_cfg, current_map.Files[i].Key, server_dir_path);
                            edit = true;
                            changes.Add($"Update: {current_map.Files[i].Key}");
                        }
                    }
                }
                else // ищи различия и обновляй/догружай/удаляй их
                {
                    List<string> current = current_map.Files.Select(x => x.Key).ToList(); // current paths
                    List<string> server = server_map.Files.Select(x => x.Key).ToList(); // server paths

                    List<string> to_upload = current.Except(server).ToList();
                    List<string> to_remove = server.Except(current).ToList();
                    List<string> to_check_hashes = current.Intersect(server).ToList(); ///-------

                    //-------UPDATE--FILES-----DIKTOR---03-01-2023---
                    for (int i = 0; i < to_check_hashes.Count; i++) // path only
                    {
                        //----------FIND--CURR---HASH-----//вспомнить хеш из списка 100 файлов что там есть
                        int index_curr = -1; // bool detect
                        for (int j = 0; j < current_map.Files.Count; j++) { if (current_map.Files[j].Key == to_check_hashes[i]) { index_curr = j; break; } }
                        if (index_curr == -1) { throw new Exception("INTERSELECT ERROR(ошибка в списках на обновление)"); }
                        // current_map.Files[index_curr].Value; current hash   to_check_hashes[i]; current filename

                        for (int j = 0; j < server_map.Files.Count; j++)
                        {
                            if (server_map.Files[j].Key == to_check_hashes[i]) // path check
                            {
                                if (server_map.Files[j].Value != current_map.Files[index_curr].Value) // hash check
                                {
                                    string server_dir_path = ConvertPath(to_check_hashes[i], _cfg);
                                    UploadAPI(_cfg, to_check_hashes[i], server_dir_path);
                                    edit = true;
                                    changes.Add($"Update: {to_check_hashes[i]}");
                                }
                                else { break; }
                            }
                        }
                    }
                    //---------------------END--UPDATE-------------------


                    //------REMOVE--FILES-------------------------------
                    for (int i = 0; i < to_remove.Count; i++)
                    {
                        string server_dir_path = ConvertPath(to_remove[i], _cfg);
                        DeleteFileAPI(_cfg, server_dir_path + Path.GetFileName(to_remove[i]));
                        edit = true;
                        changes.Add($"Remove: {to_remove[i]}");
                    }
                    //------END---REMOVE--------------------------------


                    //------UPLOAD--FILES-------------------------------
                    for (int i = 0; i < to_upload.Count; i++)
                    {
                        string server_dir_path = ConvertPath(to_upload[i], _cfg);
                        UploadAPI(_cfg, to_upload[i], server_dir_path);
                        edit = true;
                        changes.Add($"Upload: {to_upload[i]}");
                    }
                    //------END---UPLOAD--------------------------------
                }

                if (edit) { File.WriteAllText(_cfg.hash_history_file, HashMapPair.Serialize(current_map)); }
                return changes;
            }
        }




        #region HASH DIR
        public static HashMapPair HashAllDir(string path)
        {
            List<string> paths = new List<string>();
            void CollectPaths(string dir)
            {
                foreach (string s1 in Directory.GetFiles(dir)) { paths.Add(s1); }
                foreach (string s in Directory.GetDirectories(dir)) { CollectPaths(dir + "\\" + Path.GetFileName(s)); }
            }

            CollectPaths(path);
            HashMapPair hm = new HashMapPair();
            //hm.Files = hm.FileHashes.OrderBy(x => x).ToList();
            for (int i = 0; i < paths.Count; i++) { hm.Files.Add(new KeyValuePair<string, string>(paths[i], checkMD5(paths[i]))); } // path, hash
            return hm;
        }

        public static HashMap HashDir(string path) //only files
        {
            HashMap hm = new HashMap();
            hm.FileHashes.AddRange(Directory.GetFiles(path));
            hm.FileHashes = hm.FileHashes.OrderBy(x => x).ToList();
            for (int i = 0; i < hm.FileHashes.Count; i++) { hm.FileHashes[i] = checkMD5(hm.FileHashes[i]); } // hash all
            return hm;
        }

        #endregion



        public static void LocalCopyDir(string FromDir, string ToDir)
        {
            Directory.CreateDirectory(ToDir);
            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + "\\" + Path.GetFileName(s1);
                File.Copy(s1, s2);
            }
            foreach (string s in Directory.GetDirectories(FromDir))
            {
                LocalCopyDir(s, ToDir + "\\" + Path.GetFileName(s));
            }
        }

        public static string checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty); // to upper default
                }
            }
        }




        //------------UPLOAD CLASSES
        public class UploadFile
        {
            /* EXAMPLE
            using (var stream = File.Open(file, FileMode.Open))
            {
                var files = new[]
                {
                    new UploadFile
                    {
                        Name = "file1",
                        Filename = Path.GetFileName(file),
                        ContentType = "text/plain",
                        Stream = stream
                    }
                };

                var values = new NameValueCollection
                {
                    { "client", "VIP" },
                    { "name", "John Doe" },
                };

                byte[] result = UploadFiles(QUERY, files, values);
            }
             */
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public Stream Stream { get; set; }
        }


        public static byte[] UploadFiles(string address, IEnumerable<UploadFile> files, NameValueCollection values)
        {
            var request = WebRequest.Create(address);
            request.Method = "POST";
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var stream = new MemoryStream())
            {
                responseStream.CopyTo(stream);
                return stream.ToArray();
            }
        }
        public static string Upload(string URL, string PathToFile, string FormName = "file", string FileName = "") // telegram -> document(file form name)
        {
            using (var stream = File.Open(PathToFile, FileMode.Open))
            {
                var files = new[]
                {
                    new UploadFile
                    {
                        Name = FormName,
                        Filename = FileName == "" ? Path.GetFileName(PathToFile) : FileName,
                        ContentType = "text/plain",
                        Stream = stream
                    }
                };

                /*var values = new NameValueCollection //POST
                {
                    { "client", "VIP" },
                    { "name", "John Doe" },
                };*/

                var values = new NameValueCollection();

                return Encoding.UTF8.GetString(UploadFiles(URL, files, values));
            }

        }
    }
}
