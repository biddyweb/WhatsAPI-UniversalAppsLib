using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Core.Exceptions
{
    class ConnectionException : Exception
    {
        public ConnectionException()
            : base()
        {

        }
        public ConnectionException(string message)
            : base(message)
        {

        }
        public ConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
