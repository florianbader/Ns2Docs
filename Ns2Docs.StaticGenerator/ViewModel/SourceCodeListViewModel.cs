using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using System.Text.RegularExpressions;
using DotLiquid;
using System.IO;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class SourceCodeListViewModel : Drop
    {
        private IGame game;

        public string Name { get { return "Files"; } }

        public SourceCodeListViewModel(IGame game)
        {
            this.game = game;
            List<DirectoryListing> baseListings = new List<DirectoryListing>();
            foreach (ISourceCode source in game.Sources)
            {
                string dn = Path.GetDirectoryName(source.RelativeName);
                string[] dirs = Regex.Split(dn, @"[\\/]");
                DirectoryListing d = null;
                foreach (var baseListing in baseListings)
                {
                    if (baseListing.Name == dirs[0])
                    {
                        d = baseListing;
                    }
                }
                if (d == null)
                {
                    d = new DirectoryListing();
                    d.Name = dirs[0];
                    baseListings.Add(d);
                }
                d.GetOrCreateDirectoryListing(dirs).Sources.Add(source);
            }
            DirectoryListings = baseListings;
        }

        public string Url { get { return UrlConfig.ResolveUrl("sourcecode-list"); } }

        public class DirectoryListing : Drop
        {
            public string Name { get; set; }
            public List<DirectoryListing> SubDirectories { get; private set; }
            public List<ISourceCode> Sources { get; private set; }

            public DirectoryListing()
            {
                SubDirectories = new List<DirectoryListing>();
                Sources = new List<ISourceCode>();
            }

            public override string ToString()
            {
                return Name;
            }


            public DirectoryListing GetOrCreateDirectoryListing(string[] dirs)
            {
                if (dirs.Length == 1)
                {
                    if (dirs[0] == Name)
                    {
                        return this;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (dirs.Length > 1)
                {
                    return GetOrCreateDirectoryListing(dirs, 1);
                }
                
                return null;
            }

            public IEnumerable<object> Files
            {
                get
                {
                    List<object> sources = new List<object>();
                    foreach (ISourceCode source in Sources.OrderBy(x => x.RelativeName))
                    {
                        float fileSizeKB = source.FileSize / 1024.0f;
                        sources.Add(new { 
                            RelativeName = source.RelativeName,
                            Name = Path.GetFileName(source.RelativeName),
                            LastWriteTime = source.LastWriteTime, 
                            FileSize = String.Format("{0:F1}kB", fileSizeKB) });
                    }
                    return sources;
                }
            }

            public DirectoryListing GetOrCreateDirectoryListing(string[] dirs, int index)
            {
                if (index == dirs.Length)
                {
                    return this;
                }

                foreach (DirectoryListing listing in SubDirectories)
                {
                    if (listing.Name == dirs[index])
                    {
                        return listing.GetOrCreateDirectoryListing(dirs, index + 1);
                    }
                }

                var newListing = new DirectoryListing();
                newListing.Name = dirs[index];
                SubDirectories.Add(newListing);
                return newListing.GetOrCreateDirectoryListing(dirs, index + 1);
            }
        }

        public IEnumerable<DirectoryListing> DirectoryListings { get; private set; }

        public IEnumerable<object> Sources
        {
            get
            {
                List<object> sources = new List<object>();
                foreach (ISourceCode source in game.Sources.OrderBy(x => x.RelativeName))
                {
                    float fileSizeKB = source.FileSize / 1024.0f;
                    sources.Add(new { RelativeName = source.RelativeName, LastWriteTime=source.LastWriteTime, FileSize=String.Format("{0:F1}kB", fileSizeKB)});
                }
                return sources;
            }
        }
    }
}
