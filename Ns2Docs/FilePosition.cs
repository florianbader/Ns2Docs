using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs
{
    public class FilePosition
    {
        public int Line { get; private set; }
        public int Column { get; private set; }

        public FilePosition(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return String.Format("[Line: {0}, Col: {1}]", Line, Column);
        }
    }
}
