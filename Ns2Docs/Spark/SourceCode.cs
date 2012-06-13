using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Ns2Docs.Spark
{
    public class Subsection
    {
        public string Before { get; set; }
        public string Middle { get; set; }
        public string After { get; set; }

        public Subsection()
        {
            Before = "";
            Middle = "";
            After = "";
        }
    }

    public interface ISourceCode
    {
        ISanitizedPath FileName { get; set; }
        ISanitizedPath BaseDirectory { get; set; }
        Library Library { get; set; }
        string RelativeName { get; }
        int NumLines { get; }
        int FileSize { get; }
        DateTime LastWriteTime { get; set; }
        string Contents { get; set; }
        FilePosition GetFilePosition(int offset);
        Subsection CreateSubsection(int middleLineNumber, int buffer);
        string GetLine(int lineNumber);
    }

    public class SourceCode : ISourceCode
    {
        string contents;
        private IList<ISparkObject> registry = new List<ISparkObject>();
        private string[] lines;
        public ISanitizedPath FileName
        {
            get;
            set;
        }

        public DateTime LastWriteTime { get; set; }

        public Library Library { get; set; }

        public ISanitizedPath BaseDirectory
        {
            get;
            set;
        }

        public IEnumerable<ISparkObject> Registry { get { return registry; } }

        public void Register(ISparkObject obj)
        {
            if (obj.DeclaredIn != this)
            {
                obj.DeclaredIn = this;
            }
            else
            {
                registry.Add(obj);
            }
        }

        public void Unregister(ISparkObject obj)
        {
            registry.Remove(obj);
        }

        public string RelativeName
        {
            get
            {
                string relativeName = null;

                if (BaseDirectory != null && FileName != null)
                {
                    Uri fileNameUri = new Uri("file://" + FileName.Path);
                    Uri baseDirUri = new Uri("file://" + BaseDirectory.Path);
                    relativeName = Uri.UnescapeDataString(baseDirUri.MakeRelativeUri(fileNameUri).ToString());
                }

                return relativeName;
            }
        }

        public int NumLines 
        {
            get
            {
                int numLines = 0;
                if (lines != null)
                {
                    numLines = lines.Length;
                }
                return numLines;
            }
        }
        public int FileSize { get; private set; }
        public string Contents 
        {
            get { return contents; }
            set
            {
                contents = value;
                if (contents != null)
                {
                    contents = contents.Replace("\r", "");
                    FileSize = contents.Length;
                    lines = Contents.Split(new string[] { "\n" }, StringSplitOptions.None);
                }
                else
                {
                    FileSize = 0;
                    lines = new string[0];
                }
            }
        }

        public SourceCode(string contents)
        {
            Contents = contents;
        }

        public SourceCode(string fileName, string baseDirectory, string contents)
        {
            FileName = new SanitizedPath(fileName);
            BaseDirectory = new SanitizedPath(baseDirectory);
            contents = Regex.Replace(contents, "\r\n?", "\n");
            Contents = contents;
            Library = Library.Shared;
        }

        public Subsection CreateSubsection(int middleLineNumber, int buffer)
        {
            middleLineNumber -= 1;
            Subsection subsection = new Subsection();
            int startIndex = Math.Max(middleLineNumber - buffer, 0);
            int endIndex = Math.Min(middleLineNumber + buffer + 1, NumLines);

            int numSubsectionLines = endIndex - startIndex;
            
            if (numSubsectionLines > 0)
            {
                string[] subsectionArray = new string[numSubsectionLines];
                string[] before = new string[Math.Max(middleLineNumber - startIndex, 0)];
                string middle = "";
                string[] after = new string[Math.Max(endIndex - middleLineNumber - 1, 0)];


                for (int i = 0; i < before.Length; i++)
                {
                    before[i] = lines[i + startIndex];
                }

                for (int i = 0; i < after.Length; i++)
                {
                    after[i] = lines[i + middleLineNumber + 1];
                }

                subsection.Before = String.Join("\n", before);
                subsection.Middle = lines[middleLineNumber];
                subsection.After = String.Join("\n", after);
            }

            return subsection;
        }

        public FilePosition GetFilePosition(int offset)
        {
            if (offset < 0 || offset > FileSize + 1)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            int currentOffset = 0;

            for (int lineNumber=1; lineNumber <= lines.Length; lineNumber++)
            {
                string line = lines[lineNumber - 1];
                
                if (currentOffset + line.Length >= offset)
                {
                    int column = offset - currentOffset;
                    if (column == 0)
                    {

                    }
                    return new FilePosition(lineNumber, column);
                }
                
                currentOffset += line.Length + 1;
            }

            return new FilePosition(lines.Length, lines[lines.Length - 1].Length);
        }
         
        public override string ToString()
        {
            return FileName.Path;
        }

        public void PredictLibrary()
        {            
            string lowercaseFileName = FileName.Path.ToLower();

            if (lowercaseFileName.EndsWith("_client.lua"))
            {
                Library = Library.Client;
            }
            else if (lowercaseFileName.EndsWith("_server.lua"))
            {
                Library = Library.Server;
            }
            else
            {
                Library = Library.Shared;
            }

        }

        public string GetLine(int lineNumber)
        {
            int index = lineNumber - 1;
            string line = "";

            if (index >= 0 && index < lines.Length)
            {
                line = lines[index];
            }

            return line;
        }
    }
}
