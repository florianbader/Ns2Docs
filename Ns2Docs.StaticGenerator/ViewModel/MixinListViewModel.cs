using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class MixinListViewModel : Drop
    {
        private IGame game;

        public string Name { get { return "Mixins"; } }
        public MixinListViewModel(IGame game)
        {
            this.game = game;
        }

        public string Url
        {
            get
            {
                return UrlConfig.ResolveUrl("mixin-list");
            }
        }

        IDictionary<string, IList<string>> mixins = new Dictionary<string, IList<string>>();

        public IEnumerable<TableNode> MixinsByName
        {
            get
            {

                IEnumerable<ITable> mixins = game.Mixins().OrderBy(mixin => mixin.Name);
                IList<TableNode> m = new List<TableNode>();

                foreach (ITable mixin in mixins)
                {
                    TableNode n = new TableNode(mixin);
                    
                    foreach (ITable table in game.Tables)
                    {
                        if (table.Mixins.Contains(mixin))
                        {
                            n.Children.Add(new TableNode(table));
                        }
                    }
                    

                    m.Add(n);
                }


                return m;

            }
        }
    }

}
