using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace Ns2Docs.Spark
{
    public interface IPotentialStaticField
    {
        string TableName { get; }
        string FieldName { get; }
        string Assignment { get; }

        void Reaped(ITable table);
    }

    public class PotentialStaticField : IPotentialStaticField
    {
        public string TableName { get; private set; }
        public string FieldName { get; private set; }
        public string Assignment { get; private set; }
        public LuaFunction OnReaped { get; set; }

        public PotentialStaticField(string tableName, string fieldName, string assignment)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException("tableName");
            }
            if (assignment == null)
            {
                throw new ArgumentNullException("assignment");
            }
            TableName = tableName;
            FieldName = fieldName;
            Assignment = assignment;
        }

        public void Reaped(ITable table)
        {
            OnReaped.Call(table);
        }
    }
}
