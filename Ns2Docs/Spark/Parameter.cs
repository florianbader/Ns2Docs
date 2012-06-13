using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IParameter
    {
        string Name { get; }
        string Brief { get; set; }
        //int Index { get; }
        string Datatype { get; set; }
        bool IsOptional { get; set; }
    }

    public class Parameter : IParameter
    {
        public string Name { get; set; }
        public string Brief { get; set; }
        //public int Index { get; private set; }
        public string Datatype { get; set; }
        public bool IsOptional { get; set; }

        public Parameter(string name/*, int index*/)
        {
            Name = name;
            //Index = index;
        }

        public override string ToString()
        {
            string str = Name;
            if (str == null)
            {
                str = base.ToString();
            }
            return str;
        }
    }
}
