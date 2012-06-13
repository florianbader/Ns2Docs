using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IVariable : ISparkObject
    {
        bool IsConstant { get; }
        string Datatype { get; set; }
        string Assignment { get; set; }
        IList<IVariableReference> References { get; }
    }

    public class Variable : SparkObject, IVariable
    {
        public IList<IVariableReference> References { get; private set; }

        public string Datatype { get; set; }
        public string Assignment { get; set; }

        public bool IsConstant
        {
            get
            {
                bool constant = false;
                if (Name.Length >= 2)
                {
                    if (Name.StartsWith("k") && Char.IsUpper(Name[1]))
                    {
                        constant = true;
                    }
                }
                return constant;
            }
        }

        public Variable(string name)
            : base(name)
        {
            References = new List<IVariableReference>();
        }

        protected override void HandleComment(IGame game, IDictionary<string, IList<string>> tags)
        {
            base.HandleComment(game, tags);

            if (tags.ContainsKey("datatype"))
            {
                Datatype = tags["datatype"].First();
            }
        }
    }
}
