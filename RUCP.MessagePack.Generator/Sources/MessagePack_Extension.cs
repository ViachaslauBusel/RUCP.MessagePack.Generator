using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Codegen
{
    internal static class MessagePack_Extension
    {
        internal static string Source => @"
//namespaces
using RUCP;
using Protocol;

namespace $name_Space
{
    public static class $name_Extension
    {
        public static void Read(this Packet packet, out $name_Struct msg)
        {
            msg = new $name_Struct();
               //Read data
        }
        public static void Write(this Packet packet, $name_Struct msg)
        {
              //Write data
        }

        public static void Send(this Client client, $name_Struct msg)
        {
            Packet packet = Packet.Create($name_Channel);
            packet.OpCode = $name_Opcode;
            packet.Write(msg);
            client.Send(packet);
        }
    }
}";
    }
}
