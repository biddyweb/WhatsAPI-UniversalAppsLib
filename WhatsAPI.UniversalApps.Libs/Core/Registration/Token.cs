using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Utils.Encryptions;
using WhatsAPI.UniversalApps.Libs.Extensions;

namespace WhatsAPI.UniversalApps.Libs.Core.Registration
{
    public class Token
    {
        public static string GenerateToken(string number)
        {
            List<byte> key = new List<byte>(Convert.FromBase64String("/UIGKU1FVQa+ATM2A0za7G2KI9S/CwPYjgAbc67v7ep42eO/WeTLx1lb1cHwxpsEgF4+PmYpLd2YpGUdX/A2JQitsHzDwgcdBpUf7psX1BU="));

            List<byte> data = new List<byte>(Convert.FromBase64String(Constants.Token.Signature));
            data.AddRange(Convert.FromBase64String(Constants.Token.ClassMD5));
            
            var strByte = System.Text.Encoding.GetEncoding(Constants.Information.ASCIIEncoding).GetBytes(number);
            data.AddRange(strByte);
          
            List<byte> opad = GetFilledList(0x5C, 64);
            List<byte> ipad = GetFilledList(0x36, 64);
            for (int i = 0; i < opad.Count; i++)
            {
                opad[i] = (byte)(opad[i] ^ key[i]);
                ipad[i] = (byte)(ipad[i] ^ key[i]);
            }
            Core.Encryption.SHA1.SHA1 hasher = Core.Encryption.SHA1.SHA1.Create();

            ipad.AddRange(data);
         
            data = new List<byte>(hasher.ComputeHash(ipad.ToArray()));
            opad.AddRange(data);
            data = new List<byte>(hasher.ComputeHash(opad.ToArray()));
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

        public static async Task<string> GenerateTokenS40Online(string phone)
        {
            string response = await Utils.Common.HttpRequest.Get(Constants.Information.WhatsVersionScratchHost);
            string buildToken = "";
            if (response.GetJsonValue("g").Length > 0)
            {
                buildToken = response.GetJsonValue("g");
            }
            var plainToken = buildToken + response.GetJsonValue("c").ToString() + phone;
            return MD5.Encrypt(plainToken);
               
        }

        public static string GenerateTokenS40Offline(string phone)
        {
            return MD5.Encrypt("PdA2DJyKoUrwLw1Bg6EIhzh502dF9noR9uFCllGk1419900749520" + phone);
        }
    }
}
