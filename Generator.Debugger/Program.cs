using RUCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Debugger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Packet packet = Packet.Create(Channel.Reliable);
            IData data = new TestIntarfaceObject();
            packet.WriteIData(data);
            TestMessagePacket message = new TestMessagePacket();
            Client client = new Client();
            client.Send(message);
              Console.ReadLine();

        }


    }


}
