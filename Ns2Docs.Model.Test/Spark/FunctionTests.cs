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
    public class Function_TagTests
    {
        Function function;

        [SetUp]
        public void SetUp()
        {
            function = new Function("SomeFunction");
        }

        #region Param Tag

        [TestCase]
        public void param__Adds_A_Parameter_To_Parameters()
        {
            function.ParseComment(null, "@param someParam A description of the parameter.");

            IParameter someParam = function.Parameters.FirstOrDefault();

            Assert.IsNotNull(someParam);
            Assert.AreEqual("someParam", someParam.Name);
            Assert.AreEqual("A description of the parameter.", someParam.Brief);
        }

        #endregion

        #region Return Tag

        [TestCase]
        public void return__Adds_A_Return_To_Returns()
        {
            function.ParseComment(null, "@return A description of what's being returned.");

            IFunctionReturn ret = function.Returns.FirstOrDefault();

            Assert.IsNotNull(ret);
            Assert.AreEqual("A description of what's being returned.", ret.Brief);
        }

        [TestCase]
        public void return__A_Description_Is_Optional()
        {
            function.ParseComment(null, "@return <number>");

            IFunctionReturn ret = function.Returns.FirstOrDefault();

            Assert.IsNotNull(ret);
            Assert.AreEqual("number", ret.Datatypes[0]);
            Assert.IsNull(ret.Brief);
        }

        [TestCase]
        public void return__Specify_Return_Data_Type_With_Angled_Brackets()
        {
            function.ParseComment(null, "@return <boolean, number> A description of what's being returned.");

            IFunctionReturn ret = function.Returns.FirstOrDefault();

            Assert.IsNotNull(ret);
            Assert.AreEqual("boolean", ret.Datatypes[0]);
            Assert.AreEqual("number", ret.Datatypes[1]);
            Assert.AreEqual("A description of what's being returned.", ret.Brief);
        }

        [TestCase]
        public void return__Conditional_Return_Data_Types()
        {
            function.ParseComment(null, "@return when(couldn't find substring)<nil, number> A description of what's being returned.");

            IFunctionReturn ret = function.Returns.FirstOrDefault();

            Assert.IsNotNull(ret);
            Assert.AreEqual("couldn't find substring", ret.When);
            Assert.AreEqual("nil", ret.Datatypes[0]);
            Assert.AreEqual("number", ret.Datatypes[1]);
            Assert.AreEqual("A description of what's being returned.", ret.Brief);
        }

        [TestCase]
        public void return__Conditional_Statement_And_Data_Types_CANNOT_Be_Swapped_Around()
        {
            function.ParseComment(null, "@return <nil, number> when(couldn't find substring) A description of what's being returned.");

            IFunctionReturn ret = function.Returns.FirstOrDefault();

            Assert.IsNotNull(ret);
            Assert.AreNotEqual("couldn't find substring", ret.When);
            Assert.AreEqual("nil", ret.Datatypes[0]);
            Assert.AreEqual("number", ret.Datatypes[1]);
            Assert.AreEqual("when(couldn't find substring) A description of what's being returned.", ret.Brief);
        }

        #endregion
    }
}
