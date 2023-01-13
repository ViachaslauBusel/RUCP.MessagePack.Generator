using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Protocol.Generator
{
    public class FindNode
    {
        public StructDeclarationSyntax StructNode { get; private set; }
        public AttributeSyntax AttributeNode { get; private set; }
        public FindNode(StructDeclarationSyntax structNode, AttributeSyntax attributeNode)
        {
            StructNode = structNode;
            AttributeNode = attributeNode;
        }
    }
}
