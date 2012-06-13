using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Ns2Docs.Spark;

namespace Ns2Docs.Generator.Json
{
    public abstract class SparkObjectConverter : JsonConverter
    {
        protected virtual void PopulateData(IDictionary<string, object> data, object value)
        {
            ISparkObject sparkObject = (ISparkObject)value;
            data["Uuid"] = sparkObject.Uuid;
            data["Name"] = sparkObject.Name;
            data["Library"] = sparkObject.Library.ToString();
            if (Settings.IncludeObjectMeta)
            {
                data["Authors"] = sparkObject.Authors;
                data["Brief"] = sparkObject.Brief;
                data["Column"] = sparkObject.Column;
                data["ColumnEnd"] = sparkObject.ColumnEnd;
                data["Line"] = sparkObject.Line;
                data["LineEnd"] = sparkObject.LineEnd;
                data["Offset"] = sparkObject.Offset;
                data["OffsetEnd"] = sparkObject.OffsetEnd;
                if (sparkObject.Offset != null && sparkObject.OffsetEnd != null)
                {
                    data["Code"] = sparkObject.DeclaredIn.Contents.Substring((int)sparkObject.Offset - 1, (int)sparkObject.OffsetEnd - (int)sparkObject.Offset);
                }
                if (sparkObject.DeclaredIn != null)
                {
                    data["DeclaredIn"] = sparkObject.DeclaredIn.RelativeName;
                }
                data["ReasonForDeprecation"] = sparkObject.ReasonForDeprecation;
                data["SeeAlso"] = sparkObject.SeeAlso;
                data["Tags"] = sparkObject.Tags;
                data["Version"] = sparkObject.Version;
                data["Examples"] = sparkObject.Examples;
                data["QualifiedName"] = sparkObject.QualifiedName;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            PopulateData(data, value);
            IList<string> keysToRemove = new List<string>();
            foreach (var pair in data)
            {
                if (pair.Value == null)
                {
                    keysToRemove.Add(pair.Key);
                }
            }
            foreach (string keyToRemove in keysToRemove)
            {
                data.Remove(keyToRemove);
            }
            serializer.Serialize(writer, data);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


    public class TableConverter : SparkObjectConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterface("ITable") != null;
        }

        protected override void PopulateData(IDictionary<string, object> data, object value)
        {
            base.PopulateData(data, value);

            ITable table = (ITable)value;
            if (table.BaseTable != null)
            {
                data["BaseTable"] = table.BaseTable.Uuid;
            }
            data["Mixins"] = table.Mixins.Select(x => x.Uuid);
            data["Methods"] = table.Methods;
            data["StaticFunctions"] = table.StaticFunctions;
            data["StaticFields"] = table.StaticFields;
            data["Fields"] = table.Fields;
            data["CanInstantiate"] = table.CanInstantiate;
        }
    }

    public class FunctionConverter : SparkObjectConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Function) == objectType;
        }

        protected override void PopulateData(IDictionary<string, object> data, object value)
        {
            base.PopulateData(data, value);

            IFunction method = (IFunction)value;
            if (Settings.IncludeObjectMeta)
            {
                data["Parameters"] = method.Parameters;
                data["Returns"] = method.Returns;
                data["Signature"] = method.Signature;
            }
            else
            {
                data["Parameters"] = method.Parameters.Select(x => x.Name);
            }
        }
    }

    public class MethodConverter : FunctionConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterface("IMethod") != null;
        }

        protected override void PopulateData(IDictionary<string, object> data, object value)
        {
            base.PopulateData(data, value);

            IMethod method = (IMethod)value;
            if (method.Overrides != null)
            {
                data["Overrides"] = method.Overrides.Uuid;
            }
        }
    }

    public class StaticFunctionConverter : FunctionConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterface("IStaticFunction") != null;
        }

        protected override void PopulateData(IDictionary<string, object> data, object value)
        {
            base.PopulateData(data, value);

            IStaticFunction function = (IStaticFunction)value;
            if (function.Overrides != null)
            {
                data["Overrides"] = function.Overrides.Uuid;
            }
        }
    }

    public class VariableConverter : SparkObjectConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Variable) == objectType;
        }

        protected override void PopulateData(IDictionary<string, object> data, object value)
        {
            base.PopulateData(data, value);

            IVariable variable = (IVariable)value;
            if (Settings.IncludeObjectMeta)
            {
                data["IsConstant"] = variable.IsConstant;
                data["Datatype"] = variable.Datatype;
                data["Assignment"] = variable.Assignment;
                data["References"] = variable.References;
            }
        }
    }

    public class FieldConverter : VariableConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterface("IField") != null;
        }

        protected override void PopulateData(IDictionary<string, object> data, object value)
        {
            base.PopulateData(data, value);

            IField field = (IField)value;
            if (Settings.IncludeObjectMeta)
            {
                data["IsNetworkVar"] = field.IsNetworkVar;
            }
        }
    }

    public class StaticFieldConverter : VariableConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterface("IStaticField") != null;
        }
    }

    public class VariableReferenceConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return typeof(VariableReference) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            VariableReference reference = (VariableReference)value;
            serializer.Serialize(writer, new
            {
                Line = reference.Line,
                Column = reference.Column,
                DeclaredIn = reference.DeclaredIn.RelativeName,
                Assignment = reference.Assignment
            });
        }
    }
}
