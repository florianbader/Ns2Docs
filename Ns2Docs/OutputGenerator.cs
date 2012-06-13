using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Ns2Docs
{
    public interface IOutputGenerator
    {
        string Name { get; }
        string[] Args { get; set; }
        string Version { get; }
        string Out { get; set; }
        string ExecutingDir { get; }
        void Generate(IGame game);
    }

    public abstract class OutputGenerator : IOutputGenerator, IDisposable
    {
        private string[] args;

        public abstract string Name { get; }
        public abstract string Version { get; }
        public string[] Args
        {
            get
            {
                if (args == null)
                {
                    args = new string[0];
                }
                return args;
            }
            set
            {
                args = value;
            }
        }
        
        public string ExecutingDir
        {
            get
            {
                return Utils.ExecutingDir;
            }
        }
        public string Out { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public abstract void Generate(IGame game);
    }
}
