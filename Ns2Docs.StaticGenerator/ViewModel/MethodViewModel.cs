using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;
using System.Net;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public interface IMethodViewModel
    {
        IMethod Method { get; }
        string Url { get; }
        bool IsOriginatingTable { get; }
        IEnumerable<string> ContainerCssClasses { get; }
    }

    public class MethodViewModel : MethodDrop
    {
        public string Template { get { return "method"; } }
        public IMethod Method { get; private set; }
        private ITable Table { get; set; }



        public string HrefAnchor
        {
            get
            {
                return String.Format("method-{0}-{1}", Method.Name, Method.Library);
            }
        }


        protected override string BuildUrl()
        {
            var args = new Dictionary<string, object>();
            args["table"] = Table.Name;
            return UrlConfig.ResolveUrl("table-detail", args);
        }

        public MethodViewModel Overriding
        {
            get
            {
                if (IsOverriding)
                {
                    return new MethodViewModel(Method.Overrides);
                }
                return null;
            }
        }

        public MethodViewModel(ITable table, IMethod method)
            : base(table, method)
        {
            Method = method;
            Table = table;
        }

        public MethodViewModel(IMethod method)
            : this(method.Table, method)
        {
        }
    }
}
