using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using Ns2Docs.Spark;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class TableListAlphabeticalViewModel : Drop
    {
        public string Url
        {
            get;
            protected set;
        }

        public string Name
        {
            get;
            protected set;
        }

        protected IEnumerable<ITable> Tables
        {
            get;
            private set;
        }

        public TableListAlphabeticalViewModel(IEnumerable<ITable> tables)
        {
            Url = UrlConfig.ResolveUrl("table-list-alphabetical").Replace(" ", "+");
            Name = "Tables";
            Tables = tables;
        }

        public class TableInfo : Drop
        {
            private ITable table;
            public string Name { get { return table.Name; } }
            public string Summary { get { return table.Summary; } }

            public TableInfo(ITable table)
            {
                this.table = table;
            }
        }

        public IEnumerable<TableInfo> TablesByName
        {
            get
            {
                var tablesByName = new List<TableInfo>();
                foreach (ITable tbl in Tables.OrderBy(tbl => tbl.Name))
                {
                    tablesByName.Add(new TableInfo(tbl));
                }
                return tablesByName;
            }
        }
    }
}
