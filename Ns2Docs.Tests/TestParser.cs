using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LuaInterface;

namespace Ns2Docs.Tests
{
    [TestFixture()]
    public class TestParsingInvalidCode
    {
        string luaCode;
        Parser parser;

        [SetUp]
        public void Arrange()
        {
            luaCode = "local someTable = { \n" +
                      "    keyA = 'valueA', \n" +
                      "    keyB = 'valueB' \n" + 
                      "    keyC = 'valueC' \n" +
                      "}";
            parser = new Parser();
        }

        [Test]
        public void MissingCommaInTableDeclaration()
        {
            TestDelegate code = delegate
            {
                parser.ParseString(luaCode);
            };

            Assert.Throws<LuaScriptException>(code);
        }
    }
}
