using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ns2Docs.Spark
{
    public interface IFunction : ISparkObject
    {
        IList<IParameter> Parameters { get; }
        string Signature { get; }
        IList<IFunctionReturn> Returns { get; }
        IParameter GetOrCreateParameter(string name);
    }

    public class Function : SparkObject, IFunction
    {
        public IList<IParameter> Parameters { get; private set; }
        public IList<IFunctionReturn> Returns { get; private set; }
        
        public string Signature
        {
            get
            {
                IList<string> parameterNames = new List<string>();
                foreach (IParameter parameter in Parameters)
                {
                    string parameterStr = parameter.Name;
                    
                    if (parameter.Datatype != null)
                    {
                        parameterStr = String.Format("{0}:{1}", parameter.Name, parameter.Datatype);
                    }
                    if (parameter.IsOptional)
                    {
                        parameterStr = "[" + parameterStr + "]";
                    }
                    parameterNames.Add(parameterStr);
                }
                string parametersStr = String.Join(", ", parameterNames);

                return String.Format("{0}({1})", Name, parametersStr);
            }
        } 
         
        public Function(string name)
            : base(name)
        {
            Parameters = new List<IParameter>();
            Returns = new List<IFunctionReturn>();
        }

        public IParameter GetOrCreateParameter(string name)
        {

            IParameter parameter = Parameters.FirstOrDefault(x => x.Name == name);
            if (parameter == null)
            {
                parameter = new Parameter(name);
                Parameters.Add(parameter);
            }
            return parameter;
        }

        protected override void HandleComment(IGame game, IDictionary<string, IList<string>> tags)
        {
            base.HandleComment(game, tags);
            if (tags.ContainsKey("param"))
            {
                string[] p = tags["param"].ToArray();
                for (int i=0; i<p.Length; i++)
                {
                    string paramStr = tags["param"][i];

                    string paramName = null;
                    var match = Regex.Match(paramStr, @"(\w+)");
                    paramName = match.Groups[1].Value;
                    paramStr = paramStr.Substring(paramName.Length).TrimStart();

                    IParameter parameter = GetOrCreateParameter(paramName);
                    parameter.Brief = paramStr;
                }
            }
            if (tags.ContainsKey("return"))
            {
                foreach (string returnStr in tags["return"])
                {
                    string when = null;
                    IEnumerable<string> datatypes = null;
                    string description = null;
                    string unparsed = returnStr;

                    if (unparsed.StartsWith("when("))
                    {
                        Token whenToken = Utils.FindToken(unparsed, '(', ')');
                        if (whenToken.WasFound)
                        {
                            when = unparsed.Substring(whenToken.ContentStart, whenToken.ContentLength);
                            unparsed = unparsed.Substring(whenToken.End + 1).TrimStart();
                        }
                    }

                    if (unparsed.StartsWith("<"))
                    {
                        Token datatypesToken = Utils.FindToken(unparsed, '<', '>');
                        if (datatypesToken.WasFound)
                        {
                            datatypes = Regex.Split(unparsed.Substring(datatypesToken.ContentStart, datatypesToken.ContentLength), @"\s*,\s*");
                            unparsed = unparsed.Substring(datatypesToken.End + 1).TrimStart();
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(unparsed))
                    {
                        description = unparsed;
                    }

                    Returns.Add(new FunctionReturn(when, datatypes, description));
                }
            }

        }
    }
}
