using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using Ns2Docs.Cli.Properties;

namespace Ns2Docs.Cli
{
    public delegate byte[] ReadAllBytesDelegate(string path);
    public delegate string ReadAllTextDelegate(string path);
    public delegate void WriteAllBytesDelegate(string path, byte[] bytes);
    public delegate void WriteAllTextDelegate(string path, string text);
    public delegate bool FileExistsDelegate(string path);
    public delegate void CopyFileDelegate(string pathFrom, string pathTo);
    public delegate string AppDataPathDelegate();
    public delegate void CreateDirectoryDelegate(string directory);

    public abstract class App
    {
        public ReadAllBytesDelegate ReadAllBytes { get; set; }
        public WriteAllBytesDelegate WriteAllBytes { get; set; }
        public ReadAllTextDelegate ReadAllText { get; set; }
        public WriteAllTextDelegate WriteAllText { get; set; }
        public FileExistsDelegate FileExists { get; set; }
        public CopyFileDelegate CopyFile { get; set; }
        public CreateDirectoryDelegate CreateDirectory { get; set; }

        private readonly string steamKey = @"Software\Valve\Steam";
        private readonly string relNs2Path = @"steamapps\common\natural selection 2";
        private readonly RegistryKey steamHive = Registry.CurrentUser;
        public const string SteamPathKey = "SteamPath";

        public Func<string> GetAppDataPath { get; set; }

        public void Start(string[] args)
        {
            if (Debugger.IsAttached)
            {
                RunFromArgs(args);
            }
            else
            {
                try
                {
                    RunFromArgs(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        public abstract void RunFromArgs(string[] args);
        public virtual void InitIO()
        {
            InitReadingIO();
            InitWritingIO();
        }

        public virtual void InitReadingIO()
        {
            GetAppDataPath = delegate() { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); };
            ReadAllBytes = delegate(string path) { return File.ReadAllBytes(path); };
            ReadAllText = delegate(string path) { return File.ReadAllText(path); };
            FileExists = delegate(string path) { return File.Exists(path); };
        }

        public virtual void InitWritingIO()
        {
            WriteAllBytes = delegate(string path, byte[] bytes) { File.WriteAllBytes(path, bytes); };
            WriteAllText = delegate(string path, string text) { File.WriteAllText(path, text); };
            CopyFile = delegate(string pathFrom, string pathTo) { File.Copy(pathFrom, pathTo); };
            CreateDirectory = delegate(string directory)
            {
                if (!String.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            };
        }

        public string FormatFolderPath(string unformattedPath)
        {
            return FormatFolderPath(unformattedPath, null);
        }

        public string FormatFolderPath(string unformattedPath, string name)
        {
            if (unformattedPath.Contains("%game%"))
            {
                unformattedPath = unformattedPath.Replace("%game%", FindNs2Folder());
            }
            if (name != null)
            {
                unformattedPath = unformattedPath.Replace("%name%", name);
            }
            unformattedPath = unformattedPath.Replace("%appdata%", GetAppDataPath());
            unformattedPath = unformattedPath.Replace("%user%", Path.Combine(GetAppDataPath(), "Natural Selection 2"));
            return unformattedPath.Replace('/', '\\');
        }


        private string FindNs2Folder()
        {
            RegistryKey steamExeKey = steamHive.OpenSubKey(steamKey);
            if (steamExeKey == null)
            {
                throw new CommandLineException(String.Format(Resources.CouldntFindSteam, steamHive.Name, steamKey));
            }
            object steamPathObj = steamExeKey.GetValue(SteamPathKey);
            if (steamPathObj == null)
            {
                throw new CommandLineException(String.Format(Resources.CouldntFindSteamPath, steamExeKey.Name, SteamPathKey));
            }
            string steamPath = steamPathObj.ToString();
            return Path.Combine(steamPath, relNs2Path);
        }
    }
}
