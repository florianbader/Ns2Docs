using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class Emblem : Drop
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Emblem(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
