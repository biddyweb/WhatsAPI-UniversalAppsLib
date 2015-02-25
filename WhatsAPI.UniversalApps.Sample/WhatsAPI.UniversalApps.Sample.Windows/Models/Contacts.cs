using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Sample.Models
{
    [SQLite.Net.Attributes.Table("contacts")]
    public class Contacts
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string jid { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }

    }
}
