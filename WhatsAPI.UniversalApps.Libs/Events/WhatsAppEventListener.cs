using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Events
{
    interface WhatsAppEventListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="error"></param>
        void onClose(string phone, string error);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="login"></param>
        /// <param name="pw"></param>
        /// <param name="type"></param>
        /// <param name="expiration">in UNIX Timestamp Format</param>
        /// <param name="kind"></param>
        /// <param name="price"></param>
        /// <param name="cost"></param>
        /// <param name="currency"></param>
        /// <param name="price_expiration">in UNIX Timestamp Format</param>
        void onCodeRegister(string phone, string login,
        string pw,
        string type,
        long expiration,
        string kind,
        double price,
        double cost,
        string currency,
        long price_expiration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <param name="retry_after"></param>
        void onCodeRegisterFailed(
        string phone, // The user phone number including the country code.
        string status, // The server status number
        string reason, // Reason of the status (e.g. too_recent/missing_param/bad_param).
        long retry_after);// Waiting time before requesting a new code in seconds.
        void onCodeRequest(
        string phone, // The user phone number including the country code.
        string method, // Used method (SMS/voice).
        string length// Registration code length.
    );

        void onCodeRequestFailed(
            string phone, // The user phone number including the country code.
            string method, // Used method (SMS/voice).
            string reason, // Reason of the status (e.g. too_recent/missing_param/bad_param).
            string value// The missing_param/bad_param or waiting time before requesting a new code.
        );

        void onCodeRequestFailedTooRecent(
             string phone, // The user phone number including the country code.
             string method, // Used method (SMS/voice).
             string reason, // Reason of the status (too_recent).
             string retry_after // Waiting time before requesting a new code in seconds.
         );

        void onConnect(
             string phone, // The user phone number including the country code.
             string socket // The resource socket id.
         );

        void onConnectError(
             string phone, // The user phone number including the country code.
             string socket // The resource socket id.
         );

        void onCredentialsBad(
             string phone, // The user phone number including the country code.
             string status, // Account status.
             string reason // The reason.
         );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="login"></param>
        /// <param name="pw"></param>
        /// <param name="type"></param>
        /// <param name="expiration">in Unix Timestamp</param>
        /// <param name="kind"></param>
        /// <param name="price"></param>
        /// <param name="cost"></param>
        /// <param name="currency"></param>
        /// <param name="price_expiration">in Unix Timestamp</param>
        void onCredentialsGood(
            string phone, // The user phone number including the country code.
            string login, // Phone number with country code.
            string pw, // Account password.
            string type, // Type of account.
            long expiration, // Expiration date in UNIX TimeStamp.
            string kind, // Kind of account.
            string price, // Formated price of account.
            string cost, // Decimal amount of account.
            string currency, // Currency price of account.
            long price_expiration // Price expiration in UNIX TimeStamp.
        );

        void onDisconnect(
            string phone, // The user phone number including the country code.
            int socket // The resource socket id.
        );

        void onDissectPhone(
            string phone, // The user phone number including the country code.
            string country, // The detected country name.
            string cc, // The number's country code.
            string mcc, // International cell network code for the detected country.
            string lc, // Location code for the detected country
            string lg // Language code for the detected country
        );

        void onDissectPhoneFailed(
            string phone // The user phone number including the country code.
        );

        void onGetAudio(
            string phone, // The user phone number including the country code.
            string from, // The sender phone number.
            long msgid, // The message id.
            string type, // The message type.
            long time, // The unix time when send message notification.
            string name, // The sender name.
            long size, // The image size.
            string url, // The url to bigger audio version.
            string file, // The audio name.
            string mimetype, // The audio mime type.
            string filehash, // The audio file hash.
            long duration, // The audio duration.
            string acodec // The audio codec.
        );

        void onGetError(
            string phone, // The user phone number including the country code.
            long id, // The id of the request that caused the error
            string error // Array with error data for why request failed.
        );

        void onGetGroups(
            string phone, // The user phone number including the country code.
            string groupList // Array with all the groups and groupsinfo.
        );

        void onGetGroupsInfo(
            string phone, // The user phone number including the country code.
            string groupList // Array with the groupinfo.
        );

        void onGetGroupsSubject(
            string phone, // The user phone number including the country code.
            string gId, // The group JID.
            long time, // The unix time when send subject.
            string author, // The author phone number including the country code.
            string participant, // The participant phone number including the country code.
            string name, // The sender name.
            string subject // The subject (e.g. group name).
        );

        void onGetImage(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time, // The unix time when send message notification.
            string name, // The sender name.
            string size, // The image size.
            string url, // The url to bigger image version.
            string file, // The image name.
            string mimetype, // The image mime type.
            string filehash, // The image file hash.
            string width, // The image width.
            string height, // The image height.
            string thumbnail, // The base64_encode image thumbnail.
            string caption // The image caption.
        );

        void onGetLocation(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time, // The unix time when send message notification.
            string name, // The sender name.
            string place_name, // The place name.
            double longitude, // The location longitude.
            double latitude, // The location latitude.
            string url, // The place url.
            string thumbnail // The base64_encode location image thumbnail.
        );

        void onGetMessage(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time, // The unix time when send message notification.
            string name, // The sender name.
            string message // The message.
        );

        void onGetGroupMessage(
            string phone, // The user phone number including the country code.
            string from, // The group JID.
            string author, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time, // The unix time when send message notification.
            string name, // The sender name.
            string message // The message.
        );

        void onGetGroupParticipants(
            string phone,
            string groupId,
            string groupList
        );

        void onGetPrivacyBlockedList(
            string phone, // The user phone number including the country code.
            string children
            /*
            string data, // Array of data nodes containing numbers you have blocked.
            string onGetProfilePicture, //
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            string type, // The type of picture (image/preview).
            string thumbnail // The base64_encoded image.
            */
        );

        void onGetProfilePicture(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            string type, // The type of picture (image/preview).
            string thumbnail// The base64_encoded image.
        );

        void onGetRequestLastSeen(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string sec // The number of seconds since the user went offline.
        );

        void onGetServerProperties(
            string phone, // The user phone number including the country code.
            string version, // The version number on the server.
            string properties // Array of server properties.
        );

        void onGetStatus(
            string phone,
            string from,
            string type,
            string id,
            string t,
            string status
        );

        void onGetvCard(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time, // The unix time when send message notification.
            string name, // The sender name.
            string contact, // The vCard contact name.
            string vcard // The vCard.
        );

        void onGetVideo(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time, // The unix time when send message notification.
            string name, // The sender name.
            string url, // The url to bigger video version.
            string file, // The video name.
            string size, // The video size.
            string mimetype, // The video mime type.
            string filehash, // The video file hash.
            string duration, // The video duration.
            string vcodec, // The video codec.
            string acodec, // The audio codec.
            string thumbnail, // The base64_encode video thumbnail.
            string caption // The video caption.
        );

        void onGroupsChatCreate(
            string phone, // The user phone number including the country code.
            string gId // The group JID.
        );

        void onGroupsChatisCreated(
            string phone, // The user phone number including the country code.
            string creator, // The group chat creator.
            string gId, // The group JID.
            string subject // The group subject.
        );

        void onGroupsChatEnd(
            string phone, // The user phone number including the country code.
            string gId // The group JID.
        );

        void onGroupsParticipantsAdd(
            string phone, // The user phone number including the country code.
            string groupId, // The group JID.
            string participant // The participant JID.
        );

        void onGroupsParticipantsRemove(
            string phone, // The user phone number including the country code.
            string groupId, // The group JID.
            string participant // The participant JID.
        );

        void onLogin(
            string phone // The user phone number including the country code.
        );

        void onLoginFailed(
            string phone, // The user phone number including the country code.
            string tag
        );

        void onMediaMessageSent(
            string phone, // The user phone number including the country code.
            string to,
            string id,
            string filetype,
            string url,
            string filename,
            string filesize,
            string filehash,
        string caption,
            string icon
        );

        void onMediaUploadFailed(
            string phone, // The user phone number including the country code.
            string id,
            string node,
            string messageNode,
            string reason
        );

        void onMessageComposing(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time // The unix time when send message notification.
        );

        void onMessagePaused(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time // The unix time when send message notification.
        );

        void onMessageReceivedClient(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time // The unix time when send message notification.
        );

        void onMessageReceivedServer(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            long msgid, // The message id.
            string type, // The message type.
            long time // The unix time when send message notification.
        );

        void onPaidAccount(
            string phone,
            string author,
            string kind,
            string status,
            string creation,
            string expiration
        );

        void onPing(
            string phone, // The user phone number including the country code.
            long msgid // The message id.
        );

        void onGetServicePricing(
            string phone, //the user phone number including country code.
            string price, //price string.
            string cost, //cost int.
            string currency, //currency-code.
            string expiration //the unix time when account expires.
        );

        void onGetExtendAccount(
            string phone, //the user phone number including country code.
            string kind, //kind-string, e.g "free"
            string status, //status string.
            string creation, //unix timestamp of account creation.
            string expiration //the unix time when account expires.
        );

        void onGetNormalizedJid(
            string phone, //the user phone number including country code.
            string result //the normalized jid 
        );

        void onGetBroadcastLists(
            string phone, //the user phone number including country code.
            string broadcastLists //a multidimensional array with the lists 
        );

        void onPresence(
            string phone, // The user phone number including the country code.
            string from, // The sender JID.
            string type // The presence type.
        );

        void onProfilePictureChanged(
            string phone,
            string from,
            string id,
            string t
        );

        void onProfilePictureDeleted(
            string phone,
            string from,
            string id,
            string t
        );

        void onSendMessage(
            string phone, // The user phone number including the country code.
            string targets,
            string id,
            string node
        );

        void onSendMessageReceived(
            string phone, // The user phone number including the country code.
            string id, // Message ID
            string from, // The sender JID.
            string type // Message type
        );

        void onSendPong(
            string phone, // The user phone number including the country code.
            long msgid // The message id.
        );

        void onSendPresence(
            string phone, // The user phone number including the country code.
            string type, // Presence type.
            string name  // User nickname.
        );

        void onSendStatusUpdate(
            string phone, // The user phone number including the country code.
            string msg  // The status message.
        );

        void onUploadFile(
            string phone, // The user phone number including the country code.
            string name, // The filename.
            string url  // The remote url on WhatsApp servers (note, // this is NOT the URL to download the file, only used for sending message).
        );

        void onUploadFileFailed(
            string phone, // The user phone number including the country code.
            string name // The filename.
        );

        /**
         * @param string result SyncResult
         * @return mixed
         */
        void onGetSyncResult(
            string result
        );

        void onGetReceipt(
            string from,
            string id,
            string offline,
            string retry
        );


    }
}
