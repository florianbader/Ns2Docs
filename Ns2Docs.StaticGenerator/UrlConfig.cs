using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Generator.Static
{
    public class UrlConfig
    {
        public static string ResolveUrl(string name)
        {
            return ResolveUrl(name, null, null);
        }

        public static string ResolveUrl(string name, IDictionary<string, object> args)
        {
            return ResolveUrl(name, null, args);
        }

        public static string ResolveUrl(string name, string from, IDictionary<string, object> args)
        {
            IDictionary<string, string> urls = new Dictionary<string, string>();

            urls.Add("main", "/");
            urls.Add("table-list", "/tables/index.html");
            urls.Add("table-list-alphabetical", "/tables/alphabetical.html");
            urls.Add("table-detail", "/tables/(table)/index.html");
            urls.Add("mixin-list", "/mixins/index.html");
            urls.Add("sourcecode-list", "/src/index.html");
            urls.Add("sourcecode-list-dir", "/src/(fileName)/index.html");
            urls.Add("sourcecode-detail", "/src/(fileName)/index.html");
            urls.Add("globals-list", "/globals/index.html");
            urls.Add("globals-detail", "/globals/(fileName)/index.html");
            urls.Add("tasks-list", "/tasks/");
            urls.Add("css", "/css/(fileName)");
            urls.Add("js", "/js/(fileName)");

            string url = String.Empty;
            if (urls.ContainsKey(name))
            {
                url = urls[name];
                if (args != null)
                {
                    foreach (var pair in args)
                    {
                        if (pair.Value != null)
                        {
                            url = url.Replace("(" + pair.Key + ")", pair.Value.ToString());
                        }
                    }
                }

                if (from != null)
                {
                    Uri a = new Uri("http://example.com" + url);
                    Uri b = new Uri("http://example.com" + from);

                    Uri r = b.MakeRelativeUri(a);
                    url = Uri.UnescapeDataString(r.ToString());
                }
                
            }



            return url.Replace(" ", "+");
        }
    }
}
