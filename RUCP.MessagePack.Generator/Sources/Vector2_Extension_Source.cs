using System;
using System.Collections.Generic;
using System.Text;

namespace RUCP.MessagePack.Generator.Sources
{
    internal class Vector2_Extension_Source
    {
        internal static string FileName => "Vector2_Extension.g.cs";
        internal static string Source => @"
using RUCP;
using System;
using System.Numerics;

namespace Protocol
{
    public static class Vector2_Extension
    {
        public static Vector2 ReadVector2(this Packet packet)
        {
            //Read data
            Vector2 data = new Vector2();
            data.X  = packet.ReadFloat();
            data.Y = packet.ReadFloat();
            return data;
        }

        public static void WriteVector2(this Packet packet, Vector2 vector)
        {
            //Write data
            packet.WriteFloat(vector.X);
            packet.WriteFloat(vector.Y);
        }
    }
}
";
    }
}
