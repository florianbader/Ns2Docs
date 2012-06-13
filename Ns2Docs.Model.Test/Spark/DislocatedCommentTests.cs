using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark;

namespace Ns2Docs.Model.Test.Spark
{
    [TestFixture]
    class DislocatedCommentTests
    {
        [TestCase]
        public void ClassComment()
        {
            string declaration = "class SomeClass";
            IGame game = new Game();

            new DislocatedComment(game, new SourceCode(""), declaration, Library.Shared, 0);

            ITable someClass = game.FindTableWithName("SomeClass");
            Assert.IsNotNull(someClass);
        }

        [TestCase]
        public void FieldComment()
        {
            string declaration = "field SomeClass.something";
            IGame game = new Game();

            new DislocatedComment(game, new SourceCode(""), declaration, Library.Shared, 0);

            ITable someClass = game.FindTableWithName("SomeClass");
            IField something = null;
            if (someClass != null)
            {
                something = someClass.Fields.FirstOrDefault();
            }

            Assert.IsNotNull(someClass);
            Assert.IsNotNull(something);
        }
    }
}
