using Microsoft.Practices.Prism.Mvvm;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Sample.Models
{
    [SQLite.Net.Attributes.Table("contacts")]
    public class Contacts : BindableBase
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string jid { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }

        private string _profileImage = String.Empty;
        public string profileImage
        {
            get
            {
                return _profileImage;
            }
            set
            {
                _profileImage = value;
                OnPropertyChanged(() => profileImage);
            }
        }

    }
}
