using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Protocol.Generator.InformationCollector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Protocol.Codegen
{
    internal static class TypeDeclarationSyntaxHelper
    {
        internal static bool ContainsAttribute(this TypeDeclarationSyntax syntax, string attribute)
        {
            foreach (var attList in syntax.AttributeLists)
            {
                foreach (var att in attList.Attributes)
                {
                    string name = att.Name.ToString();
                    if (name.Equals(attribute)) return true;
                }
            }
            return false;
        }

        internal static bool ContainsInterface(this TypeDeclarationSyntax syntax, IEnumerable<string> interfaces)
        {
            if(syntax.BaseList == null) return false;   
            foreach(var type in syntax.BaseList.Types)
            {
                string typeName = type.ToString();
                if(interfaces.Any(i => i.Equals(typeName))) return true;
            }
            return false;
        }
        internal static bool ContainsInterface(this TypeDeclarationSyntax syntax,string interfaceName)
        {
            if (syntax.BaseList == null) return false;
            foreach (var type in syntax.BaseList.Types)
            {
                string typeName = type.ToString();
                if (interfaceName.Equals(typeName)) return true;
            }
            return false;
        }

        internal static AttributeSyntax GetAttribute(this TypeDeclarationSyntax syntax, string attribute) 
        {
            foreach(var attList in syntax.AttributeLists) 
            {
                foreach(var att in attList.Attributes)
                {
                    string name = att.Name.ToString();
                    if (name.Equals(attribute)) return att;
                }
            }
            return null;
        }
    }
}
