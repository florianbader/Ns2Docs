using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark;

namespace Ns2Docs.Model.Test.Spark.SourceCodeTest
{
    [TestFixture]
    public class SourceCodeTests
    {
        #region Fields

        private SourceCode sourceCode;
        private string filename;
        private string baseDirectory;
        private string contents;

        #endregion

        #region Arrange

        [SetUp]
        public void SetUp()
        {
            baseDirectory = "C:/lua/Weapons/";
            filename = "C:/lua/Weapons/Shotgun.lua";

            string[] contentsArray = new string[] {
                                         "class 'Shotgun' (Weapon)",
                                         ""                        ,
                                         "function Shotgun:Fire()" ,
                                         "    // Fire weapon"      ,
                                         "end"
                                     };

            contents = String.Join("\n", contentsArray);

            
            sourceCode = new SourceCode(filename, baseDirectory, contents);
        }

        #endregion

        #region Tests

        #region Tesing FileSize

        [TestCase]
        public void FileSize()
        {
            Assert.AreEqual(72, sourceCode.FileSize,
                "Contents of the file is 72 characters long.");
        }

        #endregion

        #region Proper Usages of GetFilePosition

        [TestCase]
        public void GetFilePosition_OffsetOf10()
        {
            // offset == "class 'Sho"
            FilePosition position = sourceCode.GetFilePosition(10);

            Assert.AreEqual(1, position.Line, 
                "The first line.");

            Assert.AreEqual(10, position.Column, 
                "Column is ten characters in.");
        }

        [TestCase]
        public void GetFilePosition_OffsetOfZero()
        {
            // offset == "", start of file
            FilePosition position = sourceCode.GetFilePosition(0);

            Assert.AreEqual(1, position.Line, 
                "The first line");

            Assert.AreEqual(0, position.Column, 
                "Coloumn is before the first character.");
        }

        [TestCase]
        public void GetFilePosition_OffsetOf34()
        {
            // offset == "function"
            FilePosition position = sourceCode.GetFilePosition(34);

            Assert.AreEqual(3, position.Line,
                "Third line in the file.");

            Assert.AreEqual(8, position.Column,
                "Column is eight characters in.");
        }

        [TestCase]
        public void GetFilePosition_OffsetOf72()
        {
            // offset == "end"
            FilePosition position = sourceCode.GetFilePosition(72);

            Assert.AreEqual(5, position.Line,
                "Last line of the file.");

            Assert.AreEqual(3, position.Column,
                "Column is four characters in.");

        }

        #endregion

        #region Improper Usages of GetFilePosition

        [TestCase]
        public void GetFilePositon_Offset_of_negative_one_throws_an_exception()
        {
            TestDelegate testDelegate = delegate
            {
                sourceCode.GetFilePosition(-1);
            };

            Assert.Throws<ArgumentOutOfRangeException>(testDelegate,
                "Throw exception for an offset of less than zero.");
        }

        #endregion

        #endregion
    }
}
