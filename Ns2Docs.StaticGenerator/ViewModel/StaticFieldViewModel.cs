using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public interface IStaticFieldViewModel
    {

    }

    

    public class StaticFieldViewModel : VariableDrop, IStaticFieldViewModel
    {
        public string Template { get { return "staticfield"; } }
        private TableMemberDrop tableMemberDrop;

        public IStaticField StaticField { get; set; }
        public ITable Table { get; set; }
        public string HrefAnchor
        {
            get
            {
                return String.Format("static-field-{0}-{1}", StaticField.Name, StaticField.Library);
            }
        }

        protected override void BuildContainerCssClasses(ICollection<string> classes)
        {
            base.BuildContainerCssClasses(classes);
            tableMemberDrop.BuildContainerCssClasses(classes);
            classes.Add("static-field");
        }


        protected override void BuildEmblems(ICollection<Emblem> emblems)
        {
            base.BuildEmblems(emblems);
            tableMemberDrop.BuildEmblems(emblems);
        }

        public string QualifiedName { get { return StaticField.QualifiedName; } }        
        public string TableName { get { return Table.Name; } }

        protected override string BuildUrl()
        {
            var args = new Dictionary<string, object>();
            args["table"] = Table.Name;
            return UrlConfig.ResolveUrl("table-detail", args);
        }

        public StaticFieldViewModel(ITable table, IStaticField staticField)
            : base(staticField)
        {
            StaticField = staticField;
            Table = table;
            tableMemberDrop = new TableMemberDrop(table, staticField);
        }
    }
}
