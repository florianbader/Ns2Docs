using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public interface IStaticFunctionViewModel
    {
        IStaticFunction StaticFunction { get; }
        string Url { get; }
        bool IsOriginatingTable { get; }
        IEnumerable<string> ContainerCssClasses { get; }
    }

    public class StaticFunctionViewModel : FunctionDrop, IStaticFunctionViewModel
    {
        public string Template { get { return "staticfunction"; } }
        public IStaticFunction StaticFunction { get; private set; }
        public ITable Table { get; set; }
        private TableMemberDrop tableMemberDrop;
        public bool IsOriginatingTable
        {
            get
            {
                return Table == StaticFunction.Table;
            }
        }

        public string HrefAnchor
        {
            get
            {
                return String.Format("static-function-{0}-{1}", StaticFunction.Name, StaticFunction.Library);
            }
        }

        protected override void BuildContainerCssClasses(ICollection<string> classes)
        {
            base.BuildContainerCssClasses(classes);
            tableMemberDrop.BuildContainerCssClasses(classes);
            classes.Add("static-function");
        }


        protected override string BuildUrl()
        {
            var args = new Dictionary<string, object>();
            args["table"] = Table.Name;
            return UrlConfig.ResolveUrl("table-detail", args);
        }

        public string OriginalStaticFunctionUrl
        {
            get
            {
                string url = null;
                if (IsOverriding)
                {
                    url = new StaticFunctionViewModel(StaticFunction.Overrides).Url;
                }
                return url;
            }
        }

        public bool IsOverriding
        {
            get
            {
                return StaticFunction.Overrides != null;
            }
        }

        public StaticFunctionViewModel(ITable table, IStaticFunction staticFunction)
            : base(staticFunction)
        {
            StaticFunction = staticFunction;
            Table = table;
            tableMemberDrop = new TableMemberDrop(table, staticFunction);
        }

        public StaticFunctionViewModel Overriding
        {
            get
            {
                if (IsOverriding)
                {
                    return new StaticFunctionViewModel(StaticFunction.Overrides);
                }
                return null;
            }
        }

        public string TableName { get { return Table.Name; } }

        public StaticFunctionViewModel(IStaticFunction staticFunction)
            : this(null, staticFunction)
        {
            
        }
    }
}
