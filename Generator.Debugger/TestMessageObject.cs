﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;

namespace Generator.Debugger
{
    [MessageObject]
    public struct TestMessageObject
    {
        public int Key { get; set; }
        public byte array { get; set; }
    }
}
