using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSNSimulator.Helpers;

namespace LSNSimulator.Helpers
{
    public class Connection
    {
        public NodeInfo Node;

        public LinkInfo Link;
    }
    public class DisconnectedNodes
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public NodeInfo node { get; set; }
        public Clusters cluster { get; set; }
    }
}
