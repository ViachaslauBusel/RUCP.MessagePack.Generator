using System;
using System.Collections.Generic;
using System.Text;

namespace RUCP.MessagePack.Generator.Sources
{
    internal class Vector3_Extension_Source
    {
        internal static string FileName => "Vector3_Extension.g.cs";
        internal static string Source => @"
using RUCP;
using System;
using System.Numerics;

namespace Protocol
{
    public static class Vector3_Extension
    {
        public static Vector3 ReadVector3(this Packet packet)
        {
            //Read data
            Vector3 data = new Vector3();
            data.X  = packet.ReadFloat();
            data.Y = packet.ReadFloat();
            data.Z = packet.ReadFloat();
            return data;
         
        }
        public static void WriteVector3(this Packet packet, Vector3 vector)
        {
            //Write data
            packet.WriteFloat(vector.X);
            packet.WriteFloat(vector.Y);
            packet.WriteFloat(vector.Z);
        }
    }
}
";
    }
}
