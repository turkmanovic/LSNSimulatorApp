using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using LSNSimulator.Helpers;

namespace LSNSimulator.Helpers
{
    public class Clusters
    {
        public ObservableCollection<NodeInfo> grupedNodes = new ObservableCollection<NodeInfo>();
        public string name { get; set; }

    }
}
