using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Core.Encryption.HMACSHA1
{
    [ComVisible(true)]
    public class HMACSHA1 : HMAC
    {

        public HMACSHA1()
            : this(WhatsAPI.UniversalApps.Libs.Core.Encryption.CryptoTools.KeyBuilder.Key(8))
        {
        }

        public HMACSHA1(byte[] key)
        {
#if FULL_AOT_RUNTIME
			SetHash ("SHA1", new SHA1Managed ());
#else
            HashName = "SHA1";
#endif
            HashSizeValue = 160;
            Key = key;
        }

        public HMACSHA1(byte[] key, bool useManagedSha1)
        {
            HashName = "System.Security.Cryptography.SHA1" + (useManagedSha1 ? "Managed" : "CryptoServiceProvider");
            HashSizeValue = 160;
            Key = key;
        }
    }
}
