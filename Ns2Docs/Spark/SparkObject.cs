using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ns2Docs.Spark
{
    public enum Library
    {
        Shared,
        Client,
        Server
    }

    public interface ISparkObject
    {
        string Uuid { get; }
        string Name { get; }
        string QualifiedName { get; }
        string Brief { get; set; }
        string Summary { get; }

        Library Library { get; set; }
        bool ExistsOnServer { get; }
        bool ExistsOnClient { get; }

        #region File meta

        ISourceCode DeclaredIn { get; set; }
        int? Line { get; set; }
        int? LineEnd { get; set; }
        int? Column { get; set; }
        int? ColumnEnd { get; set; }
        int? Offset { get; set; }
        int? OffsetEnd { get; set; }

        #endregion
        
        
        bool IsDeprecated { get; }
        string ReasonForDeprecation { get; set; }

        IList<IAuthor> Authors { get; }
        ISet<string> Tags { get; }
        string Version { get; set; }
        ISet<string> SeeAlso { get; }
        IList<IExample> Examples { get; }

        void ParseComment(IGame game, string comment);
    }

    public abstract class SparkObject : ISparkObject
    {
        
        private const string DefaultTag = "brief";

        private readonly string uuid = Guid.NewGuid().ToString().Replace("-", "");
        public string Uuid { get { return uuid; } }

        public string Name { get; private set; }
        public string Brief { get; set; }
        public Library Library { get; set; }
        public bool IsDeprecated { get { return !String.IsNullOrEmpty(ReasonForDeprecation); } }
        public string ReasonForDeprecation { get; set; }
        public IList<IAuthor> Authors { get; private set; }
        public ISet<string> Tags { get; private set; }
        public string Version { get; set; }
        public ISet<string> SeeAlso { get; private set; }
        public IList<IExample> Examples { get; private set; }

        public string Summary
        {
            get
            {
                string summary = null;
                if (!String.IsNullOrWhiteSpace(Brief))
                {
                    int firstNewLine = Brief.IndexOf('\n');
                    if (firstNewLine >= 0)
                    {
                        summary = Brief.Substring(0, firstNewLine);
                    }
                    else
                    {
                        summary = Brief;
                    }
                }
                return summary;
            }

        }

        public int? Line { get; set; }
        public int? LineEnd { get; set; }
        public int? Column { get; set; }
        public int? ColumnEnd { get; set; }
        public int? Offset { get; set; }
        public int? OffsetEnd { get; set; }
        public ISourceCode DeclaredIn
        {
            get; set;
        }
        public bool ExistsOnServer
        {
            get { return Library != Library.Client; }
        }

        public bool ExistsOnClient
        {
            get { return Library != Library.Server; }
        }

        public SparkObject(string name)
        {
            Name = name;
            Library = Library.Shared;
            Authors = new List<IAuthor>();
            Tags = new HashSet<string>();
            SeeAlso = new HashSet<string>();
            Examples = new List<IExample>();
        }

        public override string ToString()
        {
            return Name != null ? Name : base.ToString();
        }

        public string QualifiedName { get { return Name; } }

        public void ParseComment(IGame game, string comment)
        {
            int firstTag = comment.IndexOf("\n@");

            if (!comment.StartsWith("@"))
            {
                comment = String.Format("\n@{0} {1}", DefaultTag, comment);
            }
            else
            {
                comment = "\n" + comment;
            }
            IDictionary<string, IList<string>> tags = new Dictionary<string, IList<string>>();
            string[] tagDeclarations = Regex.Split(comment, "\n@");
            for (int i = 1; i < tagDeclarations.Length; i++ )
            {
                string tagDeclaration = tagDeclarations[i];
                
                int tagNameEnd = tagDeclaration.IndexOf(" ");
                string content = null;
                string tagName = null;
                if (tagNameEnd > 0)
                {
                    tagName = tagDeclaration.Substring(0, tagNameEnd);
                    content = tagDeclaration.Substring(tagNameEnd + 1).Trim();
                }
                else
                {
                    tagName = tagDeclaration;
                }

                if (!tags.ContainsKey(tagName))
                {
                    tags[tagName] = new List<string>();
                }
                tags[tagName].Add(content);
            }
            HandleComment(game, tags);

        }

        protected virtual void HandleComment(IGame game, IDictionary<string, IList<string>> tags)
        {
            if (tags.ContainsKey("brief"))
            {
                if (String.IsNullOrEmpty(Brief))
                {
                    Brief = tags["brief"].First();
                }
            }

            if (tags.ContainsKey("author"))
            {
                foreach (string authorStr in tags["author"])
                {
                    int authorNameEnd = authorStr.IndexOf(":");
                    string authorName = null;
                    string contribution = null;
                    if (authorNameEnd >= 0)
                    {
                        authorName = authorStr.Substring(0, authorNameEnd);
                        if (authorNameEnd + 1 < authorStr.Length)
                        {
                            contribution = authorStr.Substring(authorNameEnd + 1).Trim();
                        }
                    }
                    else
                    {
                        authorName = authorStr;
                    }
                    authorName = authorName.Trim();
                    IAuthor author = new Author(authorName, contribution);
                    Authors.Add(author);
                }
            }

            if (tags.ContainsKey("deprecated"))
            {
                ReasonForDeprecation = tags["deprecated"].First();
            }

            if (tags.ContainsKey("shared"))
            {
                Library = Library.Shared;
            }
            else if (tags.ContainsKey("serveronly"))
            {
                Library = Library.Server;
            }
            else if (tags.ContainsKey("clientonly"))
            {
                Library = Library.Client;
            }

            if (tags.ContainsKey("tag"))
            {
                foreach (string tagName in tags["tag"])
                {
                    Tags.Add(tagName);
                }
            }

            if (tags.ContainsKey("version"))
            {
                Version = tags["version"].First();
            }

            if (tags.ContainsKey("seealso"))
            {
                foreach (string see in tags["seealso"])
                {
                    SeeAlso.Add(see);
                }
            }

            if (tags.ContainsKey("example"))
            {
                foreach (string exampleStr in tags["example"])
                {
                    if (!exampleStr.Contains("\n"))
                    {
                        throw new Exception(String.Format("Example doens't contain a title: '{0}'", exampleStr));
                    }
                    string title = exampleStr.Substring(0, exampleStr.IndexOf("\n")).Trim();
                    string sample = exampleStr.Substring(exampleStr.IndexOf("\n")).Trim();

                    IExample example = new Example();
                    example.Title = title;
                    example.Sample = sample;

                    Examples.Add(example);
                }
            }
        }
    }
}
