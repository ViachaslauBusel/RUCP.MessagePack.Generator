using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Protocol.Codegen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Generator.InformationCollector
{
    internal class StructNode
    {
        private StructDeclarationSyntax m_struct;

        public StructNode(StructDeclarationSyntax @struct)
        {
            m_struct = @struct;
        }

        public string TypeName => m_struct.Identifier.ValueText;

        public string Namespace => Helper.GetNamespace(m_struct);

        public IEnumerable<PropertyDeclarationSyntax> Members =>
            m_struct.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => !p.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword)));
    }
}
