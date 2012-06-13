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
    public class SparkObjectTests
    {
        SparkObject sparkObject;

        [TestCase]
        public void QualifiedName__Defaults_To_The_Name_Of_The_Object()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Actor");

            Assert.AreEqual("Actor", sparkObject.QualifiedName);
        }

        #region Library Tests

        [TestCase]
        public void Library__Defaults_To_Shared()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");

            Assert.AreEqual(Library.Shared, sparkObject.Library);
        }

        [TestCase]
        public void ExistsOnClient__True_When_Library_Is_Client()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");
            sparkObject.Library = Library.Client;

            Assert.IsTrue(sparkObject.ExistsOnClient);
        }

        [TestCase]
        public void ExistsOnClient__True_When_Library_Is_Shared()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");
            sparkObject.Library = Library.Shared;

            Assert.IsTrue(sparkObject.ExistsOnClient);
        }

        [TestCase]
        public void ExistsOnClient__False_When_Library_Is_Server()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");
            sparkObject.Library = Library.Server;

            Assert.IsFalse(sparkObject.ExistsOnClient);
        }

        [TestCase]
        public void ExistsOnServer__True_When_Library_Is_Server()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");
            sparkObject.Library = Library.Server;

            Assert.IsTrue(sparkObject.ExistsOnServer);
        }

        [TestCase]
        public void ExistsOnServer__True_When_Library_Is_Shared()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");
            sparkObject.Library = Library.Shared;

            Assert.IsTrue(sparkObject.ExistsOnServer);
        }

        [TestCase]
        public void ExistsOnServer__False_When_Library_Is_Client()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");
            sparkObject.Library = Library.Client;

            Assert.IsFalse(sparkObject.ExistsOnServer);
        }

        #endregion

    }

    [TestFixture]
    public class SparkObject_TagTests
    {
        SparkObject sparkObject;

        [SetUp]
        public void SetUp()
        {
            sparkObject = MockRepository.GeneratePartialMock<SparkObject>("Entity");
        }

        [TestCase]
        public void serveronly__Sets_Library_To_Server()
        {
            sparkObject.ParseComment(null, "@serveronly");

            Assert.AreEqual(Library.Server, sparkObject.Library);
        }

        [TestCase]
        public void clientonly__Sets_Library_To_Client()
        {
            sparkObject.ParseComment(null, "@clientonly");

            Assert.AreEqual(Library.Client, sparkObject.Library);
        }

        [TestCase]
        public void shared__Sets_Library_To_Shared()
        {
            sparkObject.ParseComment(null, "@shared");

            Assert.AreEqual(Library.Shared, sparkObject.Library);
        }

        [TestCase]
        public void brief__Sets_Brief_Property()
        {
            sparkObject.ParseComment(null, "@brief A description");

            Assert.AreEqual("A description", sparkObject.Brief);
        }

        [TestCase]
        public void brief__Can_Span_Multiple_Lines_Using_Linux_Line_Encoding()
        {
            sparkObject.ParseComment(null, "@brief Lorem ipsum dolor sit amet, consectetur adipiscing elit. \n" +
                "Vestibulum quis nibh neque. Nam pharetra felis a tellus scelerisque ac pharetra nisi \n" +
                "porttitor.");

            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing elit. \n" +
                "Vestibulum quis nibh neque. Nam pharetra felis a tellus scelerisque ac pharetra nisi \n" +
                "porttitor.", sparkObject.Brief);
        }

        [TestCase]
        public void brief__Can_Span_Multiple_Lines_Using_Windows_Line_Encoding()
        {
            sparkObject.ParseComment(null, "@brief Lorem ipsum dolor sit amet, consectetur adipiscing elit. \r\n" +
                "Vestibulum quis nibh neque. Nam pharetra felis a tellus scelerisque ac pharetra nisi \r\n" +
                "porttitor.");

            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing elit. \r\n" +
                "Vestibulum quis nibh neque. Nam pharetra felis a tellus scelerisque ac pharetra nisi \r\n" +
                "porttitor.", sparkObject.Brief);
        }

        [TestCase]
        public void brief__Tag_Is_Implicite_When_Comment_Starts_With_No_Defined_Tag()
        {
            sparkObject.ParseComment(null, "A description");

            Assert.AreEqual("A description", sparkObject.Brief);
        }

        [TestCase]
        public void version__Sets_The_Version_Property()
        {
            sparkObject.ParseComment(null, "@version 1.2.3");

            Assert.AreEqual("1.2.3", sparkObject.Version);
        }

        [TestCase]
        public void deprecated__Sets_ReasonForDeprecation()
        {
            sparkObject.ParseComment(null, "@deprecated Reason for deprecation.");

            Assert.AreEqual("Reason for deprecation.", sparkObject.ReasonForDeprecation);
        }

        [TestCase]
        public void tag__Adds_A_Tag_To_The_Object()
        {
            sparkObject.ParseComment(null, "@tag aTag");
            sparkObject.ParseComment(null, "@tag anotherTag");

            Assert.Contains("aTag", sparkObject.Tags.ToArray());
            Assert.Contains("anotherTag", sparkObject.Tags.ToArray());
        }

        [TestCase]
        public void seealso__Adds_To_SeeAlso()
        {
            sparkObject.ParseComment(null, "@seealso Actor");
            sparkObject.ParseComment(null, "@seealso Player");
            
            Assert.Contains("Actor", sparkObject.SeeAlso.ToArray());
            Assert.Contains("Player", sparkObject.SeeAlso.ToArray());
        }

        [TestCase]
        public void author__Adds_An_Author_To_Authors()
        {
            sparkObject.ParseComment(null, "@author Damien Hauta");
            sparkObject.ParseComment(null, "@author Someone Else: What they contributed.");
            

            IAuthor damienHauta = sparkObject.Authors.Where(x => x.Name == "Damien Hauta").SingleOrDefault();
            IAuthor someoneElse = sparkObject.Authors.Where(x => x.Name == "Someone Else").SingleOrDefault();

            Assert.IsNotNull(damienHauta);
            Assert.AreEqual("Damien Hauta", damienHauta.Name);

            Assert.IsNotNull(someoneElse);
            Assert.AreEqual("Someone Else", someoneElse.Name);
            Assert.AreEqual("What they contributed.", someoneElse.Contribution);
        }

        [TestCase]
        public void example__Adds_An_Example_To_Examples()
        {
            sparkObject.ParseComment(null, "@example Title of the example\n" +
                                      "print('lua code')");

            IExample example = sparkObject.Examples.FirstOrDefault();

            Assert.IsNotNull(example);
            Assert.AreEqual("Title of the example", example.Title);
            Assert.AreEqual("print('lua code')", example.Sample);
        }
    }
}
