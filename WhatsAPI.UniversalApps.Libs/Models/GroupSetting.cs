using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GroupSetting
    {
        public string Jid;
        public bool Enabled;
        public DateTime? MuteExpiry;
    }
}
