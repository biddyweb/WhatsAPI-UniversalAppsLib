using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace WhatsAPI.UniversalApps.Libs.Utils.Encryptions
{
    static class MD5
    {
        public static string Encrypt(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

        public static string Encrypt(byte[] str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var buff = CryptographicBuffer.CreateFromByteArray(str);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
