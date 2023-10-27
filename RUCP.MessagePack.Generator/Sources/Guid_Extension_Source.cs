using System;
using System.Collections.Generic;
using System.Text;

namespace RUCP.MessagePack.Generator.Sources
{
    internal static class Guid_Extension_Source
    {
        internal static string FileName => "Guid_Extension.g.cs";
        internal static string Source => @"
using RUCP;
using System;

namespace Protocol
{
    public static class Guid_Extension
    {
        public static Guid ReadGuid(this Packet packet)
        {
                return new Guid(packet.ReadBytes());
               //Read data
        }
        public static void WriteGuid(this Packet packet, Guid guid)
        {
            packet.WriteBytes(guid.ToByteArray());
              //Write data
        }
    }
}
";
    }
}
