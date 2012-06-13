using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Ns2Docs
{
    public interface IGame
    {
        IList<IFunction> Functions { get; }
        IList<IVariable> Variables { get; }
        IList<ISourceCode> Sources { get; }
        IList<IPotentialStaticField> PotentialStaticFields { get; }
        IList<ITable> Tables { get; }

        IEnumerable<IVariable> FindVariablesWithName(string name);
        ITable FindTableWithName(string name);

        void ReapPotentialStaticFields();
        IEnumerable<ITable> BaseTables();
        IEnumerable<ITable> BaseClasses();
        IEnumerable<ITable> Mixins();
    }

    public class Game : IGame
    {
        public IList<IFunction> Functions { get; private set; }
        public IList<ISourceCode> Sources { get; private set; }
        public IList<IVariable> Variables { get; private set; }
        public IList<ITable> Tables { get; private set; }
        public IList<IPotentialStaticField> PotentialStaticFields { get; private set; }

        public Game()
        {
            Functions = new List<IFunction>();
            Sources = new List<ISourceCode>();
            Variables = new List<IVariable>();
            Tables = new List<ITable>();
            PotentialStaticFields = new List<IPotentialStaticField>();
        }

        public IEnumerable<ITable> BaseTables()
        {
            IList<ITable> baseTables = new List<ITable>();
            foreach (ITable table in Tables)
            {
                if (table.BaseTable == null)
                {
                    baseTables.Add(table);
                }
            }
            return baseTables;
        }

        public IEnumerable<ITable> BaseClasses()
        {
            IList<ITable> baseClasses = new List<ITable>();
            foreach (ITable table in Tables)
            {
                if (table.BaseTable == null && table.CanInstantiate)
                {
                    baseClasses.Add(table);
                }
            }
            return baseClasses;
        }

        public IEnumerable<ITable> Mixins()
        {
            IEnumerable<ITable> tablesWithMixins = Tables.Where(x => x.Mixins.Count > 0);
            ISet<ITable> mixins = new HashSet<ITable>();

            foreach (ITable table in tablesWithMixins)
            {
                foreach (ITable mixin in table.Mixins)
                {
                    mixins.Add(mixin);
                }
            }

            return mixins;
        }

        public ITable FindTableWithName(string name)
        {
            return Tables.SingleOrDefault(tbl => tbl.Name == name);
        }

        public IEnumerable<IVariable> FindVariablesWithName(string name)
        {
            return Variables.Where(var => var.Name == name);
        }

        public void ReapPotentialStaticFields()
        {
            var potentialsByTableName = PotentialStaticFields.GroupBy(potential => potential.TableName);
            foreach (var potentialsForTable in potentialsByTableName)
            {
                ITable table = null;
                foreach (IPotentialStaticField potential in potentialsForTable)
                {
                    if (table == null)
                    {
                        table = FindTableWithName(potential.TableName);
                        if (table == null)
                        {
                            continue;
                        }
                    }
                    potential.Reaped(table);
                }
            }
            PotentialStaticFields.Clear();
        }
    }  
}

