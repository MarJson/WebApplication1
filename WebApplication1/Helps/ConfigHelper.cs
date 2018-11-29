using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public class ConfigHelper
    {
        public static int WebPageSize
        {
            get { return int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebPageSize"]); }
        }

    }
}