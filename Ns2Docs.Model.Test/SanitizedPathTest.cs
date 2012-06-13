using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ns2Docs.Model.Test
{
    [TestFixture]
    public class SanitizedPathTest
    {
        #region Fields

        private SanitizedPath sanitized;

        #endregion

        [TestCase]
        public void SetUncleanPath()
        {
            sanitized = new SanitizedPath();
            sanitized.UncleanPath = "C:/lua/";

            Assert.AreEqual("C:/lua/", sanitized.UncleanPath);
            Assert.AreEqual(@"C:\lua", sanitized.Path);
        }

        #region Proper Usages for Setting Path

        [TestCase]
        public void SetPath()
        {
            sanitized = new SanitizedPath(@"C:\lua");

            Assert.AreEqual(@"C:\lua", sanitized.Path);
        }

        [Test]
        public void SetPath_Null()
        {
            sanitized = new SanitizedPath();

            Assert.IsNull(sanitized.Path);
        }

        [Test]
        public void SetPath_FileName()
        {
            sanitized = new SanitizedPath(@"C:\lua\Weapons\Shotgun.lua");

            Assert.AreEqual(@"C:\lua\Weapons\Shotgun.lua", sanitized.Path);
        }

        [TestCase]
        public void SetPath_UnixPathSeparators()
        {
            sanitized = new SanitizedPath("C:/lua");

            Assert.AreEqual(@"C:\lua", sanitized.Path,
                @"Convert '/' separators to '\'.");
        }

        [TestCase]
        public void SetPath_TrailingSeparator()
        {
            sanitized = new SanitizedPath(@"C:\lua\");

            Assert.AreEqual(@"C:\lua", sanitized.Path,
                "Remove trailing separators.");
        }

        #endregion

        #region Improper Usages for Setting Path

        [TestCase]
        public void SetPath_Invalid_characters_in_path_throws_an_exception()
        {
            TestDelegate test = delegate
            {
                new SanitizedPath("C>lua");
            };

            Assert.Throws<FormatException>(test,
                "Throw exception when the new path contains invalid characters.");
        }

        #endregion

        #region Proper Usages for GetRelativeName

        [TestCase]
        public void GetRelativeName()
        {
            sanitized = new SanitizedPath(@"C:\lua\Weapons\Shotgun.lua");
            ISanitizedPath baseDirectory = new SanitizedPath(@"C:\lua");

            string relativeName = sanitized.GetRelativeName(baseDirectory);

            Assert.AreEqual(@"lua/Weapons/Shotgun.lua", relativeName);
        }

        #endregion

        #region Improper Usages for GetRelativeName

        [TestCase]
        public void GetRelativeName_Passing_a_null_argument_throws_an_exception()
        {
            sanitized = new SanitizedPath(@"C:\lua\Weapons\Shotgun.lua");


            TestDelegate test = delegate
            {
                sanitized.GetRelativeName(null);
            };

            Assert.Throws<ArgumentNullException>(test);
        }

        #endregion

        #region Testing Equals

        [TestCase]
        public void Equals()
        {
            sanitized = new SanitizedPath("C:/lua");
            var other = new SanitizedPath(@"C:\lua");

            Assert.IsTrue(sanitized.Equals(other));
        }

        #endregion
    }
}
