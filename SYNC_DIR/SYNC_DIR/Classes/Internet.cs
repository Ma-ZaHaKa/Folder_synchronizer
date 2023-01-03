using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SYNC_DIR
{
    class Internet
    {
        public static bool isOK()
        {
            try
            {
                Dns.GetHostEntry("google.com");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
