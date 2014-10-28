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
    }
}
