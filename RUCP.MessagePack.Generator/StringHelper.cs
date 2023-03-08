using Protocol.Codegen;
using Protocol.Generator.InformationCollector;
using System;
using System.Collections.Generic;
using System.Text;

namespace RUCP.MessagePack.Generator
{
    internal static class StringHelper
    {

        internal static string InserAfterMark(string source, string line, string mark)
        {
            int markIndex = source.IndexOf(mark) + mark.Length;
            return source.Insert(markIndex, line);
        }
    }
}
