using RUCP.MessagePack.Generator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Codegen
{
    internal class InterfaceType_Extension
    {
        private const string NamespaceMark = "$name_Space";
        private const string UsingNamespaceMark = "//namespaces";
        private const string InterfaceType = "$InterfaceType";
        private const string TypeToIdMapMark = "//TypeToIdMark";
        private const string DataWriteMark = "//dataWriteMethodsMark";
        private const string DataReadMark = "//dataReadMethodsMark";

        private string CreateTypeToIdLine(string typeName, int id) => $"\n{{ typeof({typeName}), {id} }},";
        private string CreateWriteMethodLine(int id, string interfaceTypeName, string typeName, string wrtiteMethodName) => $"\n{{ {id}, (Packet packet, {interfaceTypeName} data) => {{  packet.{wrtiteMethodName}(({typeName})data); }} }},";
        private string CreateReadMethodLine(int id, string readMethodName) => $"\n{{ {id}, (Packet packet) => {{ return packet.{readMethodName}(); }} }},";


        private string m_interfaceTypeName;


        internal string Source => m_source;

        internal InterfaceType_Extension(string interfaceTypeName, string @namespace)
        {
            m_interfaceTypeName = interfaceTypeName;
            m_source = m_source.Replace(InterfaceType, interfaceTypeName);
            m_source = m_source.Replace(NamespaceMark, @namespace);
        }

        internal void InsertType(int id, string typeName, string readMethod, string writeMethod, string @namespace)
        {
            string namespaceLine = $"\nusing {@namespace};";
            if(!m_source.Contains(namespaceLine)) 
            { m_source = StringHelper.InserAfterMark(m_source, namespaceLine, UsingNamespaceMark); }
            m_source = StringHelper.InserAfterMark(m_source, CreateTypeToIdLine(typeName, id), TypeToIdMapMark);
            m_source = StringHelper.InserAfterMark(m_source, CreateWriteMethodLine(id, m_interfaceTypeName, typeName, writeMethod), DataWriteMark);
            m_source = StringHelper.InserAfterMark(m_source, CreateReadMethodLine(id, readMethod), DataReadMark);
        }

        private string m_source = @"
//namespaces
using RUCP;
using System;
using System.Collections.Generic;

namespace  $name_Space
{
    public static class $InterfaceType_Extension
    {
        private static Dictionary<Type, short> m_typeToIdMap = new Dictionary<Type, short>()
        {
             //TypeToIdMark
        };
        private static Dictionary<int, Action<Packet, $InterfaceType>> m_dataWriteMethods = new Dictionary<int, Action<Packet, $InterfaceType>>()
        {
            //dataWriteMethodsMark
        };
        private static Dictionary<int, Func<Packet, $InterfaceType>> m_dataReadMethods = new Dictionary<int, Func<Packet, $InterfaceType>>()
        {
            //dataReadMethodsMark
        };

        public static short GetID(this $InterfaceType data)
        {
           Type type = data?.GetType();
           return (short)((type != null && m_typeToIdMap.ContainsKey(type)) ? m_typeToIdMap[type] : 0);
        }


        public static $InterfaceType Read$InterfaceType(this Packet packet)
        {
            short typeID = packet.ReadShort();

            //Read Data
            $InterfaceType msg = (typeID != 0) ? m_dataReadMethods[typeID].Invoke(packet) : null;

            return msg;
        }
        public static void Write$InterfaceType(this Packet packet, $InterfaceType data)
        {
            //Write data
            short typeID = GetID(data);
            packet.WriteShort(typeID);
            if (typeID != 0) { m_dataWriteMethods[typeID].Invoke(packet, data); }
        }
    }
}
";

    }
}
