using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace Ns2Docs.Spark.Parsing
{
    public class SparkParser : IParser
    {
        #region Fields

        private Lua lua;
        private LuaFunction parseString;
        private string luaFileName;
        private string luaPath;
        private string luaCPath;

        #endregion

        public event CreatingObjectDelegate CreatingObject = delegate { };


        public SparkParser()
        {
            string codeBaseUri = Assembly.GetExecutingAssembly().CodeBase;
            string codeBase = new Uri(codeBaseUri).AbsolutePath;
            string baseDir = Path.GetDirectoryName(codeBase);
            luaPath = String.Format(@"{0}\Scripts\?.lua", baseDir).Replace("\\", "/");
            luaCPath = String.Format(@"{0}\Scripts\?.dll", baseDir).Replace("\\", "/");
            luaFileName = String.Format(@"{0}\Scripts\SparkParser.lua", baseDir);
            InitLua();
            InitLeg();
        }

        public void ParseSourceCode(IGame game, ISourceCode sourceCode)
        {
            game.Sources.Add(sourceCode);
            parseString.Call(game, sourceCode);
        }

        public IGame ParseSourceCode(ISourceCode sourceCode)
        {
            IGame game = new Game();
            ParseSourceCode(game, sourceCode);
            game.ReapPotentialStaticFields();
            return game;
        }
         
        private void InitLua()
        {
            lua = new Lua();

            LuaTable package = lua.GetTable("package");
            package["path"] = luaPath;
            package["cpath"] = luaCPath;

            LuaTable luanet = lua.GetTable("luanet");
            LuaTable global = lua.GetTable("_G");
            global["Ns2Docs"] = luanet["Ns2Docs"];
            global["System"] = luanet["System"];
              
            BindMethods();
            lua.DoFile(luaFileName);
            parseString = lua.GetFunction("ParseString");
            if (parseString == null)
            {
                throw new Exception("ParseString not found");
            }
        }

        private void BindMethods()
        { 
            RegisterMethod("CreatingObject", "LuaCreatingObject");
        } 

        private void LuaCreatingObject(string name)
        {
            CreatingObject(name);
        }
        

        protected void RegisterMethod(string luaName, string methodName)
        {
            RegisterMethod(luaName, this, methodName);
        }

        public void RegisterMethod(string luaName, object obj, string methodName)
        {
            MethodInfo method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null)
            {
                throw new Exception("Method not found " + methodName);
            }

            RegisterMethod(luaName, obj, method);
        }

        public void RegisterMethod(string luaName, object obj, MethodInfo method)
        {
            lua.RegisterFunction(luaName, obj, method);
        }

        private Function MakeFunction(string name)
        {
            return new Function(name);
        }
        


        private void InitLeg()
        {
            LuaFunction makeRules = lua.GetFunction("MakeRules");
            LuaTable rules = makeRules.Call()[0] as LuaTable;

            LuaFunction makeCaptures = lua.GetFunction("MakeCaptures");
            LuaTable captures = makeCaptures.Call()[0] as LuaTable;

            LuaFunction makePattern = lua.GetFunction("MakePattern");
            makePattern.Call(rules, captures);
        }
    }
}
