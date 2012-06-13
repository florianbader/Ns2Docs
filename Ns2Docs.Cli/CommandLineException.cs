using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ns2Docs.Cli
{
    public class CommandLineException : Exception
    {
        public CommandLineException()
            : base()
        { }

        public CommandLineException(string message)
            : base(message)
        { }

        protected CommandLineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public CommandLineException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
