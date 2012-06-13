using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

namespace Ns2Docs
{
    public class Token
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Length { get { return End - Start; } }
        public int ContentStart { get { return Start + 1; } }
        public int ContentEnd { get { return End - 1; } }
        public int ContentLength { get { return ContentEnd - ContentStart + 1; } }
        public bool WasFound { get { return End != -1; } }

        public Token(int start, int end)
        {
            Start = start;
            End = end;
        }
    }

    public class Utils
    {
        private static string executingDir;
        public static string ExecutingDir
        {
            get
            {
                if (executingDir == null)
                {
                    string codeBaseUri = Assembly.GetExecutingAssembly().CodeBase;
                    string codeBase = new Uri(codeBaseUri).AbsolutePath;
                    executingDir = Path.GetDirectoryName(codeBase);
                }
                return executingDir;
            }
        }

        public static string PathInExecutingDir(string path)
        {
            return Path.Combine(ExecutingDir, path);
        }

        

        public static Token FindToken(string str, char open, char close)
        {
            return FindToken(str, open, close, 0);
        }



        public static Token FindToken(string str, char open, char close, int index)
        {
            int openCount = 0;
            bool foundToken = false;
            int tokenStartIndex = 0;
            while (index < str.Length && (openCount != 0 || !foundToken))
            {
                char chr = str[index];
                if (chr == open)
                {
                    openCount++;
                    if (!foundToken)
                    {
                        foundToken = true;
                        tokenStartIndex = index;
                    }
                }
                else if (chr == close)
                {
                    openCount--;
                }
                index++;
            }

            int start = -1;
            int end = -1;
            if (foundToken)
            {
                if (openCount != 0)
                {
                    throw new Exception("Token wasn't closed.");
                }
                start = tokenStartIndex;
                end = index - 1;
            }
            return new Token(start, end);
        }

        public static string CleanUpComment(string comment)
        {
            string trimmed = comment;
            // remove carriage returns
            trimmed = trimmed.Replace("\r", "");
            // remove /** and /*#
            trimmed = Regex.Replace(trimmed, @"^/\*[\*#]\s*", "");
            // remove */
            trimmed = Regex.Replace(trimmed, @"\s*\*/$", "");
            string[] lines = trimmed.Split('\n');

            int leadingAsterisks = 0;
            for (int i=0; i<lines.Length; i++)
            {
                var line = lines[i];
                if (Regex.IsMatch(line, @"\s*\* ?"))
                {
                    leadingAsterisks += 1;
                }
            }

            if (leadingAsterisks == lines.Length)
            {
                // remove leading asterisks
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    line = Regex.Replace(line, @"\s*\* ?", "");
                    lines[i] = line;
                }
            }

            return String.Join("\n", lines);
        }

    }
}
