using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Models
{
    public class GroupInfo
    {
         public readonly string id;
        public readonly string owner;
        public readonly long creation;
        public readonly string subject;
        public readonly long subjectChangedTime;
        public readonly string subjectChangedBy;

        internal GroupInfo(string id)
        {
            this.id = id;
        }

        internal GroupInfo(string id, string owner, string creation, string subject, string subjectChanged, string subjectChangedBy)
        {
            this.id = id;
            this.owner = owner;
            long.TryParse(creation, out this.creation);
            this.subject = subject;
            long.TryParse(subjectChanged, out this.subjectChangedTime);
            this.subjectChangedBy = subjectChangedBy;
        }
    }
}
