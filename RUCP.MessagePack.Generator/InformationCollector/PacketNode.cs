using Microsoft.CodeAnalysis.CSharp.Syntax;
using Protocol.Codegen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Generator.InformationCollector
{
    internal class PacketNode
    {
        private string m_type;
        private string m_namespace;
        private string m_opcodeName;
        private string m_channelName;
        private StructDeclarationSyntax m_structDeclaration;
        private AttributeSyntax m_attribute;

        public PacketNode(StructDeclarationSyntax structDeclaration, AttributeSyntax attribute)
        {
            m_structDeclaration = structDeclaration;
            m_attribute = attribute;

            m_type = m_structDeclaration.Identifier.ValueText;
            m_namespace = Helper.GetNamespace(structDeclaration);


            AttributeArgumentSyntax arg = attribute.ArgumentList.Arguments[0];

            if (arg.Expression is LiteralExpressionSyntax literalExpression)
            {
                m_opcodeName = literalExpression.Token.Text;
            }
            if (arg.Expression is MemberAccessExpressionSyntax memberAccessExpression
            && arg.Parent is AttributeArgumentListSyntax att
            && att.Arguments[0].Expression is MemberAccessExpressionSyntax parentMember)
            {

                m_opcodeName = $"{((IdentifierNameSyntax)memberAccessExpression.Expression).Identifier.ValueText}.{memberAccessExpression.Name.ToString()}";
            }

            arg = attribute.ArgumentList.Arguments[1];
            if (arg.Expression is MemberAccessExpressionSyntax mae)
            {
                m_channelName = $"Channel.{mae.Name.ToString()}";
            }

        }

        public string Namespace => m_namespace;
        public string TypeName => m_type;
        public string OpcodeName => m_opcodeName;
        public string ChannelName => m_channelName;

        public IEnumerable<PropertyDeclarationSyntax> Members => m_structDeclaration.Members.Where(m => m is PropertyDeclarationSyntax).Select(m => m as PropertyDeclarationSyntax);
    }
}
