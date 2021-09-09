using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using LSNSimulator.Helpers;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Globalization;

namespace LSNSimulator.Pages
{
    /// <summary>
    /// Interaction logic for Topology.xaml
    /// </summary>
    public partial class Topology : UserControl
    {
        public ObservableCollection<NodeInfo> TopologyNodes = new ObservableCollection<NodeInfo>();
        public ObservableCollection<LinkInfo> TopologyLinks = new ObservableCollection<LinkInfo>();
        public ObservableCollection<DisconnectedNodes> disconnectedNodes = new ObservableCollection<DisconnectedNodes>();
        public ObservableCollection<NodeInfo> connectedNodes = new ObservableCollection<NodeInfo>();
        public ObservableCollection<TopologyInfo> topologyContent = new ObservableCollection<TopologyInfo>();
        public ObservableCollection<ProtocolInfo> protocolsDB = new ObservableCollection<ProtocolInfo>();
        public Topology()
        {
            InitializeComponent();
            TopologyNodes = (ObservableCollection<NodeInfo>)Application.Current.Properties["NodesDB"];
            disconnectedNodes = (ObservableCollection<DisconnectedNodes>)Application.Current.Properties["DisconnectedNodes1"];
            TopologyLinks = (ObservableCollection<LinkInfo>)Application.Current.Properties["LinksDB"];
            protocolsDB = (ObservableCollection<ProtocolInfo>)Application.Current.Properties["ProtocolsDB"];
            FirstNodesTable.ItemsSource = disconnectedNodes;
            SecondNodesTable.ItemsSource = disconnectedNodes;
            cb_linkSelection.ItemsSource = TopologyLinks;
            TopologyTable.ItemsSource = topologyContent;
        }

        private TopologyInfo MakeTopologyInfoFromNode(NodeInfo node) {

            TopologyInfo returnValue = new TopologyInfo();
            string NodeID;
            string NodeConnections = "";
            string NodeProducerInformation = "";

            NodeID = node.id.ToString();
            foreach (var connection in node.adjConnections) {
                NodeConnections += connection.Node.id.ToString();
                NodeConnections += "-";
                NodeConnections += connection.Link.linkID.ToString();
                if (node.adjConnections.IndexOf(connection) == (node.adjConnections.Count - 1)) break;
                NodeConnections += ",";
            }
            if ((node.type == "Producer") && (node.producerInformationAssigned == true)) {
                NodeProducerInformation += "p" + node.producerAddInfo.dataProducingPeriod.ToString() + 
                    "-" + node.producerAddInfo.assignedProtocol.protocolID.ToString() + " " +
                    node.producerAddInfo.dataSize.ToString() + " " + node.id.ToString() + "," +
                    String.Join(",", node.producerAddInfo.dataPath.Select(x => x.ToString()).ToArray()) + "";

            }
            returnValue.NodeID = NodeID;
            returnValue.Connections = NodeConnections;
            returnValue.ProducerInfo = NodeProducerInformation;
            return returnValue;
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            disconnectedNodes = (ObservableCollection<DisconnectedNodes>)Application.Current.Properties["DisconnectedNodes1"];
            TopologyNodes = (ObservableCollection<NodeInfo>)Application.Current.Properties["NodesDB"];
            TopologyLinks = (ObservableCollection<LinkInfo>)Application.Current.Properties["LinksDB"];
            protocolsDB = (ObservableCollection<ProtocolInfo>)Application.Current.Properties["ProtocolsDB"];
            cb_linkSelection.ItemsSource = TopologyLinks;
            FirstNodesTable.ItemsSource = disconnectedNodes;
            SecondNodesTable.ItemsSource = disconnectedNodes;
            cb_linkSelection.SelectedIndex = 0;
            FirstNodesTable.Items.Refresh();
            SecondNodesTable.Items.Refresh();
            cb_linkSelection.Items.Refresh(); 
            topologyContent.Clear();
            foreach (var nodeItem in TopologyNodes)
            {
                if (nodeItem.adjConnections.Count > 0)
                {
                    TopologyInfo tmpInfo = MakeTopologyInfoFromNode(nodeItem);
                    topologyContent.Add(tmpInfo);
                }
            }
            TopologyTable.ItemsSource = topologyContent;
        }

        private void connectNodes(object sender, RoutedEventArgs e)
        {
            int numberOfSelectedItemsInFirstTable = FirstNodesTable.SelectedItems.Count;
            int numberOfSelectedItemsInSecondTable = SecondNodesTable.SelectedItems.Count;
            TopologyNodes = (ObservableCollection<NodeInfo>)Application.Current.Properties["NodesDB"];
            LinkInfo selectedLink = ((LinkInfo)cb_linkSelection.SelectedItem);
            if ((numberOfSelectedItemsInFirstTable > 1) && (numberOfSelectedItemsInSecondTable > 1)) {
                MessageBox.Show("You can only connect multiple nodes to one node. It is not possible to connect multiple nodes to multiple nodes");
                return;
            }
            if (numberOfSelectedItemsInFirstTable >= numberOfSelectedItemsInSecondTable)
            {
                foreach (var obj in FirstNodesTable.SelectedItems)
                {
                    DisconnectedNodes tmpDisconnectedNode1 = (DisconnectedNodes)obj;
                    if (tmpDisconnectedNode1.Type == "Node")
                    {
                        DisconnectedNodes tmpDisconnectedNode2 = (DisconnectedNodes)SecondNodesTable.SelectedItems[0];
                        if (tmpDisconnectedNode2.Type == "Node")
                        {
                            int result = tmpDisconnectedNode2.node.connectNode(tmpDisconnectedNode1.node, selectedLink);
                            if (result != 0)
                            {
                                MessageBox.Show("There is error during connecting nodes");
                                return;
                            }
                        }
                        else {
                            MessageBox.Show("You cannot connect multiple nodes to the cluster");
                        }
                    }
                    else {
                        foreach (var nodeItem in tmpDisconnectedNode1.cluster.grupedNodes) {
                            DisconnectedNodes tmpDisconnectedNode2 = (DisconnectedNodes)SecondNodesTable.SelectedItems[0];
                            if (tmpDisconnectedNode2.Type == "Node")
                            {
                                int result = tmpDisconnectedNode2.node.connectNode(nodeItem, selectedLink);
                                if (result != 0)
                                {
                                    MessageBox.Show("There is error during connecting nodes");
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("You cannot connect cluster to the cluster");
                            }
                        }                    
                    }
                }
            }
            topologyContent.Clear();
            foreach (var nodeItem in TopologyNodes) {
                if (nodeItem.adjConnections.Count > 0) {
                    TopologyInfo tmpInfo = MakeTopologyInfoFromNode(nodeItem);
                    topologyContent.Add(tmpInfo);
                }
            }
            TopologyTable.ItemsSource = topologyContent;
        }

        private void saveTopology(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true; 
            CultureInfo culture;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName;
                string configPath = (String)path + "\\Config.txt"; 
                StreamWriter configFile = new StreamWriter(configPath);
                foreach (var nodeItem in TopologyNodes)
                {
                    string Line = "";
                    Line += nodeItem.id.ToString();
                    Line += " ";
                    if (nodeItem.type == "Producer") Line += "p";
                    if (nodeItem.type == "Interface") Line += "i";
                    if (nodeItem.type == "Consumer") Line += "c";
                    Line += " ";
                    Line += nodeItem.processSpeed.ToString(CultureInfo.CreateSpecificCulture("en-CA"));
                    Line += " ";
                    Line += nodeItem.compressionLevel.ToString();
                    Line += " ";
                    Line += nodeItem.returnSize.ToString();
                    Line += " ";
                    Line += nodeItem.overheadSize.ToString();
                    Line += " ";
                    Line += nodeItem.activeConsumption.ToString();
                    Line += " ";
                    Line += nodeItem.lowpowerConsumption.ToString();
                    Line += " ";
                    Line += nodeItem.aggLevel.ToString();
                    Line += ";";
                    TopologyInfo tmpInfo = MakeTopologyInfoFromNode(nodeItem);
                    Line += tmpInfo.Connections;
                    Line += ";";
                    if (nodeItem.type == "Producer")
                    {
                        Line += tmpInfo.ProducerInfo;
                        Line += ";";
                    }
                    configFile.WriteLine(Line);
                }
                configFile.WriteLine("0 0");
                configFile.Close();
                string protocolsPath = (String)path + "\\Protocols.txt";
                StreamWriter protocolsFile = new StreamWriter(protocolsPath);
                foreach (var protocolItem in protocolsDB)
                {
                    string Line = "";
                    Line += protocolItem.protocolID.ToString();
                    Line += " ";
                    if (protocolItem.protocolGenerateResponseFlag == true) Line += "y";
                    else Line += "n";
                    Line += " ";
                    Line += protocolItem.protocolOverhead.ToString();
                    Line += " ";
                    Line += protocolItem.protocolName;
                    protocolsFile.WriteLine(Line);
                }
                protocolsFile.WriteLine("- - - -");
                protocolsFile.Close();
                string linksPath = (String)path + "\\Links.txt";
                StreamWriter linkFile = new StreamWriter(linksPath);
                foreach (var linkItem in TopologyLinks)
                {
                    string Line = "";
                    Line += linkItem.linkID.ToString();
                    Line += " ";
                    Line += linkItem.linkSpeed.ToString(CultureInfo.CreateSpecificCulture("en-CA"));
                    Line += " ";
                    Line += linkItem.linkDeviation.ToString();
                    Line += " ";
                    Line += linkItem.linkMTUSize.ToString();
                    Line += " ";
                    Line += linkItem.linkNumberOfHops.ToString();
                    Line += " ";
                    Line += linkItem.linkTxConsumption.ToString();
                    Line += " ";
                    Line += linkItem.linkRxConsumption.ToString();
                    Line += " ";
                    Line += linkItem.linkName;
                    linkFile.WriteLine(Line);
                }
                linkFile.WriteLine("- - - -");
                linkFile.Close();
                MessageBox.Show("Configuration successfully saved");
            }

        }
    }
}
