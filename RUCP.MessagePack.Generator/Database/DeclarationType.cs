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
        private string m_castType;
        private string m_namespace;

        public DeclarationType(string type, string readMethod, string writeMethod, string castType = null, string @namespace = null)
        {
            m_type = type;
            m_readMethod = readMethod;
            m_writeMethod = writeMethod;
            m_castType = castType;
            m_namespace = @namespace;
        }

        public string ReadMethod => m_readMethod;
        public string WriteMethod => m_writeMethod;

        public string TypeName => m_type;
        public string CastTypeName => m_castType;
        public string Namespace => m_namespace; 

        public bool IsNeedCast => !string.IsNullOrEmpty(m_castType);

        public bool IsHaveNamespace => !string.IsNullOrEmpty(m_namespace);
    }
}
