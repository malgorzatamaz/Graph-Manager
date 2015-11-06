using System.Collections.Generic;
using System.Windows;
using Graph_Manager.Model;
using Graph_Manager.ViewModel;

namespace Graph_Manager.View
{
    /// <summary>
    /// Interaction logic for RandomWindow.xaml
    /// </summary>
    public partial class RandomWindow : Window
    {
        public RandomWindow(RandomWindowViewModel randomViewModel)
        {
            InitializeComponent();
            DataContext = randomViewModel;
        }
    }
}
