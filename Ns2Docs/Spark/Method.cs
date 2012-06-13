using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ns2Docs.Spark
{
    public interface IMethod : IFunction, ITableMember
    {
        IMethod Overrides { get; }
    }

    public class Method : MemberFunction, IMethod
    {
        public new string QualifiedName
        {
            get
            {
                return string.Format("{0}:{1}", Table.Name, Signature);
            }
        }

        public IMethod Overrides
        {
            get
            {
                ITable table = Table.BaseTable;
                while (table != null)
                {
                    if (table != Table)
                    {
                        foreach (IMethod method in table.Methods)
                        {
                            if (method.Name == Name)
                            {
                                return method;
                            }
                        }
                    }
                    foreach (ITable mixin in table.Mixins)
                    {
                        foreach (IMethod method in table.Methods)
                        {
                            if (method.Name == Name)
                            {
                                return method;
                            }
                        }
                    }
                    table = table.BaseTable;
                }
                return null;
            }
        }

        public Method(ITable table, string name)
            : base(name)
        {
            Table = table;
            if (name.StartsWith("_"))
            {
                IsPrivate = true;
                
            }
        }
    }
}
