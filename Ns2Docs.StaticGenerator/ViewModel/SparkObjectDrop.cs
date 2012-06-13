using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using Ns2Docs.Spark;
using System.Security.Cryptography;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class SparkObjectDrop : Drop
    {
        private ISparkObject sparkObject;

        public string Name { get { return sparkObject.Name; } }
        public string Library { get { return sparkObject.Library.ToString(); } }
        public IEnumerable<string> ContainerCssClasses 
        { 
            get 
            {
                IList<string> classes = new List<string>();
                BuildContainerCssClasses(classes);
                return classes;
            } 
        }

        public SparkObjectDrop(ISparkObject sparkObject)
        {
            this.sparkObject = sparkObject;
        }

        public string Url
        {
            get
            {
                return BuildUrl();
            }
        }

        protected virtual string BuildUrl()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Emblem> Emblems
        {
            get
            {
                IList<Emblem> emblems = new List<Emblem>();
                BuildEmblems(emblems);
                return emblems;
            }
        }

        public string DeclaredIn
        {
            get
            {
                string declaredIn = "";
                if (sparkObject.DeclaredIn != null)
                {
                    declaredIn = String.Format("{0}:{1}", sparkObject.DeclaredIn.RelativeName, sparkObject.Line);
                }
                return declaredIn;
            }
        }

        public string Brief
        {
            get { return sparkObject.Brief; }
        }

        public string DeclaredInUrl
        {
            get
            {
                if (sparkObject.DeclaredIn != null)
                {
                    var args = new Dictionary<string, object>();
                    args["fileName"] = sparkObject.DeclaredIn.RelativeName;
                    string url = UrlConfig.ResolveUrl("sourcecode-detail", Url, args);
                    return string.Format("{0}#line-{1}", url, sparkObject.Line);
                }
                return "";
            }
        }

        protected virtual void BuildContainerCssClasses(ICollection<string> classes)
        {

        }


       protected virtual void BuildEmblems(ICollection<Emblem> emblems)
       {
           if (sparkObject.IsDeprecated)
           {
               emblems.Add(new Emblem("deprecated", "Deprecated"));
           }

           if (sparkObject.Library == Ns2Docs.Spark.Library.Server)
           {
               emblems.Add(new Emblem("server", "Server-only"));
           }
           else if (sparkObject.Library == Ns2Docs.Spark.Library.Client)
           {
               emblems.Add(new Emblem("client", "Client-only"));
           }
       }
    }
}
