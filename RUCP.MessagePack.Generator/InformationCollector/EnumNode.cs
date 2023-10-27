using RUCP.MessagePack.Generator.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Generator.InformationCollector
{
    internal class EnumNode
    {
        private string m_type;
        private string m_baseType;
        private string m_namespace;

        public EnumNode(string type, string baseType, string @namespace)
        {
            m_type = type;
            m_baseType = baseType;
            m_namespace = @namespace;
        }

        public string TypeName => m_type;
        public string BaseType => m_baseType;

        internal DeclarationType CreateDeclaration(DeclarationType baseDeclarationType)
        {
            return new DeclarationType(m_type, baseDeclarationType.ReadMethod, baseDeclarationType.WriteMethod, baseType: m_baseType, @namespace: m_namespace);
        }
    }
}
