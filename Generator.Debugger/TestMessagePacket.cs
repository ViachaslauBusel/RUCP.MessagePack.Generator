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
        public IData[] dataArray { get; set; }
        public List<IData> dataList { get; set; }
        public int length { get; set; }
    }
}
