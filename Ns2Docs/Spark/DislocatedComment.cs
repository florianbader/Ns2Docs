using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ns2Docs.Spark
{
    public class DislocatedComment
    {
        public DislocatedComment(IGame game, ISourceCode source, string comment, Library library, int offset)
        {
            int typeEnd = comment.IndexOf(' ');
            if (typeEnd == -1)
            {
                throw new Exception("No type");
            }
            string type = comment.Substring(0, typeEnd);
            FilePosition position = source.GetFilePosition(offset);
            comment = comment.Substring(typeEnd + 1).TrimStart();

            ISparkObject obj = null;
            switch (type)
            {
                case "variable":
                    break;
                case "function":
                    break;
                case "userdata":
                case "table":
                case "class":
                    {
                        int tableNameEnd = comment.IndexOf('\n');
                        if (tableNameEnd == -1)
                        {
                            tableNameEnd = comment.Length;
                        }
                        string tableName = comment.Substring(0, tableNameEnd);
                        ITable table = game.FindTableWithName(tableName);
                        if (table == null)
                        {
                            table = new Table(tableName);
                            game.Tables.Add(table);
                        }

                        if (tableNameEnd < comment.Length)
                        {
                            comment = comment.Substring(tableNameEnd + 1).TrimStart();
                        }
                        else
                        {
                            comment = String.Empty;
                        }
                        obj = table;
                        break;
                    }

                case "field":
                    {
                        int tableNameEnd = comment.IndexOf('.');
                        if (tableNameEnd == -1)
                        {
                            tableNameEnd = comment.Length;
                        }
                        string tableName = comment.Substring(0, tableNameEnd);
                        ITable table = game.FindTableWithName(tableName);
                        if (table == null)
                        {
                            table = new Table(tableName);
                            game.Tables.Add(table);
                        }

                        if (tableNameEnd < comment.Length)
                        {
                            comment = comment.Substring(tableNameEnd + 1).TrimStart();
                        }
                        else
                        {
                            comment = String.Empty;
                        }

                        int fieldNameEnd = comment.IndexOf("\n");
                        if (fieldNameEnd == -1)
                        {
                            fieldNameEnd = comment.Length;
                        }
                        string fieldName = comment.Substring(0, fieldNameEnd);

                        IField field = table.Fields.FirstOrDefault(x => x.Name == fieldName);
                        if (field == null)
                        {
                            field = new Field(table, fieldName);
                            table.Fields.Add(field);
                        }
                        if (fieldNameEnd < comment.Length)
                        {
                            comment = comment.Substring(fieldNameEnd + 1).TrimStart();
                        }
                        else
                        {
                            comment = String.Empty;
                        }
                        obj = field;
                        break;
                    }
                case "staticfield":
                    {
                        int tableNameEnd = comment.IndexOf('.');
                        if (tableNameEnd == -1)
                        {
                            tableNameEnd = comment.Length;
                        }
                        string tableName = comment.Substring(0, tableNameEnd);
                        ITable table = game.FindTableWithName(tableName);
                        if (table == null)
                        {
                            table = new Table(tableName);
                            game.Tables.Add(table);
                        }

                        if (tableNameEnd < comment.Length)
                        {
                            comment = comment.Substring(tableNameEnd + 1).TrimStart();
                        }
                        else
                        {
                            comment = String.Empty;
                        }

                        int fieldNameEnd = comment.IndexOf("\n");
                        if (fieldNameEnd == -1)
                        {
                            fieldNameEnd = comment.Length;
                        }
                        string fieldName = comment.Substring(0, fieldNameEnd);
                        IStaticField field = new StaticField(table, fieldName);
                        table.StaticFields.Add(field);
                        if (fieldNameEnd < comment.Length)
                        {
                            comment = comment.Substring(fieldNameEnd + 1).TrimStart();
                        }
                        else
                        {
                            comment = String.Empty;
                        }
                        obj = field;
                        break;
                    }
                case "method":
                    {
                        int tableNameEnd = comment.IndexOf(':');
                        if (tableNameEnd == -1)
                        {
                            tableNameEnd = comment.Length;
                        }
                        string tableName = comment.Substring(0, tableNameEnd);
                        ITable table = game.FindTableWithName(tableName);
                        if (table == null)
                        {
                            table = new Table(tableName);
                            game.Tables.Add(table);
                        }

                        int methodNameEnd = comment.IndexOf("\n");
                        if (methodNameEnd == -1)
                        {
                            methodNameEnd = comment.Length;
                        }
                        string methodName = comment.Substring(tableNameEnd + 1, methodNameEnd - tableNameEnd - 1);

                        if (methodNameEnd < comment.Length)
                        {
                            comment = comment.Substring(methodNameEnd + 1).TrimStart();
                        }
                        else
                        {
                            comment = String.Empty;
                        }

                        IMethod method = table.Methods.FirstOrDefault(x => x.Name == methodName);
                        if (method == null)
                        {
                            method = new Method(table, methodName);
                            table.Methods.Add(method);
                        }
                        obj = method;
                        break;
                    }
                case "staticfunction":
                    {
                        int tableNameEnd = comment.IndexOf('.');
                        if (tableNameEnd == -1)
                        {
                            tableNameEnd = comment.Length;
                        }
                        string tableName = comment.Substring(0, tableNameEnd);
                        ITable table = game.FindTableWithName(tableName);
                        if (table == null)
                        {
                            table = new Table(tableName);
                            game.Tables.Add(table);
                        }

                        int methodNameEnd = comment.IndexOf("\n");
                        if (methodNameEnd == -1)
                        {
                            methodNameEnd = comment.Length;
                        }
                        string methodName = comment.Substring(tableNameEnd + 1, methodNameEnd - tableNameEnd - 1);

                        if (methodNameEnd < comment.Length)
                        {
                            comment = comment.Substring(methodNameEnd + 1).TrimStart();
                        }
                        else
                        {
                            comment = String.Empty;
                        }

                        
                        IStaticFunction field = table.StaticFunctions.FirstOrDefault(x => x.Name == methodName);
                        if (field == null)
                        {
                            field = new StaticFunction(table, methodName);
                            table.StaticFunctions.Add(field);
                        }

                        obj = field;
                        break;
                    }
            }

            if (obj != null)
            {
                if (obj.DeclaredIn == null)
                {
                    obj.DeclaredIn = source;
                    obj.Line = position.Line;
                    obj.Column = position.Column;
                }
                obj.ParseComment(game, comment);
            }
        }
    }
}
