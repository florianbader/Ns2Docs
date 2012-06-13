using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using Ns2Docs.Spark;
using System.Security.Cryptography;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class VariableDrop : SparkObjectDrop
    {
        private IVariable variable;

        public string Datatype { get { return variable.Datatype; } }

        protected override void BuildEmblems(ICollection<Emblem> emblems)
        {
            base.BuildEmblems(emblems);

            if (variable.IsConstant)
            {
                emblems.Add(new Emblem("constant", "Constant"));
            }
        }

        public IEnumerable<VariableReferenceViewModel> References
        {
            get
            {
                List<VariableReferenceViewModel> viewModels = new List<VariableReferenceViewModel>();
                foreach (VariableReference reference in variable.References)
                {
                    viewModels.Add(new VariableReferenceViewModel(reference));
                }
                return viewModels;
            }
        }


        public VariableDrop(IVariable variable)
            : base(variable)
        {
            this.variable = variable;
        }
    }

    public class VariableViewModel : VariableDrop
    {
        public string Template { get { return "global-variable"; } }

        private IVariable variable;

        public VariableViewModel(IVariable variable)
            : base(variable)
        {
            this.variable = variable;
        }

        public string HrefAnchor
        {
            get
            {
                string rawAnchor = String.Format("var-{0}-{1}-{2}", variable.DeclaredIn.RelativeName, variable.Name, variable.Library);
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
