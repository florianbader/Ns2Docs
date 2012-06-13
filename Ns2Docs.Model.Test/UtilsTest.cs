using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ns2Docs.Model.Test
{
    [TestFixture]
    public class UtilsTest
    {
        [TestCase]
        public void FindToken()
        {
            string str = "hello <world>!!!";
            Token token = Utils.FindToken(str, '<', '>');

            Assert.AreEqual("world", str.Substring(token.ContentStart, token.ContentLength));
        }

        [TestCase]
        public void FindToken_EmptyContents()
        {
            string str = "hello <>!!!";
            Token token = Utils.FindToken(str, '<', '>');

            Assert.AreEqual("", str.Substring(token.ContentStart, token.ContentLength));
        }

        [TestCase]
        public void FindToken_Nested()
        {
            string str = "hello <w<orl>d>!!!";
            Token token = Utils.FindToken(str, '<', '>');

            Assert.AreEqual("w<orl>d", str.Substring(token.ContentStart, token.ContentLength));
        }

        [TestCase]
        public void FindToken_NotTerminated()
        {
            string str = "hello <world!!!";

            TestDelegate notTerminated = delegate()
            {
                Utils.FindToken(str, '<', '>');
            };

            Assert.Throws<Exception>(notTerminated);
        }

        [TestCase]
        public void FindToken_BegingingOfString()
        {
            string str = "<hello> world!!!";
            Token token = Utils.FindToken(str, '<', '>');

            Assert.AreEqual("hello", str.Substring(token.ContentStart, token.ContentLength));
        }

        [TestCase]
        public void FindToken_EndOfString()
        {
            string str = "hello <world!!!>";
            Token token = Utils.FindToken(str, '<', '>');

            Assert.AreEqual("world!!!", str.Substring(token.ContentStart, token.ContentLength));
        }

        [TestCase]
        public void FindToken_Multiple()
        {
            string str = "<hello> <world>!!!";
            Token token = Utils.FindToken(str, '<', '>');

            Assert.AreEqual("hello", str.Substring(token.ContentStart, token.ContentLength));
        }
    }
}
