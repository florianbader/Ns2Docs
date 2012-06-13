using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ns2Docs.Spark
{
    public interface IAuthor
    {
        string Name { get; }
        string Contribution { get; }
    }

    public class Author : IAuthor
    {
        public string Name { get; private set; }
        public string Contribution { get; private set; }

        public Author(string name)
            : this(name, null)
        {}

        public Author(string name, string contribution)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Name = name;
            Contribution = contribution;
        }
    }
}
