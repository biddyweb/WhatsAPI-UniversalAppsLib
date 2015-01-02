using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Core.Encryption
{
#if !NET_2_1
    [ComVisible(true)]
#endif
    public abstract class RandomNumberGenerator
#if NET_4_0
	: IDisposable
#endif
    {

        protected RandomNumberGenerator()
        {
        }

        public static RandomNumberGenerator Create()
        {
#if FULL_AOT_RUNTIME
			return new System.Security.Cryptography.RNGCryptoServiceProvider ();
#else
            // create the default random number generator
            return Create("WhatsAPI.UniversalApps.Libs.Core.Encryption.RandomNumberGenerator");
#endif
        }

        public static RandomNumberGenerator Create(string rngName)
        {
            return (RandomNumberGenerator)(Activator.CreateInstance(Type.GetType(rngName)));
        }

        public abstract void GetBytes(byte[] data);

#if NET_4_5
		public virtual void GetNonZeroBytes (byte[] data)
		{
			throw new NotImplementedException ();
		}
#else
        public abstract void GetNonZeroBytes(byte[] data);
#endif


#if NET_4_0
		public void Dispose ()
		{
			Dispose (true);
		}

		protected virtual void Dispose (bool disposing)
		{}
#endif
    }
}
