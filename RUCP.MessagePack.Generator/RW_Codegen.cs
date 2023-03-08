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
            return (type.IsNeedCast ? $"({type.TypeName})" : "");
        }
        private static string IfNeedCastRead(DeclarationType type)
        {
            return (type.IsNeedCast ? $"({type.CastTypeName})" : "");
        }
        internal static string GetReadLine(string fieldName, DeclarationType type, bool isArray)
        {
            if(isArray)
            {
                    string readLine = FormatLine($"int count = packet.ReadShort();");
                    readLine += FormatLine($"msg.{fieldName} = new {type.TypeName}[count];"); 
                    readLine += FormatLine($"for(int i=0; i<count; i++)");
                    readLine += FormatLine("{");
                    readLine += FormatLine($"  msg.{fieldName}[i] = {IfNeedCastRead(type)}packet.{type.ReadMethod}();");
                    readLine += FormatLine("}");
                    return readLine;
            }
            else
            {
                return FormatLine($"msg.{fieldName} = {IfNeedCastRead(type)}packet.{type.ReadMethod}();");
            }
        }

        internal static string GetWriteLine(string fieldName, DeclarationType type, bool isArray)
        {
            if (isArray)
            {
                string readLine = FormatLine($"int count = msg.{fieldName}?.Length ?? 0;");
                readLine += FormatLine($"packet.WriteShort((short)count);");
                readLine += FormatLine($"for(int i=0; i<count; i++)");
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

        internal static string InsertRWLine(string source, string fieldName, DeclarationType declarationType, bool isArray)
        {
            //Generate Read/Write lines
            string readMark = "//Read data";
            string writeMark = "//Write data";


            string readLine = RW_Codegen.GetReadLine(fieldName, declarationType, isArray);
            source = StringHelper.InserAfterMark(source, readLine, readMark);


            string writeLine = RW_Codegen.GetWriteLine(fieldName, declarationType, isArray);
            source = StringHelper.InserAfterMark(source, writeLine, writeMark);


            string nMark = "//namespaces";
            if (declarationType.IsHaveNamespace)
            {
                source = StringHelper.InserAfterMark(source, declarationType.Namespace, nMark);
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
