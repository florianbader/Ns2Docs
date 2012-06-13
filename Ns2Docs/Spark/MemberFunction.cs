using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public abstract class MemberFunction : Function, ITableMember
    {
        public ITable Table { get; set; }

        public bool IsPrivate { get; set; }
        public bool IsPublic
        {
            get
            {
                return !IsPrivate;
            }
            set
            {
                IsPrivate = !value;
            }
        }

        public MemberFunction(string name)
            : base(name)
        {
            if (name.StartsWith("_"))
            {
                IsPrivate = true;
            }
        }

        protected override void HandleComment(IGame game, IDictionary<string, IList<string>> tags)
        {
            base.HandleComment(game, tags);

            if (tags.ContainsKey("private"))
            {
                IsPrivate = true;
            }
            else if (tags.ContainsKey("public"))
            {
                IsPublic = true;
            }
        }
    }
}
