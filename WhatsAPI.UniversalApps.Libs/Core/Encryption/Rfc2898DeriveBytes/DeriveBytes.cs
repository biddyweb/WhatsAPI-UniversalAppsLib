using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Core.Encryption.Rfc2898DeriveBytes
{
    [ComVisible(true)]
#if NET_4_0
	public abstract class DeriveBytes : IDisposable {
#else
    public abstract class DeriveBytes
    {
#endif
        protected DeriveBytes()
        {
        }

        public abstract byte[] GetBytes(int cb);

        public abstract void Reset();

#if NET_4_0
		private bool m_disposed;

		public void Dispose ()
		{
			Dispose(true);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!m_disposed) {
				if (disposing) {
					// dispose managed objects
				}
				m_disposed = true;
			}
		}
#endif
    }
}
