using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class SourceCodeLine : Drop
    {
        public int LineNumber { get; private set; }
        public string Contents { get; private set; }

        public SourceCodeLine(int lineNumber, string contents)
        {
            LineNumber = lineNumber;
            Contents = contents;
        }
    }

    public class SourceCodeViewModel : Drop
    {
        public SourceCodeViewModel(ISourceCode source)
        {
            Source = source;
        }

        public ISourceCode Source { get; set; }

        public string Url
        {
            get
            {
                var args = new Dictionary<string, object>();
                args["fileName"] = Source.RelativeName;
                return UrlConfig.ResolveUrl("sourcecode-detail", args);
            }
        }

        public string Name
        {
            get { return Source.RelativeName; }
        }

        public IEnumerable<SourceCodeLine> Lines
        {
            get
            {
                string[] lines = Source.Contents.Split('\n');
                SourceCodeLine[] sourceLines = new SourceCodeLine[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                {
                    sourceLines[i] = new SourceCodeLine(i - 1, lines[i]);
                }
                return sourceLines;
            }
        }

        public string Contents
        {
            get
            {
                return Source.Contents.Trim();
            }
        }

        public int NumLines
        {
            get 
            {
                int numLines = Contents.Count(chr => chr == '\n');
                if (!Contents.EndsWith("\n"))
                {
                    numLines++;
                }
                return numLines; 
            }
        }
    }
}
