using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace Ns2Docs.Generator.Static
{
    public static class Filters
    {
        public static string UrlEscape(string input)
        {
            return Uri.EscapeUriString(input);
        }
    }
}
