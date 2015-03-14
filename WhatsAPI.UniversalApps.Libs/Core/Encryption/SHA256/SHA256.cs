using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Core.Encryption.SHA256
{
    [ComVisible(true)]
    public abstract class SHA256 : HashAlgorithm
    {

        protected SHA256()
        {
            HashSizeValue = 256;
        }

        public static new SHA256 Create()
        {
#if FULL_AOT_RUNTIME
			return new System.Security.Cryptography.SHA256Managed ();
#else
            return new SHA256Managed();
#endif
        }

        public static new SHA256 Create(string hashName)
        {
            return (SHA256)Activator.CreateInstance(Type.GetType(hashName));
        }
    }
}
