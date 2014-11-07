using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Models
{
    public class RegisterResponse
    {
        public string Response { get; set; }
        public string Password { get; set; }
        public bool IsSuccess { get; set; }
    }
}
