using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Protocol.Codegen;
using System.Collections.Generic;
using System.Linq;

namespace Protocol.Generator.InformationCollector
{
    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    public class SyntaxReceiver : ISyntaxContextReceiver
    {
        private List<EnumNode> m_enums = new List<EnumNode>();
        private Dictionary<string, InterfaceNode> m_interfaces = new Dictionary<string, InterfaceNode>();
        private List<StructNode> m_structs = new List<StructNode>();
        private List<PacketNode> m_packets = new List<PacketNode>();

        private List<StructDeclarationSyntax> m_collectAllStructs = new List<StructDeclarationSyntax>();

        internal IEnumerable<EnumNode> Enums => m_enums;
        internal IEnumerable<InterfaceNode> Interfaces => m_interfaces.Values;
        internal IEnumerable<StructNode> Structs => m_structs;
        internal IEnumerable<PacketNode> Packets => m_packets;


        internal void OnFinishCollect()
        {
            foreach(var @struct in m_collectAllStructs) 
            {
                bool isRegistered = false;
                foreach (var @interface in m_interfaces.Values)
                {
                    if(@struct.ContainsInterface(@interface.TypeName))
                    {
                        @interface.AddType(@struct.Identifier.ValueText);
                        isRegistered = true;
                    }
                }
                if (@struct.ContainsAttribute("MessageObject") || isRegistered)
                {
                    StructNode structNode = new StructNode(@struct);
                    m_structs.Add(structNode);
                }
               
            }
        }
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
                    string @namespace = Helper.GetNamespace(enumDeclaration);
                    if (enumDeclaration.BaseList != null && enumDeclaration.BaseList.Types.Count != 0
                    && enumDeclaration.BaseList.Types[0].Type is PredefinedTypeSyntax baseType)
                    {
                        m_enums.Add(new EnumNode(enumDeclaration.Identifier.ValueText, baseType.Keyword.ValueText, @namespace));
                    }
                    else
                    {
                        m_enums.Add(new EnumNode(enumDeclaration.Identifier.ValueText, "int", @namespace));
                    }
                }

                //Register intefaces
                if (context.Node is InterfaceDeclarationSyntax interfaceNode && interfaceNode.ContainsAttribute("MessageObject"))
                {
                    string @namespace = Helper.GetNamespace(interfaceNode);
                    InterfaceNode @interface = new InterfaceNode(interfaceNode.Identifier.ValueText, @namespace);
                    m_interfaces.Add(@interface.TypeName, @interface);
                }


                //Register struct type
                if (context.Node is StructDeclarationSyntax @struct)
                {
                    m_collectAllStructs.Add(@struct);
                }


              

                // any struct with at least one attribute is a candidate for property generation
                if (context.Node is AttributeSyntax attribute)
                {
                    if (attribute.Name.ToString().Equals("MessagePack"))
                    {
                        SyntaxNode node = attribute?.Parent?.Parent;

                        if (node != null && node is StructDeclarationSyntax structDeclaration)
                        {
                            m_packets.Add(new PacketNode(structDeclaration, attribute));
                        }
                    }
                }
            }
            catch { }
        }
    }
    
}
