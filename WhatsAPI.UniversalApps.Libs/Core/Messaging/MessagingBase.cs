using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Base;
using WhatsAPI.UniversalApps.Libs.Core.Connection;
using WhatsAPI.UniversalApps.Libs.Core.Exceptions;
using WhatsAPI.UniversalApps.Libs.Models;
using WhatsAPI.UniversalApps.Libs.Utils.Common;

namespace WhatsAPI.UniversalApps.Libs.Core.Messaging
{
    public class MessagingBase : Events.EventBase
    {
        protected ProtocolTreeNode uploadResponse;

        protected AccountInfo accountinfo;

        public static bool DEBUG;

        protected string password;

        protected bool hidden;

        protected WhatsAPI.UniversalApps.Libs.Constants.Enums.CONNECTION_STATUS loginStatus;

        public WhatsAPI.UniversalApps.Libs.Constants.Enums.CONNECTION_STATUS ConnectionStatus
        {
            get
            {
                return this.loginStatus;
            }
        }

        protected KeyStream outputKey;

        protected object messageLock = new object();

        protected List<ProtocolTreeNode> messageQueue;

        protected string name;

        protected string phoneNumber;

        protected BinTreeNodeReader reader;

        protected int timeout = 300000;

        protected Sockets whatsNetwork;

        public static readonly Encoding SYSEncoding = Encoding.UTF8;

        protected byte[] _challengeBytes;

        protected BinTreeNodeWriter BinWriter;

        protected void _constructBase(string phoneNum, string imei, string nick, bool debug, bool hidden)
        {
            this.messageQueue = new List<ProtocolTreeNode>();
            this.phoneNumber = phoneNum;
            this.password = imei;
            this.name = nick;
            this.hidden = hidden;
            //WhatsApp.DEBUG = debug;
            this.reader = new BinTreeNodeReader();
            this.loginStatus = WhatsAPI.UniversalApps.Libs.Constants.Enums.CONNECTION_STATUS.DISCONNECTED;
            this.BinWriter = new BinTreeNodeWriter();
            this.whatsNetwork = new Sockets(Constants.Information.WhatsAppHost, Constants.Information.WhatsPort, this.timeout);
        }

        public async Task Connect()
        {
            try
            {
                await this.whatsNetwork.Connect();
                this.loginStatus = WhatsAPI.UniversalApps.Libs.Constants.Enums.CONNECTION_STATUS.CONNECTED;
                //success
                this.fireOnConnectSuccess();
            }
            catch (Exception e)
            {
                this.fireOnConnectFailed(e);
            }
        }

        public void Disconnect(Exception ex = null)
        {
            this.whatsNetwork.Disconnect();
            this.loginStatus = WhatsAPI.UniversalApps.Libs.Constants.Enums.CONNECTION_STATUS.DISCONNECTED;
            this.fireOnDisconnect(ex);
        }

        protected byte[] encryptPassword()
        {
            return Convert.FromBase64String(this.password);
        }

        public AccountInfo GetAccountInfo()
        {
            return this.accountinfo;
        }

        public ProtocolTreeNode[] GetAllMessages()
        {
            ProtocolTreeNode[] tmpReturn = null;
            lock (messageLock)
            {
                tmpReturn = this.messageQueue.ToArray();
                this.messageQueue.Clear();
            }
            return tmpReturn;
        }

        protected void AddMessage(ProtocolTreeNode node)
        {
            lock (messageLock)
            {
                this.messageQueue.Add(node);
            }
        }

        public bool HasMessages()
        {
            if (this.messageQueue == null)
                return false;
            return this.messageQueue.Count > 0;
        }

        protected async Task SendData(byte[] data)
        {
            try
            {
                if (this.loginStatus != Constants.Enums.CONNECTION_STATUS.CONNECTED && this.loginStatus != Constants.Enums.CONNECTION_STATUS.LOGGEDIN)
                {
                    await this.Connect();
                }
                await this.whatsNetwork.SendData(data);
            }
            catch (ConnectionException)
            {
                this.Disconnect();
            }
        }

        protected async void SendNode(ProtocolTreeNode node)
        {
            await this.SendData(this.BinWriter.Write(node));
        }

        protected string privacySettingToString(WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting s)
        {
            switch (s)
            {
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting.None:
                    return "none";
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting.Contacts:
                    return "contacts";
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting.Everyone:
                    return "all";
                default:
                    throw new Exception("Invalid visibility setting");
            }
        }

        protected string privacyCategoryToString(WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory c)
        {
            switch (c)
            {
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory.LastSeenTime:
                    return "last";
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory.Status:
                    return "status";
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory.ProfilePhoto:
                    return "profile";
                default:
                    throw new Exception("Invalid privacy category");
            }
        }

        protected WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory parsePrivacyCategory(string data)
        {
            switch (data)
            {
                case "last":
                    return WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory.LastSeenTime;
                case "status":
                    return WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory.Status;
                case "profile":
                    return WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory.ProfilePhoto;
                default:
                    throw new Exception(String.Format("Could not parse {0} as privacy category", data));
            }
        }

        protected WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting parsePrivacySetting(string data)
        {
            switch (data)
            {
                case "none":
                    return WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting.None;
                case "contacts":
                    return WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting.Contacts;
                case "all":
                    return WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting.Everyone;
                default:
                    throw new Exception(string.Format("Cound not parse {0} as privacy setting", data));
            }
        }
    }
}
