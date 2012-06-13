using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public class Declaration
    {
        public SourceCode SourceCode { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Declaration(SourceCode source, int line, int coloumn)
        {
            SourceCode = source;
            Line = line;
            Column = coloumn;
        } 
    }
}
