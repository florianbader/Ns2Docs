using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark.Parsing
{
    public delegate void CreatingObjectDelegate(string type);

    public interface IParser
    {
        void ParseSourceCode(IGame game, ISourceCode sourceCode);
        IGame ParseSourceCode(ISourceCode sourceCode);
        event CreatingObjectDelegate CreatingObject;
    }
}
