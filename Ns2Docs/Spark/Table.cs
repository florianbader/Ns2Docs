using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ns2Docs.Spark
{
    public interface ITable : ISparkObject
    {
        IList<IStaticFunction> StaticFunctions { get; }
        IList<IMethod> Methods { get; }
        ISet<IField> Fields { get; }
        IList<IStaticField> StaticFields { get; }
        ITable BaseTable { get; set; }
        IList<ITable> Children { get; }
        IList<ITable> Mixins { get; }
        bool CanInstantiate { get; set; }

        IField FindFieldWithName(string name);

        IEnumerable<ITable> AllMixins();
        IEnumerable<ITable> InheritanceChain();
        IEnumerable<IMethod> AllMethods(bool includeInherited = true);
        IEnumerable<IStaticFunction> AllStaticFunctions(bool includeInherited = true);
        IEnumerable<IField> AllFields(bool includeInherited = true);
        IEnumerable<IStaticField> AllStaticFields(bool includeInherited = true);
        IEnumerable<ITableMember> AllMembers(bool includeInherited = true);
        bool HasMixin(ITable table);
    }

    public class Table : SparkObject, ITable
    {
        protected delegate IEnumerable<ITableMember> MemberType<ITableMember>(ITable table);

        #region Properties
        public IList<IStaticFunction> StaticFunctions { get; private set; }
        public IList<IMethod> Methods { get; private set; }
        public ISet<IField> Fields { get; private set; }
        public IList<IStaticField> StaticFields { get; private set; }
        private ITable baseTable;
        public ITable BaseTable 
        {
            get
            {
                return baseTable;
            }
            set
            {
                if (baseTable != value)
                {
                    if (baseTable != null)
                    {
                        baseTable.Children.Remove(this);
                    }
                    baseTable = value;
                    if (value != null)
                    {
                        baseTable.Children.Add(this);
                    }
                }
            }
        }
        public IList<ITable> Children { get; private set; }
        public IList<ITable> Mixins { get; private set; }
        public bool CanInstantiate { get; set; }
        #endregion

        public Table(string name)
            : this(name, null)
        {
        }

        public Table(string name, ITable baseTable)
            : base(name)
        {
            BaseTable = baseTable;
            StaticFunctions = new List<IStaticFunction>();
            Methods = new List<IMethod>();
            Fields = new HashSet<IField>();
            StaticFields = new List<IStaticField>();
            Children = new List<ITable>();
            Mixins = new List<ITable>();

            if (baseTable != null)
            {
                baseTable.Children.Add(this);
            }
        }

        public IField FindFieldWithName(string name)
        {
            return Fields.FirstOrDefault(field => field.Name == name);
        }

        public IEnumerable<ITable> AllMixins()
        {
            ISet<ITable> allMixins = new HashSet<ITable>();
            ITable table = this;
            while (table != null)
            {
                foreach(ITable mixin in table.Mixins)
                {
                    allMixins.Add(mixin);
                }
                table = table.BaseTable;
            }
            return allMixins;
        }

        public bool HasMixin(ITable mixin)
        {
            return AllMixins().Contains(mixin);
        }

        public IEnumerable<ITable> InheritanceChain()
        {
            Stack<ITable> chain = new Stack<ITable>();
            ITable table = this;
            while (table != null)
            {
                chain.Push(table);
                table = table.BaseTable;
            }
            return chain;
        }

        

        public IEnumerable<IMethod> AllMethods(bool includeInherited = true)
        {
            return allMembersForType(table => table.Methods, includeInherited);
        }

        public IEnumerable<IStaticFunction> AllStaticFunctions(bool includeInherited = true)
        {
            return allMembersForType(table => table.StaticFunctions, includeInherited);
        }

        public IEnumerable<IField> AllFields(bool includeInherited = true)
        {
            return allMembersForType(table => table.Fields, includeInherited);
        }

        public IEnumerable<IStaticField> AllStaticFields(bool includeInherited = true)
        {
            return allMembersForType(table => table.StaticFields, includeInherited);
        }

        public IEnumerable<ITableMember> AllMembers(bool includeInherited = true)
        {
            IList<ITableMember> allMembers = new List<ITableMember>();
            foreach (IField field in AllFields(includeInherited))
            {
                allMembers.Add(field);
            }

            foreach (IStaticField staticField in AllStaticFields(includeInherited))
            {
                allMembers.Add(staticField);
            }

            foreach (IMethod method in AllMethods(includeInherited))
            {
                allMembers.Add(method);
            }

            foreach (IStaticFunction staticFunction in AllStaticFunctions(includeInherited))
            {
                allMembers.Add(staticFunction);
            }

            return allMembers;
        }

        protected IEnumerable<T> allMembersForType<T>(MemberType<T> memberType, bool includeInherited)
            where T : ITableMember
        {
            ISet<T> members = new HashSet<T>();
            ISet<string> clientNames = new HashSet<string>();
            ISet<string> serverNames = new HashSet<string>();

            ITable table = this;
            while (table != null)
            {
                allMembersForType(memberType(table), clientNames, serverNames, members);

                if (includeInherited)
                {
                    foreach (ITable mixin in table.Mixins)
                    {
                        allMembersForType(memberType(mixin), clientNames, serverNames, members);
                    }
                }

                if (includeInherited)
                {
                    table = table.BaseTable;
                }
                else
                {
                    table = null;
                }
            }
            return members;
        }

        private void allMembersForType<T>(IEnumerable<T> members, ISet<string> clientMethodNames, ISet<string> serverMethodNames, ISet<T> allMethods)
            where T : ITableMember
        {
            foreach (T method in members)
            {
                string methodName = method.Name;
                if (methodName == "__initmixin" || methodName == "__prepareclass")
                {
                    continue;
                }

                if (method.ExistsOnClient && !clientMethodNames.Contains(methodName))
                {
                    clientMethodNames.Add(methodName);
                    allMethods.Add(method);
                }

                if (method.ExistsOnServer && !serverMethodNames.Contains(methodName))
                {
                    serverMethodNames.Add(methodName);
                    allMethods.Add(method);
                }
            }
        }

        protected override void HandleComment(IGame game, IDictionary<string, IList<string>> tags)
        {
            if (tags.ContainsKey("inherits"))
            {
                string inheritsName = tags["inherits"].First();
                inheritsName = Regex.Split(inheritsName, @"\s+")[0];
                ITable inherits = game.FindTableWithName(inheritsName);
                if (inherits == null)
                {
                    inherits = new Table(inheritsName);
                    inherits.CanInstantiate = true;
                    game.Tables.Add(inherits);
                }
                BaseTable = inherits;
                
            }
            base.HandleComment(game, tags);
        }
    }
}
