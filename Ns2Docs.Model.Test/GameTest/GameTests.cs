using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
//using Ns2Docs.Spark.Classes;
using Ns2Docs.Spark;

namespace Ns2Docs.Model.Test
{
    namespace GameTest
    {
       /* [TestFixture]
        public class GeneralTests
        {
            #region Fields

            private Game game;

            #endregion

            #region Tests

            [TestCase]
            public void ResolveClass()
            {
                game = new Game();

                SparkClass someClass = new SparkClass("SomeClass");
                game.Classes.Add(someClass);

                ISparkClass resolvedSomeClass = game.ResolveClass("SomeClass");
                ISparkClass doesntExist = game.ResolveClass("DoesntExist");

                Assert.AreEqual(someClass, resolvedSomeClass);
                Assert.IsNull(doesntExist);
            }

            #endregion
        }

        [TestFixture]
        public class TestingAddAndRemovingClasses
        {
            #region Fields

            private Game game;
            private SparkClass entity;

            private ClassAddedDelegate classAddedHandler;
            private ClassRemovedDelegate classRemovedHandler;

            private bool addClassEventRaised;
            private bool removeClassEventRaised;

            private SparkClass classAdded;
            private SparkClass classRemoved;

            #endregion

            #region SetUp

            [SetUp]
            public void SetUp()
            {
                addClassEventRaised = false;
                removeClassEventRaised = false;

                classAddedHandler = delegate(SparkClass sparkClass)
                {
                    addClassEventRaised = true;
                    classAdded = sparkClass;
                };

                classRemovedHandler = delegate(SparkClass sparkClass)
                {
                    removeClassEventRaised = true;
                    classRemoved = sparkClass;
                };

                entity = new SparkClass("Entity");
                game = new Game();
            }

            #endregion

            #region Arrangement

            private void ApplyDelegates()
            {
                ApplyAddDelegate();
                ApplyRemoveDelegate();
            }

            private void ApplyAddDelegate()
            {
                game.ClassAdded += classAddedHandler;
            }

            private void ApplyRemoveDelegate()
            {
                game.ClassRemoved += classRemovedHandler;
            }

            #endregion

            #region Tests

            #region Adding Classes

            [TestCase]
            public void ClassAddedIsRaisedWhenUsingAddClass()
            {
                ApplyDelegates();

                game.AddClass(entity);

                Assert.IsTrue(addClassEventRaised);
                Assert.AreEqual(entity, classAdded);
            }

            [TestCase]
            public void ClassAddedIsRaisedWhenUsingClasses()
            {
                ApplyDelegates();

                game.Classes.Add(entity);

                Assert.IsTrue(addClassEventRaised);
                Assert.AreEqual(entity, classAdded);
            }

            [TestCase]
            public void ClassRemovedIsNotRaisedWhenAddingAClass()
            {
                ApplyAddDelegate();

                game.AddClass(entity);

                Assert.IsFalse(removeClassEventRaised);
                Assert.IsNull(classRemoved);
            }

            #endregion

            #region Removing Classes

            [TestCase]
            public void ClassRemovedIsRaisedWhenUsingRemoveClass()
            {
                ApplyDelegates();

                game.AddClass(entity);
                game.RemoveClass(entity);

                Assert.IsTrue(removeClassEventRaised);
                Assert.AreEqual(entity, classRemoved);
            }

            [TestCase]
            public void ClassRemovedIsRaisedWhenUsingClasses()
            {
                ApplyDelegates();

                game.Classes.Add(entity);
                game.Classes.Remove(entity);

                Assert.IsTrue(removeClassEventRaised);
                Assert.AreEqual(entity, classRemoved);
            }

            [TestCase]
            public void ClassAddedIsNotRaisedWhenRemovingAClass()
            {
                game.AddClass(entity);
                ApplyAddDelegate();

                game.RemoveClass(entity);

                Assert.IsFalse(addClassEventRaised);
                Assert.IsNull(classAdded);
            }

            #endregion

            #endregion
        }

        [TestFixture]
        public class TestingAddAndRemovingFunctions
        {
            #region Fields

            private Game game;
            private Function findEntity;

            private FunctionAddedDelegate functionAddedHandler;
            private FunctionRemovedDelegate functionRemovedHandler;

            private bool addFunctionEventRaised;
            private bool removeFunctionEventRaised;

            private Function functionAdded;
            private Function functionRemoved;

            #endregion

            #region SetUp

            [SetUp]
            public void SetUp()
            {
                addFunctionEventRaised = false;
                removeFunctionEventRaised = false;

                functionAddedHandler = delegate(Function function)
                {
                    addFunctionEventRaised = true;
                    functionAdded = function;
                };

                functionRemovedHandler = delegate(Function function)
                {
                    removeFunctionEventRaised = true;
                    functionRemoved = function;
                };

                findEntity = new Function("FindEntity");

                game = new Game();
            }

            #endregion

            #region Arrangement

            private void ApplyDelegates()
            {
                ApplyAddDelegate();
                ApplyRemoveDelegate();
            }

            private void ApplyAddDelegate()
            {
                game.FunctionAdded += functionAddedHandler;
            }

            private void ApplyRemoveDelegate()
            {
                game.FunctionRemoved += functionRemovedHandler;
            }

            #endregion

            #region Tests

            #region Adding Functions

            [TestCase]
            public void FunctionAddedIsRaisedWhenUsingAddFunction()
            {
                ApplyDelegates();

                game.AddFunction(findEntity);

                Assert.IsTrue(addFunctionEventRaised);
                Assert.AreEqual(findEntity, functionAdded);
            }

            [TestCase]
            public void FunctionAddedIsRaisedWhenUsingFunctions()
            {
                ApplyDelegates();

                game.Functions.Add(findEntity);

                Assert.IsTrue(addFunctionEventRaised);
                Assert.AreEqual(findEntity, functionAdded);
            }

            [TestCase]
            public void FunctionRemovedIsNotRaisedWhenAddingAFunction()
            {
                ApplyAddDelegate();

                game.AddFunction(findEntity);

                Assert.IsFalse(removeFunctionEventRaised);
                Assert.IsNull(functionRemoved);
            }

            #endregion

            #region Removing Functions

            [TestCase]
            public void FunctionRemovedIsRaisedWhenUsingRemoveFunction()
            {
                ApplyDelegates();

                game.AddFunction(findEntity);
                game.RemoveFunction(findEntity);

                Assert.IsTrue(addFunctionEventRaised);
                Assert.AreEqual(findEntity, functionRemoved);
            }

            [TestCase]
            public void FunctionRemovedIsRaisedWhenUsingFunctions()
            {
                ApplyDelegates();

                game.Functions.Add(findEntity);
                game.Functions.Remove(findEntity);

                Assert.IsTrue(removeFunctionEventRaised);
                Assert.AreEqual(findEntity, functionRemoved);
            }

            [TestCase]
            public void FunctionAddedIsNotRaisedWhenRemovingAFunction()
            {
                game.AddFunction(findEntity);
                ApplyAddDelegate();

                game.RemoveFunction(findEntity);

                Assert.IsFalse(addFunctionEventRaised);
                Assert.IsNull(functionAdded);
            }

            #endregion

            #endregion
        }*/
    }
}
