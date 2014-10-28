using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace WhatsAPI.UniversalApps.Libs.Utils.Encryptions
{
    class SHA1
    {
        public static byte[] Encrypt(string s)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var inputBuffer = Encoding.GetEncoding("iso-8859-1").GetBytes(s);
            var buff = CryptographicBuffer.CreateFromByteArray(inputBuffer);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return Encoding.GetEncoding("iso-8859-1").GetBytes(res);
        }

        public static byte[] Encrypt(byte[] s)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var buff = CryptographicBuffer.CreateFromByteArray(s);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return Encoding.GetEncoding("iso-8859-1").GetBytes(res);
        }
    }
}
