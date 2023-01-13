using Protocol.Generator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Codegen
{
    public static class MessageObjects
    {

        public static IEnumerable<(string name, string source)> Register(SyntaxReceiver syntaxReceiver)
        {
          foreach(FindNode findNode in syntaxReceiver.MessageObjects)
          {
                string type = findNode.StructNode.Identifier.ValueText;

                if (syntaxReceiver.writeDeclarationTypes.ContainsKey(type)) continue;

                string source = @"
//namespaces
using RUCP;

namespace $name_Space
{
    public static class $type_Extension
    {
        public static $type Read$type(this Packet packet)
        {
           $type msg = new $type();
               //Read data

               return msg;
        }
        public static void Write$type(this Packet packet, $type msg)
        {
              //Write data
        }
    }
}";

                source = source.Replace("$name_Space", $"{Helper.GetNamespace(findNode.StructNode)}");
                source = source.Replace("$type", type);

                source = RW_Codegen.InsertRWLine(source, syntaxReceiver, findNode.StructNode);

                syntaxReceiver.readDeclarationTypes.Add(type, $"Read{type}");
                syntaxReceiver.writeDeclarationTypes.Add(type, $"Write{type}");

                syntaxReceiver.namespaces.Add(type, Helper.GetNamespace(findNode.StructNode));

                yield return (name: type, source: source);
            }
        }
    }
}
