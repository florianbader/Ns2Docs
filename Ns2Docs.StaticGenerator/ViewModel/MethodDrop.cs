using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ns2Docs.Spark;

namespace Ns2Docs.Generator.Static.ViewModel
{
    public class MethodDrop : FunctionDrop
    {
        private ITable table;
        private IMethod method;
        private TableMemberDrop tableMemberDrop;

        public MethodDrop(ITable table, IMethod method)
            : base(method)
        {
            this.table = table;
            this.method = method;
            tableMemberDrop = new TableMemberDrop(table, method);
        }

        public string QualifiedName
        {
            get
            {
                return method.QualifiedName;
            }
        }

        public bool IsOverriding
        {
            get { return method.Overrides != null; }
        }

        protected override void BuildContainerCssClasses(ICollection<string> classes)
        {
            base.BuildContainerCssClasses(classes);
            tableMemberDrop.BuildContainerCssClasses(classes);
            classes.Add("method");
        }

        public string TableName
        {
            get
            {
                return table.ToString();
            }
        }

        protected override void BuildEmblems(ICollection<Emblem> emblems)
        {
            base.BuildEmblems(emblems);
            tableMemberDrop.BuildEmblems(emblems);

            if (IsOverriding)
            {
                emblems.Add(new Emblem("overrides", "Overridding a method"));
                if (tableMemberDrop.IsOriginatingTable && Name == "Trace")
                {

                }
            }
        }
    }
}
