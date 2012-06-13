using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace Ns2Docs
{
    public class ParseError
    {
        public string File { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        
        public int ParserLine { get; private set; }
        public string ParserFile { get; private set; }
        public string Name { get; private set; }
        public string Message { get; private set; }
        
        public ParseError(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
