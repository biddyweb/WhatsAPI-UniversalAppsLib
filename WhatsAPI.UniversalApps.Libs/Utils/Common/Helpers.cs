using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Utils.Encryptions;

namespace WhatsAPI.UniversalApps.Libs.Utils.Common
{
    public class Helpers
    {
        public static string GeneratePaymentLink(string number, int sku)
        {
            if (sku != 1 || sku != 3 || sku != 5)
                sku = 1;
            var baseUrl = "https://www.whatsapp.com/payments/cksum_pay.php?phone=";
            var middle = "&cksum=";
            var end = "&sku=" + sku;
            var checksum = MD5.Encrypt(number + "abc");
            var link = baseUrl + number + middle + checksum + end;
            return link;
        }

        public static string StrToHex(string str)
        {
            string hex = "0x";
            for (int i = 0; i < str.Length; i++)
                hex += ((int)str[i]).ToString("x");
            return hex;
        }

        public static string HexString2Ascii(string hexString)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= hexString.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }

        public static bool IsShort(string value)
        {
            return value.Length < 256;
        }
        public static int StrLenWa(string str)
        {
            int len = str.Length;
            if (len >= 256)
                len = len & 0xFF00 >> 8;
            return len;
        }
        public static string _hex(int val)
        {
            return (val.ToString("X").Length % 2 == 0) ? val.ToString("X") : ("0" + val.ToString("X"));
        }
        public static string RandomUUID()
        {
            var mt_rand = new Random();
            return string.Format("{0}{1}-{2}-{3}-{4}-{5}{6}{7}",
            mt_rand.Next(0, 0xffff), mt_rand.Next(0, 0xffff),
            mt_rand.Next(0, 0xffff),
            mt_rand.Next(0, 0x0fff) | 0x4000,
            mt_rand.Next(0, 0x3fff) | 0x8000,
            mt_rand.Next(0, 0xffff), mt_rand.Next(0, 0xffff), mt_rand.Next(0, 0xffff)
            );
        }
      
        public static long GetUnixTimestamp(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            return (long)span.TotalSeconds;
        }
        public static DateTime GetDateTimeFromTimestamp(string timestamp)
        {
            long data = 0;
            if (long.TryParse(timestamp, out data))
            {
                return GetDateTimeFromTimestamp(data);
            }
            return DateTime.Now;
        }
        protected static DateTime GetDateTimeFromTimestamp(long timestamp)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return UnixEpoch.AddSeconds(timestamp);
        }
        public static long GetNowUnixTimestamp()
        {
            return GetUnixTimestamp(DateTime.UtcNow);
        }

        public static bool ArrayEqual(byte[] b1, byte[] b2)
        {
            int len = b1.Length;
            if (b1.Length != b2.Length)
                return false;
            for (int i = 0; i < len; i++)
            {
                if (b1[i] != b2[i])
                    return false;
            }
            return true;
        }

#region Jid Helper
        public static string GetJID(string target)
        {
            target = target.TrimStart(new char[] { '+', '0' });
            if (!target.Contains('@'))
            {
                //check if group message
                if (target.Contains('-'))
                {
                    //to group
                    target += "@g.us";
                }
                else
                {
                    //to normal user
                    target += "@s.whatsapp.net";
                }
            }
            return target;
        }
#endregion
    }
}
