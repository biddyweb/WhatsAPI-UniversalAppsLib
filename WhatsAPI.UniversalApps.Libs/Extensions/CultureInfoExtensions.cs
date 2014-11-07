using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Extensions
{
    public static class CultureInfoExtensions
    {
        public static void GetLanguageAndLocale(this CultureInfo self, out string language, out string locale)
        {
            var name = self.Name;
            var n1 = name.IndexOf('-');
            if (n1 > 0)
            {
                var n2 = name.LastIndexOf('-');
                language = name.Substring(0, n1);
                locale = name.Substring(n2 + 1);
            }
            else
            {
                language = name;
                switch (language)
                {
                    case "cs":
                        locale = "CZ";
                        return;

                    case "da":
                        locale = "DK";
                        return;

                    case "el":
                        locale = "GR";
                        return;

                    case "ja":
                        locale = "JP";
                        return;

                    case "ko":
                        locale = "KR";
                        return;

                    case "sv":
                        locale = "SE";
                        return;

                    case "sr":
                        locale = "RS";
                        return;
                }
                locale = language.ToUpper();
            }
        }
    }
}
