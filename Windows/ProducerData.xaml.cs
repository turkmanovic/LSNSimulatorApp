using FirstFloor.ModernUI.Windows.Controls;
using System;
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
using System.Globalization;

namespace LSNSimulator.Windows
{
    /// <summary>
    /// Interaction logic for ProducerData.xaml
    /// </summary>
    public partial class ProducerData : ModernWindow
    {
        public ObservableCollection<ProtocolInfo> availableProtocols = new ObservableCollection<ProtocolInfo>();
        public ObservableCollection<NodeInfo> selectedNodes = new ObservableCollection<NodeInfo>();
        public ProducerData()
        {
            InitializeComponent();
            cb_pi_protocolSelection.ItemsSource = availableProtocols;
        }

        private void assign(object sender, RoutedEventArgs e)
        {
            //ComboBoxItem typeItem = (ComboBoxItem)cb_pi_protocolSelection.SelectedItem;
            if (selectedNodes.Count == 0) {
                MessageBox.Show("First select nodes");
                return;
            }
            var random = new Random();
            foreach (var obj in selectedNodes)
            {
                NodeInfo tmp = (NodeInfo)obj;
                foreach (var obj1 in availableProtocols)
                {
                    string selectedProtocolName = cb_pi_protocolSelection.Text;
                    if (((ProtocolInfo)obj1).protocolName == selectedProtocolName) {
                        tmp.producerAddInfo.assignedProtocol = ((ProtocolInfo)obj1);
                        break;
                    }
                }
                double randomNumber = random.NextDouble();

                //((100 - Double.Parse(tb_processingSpeedDeviation.Text)) / 100)*Double.Parse(tb_processingSpeed.Text);
                double x = (double)decimal.Parse(tb_pi_dpr_deviation.Text);
                double minValue = ((100 - x) / 100) * Double.Parse(tb_pi_dpr.Text);
                double maxValue = ((100 + x) / 100) * Double.Parse(tb_pi_dpr.Text);
                int producingPeriod = (int)((randomNumber) * (maxValue - minValue) + minValue);
                tmp.producerAddInfo.dataProducingPeriod = producingPeriod;
                tmp.producerAddInfo.dataSize = Int32.Parse(tb_pi_dataSize.Text);
                tmp.producerAddInfo.dataPath = tb_pi_dataPath.Text.Split(',').Select(int.Parse).ToList();
                tmp.producerAddInfo.deviation = Double.Parse(tb_pi_dpr_deviation.Text);
                tmp.producerInformationAssigned = true;
            }
            selectedNodes.Clear();
            Visibility = Visibility.Hidden;
        }

        private void closed(object sender, EventArgs e)
        {

            Visibility = Visibility.Hidden;
        }

        private void closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;

        }
    }
}
