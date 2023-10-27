using System;
using System.Collections.Generic;
using System.Text;

namespace RUCP.MessagePack.Generator.Database
{
    internal class DeclarationType
    {
        private string m_type;
        private string m_readMethod;
        private string m_writeMethod;
        private string m_baseType;
        private string m_namespace;


        public DeclarationType(string type, string readMethod, string writeMethod, string baseType = null, string @namespace = null)
        {
            m_type = type;
            m_readMethod = readMethod;
            m_writeMethod = writeMethod;
            m_baseType = baseType;
            m_namespace = @namespace;
        }
        public static DeclarationType Create(string type, string baseType = null, string @namespace = null) => new DeclarationType(type, $"Read{type}", $"Write{type}", baseType, @namespace);
        public string ReadMethod => m_readMethod;
        public string WriteMethod => m_writeMethod;

        public string TypeName => m_type;
        public string BaseTypeName => m_baseType;
        public string Namespace => m_namespace; 

        public bool IsNeedCast => !string.IsNullOrEmpty(m_baseType);

        public bool IsHaveNamespace => !string.IsNullOrEmpty(m_namespace);
    }
}
