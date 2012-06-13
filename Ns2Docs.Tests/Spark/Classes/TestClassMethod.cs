using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Mocks;
using Ns2Docs.Spark.Classes;

namespace Ns2Docs.Tests.Spark.Classes
{
    [TestFixture()]
    public class TestClassMethod
    {
        Class luaClass;

        [SetUp]
        public void SetUp()
        {
            luaClass = new Class("Actor");
        }

        [Test]
        public void TestQualifiedName()
        {
            ClassMethod method = new ClassMethod(luaClass, "DoSomething");

            Assert.AreEqual("Actor.DoSomething", method.QualifiedName);
        }

        [Test]
        public void AutomaticallyBuildsBiDirectionalRelationshipWithClass()
        {
            ClassMethod method = new ClassMethod(luaClass, "DoSomething");
            Assert.IsNotEmpty(luaClass.GetClassMethods());
        }

        [Test]
        public void DisposingAClassMethodRemovesItFromTheClassMethods()
        {
            ClassMethod method = new ClassMethod(luaClass, "DoSomething");
            method.Dispose();
            Assert.IsEmpty(luaClass.GetClassMethods());
        }

        [Test]
        public void MethodOverriding()
        {
            Class entity = new Class("Entity");
            Class actor = new Class("Actor", entity);
            Class scriptActor = new Class("ScriptActor", actor);

            ClassMethod entityMethod = new ClassMethod(entity, "DoSomething");
            ClassMethod actorMethod = new ClassMethod(actor, "DoSomething");
            ClassMethod scriptActorMethod = new ClassMethod(scriptActor, "DoSomething");

            Assert.IsNull(entityMethod.Overrides);
            Assert.AreEqual(entityMethod, actorMethod.Overrides);
            Assert.AreEqual(entityMethod, scriptActorMethod.Overrides);
        }
    }
}
