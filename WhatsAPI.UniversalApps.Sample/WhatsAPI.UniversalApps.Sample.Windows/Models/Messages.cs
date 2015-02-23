using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Sample.Models
{
    [SQLite.Net.Attributes.Table("messages")]
    public class Messages
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string jid { get; set; }
        public string messages { get; set; }
    }
}
