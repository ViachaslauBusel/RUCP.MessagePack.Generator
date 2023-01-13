using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Protocol.Codegen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Protocol.Generator
{

    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {



            SyntaxReceiver syntaxReceiver = context.SyntaxContextReceiver as SyntaxReceiver;

            if (syntaxReceiver == null) { return; }

            foreach(var obj in MessageObjects.Register(syntaxReceiver))
            {
                context.AddSource($"{obj.name}_Extension.g.cs", obj.source);
            }

            foreach (FindNode findNode in syntaxReceiver.MessagePackets)
            {
                string source = @"
//namespaces
using RUCP;

namespace $name_Space
{
    public static class $name_Extension
    {
        public static void Read(this Packet packet, out $name_Struct msg)
        {
            msg = new $name_Struct();
               //Read data
        }
        public static void Write(this Packet packet, $name_Struct msg)
        {
              //Write data
        }

        public static void Send(this Client client, $name_Struct msg)
        {
            Packet packet = Packet.Create($name_Channel);
            packet.OpCode = $name_Opcode;
            packet.Write(msg);
            client.Send(packet);
        }
    }
}"; 
                source = source.Replace("$name_Space", $"{Helper.GetNamespace(findNode.StructNode)}");
                source = source.Replace("$name_Extension", $"{findNode.StructNode.Identifier.Text}_Extension");
                source = source.Replace("$name_Struct", findNode.StructNode.Identifier.Text);

                if (findNode.AttributeNode.ArgumentList.Arguments.Count == 2)
                {
                    for (int i = 0; i < findNode.AttributeNode.ArgumentList.Arguments.Count; i++)
                    {
                        AttributeArgumentSyntax arg = findNode.AttributeNode.ArgumentList.Arguments[i];
                        if(i==0)
                        {
                            if (arg.Expression is LiteralExpressionSyntax literalExpression)
                            {
                                source = source.Replace("$name_Opcode", $"{literalExpression.Token.Text}");
                            }
                            if (arg.Expression is MemberAccessExpressionSyntax memberAccessExpression
                            && arg.Parent is AttributeArgumentListSyntax att
                            && att.Arguments[0].Expression is MemberAccessExpressionSyntax parentMember)
                            {

                                source = source.Replace("$name_Opcode", $"{((IdentifierNameSyntax)memberAccessExpression.Expression).Identifier.ValueText}.{memberAccessExpression.Name.ToString()}");
                            }
                        }
                        else if(i == 1)
                        {
                            if (arg.Expression is MemberAccessExpressionSyntax memberAccessExpression)
                            {
                                source = source.Replace("$name_Channel", $"Channel.{memberAccessExpression.Name.ToString()}");
                            }
                        }
                      
                    }
                }


                source = RW_Codegen.InsertRWLine(source, syntaxReceiver, findNode.StructNode);

                

               


                context.AddSource($"{findNode.StructNode.Identifier.Text}_Extension.g.cs", source);
            }
        }

      

        public void Initialize(GeneratorInitializationContext context)
        {
            var MessagePackText = SourceText.From(@"
using System;
using RUCP;

namespace Protocol
{
     [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class MessagePackAttribute : Attribute
    {
        private short m_opcode;
        private Channel m_channel;

        public MessagePackAttribute(short opcode, Channel channel)
        {
            this.m_opcode = opcode;
            this.m_channel = channel;
        }
    }
}", Encoding.UTF8);

         var MessageObjectText = SourceText.From(@"
using System;
using RUCP;

namespace Protocol
{
     [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class MessageObjectAttribute : Attribute
    {
    }
}", Encoding.UTF8);


            // Register the attribute source
            context.RegisterForPostInitialization((i) =>
            {
                i.AddSource("MessageObjectAttribute.g.cs", MessageObjectText);
                i.AddSource("MessagePackAttribute.g.cs", MessagePackText);
            });

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    }

    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
   public class SyntaxReceiver : ISyntaxContextReceiver
    {
        public Dictionary<string, string> readDeclarationTypes = new Dictionary<string, string>()
        {
          { "byte[]", "ReadBytes" },
          { "byte", "ReadByte" },
            { "short", "ReadShort" },
            { "ushort", "ReadUshort" },
            { "int", "ReadInt" },
             { "long", "ReadLong" },
           { "float", "ReadFloat" },
            { "bool", "ReadBool" },
             { "string", "ReadString" },
        };
        public Dictionary<string, string> writeDeclarationTypes = new Dictionary<string, string>()
        {
          { "byte[]", "WriteBytes" },
          { "byte", "WriteByte" },
            { "short", "WriteShort" },
            { "ushort", "WriteUshort" },
            { "int", "WriteInt" },
             { "long", "WriteLong" },
           { "float", "WriteFloat" },
            { "bool", "WriteBool" },
             { "string", "WriteString" },
        };
        public Dictionary<string, string> castDeclarationTypes = new Dictionary<string, string>()
        {
          { "byte", "byte" }
        };
        public Dictionary<string, string> namespaces = new Dictionary<string, string>();

        public List<FindNode> MessageObjects { get; } = new List<FindNode>();
        public List<FindNode> MessagePackets { get; } = new List<FindNode>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            try
            {

                //Register enum type
                if (context.Node is EnumDeclarationSyntax enumDeclaration)
                {
                    if (!readDeclarationTypes.ContainsKey(enumDeclaration.Identifier.ValueText))
                    {
                        if (enumDeclaration.BaseList != null && enumDeclaration.BaseList.Types.Count != 0)
                        {
                            if (enumDeclaration.BaseList.Types[0].Type is PredefinedTypeSyntax baseType)
                            {
                                if (readDeclarationTypes.ContainsKey(baseType.Keyword.ValueText))
                                {
                                    readDeclarationTypes.Add(enumDeclaration.Identifier.ValueText, readDeclarationTypes[baseType.Keyword.ValueText]);
                                    writeDeclarationTypes.Add(enumDeclaration.Identifier.ValueText, writeDeclarationTypes[baseType.Keyword.ValueText]);
                                    castDeclarationTypes.Add(enumDeclaration.Identifier.ValueText, baseType.Keyword.ValueText);
                                }
                            }
                        }
                        else
                        {
                            readDeclarationTypes.Add(enumDeclaration.Identifier.ValueText, readDeclarationTypes["int"]);
                            writeDeclarationTypes.Add(enumDeclaration.Identifier.ValueText, writeDeclarationTypes["int"]);
                            castDeclarationTypes.Add(enumDeclaration.Identifier.ValueText, "int");
                        }
                    }
                    if (!namespaces.ContainsKey(enumDeclaration.Identifier.ValueText))
                    {
                        namespaces.Add(enumDeclaration.Identifier.ValueText, Helper.GetNamespace(enumDeclaration));
                    }
                }



                //Register struct type
                if (context.Node is AttributeSyntax at)
                {
                    if (at.Name.ToString().Equals("MessageObject"))
                    {
                        SyntaxNode node = at?.Parent?.Parent;

                        if (node != null && node is StructDeclarationSyntax structDeclaration)
                        {
                            MessageObjects.Add(new FindNode(structDeclaration, at));
                        }
                    }
                }


              

                // any struct with at least one attribute is a candidate for property generation
                if (context.Node is AttributeSyntax attribute)
                {
                    if (attribute.Name.ToString().Equals("MessagePack"))
                    {
                        SyntaxNode node = attribute?.Parent?.Parent;

                        if (node != null && node is StructDeclarationSyntax structDeclaration)
                        {
                            MessagePackets.Add(new FindNode(structDeclaration, attribute));
                        }
                    }
                }
            }
            catch { }
        }
    }
    
}
