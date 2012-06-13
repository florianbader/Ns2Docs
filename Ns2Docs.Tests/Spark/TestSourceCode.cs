using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark;

namespace Ns2Docs.Tests.Spark
{
    [TestFixture()]
    public class TestSourceCode
    {
        [Test]
        public void RelativeName()
        {
            string filename = "C:/game/source/file.lua";
            string baseDirectory = "C:/game/";

            SourceCode source = new SourceCode(filename, baseDirectory, "");
            Assert.AreEqual("source/file.lua", source.RelativeName);
        }

        [Test]
        public void RelativeNameWithBackwardsSlashes()
        {
            string filename = @"C:\game\source\file.lua";
            string baseDirectory = @"C:\game\";

            SourceCode source = new SourceCode(filename, baseDirectory, "");
            Assert.AreEqual("source/file.lua", source.RelativeName);
        }
    }

    [TestFixture()]
    public class TestSourceCodeFilePosition
    {
        SourceCode source;
        FilePosition position;

        [SetUp]
        public void SetUp()
        {
            string contents =
                   "function table.copy(t)\n" +              
                   "    local copy = {}\n" +                 
                   "    for key, value in pairs(t) do\n" +   
                   "        copy[key] = value\n" +           
                   "    end\n" +                             
                   "    return copy\n" +                     
                   "end";                                    

            
            source = new SourceCode(contents);
        }

        [Test]
        public void GettingFilePositionWithAnOffsetOfZero()
        {
            position = source.GetFilePosition(0);

            Assert.AreEqual(1, position.Line);
            Assert.AreEqual(0, position.Column);
        }

        [Test]
        public void GettingFilePositionWithAnOffsetOf10()
        {
            position = source.GetFilePosition(10);

            Assert.AreEqual(1, position.Line, "An offset of 10 is on the first line ending at 'function t'");
            Assert.AreEqual(10, position.Column, "The column ends at 'function t'");
        }

        [Test]
        public void GettingFilePositionWithAnOffsetOf22()
        {
            position = source.GetFilePosition(22);

            Assert.AreEqual(1, position.Line, "An offset of 22 is on the first line ending at 'function table.copy(t)\n'");
            Assert.AreEqual(22, position.Column, "The column ends at 'function table.copy(t)\n'");
        }

        [Test]
        public void GettingFilePositionWithAnOffsetOf23()
        {
            position = source.GetFilePosition(23);

            Assert.AreEqual(2, position.Line, "An offset of 23 is on the second line ending");
            Assert.AreEqual(0, position.Column, "The column is the first character in the line");
        }

        [Test]
        public void GettingFilePositionWithAnOffsetOf32()
        {
            position = source.GetFilePosition(32);

            Assert.AreEqual(2, position.Line);
            Assert.AreEqual(9, position.Column);
        }

        [Test]
        public void TheLastOffsetofTheContents()
        {
            position = source.GetFilePosition(129);
            
            
            Assert.AreEqual(7, position.Line);
            Assert.AreEqual(2, position.Column);
        }
    }
}
