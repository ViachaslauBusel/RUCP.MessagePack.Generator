using System;
using System.Collections.Generic;
using System.Text;

namespace RUCP.MessagePack.Generator.Database
{
    internal class DatabaseTypes
    {
        private Dictionary<string, DeclarationType> m_declarationTypes = new Dictionary<string, DeclarationType>();

        //TODO Использовать полное имя типа
        public DatabaseTypes()
        {
            m_declarationTypes.Add("byte[]", new DeclarationType("byte[]", "ReadBytes", "WriteBytes"));
            m_declarationTypes.Add("Guid", new DeclarationType("Guid", "ReadGuid", "WriteGuid"));
            m_declarationTypes.Add("Vector3", new DeclarationType("Vector3", "ReadVector3", "WriteVector3", @namespace: "System.Numerics"));
            m_declarationTypes.Add("byte", new DeclarationType("byte", "ReadByte", "WriteByte", baseType: "byte"));
            m_declarationTypes.Add("short", new DeclarationType("short", "ReadShort", "WriteShort"));
            m_declarationTypes.Add("ushort", new DeclarationType("ushort", "ReadUshort", "WriteUshort"));
            m_declarationTypes.Add("int", new DeclarationType("int", "ReadInt", "WriteInt"));
            m_declarationTypes.Add("long", new DeclarationType("long", "ReadLong", "WriteLong"));
            m_declarationTypes.Add("float", new DeclarationType("float", "ReadFloat", "WriteFloat"));
            m_declarationTypes.Add("bool", new DeclarationType("bool", "ReadBool", "WriteBool"));
            m_declarationTypes.Add("string", new DeclarationType("string", "ReadString", "WriteString"));
        }

        internal void AddType(DeclarationType declarationType)
        {
            m_declarationTypes.Add(declarationType.TypeName, declarationType);
        }

        internal bool ContainsType(string typeName) => m_declarationTypes.ContainsKey(typeName);

        internal DeclarationType GetType(string baseType) => m_declarationTypes[baseType];
    }
}
