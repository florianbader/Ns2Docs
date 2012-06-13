using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IStaticField : IVariable, ITableMember
    {
        
    }

    public class StaticField : Variable, IStaticField
    {
        public StaticField(ITable table, string name)
            : base(name)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            Table = table;
        }


        public bool IsPrivate { get { return false; } set { } }
        public bool IsPublic { get { return false; } set { } }
        public ITable Table { get; set; }
    }
}
