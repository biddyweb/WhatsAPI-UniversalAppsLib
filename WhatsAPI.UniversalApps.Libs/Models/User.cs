using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Utils.Common;

namespace WhatsAPI.UniversalApps.Libs.Models
{
    public class User
    {
        private string serverUrl;
        public string Nickname { get; set; }
        public string Jid { get; private set; }
        public User(string jid, string srvUrl, string nickname = "")
        {
            this.Jid = jid;
            this.Nickname = nickname;
            this.serverUrl = srvUrl;
        }
        public string GetFullJid()
        {
            return Helpers.GetJID(this.Jid);
        }
        internal void SetServerUrl(string srvUrl)
        {
            this.serverUrl = srvUrl;
        }
        public override string ToString()
        {
            return this.GetFullJid();
        }
    }
}
