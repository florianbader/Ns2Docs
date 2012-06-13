using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark.Classes;

namespace Ns2Docs.Tests.Spark.Classes
{
    [TestFixture()]
    public class TestClass
    {
        [Test]
        public void AddingClassMethodWithIncorrectRelationship()
        {
            Class someClass = new Class("SomeClass");
            Class anotherClass = new Class("AnotherClass");

            ClassMethod method = new ClassMethod(anotherClass, "DoSomething");

            TestDelegate test = delegate()
            {
                someClass.AddClassMethod(method);
            };
            Assert.Throws<ArgumentException>(test);
        }
    }
}
