using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Utils.Common
{
    class StringManipulation
    {
        public static string UrlEncode(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                int i = (int)c;
                if (
                    (
                        i >= 0 && i <= 31
                    )
                    ||
                    (
                        i >= 32 && i <= 47
                    )
                    ||
                    (
                        i >= 58 && i <= 64
                    )
                    ||
                    (
                        i >= 91 && i <= 96
                    )
                    ||
                    (
                        i >= 123 && i <= 126
                    )
                    ||
                    i > 127
                )
                {
                    //encode 
                    sb.Append('%');
                    sb.AppendFormat("{0:x2}", (byte)c);
                }
                else
                {
                    //do not encode
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static void ExtractPhoneNumber(ref string phone)
        {
            if (phone.Contains("s.whatsapp"))
            {
                phone = phone.Replace("@s.whatsapp.net", "");
            }
            else
            {
                phone = phone.Replace("@g.us", "");
            }
        }
    }
}
