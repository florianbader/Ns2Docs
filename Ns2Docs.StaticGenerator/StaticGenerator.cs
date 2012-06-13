using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotLiquid;
using DotLiquid.FileSystems;
using DotLiquid.NamingConventions;
using Ns2Docs.Generator.Static.Tags;
using Ns2Docs.Generator.Static.ViewModel;
using Ns2Docs.Spark;

namespace Ns2Docs.Generator.Static
{
    public class StaticGenerator : OutputGenerator
    {
        public override string Name { get { return "Static Generator"; } }
        public override string Version { get { return "1"; } }
        public string StaticContent
        {
            get
            {
                return Utils.PathInExecutingDir("StaticContent");
            }
        }
        
        public override void Generate(IGame game)
        {
            if (String.IsNullOrWhiteSpace(Out))
            {
                Out = "ns2docs";
            }
            CopyStaticFiles();
         
            Template.NamingConvention = new CSharpNamingConvention();
            Template.RegisterFilter(typeof(StandardFilters));
            Template.RegisterFilter(typeof(Filters));
            Template.RegisterTag<UrlTag>("url");

            string templatesDir = Utils.PathInExecutingDir("Templates");
            
            Template.FileSystem = new CustomLocalFileSystem(templatesDir);
            RenderTableDetail(game);
            RenderSourceCodeDetail(game);

            RenderLists(game);
        }

        private void CopyStaticFiles()
        {
            string[] files = Directory.GetFiles(StaticContent, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string outName = Path.Combine(Out + "/", file.Substring((StaticContent + "/").Length));
                string dir = Path.GetDirectoryName(outName);
                Directory.CreateDirectory(dir);
                File.Copy(file, outName, true);
                Console.WriteLine(String.Format("Copied '{0}'", file));
            }
        }

        private void RenderTableDetail(IGame game)
        {
            foreach (var baseTable in game.BaseTables().OrderBy(x => x.Name))
            {
                RenderTable(baseTable);
            }
        }
        private void RenderLists(IGame game)
        {
            GlobalsViewModel globalsViewModel = new GlobalsViewModel(game);
            Console.WriteLine(String.Format("Rendering '{0}'...", globalsViewModel.Url.Substring(1)));
            RenderTemplate("Templates/globals-list.html", globalsViewModel, globalsViewModel.Url);

            TableListViewModel tableTreeViewModel = new TableListViewModel(game);
            Console.WriteLine(String.Format("Rendering '{0}'...", tableTreeViewModel.Url.Substring(1)));
            RenderTemplate("Templates/table-list.html", tableTreeViewModel, tableTreeViewModel.Url);

            TableListAlphabeticalViewModel tableAlphabeticalViewModel = new TableListAlphabeticalViewModel(game.Tables);
            Console.WriteLine(String.Format("Rendering '{0}'...", tableAlphabeticalViewModel.Url.Substring(1)));
            RenderTemplate("Templates/table-list-alphabetical.html", tableAlphabeticalViewModel, tableAlphabeticalViewModel.Url);

            MixinListViewModel mixinsViewModel = new MixinListViewModel(game);
            Console.WriteLine(String.Format("Rendering '{0}'...", mixinsViewModel.Url.Substring(1)));
            RenderTemplate("Templates/mixin-list.html", mixinsViewModel, mixinsViewModel.Url);

            SourceCodeListViewModel sourceCodeViewModel = new SourceCodeListViewModel(game);
            Console.WriteLine(String.Format("Rendering '{0}'...", sourceCodeViewModel.Url.Substring(1)));
            RenderTemplate("Templates/sourcecode-list.html", sourceCodeViewModel, sourceCodeViewModel.Url);
        }

        private void RenderSourceCodeDetail(IGame game)
        {
            foreach (ISourceCode source in game.Sources)
            {
                SourceCodeViewModel viewModel = new SourceCodeViewModel(source);
                RenderTemplate("Templates/sourcecode-detail.html", viewModel, viewModel.Url);
            }
        }

        private void RenderTable(ITable table)
        {
            TableViewModel tableViewModel = new TableViewModel(table);
            RenderTemplate("Templates/Table.html", tableViewModel, tableViewModel.Url);

            foreach (ITable child in table.Children)
            {
                RenderTable(child);
            }
        }

        private void RenderTemplate(string templateName, Drop viewModel, string url)
        {
            Template template = LoadTemplate(templateName);
            RenderTemplate(template, viewModel, url);
        }

        private void RenderTemplate(Template template, Drop viewModel, string url)
        {
            string renderedTemplate = template.Render(Hash.FromAnonymousObject(new { viewModel = viewModel, currentDate = DateTime.Now }));
            WriteToFile(url, renderedTemplate);
        }

        private Template LoadTemplate(string templateName)
        {
            string liquidData = File.ReadAllText(Utils.PathInExecutingDir(templateName));
            return Template.Parse(liquidData);
        }

        private void WriteToFile(string partialPath, string contents)
        {
            if (partialPath == null)
            {
                throw new ArgumentNullException("partialPath");
            }

            if (partialPath.StartsWith("/") || partialPath.StartsWith("\\"))
            {
                partialPath = partialPath.Substring(1);
            }
            
            if (String.IsNullOrWhiteSpace(partialPath))
            {
                throw new Exception("No filename");
            }

            string path = Path.Combine(Out, partialPath);
            string dir = Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);

            File.WriteAllText(path, contents);
            Console.WriteLine(String.Format("Rendered '{0}'", partialPath));
        }

        public class CustomLocalFileSystem : IFileSystem
        {
            Dictionary<string, string> fileCache;
            public string Root { get; set; }

            public CustomLocalFileSystem(string root)
            {
                fileCache = new Dictionary<string, string>();
                Root = root;
            }
            
            public string ReadTemplateFile(Context context, string templateName)
            {
                string templatePath = (string)context[templateName];
                string fullPath = FullPath(templatePath);

                string fileContents;

                if (!fileCache.ContainsKey(fullPath))
                {
                    fileContents = File.ReadAllText(fullPath);
                    fileCache[fullPath] = fileContents;
                }
                else
                {
                    fileContents = fileCache[fullPath];
                }

                return fileContents;
            }

            public string FullPath(string templatePath)
            {
                string fullPath = templatePath.Contains("/")
                    ? Path.Combine(Path.Combine(Root, Path.GetDirectoryName(templatePath)), string.Format("{0}.html", Path.GetFileName(templatePath)))
                    : Path.Combine(Root, string.Format("{0}.html", templatePath));

                return fullPath;
            }
        }
    }
}
