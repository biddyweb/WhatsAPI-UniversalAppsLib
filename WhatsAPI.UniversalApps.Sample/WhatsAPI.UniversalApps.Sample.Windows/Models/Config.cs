using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Sample.Models
{
    [SQLite.Net.Attributes.Table("config")]
    class Config
    {
        [PrimaryKey,AutoIncrement]
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
}
