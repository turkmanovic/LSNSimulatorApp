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

namespace LSNSimulator.Pages
{
    /// <summary>
    /// Interaction logic for Protocol.xaml
    /// </summary>
    public partial class Protocol : UserControl
    {
        static int idCounter = 1;
        public ObservableCollection<ProtocolInfo> protocolsDB = new ObservableCollection<ProtocolInfo>();
        public Protocol()
        {
            InitializeComponent();
            ProtocolsTable.ItemsSource = protocolsDB;
            Application.Current.Properties["ProtocolsDB"] = protocolsDB;
        }

        private void Add_Protocol(object sender, RoutedEventArgs e)
        {
            ProtocolInfo tmpProtocolInfo = new ProtocolInfo();
            tmpProtocolInfo.protocolID = idCounter++;
            tmpProtocolInfo.protocolName = tb_protocolName.Text;
            tmpProtocolInfo.protocolOverhead = Int32.Parse(tb_protocolOverheadSize.Text);
            tmpProtocolInfo.protocolGenerateResponseFlag = (bool)chb_protocolGenerateResponse.IsChecked;
            protocolsDB.Add(tmpProtocolInfo);
            if (ProtocolsTable.Visibility == Visibility.Hidden)
            {
                ProtocolsTable.Visibility = Visibility.Visible;
            }

        }

        private void Save_Protocol_Info(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                File.WriteAllText(path, JsonConvert.SerializeObject(protocolsDB, Formatting.Indented));
                MessageBox.Show("Protocol list successfully saved");
            }
        }

        private void Load_Protocol_Info(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ProtocolInfo> tmplinkDB = new ObservableCollection<ProtocolInfo>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                if (protocolsDB.Count != 0)
                {
                    protocolsDB.Clear();
                }
                var path = openFileDialog.FileName;
                tmplinkDB = JsonConvert.DeserializeObject<ObservableCollection<ProtocolInfo>>(File.ReadAllText(path));
                foreach (var item in tmplinkDB)
                {
                    protocolsDB.Add(item);
                }
                if (ProtocolsTable.Visibility == Visibility.Hidden)
                {
                    ProtocolsTable.Visibility = Visibility.Visible;
                }

            }

        }
    }
}
