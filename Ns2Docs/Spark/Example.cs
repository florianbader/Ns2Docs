using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IExample
    {
        string Title { get; set; }
        string Sample { get; set; }
    }

    public class Example : IExample
    {
        public string Title { get; set; }
        public string Sample { get; set; }
    }
}
