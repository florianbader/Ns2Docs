using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark.Parsing
{
    public class LuaErrorMessage
    {
        public string Description { get; private set; }

        public LuaErrorMessage(string description)
        {
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
