using Protocol;

namespace Generator.Debugger
{
    [MessagePack(1, RUCP.Channel.Reliable)]
    public struct TestMessageWithDoubleInterface
    {
        public ITestInterface1 Interface1 { get; set; }
    }

    [MessageObject]
    public interface ITestInterface1
    {
    }
    public struct ItestStruct1 : ITestInterface1
    {
        public ITestInterface2 Interface2 { get; set; }
    }

    [MessageObject]
    public interface ITestInterface2
    {
    }
    public struct ItestStruct2 : ITestInterface2
    {
    }
}
