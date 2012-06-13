using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class GlobalsViewModel : Drop
    {
        IGame game;

        public GlobalsViewModel(IGame game)
        {
            this.game = game;
        }

        public string Name
        {
            get { return "Globals"; }
        }

        public string Url
        {
            get { return UrlConfig.ResolveUrl("globals-list"); }
        }

        public IEnumerable<object> Members
        {
            get
            {
                List<SparkObjectDrop> l = new List<SparkObjectDrop>();
                foreach (var function in game.Functions)
                {
                    l.Add(new FunctionViewModel(function));
                }

                foreach (var variable in game.Variables)
                {
                    l.Add(new VariableViewModel(variable));
                }
                return l.OrderBy(x => x.Name);
            }
        }
    }
}
