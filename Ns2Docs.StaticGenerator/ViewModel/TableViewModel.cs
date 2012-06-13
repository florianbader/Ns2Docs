using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;
using System.Net;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public interface ITableViewModel
    {
        ITable Table { get; set; }
    }

    public class TableViewModel : SparkObjectDrop
    {
        public ITable Table { get; set; }

        public IEnumerable<object> AllMembersByVisibilityThenByName
        {
            get
            {
                IEnumerable<ITableMember> members = Table.AllMembers().OrderBy(x => x.IsPrivate).ThenBy(x => x.Name);
                IList<object> viewModels = new List<object>();
                foreach (ITableMember member in members)
                {
                    if (member is IStaticFunction)
                    {
                        viewModels.Add(new StaticFunctionViewModel(Table, (IStaticFunction)member));
                    }
                    else if (member is IMethod)
                    {
                        viewModels.Add(new MethodViewModel(Table, (IMethod)member));
                    }
                    else if (member is IField)
                    {
                        viewModels.Add(new FieldViewModel(Table, (IField)member));
                    }
                    else if (member is IStaticField)
                    {
                        viewModels.Add(new StaticFieldViewModel(Table, (IStaticField)member));
                    }
                }
                return viewModels;
            }
        }
        
        public IEnumerable<TableViewModel> InheritanceChain
        {
            get
            {
                return ToViewModels(Table.InheritanceChain());
            }
        }

        public IEnumerable<TableViewModel> Children
        {
            get
            {
                return ToViewModels(Table.Children);
            }
        }

        public IEnumerable<TableViewModel> ToViewModels(IEnumerable<ITable> tables)
        {
            List<TableViewModel> viewModels = new List<TableViewModel>();
            foreach (ITable table in tables)
            {
                viewModels.Add(new TableViewModel(table));
            }
            return viewModels;
        }

        
        public TableViewModel(ITable table)
            : base(table)
        {
            Table = table;
        }

        public IEnumerable<TableViewModel> AllMixins()
        {
            return ToViewModels(Table.AllMixins().OrderBy(mixin => mixin.Name));
        }

        protected override string BuildUrl()
        {
            IDictionary<string, object> args = new Dictionary<string, object>();
            args["table"] = Table.Name;
            return UrlConfig.ResolveUrl("table-detail", args);
        }
    }
}
