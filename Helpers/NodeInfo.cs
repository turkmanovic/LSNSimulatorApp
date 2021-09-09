using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FirstFloor.ModernUI.Presentation;

namespace LSNSimulator.Helpers
{
    public class NodeProducerInfo {
        public bool producer { get; set; }
        public int dataProducingPeriod { get; set; }
        public int dataSize { get; set; }
        public double deviation { get; set; }
        public ProtocolInfo assignedProtocol { get; set; }
        public List<int> dataPath { get; set; }

    }
    public class NodeInfo 
    {
        public int id { get; set; }
        public string type { get; set; }
        public double processSpeed { get; set; }
        public int compressionLevel { get; set; }
        public int returnSize { get; set; }
        public int overheadSize { get; set; }
        public double activeConsumption { get; set; }
        public double lowpowerConsumption { get; set; }
        public double aggLevel { get; set; }

        public ObservableCollection<Connection> adjConnections = new ObservableCollection<Connection>();

        public string cluster { get; set; } = "NA";
        public bool producerInformationAssigned { get; set; } = false;
        public NodeProducerInfo producerAddInfo;
        public int connectNode(NodeInfo Node2, LinkInfo link) {
            if (Node2.id == id) return 1;
            for (int i = 0; i < Node2.adjConnections.Count; i++)
            {
                if (Node2.adjConnections[i].Node.id == id) return 2;
            }
            for (int i = 0; i < adjConnections.Count; i++)
            {
                if (adjConnections[i].Node.id == Node2.id) return 2;
            }
            Connection connection1 = new Connection();
            Connection connection2 = new Connection();
            connection1.Node = Node2;
            connection1.Link = link;
            adjConnections.Add(connection1);
            connection2.Node = this;
            connection2.Link = link;
            Node2.adjConnections.Add(connection2);
            return 0;
        }
       

    }
}
