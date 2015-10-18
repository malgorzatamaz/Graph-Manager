using System;
using System.Windows;
using Graph_Manager.Model;

namespace Graph_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int ImageWidth = 30;
        private const int ImageHeight = 30;

        public MainWindow()
        {
            Graph = new Graph();
            InitializeComponent();
        }

        public string LocalPath => AppDomain.CurrentDomain.BaseDirectory;
        public Graph Graph { get; set; }
    }
}
