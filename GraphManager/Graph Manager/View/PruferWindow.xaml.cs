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
using System.Windows.Shapes;
using Graph_Manager.Model;
using Graph_Manager.ViewModel;

namespace Graph_Manager.View
{
    /// <summary>
    /// Interaction logic for PruferWindow.xaml
    /// </summary>
    public partial class PruferWindow : Window
    {
        public PruferWindow(PruferWindowViewModel pruferWindowViewModel)
        {
            InitializeComponent();
            this.DataContext = pruferWindowViewModel;
        }
    }
}
