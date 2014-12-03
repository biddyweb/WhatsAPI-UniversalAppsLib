using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Utils.Common;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using System.IO;

namespace WhatsAPI.UniversalApps.Libs.Utils.Encryptions
{
    class SHA1
    {
        private static CryptographicKey macKey = null;
        public static byte[] Encrypt(string s)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var data = Encoding.GetEncoding("iso-8859-1").GetBytes(s);
            var buff = CryptographicBuffer.CreateFromByteArray(data);
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

        public static byte[] EncryptBase64(string s)
        {
            var vector = CryptographicBuffer.ConvertStringToBinary(s, BinaryStringEncoding.Utf8);
            IBuffer digest = HashAlgorithmProvider.OpenAlgorithm("SHA1").HashData(vector);
            return digest.ToArray();
        }

        public static byte[] EncryptBase64(byte[] s)
        {
            var inputStream = new MemoryStream(s);
            CryptographicHash hash = HashAlgorithmProvider.OpenAlgorithm("SHA1").CreateHash();

            byte[] buffer = new byte[4096];

            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                hash.Append(buffer.AsBuffer(0, bytesRead));
            }

            var hashed = hash.GetValueAndReset();

            return hashed.ToArray();
        }

        public SHA1()
        {

        }

        public SHA1(string hmacKey)
        {
            MacAlgorithmProvider hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var inputBuffer = Encoding.GetEncoding("iso-8859-1").GetBytes(hmacKey);
            var materialKey = CryptographicBuffer.CreateFromByteArray(inputBuffer);
            macKey = hmacSha1Provider.CreateKey(materialKey);
        }

        public SHA1(byte[] hmacKey)
        {
            MacAlgorithmProvider hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var materialKey = CryptographicBuffer.CreateFromByteArray(hmacKey);
            macKey = hmacSha1Provider.CreateKey(materialKey);
        }

        public static byte[] EncryptHmacSha1(string s)
        {
            IBuffer dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(s, BinaryStringEncoding.Utf8);
            IBuffer signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            String signature = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
            return Encoding.GetEncoding("iso-8859-1").GetBytes(signature);
        }


    }
}
