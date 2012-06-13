using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IVariableReference
    {
        string Assignment { get; }
        ISourceCode DeclaredIn { get; set; }
        int Line { get; set; }
        int Column { get; set; }
    }

    public class VariableReference : IVariableReference
    {
        public ISourceCode DeclaredIn { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public string Assignment { get; private set; }

        public VariableReference(string assignment)
        {
            Assignment = assignment;
        }
    }
}
