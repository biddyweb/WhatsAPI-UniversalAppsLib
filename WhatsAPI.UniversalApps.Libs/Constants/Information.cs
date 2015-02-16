using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Constants
{
    /// <summary>
    /// This code is from shirioko code : https://github.com/shirioko/WhatsAPINet/blob/master/WhatsAppApi/Settings/WhatsConstants.cs
    /// </summary>
    public class Information
    {
        #region ServerConstants

        /// <summary>
        /// The whatsapp digest
        /// </summary>
        public const string WhatsAppDigest = "xmpp/s.whatsapp.net";

        /// <summary>
        /// The whatsapp host
        /// </summary>
        public const string WhatsAppHost = "c3.whatsapp.net";

        /// <summary>
        /// The whatsapp XMPP realm
        /// </summary>
        public const string WhatsAppRealm = "s.whatsapp.net";

        /// <summary>
        /// The whatsapp server
        /// </summary>
        public const string WhatsAppServer = "s.whatsapp.net";

        /// <summary>
        /// The whatsapp group chat server
        /// </summary>
        public const string WhatsGroupChat = "g.us";

        /// <summary>
        /// Check Credential Server
        /// </summary>
        public const string WhatsCheckHost = "v.whatsapp.net/v2/exist";

        /// <summary>
        /// Registration Host
        /// </summary>
        public const string WhatsRegisterHost = "v.whatsapp.net/v2/register";

        /// <summary>
        /// Whatsapp Request Code Host
        /// </summary>
        public const string WhatsRequestCodeHost = "v.whatsapp.net/v2/code";

        /// <summary>
        /// Upload Media Host
        /// </summary>
        public const string WhatsUploadHost = "https://mms.whatsapp.net/client/iphone/upload.php";

        /// <summary>
        /// The whatsapp version the client complies to
        /// </summary>
        public const string WhatsAppVer = "2.11.14";

        /// <summary>
        /// The port that needs to be connected to
        /// </summary>
        public const int WhatsPort = 443;

        /// <summary>
        /// iPhone device
        /// </summary>
        public const string Device = "iPhone";

        /// <summary>
        /// The useragent used for http requests
        /// </summary>
        public const string UserAgent = "WhatsApp/2.12.61 S40Version/14.26 Device/Nokia302";

        /// <summary>
        /// Check Current Whatsapp Version
        /// </summary>
        public const string WhatsVersionCheckerHost = "https://coderus.openrepos.net/whitesoft/whatsapp_version";
        #endregion

        #region ParserConstants
        /// <summary>
        /// The number style used
        /// </summary>
        public static NumberStyles WhatsAppNumberStyle = (NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);

        /// <summary>
        /// Unix epoch DateTime 
        /// </summary>
        public static DateTime UnixEpoch = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region TextEncoding
        public static string ASCIIEncoding = "US-ASCII";
        #endregion
    }
}
