using System.Windows;
using Graph_Manager.ViewModel;
using System;

namespace Graph_Manager.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            InitializeComponent();
            DataContext=new MainWindowViewModel();
        }
    }
}
