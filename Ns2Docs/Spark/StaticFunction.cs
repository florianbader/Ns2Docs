using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IStaticFunction : IFunction, ITableMember
    {
        IStaticFunction Overrides { get; }
    }

    public class StaticFunction : MemberFunction, IStaticFunction
    {
        public IStaticFunction Overrides 
        {
            get
            {
                return null;
            }
        }

        public StaticFunction(ITable table, string name)
            : base(name)
        {
            Table = table;
        }
    }
}
