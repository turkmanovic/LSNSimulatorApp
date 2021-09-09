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
    /// Interaction logic for Links.xaml
    /// </summary>
    public partial class Links : UserControl
    {
        static int idCounter = 1;
        public ObservableCollection<LinkInfo> linksDB = new ObservableCollection<LinkInfo>();
        public Links()
        {
            InitializeComponent();
            LinksTable.ItemsSource = linksDB;
            Application.Current.Properties["LinksDB"] = linksDB;
        }

        private void Save_Link_Info(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                File.WriteAllText(path, JsonConvert.SerializeObject(linksDB, Formatting.Indented));
                MessageBox.Show("Link list successfully saved");
            }
        }

        private void Load_Link_Info(object sender, RoutedEventArgs e)
        {
            ObservableCollection<LinkInfo> tmplinkDB = new ObservableCollection<LinkInfo>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                if (linksDB.Count != 0)
                {
                    linksDB.Clear();
                }
                var path = openFileDialog.FileName;
                tmplinkDB = JsonConvert.DeserializeObject<ObservableCollection<LinkInfo>>(File.ReadAllText(path));
                foreach (var item in tmplinkDB)
                {
                    linksDB.Add(item);
                }
                if (LinksTable.Visibility == Visibility.Hidden)
                {
                    LinksTable.Visibility = Visibility.Visible;
                }

            }
        }

        private void Add_Link(object sender, RoutedEventArgs e)
        {
            LinkInfo tmpLinkInfo = new LinkInfo();
            tmpLinkInfo.linkID = idCounter++;
            tmpLinkInfo.linkName = tb_linkName.Text;
            tmpLinkInfo.linkSpeed = Double.Parse(tb_linkSpeed.Text);
            tmpLinkInfo.linkDeviation = Int32.Parse(tb_linkDeviation.Text);
            tmpLinkInfo.linkMTUSize = Int32.Parse(tb_linkMTUSize.Text);
            tmpLinkInfo.linkNumberOfHops = Int32.Parse(tb_linkNumberOfHops.Text);
            tmpLinkInfo.linkTxConsumption = Double.Parse(tb_linkTxConsumption.Text);
            tmpLinkInfo.linkRxConsumption = Double.Parse(tb_linkRxConsumption.Text);
            linksDB.Add(tmpLinkInfo);
            if (LinksTable.Visibility == Visibility.Hidden)
            {
                LinksTable.Visibility = Visibility.Visible;
            }
        }
    }
}
