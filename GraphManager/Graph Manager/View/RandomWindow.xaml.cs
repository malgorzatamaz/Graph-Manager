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
        public RandomWindow(Graph graph,int width,int height)
        {
            InitializeComponent();
            DataContext = new RandomViewModel(graph,width,height);
        }
    }
}
