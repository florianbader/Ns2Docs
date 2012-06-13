using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IOPath = System.IO.Path;
using System.Text.RegularExpressions;

namespace Ns2Docs
{
    public class SanitizedPath : ISanitizedPath
    {
        private readonly string invalidCharsRegex;
        private readonly char[] invalidChars;

        public SanitizedPath()
            : this(null)
        {
        }

        public SanitizedPath(string uncleanPath)
        {
            invalidChars = IOPath.GetInvalidPathChars();
            string invalidCharsEscaped = Regex.Escape(new String(invalidChars));

            invalidCharsRegex = String.Format("[{0}]", invalidCharsEscaped);
            
            UncleanPath = uncleanPath;
        }

        private string path;
        public string Path
        {
            get
            {
                return path;
            }

            private set
            {
                if (value != null)
                {
                    if (Regex.IsMatch(value, invalidCharsRegex))
                    {
                        string message = String.Format("The path contains invalid characters");
                        throw new FormatException(message);
                    }
                    value = value.Replace("/", "\\");

                    if (value.EndsWith("\\"))
                    {
                        value = value.Remove(value.Length - 1);
                    }
                }

                path = value;
            }
        }

        private string uncleanPath;
        public string UncleanPath
        {
            get
            {
                return uncleanPath;
            }

            set
            {
                if (uncleanPath != value)
                {
                    uncleanPath = value;
                    Path = value;
                }
            }
        }

        public override string ToString()
        {
            return Path != null ? Path : base.ToString();
        }

        public string GetRelativeName(ISanitizedPath baseDirectory)
        {
            if (baseDirectory == null)
            {
                throw new ArgumentNullException("baseDirectory");
            }

            string relativeName = Path;
            string directoryPath = String.Format(@"{0}\", baseDirectory.Path);

            if (Path.StartsWith(directoryPath))
            {
                relativeName = Path.Remove(0, directoryPath.Length);
            }

            Uri a = new Uri("file://"+IOPath.Combine(baseDirectory.Path, Path).Replace(@"\", "/"));
            Uri b = new Uri("file://" + baseDirectory.Path.Replace(@"\", "/"));

            if (true) return Uri.UnescapeDataString(b.MakeRelativeUri(a).ToString());
            return IOPath.Combine(baseDirectory.Path, Path);

            
        }

        public override bool Equals(object obj)
        {
            var SanitizedPathObj = obj as SanitizedPath;

            if (SanitizedPathObj == null)
            {
                return false;
            }

            return Path == SanitizedPathObj.Path;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}
