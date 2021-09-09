using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSNSimulator.Helpers
{
    public class LinkInfo
    {
        public int linkID { get; set; }
        public string linkName { get; set; }
        public double linkSpeed { get; set; }
        public int linkDeviation { get; set; }
        public int linkMTUSize { get; set; }
        public int linkNumberOfHops { get; set; }
        public double linkTxConsumption { get; set; }
        public double linkRxConsumption { get; set; }
    }
}
