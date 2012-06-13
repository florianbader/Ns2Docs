using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs
{
    public interface ISanitizedPath
    {
        string UncleanPath { get; set;  }
        string Path { get; }

        string GetRelativeName(ISanitizedPath baseDirectory);
    }
}
