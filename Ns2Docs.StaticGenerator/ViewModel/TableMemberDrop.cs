using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using Ns2Docs.Spark;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class TableMemberDrop : Drop
    {
        private ITable table;
        private ITableMember member;

        public void BuildContainerCssClasses(ICollection<string> classes)
        {
            if (IsOriginatingTable)
            {
                classes.Add("originating-member");
            }
        }


        public TableMemberDrop(ITable table, ITableMember member)
        {
            this.table = table;
            this.member = member;
        }

        public bool IsOriginatingTable
        {
            get { return member.Table == table; }
        }

        public void BuildEmblems(ICollection<Emblem> emblems)
        {
        }
    }
}
