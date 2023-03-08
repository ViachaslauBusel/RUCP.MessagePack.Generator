using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Debugger
{
    [MessagePack(1, RUCP.Channel.Reliable)]
    public struct TestMessagePacket
    {
        public TestMessageObject Object {get; set;}
    }
}
