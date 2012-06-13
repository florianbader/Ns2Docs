using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;
using DotLiquid;
using System.Security.Cryptography;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class VariableReferenceViewModel : Drop
    {
        public IVariableReference Reference { get; set; }
        public string Assignment { get { return Reference.Assignment; } }
        public int Line { get { return Reference.Line; } }
        public string LineStr { get { return Reference.DeclaredIn.GetLine(Line); } }
        public string DeclaredIn { get { return Reference.DeclaredIn.RelativeName; } }
        public object Subsection 
        { 
            get 
            { 
                Subsection s = Reference.DeclaredIn.CreateSubsection(Reference.Line, 5);
                return Hash.FromAnonymousObject(new { Before = s.Before, Middle = s.Middle, After = s.After });
                //return ; 
            } 
        }

        public VariableReferenceViewModel(IVariableReference reference)
        {
            Reference = reference;
        }
    }

    public class FieldDrop : VariableDrop
    {
        public IField Field { get; set; }
        public ITable Table { get; set; }
        private TableMemberDrop tableMemberDrop;

        protected override void BuildEmblems(ICollection<Emblem> emblems)
        {
            base.BuildEmblems(emblems);

            tableMemberDrop.BuildEmblems(emblems);

            if (Field.IsNetworkVar)
            {
                emblems.Add(new Emblem("network-var", "Network variable"));
            }
        }

        protected override void BuildContainerCssClasses(ICollection<string> classes)
        {
            base.BuildContainerCssClasses(classes);
            tableMemberDrop.BuildContainerCssClasses(classes);
            classes.Add("field");
        }

        public FieldDrop(ITable table, IField field)
            : base(field)
        {
            Field = field;
            Table = table;
            tableMemberDrop = new TableMemberDrop(table, field);
        }
    }

    public class FieldViewModel : FieldDrop
    {
        public string Template { get { return "field"; } }

        
        public string HrefAnchor
        {
            get
            {
                return String.Format("field-{0}-{1}", Field.Name, Field.Library);
            }
        }

        protected override string BuildUrl()
        {
            var args = new Dictionary<string, object>();
            args["table"] = Table.Name;
            return UrlConfig.ResolveUrl("table-detail", args);
        }

        
        public FieldViewModel(ITable table, IField field)
            : base(table, field)
        {
            
        }
    }
}
