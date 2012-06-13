using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;


namespace Ns2Docs.Generator.Json
{
    public class JsonGenerator : OutputGenerator
    {
        public override string Name { get { return "JsonGenerator"; } }
        public override string Version { get { return "1"; } }

        public override void Generate(IGame game)
        {
            if (String.IsNullOrWhiteSpace(Out))
            {
                Out = "out.json";
            }

            Settings.IncludeObjectMeta = true;
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Converters.Add(new TableConverter());
            settings.Converters.Add(new MethodConverter());
            settings.Converters.Add(new StaticFunctionConverter());
            settings.Converters.Add(new VariableConverter());
            settings.Converters.Add(new FieldConverter());
            settings.Converters.Add(new FunctionConverter());
            settings.Converters.Add(new StaticFieldConverter());
            settings.Converters.Add(new VariableReferenceConverter());

            IDictionary<string, object> data = new Dictionary<string, object>();
            data["Tables"] = game.Tables;
            IDictionary<string, object> globals = new Dictionary<string, object>();
            data["Globals"] = globals;
            globals["Variables"] = game.Variables;
            globals["Functions"] = game.Functions;
            IDictionary<string, object> meta = new Dictionary<string, object>();
            data["Meta"] = meta;
            IDictionary<string, object> dateMeta = new Dictionary<string, object>();
            dateMeta["Utc"] = DateTime.UtcNow;
            dateMeta["SecondsSinceUnixEpoch"] = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            meta["DateCreated"] = dateMeta;
            meta["GeneratorName"] = Name;
            meta["GeneratorVersion"] = Version;

            string json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);

            string dirName = Path.GetDirectoryName(Out);
            if (!String.IsNullOrWhiteSpace(dirName)){
                Directory.CreateDirectory(dirName);
            }
            File.WriteAllText(Out, json);

            Console.WriteLine(String.Format("Wrote '{0}'", Out));
        }
    }
}
