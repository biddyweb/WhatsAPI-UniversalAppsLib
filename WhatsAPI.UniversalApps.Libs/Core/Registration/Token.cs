using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Utils.Encryptions;

namespace WhatsAPI.UniversalApps.Libs.Core.Registration
{
    public class Token
    {
        public static string GenerateToken(string number)
        {
            List<byte> key = new List<byte>(Convert.FromBase64String("/UIGKU1FVQa+ATM2A0za7G2KI9S/CwPYjgAbc67v7ep42eO/WeTLx1lb1cHwxpsEgF4+PmYpLd2YpGUdX/A2JQitsHzDwgcdBpUf7psX1BU="));

            List<byte> data = new List<byte>(Convert.FromBase64String(Constants.Token.Signature));
            data.AddRange(Convert.FromBase64String(Constants.Token.ClassMD5));
            data.AddRange(System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(number));

            List<byte> opad = GetFilledList(0x5C, 64);
            List<byte> ipad = GetFilledList(0x36, 64);
            for (int i = 0; i < opad.Count; i++)
            {
                opad[i] = (byte)(opad[i] ^ key[i]);
                ipad[i] = (byte)(ipad[i] ^ key[i]);
            }



            ipad.AddRange(data);
            var ipadString = Encoding.GetEncoding("iso-8859-1").GetString(ipad.ToArray(), 0, ipad.ToArray().Length);
            data = new List<byte>(SHA1.EncryptBase64(ipad.ToArray()));
            opad.AddRange(data);
            data = new List<byte>(SHA1.EncryptBase64(opad.ToArray()));

            return Convert.ToBase64String(data.ToArray());
        }

        private static List<byte> GetFilledList(byte item, int length)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
