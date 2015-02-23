using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs;

namespace WhatsAPI.UniversalApps.Sample
{
    class SocketInstance
    {
        private static WhatsApp _instance;

        public static WhatsApp Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                else
                {
                    throw new Exception("Instance is not set");
                }
            }
        }

        public static void Create(string username, string password, string nickname, bool debug = false)
        {
            _instance = new WhatsApp(username, password, nickname, true,false);
        }
    }
}
