using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using System.Text.RegularExpressions;
using System.Web;

namespace Ns2Docs.Generator.Static.Tags
{
    public class UrlTag : Tag
    {
        private string name;
        private IDictionary<string, string> args;
        private string from;
        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            int nameEnd = markup.IndexOf(' ');
            if (nameEnd == -1)
            {
                nameEnd = markup.Length;
            }

            name = markup.Substring(0, nameEnd);
            MatchCollection matches = Regex.Matches(markup.Substring(nameEnd), "(?<argName>\\w+)\\s*=\\s*(?<value>[^\\s]+)");
            args = new Dictionary<string, string>();
            foreach (Match match in matches)
            {
                string argName = match.Groups["argName"].Value;
                string value = match.Groups["value"].Value;

                if (argName != "from")
                {
                    args[argName] = value;
                }
                else
                {
                    from = value;
                }
            }
        }

        public override void Render(Context context, System.IO.TextWriter result)
        {
            try
            {
                IDictionary<string, object> resolvedArgs = new Dictionary<string, object>();
                foreach (string key in args.Keys)
                {
                    string value = context[args[key]].ToString();
                    resolvedArgs[key] = value;
                }

                string resolvedFrom = null;
                if (from != null)
                {
                    var f = context[from];
                    if (f == null)
                    {

                    }
                    var vm = context["viewModel"];
                    var t = DateTime.Now;
                    if (f == null)
                    {

                    }
                    resolvedFrom = f.ToString();
                }

                string str = UrlConfig.ResolveUrl(name, resolvedFrom, resolvedArgs);
                str = str.Replace(' ', '+');
                result.Write(str);
            }
            catch (Exception e)
            {
                result.Write(name);
                result.Write(" ");
                foreach (var pair in args)
                {
                    result.Write(String.Format("{0}:{1} ", pair.Key, pair.Value));
                }
            }
        }
    }
}
