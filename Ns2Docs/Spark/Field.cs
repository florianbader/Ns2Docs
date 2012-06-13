using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IField : IVariable, ITableMember
    {
        bool IsNetworkVar { get; set; }
    }

    public class Field : Variable, IField
    {
        public Field(ITable table, string name)
            : base(name)
        {
            Table = table;
        }

        public bool IsPrivate { get { return false; } set {} }
        public bool IsPublic { get { return false; } set { } }
        public bool IsNetworkVar { get; set; }
        public ITable Table { get; set; }

        protected override void HandleComment(IGame game, IDictionary<string, IList<string>> tags)
        {
            base.HandleComment(game, tags);

            if (tags.ContainsKey("networkvar"))
            {
                IsNetworkVar = true;
            }
        }
    }
}