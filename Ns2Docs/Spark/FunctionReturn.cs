using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IFunctionReturn
    {
        IList<string> Datatypes { get; }
        string When { get; set; }
        string Brief { get; set; }
    }

    public class FunctionReturn : IFunctionReturn
    {
        public IList<string> Datatypes { get; private set; }
        public string When { get; set; }
        public string Brief { get; set; }

        public FunctionReturn(string when, IEnumerable<string> datatypes, string brief)
        {
            When = when;
            if (datatypes != null)
            {
                Datatypes = new List<string>(datatypes);
            }
            else
            {
                Datatypes = new List<string>();
            }
            Brief = brief;
        }
    }
}
