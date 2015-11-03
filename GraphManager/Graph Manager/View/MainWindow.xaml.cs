using System.Windows;
using Graph_Manager.ViewModel;

namespace Graph_Manager.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {         
            InitializeComponent();
            DataContext=new MainWindowViewModel();
        }
    }
}
