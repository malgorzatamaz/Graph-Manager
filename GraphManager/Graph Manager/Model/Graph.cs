using System.Collections.ObjectModel;

namespace Graph_Manager.Model
{
    public class Graph
    {
        public Graph()
        {
            Vertexes = new ObservableCollection<Vertex>();
            Edges = new ObservableCollection<Edge>();
        }

        public ObservableCollection<Vertex> Vertexes { get; set; }
        public ObservableCollection<Edge> Edges { get; set; }
    }
}
