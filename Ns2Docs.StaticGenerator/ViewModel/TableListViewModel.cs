using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class TableNode : Drop
    {
        private ITable table;
        public string Name { get { return table.Name; } }
        public string Summary { get { return table.Summary; } }
        public ISet<TableNode> Children { get; private set; }

        public TableNode(ITable table)
        {
            this.table = table;
            Children = new HashSet<TableNode>();
            foreach (ITable child in table.Children)
            {
                Children.Add(new TableNode(child));
            }
        }

        public IEnumerable<TableNode> ChildrenByName
        {
            get
            {
                return Children.OrderBy(x => x.Name);
            }
        }

        private TableNode BuildTableTree(ITable table, TableNode parent)
        {
            TableNode node = new TableNode(table);
            foreach (ITable child in table.Children)
            {
                node.Children.Add(BuildTableTree(child, node));
            }
            return node;
        }
    }

    public class TableListViewModel : Drop
    {
        public IGame Game { get; private set; }
        public string Name { get { return "Tables"; } }
        public IEnumerable<TableNode> TableTree
        {
            get
            {
                IList<TableNode> tableTree = new List<TableNode>();

                foreach (ITable baseTable in Game.BaseTables())
                {
                    tableTree.Add(new TableNode(baseTable));
                }
                return tableTree;
            }
        }

        public IEnumerable<TableNode> TableTreeByName
        {
            get
            {
                return TableTree.OrderBy(x => x.Name);
            }
        }

        public TableListViewModel(IGame game)
        {
            Game = game;
            
            
        }

        public string Url { 
            get 
            { 
                return UrlConfig.ResolveUrl("table-list"); 
            } 
        }

        /*private TableNode BuildTableTree(ITable table, TableNode parent)
        {
            TableNode node = new TableNode(table.Name, table.Summary);
            foreach (ITable child in table.Children)
            {
                node.Children.Add(BuildTableTree(child, node));
            }
            return node;
        }*/
    }

    
}
