using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Utils.Common;
using WhatsAPI.UniversalApps.Libs.Utils.Encryptions;

namespace WhatsAPI.UniversalApps.Libs.Extensions
{
    public static class StringExtensions
    {
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string ToMD5String(this string s)
        {
            return MD5.Encrypt(s);
        }

        public static string ToShaString(this IEnumerable<char> s)
        {
            return new string(s.ToArray()).ToShaString();
        }

        public static string ToShaString(this String s)
        {
            var data = SHA1.EncryptBase64(s);
            var str = StringManipulation.UrlEncode(Encoding.GetEncoding("iso-8859-1").GetString(data, 0, data.Length));
            return str;
        }

        public static string GetJsonValue(this string s, string parameter)
        {
            Match match;
            return (match = Regex.Match(s, string.Format("\"?{0}\"?:\"(?<Value>.+?)\"", parameter), RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success ? match.Groups["Value"].Value : null;
        }

        public static string Reverse(this String text)
        {
            var cArray = text.ToCharArray();
            var reverse = String.Empty;
            for (var i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }


    }
}
