using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Graph_Manager.Model
{
    public class Graph
    {
        public int GraphId { get; set; }
        public string GraphName { get; set; }
        public ObservableCollection<Vertex> Vertexes { get; set; }
        public ObservableCollection<Edge> Edges { get; set; }
        public Graph()
        {
            Vertexes = new ObservableCollection<Vertex>();
            Edges = new ObservableCollection<Edge>();
        }


    }
}
