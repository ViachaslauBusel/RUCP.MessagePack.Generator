using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Protocol.Generator.InformationCollector
{
    internal class InterfaceNode
    {
        private string m_type;
        private string m_namespace;
        private Dictionary<short, string> m_registeredIdToTypes = new Dictionary<short, string>();
        private HashSet<string> m_cacheTypes = new HashSet<string>();

        public InterfaceNode(string type, string @namespace)
        {
            m_type = type;
            m_namespace = @namespace;
        }

        public string TypeName => m_type;
        public string Namespace => m_namespace;
        public IEnumerable<KeyValuePair<short, string>> RegisteredTypes => m_registeredIdToTypes;

        internal void AddType(string typeName)
        {
            if (!m_cacheTypes.Add(typeName)) return;

            short typeId = (short)typeName.GetHashCode();
            if (typeId == 0 || m_registeredIdToTypes.ContainsKey(typeId)) { typeId++; }
            m_registeredIdToTypes.Add(typeId, typeName);    
        }
    }
}
