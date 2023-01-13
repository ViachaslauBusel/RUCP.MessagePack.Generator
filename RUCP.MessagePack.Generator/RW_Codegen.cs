using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Protocol.Generator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Codegen
{
    internal static class RW_Codegen
    {
        private static string IfNeedCastWrite(string realType, Dictionary<string, string> castDeclaration)
        {
            if (castDeclaration.ContainsKey(realType)) return $"({castDeclaration[realType]})";
            return "";
        }
        private static string IfNeedCastRead(string realType, Dictionary<string, string> castDeclaration)
        {
            if (castDeclaration.ContainsKey(realType)) return $"({realType})";
            return "";
        }
        internal static string GetReadLine(SyntaxReceiver syntaxReceiver, string name, string type)
        {
            var readDeclarationTypes = syntaxReceiver.readDeclarationTypes;
            var castDeclaration = syntaxReceiver.castDeclarationTypes;

            if (readDeclarationTypes.ContainsKey(type))
            {
              return FormatLine($"msg.{name} = {IfNeedCastRead(type, castDeclaration)}packet.{readDeclarationTypes[type]}();");
            }
            
            if(type.Contains("[]"))
            {
                string realType = type.Replace("[]", "");
                if(readDeclarationTypes.ContainsKey(realType))
                {
                    string readLine = FormatLine($"int count = packet.ReadShort();");
                    readLine += FormatLine($"msg.{name} = new {realType}[count];"); 
                    readLine += FormatLine($"for(int i=0; i<count; i++)");
                    readLine += FormatLine("{");
                    readLine += FormatLine($"  msg.{name}[i] = {IfNeedCastRead(realType, castDeclaration)}packet.{readDeclarationTypes[realType]}();");
                    readLine += FormatLine("}");
                    return readLine;
                }
                return "";
            }

            return "";
        }

        internal static string GetWriteLine(SyntaxReceiver syntaxReceiver, string name, string type)
        {
        var writeDeclarationTypes = syntaxReceiver.writeDeclarationTypes;
        var castDeclaration = syntaxReceiver.castDeclarationTypes;
            if (writeDeclarationTypes.ContainsKey(type))
            {
                return FormatLine($"packet.{writeDeclarationTypes[type]}({IfNeedCastWrite(type, castDeclaration)}msg.{name});");
            }

            if (type.Contains("[]"))
            {
                string realType = type.Replace("[]", "");
                if (writeDeclarationTypes.ContainsKey(realType))
                {
                    string readLine = FormatLine($"int count = msg.{name}?.Length ?? 0;");
                    readLine += FormatLine($"packet.WriteShort((short)count);");
                    readLine += FormatLine($"for(int i=0; i<count; i++)");
                    readLine += FormatLine("{");
                    readLine += FormatLine($"  packet.{writeDeclarationTypes[realType]}({IfNeedCastWrite(realType, castDeclaration)}msg.{name}[i]);");
                    readLine += FormatLine("}");
                    return readLine;
                }
                return "";
            }

            return "";
        }

        public static string InsertRWLine(string source, SyntaxReceiver syntaxReceiver, StructDeclarationSyntax structNode)
        {
            List<(string name, string type)> members = new();
            //Find propertyies
            foreach (MemberDeclarationSyntax member in structNode.Members)
            {
                if (member is PropertyDeclarationSyntax property)
                {
                    members.Add((name: property.Identifier.ValueText, type: property.Type.ToString()));
                }
                //else if (member is FieldDeclarationSyntax field)
                //{
                //    members.Add((name: field.Identifier.ValueText, type: field.Type.ToString()));
                //}
            }
            //Generate Read/Write lines
            string readMark = "//Read data";
            string writeMark = "//Write data";
            List<string> namespaces = new List<string>();
            foreach (var m in members)
            {
                if (GeNamespace(syntaxReceiver.namespaces, m.type, out string nSpace)
                && !namespaces.Contains(nSpace))
                {
                    namespaces.Add(nSpace);
                }

                int readIndex = source.IndexOf(readMark) + readMark.Length;

                string readLine = RW_Codegen.GetReadLine(syntaxReceiver, m.name, m.type);
                source = source.Insert(readIndex, readLine);


                int writeIndex = source.IndexOf(writeMark) + writeMark.Length;

                string writeLine = RW_Codegen.GetWriteLine(syntaxReceiver, m.name, m.type);
                source = source.Insert(writeIndex, writeLine);
            }

            string nMark = "//namespaces";
            foreach (string nSpace in namespaces)
            {
                int spaceIndex = source.IndexOf(nMark) + nMark.Length;
                source = source.Insert(spaceIndex, Environment.NewLine + $"using {nSpace};");
            }

            return source;
        }

        private static bool GeNamespace(Dictionary<string, string> namespaces, string type, out string n)
        {
            if (type.Contains("[]")) { type = type.Replace("[]", ""); }
            if (namespaces.ContainsKey(type))
            {
                n = namespaces[type];
                return true;
            }
            n = null;
            return false;
        }
        internal static string FormatLine(string line)
        {
          return Environment.NewLine + new string(' ', 15) + line;
        }
    }
}
