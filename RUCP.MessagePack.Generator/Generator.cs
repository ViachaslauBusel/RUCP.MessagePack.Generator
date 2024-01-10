using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Protocol.Codegen;
using Protocol.Generator.InformationCollector;
using RUCP.MessagePack.Generator;
using RUCP.MessagePack.Generator.Database;
using RUCP.MessagePack.Generator.Sources;
using System;
using System.Linq;
using System.Text;

namespace Protocol.Generator
{

    [Generator]
    public class Generator : ISourceGenerator
    {
        private DatabaseTypes m_database = new DatabaseTypes();
        private SyntaxReceiver m_syntaxReceiver;
        private GeneratorExecutionContext m_context;

        public void Execute(GeneratorExecutionContext context)
        {
            m_context = context;
            m_syntaxReceiver = context.SyntaxContextReceiver as SyntaxReceiver;

            if (m_syntaxReceiver == null) { return; }
            m_syntaxReceiver.OnFinishCollect();

//            m_context.AddSource($"DebugLog.txt", $@"
//interfaces:{m_syntaxReceiver.Interfaces.Count()},
//structs:{m_syntaxReceiver.Structs.Count()},
//Packets:{m_syntaxReceiver.Packets.Count()},
//Enums:{m_syntaxReceiver.Enums.Count()}
//");

            foreach (var @enum in m_syntaxReceiver.Enums) 
            {
                if (!m_database.ContainsType(@enum.BaseType))
                {
                    //Error
                    continue;
                }

               
                if (!m_database.ContainsType(@enum.TypeName))
                {
                    DeclarationType declarationInterface = @enum.CreateDeclaration(m_database.GetType(@enum.BaseType));
                    m_database.AddType(declarationInterface);
                }
            }


            foreach(var @struct in m_syntaxReceiver.Structs) 
            {
                RegisterStruct(@struct);
            }

            foreach (var @interface in m_syntaxReceiver.Interfaces)
            {
                InterfaceType_Extension interfaceSource = new InterfaceType_Extension(@interface.TypeName, @interface.Namespace);
                foreach (var type in @interface.RegisteredTypes)
                {
                    if (!m_database.ContainsType(type.Value)) continue;
                    var declarationType = m_database.GetType(type.Value);
                    interfaceSource.InsertType(type.Key, declarationType.TypeName, declarationType.ReadMethod, declarationType.WriteMethod, declarationType.Namespace);
                }
                if (!m_database.ContainsType(@interface.TypeName)) 
                { m_database.AddType(DeclarationType.Create(@interface.TypeName, @namespace:@interface.Namespace)); }
                m_context.AddSource($"{@interface.TypeName}_Extension.g.cs", interfaceSource.Source);
            }

            foreach (PacketNode packetNode in m_syntaxReceiver.Packets)
            {
                string source = MessagePack_Extension.Source;
                source = source.Replace("$name_Space", packetNode.Namespace);
                source = source.Replace("$name_Extension", $"{packetNode.TypeName}_Extension");
                source = source.Replace("$name_Struct", packetNode.TypeName);
                source = source.Replace("$name_Opcode", packetNode.OpcodeName);
                source = source.Replace("$name_Channel", packetNode.ChannelName);

                foreach (var member in packetNode.Members)
                {
                    source = WriteMember(source, member.Identifier.ValueText, member.Type.ToString());
                }

                m_context.AddSource($"{packetNode.TypeName}_Extension.g.cs", source);
            }

           
        }

        private void RegisterStruct(StructNode @struct)
        {
            string source = DeclarationType_Extension.Source;

            source = source.Replace("$name_Space", $"{@struct.Namespace}");
            source = source.Replace("$type", @struct.TypeName);

            foreach (var member in @struct.Members)
            {
                source = WriteMember(source, member.Identifier.ValueText, member.Type.ToString());
            }
            if (!m_database.ContainsType(@struct.TypeName))
            { m_database.AddType(new DeclarationType(@struct.TypeName, $"Read{@struct.TypeName}", $"Write{@struct.TypeName}", null, @namespace: @struct.Namespace)); }
            m_context.AddSource($"{@struct.TypeName}_Extension.g.cs", source);
        }

        private string WriteMember(string source, string fieldName, string fieldType)
        {
            FieldType type = FieldType.Default;
            //Если этот тип еще не зарегестрирован
            if (!m_database.ContainsType(fieldType))
            {
                if(RegularExpressionHelper.IsArray(fieldType)) 
                {
                    fieldType = fieldType.Replace("[]", "");
                    type = FieldType.Array;
                }
                if (RegularExpressionHelper.IsList(fieldType))
                {
                    fieldType = fieldType.Replace("List<", "").Replace(">", "");
                    type = FieldType.List;
                }
                if(RegularExpressionHelper.IsNullable(fieldType))
                {
                    fieldType = fieldType.Replace("?", "");
                    type = FieldType.Nullable;
                }

                if (!m_database.ContainsType(fieldType))
                {
                    //Попытаться найти в незарегестированых типах
                    var str = m_syntaxReceiver.Structs.FirstOrDefault(s => s.TypeName.Equals(fieldType));
                    if (str != null) { RegisterStruct(str); }
                }

                //Если не удалось зарегестрировать, пропустить поля
                if (!m_database.ContainsType(fieldType))
                { return source; }//TODO вывести уведомления о томчто тип не зарегестирован
            }
          

           return RW_Codegen.InsertRWLine(source, fieldName, m_database.GetType(fieldType), type);
        }

        //public void Initialize(GeneratorInitializationContext context)
        //{
        //    throw new NotImplementedException();
        //}

        //source = RW_Codegen.InsertRWLine(source, syntaxReceiver, findNode.StructNode);

        //    syntaxReceiver.readDeclarationTypes.Add(type, $"Read{type}");
        //    syntaxReceiver.writeDeclarationTypes.Add(type, $"Write{type}");

        //    syntaxReceiver.namespaces.Add(type, Helper.GetNamespace(findNode.StructNode));
        //}

        public void Initialize(GeneratorInitializationContext context)
        {
            var MessagePackText = SourceText.From(@"
using System;
using RUCP;

namespace Protocol
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class MessagePackAttribute : Attribute
    {
        private short m_opcode;
        private Channel m_channel;

        public MessagePackAttribute(short opcode, Channel channel)
        {
            this.m_opcode = opcode;
            this.m_channel = channel;
        }
    }
}", Encoding.UTF8);

         var MessageObjectText = SourceText.From(@"
using System;
using RUCP;

namespace Protocol
{
     [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class MessageObjectAttribute : Attribute
    {
    }
}", Encoding.UTF8);



            // Register the attribute source
            context.RegisterForPostInitialization((i) =>
            {
                i.AddSource("MessageObjectAttribute.g.cs", MessageObjectText);
                i.AddSource("MessagePackAttribute.g.cs", MessagePackText);
                i.AddSource(Guid_Extension_Source.FileName, Guid_Extension_Source.Source);
                i.AddSource(Vector3_Extension_Source.FileName, Vector3_Extension_Source.Source);
            });

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    }
    
}
