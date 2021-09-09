using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace LSNSimulator.Helpers
{
    public class SaveConfiguration
    {

        public ObservableCollection<NodeInfo> nodesDB = new ObservableCollection<NodeInfo>();
        public ObservableCollection<Clusters> clustersDB = new ObservableCollection<Clusters>();
    }
}
