using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSNSimulator.Helpers
{
    public class ProtocolInfo
    {
        public int protocolID { get; set; }
        public string protocolName { get; set; }
        public int protocolOverhead { get; set; }
        public bool protocolGenerateResponseFlag { get; set; }

    }
}
