using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using System.Security.Cryptography;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class FunctionReturnDrop : Drop
    {
        IFunctionReturn funcReturn;

        public string Brief
        {
            get { return funcReturn.Brief; }
        }

        public string When
        {
            get { return funcReturn.When; }
        }

        public IEnumerable<string> Datatypes
        {
            get { return funcReturn.Datatypes; }
        }

        public FunctionReturnDrop(IFunctionReturn funcReturn)
        {
            this.funcReturn = funcReturn;
        }
    }

    public class FunctionDrop : SparkObjectDrop
    {
        private IFunction function;

        public FunctionDrop(IFunction function)
            : base(function)
        {
            this.function = function;
        }

        public string Signature { get {
            if (function.Name.Contains("SetPlayerName"))
            {

            }
            return function.Signature; 
        } }

        public IEnumerable<FunctionReturnDrop> Returns
        {
            get
            {
                List<FunctionReturnDrop> returns = new List<FunctionReturnDrop>();
                foreach (var funcReturn in function.Returns)
                {
                    returns.Add(new FunctionReturnDrop(funcReturn));
                }
                return returns;
            }
        }
    }

    public class FunctionViewModel : FunctionDrop
    {
        public string Template { get { return "global-function"; } }

        private IFunction function;

        public FunctionViewModel(IFunction function)
            : base(function)
        {
            this.function = function;
        }

        public string HrefAnchor
        {
            get
            {
                string rawAnchor = String.Format("var-{0}-{1}-{2}", function.DeclaredIn.RelativeName, function.Signature, function.Library);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(rawAnchor));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        protected override string BuildUrl()
        {
            return UrlConfig.ResolveUrl("globals-list");
        }
    }
}
