using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WhatsAPI.UniversalApps.Libs.Base;
using WhatsAPI.UniversalApps.Libs.Core.Messaging;
using WhatsAPI.UniversalApps.Libs.Utils.Common;
using WhatsAPI.UniversalApps.Libs.Utils.Messaging;
using WhatsAPI.UniversalApps.Libs.Constants;
using WhatsAPI.UniversalApps.Libs.Models;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Utils.Encryptions;
using Newtonsoft.Json;
using WhatsAPI.UniversalApps.Libs.Core.Encryption;
using WhatsAPI.UniversalApps.Libs.Core.Encryption.SHA256;
using Windows.Foundation;

namespace WhatsAPI.UniversalApps.Libs
{
    public class WhatsApp : SendMessageBase
    {
        public WhatsApp(string phoneNum, string imei, string nick, bool debug = false, bool hidden = false)
        {
            this._constructBase(phoneNum, imei, nick, debug, hidden);
        }

        public string SendMessage(string to, string txt)
        {
            var tmpMessage = new FMessage(Helpers.GetJID(to), true) { data = txt };
            this.SendMessage(tmpMessage, this.hidden);
            return tmpMessage.identifier_key.ToString();
        }

        public void SendMessageVcard(string to, string name, string vcard_data)
        {
            var tmpMessage = new FMessage(Helpers.GetJID(to), true) { data = vcard_data, media_wa_type = FMessage.Type.Contact, media_name = name };
            this.SendMessage(tmpMessage, this.hidden);
        }

        public void SendSync(string[] numbers, WhatsAPI.UniversalApps.Libs.Constants.Enums.SyncMode mode = WhatsAPI.UniversalApps.Libs.Constants.Enums.SyncMode.Delta, WhatsAPI.UniversalApps.Libs.Constants.Enums.SyncContext context = WhatsAPI.UniversalApps.Libs.Constants.Enums.SyncContext.Background, int index = 0, bool last = true)
        {
            List<ProtocolTreeNode> users = new List<ProtocolTreeNode>();
            foreach (string number in numbers)
            {
                string _number = number;
                if (!_number.StartsWith("+", StringComparison.CurrentCultureIgnoreCase))
                    _number = string.Format("+{0}", number);
                users.Add(new ProtocolTreeNode("user", null, System.Text.Encoding.UTF8.GetBytes(_number)));
            }
            ProtocolTreeNode node = new ProtocolTreeNode("iq", new KeyValue[]
            {
                new KeyValue("to", Helpers.GetJID(this.phoneNumber)),
                new KeyValue("type", "get"),
                new KeyValue("id", TicketCounter.MakeId("sendsync_")),
                new KeyValue("xmlns", "urn:xmpp:whatsapp:sync")
            }, new ProtocolTreeNode("sync", new KeyValue[]
                {
                    new KeyValue("mode", mode.ToString().ToLowerInvariant()),
                    new KeyValue("context", context.ToString().ToLowerInvariant()),
                    new KeyValue("sid", DateTime.Now.ToFileTimeUtc().ToString()),
                    new KeyValue("index", index.ToString()),
                    new KeyValue("last", last.ToString())
                },
                users.ToArray()
                )
            );
            this.SendNode(node);
        }



        protected void SendQrSync(byte[] qrkey, byte[] token = null)
        {
            string id = TicketCounter.MakeId("qrsync_");
            List<ProtocolTreeNode> children = new List<ProtocolTreeNode>();
            children.Add(new ProtocolTreeNode("sync", null, qrkey));
            if (token != null)
            {
                children.Add(new ProtocolTreeNode("code", null, token));
            }
            ProtocolTreeNode node = new ProtocolTreeNode("iq", new[] {
                new KeyValue("type", "set"),
                new KeyValue("id", id),
                new KeyValue("xmlns", "w:web")
            }, children.ToArray());
            this.SendNode(node);
        }

        public void SendActive()
        {
            var node = new ProtocolTreeNode("presence", new[] { new KeyValue("type", "active") });
            this.SendNode(node);
        }

        public void SendAddParticipants(string gjid, IEnumerable<string> participants)
        {
            string id = TicketCounter.MakeId("add_group_participants_");
            this.SendVerbParticipants(gjid, participants, id, "add");
        }

        public void SendUnavailable()
        {
            var node = new ProtocolTreeNode("presence", new[] { new KeyValue("type", "unavailable") });
            this.SendNode(node);
        }

        public void SendClientConfig(string platform, string lg, string lc)
        {
            string v = TicketCounter.MakeId("config_");
            var child = new ProtocolTreeNode("config", new[] { new KeyValue("xmlns", "urn:xmpp:whatsapp:push"), new KeyValue("platform", platform), new KeyValue("lg", lg), new KeyValue("lc", lc) });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", v), new KeyValue("type", "set"), new KeyValue("to", Constants.Information.WhatsAppRealm) }, new ProtocolTreeNode[] { child });
            this.SendNode(node);
        }

        public void SendClientConfig(string platform, string lg, string lc, Uri pushUri, bool preview, bool defaultSetting, bool groupsSetting, IEnumerable<GroupSetting> groups, Action onCompleted, Action<int> onError)
        {
            string id = TicketCounter.MakeId("config_");
            var node = new ProtocolTreeNode("iq",
                                        new[]
                                        {
                                            new KeyValue("id", id), new KeyValue("type", "set"),
                                            new KeyValue("to", "") //this.Login.Domain)
                                        },
                                        new ProtocolTreeNode[]
                                        {
                                            new ProtocolTreeNode("config",
                                            new[]
                                                {
                                                    new KeyValue("xmlns","urn:xmpp:whatsapp:push"),
                                                    new KeyValue("platform", platform),
                                                    new KeyValue("lg", lg),
                                                    new KeyValue("lc", lc),
                                                    new KeyValue("clear", "0"),
                                                    new KeyValue("id", pushUri.ToString()),
                                                    new KeyValue("preview",preview ? "1" : "0"),
                                                    new KeyValue("default",defaultSetting ? "1" : "0"),
                                                    new KeyValue("groups",groupsSetting ? "1" : "0")
                                                },
                                            this.ProcessGroupSettings(groups))
                                        });
            this.SendNode(node);
        }

        public void SendClose()
        {
            var node = new ProtocolTreeNode("presence", new[] { new KeyValue("type", "unavailable") });
            this.SendNode(node);
        }

        public void SendComposing(string to)
        {
            this.SendChatState(to, "composing");
        }

        protected void SendChatState(string to, string type)
        {
            var node = new ProtocolTreeNode("chatstate", new[] { new KeyValue("to", Helpers.GetJID(to)) }, new[] { 
                new ProtocolTreeNode(type, null)
            });
            this.SendNode(node);
        }

        public void SendCreateGroupChat(string subject)
        {
            string id = TicketCounter.MakeId("create_group_");
            var child = new ProtocolTreeNode("group", new[] { new KeyValue("action", "create"), new KeyValue("subject", subject) });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "set"), new KeyValue("xmlns", "w:g"), new KeyValue("to", "g.us") }, new ProtocolTreeNode[] { child });
            this.SendNode(node);
        }

        public void SendDeleteAccount()
        {
            string id = TicketCounter.MakeId("del_acct_");
            var node = new ProtocolTreeNode("iq",
                                            new KeyValue[]
                                                {
                                                    new KeyValue("id", id), 
                                                    new KeyValue("type", "get"),
                                                    new KeyValue("to", "s.whatsapp.net"),
                                                    new KeyValue("xmlns", "urn:xmpp:whatsapp:account")
                                                },
                                            new ProtocolTreeNode[]
                                                {
                                                    new ProtocolTreeNode("remove",
                                                                         null
                                                                         )
                                                });
            this.SendNode(node);
        }

        public void SendDeleteFromRoster(string jid)
        {
            string v = TicketCounter.MakeId("roster_");
            var innerChild = new ProtocolTreeNode("item", new[] { new KeyValue("jid", jid), new KeyValue("subscription", "remove") });
            var child = new ProtocolTreeNode("query", new[] { new KeyValue("xmlns", "jabber:iq:roster") }, new ProtocolTreeNode[] { innerChild });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("type", "set"), new KeyValue("id", v) }, new ProtocolTreeNode[] { child });
            this.SendNode(node);
        }

        public void SendEndGroupChat(string gjid)
        {
            string id = TicketCounter.MakeId("remove_group_");
            var child = new ProtocolTreeNode("group", new[] { new KeyValue("action", "delete") });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "set"), new KeyValue("xmlns", "w:g"), new KeyValue("to", gjid) }, new ProtocolTreeNode[] { child });
            this.SendNode(node);
        }

        public void SendGetClientConfig()
        {
            string id = TicketCounter.MakeId("get_config_");
            var child = new ProtocolTreeNode("config", new[] { new KeyValue("xmlns", "urn:xmpp:whatsapp:push") });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("to", Constants.Information.WhatsAppRealm) }, new ProtocolTreeNode[] { child });
            this.SendNode(node);
        }

        public void SendGetDirty()
        {
            string id = TicketCounter.MakeId("get_dirty_");
            var child = new ProtocolTreeNode("status", new[] { new KeyValue("xmlns", "urn:xmpp:whatsapp:dirty") });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("to", "s.whatsapp.net") }, new ProtocolTreeNode[] { child });
            this.SendNode(node);
        }

        public void SendGetGroupInfo(string gjid)
        {
            string id = TicketCounter.MakeId("get_g_info_");
            var child = new ProtocolTreeNode("query", null);
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("xmlns", "w:g"), new KeyValue("to", Helpers.GetJID(gjid)) }, new ProtocolTreeNode[] { child });
            this.SendNode(node);
        }

        public void SendGetGroups()
        {
            string id = TicketCounter.MakeId("get_groups_");
            this.SendGetGroups(id, "participating");
        }

        public void SendGetOwningGroups()
        {
            string id = TicketCounter.MakeId("get_owning_groups_");
            this.SendGetGroups(id, "owning");
        }

        public void SendGetParticipants(string gjid)
        {
            string id = TicketCounter.MakeId("get_participants_");
            var child = new ProtocolTreeNode("list", null);
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("xmlns", "w:g"), new KeyValue("to", Helpers.GetJID(gjid)) }, child);
            this.SendNode(node);
        }

        public string SendGetPhoto(string jid, string expectedPhotoId, bool largeFormat)
        {
            string id = TicketCounter.MakeId("get_photo_");
            var attrList = new List<KeyValue>();
            if (!largeFormat)
            {
                attrList.Add(new KeyValue("type", "preview"));
            }
            if (expectedPhotoId != null)
            {
                attrList.Add(new KeyValue("id", expectedPhotoId));
            }
            var child = new ProtocolTreeNode("picture", attrList.ToArray());
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("xmlns", "w:profile:picture"), new KeyValue("to", Helpers.GetJID(jid)) }, child);
            this.SendNode(node);
            return id;
        }

        public void SendGetPhotoIds(IEnumerable<string> jids)
        {
            string id = TicketCounter.MakeId("get_photo_id_");
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("to", Helpers.GetJID(this.phoneNumber)) },
                new ProtocolTreeNode("list", new[] { new KeyValue("xmlns", "w:profile:picture") },
                    (from jid in jids select new ProtocolTreeNode("user", new[] { new KeyValue("jid", jid) })).ToArray<ProtocolTreeNode>()));
            this.SendNode(node);
        }

        public void SendGetPrivacyList()
        {
            string id = TicketCounter.MakeId("privacylist_");
            var innerChild = new ProtocolTreeNode("list", new[] { new KeyValue("name", "default") });
            var child = new ProtocolTreeNode("query", new KeyValue[] { new KeyValue("xmlns", "jabber:iq:privacy") }, innerChild);
            var node = new ProtocolTreeNode("iq", new KeyValue[] { new KeyValue("id", id), new KeyValue("type", "get") }, child);
            this.SendNode(node);
        }

        public void SendGetServerProperties()
        {
            string id = TicketCounter.MakeId("get_server_properties_");
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("xmlns", "w"), new KeyValue("to", "s.whatsapp.net") },
                new ProtocolTreeNode("props", null));
            this.SendNode(node);
        }

        public void SendGetStatuses(string[] jids)
        {
            List<ProtocolTreeNode> targets = new List<ProtocolTreeNode>();
            foreach (string jid in jids)
            {
                targets.Add(new ProtocolTreeNode("user", new[] { new KeyValue("jid", Helpers.GetJID(jid)) }, null, null));
            }

            ProtocolTreeNode node = new ProtocolTreeNode("iq", new[] {
                new KeyValue("to", "s.whatsapp.net"),
                new KeyValue("type", "get"),
                new KeyValue("xmlns", "status"),
                new KeyValue("id", TicketCounter.MakeId("getstatus"))
            }, new[] {
                new ProtocolTreeNode("status", null, targets.ToArray(), null)
            }, null);

            this.SendNode(node);
        }

        public void SendInactive()
        {
            var node = new ProtocolTreeNode("presence", new[] { new KeyValue("type", "inactive") });
            this.SendNode(node);
        }

        public void SendLeaveGroup(string gjid)
        {
            this.SendLeaveGroups(new string[] { gjid });
        }

        public void SendLeaveGroups(IEnumerable<string> gjids)
        {
            string id = TicketCounter.MakeId("leave_group_");
            IEnumerable<ProtocolTreeNode> innerChilds = from gjid in gjids select new ProtocolTreeNode("group", new[] { new KeyValue("id", gjid) });
            var child = new ProtocolTreeNode("leave", null, innerChilds);
            var node = new ProtocolTreeNode("iq", new KeyValue[] { new KeyValue("id", id), new KeyValue("type", "set"), new KeyValue("xmlns", "w:g"), new KeyValue("to", "g.us") }, child);
            this.SendNode(node);
        }

        public void SendMessage(FMessage message, bool hidden = false)
        {
            if (message.media_wa_type != FMessage.Type.Undefined)
            {
                this.SendMessageWithMedia(message);
            }
            else
            {
                this.SendMessageWithBody(message, hidden);
            }
        }

        public void SendMessageBroadcast(string[] to, string message)
        {
            this.SendMessageBroadcast(to, new FMessage(string.Empty, true) { data = message, media_wa_type = FMessage.Type.Undefined });
        }



        public void SendMessageBroadcast(string[] to, FMessage message)
        {
            if (to != null && to.Length > 0 && message != null && !string.IsNullOrEmpty(message.data))
            {
                ProtocolTreeNode child;
                if (message.media_wa_type == FMessage.Type.Undefined)
                {
                    //text broadcast
                    child = new ProtocolTreeNode("body", null, null, WhatsApp.SYSEncoding.GetBytes(message.data));
                }
                else
                {
                    throw new NotImplementedException();
                }

                //compose broadcast envelope
                ProtocolTreeNode xnode = new ProtocolTreeNode("x", new KeyValue[] {
                    new KeyValue("xmlns", "jabber:x:event")
                }, new ProtocolTreeNode("server", null));
                List<ProtocolTreeNode> toNodes = new List<ProtocolTreeNode>();
                foreach (string target in to)
                {
                    toNodes.Add(new ProtocolTreeNode("to", new KeyValue[] { new KeyValue("jid", Helpers.GetJID(target)) }));
                }

                ProtocolTreeNode broadcastNode = new ProtocolTreeNode("broadcast", null, toNodes);
                ProtocolTreeNode messageNode = new ProtocolTreeNode("message", new KeyValue[] {
                    new KeyValue("to", "broadcast"),
                    new KeyValue("type", message.media_wa_type == FMessage.Type.Undefined?"text":"media"),
                    new KeyValue("id", message.identifier_key.id)
                }, new ProtocolTreeNode[] {
                    broadcastNode,
                    xnode,
                    child
                });
                this.SendNode(messageNode);
            }
        }

        public void SendNop()
        {
            this.SendNode(null);
        }

        public void SendPaused(string to)
        {
            this.SendChatState(to, "paused");
        }

        public void SendPing()
        {
            string id = TicketCounter.MakeId("ping_");
            var child = new ProtocolTreeNode("ping", new[] { new KeyValue("xmlns", "w:p") });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get") }, child);
            this.SendNode(node);
        }

        public void SendPresenceSubscriptionRequest(string to)
        {
            var node = new ProtocolTreeNode("presence", new[] { new KeyValue("type", "subscribe"), new KeyValue("to", Helpers.GetJID(to)) });
            this.SendNode(node);
        }

        public void SendQueryLastOnline(string jid)
        {
            string id = TicketCounter.MakeId("last_");
            var child = new ProtocolTreeNode("query", null);
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("to", Helpers.GetJID(jid)), new KeyValue("xmlns", "jabber:iq:last") }, child);
            this.SendNode(node);
        }

        public void SendRemoveParticipants(string gjid, List<string> participants)
        {
            string id = TicketCounter.MakeId("remove_group_participants_");
            this.SendVerbParticipants(gjid, participants, id, "remove");
        }

        public void SendSetGroupSubject(string gjid, string subject)
        {
            string id = TicketCounter.MakeId("set_group_subject_");
            var child = new ProtocolTreeNode("subject", new[] { new KeyValue("value", subject) });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "set"), new KeyValue("xmlns", "w:g"), new KeyValue("to", gjid) }, child);
            this.SendNode(node);
        }



        public void SendSetPrivacyBlockedList(IEnumerable<string> jidSet)
        {
            string id = TicketCounter.MakeId("privacy_");
            ProtocolTreeNode[] nodeArray = Enumerable.Select<string, ProtocolTreeNode>(jidSet, (Func<string, int, ProtocolTreeNode>)((jid, index) => new ProtocolTreeNode("item", new KeyValue[] { new KeyValue("type", "jid"), new KeyValue("value", jid), new KeyValue("action", "deny"), new KeyValue("order", index.ToString(CultureInfo.InvariantCulture)) }))).ToArray<ProtocolTreeNode>();
            var child = new ProtocolTreeNode("list", new KeyValue[] { new KeyValue("name", "default") }, (nodeArray.Length == 0) ? null : nodeArray);
            var node2 = new ProtocolTreeNode("query", new KeyValue[] { new KeyValue("xmlns", "jabber:iq:privacy") }, child);
            var node3 = new ProtocolTreeNode("iq", new KeyValue[] { new KeyValue("id", id), new KeyValue("type", "set") }, node2);
            this.SendNode(node3);
        }

        public void SendStatusUpdate(string status)
        {
            string id = TicketCounter.MakeId("sendstatus_");

            ProtocolTreeNode node = new ProtocolTreeNode("iq", new KeyValue[] {
                new KeyValue("to", "s.whatsapp.net"),
                new KeyValue("type", "set"),
                new KeyValue("id", id),
                new KeyValue("xmlns", "status")
            },
            new[] {
                new ProtocolTreeNode("status", null, System.Text.Encoding.UTF8.GetBytes(status))
            });

            this.SendNode(node);
        }

        public void SendSubjectReceived(string to, string id)
        {
            var child = new ProtocolTreeNode("received", new[] { new KeyValue("xmlns", "urn:xmpp:receipts") });
            var node = GetSubjectMessage(to, id, child);
            this.SendNode(node);
        }

        public void SendUnsubscribeHim(string jid)
        {
            var node = new ProtocolTreeNode("presence", new[] { new KeyValue("type", "unsubscribed"), new KeyValue("to", jid) });
            this.SendNode(node);
        }

        public void SendUnsubscribeMe(string jid)
        {
            var node = new ProtocolTreeNode("presence", new[] { new KeyValue("type", "unsubscribe"), new KeyValue("to", jid) });
            this.SendNode(node);
        }

        public void SendGetGroups(string id, string type)
        {
            var child = new ProtocolTreeNode("list", new[] { new KeyValue("type", type) });
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "get"), new KeyValue("xmlns", "w:g"), new KeyValue("to", "g.us") }, child);
            this.SendNode(node);
        }

        protected void SendMessageWithBody(FMessage message, bool hidden = false)
        {
            var child = new ProtocolTreeNode("body", null, null, WhatsApp.SYSEncoding.GetBytes(message.data));
            this.SendNode(GetMessageNode(message, child, hidden));
        }

        protected void SendMessageWithMedia(FMessage message)
        {
            ProtocolTreeNode node;
            if (FMessage.Type.System == message.media_wa_type)
            {
                throw new Exception("Cannot send system message over the network");
            }

            List<KeyValue> list = new List<KeyValue>(new KeyValue[] { new KeyValue("xmlns", "urn:xmpp:whatsapp:mms"), new KeyValue("type", FMessage.GetMessage_WA_Type_StrValue(message.media_wa_type)) });
            if (FMessage.Type.Location == message.media_wa_type)
            {
                list.AddRange(new KeyValue[] { new KeyValue("latitude", message.latitude.ToString(CultureInfo.InvariantCulture)), new KeyValue("longitude", message.longitude.ToString(CultureInfo.InvariantCulture)) });
                if (message.location_details != null)
                {
                    list.Add(new KeyValue("name", message.location_details));
                }
                if (message.location_url != null)
                {
                    list.Add(new KeyValue("url", message.location_url));
                }
            }
            else if (((FMessage.Type.Contact != message.media_wa_type) && (message.media_name != null)) && ((message.media_url != null) && (message.media_size > 0L)))
            {
                list.AddRange(new KeyValue[] { new KeyValue("file", message.media_name), new KeyValue("size", message.media_size.ToString(CultureInfo.InvariantCulture)), new KeyValue("url", message.media_url) });
                if (message.media_duration_seconds > 0)
                {
                    list.Add(new KeyValue("seconds", message.media_duration_seconds.ToString(CultureInfo.InvariantCulture)));
                }
            }
            if ((FMessage.Type.Contact == message.media_wa_type) && (message.media_name != null))
            {
                node = new ProtocolTreeNode("media", list.ToArray(), new ProtocolTreeNode("vcard", new KeyValue[] { new KeyValue("name", message.media_name) }, WhatsApp.SYSEncoding.GetBytes(message.data)));
            }
            else
            {
                byte[] data = message.binary_data;
                if ((data == null) && !string.IsNullOrEmpty(message.data))
                {
                    try
                    {
                        data = Convert.FromBase64String(message.data);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (data != null)
                {
                    list.Add(new KeyValue("encoding", "raw"));
                }
                node = new ProtocolTreeNode("media", list.ToArray(), null, data);
            }
            this.SendNode(GetMessageNode(message, node));
        }

        protected void SendVerbParticipants(string gjid, IEnumerable<string> participants, string id, string inner_tag)
        {
            IEnumerable<ProtocolTreeNode> source = from jid in participants select new ProtocolTreeNode("participant", new[] { new KeyValue("jid", Helpers.GetJID(jid)) });
            var child = new ProtocolTreeNode(inner_tag, null, source);
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "set"), new KeyValue("xmlns", "w:g"), new KeyValue("to", Helpers.GetJID(gjid)) }, child);
            this.SendNode(node);
        }

        public void SendSetPrivacySetting(WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilityCategory category, WhatsAPI.UniversalApps.Libs.Constants.Enums.VisibilitySetting setting)
        {
            ProtocolTreeNode node = new ProtocolTreeNode("iq", new[] { 
                new KeyValue("to", "s.whatsapp.net"),
                new KeyValue("id", TicketCounter.MakeId("setprivacy_")),
                new KeyValue("type", "set"),
                new KeyValue("xmlns", "privacy")
            }, new ProtocolTreeNode[] {
                new ProtocolTreeNode("privacy", null, new ProtocolTreeNode[] {
                    new ProtocolTreeNode("category", new [] {
                    new KeyValue("name", this.privacyCategoryToString(category)),
                    new KeyValue("value", this.privacySettingToString(setting))
                    })
            })
            });

            this.SendNode(node);
        }

        public void SendGetPrivacySettings()
        {
            ProtocolTreeNode node = new ProtocolTreeNode("iq", new KeyValue[] {
                new KeyValue("to", "s.whatsapp.net"),
                new KeyValue("id", TicketCounter.MakeId("getprivacy_")),
                new KeyValue("type", "get"),
                new KeyValue("xmlns", "privacy")
            }, new ProtocolTreeNode[] {
                new ProtocolTreeNode("privacy", null)
            });
            this.SendNode(node);
        }

        protected IEnumerable<ProtocolTreeNode> ProcessGroupSettings(IEnumerable<GroupSetting> groups)
        {
            ProtocolTreeNode[] nodeArray = null;
            if ((groups != null) && groups.Any<GroupSetting>())
            {
                DateTime now = DateTime.Now;
                nodeArray = (from @group in groups
                             select new ProtocolTreeNode("item", new[] 
                { new KeyValue("jid", @group.Jid),
                    new KeyValue("notify", @group.Enabled ? "1" : "0"),
                    new KeyValue("mute", string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", new object[] { (!@group.MuteExpiry.HasValue || (@group.MuteExpiry.Value <= now)) ? 0 : ((int) (@group.MuteExpiry.Value - now).TotalSeconds) })) })).ToArray<ProtocolTreeNode>();
            }
            return nodeArray;
        }

        protected static ProtocolTreeNode GetMessageNode(FMessage message, ProtocolTreeNode pNode, bool hidden = false)
        {
            return new ProtocolTreeNode("message", new[] { 
                new KeyValue("to", message.identifier_key.remote_jid), 
                new KeyValue("type", message.media_wa_type == FMessage.Type.Undefined?"text":"media"), 
                new KeyValue("id", message.identifier_key.id) 
            },
            new ProtocolTreeNode[] {
                new ProtocolTreeNode("x", new KeyValue[] { new KeyValue("xmlns", "jabber:x:event") }, new ProtocolTreeNode("server", null)),
                pNode,
                new ProtocolTreeNode("offline", null)
            });
        }

        protected static ProtocolTreeNode GetSubjectMessage(string to, string id, ProtocolTreeNode child)
        {
            return new ProtocolTreeNode("message", new[] { new KeyValue("to", to), new KeyValue("type", "subject"), new KeyValue("id", id) }, child);
        }

        #region Message Multimedia
        public async void SendMessageBroadcastImage(string[] recipients, byte[] ImageData, WhatsAPI.UniversalApps.Libs.Constants.Enums.ImageType imgtype)
        {
            string to;
            List<string> foo = new List<string>();
            foreach (string s in recipients)
            {
                foo.Add(Helpers.GetJID(s));
            }
            to = string.Join(",", foo.ToArray());
            FMessage msg = await this.getFmessageImage(to, ImageData, imgtype);
            if (msg != null)
            {
                this.SendMessage(msg);
            }
        }

        public async void SendMessageBroadcastAudio(string[] recipients, byte[] AudioData, WhatsAPI.UniversalApps.Libs.Constants.Enums.AudioType audtype)
        {
            string to;
            List<string> foo = new List<string>();
            foreach (string s in recipients)
            {
                foo.Add(Helpers.GetJID(s));
            }
            to = string.Join(",", foo.ToArray());
            FMessage msg = await this.getFmessageAudio(to, AudioData, audtype);
            if (msg != null)
            {
                this.SendMessage(msg);
            }
        }

        public async void SendMessageBroadcastVideo(string[] recipients, byte[] VideoData, WhatsAPI.UniversalApps.Libs.Constants.Enums.VideoType vidtype)
        {
            string to;
            List<string> foo = new List<string>();
            foreach (string s in recipients)
            {
                foo.Add(Helpers.GetJID(s));
            }
            to = string.Join(",", foo.ToArray());
            FMessage msg = await this.getFmessageVideo(to, VideoData, vidtype);
            if (msg != null)
            {
                this.SendMessage(msg);
            }
        }
        public async void SendSetPhoto(byte[] bytes, byte[] thumbnailBytes = null)
        {
            string id = TicketCounter.MakeId("set_photo_");

            bytes = await this.ProcessProfilePicture(bytes);

            var list = new List<ProtocolTreeNode> { new ProtocolTreeNode("picture", null, null, bytes) };

            if (thumbnailBytes == null)
            {
                //auto generate
                thumbnailBytes = await this.CreateThumbnail(bytes);
            }

            //debug
            await FileHelper.SaveFileFromByteArray(bytes, "pic.jpg");
            await FileHelper.SaveFileFromByteArray(thumbnailBytes, "pic.jpg");

            if (thumbnailBytes != null)
            {
                list.Add(new ProtocolTreeNode("picture", new[] { new KeyValue("type", "preview") }, null, thumbnailBytes));
            }
            var node = new ProtocolTreeNode("iq", new[] { new KeyValue("id", id), new KeyValue("type", "set"), new KeyValue("xmlns", "w:profile:picture"), new KeyValue("to", Helpers.GetJID(this.phoneNumber)) }, list.ToArray());
            this.SendNode(node);
        }

        public async void SendMessageImage(string to, byte[] ImageData, WhatsAPI.UniversalApps.Libs.Constants.Enums.ImageType imgtype)
        {
            FMessage msg = await this.getFmessageImage(to, ImageData, imgtype);
            if (msg != null)
            {
                this.SendMessage(msg);
            }
        }

        protected async Task<FMessage> getFmessageImage(string to, byte[] ImageData, WhatsAPI.UniversalApps.Libs.Constants.Enums.ImageType imgtype)
        {
            string type = string.Empty;
            string extension = string.Empty;
            switch (imgtype)
            {
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.ImageType.PNG:
                    type = "image/png";
                    extension = "png";
                    break;
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.ImageType.GIF:
                    type = "image/gif";
                    extension = "gif";
                    break;
                default:
                    type = "image/jpeg";
                    extension = "jpg";
                    break;
            }

            //create hash
            string filehash = string.Empty;
            SHA256 sha = SHA256.Create();
            byte[] raw = sha.ComputeHash(ImageData);
            filehash = Convert.ToBase64String(raw);


            //request upload
            UploadResponse response = await this.UploadFile(filehash, "image", ImageData.Length, ImageData, to, type, extension);

            if (response != null && !String.IsNullOrEmpty(response.url))
            {
                //send message
                FMessage msg = new FMessage(to, true)
                {
                    media_wa_type = FMessage.Type.Image,
                    media_mime_type = response.mimetype,
                    media_name = response.url.Split('/').Last(),
                    media_size = response.size,
                    media_url = response.url,
                    binary_data = await this.CreateThumbnail(ImageData, "thumb-"+response.url.Split('/').Last())
                };
                return msg;
            }
            return null;
        }

        public async void SendMessageVideo(string to, byte[] videoData, WhatsAPI.UniversalApps.Libs.Constants.Enums.VideoType vidtype)
        {
            FMessage msg = await this.getFmessageVideo(to, videoData, vidtype);
            if (msg != null)
            {
                this.SendMessage(msg);
            }
        }

        protected async Task<FMessage> getFmessageVideo(string to, byte[] videoData, WhatsAPI.UniversalApps.Libs.Constants.Enums.VideoType vidtype)
        {
            to = Helpers.GetJID(to);
            string type = string.Empty;
            string extension = string.Empty;
            switch (vidtype)
            {
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VideoType.MOV:
                    type = "video/quicktime";
                    extension = "mov";
                    break;
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.VideoType.AVI:
                    type = "video/x-msvideo";
                    extension = "avi";
                    break;
                default:
                    type = "video/mp4";
                    extension = "mp4";
                    break;
            }

            //create hash
            string filehash = string.Empty;
            SHA256 sha = SHA256.Create();
            byte[] raw = sha.ComputeHash(videoData);
            filehash = Convert.ToBase64String(raw);


            //request upload
            UploadResponse response = await this.UploadFile(filehash, "video", videoData.Length, videoData, to, type, extension);

            if (response != null && !String.IsNullOrEmpty(response.url))
            {
                //send message
                FMessage msg = new FMessage(to, true)
                {
                    media_wa_type = FMessage.Type.Video,
                    media_mime_type = response.mimetype,
                    media_name = response.url.Split('/').Last(),
                    media_size = response.size,
                    media_url = response.url,
                    media_duration_seconds = response.duration,
                    binary_data = await this.CreateVideoThumbnail(videoData, "thumb-"+response.url.Split('/').Last().Replace(getExtension(response.url.Split('/').Last()),"jpg"))
                };
                return msg;
            }
            return null;
        }

        private string getExtension(string fileName)
        {
            var nameExt = fileName.Split('.');
            if (nameExt.Count() == 2)
            {
                return nameExt[1];
            }
            return nameExt[0];
        }

        public async Task SendMessageAudio(string to, byte[] audioData, WhatsAPI.UniversalApps.Libs.Constants.Enums.AudioType audtype)
        {
            FMessage msg = await this.getFmessageAudio(to, audioData, audtype);
            if (msg != null)
            {
                this.SendMessage(msg);
            }
        }

        protected async Task<FMessage> getFmessageAudio(string to, byte[] audioData, WhatsAPI.UniversalApps.Libs.Constants.Enums.AudioType audtype)
        {
            to = Helpers.GetJID(to);
            string type = string.Empty;
            string extension = string.Empty;
            switch (audtype)
            {
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.AudioType.WAV:
                    type = "audio/wav";
                    extension = "wav";
                    break;
                case WhatsAPI.UniversalApps.Libs.Constants.Enums.AudioType.OGG:
                    type = "audio/ogg";
                    extension = "ogg";
                    break;
                default:
                    type = "audio/mpeg";
                    extension = "mp3";
                    break;
            }

            //create hash
            string filehash = string.Empty;
            using (HashAlgorithm sha = HashAlgorithm.Create("sha256"))
            {
                byte[] raw = sha.ComputeHash(audioData);
                filehash = Convert.ToBase64String(raw);
            }

            //request upload
            UploadResponse response = await this.UploadFile(filehash, "audio", audioData.Length, audioData, to, type, extension);

            if (response != null && !String.IsNullOrEmpty(response.url))
            {
                //send message
                FMessage msg = new FMessage(to, true)
                {
                    media_wa_type = FMessage.Type.Audio,
                    media_mime_type = response.mimetype,
                    media_name = response.url.Split('/').Last(),
                    media_size = response.size,
                    media_url = response.url,
                    media_duration_seconds = response.duration
                };
                return msg;
            }
            return null;
        }

        protected async Task<UploadResponse> UploadFile(string b64hash, string type, long size, byte[] fileData, string to, string contenttype, string extension)
        {
            ProtocolTreeNode media = new ProtocolTreeNode("media", new KeyValue[] {
                new KeyValue("hash", b64hash),
                new KeyValue("type", type),
                new KeyValue("size", size.ToString())
            });
            string id = TicketManager.GenerateId();
            ProtocolTreeNode node = new ProtocolTreeNode("iq", new KeyValue[] {
                new KeyValue("id", id),
                new KeyValue("to", Constants.Information.WhatsAppServer),
                new KeyValue("type", "set"),
                new KeyValue("xmlns", "w:m")
            }, media);
            this.uploadResponse = null;
            this.SendNode(node);
            int i = 0;

            IAsyncOperation<bool> taskLoad = this.pollMessage().AsAsyncOperation<bool>();
            taskLoad.AsTask().Wait(5000);
            
            if (this.uploadResponse == null)
            {
                IAsyncOperation<bool> taskLoad2 = this.pollMessage().AsAsyncOperation<bool>();
                taskLoad.AsTask().Wait(5000);
              
            }

            if (this.uploadResponse != null && this.uploadResponse.GetChild("duplicate") != null)
            {
                UploadResponse res = new UploadResponse(this.uploadResponse);
                this.uploadResponse = null;
                return res;
            }
            else
            {
                try
                {
                    if (this.uploadResponse == null) return null;
                    string uploadUrl = this.uploadResponse.GetChild("media").GetAttribute("url");
                    this.uploadResponse = null;

                    Uri uri = new Uri(uploadUrl);

                    string hashname = string.Empty;
                    byte[] buff = System.Text.Encoding.GetEncoding(Constants.Information.ASCIIEncoding).GetBytes(MD5.Encrypt(System.Text.Encoding.GetEncoding(Constants.Information.ASCIIEncoding).GetBytes(b64hash)));
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in buff)
                    {
                        sb.Append(b.ToString("X2"));
                    }
                    hashname = String.Format("{0}.{1}", sb.ToString(), extension);

                    string boundary = "zzXXzzYYzzXXzzQQ";

                    sb = new StringBuilder();

                    sb.AppendFormat("--{0}\r\n", boundary);
                    sb.Append("Content-Disposition: form-data; name=\"to\"\r\n\r\n");
                    sb.AppendFormat("{0}\r\n", to);
                    sb.AppendFormat("--{0}\r\n", boundary);
                    sb.Append("Content-Disposition: form-data; name=\"from\"\r\n\r\n");
                    sb.AppendFormat("{0}\r\n", this.phoneNumber);
                    sb.AppendFormat("--{0}\r\n", boundary);
                    sb.AppendFormat("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\n", hashname);
                    sb.AppendFormat("Content-Type: {0}\r\n\r\n", contenttype);
                    string header = sb.ToString();

                    sb = new StringBuilder();
                    sb.AppendFormat("\r\n--{0}--\r\n", boundary);
                    string footer = sb.ToString();

                    long clength = size + header.Length + footer.Length;

                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("User-Agent", Constants.Information.UserAgent);
                    headers.Add("Content-Length", clength.ToString());

                    List<byte> buf = new List<byte>();
                    //buf.AddRange(Encoding.UTF8.GetBytes(post));
                    buf.AddRange(Encoding.UTF8.GetBytes(header));
                    buf.AddRange(fileData);
                    buf.AddRange(Encoding.UTF8.GetBytes(footer));

                    var response = await HttpRequest.UploadPhotoContinueAsync(buf.ToArray(), uploadUrl, boundary, headers);
                    buff = await response.Content.ReadAsByteArrayAsync();
                    string result = Encoding.UTF8.GetString(buff, 0, buff.Length);
                    foreach (string line in result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.StartsWith("{"))
                        {
                            string fooo = line.TrimEnd(new char[] { (char)0 });
                            UploadResponse resp = JsonConvert.DeserializeObject<UploadResponse>(fooo);
                            if (!String.IsNullOrEmpty(resp.url))
                            {
                                return resp;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WhatsAPI.UniversalApps.Libs.Utils.Logger.Log.Write("Error when upload media : " + ex.Message);
                }
            }
            return null;
        }
        #endregion
    }
}
