using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface ITableMember : ISparkObject
    {
        ITable Table { get; }
        bool IsPrivate { get; set; }
        bool IsPublic { get; set; }
    }
}
