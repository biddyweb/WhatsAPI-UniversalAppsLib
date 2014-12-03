using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Constants
{
    public static class Enums
    {
        public enum CONNECTION_STATUS
        {
            UNAUTHORIZED,
            DISCONNECTED,
            CONNECTED,
            LOGGEDIN
        }
        public enum ImageType
        {
            JPEG,
            GIF,
            PNG
        }
        public enum VideoType
        {
            MOV,
            AVI,
            MP4
        }
        public enum AudioType
        {
            WAV,
            OGG,
            MP3
        }
        public enum VisibilityCategory
        {
            ProfilePhoto,
            Status,
            LastSeenTime
        }
        public enum VisibilitySetting
        {
            None,
            Contacts,
            Everyone
        }
        public enum SyncMode
        {
            Full,
            Delta,
            Query,
            Chunked
        }
        public enum SyncContext
        {
            Interactive,
            Background,
            Registration
        }
    }
}
