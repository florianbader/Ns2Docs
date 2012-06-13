using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ns2Docs.Tests
{
    [TestFixture()]
    public class TestExpressionParsing
    { 
        string luaStr;
        Parser parser;
        ParseResult result;

        [SetUp]
        public void SetUp()
        {
            parser = new Parser();
        }

        [Test]
        public void ParsingAFunctionCall()
        {
            luaStr = "DoSomething(1, 2, 3)";
            result = parser.ParseString(luaStr);
        }
    }
}
