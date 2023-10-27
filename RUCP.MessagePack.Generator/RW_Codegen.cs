using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Protocol.Generator;
using RUCP.MessagePack.Generator;
using RUCP.MessagePack.Generator.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Protocol.Codegen
{
    internal static class RW_Codegen
    {
        private static string IfNeedCastWrite(DeclarationType type)
        {
            return (type.IsNeedCast ? $"({type.BaseTypeName})" : "");
        }
        private static string IfNeedCastRead(DeclarationType type)
        {
            return (type.IsNeedCast ? $"({type.TypeName})" : "");
        }
        internal static string GetReadLine(string fieldName, DeclarationType type, FieldType t)
        {
            if(t == FieldType.Array)
            {
                    string readLine = FormatLine($"int {fieldName}Count = packet.ReadShort();");
                    readLine += FormatLine($"msg.{fieldName} = new {type.TypeName}[{fieldName}Count];"); 
                    readLine += FormatLine($"for(int i=0; i<{fieldName}Count; i++)");
                    readLine += FormatLine("{");
                    readLine += FormatLine($"  msg.{fieldName}[i] = {IfNeedCastRead(type)}packet.{type.ReadMethod}();");
                    readLine += FormatLine("}");
                    return readLine;
            }
            else if (t == FieldType.List)
            {
                string readLine = FormatLine($"int {fieldName}Count = packet.ReadShort();");
                readLine += FormatLine($"msg.{fieldName} = new List<{type.TypeName}>({fieldName}Count);");
                readLine += FormatLine($"for(int i=0; i<{fieldName}Count; i++)");
                readLine += FormatLine("{");
                readLine += FormatLine($"  msg.{fieldName}.Add({IfNeedCastRead(type)}packet.{type.ReadMethod}());");
                readLine += FormatLine("}");
                return readLine;
            }
            else
            {
                return FormatLine($"msg.{fieldName} = {IfNeedCastRead(type)}packet.{type.ReadMethod}();");
            }
        }

        internal static string GetWriteLine(string fieldName, DeclarationType type, FieldType t)
        {
            if (t== FieldType.Array)
            {
                string readLine = FormatLine($"int {fieldName}Count = msg.{fieldName}?.Length ?? 0;");
                readLine += FormatLine($"packet.WriteShort((short){fieldName}Count);");
                readLine += FormatLine($"for(int i=0; i<{fieldName}Count; i++)");
                readLine += FormatLine("{");
                readLine += FormatLine($"  packet.{type.WriteMethod}({IfNeedCastWrite(type)}msg.{fieldName}[i]);");
                readLine += FormatLine("}");
                return readLine;
            }
            else if (t == FieldType.List)
            {
                string readLine = FormatLine($"int {fieldName}Count = msg.{fieldName}?.Count ?? 0;");
                readLine += FormatLine($"packet.WriteShort((short){fieldName}Count);");
                readLine += FormatLine($"for(int i=0; i<{fieldName}Count; i++)");
                readLine += FormatLine("{");
                readLine += FormatLine($"  packet.{type.WriteMethod}({IfNeedCastWrite(type)}msg.{fieldName}[i]);");
                readLine += FormatLine("}");
                return readLine;
            }
            else
            {
                return FormatLine($"packet.{type.WriteMethod}({IfNeedCastWrite(type)}msg.{fieldName});");
            }
        }

        internal static string InsertRWLine(string source, string fieldName, DeclarationType declarationType, FieldType type)
        {
            //Generate Read/Write lines
            string readMark = "//Read data";
            string writeMark = "//Write data";


            string readLine = RW_Codegen.GetReadLine(fieldName, declarationType, type);
            source = StringHelper.InserAfterMark(source, readLine, readMark);


            string writeLine = RW_Codegen.GetWriteLine(fieldName, declarationType, type);
            source = StringHelper.InserAfterMark(source, writeLine, writeMark);


            string nMark = "//namespaces";
            if (declarationType.IsHaveNamespace)
            {
                string namespaceLine = $"\nusing {declarationType.Namespace};";
                if(!source.Contains(namespaceLine))
                { source = StringHelper.InserAfterMark(source, namespaceLine, nMark); }
             
            }

            return source;
        }

        //private static bool GeNamespace(Dictionary<string, string> namespaces, string type, out string n)
        //{
        //    if (type.Contains("[]")) { type = type.Replace("[]", ""); }
        //    if (namespaces.ContainsKey(type))
        //    {
        //        n = namespaces[type];
        //        return true;
        //    }
        //    n = null;
        //    return false;
        //}
        internal static string FormatLine(string line)
        {
          return Environment.NewLine + new string(' ', 15) + line;
        }


    }
}
