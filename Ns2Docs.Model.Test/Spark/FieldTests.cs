using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark;
using Rhino.Mocks;

namespace Ns2Docs.Model.Test.Spark
{
    [TestFixture]
    public class Field_TagTests
    {
        Field field;

        [SetUp]
        public void SetUp()
        {
            ITable table = MockRepository.GenerateStub<ITable>();
            field = new Field(table, "someField");
        }

        [TestCase]
        public void networkvar__Sets_IsNetworkVar_To_True()
        {
            field.ParseComment(null, "@networkvar");

            Assert.IsTrue(field.IsNetworkVar);
        }
    }
}
