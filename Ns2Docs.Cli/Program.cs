using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Ns2Docs.Spark;
using Ns2Docs.Spark.Parsing;
using NDesk.Options;
using System.Diagnostics;
using Ns2Docs.Cli.Properties;

namespace Ns2Docs.Cli
{
    public class Program : App
    {
        static void Main(string[] args)
        {
            App app = new Program();
            app.InitIO();
            app.Start(args);
        }

        public override void RunFromArgs(string[] args)
        {
            string generatorName = null;
            string outName = null;
            OptionSet optionSet = new OptionSet()
            {
                {"g|generator=", x => generatorName = x},
                {"o|out=", x => outName = x}
            };

            List<string> codeDirs = optionSet.Parse(args);
            
            if (String.IsNullOrWhiteSpace(generatorName))
            {
                generatorName = "Static";
            }

            if (codeDirs.Count == 0)
            {
                codeDirs.Add("%game%");
            }

            IOutputGenerator generator = CreateGenerator(generatorName);
            generator.Out = outName;

            SparkParser parser = new SparkParser();
            Game game = new Game();

            foreach (string unformattedCodeDir in codeDirs)
            {
                string codeDir = FormatFolderPath(unformattedCodeDir);

                string[] ignoreList;
                try
                {
                    ignoreList = File.ReadAllLines(Utils.PathInExecutingDir("ns2docs-ignore.txt"));
                }
                catch (FileNotFoundException)
                {
                    ignoreList = new string[0];
                }

                IEnumerable<string> files = FindFiles(codeDir, ignoreList);
                foreach (string file in files)
                {
                    string contents = ReadAllText(file);
                    contents = contents.Replace("Copyright \uFFFD", "Copyright ©");

                    SourceCode sourceCode = new SourceCode(file, codeDir, contents);
                    sourceCode.LastWriteTime = new FileInfo(file).LastWriteTime;
                    sourceCode.PredictLibrary();

                    try
                    {
                        parser.ParseSourceCode(game, sourceCode);
                        Console.WriteLine(String.Format(Resources.ParsedLuaFile, sourceCode.RelativeName));
                    }
                    catch (ParseException e)
                    {
                        var msg = String.Format(Resources.ParseError, file, e.InnerException.Message);
                        Console.WriteLine(msg);

                        if (Debugger.IsAttached)
                        {
                            throw e;
                        }
                    }

                }
            }
            game.ReapPotentialStaticFields();

            GenerateDocumentation(generator, game);
        }

        public IEnumerable<string> FindFiles(string baseDir, IEnumerable<string> ignores)
        {
            List<string> filesToParse = new List<string>();

            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(baseDir, "*.lua", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(baseDir, "*.ns2doc", SearchOption.AllDirectories));


            foreach (string path in files)
            {

                Uri fileNameUri = new Uri("file://" + path);
                Uri baseDirUri = new Uri("file://" + baseDir);
                string fileName = Uri.UnescapeDataString(baseDirUri.MakeRelativeUri(fileNameUri).ToString());
                
                bool skip = false;
                foreach (string ignore in ignores)
                {
                    if (ignore.StartsWith("~"))
                    {
                        skip = !fileName.StartsWith(ignore.Substring(1));
                    }
                    else if (fileName.StartsWith(ignore))
                    {
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                {
                    filesToParse.Add(path);
                }
            }
            return filesToParse;
        }

        public void GenerateDocumentation(IOutputGenerator generator, IGame game)
        {
            Console.WriteLine(Resources.GeneratingDocumentation, generator.Name, generator.Version);
            generator.Generate(game);
            Console.WriteLine(Resources.FinishedGeneratingDocumentation);
        }

        public IOutputGenerator CreateGenerator(string generatorName)
        {
            string generatorAssembly = String.Format("Ns2Docs.Generator.{0}.dll", generatorName);
            string exePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            string exeDir = Path.GetDirectoryName(exePath);
            string generatorPath = Path.Combine(exeDir, generatorAssembly);
            Assembly generatorPlugin = Assembly.LoadFile(generatorPath);
            Type generatorType = generatorPlugin.GetExportedTypes().First(x => x.GetInterface(typeof(IOutputGenerator).Name) != null);
            ConstructorInfo generatorConstructor = generatorType.GetConstructor(new Type[0]);

            return Activator.CreateInstance(generatorType) as IOutputGenerator;
        }
    }
}
