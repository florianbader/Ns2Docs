using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark.Parsing;
using NUnit.Mocks;
using Ns2Docs.Spark;

namespace Ns2Docs.Model.Test.Spark.Parsing
{
    [TestFixture]
    public class SparkParserTest
    {
        private IGame game;
        private IParser parser;
        private ISourceCode sourceCode;

        private DynamicMock sourceCodeMock;

        public class ParsingBase
        {
            protected SparkParser Parser { get; set; }
            protected virtual void MakeParser()
            {
                Parser = new SparkParser();
            }

            protected class CreatingObjectIsRaisedReturn
            {
                public string Type { get; set; }
                public int TimesRaised { get; set; }
                public bool WasRaisedOnce { get { return TimesRaised == 1; } }
            }

            protected CreatingObjectIsRaisedReturn CreatingObjectIsRaised()
            {
                CreatingObjectIsRaisedReturn ret = new CreatingObjectIsRaisedReturn();
                Parser.CreatingObject += delegate(string type)
                {
                    ret.TimesRaised++;
                    ret.Type = type;
                };
                return ret;
            }

        }

        [TestFixture]
        public class Classes : ParsingBase
        {
            [TestCase]
            public void BaseClass()
            {
                string entityDeclaration = "class 'Entity'";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(entityDeclaration));
                ITable entity = game.FindTableWithName("Entity");

                Assert.IsNotNull(entity);
                
                Assert.AreEqual("Entity", entity.Name);
            }

            [TestCase]
            public void Subclass()
            {
                string actorDeclaration = "class 'Actor' (Entity)";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(actorDeclaration));
                ITable actor = game.FindTableWithName("Actor");
                ITable entity = game.FindTableWithName("Entity");

                Assert.IsNotNull(actor);
                Assert.IsNotNull(entity);

                Assert.AreEqual(entity, actor.BaseTable);
                Assert.AreEqual("Actor", actor.Name);
                Assert.AreEqual("Entity", entity.Name);
            }

            [TestCase]
            public void ClassWithDoubleQuotes()
            {
                string entityDeclaration = "class \"Entity\"";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(entityDeclaration));
                ITable entity = game.FindTableWithName("Entity");

                Assert.IsNotNull(entity);
            }
        }

        [TestFixture]
        public class Functions : ParsingBase
        {
            [TestCase]
            public void SimpleFunction()
            {
                string functionDeclaration = "function SomeFunc() end";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(functionDeclaration));
                IFunction someFunc = game.Functions.FirstOrDefault();

                Assert.AreEqual(1, game.Functions.Count);

                Assert.AreEqual("SomeFunc", someFunc.Name);
            }

            [TestCase]
            public void IgnoresLocalFunctions()
            {
                string functionDeclaration = "local function SomeFunc() end";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(functionDeclaration));

                Assert.AreEqual(0, game.Functions.Count);
            }

            [TestCase]
            public void FunctionWithParameters()
            {
                string declaration = "function SomeFunc(argOne, argTwo, ...) end";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                IFunction someFunc = game.Functions.FirstOrDefault();
                IList<IParameter> parameters = null;
                IParameter argOne = null;
                IParameter argTwo = null; 
                IParameter varArg = null;
                if (someFunc != null)
                {
                    parameters = someFunc.Parameters;
                    argOne = parameters[0];
                    argTwo = parameters[1];
                    varArg = parameters[2];
                }

                Assert.IsNotNull(someFunc);

                Assert.AreEqual("argOne", argOne.Name);
                Assert.AreEqual("argTwo",  argTwo.Name);
                Assert.AreEqual("...", varArg.Name);
                /*Assert.AreEqual(0, argOne.Index);
                Assert.AreEqual(1, argTwo.Index);
                Assert.AreEqual(2, varArg.Index);*/

                Assert.AreEqual(3, parameters.Count);
            }

            [TestCase]
            public void FunctionWithOnlyVarArgForParameter()
            {
                string declaration = "function SomeFunc(...) end";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                IFunction someFunc = game.Functions.FirstOrDefault();
                IList<IParameter> parameters = null;
                if (someFunc != null)
                {
                    parameters = someFunc.Parameters;
                }

                Assert.IsNotNull(someFunc);

                Assert.AreEqual("...", parameters[0].Name);
            }
        }
        
        [TestFixture]
        public class PrepareMixins : ParsingBase
        {
            [TestCase]
            public void SimpleMixin()
            {
                string prepareClass = "PrepareClassForMixin(Actor, SomeMixin)";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(prepareClass));
                ITable actor = game.FindTableWithName("Actor");
                ITable someMixin = game.FindTableWithName("SomeMixin");

                Assert.IsNotNull(actor, "Create the class if it doesn't already exsist.");
                Assert.IsNotNull(someMixin, "Create the mixin table if it doesn't already exist.");
                Assert.AreSame(someMixin, actor.Mixins.FirstOrDefault(), "Add the mixin to the class.");
            }
        }

        [TestFixture]
        public class StaticFunctions : ParsingBase
        {
            [TestCase]
            public void SimpleStaticFunction()
            {
                string declaration = "function Actor.DoSomething() end";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable actor = game.FindTableWithName("Actor");
                IStaticFunction doSomething = actor.StaticFunctions.FirstOrDefault();
                
                Assert.IsNotNull(actor);
                Assert.IsNotNull(doSomething);

                Assert.AreEqual("DoSomething", doSomething.Name);
                Assert.AreEqual(actor, doSomething.Table);
            }

            [TestCase]
            public void StaticFunctionWithParameters()
            {
                string declaration = "function SomeClass.SomeFunc(argOne, argTwo, ...) end";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable someClass = game.FindTableWithName("SomeClass");
                IStaticFunction someFunc = someClass.StaticFunctions.FirstOrDefault();

                IList<IParameter> parameters = null;
                IParameter argOne = null;
                IParameter argTwo = null;
                IParameter varArg = null;
                if (someFunc != null)
                {
                    parameters = someFunc.Parameters;
                    argOne = parameters[0];
                    argTwo = parameters[1];
                    varArg = parameters[2];
                }

                Assert.IsNotNull(someFunc);

                Assert.AreEqual("argOne", argOne.Name);
                Assert.AreEqual("argTwo", argTwo.Name);
                Assert.AreEqual("...", varArg.Name);
                /*Assert.AreEqual(0, argOne.Index);
                Assert.AreEqual(1, argTwo.Index);
                Assert.AreEqual(2, varArg.Index);
                */
                Assert.AreEqual(3, parameters.Count);
            }

            [TestCase]
            public void MixinStaticFunction()
            {
                string statement = String.Join("\n", new string[] {
                    "SomeMixin = {}",
                    "function SomeMixin.DoSomething()",
                    "end"
                });
                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(statement));
                ITable mixin = game.Tables.FirstOrDefault();
                IStaticFunction doSomething = null;
                if (mixin != null)
                {
                    doSomething = mixin.StaticFunctions.FirstOrDefault();
                }

                Assert.IsNotNull(mixin);
                Assert.IsNotNull(doSomething);
                Assert.AreEqual("DoSomething", doSomething.Name);
            }
        }
        
        [TestFixture]
        public class Methods : ParsingBase
        {
            [TestCase]
            public void SimpleMethod()
            {
                string declaration = "function Actor:SomeMethod() end";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable actor = game.FindTableWithName("Actor");

                Assert.IsNotNull(actor);
                Assert.AreEqual(1, actor.Methods.Count);
            }
        }
        
        [TestFixture]
        public class Variables : ParsingBase
        {
            [TestCase]
            public void SimpleAssignment()
            {
                string assingment = "someVar = 'hello'";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(assingment));
                var variables = game.FindVariablesWithName("someVar");
                IVariable someVar = variables.FirstOrDefault();
                
                Assert.IsNotNull(someVar);
                Assert.AreEqual("'hello'", someVar.Assignment);
            }
            
            [TestCase]
            public void OnlyParsesWhenInTheGlobalScope()
            {
                string assignment = String.Join("\n", new string[] {
                    "do",
                    "    someVar = 123",
                    "end"
                });
                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(assignment));

                Assert.AreEqual(0, game.Variables.Count);
            }

            [TestCase]
            public void IgnoresLocalVariable()
            {
                string assignment = "local someVar = 123";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(assignment));

                Assert.AreEqual(0, game.Variables.Count);
            }

            [TestCase]
            public void IgnoresSubscriptedAssignments()
            {
                string subscripted = "someVar.value = 123";
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(subscripted));
                
                Assert.AreEqual(0, game.Variables.Count);
            }
            
            [TestCase]
            public void IgnoresIndexedAssignments()
            {
                string indexed = "someVar[1] = 'hello'";
                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(indexed));

                Assert.AreEqual(0, game.Variables.Count);
            }
            
            [TestCase]
            public void FunctionAsAnAssignment()
            {
                string assignment = "someVar = function() end";
                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(assignment));
                IVariable someVar = game.Variables.FirstOrDefault();

                Assert.IsNotNull(someVar);
                Assert.AreEqual("function() end", someVar.Assignment);
            }
        }
        
        [TestFixture]
        public class Fields : ParsingBase
        {
            [TestCase]
            public void SimpleField()
            {
                string declaration = String.Join("\n", new string[] {
                    "class 'SomeClass'",
                    "function SomeClass:OnInit()",
                    "    hello = 2",
                    "    self.someField = 123",
                    "end"
                });
                MakeParser();
                
                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable someClass = game.FindTableWithName("SomeClass");
                IField someField = null;
                if (someClass != null)
                {
                    someField = someClass.Fields.FirstOrDefault();
                }
                
                Assert.AreEqual(0, game.Variables.Count);
                Assert.IsNotNull(someClass);
                Assert.IsNotNull(someField);
                Assert.AreEqual("someField", someField.Name);
                Assert.AreEqual("123", someField.Assignment);
            }
        }

        [TestFixture]
        public class StaticFields : ParsingBase
        {
            [TestCase]
            public void SimpleStaticField()
            {
                string declaration = String.Join("\n", new string[] {
                    "class 'SomeClass'",
                    "SomeClass.kMapName = 'someclass'"
                });
                MakeParser();

                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable someClass = game.FindTableWithName("SomeClass");
                IStaticField kMapName = null;
                if (someClass != null)
                {
                    kMapName = someClass.StaticFields.FirstOrDefault();
                }
                
                Assert.AreEqual(0, game.Variables.Count);
                Assert.IsNotNull(someClass);
                Assert.IsNotNull(kMapName);
                
                Assert.AreEqual("kMapName", kMapName.Name);
                Assert.AreEqual("'someclass'", kMapName.Assignment);
                Assert.AreEqual(someClass, kMapName.Table);
            }
            
            [TestCase]
            public void AssignedToAFunction()
            {
                string declaration = String.Join("\n", new string[] {
                    "class 'SomeClass'",
                    "SomeClass.Instance = function() end"
                });
                MakeParser();
                
                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable someClass = game.FindTableWithName("SomeClass");
                IStaticField instance = null;
                if (someClass != null)
                {
                    instance = someClass.StaticFields.FirstOrDefault();
                }

                Assert.AreEqual(0, game.Variables.Count);
                Assert.IsNotNull(someClass);
                Assert.IsNotNull(instance);

                Assert.AreEqual("Instance", instance.Name);
                Assert.AreEqual("function() end", instance.Assignment);
                Assert.AreEqual(someClass, instance.Table);
            }
        }
        
        [TestFixture]
        public class Mixins : ParsingBase
        {
            [TestCase]
            public void AVariableWithANameEndingWithMixinBeingAssignedToATableConstructorRegistersAsATable()
            {
                string declaration = "AttackOrderMixin = {}";
                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable attackOrderMixin = game.FindTableWithName("AttackOrderMixin");

                Assert.IsNotNull(attackOrderMixin);
                Assert.AreEqual("AttackOrderMixin", attackOrderMixin.Name);
                Assert.AreEqual(0, game.Variables.Count, "Don't count mixin declarations as variables");
            }

            [TestCase]
            public void ClientOnlyMixin()
            {
                string declaration = String.Join("\n", new string[] {
                    "if Client then",
                    "    AttackOrderMixin = {}",
                    "end"
                });
                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable attackOrderMixin = game.Tables.FirstOrDefault();

                Assert.IsNotNull(attackOrderMixin);
                Assert.AreEqual(Library.Client, attackOrderMixin.Library);
            }

            [TestCase]
            public void ServerOnlyMixin()
            {
                string declaration = String.Join("\n", new string[] {
                    "if Server then",
                    "    AttackOrderMixin = {}",
                    "end"
                });
                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                ITable attackOrderMixin = game.Tables.FirstOrDefault();

                Assert.IsNotNull(attackOrderMixin);
                Assert.AreEqual(Library.Server, attackOrderMixin.Library);
            }
        }

        [TestFixture]
        public class Ifs : ParsingBase
        {
            [TestCase]
            public void ServerOnlyIfStatement()
            {
                string declaration = String.Join("\n", new string[] {
                    "if Server then",
                    "    function SomeFunc()",
                    "    end",
                    "end"
                });

                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                IFunction someFunc = game.Functions.FirstOrDefault();

                Assert.IsNotNull(someFunc);
                Assert.AreEqual(Library.Server, someFunc.Library);
            }

            [TestCase]
            public void ClientOnlyIfStatement()
            {
                string declaration = String.Join("\n", new string[] {
                    "if Client then",
                    "    function SomeFunc()",
                    "    end",
                    "end"
                });

                MakeParser();
                IGame game = Parser.ParseSourceCode(new SourceCode(declaration));
                IFunction someFunc = game.Functions.FirstOrDefault();

                Assert.IsNotNull(someFunc);
                Assert.AreEqual(Library.Client, someFunc.Library);
            }
        }
        
        [SetUp]
        public void SetUp()
        {
            game = new Game();
            sourceCodeMock = new DynamicMock(typeof(ISourceCode));
        }

        private void MakeParser()
        {
            parser = new SparkParser();
        }

        class CreatingObjectIsRaisedReturn
        {
            public string Type { get; set; }
            public int TimesRaised { get; set; }
            public bool WasRaised { get { return TimesRaised > 0; } }
        }
        private CreatingObjectIsRaisedReturn CreatingObjectIsRaised(IParser parser)
        {
            CreatingObjectIsRaisedReturn ret = new CreatingObjectIsRaisedReturn();
            parser.CreatingObject += delegate(string type)
            {
                ret.TimesRaised++;
                ret.Type = type;
            };
            return ret;
        }

        [TestCase]
        public void CreatingObjectRaisedWhenParsingAClassWithNoBaseClass()
        {
            string classDeclaration = "class 'Entity'";
            MakeParser();
            var creatingObject = CreatingObjectIsRaised(parser);

            parser.ParseSourceCode(new SourceCode(classDeclaration));

            Assert.True(creatingObject.WasRaised);
            Assert.AreEqual(1, creatingObject.TimesRaised);
            Assert.AreEqual("Class", creatingObject.Type);
        }

        [TestCase]
        public void CreatingObjectRaisedWhenParsingAClassWithABaseClass()
        {
            string classDeclaration = "class 'Actor' (Entity)";
            MakeParser();
            var creatingObject = CreatingObjectIsRaised(parser);

            parser.ParseSourceCode(new SourceCode(classDeclaration));

            Assert.True(creatingObject.WasRaised);
            Assert.AreEqual(1, creatingObject.TimesRaised);
            Assert.AreEqual("Class", creatingObject.Type);
        }

        [TestCase]
        public void CreatingObjectRaisedWhenPreparingAClassForAMixin()
        {
            string prepareMixin = "PrepareClassForMixin(hello, world)";
            MakeParser();
            var creatingObject = CreatingObjectIsRaised(parser);

            parser.ParseSourceCode(new SourceCode(prepareMixin));

            Assert.True(creatingObject.WasRaised);
            Assert.AreEqual(1, creatingObject.TimesRaised);
            Assert.AreEqual("PrepareMixin", creatingObject.Type);
        }

        /*
        private void MakeSourceCode(string contents)
        {
            sourceCode = new SourceCode(contents);
        }
        
        private void Parse()
        {
            parser.ParseSourceCode(game, sourceCode);
        }

        [TestCase]
        public void ParsingAClass()
        {
            MakeParser();
            sourceCode = new SourceCode("class 'Actor'");

            parser.ParseSourceCode(game, sourceCode);
            Assert.AreEqual(1, game.Classes.Count);
        }

        [TestCase]
        public void ParsingASubClass()
        {
            MakeParser();
            MakeSourceCode("class 'Actor' (Entity)");

            Parse();

            Assert.AreEqual(2, game.Classes.Count);
        }

        [TestCase]
        public void ParsingAFunction()
        {
            MakeParser();
            MakeSourceCode("function DoSomething()\nend");
            
            Parse();

            Assert.AreEqual(1, game.Functions.Count);
        }

        [TestCase]
        public void Parsing_a_function_residing_only_on_the_client()
        {
            MakeParser();
            MakeSourceCode("function DoSomething()\nend");
            sourceCode.FileName = new SanitizedPath(@"C:\lua\Something_Client.lua");
            sourceCode.BaseDirectory = new SanitizedPath(@"C:\lua");
            sourceCode.PredictLibrary();

            Parse();

            Assert.AreEqual(Library.Client, game.Functions[0].Library);
        } 

        [TestCase]
        public void Parsing_a_function_residing_only_on_the_server()
        {
            MakeParser();
            MakeSourceCode("function DoSomething()\nend");
            sourceCode.FileName = new SanitizedPath(@"C:\lua\Something_Server.lua");
            sourceCode.BaseDirectory = new SanitizedPath(@"C:\lua");
            sourceCode.PredictLibrary();

            Parse();

            Assert.AreEqual(Library.Server, game.Functions[0].Library);
        }

        [TestCase]
        public void Parsing_a_function_residing_only_on_both_server_and_client()
        {
            MakeParser();
            MakeSourceCode("function DoSomething()\nend");
            sourceCode.FileName = new SanitizedPath(@"C:\lua\Something.lua");
            sourceCode.BaseDirectory = new SanitizedPath(@"C:\lua");

            Parse();

            Assert.AreEqual(Library.Shared, game.Functions[0].Library);
        }

        [TestCase]
        public void Parsing_a_method_residing_only_on_both_server_and_client()
        {
            MakeParser();
            MakeSourceCode("function Actor:DoSomething()\nend");
            sourceCode.FileName = new SanitizedPath(@"C:\lua\Something.lua");
            sourceCode.BaseDirectory = new SanitizedPath(@"C:\lua");

            Parse();

            Assert.AreEqual(Library.Shared, game.Classes[0].Methods[0].Library);
            Assert.AreEqual(sourceCode, game.Classes[0].Methods[0].DeclaredIn);
        }

        [TestCase]
        public void ParsingAMethod()
        {
            MakeParser();
            MakeSourceCode("function Actor:DoSomething()\nend");

            Parse();

            Assert.AreEqual(1, game.Classes.Count);
            Assert.AreEqual(1, game.Classes[0].Methods.Count);
        }

        [TestCase]
        public void ParsingAClassMethod()
        {
            MakeParser();
            MakeSourceCode("function Actor.DoSomething()\nend");

            Parse();

            Assert.AreEqual(1, game.Classes.Count);
            Assert.AreEqual(1, game.Classes[0].ClassMethods.Count);
        }
        */
    }
}
