using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;
using System.Reflection;

namespace NETSQLParser
{
    public class NETSQLParser
    {
        static StringBuilder result; // resulting XML string
        static TSqlFragment ast; // SQL abstract syntax tree
        static TextReader sqltext;
        
        public static void LoadFile(string sqlFile) 
        {
            sqltext = new StreamReader(sqlFile);
        }

        public static void LoadText(string text) 
        {
            sqltext = new StringReader(text);
        }
       
        public static bool Parse()
        {
            IList<ParseError> errors = null;
            var parser = new TSql150Parser(true, SqlEngineType.All);
            ast = parser.Parse(sqltext, out errors);

            if (errors.Count > 0)
            {
                foreach (ParseError err in errors)
                {
                    Console.WriteLine(err.Message);
                }
                return (false);
            }

            return (true);
        }

        public static string SQL2XML()
        {
            result = new StringBuilder();
            SQLTreeXML(ast, "root");
            return result.ToString();
        }

        /// <summary>
        /// method <c>ConcatMemberAttributes</c> concatenate all value type properties as xml attributes
        /// <param>fragment</param>
        /// <param>properties</param>
        ///</summary>
        private static string ConcatMemberAttributes(object fragment, PropertyInfo[] properties)
        {
            StringBuilder res = new StringBuilder();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.BaseType != null && (property.PropertyType.BaseType.Name == "ValueType" || property.PropertyType.BaseType.Name == "Enum") && property.GetValue(fragment, null) != null)
                {                   
                    res.Append(" " + property.Name + " = '" + property.GetValue(fragment, null).ToString() + "'");
                }
            }

            return res.ToString();
        }

        private static void SQLTreeXML(object fragment, string memberName)
        {
            Type fragmentType = fragment.GetType();

            // open node
            result.Append(
                "<" +
                fragmentType.Name +
                " memberName = '" + memberName + "'" +
                ConcatMemberAttributes(fragment, fragmentType.GetProperties()) +
                ">"
            );

            foreach (PropertyInfo property in fragmentType.GetProperties())
            {
                // skip empty
                if (property.GetIndexParameters().Length != 0) continue;

                // skip attributes
                if (property.PropertyType.BaseType != null && (property.PropertyType.BaseType.Name == "ValueType" || property.PropertyType.BaseType.Name == "Enum")) continue;

                // text values
                if (property.PropertyType.Name == "String")
                {
                    result.Append(property.GetValue(fragment, null));
                    continue;
                }

                // properties that are nested (lists of) fragments
                if (property.PropertyType.Name.Contains(@"IList`1"))
                {
                    if (property.Name != "ScriptTokenStream")
                    {
                        var listMembers = property.GetValue(fragment, null) as IEnumerable<object>;

                        foreach (object listItem in listMembers)
                        {
                            SQLTreeXML(listItem, property.Name);
                        }
                    }
                }
                else
                {
                    object childObj = property.GetValue(fragment, null);
                    if (childObj != null) SQLTreeXML(childObj, property.Name);
                }
            }

            // close the node
            result.AppendLine("</" + fragment.GetType().Name + ">");
        }
    }
}
