using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LSNSimulator.Helpers;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.RightsManagement;
using FirstFloor.ModernUI.Windows.Media;
using LSNSimulator.Windows;

namespace LSNSimulator.Pages
{
    /// <summary>
    /// Interaction logic for Nodes.xaml
    /// </summary>
    public partial class Nodes : UserControl
    {
        static int nodeIDCounter = 1;
        static int layerIDCounter = 0;
        public ObservableCollection<NodeInfo> nodesDB ;
        public ObservableCollection<Clusters> clustersDB = new ObservableCollection<Clusters>();
        public ObservableCollection<DisconnectedNodes> disconectNodes2 = new ObservableCollection<DisconnectedNodes>();

        SaveConfiguration configurationInfo;
        ProducerData producerNodeInfoWindow;
        public Nodes()
        {
            InitializeComponent();
            NodesTable.Items.Clear();
            nodesDB = new ObservableCollection<NodeInfo>();
            NodesTable.ItemsSource = nodesDB;
            li_clusters.ItemsSource = clustersDB;
            configurationInfo = new SaveConfiguration();
            Application.Current.Properties["NodesDB"] = nodesDB;
            Application.Current.Properties["DisconnectedNodes1"] = disconectNodes2;
            producerNodeInfoWindow = new ProducerData();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            for (int i = 0; i < Int32.Parse(tb_numberOfNodes.Text); i++)
            {
                NodeInfo tmpNodeInfo = new NodeInfo();
                tmpNodeInfo.id = nodeIDCounter++;
                tmpNodeInfo.type = cb_type.Text;
                if (tmpNodeInfo.type == "Producer")
                {
                    tmpNodeInfo.producerAddInfo = new NodeProducerInfo();
                    tmpNodeInfo.producerAddInfo.producer = true;
                    tmpNodeInfo.producerInformationAssigned = false;
                }
                double randomNumber = random.NextDouble();
                double minSpeed = ((100 - Double.Parse(tb_processingSpeedDeviation.Text)) / 100)*Double.Parse(tb_processingSpeed.Text);
                double maxSpeed = ((100 + Double.Parse(tb_processingSpeedDeviation.Text)) / 100) * Double.Parse(tb_processingSpeed.Text); ;
                tmpNodeInfo.processSpeed = (randomNumber) * (maxSpeed - minSpeed) + minSpeed;
                tmpNodeInfo.compressionLevel = Int32.Parse(tb_compresionRate.Text);
                tmpNodeInfo.returnSize = Int32.Parse(tb_responseSize.Text);
                tmpNodeInfo.overheadSize = Int32.Parse(tb_overheadSize.Text);
                tmpNodeInfo.activeConsumption = Int32.Parse(tb_activeConsumption.Text);
                tmpNodeInfo.lowpowerConsumption = Int32.Parse(tb_lowPowerConsumption.Text);
                tmpNodeInfo.aggLevel = Int32.Parse(tb_aggLevel.Text);
                nodesDB.Add(tmpNodeInfo);
                DisconnectedNodes tmpDisconnectedNodes = new DisconnectedNodes();
                tmpDisconnectedNodes.Name = (nodeIDCounter - 1).ToString();
                tmpDisconnectedNodes.Type = "Node";
                tmpDisconnectedNodes.node = tmpNodeInfo;
                disconectNodes2.Add(tmpDisconnectedNodes);
            }
            if (NodesTable.Visibility == Visibility.Hidden) {
                NodesTable.Visibility = Visibility.Visible;
            }
        }
        private void UpdateScale(object sender, RoutedEventArgs e)
        {
            foreach (var item in clustersDB)
            {
                Clusters tmpClusterInfo = item;
                int clusterSize = tmpClusterInfo.grupedNodes.Count;
                NodeInfo tmpNodeInfoReference = tmpClusterInfo.grupedNodes[0];
                var random = new Random();
                int size = ((clusterSize) *(Int32.Parse(tb_scale.Text) - 1)) > 0 ? ((clusterSize) * (Int32.Parse(tb_scale.Text) - 1)) : 1;
                for (int i = 0; i < size; i++) {
                    NodeInfo tmpNodeInfo = new NodeInfo();
                    tmpNodeInfo.id = nodeIDCounter++;
                    tmpNodeInfo.type = tmpNodeInfoReference.type;
                    double randomNumber = random.NextDouble();
                    if (tmpNodeInfo.type == "Producer")
                    {
                        tmpNodeInfo.producerAddInfo = new NodeProducerInfo();
                        tmpNodeInfo.producerAddInfo.producer = tmpNodeInfoReference.producerAddInfo.producer;
                        tmpNodeInfo.producerInformationAssigned = tmpNodeInfoReference.producerInformationAssigned;
                        if (tmpNodeInfo.producerInformationAssigned == true)
                        {
                            tmpNodeInfo.producerAddInfo.dataPath = tmpNodeInfoReference.producerAddInfo.dataPath;
                            tmpNodeInfo.producerAddInfo.assignedProtocol = tmpNodeInfoReference.producerAddInfo.assignedProtocol;
                            tmpNodeInfo.producerAddInfo.dataSize = tmpNodeInfoReference.producerAddInfo.dataSize;
                            double x = 0.01;
                            double minValue = ((100 - x) / 100) * Double.Parse("2000000");
                            double maxValue = ((100 + x) / 100) * Double.Parse("2000000");
                            int producingPeriod = (int)((randomNumber) * (maxValue - minValue) + minValue);
                            tmpNodeInfo.producerAddInfo.dataProducingPeriod = producingPeriod;
                            tmpNodeInfo.producerAddInfo.deviation = tmpNodeInfoReference.producerAddInfo.deviation;
                        }
                    }
                    tmpNodeInfo.processSpeed = tmpNodeInfoReference.processSpeed;
                    tmpNodeInfo.compressionLevel = tmpNodeInfoReference.compressionLevel;
                    tmpNodeInfo.returnSize = tmpNodeInfoReference.returnSize;
                    tmpNodeInfo.overheadSize = tmpNodeInfoReference.overheadSize;
                    tmpNodeInfo.activeConsumption = tmpNodeInfoReference.activeConsumption;
                    tmpNodeInfo.lowpowerConsumption = tmpNodeInfoReference.lowpowerConsumption;
                    tmpNodeInfo.aggLevel = tmpNodeInfoReference.aggLevel;
                    tmpNodeInfo.cluster = tmpClusterInfo.name;
                    nodesDB.Add(tmpNodeInfo); 
                    tmpClusterInfo.grupedNodes.Add(tmpNodeInfo);
                }                
            }
        }
        private void Save_Nodes_Info(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                File.WriteAllText(path, JsonConvert.SerializeObject(nodesDB, Formatting.Indented));
                MessageBox.Show("Nodes list successfully saved");
            }
        }

        private void Load_Nodes_Info(object sender, RoutedEventArgs e)
        {
            ObservableCollection<NodeInfo> tmpnodesDB = new ObservableCollection<NodeInfo>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                if (nodesDB.Count != 0)
                {
                    nodesDB.Clear();
                }
                var path = openFileDialog.FileName;
                tmpnodesDB = JsonConvert.DeserializeObject<ObservableCollection<NodeInfo>>(File.ReadAllText(path));
                foreach (var item in tmpnodesDB)
                {
                    ((NodeInfo)item).cluster = "NA";
                    nodesDB.Add(item);
                }               
                if (NodesTable.Visibility == Visibility.Hidden)
                {
                    NodesTable.Visibility = Visibility.Visible;
                }
            }
        }

        private void test(object sender, MouseButtonEventArgs e)
        {

        }

        private void assigne_to_cluster(object sender, RoutedEventArgs e)
        {            
            int numberOfSelectedNodes = NodesTable.SelectedItems.Count;
            string selectedClusterName = ((Clusters)li_clusters.SelectedItem).name;
            Clusters selectedCluster = null;
            foreach (var obj in clustersDB) {
                if (((Clusters)obj).name == selectedClusterName) selectedCluster = ((Clusters)obj);
            }
            if (selectedCluster == null) return;
            foreach (var obj in NodesTable.SelectedItems)
            {                
                NodeInfo tmp = (NodeInfo)obj;
                tmp.cluster = selectedClusterName;
                selectedCluster.grupedNodes.Add(tmp);
                DisconnectedNodes nodeToRemove = null;
                foreach (var obj1 in disconectNodes2) {
                    if (obj1.Type == "Node") {
                        if (Int32.Parse(obj1.Name) == tmp.id) {
                            nodeToRemove = obj1;
                            break;
                        }
                    }
                }
                disconectNodes2.Remove(nodeToRemove);
            }
            DisconnectedNodes tmpDN = new DisconnectedNodes();
            tmpDN.Name = selectedClusterName;
            tmpDN.Type = "Cluster";
            tmpDN.cluster = selectedCluster;
            disconectNodes2.Add(tmpDN);
            NodesTable.Items.Refresh();
            //addToClusterWindow.Show();

        }
        private void set_producer_information(object sender, RoutedEventArgs e) {
            int numberOfSelectedNodes = NodesTable.SelectedItems.Count;
            foreach (var obj in NodesTable.SelectedItems)
            {
                NodeInfo tmp = (NodeInfo)obj;
                if (tmp.type != "Producer") {
                    MessageBox.Show("Producer information is only possible to set for producer node. Please select only producer nodes");
                    return;
                }                
            }
            producerNodeInfoWindow.availableProtocols = ((ObservableCollection<ProtocolInfo>)Application.Current.Properties["ProtocolsDB"]);
            if (producerNodeInfoWindow.availableProtocols == null)
            {
                MessageBox.Show("First create protocol");
                return;
            }
            if ((NodesTable.SelectedItems.Count > 1) && (((NodeInfo)NodesTable.SelectedItems[0]).producerInformationAssigned == true))
            {
                producerNodeInfoWindow.tb_pi_dataPath.Text = String.Join(",", ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.dataPath.Select(x => x.ToString()).ToArray());
                producerNodeInfoWindow.tb_pi_dataSize.Text = ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.dataSize.ToString();
                producerNodeInfoWindow.tb_pi_dpr.Text = ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.dataProducingPeriod.ToString();
                producerNodeInfoWindow.tb_pi_dpr_deviation.Text = ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.deviation.ToString();
            }
            else if ((NodesTable.SelectedItems.Count == 1) && (((NodeInfo)NodesTable.SelectedItems[0]).producerInformationAssigned == true))
            {
                producerNodeInfoWindow.tb_pi_dataPath.Text = String.Join(",", ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.dataPath.Select(x => x.ToString()).ToArray());
                producerNodeInfoWindow.tb_pi_dataSize.Text = ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.dataSize.ToString();
                producerNodeInfoWindow.tb_pi_dpr.Text = ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.dataProducingPeriod.ToString();
                producerNodeInfoWindow.tb_pi_dpr_deviation.Text = ((NodeInfo)NodesTable.SelectedItems[0]).producerAddInfo.deviation.ToString();
            }
            else {/*
                producerNodeInfoWindow.tb_pi_dataPath.Text = "";
                producerNodeInfoWindow.tb_pi_dataSize.Text = "";
                producerNodeInfoWindow.tb_pi_dpr.Text = "";
                producerNodeInfoWindow.tb_pi_dpr_deviation.Text = "";*/

            }
            producerNodeInfoWindow.cb_pi_protocolSelection.ItemsSource = ((ObservableCollection<ProtocolInfo>)Application.Current.Properties["ProtocolsDB"]);
            producerNodeInfoWindow.cb_pi_protocolSelection.Items.Refresh();
            foreach (var obj in NodesTable.SelectedItems)
            {
                NodeInfo tmp = (NodeInfo)obj;
                producerNodeInfoWindow.selectedNodes.Add(tmp);
            }
            if (!producerNodeInfoWindow.IsActive)
            {
                producerNodeInfoWindow.Show();
            }
        }

        private void add_cluster(object sender, RoutedEventArgs e)
        {
            Clusters tmp = new Clusters();
            tmp.name = tb_clusterName.Text;
            clustersDB.Add(tmp);
        }

        private void Load_Nodes_Info_All(object sender, RoutedEventArgs e)
        {
            ObservableCollection<NodeInfo> tmpnodesDB = new ObservableCollection<NodeInfo>();
            ObservableCollection<Clusters> tmpclusterDB = new ObservableCollection<Clusters>();
            ObservableCollection<ProtocolInfo> protocolsDB = (ObservableCollection<ProtocolInfo>)Application.Current.Properties["ProtocolsDB"];
            if ((protocolsDB == null) || (protocolsDB.Count == 0)) {
                MessageBox.Show("Load corresponding protol first");
                return;
            } 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                if (nodesDB.Count != 0)
                {
                    nodesDB.Clear();
                }
                var path = openFileDialog.FileName;
                configurationInfo = JsonConvert.DeserializeObject<SaveConfiguration>(File.ReadAllText(path));
                tmpclusterDB = configurationInfo.clustersDB;
                tmpnodesDB = configurationInfo.nodesDB;
                foreach (var item in tmpnodesDB)
                {
                    NodeInfo tmpNodeInfo = new NodeInfo();
                    tmpNodeInfo = item;
                    if (tmpNodeInfo.type == "Producer")
                    {
                        NodeProducerInfo tmpNodeProducerInfo = new NodeProducerInfo();
                        if(tmpNodeInfo.producerInformationAssigned == true) { 
                            tmpNodeProducerInfo.dataPath = item.producerAddInfo.dataPath;
                            tmpNodeProducerInfo.dataProducingPeriod = item.producerAddInfo.dataProducingPeriod;
                            tmpNodeProducerInfo.dataSize = item.producerAddInfo.dataSize;
                            foreach (var item1 in protocolsDB) {
                                if (item1.protocolName == item.producerAddInfo.assignedProtocol.protocolName) {
                                    tmpNodeProducerInfo.assignedProtocol = item1;
                                    break;
                                }
                            }
                        }
                        tmpNodeInfo.producerInformationAssigned = item.producerInformationAssigned;
                        tmpNodeInfo.producerAddInfo = tmpNodeProducerInfo;
                    }
                    nodesDB.Add(tmpNodeInfo);
                    if (tmpNodeInfo.cluster == "NA") {
                        DisconnectedNodes tmpDN = new DisconnectedNodes();
                        tmpDN.Name = item.id.ToString();
                        tmpDN.Type = "Node";
                        tmpDN.node = tmpNodeInfo;
                        disconectNodes2.Add(tmpDN);
                    }
                }
                foreach (var item in tmpclusterDB)
                {
                    Clusters tmpCluster = new Clusters();
                    foreach (var item1 in nodesDB) {
                        if (item1.cluster == item.name) {
                            tmpCluster.grupedNodes.Add(item1);
                        }
                    }
                    tmpCluster.name = item.name;
                    clustersDB.Add(tmpCluster);
                    DisconnectedNodes tmpDN = new DisconnectedNodes();
                    tmpDN.Name = item.name;
                    tmpDN.Type = "Cluster";
                    tmpDN.cluster = tmpCluster;
                    disconectNodes2.Add(tmpDN);
                }
                Application.Current.Properties["NodesDB"] = nodesDB;
                Application.Current.Properties["DisconnectedNodes1"] = disconectNodes2;
                if (NodesTable.Visibility == Visibility.Hidden)
                {
                    NodesTable.Visibility = Visibility.Visible;
                }
                nodeIDCounter = nodesDB.Count+1;
            }
        }

        private void Save_Nodes_Info_All(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                configurationInfo.clustersDB = clustersDB;
                configurationInfo.nodesDB = nodesDB;
                File.WriteAllText(path, JsonConvert.SerializeObject(configurationInfo, Formatting.Indented));
                MessageBox.Show("Nodes list successfully saved");
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {

            //MessageBox.Show("Page loaded");
        }


    }
}
