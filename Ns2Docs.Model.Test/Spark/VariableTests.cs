using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark;

namespace Ns2Docs.Model.Test.Spark
{
    [TestFixture]
    public class VariableTests
    {
        [TestCase]
        public void IsConstant__True_When_Name_Is_Prefixed_With_k()
        {
            Variable constantVariable = new Variable("kSomeVar");

            Assert.IsTrue(constantVariable.IsConstant);
        }
    }

    [TestFixture]
    public class Variable_TagTests
    {
        Variable variable;

        [SetUp]
        public void SetUp()
        {
            variable = new Variable("SomeVar");
        }

        [TestCase]
        public void datatype__Sets_Datatype_Property()
        {
            variable.ParseComment(null, "@datatype number");

            Assert.AreEqual("number", variable.Datatype);
        }
    }
}
