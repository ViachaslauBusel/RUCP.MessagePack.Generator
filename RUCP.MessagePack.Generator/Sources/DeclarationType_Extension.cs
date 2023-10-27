using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Codegen
{
    internal static class DeclarationType_Extension
    {
        internal static string Source => @"
//namespaces
using RUCP;
using Protocol;
using System.Collections.Generic;

namespace $name_Space
{
    public static class $type_Extension
    {
        public static $type Read$type(this Packet packet)
        {
           $type msg = new $type();
               //Read data

               return msg;
        }
        public static void Write$type(this Packet packet, $type msg)
        {
              //Write data
        }
    }
}";
    }
}
