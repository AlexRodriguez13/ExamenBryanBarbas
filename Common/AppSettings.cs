using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public class AppSettings
    {
        public static string ApiUrl { get => ConfigurationManager.AppSettings.Get("ApiUrl"); }
        public static string Token { get => ConfigurationManager.AppSettings.Get("Token"); }
    }
}
