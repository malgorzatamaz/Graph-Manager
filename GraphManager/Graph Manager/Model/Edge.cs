using System.Windows;
using System.Windows.Input;
using Graph_Manager.ViewModel;

namespace Graph_Manager.Model
{
    public class Edge
    {
        public Point StartPoint { get; set; }
        public ICommand LineMouseLeftButtonDownCommand { get; set; }
        public Point EndPoint { get; set; }
        public int IdEdge { get; set; }
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
        public bool IsMouseLeftButtonDown { get; set; }

        public Edge()
        {
            LineMouseLeftButtonDownCommand = new RelayCommand((m)=> IsMouseLeftButtonDown = true, (n) => true);
        }
    }
}
