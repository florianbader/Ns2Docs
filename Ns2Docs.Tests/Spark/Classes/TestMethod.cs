using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark.Classes;

namespace Ns2Docs.Tests.Spark.Classes
{
    [TestFixture()]
    public class TestMethod
    {
        Class actor;

        [SetUp]
        public void SetUp()
        {
            actor = new Class("Actor");
        }

        [Test]
        public void TestQualifiedName()
        {
            Method method = new Method(actor, "DoSomething");

            Assert.AreEqual("Actor:DoSomething", method.QualifiedName);
        }

        [Test]
        public void AutomaticallyBuildsBiDirectionalRelationshipWithClass()
        {
            Method method = new Method(actor, "DoSomething");
            Assert.IsNotEmpty(actor.GetMethods());
        }

        [Test]
        public void DisposingAMethodRemovesItFromTheMethods()
        {
            Method method = new Method(actor, "DoSomething");
            method.Dispose();
            Assert.IsEmpty(actor.GetMethods());
        }

        [Test]
        public void MethodOverriding()
        {
            Class entity = new Class("Entity");
            Class actor = new Class("Actor", entity);
            Class scriptActor = new Class("ScriptActor", actor);

            Method entityMethod = new Method(entity, "DoSomething");
            Method actorMethod = new Method(actor, "DoSomething");
            Method scriptActorMethod = new Method(scriptActor, "DoSomething");

            Assert.IsNull(entityMethod.Overrides);
            Assert.AreEqual(entityMethod, actorMethod.Overrides);
            Assert.AreEqual(entityMethod, scriptActorMethod.Overrides);
        }

    }
}
